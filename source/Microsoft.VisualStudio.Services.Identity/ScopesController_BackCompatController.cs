// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.ScopesController_BackCompatController
// Assembly: Microsoft.VisualStudio.Services.Identity, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1372DA81-8681-4BFE-8E91-D1AB4333F834
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Identity.dll

using Microsoft.TeamFoundation.Framework.Server;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Identity
{
  [BackCompatJsonFormatter]
  [ControllerApiVersion(0.0)]
  [VersionedApiControllerCustomName(Area = "IMS", ResourceName = "Scopes")]
  public class ScopesController_BackCompatController : IdentitiesControllerBase
  {
    private static readonly Dictionary<Type, HttpStatusCode> s_httpExceptions = new Dictionary<Type, HttpStatusCode>();

    [HttpGet]
    public HttpResponseMessage GetScope(string scopeName) => this.Request.CreateResponse<IdentityScope_BackCompat>(HttpStatusCode.OK, new IdentityScope_BackCompat(this.TfsRequestContext.GetService<IdentityService>().GetScope(this.TfsRequestContext, scopeName)));

    [HttpPut]
    public HttpResponseMessage CreateScope(Guid scopeId, JObject container)
    {
      Guid parentScopeId = container["parentScopeId"].ToObject<Guid>();
      GroupScopeType scopeType = container["scopeType"].ToObject<GroupScopeType>();
      string scopeName = container["scopeName"].ToObject<string>();
      string adminGroupName = container["adminGroupName"].ToObject<string>();
      string adminGroupDescription = container["adminGroupDescription"].ToObject<string>();
      Guid empty = Guid.Empty;
      JToken jtoken = container["creatorId"];
      if (jtoken != null)
        empty = jtoken.ToObject<Guid>();
      return this.Request.CreateResponse<IdentityScope_BackCompat>(HttpStatusCode.Created, new IdentityScope_BackCompat(this.TfsRequestContext.GetService<IdentityService>().CreateScope(this.TfsRequestContext, scopeId, parentScopeId, scopeType, scopeName, adminGroupName, adminGroupDescription, empty)));
    }

    [HttpDelete]
    public HttpResponseMessage DeleteScope(Guid scopeId)
    {
      this.TfsRequestContext.GetService<IdentityService>().DeleteScope(this.TfsRequestContext, scopeId);
      return new HttpResponseMessage()
      {
        StatusCode = HttpStatusCode.OK,
        RequestMessage = this.Request
      };
    }

    [HttpPost]
    public HttpResponseMessage RenameScope(Guid scopeId, JObject container)
    {
      string newName = container["name"].ToObject<string>();
      this.TfsRequestContext.GetService<IdentityService>().RenameScope(this.TfsRequestContext, scopeId, newName);
      return new HttpResponseMessage()
      {
        StatusCode = HttpStatusCode.OK,
        RequestMessage = this.Request
      };
    }

    [HttpGet]
    public HttpResponseMessage GetScope(Guid scopeId) => this.Request.CreateResponse<IdentityScope_BackCompat>(HttpStatusCode.OK, new IdentityScope_BackCompat(this.TfsRequestContext.GetService<IdentityService>().GetScope(this.TfsRequestContext, scopeId)));

    public override IDictionary<Type, HttpStatusCode> HttpExceptions => (IDictionary<Type, HttpStatusCode>) ScopesController_BackCompatController.s_httpExceptions;

    public override string TraceArea => "IdentityService";
  }
}
