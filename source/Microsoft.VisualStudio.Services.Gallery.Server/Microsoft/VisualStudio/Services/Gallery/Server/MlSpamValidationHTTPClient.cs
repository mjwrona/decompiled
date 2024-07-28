// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.MlSpamValidationHTTPClient
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using System;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Gallery.Server
{
  internal class MlSpamValidationHTTPClient : IMlSpamValidationHTTPClient
  {
    private readonly HttpClient httpClient;

    public MlSpamValidationHTTPClient(HttpClient httpClient, string apiKey)
    {
      this.httpClient = httpClient ?? throw new ArgumentNullException(nameof (httpClient));
      httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
    }

    public async Task<string> IsSpamOrNot(
      IVssRequestContext requestContext,
      UnpackagedExtensionData extensionData,
      string detailsContent,
      string url)
    {
      requestContext.TraceEnter(12062098, "Gallery", nameof (MlSpamValidationHTTPClient), nameof (IsSpamOrNot));
      StringContent content = new StringContent(new MlSpamValidationHTTPClient.MlSpamValidationRequestBody(extensionData.DisplayName, extensionData.Description, detailsContent).Serialize<MlSpamValidationHTTPClient.MlSpamValidationRequestBody>());
      content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
      requestContext.Trace(12062098, TraceLevel.Info, "Gallery", "CallToMlAPIForSpamCheck", string.Format("Checking extension {0} for spam ", (object) extensionData.ExtensionName));
      HttpResponseMessage httpResponseMessage = await this.httpClient.PostAsync(url, (HttpContent) content).ConfigureAwait(false);
      requestContext.Trace(12062098, TraceLevel.Info, "Gallery", "CallToMlAPIForSpamCheck", string.Format("Response Status Code : {0}", (object) httpResponseMessage.StatusCode));
      if (httpResponseMessage.IsSuccessStatusCode)
      {
        if (httpResponseMessage.Content == null)
          throw new ArgumentException("Content property is null", "response");
        string str = await httpResponseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);
        if (string.IsNullOrWhiteSpace(str))
          throw new ArgumentException("MlSpamValidationHTTPClient recieved null or empty response.", "response");
        requestContext.TraceLeave(12062098, "Gallery", nameof (MlSpamValidationHTTPClient), nameof (IsSpamOrNot));
        return str;
      }
      string message = string.Format("The request failed with status code: {0}", (object) httpResponseMessage.StatusCode);
      message += (await httpResponseMessage.Content.ReadAsStringAsync().ConfigureAwait(false)).ToString();
      requestContext.Trace(12062098, TraceLevel.Error, "Gallery", "CallToMlAPIForSpamCheck", message);
      requestContext.TraceLeave(12062098, "Gallery", nameof (MlSpamValidationHTTPClient), nameof (IsSpamOrNot));
      throw new HttpRequestException(message);
    }

    private class MlSpamValidationRequestBody
    {
      public MlSpamValidationHTTPClient.MlSpamValidationRequestBody.ExtensionMetaDataForMlModel Inputs { get; set; }

      public MlSpamValidationRequestBody(
        string displayName,
        string shortDescription,
        string longDescription)
      {
        this.Inputs = new MlSpamValidationHTTPClient.MlSpamValidationRequestBody.ExtensionMetaDataForMlModel(displayName, shortDescription, longDescription);
      }

      public class ExtensionMetaDataForMlModel
      {
        public MlSpamValidationHTTPClient.MlSpamValidationRequestBody.ExtensionMetaDataForMlSpamCheck[] data { get; set; }

        public MlSpamValidationHTTPClient.MlSpamValidationRequestBody.globalParameters GlobalParameters { get; set; }

        public ExtensionMetaDataForMlModel(
          string displayName,
          string shortDescription,
          string longDescription)
        {
          this.GlobalParameters = new MlSpamValidationHTTPClient.MlSpamValidationRequestBody.globalParameters();
          this.data = new MlSpamValidationHTTPClient.MlSpamValidationRequestBody.ExtensionMetaDataForMlSpamCheck[1];
          this.data[0] = new MlSpamValidationHTTPClient.MlSpamValidationRequestBody.ExtensionMetaDataForMlSpamCheck(displayName, shortDescription, longDescription);
        }
      }

      public class ExtensionMetaDataForMlSpamCheck
      {
        public string DisplayName { get; set; }

        public string ShortDescription { get; set; }

        public string LongDescription { get; set; }

        public ExtensionMetaDataForMlSpamCheck(
          string displayName,
          string shortDescription,
          string longDescription)
        {
          this.LongDescription = Regex.Replace(longDescription, "(\\s+)|['\":]", " ");
          this.ShortDescription = Regex.Replace(shortDescription, "(\\s+)|['\":]", " ");
          this.DisplayName = Regex.Replace(displayName, "(\\s+)|['\":]", " ");
        }
      }

      public class globalParameters
      {
        public string method { get; set; }

        public globalParameters() => this.method = "predict";
      }
    }
  }
}
