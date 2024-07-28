// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.DevOps.ServiceEndpoints.Server.PlatformOAuthConfigurationService
// Assembly: Microsoft.Azure.DevOps.ServiceEndpoints.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B7D66E3F-07ED-4CF3-859D-36958D465656
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DevOps.ServiceEndpoints.Server.dll

using Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server;
using Microsoft.Azure.DevOps.ServiceEndpoints.Server.DataAccess;
using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Azure.DevOps.ServiceEndpoints.Server
{
  internal class PlatformOAuthConfigurationService : 
    IOAuthConfigurationService2,
    IVssFrameworkService
  {
    private readonly IOAuthConfigurationSecretsHelper secretsHelper;
    private readonly LibrarySecurityProvider securityProvider;
    private InternalAuthConfigurationStore configurationStore;
    private const string c_layer = "PlatformOAuthConfigurationService";

    public PlatformOAuthConfigurationService()
      : this((IOAuthConfigurationSecretsHelper) new OAuthConfigurationSecretsHelper(), (LibrarySecurityProvider) new OAuthConfigurationSecurityProvider())
    {
    }

    protected PlatformOAuthConfigurationService(
      IOAuthConfigurationSecretsHelper oauthConfigurationSecretsHelper,
      LibrarySecurityProvider oauthConfigurationSecurityProvider)
    {
      this.secretsHelper = oauthConfigurationSecretsHelper;
      this.securityProvider = oauthConfigurationSecurityProvider;
    }

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
      try
      {
        this.configurationStore = new InternalAuthConfigurationStore(systemRequestContext);
      }
      catch (Exception ex)
      {
        systemRequestContext.TraceException(34000900, "ServiceEndpoints", "InternalAuthConfigurationStore", ex);
        throw;
      }
      finally
      {
        systemRequestContext.TraceLeave(34000900, "ServiceEndpoints", "InternalAuthConfigurationStore", "ServiceLeave");
      }
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public AuthConfiguration CreateAuthConfiguration(
      IVssRequestContext requestContext,
      OAuthConfigurationParams configurationParams)
    {
      using (new MethodScope(requestContext, nameof (PlatformOAuthConfigurationService), nameof (CreateAuthConfiguration)))
      {
        try
        {
          if (PlatformOAuthConfigurationService.IsJiraEndpointType(configurationParams.EndpointType))
            PlatformOAuthConfigurationService.CheckCallerIsServicePrincipal(requestContext);
          OAuthConfigurationValidator.ValidateConfiguration(requestContext, configurationParams);
          this.securityProvider.CheckCreatePermissions(requestContext, new Guid?(), ServiceEndpointResources.OAuthConfiguration());
          string clientSecret = configurationParams.ClientSecret;
          Microsoft.VisualStudio.Services.Identity.Identity userIdentity = requestContext.GetUserIdentity();
          OAuthConfiguration configuration;
          using (new MethodScope(requestContext, nameof (PlatformOAuthConfigurationService), nameof (CreateAuthConfiguration)))
          {
            using (OAuthConfigurationComponent component = requestContext.CreateComponent<OAuthConfigurationComponent>())
              configuration = component.AddOAuthConfiguration(requestContext, configurationParams);
          }
          if (configuration == null)
            return (AuthConfiguration) null;
          this.secretsHelper.StoreSecrets(requestContext, configuration.Id, configuration.ClientId, clientSecret);
          this.securityProvider.AddLibraryItemCreatorAsItemAdministrator(requestContext, new Guid?(), userIdentity, configuration.Id.ToString());
          configuration.ResolveIdentityRef(requestContext);
          return configuration.ConvertToAuthConfiguration();
        }
        catch (Exception ex)
        {
          requestContext.TraceException(nameof (PlatformOAuthConfigurationService), ex);
          throw;
        }
      }
    }

    public AuthConfiguration UpdateAuthConfiguration(
      IVssRequestContext requestContext,
      Guid configurationId,
      OAuthConfigurationParams configurationParams)
    {
      if (PlatformOAuthConfigurationService.IsJiraEndpointType(configurationParams.EndpointType))
        PlatformOAuthConfigurationService.CheckCallerIsServicePrincipal(requestContext);
      OAuthConfigurationValidator.ValidateConfigurationForUpdate(requestContext, configurationParams);
      OAuthConfiguration configuration;
      using (new MethodScope(requestContext, nameof (PlatformOAuthConfigurationService), nameof (UpdateAuthConfiguration)))
      {
        this.securityProvider.CheckPermissions(requestContext, new Guid?(), configurationId.ToString(), 2, false, ServiceEndpointResources.OAuthConfigurationAccessDeniedForAdminOperation());
        using (OAuthConfigurationComponent component = requestContext.CreateComponent<OAuthConfigurationComponent>())
          configuration = component.UpdateOAuthConfiguration(requestContext, configurationId, configurationParams);
      }
      configuration.ResolveIdentityRef(requestContext);
      return configuration.ConvertToAuthConfiguration();
    }

    public AuthConfiguration GetAuthConfiguration(
      IVssRequestContext requestContext,
      Guid configurationId)
    {
      return this.configurationStore.IsInternalAuthConfiguration(configurationId) ? this.GetInternalOAuthConfiguration(requestContext, configurationId) : this.GetUserOAuthConfiguration(requestContext, configurationId).ConvertToAuthConfiguration();
    }

    public IList<AuthConfiguration> GetAuthConfigurationsByIds(
      IVssRequestContext requestContext,
      IList<Guid> configurationIds)
    {
      List<OAuthConfiguration> resultInternalConfigurations;
      List<Guid> userConfigurationIds;
      this.FilterInternalAndUserConfigurations(requestContext, configurationIds, out resultInternalConfigurations, out userConfigurationIds);
      resultInternalConfigurations.AddRange((IEnumerable<OAuthConfiguration>) this.GetUserOAuthConfigurationsByIds(requestContext, (IList<Guid>) userConfigurationIds));
      return resultInternalConfigurations.ConvertToAuthConfiguration();
    }

    public IList<AuthConfiguration> GetAuthConfigurations(
      IVssRequestContext requestContext,
      string endpointType,
      OAuthConfigurationActionFilter actionFilter)
    {
      IList<AuthConfiguration> authConfigurations1 = this.GetUserAuthConfigurations(requestContext, endpointType, actionFilter);
      if (!endpointType.IsNullOrEmpty<char>())
      {
        List<AuthConfiguration> authConfigurations2 = this.configurationStore.GetInternalAuthConfigurations(requestContext, endpointType);
        if (!authConfigurations2.IsNullOrEmpty<AuthConfiguration>())
          authConfigurations1.AddRange<AuthConfiguration, IList<AuthConfiguration>>((IEnumerable<AuthConfiguration>) authConfigurations2);
      }
      return authConfigurations1;
    }

    public AuthConfiguration DeleteAuthConfiguration(
      IVssRequestContext requestContext,
      Guid configurationId)
    {
      using (new MethodScope(requestContext, nameof (PlatformOAuthConfigurationService), nameof (DeleteAuthConfiguration)))
      {
        this.securityProvider.CheckPermissions(requestContext, new Guid?(), configurationId.ToString(), 2, false, ServiceEndpointResources.OAuthConfigurationAccessDeniedForAdminOperation());
        requestContext.GetService<IServiceEndpointService2>().HandleOAuthConfigurationDelete(requestContext, configurationId);
        OAuthConfiguration configuration;
        using (OAuthConfigurationComponent component = requestContext.CreateComponent<OAuthConfigurationComponent>())
          configuration = component.DeleteOAuthConfiguration(configurationId);
        if (configuration != null)
          this.secretsHelper.DeleteSecrets(requestContext, configurationId);
        return configuration.ConvertToAuthConfiguration();
      }
    }

    public void ReadAuthConfigurationSecrets(
      IVssRequestContext requestContext,
      AuthConfiguration configuration)
    {
      if (this.securityProvider.HasPermissions(requestContext, new Guid?(), configuration.Id.ToString(), 8))
      {
        if (this.configurationStore.IsInternalAuthConfiguration(configuration.Id))
        {
          IDictionary<string, Parameter> dictionary = this.configurationStore.ReadInternalAuthConfigurationSecrets(requestContext, configuration.Id);
          configuration.ClientSecret = dictionary.GetValueOrDefault<string, Parameter>("ClientSecret").Value;
          configuration.Parameters.AddRange<KeyValuePair<string, Parameter>, IDictionary<string, Parameter>>(dictionary.Where<KeyValuePair<string, Parameter>>((Func<KeyValuePair<string, Parameter>, bool>) (secret => secret.Key != "ClientSecret")));
        }
        else
          configuration.ClientSecret = this.secretsHelper.ReadSecrets(requestContext, configuration.Id, configuration.ClientId);
      }
      else
        configuration.ClientSecret = (string) null;
    }

    private AuthConfiguration GetInternalOAuthConfiguration(
      IVssRequestContext requestContext,
      Guid configurationId)
    {
      AuthConfiguration authConfiguration = this.configurationStore.GetInternalAuthConfiguration(configurationId);
      this.ReadAuthConfigurationSecrets(requestContext, authConfiguration);
      return authConfiguration;
    }

    private OAuthConfiguration GetUserOAuthConfiguration(
      IVssRequestContext requestContext,
      Guid configurationId)
    {
      using (new MethodScope(requestContext, nameof (PlatformOAuthConfigurationService), nameof (GetUserOAuthConfiguration)))
      {
        try
        {
          this.securityProvider.CheckPermissions(requestContext, new Guid?(), configurationId.ToString(), 1, false, ServiceEndpointResources.OAuthConfigurationAccessDeniedForView());
          OAuthConfiguration oauthConfiguration;
          using (OAuthConfigurationComponent component = requestContext.CreateComponent<OAuthConfigurationComponent>())
            oauthConfiguration = component.GetOAuthConfiguration(configurationId);
          if (oauthConfiguration == null)
            return (OAuthConfiguration) null;
          AuthConfiguration authConfiguration = oauthConfiguration.ConvertToAuthConfiguration();
          this.ReadAuthConfigurationSecrets(requestContext, authConfiguration);
          authConfiguration.ResolveIdentityRef(requestContext);
          return (OAuthConfiguration) authConfiguration;
        }
        catch (Exception ex)
        {
          requestContext.TraceException(nameof (PlatformOAuthConfigurationService), ex);
          throw;
        }
      }
    }

    private void FilterInternalAndUserConfigurations(
      IVssRequestContext requestContext,
      IList<Guid> configurationIds,
      out List<OAuthConfiguration> resultInternalConfigurations,
      out List<Guid> userConfigurationIds)
    {
      resultInternalConfigurations = new List<OAuthConfiguration>();
      userConfigurationIds = new List<Guid>();
      this.configurationStore.LoadConfigurations(requestContext);
      IList<AuthConfiguration> authConfigurations = this.configurationStore.GetInternalAuthConfigurations();
      foreach (Guid configurationId1 in (IEnumerable<Guid>) configurationIds)
      {
        Guid configurationId = configurationId1;
        AuthConfiguration authConfiguration = authConfigurations.SingleOrDefault<AuthConfiguration>((Func<AuthConfiguration, bool>) (c => c.Id.Equals(configurationId)));
        if (authConfiguration != null)
          resultInternalConfigurations.Add((OAuthConfiguration) authConfiguration);
        else
          userConfigurationIds.Add(configurationId);
      }
    }

    private IList<OAuthConfiguration> GetUserOAuthConfigurationsByIds(
      IVssRequestContext requestContext,
      IList<Guid> configurationIds)
    {
      IList<OAuthConfiguration> configurationsByIds;
      using (new MethodScope(requestContext, nameof (PlatformOAuthConfigurationService), nameof (GetUserOAuthConfigurationsByIds)))
      {
        using (OAuthConfigurationComponent component = requestContext.CreateComponent<OAuthConfigurationComponent>())
          configurationsByIds = component.GetOAuthConfigurationsByIds(configurationIds);
      }
      configurationsByIds.ResolveIdentityRefs(requestContext);
      return configurationsByIds;
    }

    private IList<AuthConfiguration> GetUserAuthConfigurations(
      IVssRequestContext requestContext,
      string endpointType,
      OAuthConfigurationActionFilter actionFilter)
    {
      this.securityProvider.CheckAndInitializeLibraryPermissions(requestContext, new Guid?());
      IList<OAuthConfiguration> oauthConfigurations;
      using (new MethodScope(requestContext, nameof (PlatformOAuthConfigurationService), nameof (GetUserAuthConfigurations)))
      {
        using (OAuthConfigurationComponent component = requestContext.CreateComponent<OAuthConfigurationComponent>())
          oauthConfigurations = component.GetOAuthConfigurations(endpointType);
      }
      IList<OAuthConfiguration> configurations = this.FilterOAuthConfigurations(requestContext, oauthConfigurations, actionFilter);
      configurations.ResolveIdentityRefs(requestContext);
      return configurations.ConvertToAuthConfiguration();
    }

    private IList<OAuthConfiguration> FilterOAuthConfigurations(
      IVssRequestContext requestContext,
      IList<OAuthConfiguration> configList,
      OAuthConfigurationActionFilter actionFilter)
    {
      if (configList != null && configList.Any<OAuthConfiguration>())
      {
        int permissions = 1;
        if ((actionFilter & OAuthConfigurationActionFilter.Use) == OAuthConfigurationActionFilter.Use)
          permissions |= 16;
        if ((actionFilter & OAuthConfigurationActionFilter.Manage) == OAuthConfigurationActionFilter.Manage)
          permissions |= 2;
        configList = (IList<OAuthConfiguration>) configList.Where<OAuthConfiguration>((Func<OAuthConfiguration, bool>) (config => this.securityProvider.HasPermissions(requestContext, new Guid?(), config.Id.ToString(), permissions))).ToList<OAuthConfiguration>();
      }
      return configList;
    }

    private static void CheckCallerIsServicePrincipal(IVssRequestContext requestContext) => new ServiceEndpointSecurity().CheckCallerIsServicePrincipal(requestContext, Guid.Empty.ToString("D"));

    private static bool IsJiraEndpointType(string endpointType) => string.Equals(endpointType, "Jira", StringComparison.OrdinalIgnoreCase);
  }
}
