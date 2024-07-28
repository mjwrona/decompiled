// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Graph.UserMapperController
// Assembly: Microsoft.VisualStudio.Services.Graph, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 00390AA0-D8BB-45EB-AEF5-70DC8BFC765D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Graph.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.ConfigFramework;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Graph.Client;
using Microsoft.VisualStudio.Services.Graph.Services;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Graph
{
  [ControllerApiVersion(7.1)]
  [VersionedApiControllerCustomName(Area = "Graph", ResourceName = "ResolveDisconnectedUsers")]
  public class UserMapperController : GraphControllerBase
  {
    private static readonly IConfigPrototype<bool> ResolveDisconnectedUsersApiEnabledConfigPrototype = ConfigPrototype.Create<bool>("Identity.ResolveDisconnectedUsersApiAvailable.M211", false);
    private readonly IConfigQueryable<bool> ResolveDisconnectedUsersApiEnabledConfig;
    private const string resolveDisconnectedUsersLimitRegistryPath = "/Configuration/Service/Graph/ResolveDisconnectedUsersApiLimit";

    public UserMapperController()
      : this(ConfigProxy.Create<bool>(UserMapperController.ResolveDisconnectedUsersApiEnabledConfigPrototype))
    {
    }

    public UserMapperController(
      IConfigQueryable<bool> resolveDisconnectedUsersApiEnabledConfig)
    {
      this.ResolveDisconnectedUsersApiEnabledConfig = resolveDisconnectedUsersApiEnabledConfig;
    }

    [HttpPost]
    [ClientInternalUseOnly(true)]
    [ClientResponseType(typeof (ResolveDisconnectedUsersResponse), null, null)]
    public HttpResponseMessage Resolve([FromBody] IdentityMappings mappings)
    {
      this.CheckPermissionsToTransferIdentity(this.TfsRequestContext);
      this.CheckIfResolveDisconnectedUsersApiEnabled(this.TfsRequestContext);
      int num = this.TfsRequestContext.GetService<IVssRegistryService>().GetValue<int>(this.TfsRequestContext, (RegistryQuery) "/Configuration/Service/Graph/ResolveDisconnectedUsersApiLimit", 20);
      if (mappings == null || mappings.Mappings == null || !mappings.Mappings.Any<IdentityMapping>())
        return this.Request.CreateResponse<ResolveDisconnectedUsersResponse>(HttpStatusCode.BadRequest, new ResolveDisconnectedUsersResponse(new HttpStatusCode?(HttpStatusCode.BadRequest), "No identities to be mapped"));
      if (mappings.Mappings.Count > num)
        return this.Request.CreateResponse<ResolveDisconnectedUsersResponse>(HttpStatusCode.BadRequest, new ResolveDisconnectedUsersResponse(new HttpStatusCode?(HttpStatusCode.BadRequest), string.Format("The maximum limit of identities to be mapped is {0}", (object) num)));
      IdentityService service1 = this.TfsRequestContext.GetService<IdentityService>();
      GraphUsersService service2 = this.TfsRequestContext.GetService<GraphUsersService>();
      ResolveDisconnectedUsersResponse disconnectedUsersResponse = new ResolveDisconnectedUsersResponse(new HttpStatusCode?(HttpStatusCode.OK));
      foreach (IdentityMapping mapping in mappings.Mappings)
      {
        try
        {
          if (string.IsNullOrWhiteSpace(mapping.Source?.PrincipalName))
            disconnectedUsersResponse.AddMappingResult(new MappingResult(new HttpStatusCode?(HttpStatusCode.BadRequest), "Request format error; Source identity not specified in the request body"));
          else if (string.IsNullOrWhiteSpace(mapping.Target?.PrincipalName))
          {
            disconnectedUsersResponse.AddMappingResult(new MappingResult(new HttpStatusCode?(HttpStatusCode.BadRequest), "Request format error; Target identity not specified in the request body"));
          }
          else
          {
            string principalName1 = mapping.Source.PrincipalName;
            string principalName2 = mapping.Target.PrincipalName;
            Microsoft.VisualStudio.Services.Identity.Identity currentIdentity = service1.ReadIdentities(this.TfsRequestContext, IdentitySearchFilter.MailAddress, principalName1, QueryMembership.None, (IEnumerable<string>) null).SingleOrDefault<Microsoft.VisualStudio.Services.Identity.Identity>();
            if (currentIdentity == null)
              disconnectedUsersResponse.AddMappingResult(new MappingResult(new HttpStatusCode?(HttpStatusCode.BadRequest), "Identity " + principalName1 + " not found"));
            else
              service2.UpdateUserInternal(this.TfsRequestContext, new GraphUserPrincipalNameUpdateContext(principalName2), currentIdentity);
          }
        }
        catch (Exception ex)
        {
          disconnectedUsersResponse.AddMappingResult(new MappingResult(new HttpStatusCode?(HttpStatusCode.BadRequest), "Failed mapping: source = " + mapping.Source?.PrincipalName + ", target = " + mapping.Target?.PrincipalName));
        }
      }
      return this.Request.CreateResponse<ResolveDisconnectedUsersResponse>(HttpStatusCode.OK, disconnectedUsersResponse);
    }

    private void CheckIfResolveDisconnectedUsersApiEnabled(IVssRequestContext requestContext)
    {
      if (!this.ResolveDisconnectedUsersApiEnabledConfig.QueryByCtx<bool>(requestContext))
        throw new VssUnauthorizedException("You are not authorized to access this api.");
    }

    private void CheckPermissionsToTransferIdentity(IVssRequestContext requestContext)
    {
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Application);
      vssRequestContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(vssRequestContext, FrameworkSecurity.IdentitiesNamespaceId).CheckPermission(vssRequestContext, requestContext.ServiceHost.InstanceId.ToString(), 31);
    }
  }
}
