// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.ConnectedService.Server.ConnectedServiceAuthRequestsController
// Assembly: Microsoft.TeamFoundation.ConnectedService.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 5BEB400C-7A81-4197-B897-D0116BC50257
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.ConnectedService.Server.dll

using Microsoft.Azure.Devops.Teams.Service;
using Microsoft.TeamFoundation.ConnectedService.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Web.Http;

namespace Microsoft.TeamFoundation.ConnectedService.Server
{
  [ControllerApiVersion(1.0)]
  [VersionedApiControllerCustomName(Area = "connectedService", ResourceName = "authRequests")]
  public class ConnectedServiceAuthRequestsController : TfsTeamApiController
  {
    private const int c_tracepoint = 1063120;
    private const string c_area = "Microsoft.TeamFoundation.ConnectedService";
    private const string c_layer = "ConnectedServiceAuthRequestsController";

    [HttpPost]
    public AuthRequest CreateAuthRequest(AuthRequest authRequest, string providerId) => this.CreateAuthRequest(authRequest, providerId, new Guid(), (string) null);

    [HttpPost]
    public AuthRequest CreateAuthRequest(
      AuthRequest authRequest,
      string providerId,
      Guid configurationId)
    {
      return this.CreateAuthRequest(authRequest, providerId, configurationId, (string) null);
    }

    [HttpPost]
    public AuthRequest CreateAuthRequest(
      AuthRequest authRequest,
      string providerId,
      Guid configurationId,
      string scope)
    {
      return this.CreateAuthRequest(authRequest, providerId, configurationId, (string) null, (string) null);
    }

    [HttpPost]
    public AuthRequest CreateAuthRequest(
      AuthRequest authRequest,
      string providerId,
      Guid configurationId,
      string scope,
      string callbackQueryParams)
    {
      return this.CreateAuthRequest(authRequest, providerId, configurationId, scope, callbackQueryParams, (string) null);
    }

    [HttpPost]
    public AuthRequest CreateAuthRequest(
      AuthRequest authRequest,
      string providerId,
      Guid configurationId,
      string scope,
      string callbackQueryParams,
      string endpointType)
    {
      IVssRequestContext tfsRequestContext1 = this.TfsRequestContext;
      providerId = providerId.ToLower(CultureInfo.CurrentCulture);
      try
      {
        tfsRequestContext1.TraceEnter(1063120, "Microsoft.TeamFoundation.ConnectedService", nameof (ConnectedServiceAuthRequestsController), nameof (CreateAuthRequest));
        ConnectedServiceProviderService service = this.TfsRequestContext.GetService<ConnectedServiceProviderService>();
        tfsRequestContext1.Trace(1063120, TraceLevel.Info, "Microsoft.TeamFoundation.ConnectedService", nameof (ConnectedServiceAuthRequestsController), "Get provider '" + providerId + "'");
        IVssRequestContext tfsRequestContext2 = this.TfsRequestContext;
        string providerId1 = providerId;
        return service.GetProvider(tfsRequestContext2, providerId1).CreateAuthRequest(this.TfsRequestContext, this.ProjectId, authRequest, configurationId, endpointType, scope, callbackQueryParams);
      }
      catch (Exception ex)
      {
        tfsRequestContext1.TraceException(1063120, "Microsoft.TeamFoundation.ConnectedService", nameof (ConnectedServiceAuthRequestsController), ex);
        return new AuthRequest()
        {
          ErrorMessage = ex.Message
        };
      }
      finally
      {
        tfsRequestContext1.TraceLeave(1063120, "Microsoft.TeamFoundation.ConnectedService", nameof (ConnectedServiceAuthRequestsController), nameof (CreateAuthRequest));
      }
    }

    [HttpPost]
    public List<Installation> GetAppInstallations(string oauthTokenKey, string providerId)
    {
      IVssRequestContext tfsRequestContext = this.TfsRequestContext;
      providerId = providerId.ToLower(CultureInfo.CurrentCulture);
      try
      {
        tfsRequestContext.TraceEnter(1063120, "Microsoft.TeamFoundation.ConnectedService", nameof (ConnectedServiceAuthRequestsController), nameof (GetAppInstallations));
        ConnectedServiceProviderService service = tfsRequestContext.GetService<ConnectedServiceProviderService>();
        tfsRequestContext.Trace(1063120, TraceLevel.Info, "Microsoft.TeamFoundation.ConnectedService", nameof (ConnectedServiceAuthRequestsController), "Get provider '" + providerId + "'");
        IVssRequestContext requestContext = tfsRequestContext;
        string providerId1 = providerId;
        return service.GetProvider(requestContext, providerId1).GetAppInstallations(tfsRequestContext, this.ProjectId, oauthTokenKey);
      }
      catch (Exception ex)
      {
        tfsRequestContext.TraceException(1063120, "Microsoft.TeamFoundation.ConnectedService", nameof (ConnectedServiceAuthRequestsController), ex);
        return new List<Installation>();
      }
      finally
      {
        tfsRequestContext.TraceLeave(1063120, "Microsoft.TeamFoundation.ConnectedService", nameof (ConnectedServiceAuthRequestsController), nameof (GetAppInstallations));
      }
    }
  }
}
