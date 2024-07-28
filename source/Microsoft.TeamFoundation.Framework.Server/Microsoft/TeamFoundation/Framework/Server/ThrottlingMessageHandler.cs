// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ThrottlingMessageHandler
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.CircuitBreaker;
using Microsoft.VisualStudio.Services.Common.Internal;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class ThrottlingMessageHandler : DelegatingHandler
  {
    private readonly string m_traceArea = "Framework";
    public readonly string m_traceLayer = nameof (ThrottlingMessageHandler);
    public const string TimeoutKey = "VssRequestTimeout";

    internal ICommandFactory CircuitBreakerCommandFactory { get; }

    internal IUrlDeploymentCorrelationService UrlCorrelationService { get; }

    internal TimeSpan? RequestCancellationTimeout { get; }

    internal string ClientType { get; }

    protected ThrottlingMessageHandler(
      ICommandFactory commandFactory,
      IUrlDeploymentCorrelationService correlationService,
      TimeSpan? timeout,
      string clientType)
    {
      this.CircuitBreakerCommandFactory = commandFactory;
      this.UrlCorrelationService = correlationService;
      this.RequestCancellationTimeout = timeout;
      this.ClientType = clientType;
    }

    internal static ThrottlingMessageHandler Create(
      IVssRequestContext requestContext,
      Type requestedType)
    {
      TimeSpan? defaultValue1 = new TimeSpan?();
      IUrlDeploymentCorrelationService service = requestContext.GetService<IUrlDeploymentCorrelationService>();
      TimeSpan defaultValue2 = TimeSpan.FromSeconds(100.0);
      int defaultValue3 = 80;
      int defaultValue4 = int.MaxValue;
      string implementationName = ThrottlingMessageHandler.GetImplementationName(requestedType);
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      RegistryEntryCollection registryEntryCollection = vssRequestContext.GetService<IVssRegistryService>().ReadEntries(vssRequestContext, new RegistryQuery("/Configuration/HttpClientThrottler/" + implementationName, "", 1));
      ClientCircuitBreakerSettingsAttribute[] customAttributes1 = (ClientCircuitBreakerSettingsAttribute[]) requestedType.GetCustomAttributes(typeof (ClientCircuitBreakerSettingsAttribute), true);
      if (customAttributes1.Length != 0)
      {
        defaultValue2 = customAttributes1[0].Timeout;
        defaultValue3 = customAttributes1[0].ErrorPercentage;
        if (customAttributes1[0].MaxConcurrentRequests != 0)
          defaultValue4 = customAttributes1[0].MaxConcurrentRequests;
      }
      ClientCancellationTimeoutAttribute[] customAttributes2 = (ClientCancellationTimeoutAttribute[]) requestedType.GetCustomAttributes(typeof (ClientCancellationTimeoutAttribute), true);
      if (customAttributes2.Length != 0)
        defaultValue1 = new TimeSpan?(customAttributes2[0].Timeout);
      TimeSpan valueFromPath1 = registryEntryCollection.GetValueFromPath<TimeSpan>("CircuitBreakerTimeout", defaultValue2);
      int valueFromPath2 = registryEntryCollection.GetValueFromPath<int>("CircuitBreakerErrorPercentage", defaultValue3);
      int valueFromPath3 = registryEntryCollection.GetValueFromPath<int>("CircuitBreakerMaxConcurrentRequests", defaultValue4);
      TimeSpan? timeout = registryEntryCollection.GetValueFromPath<TimeSpan?>("ClientCancellationTimeout", defaultValue1);
      if (requestContext.IsAnonymousPrincipal() || requestContext.IsPublicUser())
      {
        TimeSpan valueFromPath4 = registryEntryCollection.GetValueFromPath<TimeSpan>("AnonymousRequestTimeout", TimeSpan.FromSeconds(30.0));
        if (timeout.HasValue)
        {
          TimeSpan timeSpan = valueFromPath4;
          TimeSpan? nullable = timeout;
          if ((nullable.HasValue ? (timeSpan < nullable.GetValueOrDefault() ? 1 : 0) : 0) == 0)
            goto label_9;
        }
        timeout = new TimeSpan?(valueFromPath4);
      }
label_9:
      CommandPropertiesSetter defaultProperties = new CommandPropertiesSetter().WithCircuitBreakerErrorThresholdPercentage(valueFromPath2).WithExecutionTimeout(valueFromPath1).WithExecutionMaxRequests(int.MaxValue).WithExecutionMaxConcurrentRequests(valueFromPath3);
      return new ThrottlingMessageHandler((ICommandFactory) new CommandServiceFactory(requestContext, (CommandKey) ("HttpClientThrottler-" + implementationName), defaultProperties), service, timeout, implementationName);
    }

    private static string GetImplementationName(Type requestedType)
    {
      if (requestedType == (Type) null)
        return (string) null;
      VssClientServiceImplementationAttribute[] customAttributes = (VssClientServiceImplementationAttribute[]) requestedType.GetCustomAttributes(typeof (VssClientServiceImplementationAttribute), true);
      return customAttributes.Length != 0 ? customAttributes[0].TypeName : requestedType.Name;
    }

    protected override Task<HttpResponseMessage> SendAsync(
      HttpRequestMessage request,
      CancellationToken cancellationToken)
    {
      return this.ExecuteInCircuitBreaker(request, cancellationToken);
    }

    private async Task<HttpResponseMessage> ExecuteInCircuitBreaker(
      HttpRequestMessage request,
      CancellationToken cancellationToken)
    {
      HttpResponseMessage executionResult = (HttpResponseMessage) null;
      bool isConfigureAwaitFeatureEnabled = false;
      request.Properties.TryGetValue<bool>(FrameworkServerConstants.HandlerConfigureAwaitFeatureFlag, out isConfigureAwaitFeatureEnabled);
      TimeSpan? timeout = this.RequestCancellationTimeout;
      object obj;
      if (request.Properties.TryGetValue("VssRequestTimeout", out obj))
        timeout = obj as TimeSpan? ?? this.RequestCancellationTimeout;
      CorrelationKey correlationKey = this.UrlCorrelationService.GetCorrelationKey(request.RequestUri);
      string correlation = this.UrlCorrelationService.GetCorrelation(correlationKey);
      using (CancellationTokenSource linkedCancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken))
      {
        try
        {
          if (timeout.HasValue)
            linkedCancellationTokenSource.CancelAfter(timeout.Value);
          string str = correlation ?? request.RequestUri.Host;
          await this.CircuitBreakerCommandFactory.CreateCommandAsync(new CommandSetter((CommandGroupKey) "Framework.").AndCommandKey((CommandKey) (this.CircuitBreakerCommandFactory.CommandKey?.ToString() + "-" + str)), (Func<Task>) (async () =>
          {
            executionResult = await this.BaseSendAsync(request, linkedCancellationTokenSource.Token).ConfigureAwait(isConfigureAwaitFeatureEnabled);
            this.AnalyzeResponseAndThrow(executionResult);
          }), continueOnCapturedContext: isConfigureAwaitFeatureEnabled).Execute().ConfigureAwait(isConfigureAwaitFeatureEnabled);
        }
        catch (CircuitBreakerException ex)
        {
          if (request.GetConfiguration() != null)
            executionResult = request.CreateResponse<string>(HttpStatusCode.ServiceUnavailable, ex.Message);
          else
            throw;
        }
        catch (VssServiceResponseException ex)
        {
        }
        catch (System.OperationCanceledException ex)
        {
          if (cancellationToken.IsCancellationRequested)
          {
            throw;
          }
          else
          {
            string orchestrationId;
            request.Headers.TryGetSingleValue("X-VSS-OrchestrationId", out orchestrationId);
            Guid e2eId;
            HttpRequestMessageExtensions.TryGetHeaderGuid((System.Net.Http.Headers.HttpHeaders) request.Headers, "X-VSS-E2EID", out e2eId);
            Guid uniqueIdentifier;
            HttpRequestMessageExtensions.TryGetHeaderGuid((System.Net.Http.Headers.HttpHeaders) request.Headers, "X-TFS-Session", out uniqueIdentifier);
            TeamFoundationTracingService.TraceRaw(100010, TraceLevel.Error, this.m_traceArea, this.m_traceLayer, e2eId, orchestrationId, uniqueIdentifier, "{{ \"Client\":\"{0}\", \"Endpoint\":\"{1}\", \"Timeout\":{2} }}", (object) this.ClientType, (object) request.RequestUri.Host, (object) timeout.Value.TotalMilliseconds);
            throw new TimeoutException(CommonResources.HttpRequestTimeout((object) timeout.Value), (Exception) ex);
          }
        }
        finally
        {
          int num;
          if (num < 0 && executionResult != null && executionResult.Headers.TryGetSingleValue("X-VSS-SenderDeploymentId", out correlation, false))
            this.UrlCorrelationService.SetCorrelation(correlationKey, correlation);
        }
      }
      HttpResponseMessage httpResponseMessage = executionResult;
      correlationKey = (CorrelationKey) null;
      return httpResponseMessage;
    }

    protected virtual Task<HttpResponseMessage> BaseSendAsync(
      HttpRequestMessage request,
      CancellationToken cancel)
    {
      return base.SendAsync(request, cancel);
    }

    private void AnalyzeResponseAndThrow(HttpResponseMessage response)
    {
      if (response.StatusCode >= HttpStatusCode.InternalServerError)
      {
        string message = (string) null;
        IEnumerable<string> values;
        if (response.Headers.TryGetValues("X-TFS-ServiceError", out values))
          message = UriUtility.UrlDecode(values.FirstOrDefault<string>());
        else if (string.IsNullOrEmpty(message) && !string.IsNullOrEmpty(response.ReasonPhrase))
          message = response.ReasonPhrase;
        throw new VssServiceResponseException(response.StatusCode, message, (Exception) null);
      }
    }
  }
}
