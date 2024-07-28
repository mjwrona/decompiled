// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.ScopesBaseController
// Assembly: Microsoft.VisualStudio.Services.Identity, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1372DA81-8681-4BFE-8E91-D1AB4333F834
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Identity.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Identity
{
  public abstract class ScopesBaseController : IdentitiesControllerBase
  {
    [HttpGet]
    [ClientResponseType(typeof (IdentityScope), null, null)]
    public HttpResponseMessage GetScopeByName(string scopeName) => this.Request.CreateResponse<IdentityScope>(HttpStatusCode.OK, this.TfsRequestContext.GetService<IdentityService>().GetScope(this.TfsRequestContext, scopeName));

    [HttpPut]
    [ClientResponseType(typeof (IdentityScope), null, null)]
    public HttpResponseMessage CreateScope(Guid scopeId, CreateScopeInfo info)
    {
      Guid parentScopeId = info.ParentScopeId;
      GroupScopeType scopeType = info.ScopeType;
      string scopeName = info.ScopeName;
      string adminGroupName = info.AdminGroupName;
      string groupDescription = info.AdminGroupDescription;
      Guid creatorId = info.CreatorId;
      return this.Request.CreateResponse<IdentityScope>(HttpStatusCode.Created, this.TfsRequestContext.GetService<IdentityService>().CreateScope(this.TfsRequestContext, scopeId, parentScopeId, scopeType, scopeName, adminGroupName, groupDescription, creatorId));
    }

    [HttpDelete]
    [ClientResponseType(typeof (void), null, null)]
    public HttpResponseMessage DeleteScope(Guid scopeId)
    {
      this.TfsRequestContext.GetService<IdentityService>().DeleteScope(this.TfsRequestContext, scopeId);
      return new HttpResponseMessage()
      {
        StatusCode = HttpStatusCode.OK,
        RequestMessage = this.Request
      };
    }

    [HttpGet]
    [ClientResponseType(typeof (IdentityScope), null, null)]
    public HttpResponseMessage GetScopeById(Guid scopeId) => this.Request.CreateResponse<IdentityScope>(HttpStatusCode.OK, this.TfsRequestContext.GetService<IdentityService>().GetScope(this.TfsRequestContext, scopeId));

    public override IDictionary<Type, HttpStatusCode> HttpExceptions => (IDictionary<Type, HttpStatusCode>) IdentitiesController.s_httpIdentityServiceExceptions;

    public override string TraceArea => "IdentityService";
  }
}
