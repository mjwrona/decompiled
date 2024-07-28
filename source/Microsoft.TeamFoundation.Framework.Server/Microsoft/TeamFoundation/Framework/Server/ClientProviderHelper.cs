// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ClientProviderHelper
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server.WebApis;
using Microsoft.TeamFoundation.Framework.Server.WebApis.CustomHandlers;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Reflection;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public static class ClientProviderHelper
  {
    private const string c_tooManyRequestHandlerFeatureName = "VisualStudio.FrameworkService.TooManyRequestsHandler";

    public static List<DelegatingHandler> GetMinimalDelegatingHandlers(
      IVssRequestContext requestContext,
      Type requestedType,
      ClientProviderHelper.Options options,
      string logAs,
      VssHttpClientOptions httpClientOptions = null,
      bool useAcceptLanguageHandler = true)
    {
      List<DelegatingHandler> delegatingHandlers = new List<DelegatingHandler>(8);
      delegatingHandlers.Add((DelegatingHandler) new VssRequestContextCaptureHandler(requestContext));
      if (useAcceptLanguageHandler)
        delegatingHandlers.Add((DelegatingHandler) new AcceptLanguageHandler());
      delegatingHandlers.Add((DelegatingHandler) new VssTracingHttpRetryMessageHandler(options.MaxRetryCount, requestedType.Name));
      delegatingHandlers.Add((DelegatingHandler) new ClientTraceHandler(requestedType, "Framework" + logAs, logAs, options.SlowRequestThreshold, options.TracePercentage, ClientProviderHelper.GetSensitiveHeadersOfClientType(requestedType)));
      if (requestContext.ExecutionEnvironment.IsHostedDeployment)
      {
        delegatingHandlers.Add((DelegatingHandler) ThrottlingMessageHandler.Create(requestContext, requestedType));
        if (httpClientOptions != null && httpClientOptions.ReadConsistencyLevel.HasValue)
          delegatingHandlers.Add((DelegatingHandler) new ReadConsistencyLevelHandler(httpClientOptions.ReadConsistencyLevel));
      }
      return delegatingHandlers;
    }

    public static List<DelegatingHandler> GetDelegatingHandlers(
      IVssRequestContext requestContext,
      Type requestedType,
      Uri baseUri,
      ClientProviderHelper.Options options,
      string logAs,
      IEnumerable<DelegatingHandler> customDelegatingHandlers = null,
      VssHttpClientOptions httpClientOptions = null)
    {
      List<DelegatingHandler> delegatingHandlers = new List<DelegatingHandler>();
      delegatingHandlers.Add((DelegatingHandler) new VssRequestContextCaptureHandler(requestContext));
      if (customDelegatingHandlers != null && customDelegatingHandlers.Any<DelegatingHandler>())
        delegatingHandlers.AddRange(customDelegatingHandlers);
      if (requestContext.ExecutionEnvironment.IsHostedDeployment && requestContext.IsFeatureEnabled(FrameworkServerConstants.HandlerConfigureAwaitFeatureFlag) && requestContext.IsFeatureEnabled("VisualStudio.FrameworkService.TooManyRequestsHandler"))
        delegatingHandlers.Add((DelegatingHandler) new RetryAfterHandler(requestContext, requestedType.Name, baseUri.Host));
      delegatingHandlers.Add((DelegatingHandler) new TfsImpersonationMessageHandler());
      delegatingHandlers.Add((DelegatingHandler) new TfsSubjectDescriptorImpersonationMessageHandler(requestContext));
      delegatingHandlers.Add((DelegatingHandler) new AcceptLanguageHandler());
      delegatingHandlers.Add((DelegatingHandler) new AfdClientIpHandler(requestContext));
      delegatingHandlers.Add((DelegatingHandler) new VssTracingHttpRetryMessageHandler(options.MaxRetryCount, requestedType.Name));
      delegatingHandlers.Add((DelegatingHandler) new ClientTraceHandler(requestedType, "Framework" + logAs, logAs, options.SlowRequestThreshold, options.TracePercentage, ClientProviderHelper.GetSensitiveHeadersOfClientType(requestedType)));
      TeamFoundationExecutionEnvironment executionEnvironment = requestContext.ExecutionEnvironment;
      if (executionEnvironment.IsHostedDeployment)
      {
        delegatingHandlers.Add((DelegatingHandler) S2SUnauthorizedHandler.Create(requestContext));
        delegatingHandlers.Add((DelegatingHandler) new VssPriorityHandler());
        delegatingHandlers.Add((DelegatingHandler) ThrottlingMessageHandler.Create(requestContext, requestedType));
        delegatingHandlers.Add((DelegatingHandler) new FaultInjectionHandler(requestContext, baseUri.Host));
        delegatingHandlers.Add((DelegatingHandler) new ClientAccessMappingHandler(requestContext));
        if (httpClientOptions != null && httpClientOptions.ReadConsistencyLevel.HasValue)
          delegatingHandlers.Add((DelegatingHandler) new ReadConsistencyLevelHandler(httpClientOptions.ReadConsistencyLevel));
      }
      executionEnvironment = requestContext.ExecutionEnvironment;
      if (executionEnvironment.IsOnPremisesDeployment && !requestContext.RootContext.Items.ContainsKey(RequestContextItemsKeys.BypassLoopbackHandler))
        delegatingHandlers.Add((DelegatingHandler) new LoopbackHandler());
      delegatingHandlers.Add((DelegatingHandler) new ConnectionLockHandler());
      return delegatingHandlers;
    }

    private static HashSet<string> GetSensitiveHeadersOfClientType(Type clientType)
    {
      object[] customAttributes = clientType.GetCustomAttributes(typeof (ClientSensitiveHeaderAttribute), true);
      return customAttributes == null ? (HashSet<string>) null : customAttributes.Cast<ClientSensitiveHeaderAttribute>().Select<ClientSensitiveHeaderAttribute, string>((Func<ClientSensitiveHeaderAttribute, string>) (csha => csha.HeaderName)).ToHashSet<string>();
    }

    public static IEnumerable<DelegatingHandler> GetCustomHandlersFromType(
      IVssRequestContext requestContext,
      Type type,
      string area,
      string layer)
    {
      if (type == (Type) null)
        return Enumerable.Empty<DelegatingHandler>();
      List<DelegatingHandler> handlersFromType = new List<DelegatingHandler>();
      try
      {
        ReactiveClientToThrottlingAttribute customAttribute1 = type.GetCustomAttribute<ReactiveClientToThrottlingAttribute>(true);
        if ((customAttribute1 != null ? (customAttribute1.ReactToThrottlingHeaders ? 1 : 0) : 0) != 0)
          handlersFromType.Add((DelegatingHandler) new ClientRateLimiterHandler(type));
        foreach (AddCustomHandlerBaseAttribute customAttribute2 in type.GetCustomAttributes<AddCustomHandlerBaseAttribute>(true))
          handlersFromType.Add(customAttribute2.CreateHandler(requestContext));
        return (IEnumerable<DelegatingHandler>) handlersFromType;
      }
      catch (Exception ex)
      {
        requestContext.Trace(34004000, TraceLevel.Error, area, layer, string.Format("Custom Delegating Handlers could not be retrieved from {0} ({1}). Ex: {2}", (object) type, (object) "IVssHttpClient", (object) ex));
        return Enumerable.Empty<DelegatingHandler>();
      }
    }

    public class Options
    {
      public Options(int maxRetryCount, TimeSpan slowRequestThreshold, byte tracePercentage)
      {
        this.MaxRetryCount = maxRetryCount;
        this.SlowRequestThreshold = slowRequestThreshold;
        this.TracePercentage = tracePercentage;
      }

      public static ClientProviderHelper.Options CreateDefault(IVssRequestContext requestContext)
      {
        IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment).Elevate();
        IVssRegistryService service = vssRequestContext.GetService<IVssRegistryService>();
        return new ClientProviderHelper.Options(service.GetValue<int>(vssRequestContext, (RegistryQuery) "/Service/HttpResourceManagementService/MaxRetryHttpRequest", 5), service.GetValue<TimeSpan>(vssRequestContext, (RegistryQuery) "/Service/HttpResourceManagementService/SlowRequestThreshold", TimeSpan.FromSeconds(5.0)), service.GetValue<byte>(vssRequestContext, (RegistryQuery) "/Service/HttpResourceManagementService/TracePercentage", (byte) 100));
      }

      public int MaxRetryCount { get; }

      public TimeSpan SlowRequestThreshold { get; }

      public byte TracePercentage { get; }
    }
  }
}
