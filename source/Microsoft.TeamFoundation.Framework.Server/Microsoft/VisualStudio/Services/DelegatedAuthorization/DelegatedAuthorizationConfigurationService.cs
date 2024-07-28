// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.DelegatedAuthorization.DelegatedAuthorizationConfigurationService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.DelegatedAuthorization.Settings;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

namespace Microsoft.VisualStudio.Services.DelegatedAuthorization
{
  internal class DelegatedAuthorizationConfigurationService : 
    IDelegatedAuthorizationConfigurationService,
    IVssFrameworkService
  {
    private const string s_area = "DelegatedAuthorization";
    private const string s_layer = "Service";
    private AuthorizationScopeConfiguration configuration;
    private readonly ServiceFactory<IVssRegistryService> registryService;
    private IDelegatedAuthorizationSettingsManager settingsManager;
    private DelegatedAuthorizationSettings settings;

    public DelegatedAuthorizationConfigurationService()
      : this((IDelegatedAuthorizationSettingsManager) new DelegatedAuthorizationSettingsManager(), (ServiceFactory<IVssRegistryService>) (requestContext => (IVssRegistryService) requestContext.GetService<CachedRegistryService>()))
    {
    }

    internal DelegatedAuthorizationConfigurationService(
      IDelegatedAuthorizationSettingsManager settingsManager,
      ServiceFactory<IVssRegistryService> registryService)
    {
      this.settingsManager = settingsManager;
      this.registryService = registryService;
    }

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
      if (!systemRequestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
        return;
      IVssRegistryService registryService = this.registryService(systemRequestContext);
      registryService.RegisterNotification(systemRequestContext, new RegistrySettingsChangedCallback(this.OnRegistryChanged), "/Service/DelegatedAuthorization/...");
      registryService.RegisterNotification(systemRequestContext, new RegistrySettingsChangedCallback(this.OnRegistryChanged), "FeatureAvailability/Definitions/AzureDevOps.Services.Framework.UseJWTSigningCertificateThumbprint.M168/...");
      this.Initialize(systemRequestContext);
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
      if (!systemRequestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
        return;
      this.registryService(systemRequestContext).UnregisterNotification(systemRequestContext, new RegistrySettingsChangedCallback(this.OnRegistryChanged));
    }

    public AuthorizationScopeConfiguration GetConfiguration(IVssRequestContext requestContext)
    {
      if (requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
        return this.configuration;
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      return vssRequestContext.GetService<IDelegatedAuthorizationConfigurationService>().GetConfiguration(vssRequestContext);
    }

    public DelegatedAuthorizationSettings GetSettings(IVssRequestContext requestContext)
    {
      if (requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
        return this.settings;
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      return vssRequestContext.GetService<IDelegatedAuthorizationConfigurationService>().GetSettings(vssRequestContext);
    }

    public VssSigningCredentials GetSigningCredentials(
      IVssRequestContext requestContext,
      bool useOldSigningCredentials,
      bool force = false)
    {
      if (!requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
      {
        IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
        return vssRequestContext.GetService<IDelegatedAuthorizationConfigurationService>().GetSigningCredentials(vssRequestContext, useOldSigningCredentials);
      }
      IVssRequestContext vssRequestContext1 = requestContext.To(TeamFoundationHostType.Deployment).Elevate();
      TeamFoundationStrongBoxService service = vssRequestContext1.GetService<TeamFoundationStrongBoxService>();
      bool flag = requestContext.IsFeatureEnabled("AzureDevOps.Services.Framework.UseJWTSigningCertificateThumbprint.M168") && !useOldSigningCredentials;
      if (force)
        flag = true;
      Guid drawerId = service.UnlockDrawer(vssRequestContext1, flag ? "ConfigurationSecrets" : this.settings.SigningCertificateDrawerName, false);
      if (drawerId == Guid.Empty)
        throw new ArgumentException("Error in unlocking strongbox drawer.");
      return VssSigningCredentials.Create(service.RetrieveFileAsCertificate(vssRequestContext1, drawerId, flag ? this.settings.JWTSigningCertificateThumbprintLookupKey : this.settings.SigningCertificateThumbprint, false, true));
    }

    public IList<X509Certificate2> GetSigningPublicKeys(IVssRequestContext requestContext)
    {
      requestContext.TraceEnter(1457002, "DelegatedAuthorization", "Service", nameof (GetSigningPublicKeys));
      try
      {
        return (IList<X509Certificate2>) new X509Certificate2[2]
        {
          new X509Certificate2(this.GetCertificate(requestContext, this.settings.PrimaryJWTSigningCertificateThumbprintLookupKey).Export(X509ContentType.Cert)),
          new X509Certificate2(this.GetCertificate(requestContext, this.settings.SecondaryJWTSigningCertificateThumbprintLookUpKey).Export(X509ContentType.Cert))
        };
      }
      finally
      {
        requestContext.TraceLeave(1457002, "DelegatedAuthorization", "Service", nameof (GetSigningPublicKeys));
      }
    }

    private X509Certificate2 GetCertificate(IVssRequestContext requestContext, string lookupKey)
    {
      IVssRequestContext vssRequestContext = requestContext.Elevate();
      TeamFoundationStrongBoxService service = vssRequestContext.GetService<TeamFoundationStrongBoxService>();
      Guid drawerId = service.UnlockDrawer(vssRequestContext, "ConfigurationSecrets", true);
      return service.RetrieveFileAsCertificate(vssRequestContext, drawerId, lookupKey, false, true);
    }

    private void OnRegistryChanged(
      IVssRequestContext requestContext,
      RegistryEntryCollection changedEntries)
    {
      this.Initialize(requestContext);
    }

    private void Initialize(IVssRequestContext requestContext)
    {
      this.settings = this.settingsManager.ReadDelegatedAuthorizationSettings(requestContext);
      AuthorizationScopeDefinitions scopeDefinitions = (AuthorizationScopeDefinitions) null;
      this.configuration = AuthorizationScopeDefinitions.TryParse(this.settings.AuthorizationScopeDefinitionsJson, out scopeDefinitions) ? new AuthorizationScopeConfiguration(scopeDefinitions) : new AuthorizationScopeConfiguration(AuthorizationScopeDefinitions.Default);
    }
  }
}
