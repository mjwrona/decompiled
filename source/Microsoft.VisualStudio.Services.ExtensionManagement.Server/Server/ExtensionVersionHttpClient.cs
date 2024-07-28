// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ExtensionManagement.Server.ExtensionVersionHttpClient
// Assembly: Microsoft.VisualStudio.Services.ExtensionManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 57F50803-C5C4-41A9-A26F-AD293D563111
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ExtensionManagement.Server.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Common.Diagnostics;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.ExtensionManagement.Server
{
  public class ExtensionVersionHttpClient : VssHttpClientBase
  {
    private const string c_jsonMediaType = "application/json";

    public ExtensionVersionHttpClient(Uri baseUrl)
      : base(baseUrl, new VssCredentials())
    {
    }

    public async Task<List<SupportedExtension>> GetSupportedVersionsAsync(
      string versionCheckUrl,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      ExtensionVersionHttpClient versionHttpClient = this;
      HttpMethod method = new HttpMethod("OPTIONS");
      List<SupportedExtension> supportedVersionsAsync;
      using (VssTraceActivity.GetOrCreate().EnterCorrelationScope())
      {
        using (HttpRequestMessage requestMessage = versionHttpClient.CreateVersionRequestMessage(method, versionCheckUrl))
          supportedVersionsAsync = await versionHttpClient.SendAsync<List<SupportedExtension>>(requestMessage, userState, cancellationToken).ConfigureAwait(false);
      }
      return supportedVersionsAsync;
    }

    protected HttpRequestMessage CreateVersionRequestMessage(
      HttpMethod method,
      string versionCheckUrl,
      string mediaType = "application/json")
    {
      MediaTypeWithQualityHeaderValue qualityHeaderValue = new MediaTypeWithQualityHeaderValue(mediaType);
      return new HttpRequestMessage(method, versionCheckUrl)
      {
        Headers = {
          Accept = {
            qualityHeaderValue
          }
        }
      };
    }
  }
}
