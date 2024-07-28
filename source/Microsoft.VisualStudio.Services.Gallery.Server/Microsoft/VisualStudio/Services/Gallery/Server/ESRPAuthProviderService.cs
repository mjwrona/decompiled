// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.ESRPAuthProviderService
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.Identity.Client;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Cloud;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Microsoft.VisualStudio.Services.Gallery.Server
{
  public class ESRPAuthProviderService : IESRPAuthProviderService, IVssFrameworkService
  {
    private IConfidentialClientApplication confidentialClientApplication;
    private ConfidentialClientApplicationOptions confidentialClientApplicationOptions;
    private bool m_Initialized;
    private readonly object m_syncObject = new object();
    private static readonly string s_layer = nameof (ESRPAuthProviderService);
    private string m_esrpClientId;
    private string m_esrpApiEndPoint;

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
      systemRequestContext.TraceEnter(12062095, "gallery", ESRPAuthProviderService.s_layer, nameof (ServiceStart));
      if (systemRequestContext.ExecutionEnvironment.IsHostedDeployment)
      {
        systemRequestContext.GetService<IVssRegistryService>().RegisterNotification(systemRequestContext, new RegistrySettingsChangedCallback(this.ConfigurationChangeCallback), false, "/Configuration/Service/Gallery/PackageVerification/**");
        systemRequestContext.GetService<ITeamFoundationStrongBoxService>().RegisterNotification(systemRequestContext, new StrongBoxItemChangedCallback(this.StrongboxValueChangeCallback), "ConfigurationSecrets", (IEnumerable<string>) new string[1]
        {
          "ESRPClientToken"
        });
        this.LoadESRPConfiguration(systemRequestContext);
        this.UpdateClientSettings(systemRequestContext);
      }
      systemRequestContext.TraceLeave(12062095, "gallery", ESRPAuthProviderService.s_layer, nameof (ServiceStart));
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
      systemRequestContext.TraceEnter(12062095, "gallery", ESRPAuthProviderService.s_layer, nameof (ServiceEnd));
      if (systemRequestContext.ExecutionEnvironment.IsHostedDeployment)
      {
        systemRequestContext.GetService<ITeamFoundationStrongBoxService>().UnregisterNotification(systemRequestContext, new StrongBoxItemChangedCallback(this.StrongboxValueChangeCallback));
        systemRequestContext.GetService<IVssRegistryService>().UnregisterNotification(systemRequestContext, new RegistrySettingsChangedCallback(this.ConfigurationChangeCallback));
      }
      systemRequestContext.TraceLeave(12062095, "gallery", ESRPAuthProviderService.s_layer, nameof (ServiceEnd));
    }

    private void ConfigurationChangeCallback(
      IVssRequestContext requestContext,
      RegistryEntryCollection changedEntries)
    {
      this.LoadESRPConfiguration(requestContext);
      this.UpdateClientSettings(requestContext);
    }

    private void LoadESRPConfiguration(IVssRequestContext requestContext)
    {
      requestContext.TraceEnter(12061119, "gallery", ESRPAuthProviderService.s_layer, nameof (LoadESRPConfiguration));
      RegistryEntryCollection registryEntryCollection = requestContext.GetService<IVssRegistryService>().ReadEntries(requestContext, (RegistryQuery) "/Configuration/Service/Gallery/PackageVerification/**");
      string configurationSetting = AzureRoleUtil.GetOverridableConfigurationSetting("RuntimeServicePrincipalClientId");
      this.m_esrpClientId = registryEntryCollection.GetValueFromPath<string>("/Configuration/Service/Gallery/PackageVerification/ESRP/ESRPClientId", configurationSetting);
      this.m_esrpApiEndPoint = registryEntryCollection.GetValueFromPath<string>("/Configuration/Service/Gallery/PackageVerification/ESRP/ESRPApiEndpoint", "https://api.esrp.microsoft.com/.default");
      requestContext.TraceLeave(12061119, "gallery", ESRPAuthProviderService.s_layer, nameof (LoadESRPConfiguration));
    }

    private void StrongboxValueChangeCallback(
      IVssRequestContext requestContext,
      IEnumerable<StrongBoxItemName> itemNames)
    {
      this.UpdateClientSettings(requestContext);
    }

    private void UpdateClientSettings(IVssRequestContext systemRequestContext)
    {
      systemRequestContext.TraceEnter(12062095, "gallery", ESRPAuthProviderService.s_layer, nameof (UpdateClientSettings));
      try
      {
        ITeamFoundationStrongBoxService service = systemRequestContext.GetService<ITeamFoundationStrongBoxService>();
        StrongBoxItemInfo itemInfo = service.GetItemInfo(systemRequestContext, "ConfigurationSecrets", "ESRPClientToken", false);
        if (itemInfo == null)
        {
          systemRequestContext.Trace(12062095, TraceLevel.Error, "gallery", ESRPAuthProviderService.s_layer, nameof (UpdateClientSettings), (object) "No strongbox item info found.");
        }
        else
        {
          string str = service.GetString(systemRequestContext, itemInfo);
          if (string.IsNullOrWhiteSpace(str))
          {
            systemRequestContext.Trace(12062095, TraceLevel.Error, "gallery", ESRPAuthProviderService.s_layer, nameof (UpdateClientSettings), (object) "No clientSecret string found.");
          }
          else
          {
            ConfidentialClientApplicationOptions applicationOptions = new ConfidentialClientApplicationOptions();
            applicationOptions.ClientId = this.m_esrpClientId;
            applicationOptions.ClientSecret = str;
            this.confidentialClientApplicationOptions = applicationOptions;
            this.m_Initialized = false;
          }
        }
      }
      catch (Exception ex)
      {
        systemRequestContext.TraceException(12062095, "gallery", ESRPAuthProviderService.s_layer, ex);
      }
      finally
      {
        systemRequestContext.TraceLeave(12062095, "gallery", ESRPAuthProviderService.s_layer, nameof (UpdateClientSettings));
      }
    }

    private IConfidentialClientApplication GetConfidentialClientApplication()
    {
      if (!this.m_Initialized)
      {
        lock (this.m_syncObject)
        {
          if (!this.m_Initialized)
          {
            this.confidentialClientApplication = ConfidentialClientApplicationBuilder.CreateWithApplicationOptions(this.confidentialClientApplicationOptions).Build();
            this.m_Initialized = true;
          }
        }
      }
      return this.confidentialClientApplication;
    }

    public string GetOAuthToken(IVssRequestContext requestContext, Guid tenantId)
    {
      try
      {
        AuthenticationResult result = this.GetConfidentialClientApplication().AcquireTokenForClient((IEnumerable<string>) new List<string>()
        {
          this.m_esrpApiEndPoint
        }).WithAuthority(string.Format("https://login.microsoftonline.com/{0}", (object) tenantId)).WithSendX5C(true).ExecuteAsync().GetAwaiter().GetResult();
        requestContext.Trace(12062095, TraceLevel.Info, "gallery", ESRPAuthProviderService.s_layer, string.Format("ESRP Auth token retrieved from source: {0}", (object) result.AuthenticationResultMetadata.TokenSource));
        return result.AccessToken;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(12062095, "gallery", ESRPAuthProviderService.s_layer, ex);
        throw;
      }
    }
  }
}
