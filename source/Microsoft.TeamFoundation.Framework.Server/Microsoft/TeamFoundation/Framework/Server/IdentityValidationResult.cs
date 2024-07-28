// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.IdentityValidationResult
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Diagnostics;
using System.Net;
using System.Web;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class IdentityValidationResult
  {
    public static readonly IdentityValidationResult Success = new IdentityValidationResult(HttpStatusCode.OK, (string) null);

    private IdentityValidationResult(
      HttpStatusCode httpStatusCode,
      string resultMessage,
      Exception exception = null)
    {
      this.HttpStatusCode = httpStatusCode;
      this.ResultMessage = resultMessage;
      this.Exception = exception;
      if (this.IsSuccess)
        return;
      ArgumentUtility.CheckForNull<Exception>(exception, nameof (exception));
    }

    public HttpStatusCode HttpStatusCode { get; set; }

    public string ResultMessage { get; }

    public Exception Exception { get; }

    public bool IsSuccess => this.HttpStatusCode == HttpStatusCode.OK;

    public static IdentityValidationResult Unauthorized(string unauthorizedMessage)
    {
      if (HttpContext.Current?.Items[(object) HttpContextConstants.IVssRequestContext] is IVssRequestContext requestContext)
        requestContext.TraceConditionally(80001, TraceLevel.Info, "IdentityValidationService", nameof (IdentityValidationResult), (Func<string>) (() => unauthorizedMessage + " --- " + EnvironmentWrapper.ToReadableStackTrace()));
      return new IdentityValidationResult(HttpStatusCode.Unauthorized, unauthorizedMessage, (Exception) new UnauthorizedRequestException(unauthorizedMessage, HttpStatusCode.Unauthorized));
    }

    public static IdentityValidationResult Forbidden(string forbiddenMessage) => new IdentityValidationResult(HttpStatusCode.Forbidden, forbiddenMessage, (Exception) new UnauthorizedRequestException(forbiddenMessage, HttpStatusCode.Forbidden));

    public static IdentityValidationResult Error(string errorMessage, Exception exception) => new IdentityValidationResult(HttpStatusCode.InternalServerError, errorMessage, exception);
  }
}
