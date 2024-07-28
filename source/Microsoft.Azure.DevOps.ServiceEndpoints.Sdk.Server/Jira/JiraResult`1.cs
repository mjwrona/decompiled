// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.Jira.JiraResult`1
// Assembly: Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 002C83BC-B53E-470A-8038-76E47B5E5BF3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Net;

namespace Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.Jira
{
  public sealed class JiraResult<T> : JiraResult
  {
    private JiraResult(T result, string errorMessage, HttpStatusCode httpStatusCode, bool success)
      : base(errorMessage, httpStatusCode, success)
    {
      this.Result = result;
    }

    public T Result { get; }

    public static JiraResult<T> Error(string errorMessage, HttpStatusCode httpStatusCode)
    {
      ArgumentUtility.CheckForNull<string>(errorMessage, nameof (errorMessage));
      return new JiraResult<T>(default (T), errorMessage, httpStatusCode, false);
    }

    public static JiraResult<T> Success(T result, HttpStatusCode httpStatusCode) => new JiraResult<T>(result, (string) null, httpStatusCode, true);

    public JiraResult<U> ConvertError<U>()
    {
      if (this.IsSuccessful)
        throw new InvalidOperationException("Cannot use JiraResult.ConvertError on a successful result!");
      return JiraResult<U>.Error(this.ErrorMessage, this.StatusCode);
    }

    public JiraResult<U> Convert<U>(Func<T, U> convert)
    {
      ArgumentUtility.CheckForNull<Func<T, U>>(convert, nameof (convert));
      return this.IsSuccessful ? JiraResult<U>.Success(convert(this.Result), this.StatusCode) : this.ConvertError<U>();
    }
  }
}
