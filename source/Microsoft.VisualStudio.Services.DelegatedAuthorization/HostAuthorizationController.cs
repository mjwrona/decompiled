// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.DelegatedAuthorization.HostAuthorizationController
// Assembly: Microsoft.VisualStudio.Services.DelegatedAuthorization, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 76926D67-5A10-414E-AFAB-34A210884CEB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.DelegatedAuthorization.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.DelegatedAuthorization
{
  [ControllerApiVersion(1.0)]
  [VersionedApiControllerCustomName(Area = "DelegatedAuth", ResourceName = "HostAuthorization")]
  public class HostAuthorizationController : DelegatedAuthorizationControllerBase
  {
    private const string Area = "DelegatedAuthorization";
    private const string Layer = "HostAuthorizationController";

    [HttpPost]
    [ClientResponseType(typeof (HostAuthorizationDecision), null, null)]
    public HttpResponseMessage AuthorizeHost(Guid clientId)
    {
      ArgumentUtility.CheckForEmptyGuid(clientId, nameof (clientId));
      try
      {
        HostAuthorizationDecision authorizationDecision = this.TfsRequestContext.GetService<IDelegatedAuthorizationService>().AuthorizeHost(this.TfsRequestContext, clientId);
        if (authorizationDecision.HasError)
        {
          this.TfsRequestContext.Trace(1048090, TraceLevel.Error, "DelegatedAuthorization", nameof (HostAuthorizationController), string.Format("{0} - error creating AuthorizeHost", (object) authorizationDecision.HostAuthorizationError));
          throw new HostAuthorizationCreateException(authorizationDecision.HostAuthorizationError.ToString());
        }
        return this.Request.CreateResponse<HostAuthorizationDecision>(HttpStatusCode.OK, authorizationDecision);
      }
      catch (DelegatedAuthorizationControllerBase.InternalServerErrorException ex)
      {
        return this.Request.CreateResponse<string>(HttpStatusCode.InternalServerError, ex.Message);
      }
    }

    [HttpDelete]
    [ClientResponseType(typeof (void), null, null)]
    public HttpResponseMessage RevokeHostAuthorization(Guid clientId, Guid? hostId = null)
    {
      ArgumentUtility.CheckForEmptyGuid(clientId, nameof (clientId));
      try
      {
        this.TfsRequestContext.GetService<IDelegatedAuthorizationService>().RevokeHostAuthorization(this.TfsRequestContext, clientId, hostId);
        return this.Request.CreateResponse(HttpStatusCode.OK);
      }
      catch (DelegatedAuthorizationControllerBase.InternalServerErrorException ex)
      {
        return this.Request.CreateResponse<string>(HttpStatusCode.InternalServerError, ex.Message);
      }
    }

    [HttpGet]
    [ClientResponseType(typeof (List<HostAuthorization>), null, null)]
    public HttpResponseMessage GetHostAuthorizations(Guid hostId)
    {
      try
      {
        return this.Request.CreateResponse<IList<HostAuthorization>>(HttpStatusCode.OK, this.TfsRequestContext.GetService<IDelegatedAuthorizationService>().GetHostAuthorizations(this.TfsRequestContext, hostId));
      }
      catch (DelegatedAuthorizationControllerBase.InternalServerErrorException ex)
      {
        return this.Request.CreateResponse<string>(HttpStatusCode.InternalServerError, ex.Message);
      }
    }
  }
}
