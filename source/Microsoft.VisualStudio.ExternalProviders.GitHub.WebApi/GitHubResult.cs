// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.ExternalProviders.GitHub.WebApi.GitHubResult
// Assembly: Microsoft.VisualStudio.ExternalProviders.GitHub.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4FE25D33-B783-4B98-BAFC-7E522D8D8D08
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.ExternalProviders.GitHub.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;

namespace Microsoft.VisualStudio.ExternalProviders.GitHub.WebApi
{
  public class GitHubResult
  {
    public GitHubData.V4.RateLimit RateLimit;

    public GitHubResult(
      IReadOnlyList<GitHubError> errors,
      HttpStatusCode httpStatusCode,
      HttpResponseHeaders headers = null)
      : this(errors, httpStatusCode, -1, -1, (string) null, headers)
    {
    }

    public GitHubResult(
      IReadOnlyList<GitHubError> errors,
      HttpStatusCode httpStatusCode,
      GitHubData.V4.PageInfo pageInfo,
      GitHubData.V4.RateLimit rateLimit,
      HttpResponseHeaders headers = null)
      : this(errors, httpStatusCode, -1, -1, (string) null, headers, pageInfo, rateLimit)
    {
    }

    public GitHubResult(
      IReadOnlyList<GitHubError> errors,
      HttpStatusCode httpStatusCode,
      int pageSize,
      int totalPageCount,
      string nextPageToken,
      HttpResponseHeaders headers = null,
      GitHubData.V4.PageInfo pageInfo = null,
      GitHubData.V4.RateLimit rateLimit = null)
    {
      this.Errors = (IReadOnlyList<GitHubError>) ((object) errors ?? (object) Array.Empty<GitHubError>());
      this.StatusCode = httpStatusCode;
      this.ResponseHeaders = headers;
      this.PageSize = pageSize;
      this.TotalPageCount = totalPageCount;
      this.NextPageToken = nextPageToken;
      this.PageInfo = pageInfo;
      this.RateLimit = rateLimit;
      if (GitHubResult.IsSuccessStatusCode(httpStatusCode) || this.Errors.Count != 0)
        return;
      this.Errors = (IReadOnlyList<GitHubError>) new GitHubError[1]
      {
        new GitHubError()
        {
          Message = GitHubResult.ToErrorMessage(httpStatusCode),
          Type = httpStatusCode.ToString()
        }
      };
    }

    public IReadOnlyList<GitHubError> Errors { get; }

    public HttpStatusCode StatusCode { get; }

    public HttpResponseHeaders ResponseHeaders { get; }

    public string ErrorMessage => this.Errors.FirstOrDefault<GitHubError>((Func<GitHubError, bool>) (e => !string.IsNullOrEmpty(e.Message)))?.Message;

    public bool IsSuccessful => this.Errors.Count == 0;

    public string NextPageToken { get; }

    public int TotalPageCount { get; }

    public string ETagValue => this.ResponseHeaders?.ETag?.Tag;

    public bool IsUnchangedConditionalResult => this.StatusCode == HttpStatusCode.NotModified;

    public int PageSize { get; }

    public GitHubData.V4.PageInfo PageInfo { get; }

    public static GitHubResult Error(
      string errorMessage,
      HttpStatusCode httpStatusCode,
      HttpResponseHeaders headers = null)
    {
      return GitHubResult.Error((IReadOnlyList<GitHubError>) new GitHubError[1]
      {
        new GitHubError() { Message = errorMessage }
      }, httpStatusCode, headers);
    }

    public static GitHubResult Error(
      IReadOnlyList<GitHubError> errors,
      HttpStatusCode httpStatusCode,
      HttpResponseHeaders headers = null)
    {
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) errors, nameof (errors));
      return new GitHubResult(errors, httpStatusCode, headers);
    }

    private static bool IsSuccessStatusCode(HttpStatusCode httpStatusCode) => HttpStatusCode.OK <= httpStatusCode && httpStatusCode <= (HttpStatusCode) 299 || httpStatusCode == HttpStatusCode.NotModified;

    private static string ToErrorMessage(HttpStatusCode httpStatusCode) => string.Format("{0} {1}", (object) (int) httpStatusCode, (object) httpStatusCode);
  }
}
