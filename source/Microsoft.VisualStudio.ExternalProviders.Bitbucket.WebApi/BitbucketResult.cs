// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.ExternalProviders.Bitbucket.WebApi.BitbucketResult
// Assembly: Microsoft.VisualStudio.ExternalProviders.Bitbucket.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 223C9BE7-A3E9-431B-86B7-A81B8A6447FF
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.ExternalProviders.Bitbucket.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using System.Net;

namespace Microsoft.VisualStudio.ExternalProviders.Bitbucket.WebApi
{
  public class BitbucketResult
  {
    protected BitbucketResult(string errorMessage, HttpStatusCode httpStatusCode, bool success)
    {
      this.ErrorMessage = errorMessage;
      this.StatusCode = httpStatusCode;
      this.IsSuccessful = success;
    }

    public HttpStatusCode StatusCode { get; }

    public string ErrorMessage { get; }

    public bool IsSuccessful { get; }

    public static BitbucketResult Error(string errorMessage, HttpStatusCode httpStatusCode)
    {
      ArgumentUtility.CheckForNull<string>(errorMessage, nameof (errorMessage));
      return new BitbucketResult(errorMessage, httpStatusCode, false);
    }

    public static BitbucketResult Success(HttpStatusCode httpStatusCode) => new BitbucketResult((string) null, httpStatusCode, true);
  }
}
