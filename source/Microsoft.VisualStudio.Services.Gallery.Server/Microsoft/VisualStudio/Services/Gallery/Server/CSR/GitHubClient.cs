// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.CSR.GitHubClient
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Gallery.Server.CSR
{
  public class GitHubClient : IGitHubClient, IVssFrameworkService
  {
    [SuppressMessage("Microsoft.VisualStudio.Services", "VSS0001:StaticMembersMayLeakCrossAccount", Justification = "Inspected")]
    private static readonly HttpClient httpClient = new HttpClient();

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public async Task<HttpResponseMessage> CreateGitHubIssue(
      string issuePayload,
      string gitHubRepoUrl,
      string bearer,
      string userAgent)
    {
      HttpContent httpContent = (HttpContent) new StringContent(issuePayload, Encoding.UTF8, "application/json");
      GitHubClient.httpClient.DefaultRequestHeaders.Add("User-Agent", userAgent);
      GitHubClient.httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearer);
      return await GitHubClient.httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Post, new Uri(gitHubRepoUrl))
      {
        Content = httpContent
      });
    }

    public int GetIssueNumberFromGitHubReponseUri(Uri responseUri)
    {
      if (responseUri != (Uri) null && !string.IsNullOrWhiteSpace(responseUri.AbsolutePath))
      {
        string[] strArray = responseUri.AbsolutePath.Split(new char[1]
        {
          '/'
        }, StringSplitOptions.RemoveEmptyEntries);
        if (strArray != null && strArray.Length != 0)
        {
          string s = strArray[strArray.Length - 1];
          int result;
          if (!string.IsNullOrWhiteSpace(s) && int.TryParse(s, out result))
            return result;
        }
      }
      return 0;
    }
  }
}
