// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.MlSpamValidationService
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Diagnostics;
using System.Net.Http;

namespace Microsoft.VisualStudio.Services.Gallery.Server
{
  internal class MlSpamValidationService : IMlSpamValidationService, IVssFrameworkService
  {
    private MlSpamValidationHTTPClient m_httpClient;
    private string m_url;
    private string m_apiKey = string.Empty;
    private const string Layer = "MlSpamValidationService";

    private void ConfigurationChangeCallback(
      IVssRequestContext requestContext,
      RegistryEntryCollection changedEntries)
    {
      this.LoadConfiguration(requestContext);
    }

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
      using (systemRequestContext.TraceBlock(12062100, 12062100, "gallery", nameof (MlSpamValidationService), nameof (ServiceStart)))
      {
        systemRequestContext.GetService<IVssRegistryService>().RegisterNotification(systemRequestContext, new RegistrySettingsChangedCallback(this.ConfigurationChangeCallback), false, "/Configuration/Service/Gallery/PackageVerification/**");
        if (this.m_httpClient != null)
          return;
        this.LoadConfiguration(systemRequestContext);
        this.m_httpClient = new MlSpamValidationHTTPClient(new HttpClient(), this.m_apiKey);
      }
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
      using (systemRequestContext.TraceBlock(12062100, 12062100, "gallery", nameof (MlSpamValidationService), nameof (ServiceEnd)))
        systemRequestContext.GetService<IVssRegistryService>().UnregisterNotification(systemRequestContext, new RegistrySettingsChangedCallback(this.ConfigurationChangeCallback));
    }

    public bool IsSpamOrNot(
      IVssRequestContext requestContext,
      UnpackagedExtensionData extensionData,
      string detailsContent)
    {
      requestContext.TraceEnter(12062100, "Gallery", nameof (MlSpamValidationService), nameof (IsSpamOrNot));
      try
      {
        MlSpamValidationService.MlSpamValidationResponse validationResponse = JsonUtilities.Deserialize<MlSpamValidationService.MlSpamValidationResponse>(this.m_httpClient.IsSpamOrNot(requestContext, extensionData, detailsContent, this.m_url).SyncResult<string>());
        requestContext.Trace(12062100, TraceLevel.Info, "Gallery", "CallToMlAPIForSpamCheck", string.Format("extension {0} has spam : {1} ", (object) extensionData.ExtensionName, (object) validationResponse.getResult()));
        requestContext.TraceLeave(12062100, "Gallery", nameof (MlSpamValidationService), nameof (IsSpamOrNot));
        return validationResponse.getResult();
      }
      catch (Exception ex)
      {
        requestContext.TraceException(12062100, "Gallery", nameof (MlSpamValidationService), ex);
        requestContext.TraceLeave(12062100, "Gallery", nameof (MlSpamValidationService), nameof (IsSpamOrNot));
        return false;
      }
    }

    private void LoadConfiguration(IVssRequestContext requestContext)
    {
      requestContext.TraceEnter(12062100, "gallery", nameof (MlSpamValidationService), nameof (LoadConfiguration));
      this.m_url = requestContext.GetService<IVssRegistryService>().ReadEntries(requestContext, (RegistryQuery) "/Configuration/Service/Gallery/PackageVerification/**").GetValueFromPath<string>("/Configuration/Service/Gallery/PackageVerification/MlSpamValidationServiceEndpoint", string.Empty);
      requestContext.TraceLeave(12062100, "gallery", nameof (MlSpamValidationService), nameof (LoadConfiguration));
    }

    private class MlSpamValidationResponse
    {
      public bool[] results { get; set; }

      public bool getResult() => this.results[0];
    }
  }
}
