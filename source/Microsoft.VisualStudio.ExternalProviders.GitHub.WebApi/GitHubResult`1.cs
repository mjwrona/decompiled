// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.ExternalProviders.GitHub.WebApi.GitHubResult`1
// Assembly: Microsoft.VisualStudio.ExternalProviders.GitHub.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4FE25D33-B783-4B98-BAFC-7E522D8D8D08
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.ExternalProviders.GitHub.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Http.Headers;

namespace Microsoft.VisualStudio.ExternalProviders.GitHub.WebApi
{
  public class GitHubResult<T> : GitHubResult
  {
    public GitHubResult(
      T result,
      IReadOnlyList<GitHubError> errors,
      HttpStatusCode httpStatusCode,
      HttpResponseHeaders headers = null)
      : base(errors, httpStatusCode, headers)
    {
      this.Result = result;
    }

    public GitHubResult(
      T result,
      IReadOnlyList<GitHubError> errors,
      HttpStatusCode httpStatusCode,
      GitHubData.V4.PageInfo pageInfo,
      GitHubData.V4.RateLimit rateLimit,
      HttpResponseHeaders headers = null)
      : base(errors, httpStatusCode, pageInfo, rateLimit, headers)
    {
      this.Result = result;
    }

    public GitHubResult(
      T result,
      IReadOnlyList<GitHubError> errors,
      HttpStatusCode httpStatusCode,
      int pageSize,
      int totalPageCount,
      string nextPageToken,
      HttpResponseHeaders headers = null)
      : base(errors, httpStatusCode, pageSize, totalPageCount, nextPageToken, headers)
    {
      this.Result = result;
    }

    public T Result { get; }

    public static GitHubResult<T> Error(
      string errorMessage,
      HttpStatusCode httpStatusCode,
      HttpResponseHeaders headers = null)
    {
      return GitHubResult<T>.Error((IReadOnlyList<GitHubError>) new GitHubError[1]
      {
        new GitHubError() { Message = errorMessage }
      }, httpStatusCode, headers);
    }

    public static GitHubResult<T> Error(
      IReadOnlyList<GitHubError> errors,
      HttpStatusCode httpStatusCode,
      HttpResponseHeaders headers = null)
    {
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) errors, nameof (errors));
      return new GitHubResult<T>(default (T), errors, httpStatusCode, headers);
    }

    public static GitHubResult<T> Success(
      T result,
      HttpStatusCode httpStatusCode,
      HttpResponseHeaders headers = null)
    {
      return new GitHubResult<T>(result, (IReadOnlyList<GitHubError>) null, httpStatusCode, headers);
    }

    public static GitHubResult<T> Success(
      T result,
      HttpStatusCode httpStatusCode,
      int pageSize,
      int totalPageCount,
      string nextPageToken,
      HttpResponseHeaders headers = null)
    {
      return new GitHubResult<T>(result, (IReadOnlyList<GitHubError>) null, httpStatusCode, pageSize, totalPageCount, nextPageToken, headers);
    }

    public static GitHubResult<T> PartialSuccess(
      T result,
      HttpStatusCode httpStatusCode,
      IReadOnlyList<GitHubError> errors,
      HttpResponseHeaders headers = null)
    {
      ArgumentUtility.CheckForDefault<T>(result, nameof (result));
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) errors, nameof (errors));
      return new GitHubResult<T>(result, errors, httpStatusCode, headers);
    }

    public GitHubResult<U> Convert<U>() => GitHubResult<U>.Error(this.Errors, this.StatusCode, this.ResponseHeaders);

    public GitHubResult<U> Convert<U>(Func<T, U> convert, bool convertOnPartialSuccess = false)
    {
      if (this.IsUnchangedConditionalResult)
        return GitHubResult<U>.Success(default (U), this.StatusCode, this.PageSize, this.TotalPageCount, this.NextPageToken, this.ResponseHeaders);
      if (this.IsSuccessful)
        return GitHubResult<U>.Success(convert(this.Result), this.StatusCode, this.PageSize, this.TotalPageCount, this.NextPageToken, this.ResponseHeaders);
      return convertOnPartialSuccess && (object) this.Result != null ? GitHubResult<U>.PartialSuccess(convert(this.Result), this.StatusCode, this.Errors, this.ResponseHeaders) : this.Convert<U>();
    }
  }
}
