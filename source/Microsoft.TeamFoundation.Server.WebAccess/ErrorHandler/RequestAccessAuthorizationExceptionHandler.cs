// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.ErrorHandler.RequestAccessAuthorizationExceptionHandler
// Assembly: Microsoft.TeamFoundation.Server.WebAccess, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A2CCA8C5-6910-48A5-82D9-BDC1350B5B4D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.TeamFoundation.WebPlatform.Utils;
using Microsoft.VisualStudio.Services.DelegatedAuthorization;
using Microsoft.VisualStudio.Services.Location;
using Microsoft.VisualStudio.Services.Location.Server;
using Microsoft.VisualStudio.Services.Organization;
using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server;
using Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server.Contributions;
using Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server.Error;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Microsoft.TeamFoundation.Server.WebAccess.ErrorHandler
{
  internal class RequestAccessAuthorizationExceptionHandler : IErrorPageHandler
  {
    private readonly string EnableRequestAccessFeatureFlag = "VisualStudio.Services.WebAccess.EnableRequestAccess";
    private readonly string TraceArea = "ErrorPageHandler";
    private readonly string TraceLayer = nameof (RequestAccessAuthorizationExceptionHandler);

    public void HandleError(IVssRequestContext requestContext, ErrorContext context)
    {
      if (context == null || !context.IsHosted || context.Exception == null || context.StatusCode != 401 && context.StatusCode != 403 || requestContext == null)
        return;
      bool flag = this.IsEnableRequestAccessFeatureEnabled(requestContext);
      Microsoft.VisualStudio.Services.Identity.Identity userIdentity = requestContext.GetUserIdentity();
      if (userIdentity == null || !userIdentity.IsExternalUser || !this.IsUserInSameTenant(requestContext, userIdentity))
        return;
      if (OrganizationTakeoverHelpers.CanCurrentUserTakeOverOrg(requestContext))
      {
        OrganizationTakeOverData organizationTakeOverData = new OrganizationTakeOverData();
        organizationTakeOverData.OrganizationTakeOverEndPoint = this.GetForceUpdateOwnerEndPoint(requestContext);
        organizationTakeOverData.OrganizationSettingsEndPoint = this.GetOrganizationSettingsEndPoint(requestContext);
        organizationTakeOverData.UserSessionToken = this.GetUserSessionToken(requestContext);
        organizationTakeOverData.NewOwnerId = userIdentity.Id;
        context.PrimaryAction = new ErrorAction();
        context.PrimaryAction.ContentContributionIds.Add("ms.vss-tfs-web.organization-takeover-content");
        context.PrimaryAction.ActionData.Add("OrganizationTakeOverData", (object) organizationTakeOverData);
        requestContext.TraceAlways(11997, TraceLevel.Info, this.TraceArea, this.TraceLayer, string.Format("Claim ownership button showed for user {0}", (object) userIdentity.Id));
      }
      else
      {
        if (!flag)
          return;
        AuthExceptionData authExceptionData = new AuthExceptionData();
        authExceptionData.EmailAddress = ErrorUtilities.GetUserEmail(requestContext);
        authExceptionData.OrganizationName = requestContext.ServiceHost.Name;
        authExceptionData.RequestAccessEndpoint = this.GetRequestAccessEndpoint(requestContext);
        authExceptionData.UserSessionToken = this.GetUserSessionToken(requestContext);
        authExceptionData.UrlRequested = requestContext.RequestUri().AbsoluteUri;
        authExceptionData.ProjectUri = this.GetProjectUri(requestContext);
        authExceptionData.IsPolicyEnabled = this.IsRequestAccessPolicyEnabled(requestContext);
        authExceptionData.RequestAccessUrl = this.GetOrganizationRequestAccessUrl(requestContext);
        context.PrimaryAction = new ErrorAction();
        context.PrimaryAction.ContentContributionIds.Add("ms.vss-tfs-web.authorization-error-content");
        context.PrimaryAction.ActionData.Add("AuthExceptionData", (object) authExceptionData);
        requestContext.Trace(11996, TraceLevel.Info, this.TraceArea, this.TraceLayer, string.Format("Request access button showed for user {0}", (object) userIdentity.Id));
      }
    }

    private bool IsEnableRequestAccessFeatureEnabled(IVssRequestContext requestContext) => requestContext.IsFeatureEnabled(this.EnableRequestAccessFeatureFlag);

    private bool IsRequestAccessPolicyEnabled(IVssRequestContext requestContext)
    {
      if (requestContext.GetService<IOrganizationPolicyService>().GetPolicy<bool>(requestContext.Elevate(), "Policy.AllowRequestAccessToken", true).EffectiveValue)
        return true;
      requestContext.Trace(12001, TraceLevel.Info, this.TraceArea, this.TraceLayer, "Request access policy validation enterprise policy was not enabled.");
      return false;
    }

    private string GetOrganizationRequestAccessUrl(IVssRequestContext requestContext)
    {
      string empty = string.Empty;
      if (requestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection))
        empty = requestContext.GetService<ICollectionService>().GetCollection(requestContext.Elevate(), (IEnumerable<string>) new string[1]
        {
          "SystemProperty.RequestAccessUrl"
        }).Properties.GetValue<string>("SystemProperty.RequestAccessUrl", string.Empty);
      return empty;
    }

    private string GetProjectUri(IVssRequestContext requestContext)
    {
      IContributionRoutingService service = requestContext.GetService<IContributionRoutingService>();
      string projectName = (string) null;
      try
      {
        projectName = service.GetRouteValue<string>(requestContext, "project");
      }
      catch (Exception ex)
      {
      }
      if (projectName == null)
        return (string) null;
      return requestContext.GetService<IProjectService>().GetProject(requestContext.Elevate(), projectName)?.Uri;
    }

    private string GetUserSessionToken(IVssRequestContext requestContext)
    {
      IVssRequestContext context = requestContext.To(TeamFoundationHostType.Deployment);
      IDelegatedAuthorizationService service = context.GetService<IDelegatedAuthorizationService>();
      try
      {
        IDelegatedAuthorizationService authorizationService = service;
        IVssRequestContext requestContext1 = context;
        DateTime? nullable = new DateTime?(DateTime.UtcNow.AddHours(1.0));
        Guid? clientId = new Guid?();
        Guid? userId = new Guid?();
        DateTime? validTo = nullable;
        Guid? authorizationId = new Guid?();
        Guid? accessId = new Guid?();
        return authorizationService.IssueSessionToken(requestContext1, clientId, userId, "sessiontoken-requestaccess", validTo, authorizationId: authorizationId, accessId: accessId).SessionToken.Token;
      }
      catch (Exception ex)
      {
        requestContext.Trace(11990, TraceLevel.Error, this.TraceArea, this.TraceLayer, "Unable get session token for user");
        requestContext.TraceException(11990, this.TraceArea, this.TraceLayer, ex);
        throw;
      }
    }

    private string GetRequestAccessEndpoint(IVssRequestContext requestContext) => this.GetSpsOrgUrl(requestContext) + "_apis/Graph/RequestAccess";

    private string GetSpsOrgUrl(IVssRequestContext requestContext) => requestContext.GetService<ILocationService>().GetLocationServiceUrl(requestContext, ServiceInstanceTypes.SPS, AccessMappingConstants.PublicAccessMappingMoniker);

    private bool IsUserInSameTenant(IVssRequestContext requestContext, Microsoft.VisualStudio.Services.Identity.Identity requestIdentity)
    {
      Guid property = requestIdentity.GetProperty<Guid>("Domain", Guid.Empty);
      Guid organizationAadTenantId = requestContext.GetOrganizationAadTenantId();
      return !property.Equals(Guid.Empty) && property.Equals(organizationAadTenantId);
    }

    private string GetForceUpdateOwnerEndPoint(IVssRequestContext requestContext) => string.Format("{0}_apis/Organization/Collections/{1}", (object) this.GetSpsOrgUrl(requestContext), (object) requestContext.ServiceHost.InstanceId);

    private string GetOrganizationSettingsEndPoint(IVssRequestContext requestContext)
    {
      string accessMappingMoniker = requestContext.UseDevOpsDomainUrls() ? AccessMappingConstants.DevOpsAccessMapping : AccessMappingConstants.VstsAccessMapping;
      return requestContext.GetService<ILocationService>().GetLocationServiceUrl(requestContext, ServiceInstanceTypes.TFS, accessMappingMoniker) + "_settings/";
    }
  }
}
