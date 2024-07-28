// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.ExternalProviders.Common.GitHubHttpClientFactory
// Assembly: Microsoft.VisualStudio.ExternalProviders.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7E34B318-B0E9-49BD-88C0-4A425E8D0753
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.ExternalProviders.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.ExternalProviders.GitHub.WebApi;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading;

namespace Microsoft.VisualStudio.ExternalProviders.Common
{
  public static class GitHubHttpClientFactory
  {
    private const string c_area = "ExternalProviders";
    private const string c_layer = "GitHubHttpClientFactory";

    public static GitHubHttpClient Create(
      IVssRequestContext requestContext,
      TimeSpan? timeout = null,
      GitHubRateLimitTraceOptions rateLimitTraceOptions = null,
      GitHubAppType appType = GitHubAppType.Pipelines,
      string projectId = null,
      string endpointId = null)
    {
      IGitHubAppAccessTokenProvider accessTokenProvider = GitHubHttpClientFactory.GetAppAccessTokenProvider(requestContext, appType, projectId, endpointId);
      if (accessTokenProvider == null)
        requestContext.Trace(ExternalProvidersTracePoints.LoadAppExtensionFailed, TraceLevel.Warning, "ExternalProviders", nameof (GitHubHttpClientFactory), "Unable to load the {0} extension.", (object) "IGitHubAppAccessTokenProvider");
      return GitHubHttpClientFactory.Create(requestContext, accessTokenProvider, timeout, rateLimitTraceOptions);
    }

    public static GitHubHttpClient Create(
      IVssRequestContext requestContext,
      IGitHubAppAccessTokenProvider tokenProvider,
      TimeSpan? timeout = null,
      GitHubRateLimitTraceOptions rateLimitTraceOptions = null,
      CancellationToken? cancellationToken = null)
    {
      int maxResults = requestContext.GetService<IVssRegistryService>().GetValue<int>(requestContext, (RegistryQuery) GitHubHttpClient.s_registryMaxPageResultsOverride, 2000);
      if (!cancellationToken.HasValue && requestContext.GetType() == typeof (JobRequestContext))
        cancellationToken = new CancellationToken?(requestContext.CancellationToken);
      IExternalProviderHttpRequesterFactory gitHubHttpRequesterFactory = (IExternalProviderHttpRequesterFactory) null;
      try
      {
        using (IDisposableReadOnlyList<IExternalProviderHttpRequesterFactory> extensions = requestContext.GetExtensions<IExternalProviderHttpRequesterFactory>())
        {
          if (extensions != null)
          {
            if (extensions.Count > 0)
            {
              if (requestContext.ServiceHost.IsProduction)
                requestContext.TraceAlways(ExternalProvidersTracePoints.HttpRequesterMocked, TraceLevel.Info, "ExternalProviders", nameof (GitHubHttpClientFactory), "Mock GitHub Requester used instead of real requester. This should NOT happen in production!");
              gitHubHttpRequesterFactory = extensions.FirstOrDefault<IExternalProviderHttpRequesterFactory>((Func<IExternalProviderHttpRequesterFactory, bool>) (r => string.Equals(r.ProviderType, GitHubHttpClientFactory.GitHubHttpRequesterFactory.GitHubProviderType)));
            }
          }
        }
      }
      catch (Exception ex)
      {
        requestContext.TraceException(ExternalProvidersTracePoints.HttpRequesterFactoryLoadFailed, "ExternalProviders", nameof (GitHubHttpClientFactory), ex);
      }
      if (gitHubHttpRequesterFactory == null)
        gitHubHttpRequesterFactory = (IExternalProviderHttpRequesterFactory) new GitHubHttpClientFactory.GitHubHttpRequesterFactory();
      gitHubHttpRequesterFactory.Initialize((object) requestContext);
      GitHubHttpClientFactory.InstallationAccessTokenCacheService service = requestContext.To(TeamFoundationHostType.Deployment).GetService<GitHubHttpClientFactory.InstallationAccessTokenCacheService>();
      return new GitHubHttpClient(gitHubHttpRequesterFactory, (IGitHubRateLimitTracer) new GitHubHttpClientFactory.GitHubRateLimitTracer(requestContext, rateLimitTraceOptions), (IGitHubConditionalResponseTracer) new GitHubHttpClientFactory.GitHubConditionalResponseTracer(requestContext), tokenProvider, maxResults, timeout, (IGitHubInstallationAccessTokenCache) service, cancellationToken);
    }

    public static IGitHubAppAccessTokenProvider GetAppAccessTokenProvider(
      IVssRequestContext requestContext,
      GitHubAppType appType,
      string projectId = null,
      string endpointId = null)
    {
      GitHubHttpClientFactory.DeferredGitHubAppAccessTokenProvider accessTokenProvider = new GitHubHttpClientFactory.DeferredGitHubAppAccessTokenProvider(appType);
      accessTokenProvider.Initialize((object) requestContext, projectId, endpointId);
      return (IGitHubAppAccessTokenProvider) accessTokenProvider;
    }

    private class DeferredGitHubAppAccessTokenProvider : IGitHubAppAccessTokenProvider, IDisposable
    {
      private IVssRequestContext m_requestContext;
      private string m_projectId;
      private string m_endpointId;

      public DeferredGitHubAppAccessTokenProvider(GitHubAppType appType) => this.AppType = appType;

      public GitHubAppType AppType { get; }

      public string CreateEncodedAppAccessToken()
      {
        using (IDisposableReadOnlyList<IGitHubAppAccessTokenProvider> extensions = this.m_requestContext.GetExtensions<IGitHubAppAccessTokenProvider>())
        {
          IGitHubAppAccessTokenProvider accessTokenProvider = extensions.FirstOrDefault<IGitHubAppAccessTokenProvider>((Func<IGitHubAppAccessTokenProvider, bool>) (provider => provider.AppType == this.AppType));
          if (accessTokenProvider != null)
          {
            accessTokenProvider.Initialize((object) this.m_requestContext, this.m_projectId, this.m_endpointId);
            return accessTokenProvider.CreateEncodedAppAccessToken();
          }
          this.m_requestContext.Trace(ExternalProvidersTracePoints.LoadAppExtensionFailed, TraceLevel.Warning, "ExternalProviders", nameof (GitHubHttpClientFactory), string.Format("Unable to load {0} extension with type {1}.", (object) "IGitHubAppAccessTokenProvider", (object) this.AppType));
          return (string) null;
        }
      }

      public void Dispose()
      {
      }

      public void Initialize(object requestContext) => this.m_requestContext = (IVssRequestContext) requestContext;

      public void Initialize(object requestContext, string projectId, string endpointId)
      {
        this.m_requestContext = (IVssRequestContext) requestContext;
        this.m_projectId = projectId;
        this.m_endpointId = endpointId;
      }
    }

    internal class GitHubHttpRequesterFactory : IExternalProviderHttpRequesterFactory
    {
      internal static readonly string GitHubProviderType = "github";
      private IVssRequestContext m_requestContext;

      public string ProviderType => GitHubHttpClientFactory.GitHubHttpRequesterFactory.GitHubProviderType;

      public void Initialize(object requestContext) => this.m_requestContext = requestContext as IVssRequestContext;

      public IExternalProviderHttpRequester GetRequester(HttpMessageHandler httpMessageHandler)
      {
        if (this.m_requestContext != null)
        {
          List<DelegatingHandler> delegatingHandlers = ClientProviderHelper.GetMinimalDelegatingHandlers(this.m_requestContext, typeof (GitHubVssHttpClientRequester), new ClientProviderHelper.Options(3, TimeSpan.FromSeconds(5.0), (byte) 100), "GitHub");
          return (IExternalProviderHttpRequester) new GitHubVssHttpClientRequester(HttpClientFactory.CreatePipeline(httpMessageHandler, (IEnumerable<DelegatingHandler>) delegatingHandlers), this.m_requestContext.IsFeatureEnabled(GitHubFeatureFlags.DistributedTask_GitHubStrictPayloadValidation));
        }
        TeamFoundationTracingService.TraceRaw(ExternalProvidersTracePoints.HttpRequesterFactoryUninitialized, TraceLevel.Error, "ExternalProviders", nameof (GitHubHttpClientFactory), "GitHubVssHttpClientRequester created without a request context!");
        return (IExternalProviderHttpRequester) new GitHubVssHttpClientRequester(httpMessageHandler, false);
      }
    }

    internal class GitHubRateLimitTracer : IGitHubRateLimitTracer
    {
      private IVssRequestContext m_requestContext;
      private GitHubRateLimitTraceOptions m_traceOptions;
      private const string c_layer = "GitHubRateLimitTracer";
      private static readonly GitHubRateLimitTraceOptions s_defaultTraceOptions = new GitHubRateLimitTraceOptions(10, 3, false);

      public GitHubRateLimitTracer(
        IVssRequestContext requestContext,
        GitHubRateLimitTraceOptions rateLimitTraceOptions = null)
      {
        ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
        this.m_requestContext = requestContext;
        this.m_traceOptions = rateLimitTraceOptions ?? GitHubHttpClientFactory.GitHubRateLimitTracer.s_defaultTraceOptions;
      }

      public void Trace(
        IGitHubRateLimit rateLimit,
        GitHubAuthentication authentication,
        string entryMethodName,
        HttpMethod httpMethod,
        Uri requestUri)
      {
        ArgumentUtility.CheckForNull<IGitHubRateLimit>(rateLimit, nameof (rateLimit));
        int remainingPercentage = 100 * rateLimit.Remaining / rateLimit.Limit;
        int minutesRemaining = (int) (rateLimit.ResetAt - DateTime.UtcNow).TotalMinutes;
        this.m_requestContext.TraceConditionally(ExternalProvidersTracePoints.GraphQlRateLimitDetails, TraceLevel.Info, "ExternalProviders", nameof (GitHubRateLimitTracer), (Func<string>) (() => "Rate limit details: " + this.GenerateRateLimitDetailsMessage(rateLimit, remainingPercentage, minutesRemaining, authentication, entryMethodName, httpMethod, requestUri)));
        if (rateLimit.Cost >= this.m_traceOptions.GraphQLcostWarningThreshold)
          this.m_requestContext.TraceAlways(ExternalProvidersTracePoints.GraphQlExpensive, TraceLevel.Warning, "ExternalProviders", nameof (GitHubRateLimitTracer), string.Format("GraphQL call is expensive ({0} points). {1}", (object) rateLimit.Cost, (object) this.GenerateRateLimitDetailsMessage(rateLimit, remainingPercentage, minutesRemaining, authentication, entryMethodName, httpMethod, requestUri)));
        if (remainingPercentage <= this.m_traceOptions.WarningRemainingPercentage)
          this.m_requestContext.TraceAlways(ExternalProvidersTracePoints.GraphQlRateLimitWarning, TraceLevel.Warning, "ExternalProviders", nameof (GitHubRateLimitTracer), string.Format("Close to hitting the rate limit ({0}% remaining). {1}", (object) remainingPercentage, (object) this.GenerateRateLimitDetailsMessage(rateLimit, remainingPercentage, minutesRemaining, authentication, entryMethodName, httpMethod, requestUri)));
        if (this.m_traceOptions.LogPerUserUsage)
          this.m_requestContext.TraceAlways(ExternalProvidersTracePoints.GraphQlRateLimitPerUser, TraceLevel.Info, "ExternalProviders", nameof (GitHubRateLimitTracer), string.Format("User with VSID {0} cost {1} points. {2}", (object) this.m_requestContext.GetUserId(), (object) rateLimit.Cost, (object) this.GenerateRateLimitDetailsMessage(rateLimit, remainingPercentage, minutesRemaining, authentication, entryMethodName, httpMethod, requestUri)));
        else
          this.m_requestContext.Trace(ExternalProvidersTracePoints.GraphQlRateLimitPerUser, TraceLevel.Info, "ExternalProviders", nameof (GitHubRateLimitTracer), string.Format("User with VSID {0} cost {1} points. {2}", (object) this.m_requestContext.GetUserId(), (object) rateLimit.Cost, (object) this.GenerateRateLimitDetailsMessage(rateLimit, remainingPercentage, minutesRemaining, authentication, entryMethodName, httpMethod, requestUri)));
      }

      private string GenerateRateLimitDetailsMessage(
        IGitHubRateLimit rateLimit,
        int remainingPercentage,
        int minutesRemaining,
        GitHubAuthentication authentication,
        string entryMethodName,
        HttpMethod httpMethod,
        Uri requestUri)
      {
        return string.Format("Rate limit details: Cost: {0}, NodeCount: {1}, Limit: {2}, Remaining: {3} ({4}%), ResetAt: {5} ({6} minutes). Authentication: {7} Method: {8}. Request: {9} {10}", (object) rateLimit.Cost, (object) rateLimit.NodeCount, (object) rateLimit.Limit, (object) rateLimit.Remaining, (object) remainingPercentage, (object) rateLimit.ResetAt, (object) minutesRemaining, (object) authentication?.ToTelemetryString(), (object) entryMethodName, (object) httpMethod, (object) requestUri);
      }
    }

    internal class GitHubConditionalResponseTracer : IGitHubConditionalResponseTracer
    {
      private IVssRequestContext m_requestContext;
      private const string c_layer = "GitHubConditionalResponseTracer";

      public GitHubConditionalResponseTracer(IVssRequestContext requestContext)
      {
        ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
        this.m_requestContext = requestContext;
      }

      public void Trace<T>(
        GitHubResult<T> gitHubResult,
        GitHubAuthentication authentication,
        string entryMethodName,
        HttpMethod httpMethod,
        Uri requestUri,
        string[] requestTags)
      {
        ArgumentUtility.CheckForNull<GitHubResult<T>>(gitHubResult, nameof (gitHubResult));
        this.m_requestContext.Trace(ExternalProvidersTracePoints.GitHubConditionalRequestDetails, TraceLevel.Verbose, "ExternalProviders", nameof (GitHubConditionalResponseTracer), "Github conditional request details. Authentication: " + authentication?.ToTelemetryString() + " , Token: " + authentication?.InstallationAccessToken?.Token?.Substring(authentication.InstallationAccessToken.Token.Length - 10) + " " + string.Format("Method: {0}. Request: {1} {2} , Used Tags: {3} , Returned Tag: {4}", (object) entryMethodName, (object) httpMethod, (object) requestUri, (object) requestTags, (object) gitHubResult.ETagValue));
        if (gitHubResult.IsUnchangedConditionalResult)
          this.m_requestContext.TraceAlways(ExternalProvidersTracePoints.GitHubConditionalRequestUnchangedDetails, TraceLevel.Info, "ExternalProviders", nameof (GitHubConditionalResponseTracer), string.Format("GitHubAPI request returned Unchanged tagged response and was not counted against rate limits. Authentication: {0} Method: {1}. Request: {2} {3}", (object) authentication?.ToTelemetryString(), (object) entryMethodName, (object) httpMethod, (object) requestUri));
        else if (gitHubResult.IsSuccessful)
          this.m_requestContext.TraceAlways(ExternalProvidersTracePoints.GitHubConditionalRequestUpdatedDetails, TraceLevel.Info, "ExternalProviders", nameof (GitHubConditionalResponseTracer), string.Format("GitHubAPI request returned New response and was counted against rate limits. Authentication: {0} Method: {1}. Request: {2} {3}", (object) authentication?.ToTelemetryString(), (object) entryMethodName, (object) httpMethod, (object) requestUri));
        else
          this.m_requestContext.TraceAlways(ExternalProvidersTracePoints.GitHubConditionalRequestFailedDetails, TraceLevel.Info, "ExternalProviders", nameof (GitHubConditionalResponseTracer), string.Format("GitHubAPI request was Unsuccessful with error message: {0} Authentication: {1} Method: {2}. Request: {3} {4}", (object) gitHubResult.ErrorMessage, (object) authentication?.ToTelemetryString(), (object) entryMethodName, (object) httpMethod, (object) requestUri));
      }
    }

    public sealed class InstallationAccessTokenCacheService : 
      VssMemoryCacheService<string, GitHubData.InstallationAccessToken>,
      IGitHubInstallationAccessTokenCache
    {
      public InstallationAccessTokenCacheService()
        : base(TimeSpan.FromHours(1.0))
      {
        this.MaxCacheLength.Value = 10000;
        this.ExpiryInterval.Value = TimeSpan.FromHours(1.0);
      }

      public bool TryGetToken(
        GitHubAppType type,
        string key,
        out GitHubData.InstallationAccessToken token)
      {
        this.CheckCollision(type, key, ExternalProvidersTracePoints.TokenCacheAccessed);
        return this.MemoryCache.TryGetValue(type.ToString() + key, out token);
      }

      public void AddOrUpdate(
        GitHubAppType type,
        string key,
        GitHubData.InstallationAccessToken token)
      {
        this.CheckCollision(type, key, ExternalProvidersTracePoints.TokenCacheAdded);
        this.MemoryCache.Add(type.ToString() + key, token, true);
      }

      public void Remove(GitHubAppType type, string key)
      {
        this.CheckCollision(type, key, ExternalProvidersTracePoints.TokenCacheRemoved);
        bool flag = this.MemoryCache.Remove(type.ToString() + key);
        this.TraceRaw(ExternalProvidersTracePoints.TokenCacheRemoved, TraceLevel.Info, "InstallationAccessToken removed key '" + key + "' " + (flag ? "was in cache" : "was not found"));
      }

      private void CheckCollision(GitHubAppType type, string key, int tracepoint)
      {
        foreach (GitHubAppType gitHubAppType in (GitHubAppType[]) Enum.GetValues(typeof (GitHubAppType)))
        {
          GitHubData.InstallationAccessToken installationAccessToken;
          if (gitHubAppType != type && this.MemoryCache.TryGetValue(gitHubAppType.ToString() + key, out installationAccessToken))
            this.TraceRaw(tracepoint, TraceLevel.Error, string.Format("Collision found for key '{0}' types '{1}', '{2}' expires at '{3}'", (object) key, (object) type, (object) gitHubAppType, (object) installationAccessToken?.Expires_at));
        }
      }

      private void TraceRaw(int tracePoint, TraceLevel level, string message) => TeamFoundationTracingService.TraceRaw(tracePoint, level, "ExternalProviders", nameof (GitHubHttpClientFactory), message);
    }
  }
}
