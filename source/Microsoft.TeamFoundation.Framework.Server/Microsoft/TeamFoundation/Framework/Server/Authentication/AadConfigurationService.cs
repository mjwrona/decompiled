// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.Authentication.AadConfigurationService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server.Authentication.Settings;
using System;
using System.Linq;

namespace Microsoft.TeamFoundation.Framework.Server.Authentication
{
  internal class AadConfigurationService : IAadConfigurationService, IVssFrameworkService
  {
    private AadSettings settings;
    private const string Filter = "/...";
    private const string AadAuthRoot = "/Service/Aad/Authentication";
    private const string AadServiceRoot = "/Service/Aad";
    private const string OrgAuthRoot = "/OrgId/Authentication";
    private const string AadAuthRootFilter = "/Service/Aad/Authentication/...";
    private const string AadServiceRootFilter = "/Service/Aad/...";
    private const string OrgAuthRootFilter = "/OrgId/Authentication/...";
    private const string ClientIdPath = "/OrgId/Authentication/ClientId";
    private const string AuthBaseUrlPath = "/Service/Aad/Authentication/BaseUrl";
    private const string AuthUrlPathTemplatePath = "/Service/Aad/Authentication/UrlPathTemplate";
    private const string AuthSiteIdPath = "/Service/Aad/Authentication/SiteId";
    private const string GraphApiResourcePath = "/Service/Aad/GraphApiResource";
    private const string MicrosoftServicesTenantPath = "/Service/Aad/MicrosoftServicesTenant";
    private readonly Guid DefaultClientId = new Guid("499b84ac-1321-427f-aa17-267ca6975798");
    private const string DefaultPathTemplate = "/{0}/oauth2/authorize?client_id={1}&site_id={2}&response_mode=form_post&response_type=code+id_token&redirect_uri={3}&nonce={4}&state={5}&resource={6}&cid={7}&wsucxt=1";
    private const string DefaultSiteId = "501454";
    private const string DefaultResource = "https://management.core.windows.net/";
    private const string DefaultMicrosoftServicesTenant = "microsoftservices.onmicrosoft.com";

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
      systemRequestContext.CheckDeploymentRequestContext();
      IVssRegistryService service = systemRequestContext.GetService<IVssRegistryService>();
      this.Initialize(systemRequestContext);
      service.RegisterNotification(systemRequestContext, new RegistrySettingsChangedCallback(this.OnRegistryChanged), "/Service/Aad/Authentication/...");
      service.RegisterNotification(systemRequestContext, new RegistrySettingsChangedCallback(this.OnRegistryChanged), "/Service/Aad/...");
      service.RegisterNotification(systemRequestContext, new RegistrySettingsChangedCallback(this.OnRegistryChanged), "/OrgId/Authentication/...");
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext) => systemRequestContext.GetService<IVssRegistryService>().UnregisterNotification(systemRequestContext, new RegistrySettingsChangedCallback(this.OnRegistryChanged));

    public AadSettings GetSettings(IVssRequestContext requestContext) => this.settings;

    private void OnRegistryChanged(
      IVssRequestContext deploymentContext,
      RegistryEntryCollection changedEntries)
    {
      if (!changedEntries.Any<RegistryEntry>())
        return;
      this.Initialize(deploymentContext);
    }

    private void Initialize(IVssRequestContext deploymentContext)
    {
      IVssRegistryService service = deploymentContext.GetService<IVssRegistryService>();
      this.settings = new AadSettings()
      {
        ClientId = service.GetValue<Guid>(deploymentContext, (RegistryQuery) "/OrgId/Authentication/ClientId", this.DefaultClientId).ToString(),
        AuthBaseUrl = service.GetValue(deploymentContext, (RegistryQuery) "/Service/Aad/Authentication/BaseUrl", false, (string) null),
        AuthUrlPathTemplate = service.GetValue(deploymentContext, (RegistryQuery) "/Service/Aad/Authentication/UrlPathTemplate", "/{0}/oauth2/authorize?client_id={1}&site_id={2}&response_mode=form_post&response_type=code+id_token&redirect_uri={3}&nonce={4}&state={5}&resource={6}&cid={7}&wsucxt=1"),
        AuthSiteId = service.GetValue(deploymentContext, (RegistryQuery) "/Service/Aad/Authentication/SiteId", "501454"),
        GraphApiResource = service.GetValue(deploymentContext, (RegistryQuery) "/Service/Aad/GraphApiResource", "https://management.core.windows.net/"),
        MicrosoftServicesTenant = service.GetValue(deploymentContext, (RegistryQuery) "/Service/Aad/MicrosoftServicesTenant", "microsoftservices.onmicrosoft.com")
      };
    }
  }
}
