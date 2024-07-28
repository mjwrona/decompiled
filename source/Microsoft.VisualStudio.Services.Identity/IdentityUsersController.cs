// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.IdentityUsersController
// Assembly: Microsoft.VisualStudio.Services.Identity, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1372DA81-8681-4BFE-8E91-D1AB4333F834
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Identity.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Security;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Identity
{
  [ControllerApiVersion(0.0)]
  [VersionedApiControllerCustomName(Area = "IMS", ResourceName = "Users")]
  [ClientInternalUseOnly(true)]
  public class IdentityUsersController : IdentitiesControllerBase
  {
    [HttpGet]
    [ClientResponseType(typeof (PagedIdentities), null, null)]
    public HttpResponseMessage ListUsersAsync(
      [FromUri] string continuationToken = null,
      [ClientParameterType(typeof (string), false), FromUri] SubjectDescriptor? scopeDescriptor = null)
    {
      IVssRequestContext vssRequestContext = this.TfsRequestContext.To(TeamFoundationHostType.Deployment);
      vssRequestContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(vssRequestContext, FrameworkSecurity.FrameworkNamespaceId);
      IdentityDescriptor authenticatedDescriptor = this.TfsRequestContext.GetAuthenticatedDescriptor();
      if (!ServicePrincipals.IsServicePrincipal(vssRequestContext, authenticatedDescriptor))
        throw new AccessCheckException(FrameworkResources.InvalidAccessException());
      PlatformIdentityService service = this.TfsRequestContext.GetService<PlatformIdentityService>();
      ScopePagingContext scopePagingContext1 = continuationToken == null ? new ScopePagingContext(this.TfsRequestContext.ServiceHost.InstanceId, PagingHelper.GetPageSize(this.TfsRequestContext), false, true) : ScopePagingContext.FromContinuationToken(continuationToken);
      IVssRequestContext tfsRequestContext = this.TfsRequestContext;
      ScopePagingContext scopePagingContext2 = scopePagingContext1;
      IdentitiesPage identitiesPage = service.ReadIdentitiesByScopeByPage(tfsRequestContext, scopePagingContext2, false);
      HttpResponseMessage response = this.Request.CreateResponse<IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>>(HttpStatusCode.OK, identitiesPage.Identities.Where<Microsoft.VisualStudio.Services.Identity.Identity>((Func<Microsoft.VisualStudio.Services.Identity.Identity, bool>) (x => !x.IsContainer)));
      if (identitiesPage.ContinuationToken != null)
        response.Headers.Add("X-MS-ContinuationToken", identitiesPage.ContinuationToken);
      return response;
    }
  }
}
