// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.S2SUnauthorizedHandler
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.CircuitBreaker;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class S2SUnauthorizedHandler : DelegatingHandler
  {
    private readonly ICommandFactory m_commandFactory;
    private readonly IUrlDeploymentCorrelationService m_correlationService;
    internal static readonly string BaseCommandKey = nameof (S2SUnauthorizedHandler);
    private static readonly TimeSpan s_defaultCircuitBreakerTimeout = TimeSpan.FromDays(1.0);
    private static readonly TimeSpan s_defaultRollingWindow = TimeSpan.FromMinutes(5.0);
    private static readonly int s_defaultRollingWindowBuckets = 60;
    private static readonly int s_defaultMinimumRequestVolume = 100;
    private static readonly CommandPropertiesSetter s_defaultCircuitBreakerSettings = new CommandPropertiesSetter().WithExecutionTimeout(S2SUnauthorizedHandler.s_defaultCircuitBreakerTimeout).WithMetricsRollingStatisticalWindow(S2SUnauthorizedHandler.s_defaultRollingWindow).WithMetricsRollingStatisticalWindowBuckets(S2SUnauthorizedHandler.s_defaultRollingWindowBuckets).WithCircuitBreakerRequestVolumeThreshold(S2SUnauthorizedHandler.s_defaultMinimumRequestVolume);

    protected S2SUnauthorizedHandler(
      ICommandFactory commandFactory,
      IUrlDeploymentCorrelationService correlationService)
    {
      this.m_commandFactory = commandFactory;
      this.m_correlationService = correlationService;
    }

    public static S2SUnauthorizedHandler Create(IVssRequestContext requestContext)
    {
      IUrlDeploymentCorrelationService service = requestContext.GetService<IUrlDeploymentCorrelationService>();
      return new S2SUnauthorizedHandler((ICommandFactory) new CommandServiceFactory(requestContext, (CommandKey) S2SUnauthorizedHandler.BaseCommandKey, S2SUnauthorizedHandler.DefaultCircuitBreakerSettings), service);
    }

    protected override async Task<HttpResponseMessage> SendAsync(
      HttpRequestMessage request,
      CancellationToken cancellationToken)
    {
      bool isConfigureAwaitFeatureEnabled = false;
      request.Properties.TryGetValue<bool>(FrameworkServerConstants.HandlerConfigureAwaitFeatureFlag, out isConfigureAwaitFeatureEnabled);
      string str = this.m_correlationService.GetCorrelation(this.m_correlationService.GetCorrelationKey(request.RequestUri)) ?? request.RequestUri.Host;
      CommandSetter commandSetter = CommandSetter.WithGroupKey((CommandGroupKey) "Framework.").AndCommandKey((CommandKey) (this.m_commandFactory.CommandKey?.ToString() + "." + str));
      CommandAsync<HttpResponseMessage> command = (CommandAsync<HttpResponseMessage>) null;
      command = this.m_commandFactory.CreateCommandAsync<HttpResponseMessage>(commandSetter, (Func<Task<HttpResponseMessage>>) (async () =>
      {
        HttpResponseMessage httpResponseMessage;
        try
        {
          httpResponseMessage = await this.SendAsyncImpl(request, cancellationToken).ConfigureAwait(isConfigureAwaitFeatureEnabled);
        }
        catch (Exception ex) when (!S2SUnauthorizedHandler.IsS2SUnauthorizedException(ex))
        {
          ex.Data.Add((object) "{421AC3F1-A306-4C9B-B3F6-5812F9121FC8}", (object) true);
          throw;
        }
        return httpResponseMessage;
      }), (Func<Task<HttpResponseMessage>>) (() =>
      {
        IVssRequestContext deploymentContext;
        if (command.IsCircuitBreakerOpen && S2SUnauthorizedHandler.TryCreateDeploymentContext(request, out deploymentContext))
        {
          using (deploymentContext)
          {
            if (!deploymentContext.IsFeatureEnabled(FrameworkServerConstants.DisableSwitchToSecondaryCertificate))
            {
              IVssRegistryService service = deploymentContext.GetService<IVssRegistryService>();
              if (!service.GetValue<bool>(deploymentContext, (RegistryQuery) OAuth2RegistryConstants.S2SUseSecondarySigningCertificate, false))
                service.SetValue<bool>(deploymentContext, OAuth2RegistryConstants.S2SUseSecondarySigningCertificate, true);
            }
          }
        }
        throw new Exception("Intentionally failed the S2S Circuit Breaker fallback.")
        {
          Data = {
            {
              (object) "{421AC3F1-A306-4C9B-B3F6-5812F9121FC8}",
              (object) true
            }
          }
        };
      }), isConfigureAwaitFeatureEnabled);
      return await command.Execute().ConfigureAwait(isConfigureAwaitFeatureEnabled);
    }

    protected virtual Task<HttpResponseMessage> SendAsyncImpl(
      HttpRequestMessage request,
      CancellationToken cancellationToken)
    {
      return base.SendAsync(request, cancellationToken);
    }

    private static bool IsS2SUnauthorizedException(Exception exception)
    {
      if (!(exception is VssUnauthorizedException))
        return false;
      VssCredentialsType? nullable = (VssCredentialsType?) exception.Data[(object) "CredentialsType"];
      VssCredentialsType vssCredentialsType = VssCredentialsType.S2S;
      return nullable.GetValueOrDefault() == vssCredentialsType & nullable.HasValue;
    }

    private static bool TryCreateDeploymentContext(
      HttpRequestMessage request,
      out IVssRequestContext deploymentContext)
    {
      if (TeamFoundationApplicationCore.DeploymentInitialized)
      {
        deploymentContext = TeamFoundationApplicationCore.DeploymentServiceHost.CreateSystemContext(false);
        return true;
      }
      object obj;
      if (request.Properties.TryGetValue(TfsApiPropertyKeys.TfsRequestContextClient, out obj) && obj is IVssRequestContext vssRequestContext)
      {
        deploymentContext = vssRequestContext.ServiceHost.DeploymentServiceHost.CreateSystemContext(false);
        return true;
      }
      deploymentContext = (IVssRequestContext) null;
      return false;
    }

    internal static CommandPropertiesSetter DefaultCircuitBreakerSettings => S2SUnauthorizedHandler.s_defaultCircuitBreakerSettings;
  }
}
