// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.ExternalProviders.Bitbucket.WebApi.BitbucketResult`1
// Assembly: Microsoft.VisualStudio.ExternalProviders.Bitbucket.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 223C9BE7-A3E9-431B-86B7-A81B8A6447FF
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.ExternalProviders.Bitbucket.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Net;

namespace Microsoft.VisualStudio.ExternalProviders.Bitbucket.WebApi
{
  public sealed class BitbucketResult<T> : BitbucketResult
  {
    private BitbucketResult(
      T result,
      string errorMessage,
      HttpStatusCode httpStatusCode,
      bool success)
      : base(errorMessage, httpStatusCode, success)
    {
      this.Result = result;
    }

    public T Result { get; }

    public static BitbucketResult<T> Error(string errorMessage, HttpStatusCode httpStatusCode)
    {
      ArgumentUtility.CheckForNull<string>(errorMessage, nameof (errorMessage));
      return new BitbucketResult<T>(default (T), errorMessage, httpStatusCode, false);
    }

    public static BitbucketResult<T> Success(T result, HttpStatusCode httpStatusCode) => new BitbucketResult<T>(result, (string) null, httpStatusCode, true);

    public BitbucketResult<U> ConvertError<U>()
    {
      if (this.IsSuccessful)
        throw new InvalidOperationException("Cannot use BitbucketResult.ConvertError on a successful result!");
      return BitbucketResult<U>.Error(this.ErrorMessage, this.StatusCode);
    }

    public BitbucketResult<U> Convert<U>(Func<T, U> convert)
    {
      ArgumentUtility.CheckForNull<Func<T, U>>(convert, nameof (convert));
      return this.IsSuccessful ? BitbucketResult<U>.Success(convert(this.Result), this.StatusCode) : this.ConvertError<U>();
    }

    public BitbucketResult<U> Then<U>(Func<T, BitbucketResult<U>> callBitbucket)
    {
      ArgumentUtility.CheckForNull<Func<T, BitbucketResult<U>>>(callBitbucket, nameof (callBitbucket));
      return this.IsSuccessful ? callBitbucket(this.Result) : this.ConvertError<U>();
    }
  }
}
