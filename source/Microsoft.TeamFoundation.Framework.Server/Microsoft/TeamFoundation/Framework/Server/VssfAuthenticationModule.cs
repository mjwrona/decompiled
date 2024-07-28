// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.VssfAuthenticationModule
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server.Authentication;
using Microsoft.TeamFoundation.Framework.Server.Authorization;
using Microsoft.TeamFoundation.Framework.Server.Serialization;
using Microsoft.VisualStudio.Services.Authentication;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.Organization;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Security.Principal;
using System.Web;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class VssfAuthenticationModule : IHttpModule
  {
    private readonly IAuthenticationAudit m_authenticationAudit = (IAuthenticationAudit) new AuthenticationAudit((IEventSerializer) new EventSerializer());
    private static readonly string s_Area = nameof (VssfAuthenticationModule);
    private static readonly string s_Layer = "IHttpModule";

    public void Init(HttpApplication context)
    {
      context.AuthenticateRequest += this.AuthEventHandler(new Action<AspNetRequestContext, IHttpApplication>(this.OnAuthenticateRequest));
      context.PostAuthenticateRequest += this.AuthEventHandler(new Action<AspNetRequestContext, IHttpApplication>(this.OnPostAuthenticateRequest));
      context.PostAuthorizeRequest += this.AuthEventHandler(new Action<AspNetRequestContext, IHttpApplication>(this.OnPostAuthorizeRequest));
    }

    public void Dispose()
    {
    }

    private EventHandler AuthEventHandler(
      Action<AspNetRequestContext, IHttpApplication> handler)
    {
      return (EventHandler) ((sender, e) =>
      {
        if (!TeamFoundationApplicationCore.DeploymentServiceHost.HasDatabaseAccess)
          return;
        HttpApplicationWrapper applicationWrapper = new HttpApplicationWrapper((HttpApplication) sender);
        if (!(applicationWrapper.Context.Items[(object) HttpContextConstants.IVssRequestContext] is AspNetRequestContext netRequestContext2))
          return;
        handler(netRequestContext2, (IHttpApplication) applicationWrapper);
      });
    }

    private void OnAuthenticateRequest(
      AspNetRequestContext requestContext,
      IHttpApplication application)
    {
      requestContext.TraceEnter(209338, VssfAuthenticationModule.s_Area, VssfAuthenticationModule.s_Layer, "VssfAuthenticationModule.OnAuthenticateRequest");
      try
      {
        requestContext.RootContext.Items[RequestContextItemsKeys.IsProcessingAuthenticationModules] = (object) true;
        SessionCookieHelper.GetSessionCookieProcessor((IVssRequestContext) requestContext).EnsureSessionCookieExists((IVssRequestContext) requestContext);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(921500, VssfAuthenticationModule.s_Area, VssfAuthenticationModule.s_Layer, ex);
        TeamFoundationApplicationCore.CompleteRequest((IVssRequestContext) requestContext, application, HttpStatusCode.InternalServerError, ex);
      }
      finally
      {
        requestContext.TraceLeave(846237, VssfAuthenticationModule.s_Area, VssfAuthenticationModule.s_Layer, "VssfAuthenticationModule.OnAuthenticateRequest");
      }
    }

    private void OnPostAuthenticateRequest(
      AspNetRequestContext requestContext,
      IHttpApplication application)
    {
      requestContext.TraceEnter(591877, VssfAuthenticationModule.s_Area, VssfAuthenticationModule.s_Layer, "VssfAuthenticationModule.OnPostAuthenticateRequest");
      try
      {
        HttpContextBase context = application.Context;
        requestContext.RootContext.Items.Remove(RequestContextItemsKeys.IsProcessingAuthenticationModules);
        Microsoft.VisualStudio.Services.Identity.Identity userContextIdentity = (Microsoft.VisualStudio.Services.Identity.Identity) null;
        bool flag = requestContext.IsFeatureEnabled("VisualStudio.Services.DelayedIdentityValidation");
        if (context.User != null && context.User.Identity.IsAuthenticated)
        {
          if (requestContext.IsTracing(437394, TraceLevel.Verbose, VssfAuthenticationModule.s_Area, VssfAuthenticationModule.s_Layer))
            requestContext.Trace(437394, TraceLevel.Verbose, VssfAuthenticationModule.s_Area, VssfAuthenticationModule.s_Layer, "User {0} is authenticated.", (object) context.User.Identity.Name);
          this.IsExternalUserResolutionRequired(requestContext, context.User.Identity as ClaimsIdentity);
          IIdentityAuthenticationService service = requestContext.GetService<IIdentityAuthenticationService>();
          ITeamFoundationAuthenticationServiceInternal authenticationServiceInternal = requestContext.GetService<ITeamFoundationAuthenticationService>().AuthenticationServiceInternal();
          AspNetRequestContext requestContext1 = requestContext;
          IIdentity identity1 = context.User.Identity;
          Microsoft.VisualStudio.Services.Identity.Identity identity2 = service.ResolveIdentity((IVssRequestContext) requestContext1, identity1);
          this.PreValidateRequestAuthentication((IVssRequestContext) requestContext, (ITeamFoundationAuthenticationService) authenticationServiceInternal, identity2);
          if (!context.User.Identity.IsAuthenticated)
          {
            userContextIdentity = (Microsoft.VisualStudio.Services.Identity.Identity) null;
          }
          else
          {
            this.ResolveIdentities(requestContext, identity2, context.User.Identity as ClaimsIdentity, application, out userContextIdentity);
            if (userContextIdentity != null && !requestContext.Items.ContainsKey(userContextIdentity.Descriptor.ToString()))
              requestContext.Items.Add(userContextIdentity.Descriptor.ToString(), (object) userContextIdentity);
            if (requestContext.Items.ContainsKey("PostAuthCreateSilentAadProfile"))
            {
              requestContext.Items.Add(ProfileCreateSourceConstants.Key, (object) ProfileCreateSourceConstants.Marketplace);
              requestContext.Trace(TracePoints.VssfAuthenticationModuleTrace.CreatingSilentAADProfile, TraceLevel.Info, VssfAuthenticationModule.s_Area, VssfAuthenticationModule.s_Layer, "Creating silent profile for AAD User {0}", (object) userContextIdentity.Id);
              AadAuthUrlUtility.TryCreateAadProfileSilently((IVssRequestContext) requestContext, userContextIdentity, string.Empty);
            }
            if (identity2 != null && !IdentityDescriptorComparer.Instance.Equals(identity2.Descriptor, userContextIdentity.Descriptor) && !requestContext.Items.ContainsKey(identity2.Descriptor.ToString()))
              requestContext.Items.Add(identity2.Descriptor.ToString(), (object) identity2);
            if (!authenticationServiceInternal.IsRequestAuthenticationValid((IVssWebRequestContext) requestContext, false))
              return;
            IVssRequestContext requestContext2 = requestContext.To(TeamFoundationHostType.Deployment);
            try
            {
              if (requestContext2.IsFeatureEnabled("VisualStudio.Services.Identity.SeamlessTenantSwitching"))
              {
                SwitchHintParameter switchHintParameter;
                if (this.IsTenantSwitchRequired((IVssRequestContext) requestContext, out switchHintParameter))
                {
                  string redirectLocation = authenticationServiceInternal.GetSignInRedirectLocation((IVssRequestContext) requestContext, true, switchHintParameter: switchHintParameter);
                  context.Response.Redirect(redirectLocation, false);
                  application.CompleteRequest();
                  return;
                }
              }
            }
            catch (TenantSwitchException ex)
            {
              requestContext.TraceException(54678, VssfAuthenticationModule.s_Area, VssfAuthenticationModule.s_Layer, (Exception) ex);
              TeamFoundationApplicationCore.CompleteRequest((IVssRequestContext) requestContext, application, HttpStatusCode.Unauthorized, (Exception) ex);
              return;
            }
          }
        }
        else if (flag)
        {
          SessionTrackingState sessionState;
          if (requestContext.GetSessionTrackingState(out sessionState) && sessionState != null)
            requestContext.SetAnonymousIdentifier(sessionState.PersistentSessionId.ToString("N"));
          requestContext.RequestContextInternal().Actors = (IReadOnlyList<IRequestActor>) new List<IRequestActor>()
          {
            RequestActor.CreateAnonymousRequestActor((IVssRequestContext) requestContext)
          };
        }
        context.Items[(object) HttpContextConstants.UserIdentityInternal] = (object) userContextIdentity;
        if (!flag)
        {
          IdentityValidationResult validationResult = requestContext.To(TeamFoundationHostType.Deployment).GetService<IIdentityValidationService>().ValidateRequestIdentity((IVssRequestContext) requestContext);
          if (!validationResult.IsSuccess)
          {
            if (validationResult.Exception != null)
              TeamFoundationApplicationCore.CompleteRequest((IVssRequestContext) requestContext, application, validationResult.HttpStatusCode, validationResult.Exception);
            else
              TeamFoundationApplicationCore.CompleteRequest(application, validationResult.HttpStatusCode, validationResult.ResultMessage, (IEnumerable<KeyValuePair<string, string>>) null, (Exception) null, validationResult.ResultMessage, (string) null);
          }
          else
          {
            TeamFoundationApplicationCore.ApplyLicensePrincipals((IVssRequestContext) requestContext);
            TeamFoundationRequestFilterHelper.PostAuthorizeRequest((IVssRequestContext) requestContext);
          }
        }
        else
        {
          if (userContextIdentity == null)
            return;
          requestContext.ComputePublicUser();
        }
      }
      catch (RequestFilterException ex)
      {
        requestContext.TraceException(54678, VssfAuthenticationModule.s_Area, VssfAuthenticationModule.s_Layer, (Exception) ex);
        TeamFoundationApplicationCore.CompleteRequest((IVssRequestContext) requestContext, application, ex.HttpStatusCode, (Exception) ex);
      }
      catch (UnauthorizedAccessException ex)
      {
        requestContext.TraceException(54678, VssfAuthenticationModule.s_Area, VssfAuthenticationModule.s_Layer, (Exception) ex);
        TeamFoundationApplicationCore.CompleteRequest((IVssRequestContext) requestContext, application, HttpStatusCode.Forbidden, (Exception) ex);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(54678, VssfAuthenticationModule.s_Area, VssfAuthenticationModule.s_Layer, ex);
        TeamFoundationApplicationCore.CompleteRequest((IVssRequestContext) requestContext, application, HttpStatusCode.InternalServerError, ex);
      }
      finally
      {
        requestContext.TraceLeave(108151, VssfAuthenticationModule.s_Area, VssfAuthenticationModule.s_Layer, "VssfAuthenticationModule.OnPostAuthenticateRequest");
      }
    }

    private void OnPostAuthorizeRequest(
      AspNetRequestContext requestContext,
      IHttpApplication application)
    {
      if (!requestContext.ExecutionEnvironment.IsHostedDeployment || application.Context.Items.Contains((object) HttpContextConstants.ArrRequestRouted))
        return;
      requestContext.TraceEnter(558403, VssfAuthenticationModule.s_Area, VssfAuthenticationModule.s_Layer, "VssfAuthenticationModule.OnPostAuthorizeRequest");
      try
      {
        ISignedInProcessor signedInProcessor = requestContext.GetExtensions<ISignedInProcessor>(ExtensionLifetime.Service).FirstOrDefault<ISignedInProcessor>();
        bool isSignIn = signedInProcessor != null && signedInProcessor.IsSignedInRequest((IVssRequestContext) requestContext, application);
        if (isSignIn)
        {
          application.Response.Cache.SetCacheability(HttpCacheability.NoCache);
          AuthenticationHelpers.EnterMethodIfNull((IVssRequestContext) requestContext, "Authentication", "Signedin");
          signedInProcessor.HandleSignedInRequest((IVssRequestContext) requestContext, application);
        }
        this.m_authenticationAudit.AuditAuthorizedRequest((IVssRequestContext) requestContext, isSignIn);
      }
      catch (UnauthorizedRequestException ex)
      {
        TeamFoundationApplicationCore.CompleteRequest((IVssRequestContext) requestContext, application, ex.HttpStatusCode, (Exception) ex);
      }
      catch (InvalidReplyToException ex)
      {
        requestContext.TraceException(634735, VssfAuthenticationModule.s_Area, VssfAuthenticationModule.s_Layer, (Exception) ex);
        TeamFoundationApplicationCore.CompleteRequest((IVssRequestContext) requestContext, application, HttpStatusCode.BadRequest, (Exception) ex);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(634734, VssfAuthenticationModule.s_Area, VssfAuthenticationModule.s_Layer, ex);
        TeamFoundationApplicationCore.CompleteRequest((IVssRequestContext) requestContext, application, HttpStatusCode.InternalServerError, ex);
      }
      finally
      {
        requestContext.TraceLeave(834997, VssfAuthenticationModule.s_Area, VssfAuthenticationModule.s_Layer, "VssfAuthenticationModule.OnPostAuthorizeRequest");
      }
    }

    private bool IsExternalUserResolutionRequired(
      AspNetRequestContext requestContext,
      ClaimsIdentity claimsIdentity)
    {
      if (claimsIdentity == null || !requestContext.ExecutionEnvironment.IsHostedDeployment || !requestContext.IsFeatureEnabled("VisualStudio.Services.Identity.OrgIdDisambiguation") || requestContext.RequestRestrictions().RequiredAuthentication <= RequiredAuthentication.ExternalUser)
      {
        requestContext.Trace(TracePoints.VssfAuthenticationModuleTrace.ExternalUserResolutionDecision, TraceLevel.Verbose, VssfAuthenticationModule.s_Area, VssfAuthenticationModule.s_Layer, "External user resolution decision: False. Execution Environment : {0}, Disambiguation Flag Enabled: {1}, Required Authentication: {2}", (object) requestContext.ServiceHost.HostType.ToString(), (object) requestContext.IsFeatureEnabled("VisualStudio.Services.Identity.OrgIdDisambiguation"), (object) requestContext.RequestRestrictions().RequiredAuthentication.ToString());
        return false;
      }
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      if (!vssRequestContext.Items.ContainsKey("IsAadAuthFlow") || !(bool) vssRequestContext.Items["IsAadAuthFlow"])
      {
        requestContext.Trace(TracePoints.VssfAuthenticationModuleTrace.ExternalUserResolutionDecision, TraceLevel.Verbose, VssfAuthenticationModule.s_Area, VssfAuthenticationModule.s_Layer, "External user resolution decision: False. Current authentication flow is not AAD.");
        return false;
      }
      Claim first1 = claimsIdentity.FindFirst("http://schemas.microsoft.com/teamfoundationserver/2010/12/claims/identityprovider");
      if (first1 == null)
      {
        requestContext.Trace(TracePoints.VssfAuthenticationModuleTrace.ExternalUserResolutionDecision, TraceLevel.Warning, VssfAuthenticationModule.s_Area, VssfAuthenticationModule.s_Layer, "External user resolution decision: False. Claims Identity does not have a IdentityProviderType claim.");
        return false;
      }
      if (first1.Value != "Windows Live ID" && !Guid.TryParse(first1.Value, out Guid _))
      {
        requestContext.Trace(TracePoints.VssfAuthenticationModuleTrace.ExternalUserResolutionDecision, TraceLevel.Verbose, VssfAuthenticationModule.s_Area, VssfAuthenticationModule.s_Layer, "External user resolution decision: False. Claims identity provider not applicable for disambiguation. Identity provider {0}", (object) first1.Value);
        return false;
      }
      Claim first2 = claimsIdentity.FindFirst("tenant_disambiguate");
      if (first2 != null)
        claimsIdentity.TryRemoveClaim(first2);
      if (first2?.Value != "true")
      {
        requestContext.Trace(TracePoints.VssfAuthenticationModuleTrace.ExternalUserResolutionDecision, TraceLevel.Verbose, VssfAuthenticationModule.s_Area, VssfAuthenticationModule.s_Layer, "External user resolution decision: False. Tenant disambiguation claim not found or is set to false.");
        return false;
      }
      if (TenantPickerHelper.CheckSkipTenantPicker((IVssRequestContext) requestContext))
      {
        requestContext.Trace(TracePoints.VssfAuthenticationModuleTrace.ExternalUserResolutionDecision, TraceLevel.Verbose, VssfAuthenticationModule.s_Area, VssfAuthenticationModule.s_Layer, "External user resolution decision: False. Tenant hint already found in request uri. Skipping external user resolution.");
        return false;
      }
      requestContext.Trace(TracePoints.VssfAuthenticationModuleTrace.ExternalUserResolutionDecision, TraceLevel.Verbose, VssfAuthenticationModule.s_Area, VssfAuthenticationModule.s_Layer, "External user resolution decision: True.");
      return true;
    }

    private bool IsTenantSwitchRequired(
      IVssRequestContext requestContext,
      out SwitchHintParameter switchHintParameter)
    {
      requestContext.TraceEnter(TracePoints.VssfAuthenticationModuleTrace.TenantSwitchRequiredEntry, VssfAuthenticationModule.s_Area, VssfAuthenticationModule.s_Layer, nameof (IsTenantSwitchRequired));
      switchHintParameter = (SwitchHintParameter) null;
      try
      {
        if (HttpContext.Current.Request.Headers["X-TFS-FedAuthRedirect"] == "Suppress" || HttpContext.Current.Request.Path.EndsWith(".asmx", StringComparison.OrdinalIgnoreCase) || HttpContext.Current.Request.Path.EndsWith(".ashx", StringComparison.OrdinalIgnoreCase) || (requestContext.RequestRestrictions().MechanismsToAdvertise & AuthenticationMechanisms.FederatedRedirect) == AuthenticationMechanisms.None || requestContext.UserContext == (IdentityDescriptor) null || IdentityHelper.IsShardedFrameworkIdentity(requestContext, requestContext.UserContext) || requestContext.UserContext.IsSystemServicePrincipalType())
          return false;
        IVssRequestContext requestContext1 = requestContext.To(TeamFoundationHostType.Deployment);
        Microsoft.VisualStudio.Services.Identity.Identity userIdentity = requestContext.GetUserIdentity();
        if (requestContext.ExecutionEnvironment.IsHostedDeployment && IdentityHelper.IsUserIdentity(requestContext, (IReadOnlyVssIdentity) userIdentity) && !requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment) && requestContext.RequestRestrictions().RequiredAuthentication >= RequiredAuthentication.ValidatedUser)
        {
          IVssRequestContext context = requestContext.To(TeamFoundationHostType.Application);
          Guid organizationAadTenantId = context.GetOrganizationAadTenantId();
          IdentityService service = context.GetService<IdentityService>();
          Guid result;
          Guid.TryParse(userIdentity.GetProperty<string>("Domain", string.Empty), out result);
          requestContext.Trace(TracePoints.VssfAuthenticationModuleTrace.TenantSwitchInfo, TraceLevel.Verbose, VssfAuthenticationModule.s_Area, VssfAuthenticationModule.s_Layer, "Identity Tenant: {0} | Account Tenant: {1}", (object) result, (object) organizationAadTenantId);
          if (organizationAadTenantId != result)
          {
            string property = userIdentity.GetProperty<string>("Account", string.Empty);
            IList<Microsoft.VisualStudio.Services.Identity.Identity> source = service.ReadIdentities(context.Elevate(), IdentitySearchFilter.AccountName, property, QueryMembership.None, (IEnumerable<string>) null);
            requestContext.Trace(343973, TraceLevel.Verbose, VssfAuthenticationModule.s_Area, VssfAuthenticationModule.s_Layer, "Found {0} identities in account matching.", (object) source.Count);
            if (source.Any<Microsoft.VisualStudio.Services.Identity.Identity>())
            {
              if (organizationAadTenantId == Guid.Empty)
              {
                requestContext.Trace(113779, TraceLevel.Verbose, VssfAuthenticationModule.s_Area, VssfAuthenticationModule.s_Layer, "The tenant is a Msa tenant and signed in user is AAD backed");
                if (userIdentity.IsFpmsa() && source.Any<Microsoft.VisualStudio.Services.Identity.Identity>((Func<Microsoft.VisualStudio.Services.Identity.Identity, bool>) (i => i.IsActive && i.IsMsa())))
                {
                  requestContext.Trace(941350, TraceLevel.Info, VssfAuthenticationModule.s_Area, VssfAuthenticationModule.s_Layer, "FP msa login detected instead of the MSA version. Tenant switch required");
                  switchHintParameter = SwitchHintParameter.PersonalWithAccountName(property);
                  return true;
                }
                if (!source.Any<Microsoft.VisualStudio.Services.Identity.Identity>((Func<Microsoft.VisualStudio.Services.Identity.Identity, bool>) (i => i.IsActive && i.IsClaims && i.IsMsa())))
                  return false;
                requestContext.Trace(941352, TraceLevel.Info, VssfAuthenticationModule.s_Area, VssfAuthenticationModule.s_Layer, "Tenant Switching: AAD version of Dual Homed User detected. Expected MSA version. Redirecting to IDP");
                switchHintParameter = SwitchHintParameter.PersonalWithAccountName(property);
                return true;
              }
              if (AadIdentityHelper.IsAadTenant(organizationAadTenantId) && requestContext1.IsFeatureEnabled("VisualStudio.Services.Identity.SeamlessTenantSwitching.InvitationPendingCheck") && source.Any<Microsoft.VisualStudio.Services.Identity.Identity>((Func<Microsoft.VisualStudio.Services.Identity.Identity, bool>) (i => i.IsActive && AadIdentityHelper.IsInvitationPending(requestContext, (IReadOnlyVssIdentity) i))))
              {
                requestContext.Trace(1450916, TraceLevel.Info, VssfAuthenticationModule.s_Area, VssfAuthenticationModule.s_Layer, "Tenant Switching: AAD user with AAD Invite Pending detected.");
                throw new InvitationPendingException(property, AadIdentityHelper.GetTenantName(requestContext.Elevate()));
              }
              if (requestContext1.IsFeatureEnabled("VisualStudio.Services.Identity.SeamlessTenantSwitching.WrongWorkOrPersonalCheck"))
              {
                bool currentIdentityIsMsaOrFpmsa = userIdentity.IsMsaOrFpmsa();
                if (!source.Any<Microsoft.VisualStudio.Services.Identity.Identity>((Func<Microsoft.VisualStudio.Services.Identity.Identity, bool>) (i => i.IsActive && i.IsMsaOrFpmsa() == currentIdentityIsMsaOrFpmsa)))
                {
                  bool shouldBePersonal = !currentIdentityIsMsaOrFpmsa;
                  bool shouldCreatePersonal = false;
                  requestContext.Trace(1450915, TraceLevel.Info, VssfAuthenticationModule.s_Area, VssfAuthenticationModule.s_Layer, string.Format("Tenant Switching: Wrong version of Dual Homed User detected. shouldBePersonal={0}; shouldCreatePresonal={1};", (object) shouldBePersonal, (object) shouldCreatePersonal));
                  throw new WrongWorkOrPersonalException(property, shouldBePersonal, shouldCreatePersonal);
                }
              }
              requestContext.Trace(1450914, TraceLevel.Info, VssfAuthenticationModule.s_Area, VssfAuthenticationModule.s_Layer, "Tenant Switching: Redirecting to attempt seamless tenant switch.");
              return true;
            }
          }
        }
        return false;
      }
      finally
      {
        requestContext.TraceLeave(TracePoints.VssfAuthenticationModuleTrace.TenantSwitchRequiredExit, VssfAuthenticationModule.s_Area, VssfAuthenticationModule.s_Layer, nameof (IsTenantSwitchRequired));
      }
    }

    private void PreValidateRequestAuthentication(
      IVssRequestContext requestContext,
      ITeamFoundationAuthenticationService authenticationService,
      Microsoft.VisualStudio.Services.Identity.Identity identity)
    {
      if (requestContext.ExecutionEnvironment.IsOnPremisesDeployment)
        return;
      requestContext.TraceSerializedConditionally(15182000, TraceLevel.Verbose, VssfAuthenticationModule.s_Area, VssfAuthenticationModule.s_Layer, false, "Resolved identity: {0}", (object[]) new Microsoft.VisualStudio.Services.Identity.Identity[1]
      {
        identity
      });
      if (identity != null && identity.Id == Guid.Empty && requestContext.RequestRestrictions().RequiredAuthentication > RequiredAuthentication.Anonymous && (requestContext.ExecutionEnvironment.IsOnPremisesDeployment || requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment)))
      {
        requestContext.TraceSerializedConditionally(15182001, TraceLevel.Error, VssfAuthenticationModule.s_Area, VssfAuthenticationModule.s_Layer, false, "The resolved identity has an empty VSID. This request will be terminated. Identity: {0}", (object[]) new Microsoft.VisualStudio.Services.Identity.Identity[1]
        {
          identity
        });
        bool flag1;
        requestContext.RootContext.TryGetItem<bool>("AuthenticationByIdentityProvider", out flag1);
        bool flag2;
        requestContext.RootContext.TryGetItem<bool>("AuthenticationWithSessionAuth", out flag2);
        requestContext.TraceSerializedConditionally(15182001, TraceLevel.Error, VssfAuthenticationModule.s_Area, VssfAuthenticationModule.s_Layer, false, string.Format("AuthenticationByIdentityProvider: {0} | AuthenticationWithSessionAuth: {1}", (object) flag1, (object) flag2));
        requestContext.GetService<IInvalidRequestCompletionService>().CompleteInvalidRequest(requestContext, authenticationService, FrameworkResources.UnableToCreateIdentityFromToken(), "TeamFoundationModule");
      }
      new TokenRevocationValidator().PreValidateRequestAuthentication(requestContext, authenticationService, identity);
    }

    private bool TryGetImpersonatedIdentity(
      IVssRequestContext requestContext,
      IHttpApplication application,
      Microsoft.VisualStudio.Services.Identity.Identity authenticatedIdentity,
      out Microsoft.VisualStudio.Services.Identity.Identity impersonatedIdentity,
      out ClaimsIdentity claimsIdentity,
      out bool performSecurityCheck)
    {
      impersonatedIdentity = (Microsoft.VisualStudio.Services.Identity.Identity) null;
      claimsIdentity = (ClaimsIdentity) null;
      performSecurityCheck = true;
      IdentityDescriptor y = (IdentityDescriptor) null;
      if (requestContext.Items.ContainsKey(RequestContextItemsKeys.TfsImpersonate))
      {
        object obj = requestContext.Items[RequestContextItemsKeys.TfsImpersonate];
        claimsIdentity = obj as ClaimsIdentity;
        if (claimsIdentity != null)
        {
          IIdentityAuthenticationService service = requestContext.GetService<IIdentityAuthenticationService>();
          impersonatedIdentity = service.ResolveIdentity(requestContext, (IIdentity) claimsIdentity);
        }
        else
        {
          y = (IdentityDescriptor) obj;
          performSecurityCheck = false;
        }
      }
      else
      {
        string descriptorValue = application.Request.Headers.Get("X-TFS-Impersonate");
        if (!string.IsNullOrEmpty(descriptorValue))
        {
          string identityType;
          string identifier;
          TFCommonUtil.ParseDescriptorSearchFactor(descriptorValue, out identityType, out identifier);
          y = new IdentityDescriptor(identityType, identifier);
        }
      }
      IdentityService service1 = requestContext.GetService<IdentityService>();
      try
      {
        if (impersonatedIdentity == null)
        {
          string subjectDescriptorString = application.Request.Headers.Get("X-TFS-SubjectDescriptorImpersonate");
          if (!string.IsNullOrEmpty(subjectDescriptorString))
          {
            SubjectDescriptor objB = SubjectDescriptor.FromString(subjectDescriptorString);
            if (!object.Equals((object) authenticatedIdentity.SubjectDescriptor, (object) objB))
            {
              impersonatedIdentity = service1.ReadIdentities(requestContext, (IList<SubjectDescriptor>) new SubjectDescriptor[1]
              {
                objB
              }, QueryMembership.None, (IEnumerable<string>) null)[0];
              if (impersonatedIdentity == null)
                requestContext.Trace(60070, TraceLevel.Error, VssfAuthenticationModule.s_Area, VssfAuthenticationModule.s_Layer, string.Format("Unable to find Identity using Subject Descriptor {0}, so falling back to reading using Identity Descriptor", (object) objB));
              else
                requestContext.Trace(60078, TraceLevel.Verbose, VssfAuthenticationModule.s_Area, VssfAuthenticationModule.s_Layer, "Identity " + impersonatedIdentity.DisplayName + " found.");
            }
          }
        }
      }
      catch (Exception ex)
      {
        requestContext.TraceException(60079, TraceLevel.Verbose, VssfAuthenticationModule.s_Area, VssfAuthenticationModule.s_Layer, ex);
        requestContext.Trace(60082, TraceLevel.Verbose, VssfAuthenticationModule.s_Area, VssfAuthenticationModule.s_Layer, "Exception was throw during impersonation by Subject Descriptor, so falling back to impersonation by Identity Descriptor");
      }
      if (impersonatedIdentity == null && y != (IdentityDescriptor) null && !IdentityDescriptorComparer.Instance.Equals(authenticatedIdentity.Descriptor, y))
      {
        requestContext.Trace(60064, TraceLevel.Verbose, VssfAuthenticationModule.s_Area, VssfAuthenticationModule.s_Layer, "Getting the impersonated identity from descriptor {0}.", (object) y.Identifier);
        impersonatedIdentity = service1.ReadIdentities(requestContext, (IList<IdentityDescriptor>) new IdentityDescriptor[1]
        {
          y
        }, QueryMembership.None, (IEnumerable<string>) null)[0];
        if (impersonatedIdentity == null)
        {
          requestContext.Trace(60065, TraceLevel.Warning, VssfAuthenticationModule.s_Area, VssfAuthenticationModule.s_Layer, "Identity not found.");
          TeamFoundationApplicationCore.CompleteRequest(application, HttpStatusCode.Forbidden, (string) null, (IEnumerable<KeyValuePair<string, string>>) null, (Exception) null, FrameworkResources.IdentityNotFoundMessage((object) y.IdentityType, (object) y.Identifier), (string) null);
          return false;
        }
        requestContext.Trace(60066, TraceLevel.Verbose, VssfAuthenticationModule.s_Area, VssfAuthenticationModule.s_Layer, "Identity {0} found.", (object) impersonatedIdentity.DisplayName);
      }
      return true;
    }

    private void ResolveIdentities(
      AspNetRequestContext requestContext,
      Microsoft.VisualStudio.Services.Identity.Identity authenticatedIdentity,
      ClaimsIdentity claimsIdentity,
      IHttpApplication application,
      out Microsoft.VisualStudio.Services.Identity.Identity userContextIdentity)
    {
      userContextIdentity = authenticatedIdentity;
      string domainUserName1 = IdentityHelper.GetDomainUserName(authenticatedIdentity);
      string domainUserName2 = domainUserName1;
      List<IRequestActor> requestActorList = new List<IRequestActor>()
      {
        RequestActor.CreateRequestActor((IVssRequestContext) requestContext, authenticatedIdentity.Descriptor, authenticatedIdentity.Id)
      };
      ClaimsIdentity claimsIdentity1 = claimsIdentity;
      requestContext.RequestContextInternal().Actors = (IReadOnlyList<IRequestActor>) requestActorList;
      Microsoft.VisualStudio.Services.Identity.Identity impersonatedIdentity;
      ClaimsIdentity claimsIdentity2;
      bool performSecurityCheck;
      if (!this.TryGetImpersonatedIdentity((IVssRequestContext) requestContext, application, authenticatedIdentity, out impersonatedIdentity, out claimsIdentity2, out performSecurityCheck))
        return;
      if (impersonatedIdentity != null)
      {
        if (performSecurityCheck)
        {
          IVssRequestContext vssRequestContext = requestContext.ExecutionEnvironment.IsHostedDeployment ? requestContext.To(TeamFoundationHostType.Deployment) : (IVssRequestContext) requestContext;
          requestContext.Trace(60063, TraceLevel.Verbose, VssfAuthenticationModule.s_Area, VssfAuthenticationModule.s_Layer, "Checking permission to impersonate.");
          vssRequestContext.GetService<TeamFoundationSecurityService>().GetSecurityNamespace(vssRequestContext, FrameworkSecurity.FrameworkNamespaceId).CheckPermission(vssRequestContext, FrameworkSecurity.FrameworkNamespaceToken, 4, false);
        }
        userContextIdentity = impersonatedIdentity;
        if (requestContext.ExecutionEnvironment.IsOnPremisesDeployment)
        {
          requestActorList.Clear();
          requestActorList.Add(RequestActor.CreateRequestActor((IVssRequestContext) requestContext, authenticatedIdentity.Descriptor, authenticatedIdentity.Id, false));
        }
        requestActorList.Add(RequestActor.CreateRequestActor((IVssRequestContext) requestContext, impersonatedIdentity.Descriptor, impersonatedIdentity.Id));
        domainUserName2 = IdentityHelper.GetDomainUserName(impersonatedIdentity);
        claimsIdentity1 = claimsIdentity2;
        requestContext.ClearUserContextCache();
      }
      try
      {
        requestContext.RootContext.Items[RequestContextItemsKeys.IdentityTracingItems] = (object) IdentityHelper.GenerateIdentityTracingItems((IVssRequestContext) requestContext, (IReadOnlyVssIdentity) userContextIdentity);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(60074, TraceLevel.Warning, VssfAuthenticationModule.s_Area, VssfAuthenticationModule.s_Layer, ex);
      }
      requestContext.RequestContextInternal().SetAuthenticatedUserName(domainUserName1);
      requestContext.RequestContextInternal().SetDomainUserName(domainUserName2);
      if (claimsIdentity1 == null)
        return;
      requestContext.Items[RequestContextItemsKeys.AuthorizedClaimsIdentity] = (object) claimsIdentity1;
    }
  }
}
