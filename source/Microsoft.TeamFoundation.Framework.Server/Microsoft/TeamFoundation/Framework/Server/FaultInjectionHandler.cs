// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.FaultInjectionHandler
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class FaultInjectionHandler : DelegatingHandler
  {
    private readonly string m_host;
    private readonly bool m_faultInjectionEnabled;
    private readonly ExceptionFaultSettings[] m_exceptionFaults;
    private readonly DelayFaultSettings[] m_delayFaults;
    private readonly string m_faultConfiguration;

    public FaultInjectionHandler(IVssRequestContext requestContext, string host)
    {
      this.m_host = host;
      IFaultInjectionService service = requestContext.GetService<IFaultInjectionService>();
      this.m_faultInjectionEnabled = service.IsFaultInjectionEnabled(requestContext);
      if (!this.m_faultInjectionEnabled)
        return;
      this.m_exceptionFaults = service.GetFaultsForTarget<ExceptionFaultSettings>(requestContext, "Rest", "Exception", this.m_host);
      this.m_delayFaults = service.GetFaultsForTarget<DelayFaultSettings>(requestContext, "Rest", "Delay", this.m_host);
      this.m_faultConfiguration = requestContext.GetService<IFaultInjectionRequestSettingsService>().GetSerializedFaultSettings(requestContext);
    }

    protected override async Task<HttpResponseMessage> SendAsync(
      HttpRequestMessage request,
      CancellationToken cancellationToken)
    {
      if (this.m_faultInjectionEnabled)
      {
        StandardFaultInjection.InjectException(this.m_exceptionFaults, "Rest");
        StandardFaultInjection.InjectDelay(this.m_delayFaults, "Rest");
        FaultInjectionHandler.AddFaultInjectionHeader(request, this.m_faultConfiguration);
      }
      bool continueOnCapturedContext = false;
      request.Properties.TryGetValue<bool>(FrameworkServerConstants.HandlerConfigureAwaitFeatureFlag, out continueOnCapturedContext);
      return await base.SendAsync(request, cancellationToken).ConfigureAwait(continueOnCapturedContext);
    }

    private static void AddFaultInjectionHeader(
      HttpRequestMessage request,
      string faultConfiguration)
    {
      if (string.IsNullOrWhiteSpace(faultConfiguration))
        return;
      request.Headers.Add("AzDevRequestFaults", faultConfiguration);
    }
  }
}
