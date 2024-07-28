// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.UxServices.UxServicesHttpRequestWrapper
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Platform, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A6A2C403-5081-466C-A570-9B50BFA8E213
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Platform.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Web;
using System.Web.Http;

namespace Microsoft.TeamFoundation.Server.WebAccess.UxServices
{
  public class UxServicesHttpRequestWrapper : IContentService
  {
    private readonly IVssRequestContext tfsRequestContext;
    private const string m_featureFlagEnableOriginatingUxService = "VisualStudio.WorkflowService.EnableOriginatingUxService";
    private const string m_defaultUxServiceRegistryPath = "/WebAccess/UxServices/VisualStudio/{0}";

    public UxServicesHttpRequestWrapper(IVssRequestContext tfsRequestContext) => this.tfsRequestContext = tfsRequestContext;

    public string GetContent(UxServicesRequestData uxRequestData)
    {
      string format = string.Empty;
      if (this.tfsRequestContext.IsFeatureEnabled("VisualStudio.WorkflowService.EnableOriginatingUxService"))
        format = !string.IsNullOrWhiteSpace(uxRequestData.UxServiceHeaderUrl) || !string.IsNullOrWhiteSpace(uxRequestData.UxServiceFooterUrl) ? (!(uxRequestData.Control == "header") ? uxRequestData.UxServiceFooterUrl : uxRequestData.UxServiceHeaderUrl) : this.GetDefaultUxServiceContent(uxRequestData);
      if (string.IsNullOrWhiteSpace(format))
        format = this.GetDefaultUxServiceContent(uxRequestData);
      return !string.IsNullOrWhiteSpace(format) ? this.GetWebResponse(new Uri(string.Format(format, (object) uxRequestData.Locale))) : string.Empty;
    }

    private string GetWebResponse(Uri uri)
    {
      this.tfsRequestContext.Trace(1049004, TraceLevel.Info, "WebAccess", "UxServices", "Accessing UxServices Url {0}", (object) uri.ToString());
      try
      {
        string webResponse = string.Empty;
        WebRequest webRequest = WebRequest.Create(uri);
        webRequest.Credentials = CredentialCache.DefaultCredentials;
        HttpWebResponse response = (HttpWebResponse) webRequest.GetResponse();
        using (Stream responseStream = response.GetResponseStream())
        {
          if (responseStream != null)
          {
            using (StreamReader streamReader = new StreamReader(responseStream))
            {
              webResponse = streamReader.ReadToEnd();
              streamReader.Close();
            }
            responseStream.Close();
          }
        }
        if (response.StatusCode != HttpStatusCode.OK || string.IsNullOrWhiteSpace(webResponse))
          throw new HttpResponseException(response.StatusCode);
        if (response.ResponseUri != (Uri) null && response.ResponseUri.PathAndQuery.Contains("500.htm"))
          throw new HttpException("Ux services end point returning a 500 htm page.");
        response.Close();
        return webResponse;
      }
      catch (Exception ex)
      {
        this.tfsRequestContext.Trace(1049001, TraceLevel.Error, "WebAccess", "UxServices", "Error accessing UxServices URL {0}", (object) uri);
        throw;
      }
    }

    private string GetDefaultUxServiceContent(UxServicesRequestData uxRequestData) => this.tfsRequestContext.GetService<CachedRegistryService>().GetValue<string>(this.tfsRequestContext, (RegistryQuery) string.Format("/WebAccess/UxServices/VisualStudio/{0}", (object) uxRequestData.Control), (string) null);
  }
}
