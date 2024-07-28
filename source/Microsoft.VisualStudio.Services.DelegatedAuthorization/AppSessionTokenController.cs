// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.DelegatedAuthorization.AppSessionTokenController
// Assembly: Microsoft.VisualStudio.Services.DelegatedAuthorization, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 76926D67-5A10-414E-AFAB-34A210884CEB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.DelegatedAuthorization.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.DelegatedAuthorization
{
  [ControllerApiVersion(1.0)]
  [VersionedApiControllerCustomName(Area = "Token", ResourceName = "AppSessionTokens")]
  public class AppSessionTokenController : DelegatedAuthorizationControllerBase
  {
    private const string Area = "DelegatedAuthorization";
    private const string Layer = "AppSessionTokenController";

    [HttpPost]
    [ClientResponseType(typeof (AppSessionTokenResult), null, null)]
    public HttpResponseMessage IssueAppSessionToken(Guid clientId, Guid? userId = null)
    {
      ArgumentUtility.CheckForEmptyGuid(clientId, nameof (clientId));
      if (!userId.HasValue)
      {
        userId = new Guid?(this.TfsRequestContext.GetUserId());
      }
      else
      {
        Guid? nullable = userId;
        Guid empty = Guid.Empty;
        if ((nullable.HasValue ? (nullable.HasValue ? (nullable.GetValueOrDefault() == empty ? 1 : 0) : 1) : 0) != 0)
          throw new ArgumentException("userId is not a valid Guid");
      }
      try
      {
        AppSessionTokenResult sessionTokenResult = this.TfsRequestContext.GetService<IDelegatedAuthorizationService>().IssueAppSessionToken(this.TfsRequestContext, clientId, userId);
        if (sessionTokenResult.HasError)
        {
          this.TfsRequestContext.Trace(1049025, TraceLevel.Error, "DelegatedAuthorization", nameof (AppSessionTokenController), string.Format("{0} - error creating application token.", (object) sessionTokenResult.AppSessionTokenError));
          throw new AppSessionTokenCreateException(sessionTokenResult.AppSessionTokenError.ToString());
        }
        return this.Request.CreateResponse<AppSessionTokenResult>(HttpStatusCode.OK, sessionTokenResult);
      }
      catch (DelegatedAuthorizationControllerBase.InternalServerErrorException ex)
      {
        return this.Request.CreateResponse<string>(HttpStatusCode.InternalServerError, ex.Message);
      }
    }
  }
}
