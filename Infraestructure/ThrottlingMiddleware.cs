using Compartimoss.Example.Throotling.Model;
using Compartimoss.Example.Throttling.Model;
using Compartimoss.Example.Throttling.Services;
using Compartimoss.Example.Throttling.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace  Compartimoss.Example.Throttling
{
    internal class ThrottlingMiddleware : IMiddleware
    {
        private readonly ThrottlingOptions _options;
        private readonly IThrottlingService _processor;
        private readonly IRequestIdentityService _requestIdentityService;
        private readonly ILogger<ThrottlingMiddleware> _logger;

        public ThrottlingMiddleware(IOptions<ThrottlingOptions> options, IThrottlingService processor, IRequestIdentityService requestIdentityService, ILogger<ThrottlingMiddleware> logger)
        {
            _options = options.Value;
            _processor = processor;
            _requestIdentityService = requestIdentityService;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            // check if rate limiting is enabled
            if (_options == null)
            {
                await next.Invoke(context);
                return;
            }

            // check if throotling is enabled
            if (!_options.Enabled)
            {
                await next.Invoke(context);
                return;
            }

            // compute identity from request
            var identity = _requestIdentityService.Resolve();

            // check white list
            if (await _processor.IsWhitelistedAsync(identity))
            {
                await next.Invoke(context);
                return;
            }

            var rules = await _processor.GetMatchingRulesAsync(identity, context.RequestAborted);
            var rulesDict = new Dictionary<Rule, Counter>();
            foreach (var rule in rules)
            {
                // increment counter
                var rateLimitCounter = await _processor.ProcessRequestAsync(identity, rule, context.RequestAborted);
                if (rule.Limit > 0)
                {
                    // check if key expired
                    if (rateLimitCounter.Timestamp + rule.PeriodTimespan.Value < DateTime.UtcNow)
                    {
                        continue;
                    }

                    // check if limit is reached
                    if (rateLimitCounter.Count > rule.Limit)
                    {
                        //compute retry after value
                        var retryAfter = rateLimitCounter.Timestamp.RetryAfterFrom(rule);

                        // log blocked request
                        LogBlockedRequest(context, identity, rateLimitCounter, rule, _options.Mode);
                        if (_options.Mode == ThrottlingMode.Restricted)
                        {
                            if (_options.RequestBlockedBehaviorAsync != null)
                            {
                                await _options.RequestBlockedBehaviorAsync(context, identity, rateLimitCounter, rule);
                            }

                            // break execution
                            await ReturnQuotaExceededResponse(context, rule, retryAfter);
                            return;
                        }
                    }
                }
                // if limit is zero or less, block the request.
                else
                {
                    // log blocked request
                    LogBlockedRequest(context, identity, rateLimitCounter, rule, _options.Mode);
                    if (_options.Mode == ThrottlingMode.Restricted)
                    {
                        if (_options.RequestBlockedBehaviorAsync != null)
                        {
                            await _options.RequestBlockedBehaviorAsync(context, identity, rateLimitCounter, rule);
                        }

                        // break execution (Int32 max used to represent infinity)
                        await ReturnQuotaExceededResponse(context, rule, int.MaxValue.ToString(System.Globalization.CultureInfo.InvariantCulture));

                        return;
                    }
                }

                rulesDict.Add(rule, rateLimitCounter);
            }

            // set X-Rate-Limit headers for the longest period
            if (rulesDict.Any() && !_options.DisableRateLimitHeaders)
            {
                var rule = rulesDict.OrderByDescending(x => x.Key.PeriodTimespan).FirstOrDefault();
                var headers = _processor.GetRateLimitHeaders(rule.Value, rule.Key, context.RequestAborted);
                headers.Context = context;
                context.Response.OnStarting(SetRateLimitHeaders, state: headers);
            }

            await next.Invoke(context);
        }

        private Task ReturnQuotaExceededResponse(HttpContext httpContext, Rule rule, string retryAfter)
        {
            var message = string.Format(
                _options.QuotaExceededResponse?.Content ??
                _options.QuotaExceededMessage ??
                "API calls quota exceeded! maximum admitted {0} per {1}.", rule.Limit, rule.Period, retryAfter);

            if (!_options.DisableRateLimitHeaders)
            {
                httpContext.Response.Headers["Retry-After"] = retryAfter;
            }

            httpContext.Response.StatusCode = _options.QuotaExceededResponse?.StatusCode ?? _options.HttpStatusCode;
            httpContext.Response.ContentType = _options.QuotaExceededResponse?.ContentType ?? "text/plain";

            return httpContext.Response.WriteAsync(message);
        }

        private Task SetRateLimitHeaders(object rateLimitHeaders)
        {
            var headers = (RateLimitHeaders)rateLimitHeaders;

            headers.Context.Response.Headers["X-Rate-Limit-Limit"] = headers.Limit;
            headers.Context.Response.Headers["X-Rate-Limit-Remaining"] = headers.Remaining;
            headers.Context.Response.Headers["X-Rate-Limit-Reset"] = headers.Reset;

            return Task.CompletedTask;
        }

        private void LogBlockedRequest(HttpContext httpContext, RequestIdentity identity, Counter counter, Rule rule, ThrottlingMode mode)
        {
            var message = (mode == ThrottlingMode.Restricted)
                ? $"Request {identity.HttpMethod}:{identity.Path} from IP {identity.Ip} has been blocked, quota {rule.Limit}/{rule.Period} exceeded by {counter.Count - rule.Limit}. Blocked by rule {rule.Endpoint}, TraceIdentifier {httpContext.TraceIdentifier}."
                : $"[Skip] Request {identity.HttpMethod}:{identity.Path} from IP {identity.Ip} shold be blocked but i'm in observe mode, quota {rule.Limit}/{rule.Period} exceeded by {counter.Count - rule.Limit}. Blocked by rule {rule.Endpoint}, TraceIdentifier {httpContext.TraceIdentifier}.";
            _logger.LogWarning(message);
        }
    }
}
