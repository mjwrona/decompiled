// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.Jira.JiraResult
// Assembly: Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 002C83BC-B53E-470A-8038-76E47B5E5BF3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.dll

using Microsoft.VisualStudio.Services.Common;
using System.Net;

namespace Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.Jira
{
  public class JiraResult
  {
    protected JiraResult(string errorMessage, HttpStatusCode httpStatusCode, bool success)
    {
      this.ErrorMessage = errorMessage;
      this.StatusCode = httpStatusCode;
      this.IsSuccessful = success;
    }

    public HttpStatusCode StatusCode { get; }

    public string ErrorMessage { get; }

    public bool IsSuccessful { get; }

    public static JiraResult Error(string errorMessage, HttpStatusCode httpStatusCode)
    {
      ArgumentUtility.CheckForNull<string>(errorMessage, nameof (errorMessage));
      return new JiraResult(errorMessage, httpStatusCode, false);
    }

    public static JiraResult Success(HttpStatusCode httpStatusCode) => new JiraResult((string) null, httpStatusCode, true);
  }
}
