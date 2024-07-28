// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.IdentityValidationService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server.Authentication;
using Microsoft.TeamFoundation.Framework.Server.Authorization;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.Licensing;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class IdentityValidationService : IIdentityValidationService, IVssFrameworkService
  {
    private static readonly string s_Area = nameof (IdentityValidationService);
    private static readonly string s_Layer = "IVssFrameworkService";
    private const string s_includeMaterializedAadGroupsNotActiveInScopeFF = "VisualStudio.Services.Aad.Expansion.IncludeMaterializedAadGroupsNotActiveInScope";
    private const string s_ignoreMembershipCacheOnRecomputePublicUserFF = "Microsoft.VisualStudio.Services.IdentityValidation.IgnoreMembershipCacheOnRecomputePublicUser";
    private const string c_FeatureDisableUserMaterialization = "VisualStudio.Services.Identity.DisableUserMaterialization";

    public void ServiceStart(IVssRequestContext systemRequestContext) => systemRequestContext.CheckDeploymentRequestContext();

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public IdentityValidationResult ValidateRequestIdentity(IVssRequestContext requestContext)
    {
      requestContext.TraceEnter(0, IdentityValidationService.s_Area, IdentityValidationService.s_Layer, "IdentityValidationHandler.ValidateRequestIdentity");
      try
      {
        requestContext.RequestContextInternal().IdentityValidationStatus |= IdentityValidationStatus.Validated;
        int num = requestContext.RequestRestrictions() != null ? 1 : 0;
        if (num == 0)
          requestContext.Trace(61010, TraceLevel.Warning, IdentityValidationService.s_Area, IdentityValidationService.s_Layer, "Request does not have restrictions.");
        RequiredAuthentication requiredAuthentication = num != 0 ? requestContext.RequestRestrictions().RequiredAuthentication : RequiredAuthentication.ValidatedUser;
        if (requiredAuthentication == RequiredAuthentication.Anonymous)
        {
          SupportsPublicAccess supportsPublicAccess = (SupportsPublicAccess) (3 | (requestContext.IsAnonymous() ? 4 : 0) | (requestContext.IsPublicUser() ? 8 : 0));
          requestContext.RootContext.Items[RequestContextItemsKeys.SupportsPublicAccess] = (object) supportsPublicAccess;
          return IdentityValidationResult.Success;
        }
        IdentityValidationResult validationResult = IdentityValidationService.ValidateUserContext(requestContext);
        if (validationResult.IsSuccess && requiredAuthentication > RequiredAuthentication.ExternalUser)
        {
          if (!AadGuestIdentityAuthenticationValidator.IsValid(requestContext))
            return IdentityValidationResult.Forbidden(FrameworkResources.AadGuestUserNotAllowedInAccount());
          if (requestContext.UserContext.IsCspPartnerIdentityType())
          {
            if (!IdentityValidationService.CanCspPartnerUserAccessHost(requestContext))
            {
              requestContext.Trace(61006, TraceLevel.Info, IdentityValidationService.s_Area, IdentityValidationService.s_Layer, string.Format("Denying CSP identity: {0} to access host: {1}.", (object) requestContext.UserContext, (object) requestContext.ServiceHost));
              return IdentityValidationService.CompleteUnauthorizedRequest(requestContext);
            }
            requestContext.Trace(61005, TraceLevel.Info, IdentityValidationService.s_Area, IdentityValidationService.s_Layer, string.Format("Allowing CSP identity: {0} to access host: {1}.", (object) requestContext.UserContext, (object) requestContext.ServiceHost));
            return validationResult;
          }
          if (!IdentityValidationService.ValidateIdentityGroupMembership(requestContext))
          {
            Microsoft.VisualStudio.Services.Identity.Identity authenticatedIdentity = requestContext.GetUserIdentity();
            if (authenticatedIdentity == null || authenticatedIdentity.Id == Guid.Empty)
            {
              requestContext.TraceConditionally(61004, TraceLevel.Info, IdentityValidationService.s_Area, IdentityValidationService.s_Layer, (Func<string>) (() => string.Format("Identity with descriptor {0} is invalid. Asking for web signin. Requested URL: {1}", (object) requestContext.UserContext, (object) requestContext.RequestUriForTracing())));
              // ISSUE: reference to a compiler-generated field
              // ISSUE: reference to a compiler-generated field
              validationResult = IdentityValidationService.CompleteUnauthorizedRequest(requestContext, IdentityValidationService.\u003C\u003EO.\u003C0\u003E__WebSigninRequiredError ?? (IdentityValidationService.\u003C\u003EO.\u003C0\u003E__WebSigninRequiredError = new Func<string, string>(FrameworkResources.WebSigninRequiredError)));
            }
            else if (IdentityValidationService.IsIdentityGroupMembershipCheckBypassApplicable(requestContext))
            {
              if (!requestContext.IsFeatureEnabled("VisualStudio.Services.Identity.DisableUserMaterialization"))
              {
                requestContext.TraceDataConditionally(358953, TraceLevel.Verbose, IdentityValidationService.s_Area, nameof (ValidateRequestIdentity), "Materializing user ", (Func<object>) (() => (object) new
                {
                  authenticatedIdentity = authenticatedIdentity
                }), nameof (ValidateRequestIdentity));
                IdentityHelper.MaterializeUser(requestContext, (IVssIdentity) authenticatedIdentity, nameof (ValidateRequestIdentity));
              }
              requestContext.TraceAlways(61005, TraceLevel.Info, IdentityValidationService.s_Area, IdentityValidationService.s_Layer, "User {0} was found to have group memberships but no license. Therefore trying to apply on-demand licensing for the user.", (object) authenticatedIdentity.Id);
              LicenseAssignmentHelper.AssignLicenseToIdentity(requestContext, authenticatedIdentity.Id);
              try
              {
                if (requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.IdentityValidation.IgnoreMembershipCacheOnRecomputePublicUser"))
                {
                  requestContext.RootContext.Items[RequestContextItemsKeys.IgnoreMembershipCache] = (object) true;
                  requestContext.RecomputePublicUser();
                }
                else
                  requestContext.RecomputePublicUser();
              }
              finally
              {
                requestContext.RootContext.Items.Remove(RequestContextItemsKeys.IgnoreMembershipCache);
              }
            }
            else
            {
              requestContext.Trace(61004, TraceLevel.Info, IdentityValidationService.s_Area, IdentityValidationService.s_Layer, "Sending 401 Unauthorized for user not in the Everyone group, usually indicating they do not belong to an account. Requested URL: {0}", (object) requestContext.RequestUriForTracing());
              validationResult = IdentityValidationService.CompleteUnauthorizedRequest(requestContext);
            }
          }
        }
        return validationResult;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(61009, IdentityValidationService.s_Area, IdentityValidationService.s_Layer, ex);
        return IdentityValidationResult.Error(FrameworkResources.IdentityValidationHandlerError((object) ex.Message), ex);
      }
      finally
      {
        requestContext.TraceLeave(0, IdentityValidationService.s_Area, IdentityValidationService.s_Layer, "IdentityValidationHandler.ValidateRequestIdentity");
      }
    }

    private static IdentityValidationResult ValidateUserContext(IVssRequestContext requestContext)
    {
      IdentityValidationResult validationResult = IdentityValidationResult.Success;
      if (requestContext.IsAnonymous())
      {
        string str = string.Empty;
        if (requestContext.IsTracing(61003, TraceLevel.Info, IdentityValidationService.s_Area, IdentityValidationService.s_Layer) && requestContext.IsImpersonating)
        {
          IdentityDescriptor authenticatedDescriptor = requestContext.GetAuthenticatedDescriptor();
          str = " - Impersonator: " + (authenticatedDescriptor == (IdentityDescriptor) null ? "(none)" : authenticatedDescriptor.Identifier);
        }
        requestContext.Trace(61003, TraceLevel.Info, IdentityValidationService.s_Area, IdentityValidationService.s_Layer, "Sending 401 Unauthorized for unauthenticated user. Client IP: {0} Requested URL: {1}{2}", (object) requestContext.RemoteIPAddress(), (object) requestContext.RequestUriForTracing(), (object) str);
        validationResult = IdentityValidationService.CompleteUnauthorizedRequest(requestContext);
      }
      return validationResult;
    }

    internal static bool IsIdentityGroupMembershipCheckBypassApplicable(
      IVssRequestContext requestContext)
    {
      bool hostedDeployment = requestContext.ExecutionEnvironment.IsHostedDeployment;
      bool flag1 = requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment);
      if (!hostedDeployment | flag1)
      {
        requestContext.Trace(61011, TraceLevel.Verbose, IdentityValidationService.s_Area, IdentityValidationService.s_Layer, "IsHosted: {0}, isDeploymentHost: {1}.", (object) hostedDeployment, (object) flag1);
        return false;
      }
      Microsoft.VisualStudio.Services.Identity.Identity userIdentity = requestContext.GetUserIdentity();
      bool flag2;
      if (requestContext.RootContext.Items.TryGetValue<bool>(RequestContextItemsKeys.IsOrgUser, out flag2) & flag2)
        return true;
      if (!userIdentity.IsExternalUser)
      {
        requestContext.Trace(61012, TraceLevel.Verbose, IdentityValidationService.s_Area, IdentityValidationService.s_Layer, "User {0} is not an external user.", (object) userIdentity);
        return false;
      }
      if (userIdentity.MemberOf != null && IdentityValidationService.ContainsActiveAadGroupInCurrentScope(requestContext, userIdentity.MemberOf))
      {
        if (requestContext.IsTracing(61013, TraceLevel.Verbose, IdentityValidationService.s_Area, IdentityValidationService.s_Layer))
          requestContext.Trace(61013, TraceLevel.Verbose, IdentityValidationService.s_Area, IdentityValidationService.s_Layer, string.Format("[PreDecoration] User {0}'s group memberships are: {1}.", (object) userIdentity, userIdentity.MemberOf == null ? (object) string.Empty : (object) userIdentity.MemberOf.Serialize<ICollection<IdentityDescriptor>>()));
        return true;
      }
      try
      {
        ITeamFoundationEventService service = requestContext.GetService<ITeamFoundationEventService>();
        IVssRequestContext vssRequestContext = requestContext.Elevate();
        vssRequestContext.Items["RequestContextItemKeyForAadOnDemandLicensing"] = (object) true;
        IVssRequestContext requestContext1 = vssRequestContext;
        AfterCoreReadIdentitiesEvent notificationEvent = new AfterCoreReadIdentitiesEvent()
        {
          ReadIdentities = (IList<Microsoft.VisualStudio.Services.Identity.Identity>) new Microsoft.VisualStudio.Services.Identity.Identity[1]
          {
            userIdentity
          },
          ParentQueryMembership = QueryMembership.Expanded,
          ChildQueryMembership = QueryMembership.None
        };
        service.PublishDecisionPoint(requestContext1, (object) notificationEvent);
        vssRequestContext.Items.Remove("RequestContextItemKeyForAadOnDemandLicensing");
        if (requestContext.IsTracing(61014, TraceLevel.Verbose, IdentityValidationService.s_Area, IdentityValidationService.s_Layer))
          requestContext.Trace(61014, TraceLevel.Verbose, IdentityValidationService.s_Area, IdentityValidationService.s_Layer, string.Format("[PostDecoration] User {0}'s group memberships are: {1}.", (object) userIdentity, userIdentity.MemberOf == null ? (object) string.Empty : (object) userIdentity.MemberOf.Serialize<ICollection<IdentityDescriptor>>()));
        return userIdentity.MemberOf != null && IdentityValidationService.ContainsActiveAadGroupInCurrentScope(requestContext, userIdentity.MemberOf);
      }
      catch (ActionDeniedBySubscriberException ex)
      {
        if (requestContext.IsTracing(61015, TraceLevel.Warning, IdentityValidationService.s_Area, IdentityValidationService.s_Layer))
          requestContext.TraceException(61015, TraceLevel.Warning, IdentityValidationService.s_Area, IdentityValidationService.s_Layer, (Exception) ex);
        return false;
      }
    }

    private static bool ValidateIdentityGroupMembership(IVssRequestContext requestContext)
    {
      if (requestContext.ExecutionEnvironment.IsHostedDeployment && requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment) || requestContext.UserContext.IsSystemServicePrincipalType() || requestContext.UserContext.IsClaimsIdentityType() && ServicePrincipals.IsServicePrincipal(requestContext, requestContext.UserContext))
        return true;
      IdentityValidationService.ValidateIdentityOrgGroupMembership(requestContext);
      IVssRequestContext vssRequestContext = requestContext.Elevate();
      return vssRequestContext.GetService<IdentityService>().IsMember(vssRequestContext, GroupWellKnownIdentityDescriptors.EveryoneGroup, requestContext.UserContext);
    }

    private static void ValidateIdentityOrgGroupMembership(IVssRequestContext requestContext)
    {
      if (requestContext.RootContext.Items.TryGetValue<bool>(RequestContextItemsKeys.IsOrgUser, out bool _))
        return;
      bool flag = IdentityHelper.IsOrgUser(requestContext, requestContext.GetUserIdentity());
      requestContext.RootContext.Items[RequestContextItemsKeys.IsOrgUser] = (object) flag;
      if (!flag)
        return;
      IRequestContextInternal requestContextInternal = requestContext.RequestContextInternal();
      List<IRequestActor> source = new List<IRequestActor>((IEnumerable<IRequestActor>) requestContextInternal.Actors);
      IRequestActor requestActor = source.Last<IRequestActor>();
      source.RemoveAt(source.Count - 1);
      source.Add(RequestActor.CreateRequestActorWithMultipleIdentitySubjects(requestContext, requestActor.Descriptor, requestActor.Identifier, OrgAccessConstants.OrgUserSubject.ToDescriptor()));
      requestContextInternal.ClearActors();
      requestContextInternal.Actors = (IReadOnlyList<IRequestActor>) source;
    }

    private static IdentityValidationResult CompleteUnauthorizedRequest(
      IVssRequestContext requestContext)
    {
      return IdentityValidationService.CompleteUnauthorizedRequest(requestContext, (Func<string, string>) null);
    }

    private static IdentityValidationResult CompleteUnauthorizedRequest(
      IVssRequestContext requestContext,
      Func<string, string> messageFunction)
    {
      IdentityValidationResult validationResult = !(requestContext.UserContext != (IdentityDescriptor) null) ? IdentityValidationResult.Unauthorized(FrameworkResources.AuthenticationRequiredError()) : (messageFunction == null ? IdentityValidationResult.Unauthorized(FrameworkResources.UnauthorizedUserError((object) requestContext.GetUserId())) : IdentityValidationResult.Unauthorized(messageFunction(requestContext.DomainUserName)));
      return requestContext.GetService<ITeamFoundationAuthenticationService>().AuthenticationServiceInternal().CompleteUnauthorizedRequest(requestContext, HttpContextFactory.Current.Response, validationResult, (Uri) null);
    }

    private static bool CanCspPartnerUserAccessHost(IVssRequestContext context)
    {
      if (context.ExecutionEnvironment.IsOnPremisesDeployment)
        return false;
      IVssRequestContext vssRequestContext = context.Elevate();
      return vssRequestContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(vssRequestContext, FrameworkSecurity.FrameworkNamespaceId).HasPermission(context, FrameworkSecurity.FrameworkNamespaceToken, 1);
    }

    private static bool ContainsActiveAadGroupInCurrentScope(
      IVssRequestContext requestContext,
      ICollection<IdentityDescriptor> memberOf)
    {
      bool result = false;
      IdentityDescriptor validUsersGroupDescriptor = IdentityDomain.MapFromWellKnownIdentifier(requestContext.ServiceHost.InstanceId, GroupWellKnownIdentityDescriptors.EveryoneGroup);
      if (!memberOf.IsNullOrEmpty<IdentityDescriptor>())
      {
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        result = memberOf.Any<IdentityDescriptor>(IdentityValidationService.\u003C\u003EO.\u003C1\u003E__IsAadGroup ?? (IdentityValidationService.\u003C\u003EO.\u003C1\u003E__IsAadGroup = new Func<IdentityDescriptor, bool>(AadIdentityHelper.IsAadGroup)));
        if (requestContext.IsFeatureEnabled("VisualStudio.Services.Aad.Expansion.IncludeMaterializedAadGroupsNotActiveInScope"))
          result = result && memberOf.Any<IdentityDescriptor>(new Func<IdentityDescriptor, bool>(validUsersGroupDescriptor.Equals));
      }
      requestContext.TraceConditionally(61016, TraceLevel.Info, IdentityValidationService.s_Area, IdentityValidationService.s_Layer, (Func<string>) (() => string.Format("ContainsActiveAadGroupInCurrentScope:{0}, memberOf:{1}, validUsersGroupDescriptor:{2}", (object) result, memberOf == null ? (object) "null" : (object) memberOf.Serialize<ICollection<IdentityDescriptor>>(), (object) validUsersGroupDescriptor)));
      return result;
    }
  }
}
