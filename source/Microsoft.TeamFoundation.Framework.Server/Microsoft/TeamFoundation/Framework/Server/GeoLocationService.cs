// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.GeoLocationService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class GeoLocationService : IGeoLocationService, IVssFrameworkService
  {
    private IGeoLocationProvider m_geoLocationProvider;
    private bool m_geoLocationProviderInitialized;
    private bool m_isReverseIPLookupEnabled;
    private const string c_disableReverseIpLookup = "/Configuration/GeoLocation/DisableReverseIPLookup";

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
      systemRequestContext.CheckDeploymentRequestContext();
      if (systemRequestContext.IsHostProcessType(HostProcessType.JobAgent))
        return;
      using (IDisposableReadOnlyList<IGeoLocationProvider> extensions = systemRequestContext.GetExtensions<IGeoLocationProvider>(ExtensionLifetime.Service))
      {
        if (extensions != null && extensions.Count > 0)
          this.m_geoLocationProvider = extensions[0];
        IVssRegistryService service = systemRequestContext.GetService<IVssRegistryService>();
        service.RegisterNotification(systemRequestContext, new RegistrySettingsChangedCallback(this.OnDisableReverseIpLookupChanged), "/Configuration/GeoLocation/DisableReverseIPLookup");
        this.m_isReverseIPLookupEnabled = !service.GetValue<bool>(systemRequestContext, (RegistryQuery) "/Configuration/GeoLocation/DisableReverseIPLookup", false);
        if (!this.m_isReverseIPLookupEnabled)
          return;
        this.m_geoLocationProvider.InitializeProvider(systemRequestContext);
        this.m_geoLocationProviderInitialized = true;
      }
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public string GetRequestCountryCode(IVssRequestContext requestContext, string ip = null)
    {
      requestContext.CheckDeploymentRequestContext();
      if (this.m_geoLocationProvider == null || !this.m_isReverseIPLookupEnabled)
        return (string) null;
      if (!this.m_geoLocationProviderInitialized)
      {
        lock (this.m_geoLocationProvider)
        {
          this.m_geoLocationProvider.InitializeProvider(requestContext);
          this.m_geoLocationProviderInitialized = true;
        }
      }
      return this.m_geoLocationProvider.GetRequestCountryCode(requestContext, ip);
    }

    private void OnDisableReverseIpLookupChanged(
      IVssRequestContext requestContext,
      RegistryEntryCollection changedEntries)
    {
      this.m_isReverseIPLookupEnabled = !changedEntries.GetValueFromPath<bool>("/Configuration/GeoLocation/DisableReverseIPLookup", false);
    }
  }
}
