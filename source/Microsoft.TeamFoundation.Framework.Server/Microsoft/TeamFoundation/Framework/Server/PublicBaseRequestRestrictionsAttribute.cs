// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.PublicBaseRequestRestrictionsAttribute
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server.Authorization;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.Organization;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public abstract class PublicBaseRequestRestrictionsAttribute : RequestRestrictionsAttribute
  {
    private readonly bool m_s2sOnly;
    private readonly bool m_enforceDataspaceRestrictionsForMembers;
    private readonly Version m_minApiVersion;
    private readonly TeamFoundationHostType m_supportedHostTypes;
    private const string c_Area = "Authorization";
    private const string c_Layer = "PublicBaseRequestRestrictionsAttribute";

    public PublicBaseRequestRestrictionsAttribute(
      bool s2sOnly = false,
      bool enforceDataspaceRestrictionsForMembers = true,
      string minApiVersion = null,
      TeamFoundationHostType supportedHostTypes = TeamFoundationHostType.ProjectCollection)
      : this(s2sOnly, enforceDataspaceRestrictionsForMembers, minApiVersion, RequestRestrictions.DefaultRequestRestrictions.RequiredAuthentication, RequestRestrictions.DefaultRequestRestrictions.AllowNonSsl, RequestRestrictions.DefaultRequestRestrictions.AllowCORS, RequestRestrictions.DefaultRequestRestrictions.MechanismsToAdvertise, RequestRestrictions.DefaultRequestRestrictions.Description, UserAgentFilterType.None, (string) null, supportedHostTypes)
    {
    }

    public PublicBaseRequestRestrictionsAttribute(
      bool s2sOnly,
      bool enforceDataspaceRestrictionsForMembers,
      string minApiVersion,
      RequiredAuthentication requiredAuthentication,
      bool allowNonSsl,
      bool allowCors,
      AuthenticationMechanisms mechanisms,
      string description,
      UserAgentFilterType agentFilterType,
      string agentFilter,
      TeamFoundationHostType supportedHostTypes)
      : base(requiredAuthentication, allowNonSsl, allowCors, mechanisms, description, agentFilterType, agentFilter)
    {
      this.m_s2sOnly = s2sOnly;
      this.m_enforceDataspaceRestrictionsForMembers = enforceDataspaceRestrictionsForMembers;
      if (!string.IsNullOrEmpty(minApiVersion))
      {
        if (!Version.TryParse(minApiVersion, out this.m_minApiVersion))
          TeamFoundationTracingService.TraceRaw(134217801, TraceLevel.Error, "Authorization", nameof (PublicBaseRequestRestrictionsAttribute), "Failed to parse minApiVersion as Version, raw value:$" + minApiVersion);
      }
      else
        this.m_minApiVersion = (Version) null;
      this.m_supportedHostTypes = supportedHostTypes;
    }

    public override void ApplyRequestRestrictions(
      IVssRequestContext requestContext,
      IDictionary<string, object> routeValues)
    {
      requestContext.RootContext.Items[RequestContextItemsKeys.SupportsPublicAccess] = (object) SupportsPublicAccess.Potentially;
      bool flag1 = false;
      if (requestContext.IsFeatureEnabled("VisualStudio.Services.Identity.AnonymousAccess") && this.VerifyHostRestrictions(requestContext))
      {
        bool flag2 = true;
        if (this.m_s2sOnly)
          flag2 = ServicePrincipals.IsServicePrincipal(requestContext, requestContext.GetAuthenticatedDescriptor(), false);
        if (flag2 & this.MeetsMinApiRequirement(routeValues))
        {
          AllowPublicAccessResult publicAccessResult = this.Allow(requestContext, routeValues);
          if (publicAccessResult.SupportsPublicAccess)
          {
            bool isAnonymous = false;
            bool isPublicUser = false;
            if (publicAccessResult.AllowPublicAccess)
            {
              flag1 = true;
              RequestRestrictions requestRestrictions = new RequestRestrictions(this.GetLabel(routeValues), RequiredAuthentication.Anonymous, RequestRestrictions.DefaultRequestRestrictions.AllowedHandlers, this.Description, this.AllowNonSsl, this.AllowCORS, this.MechanismsToAdvertise);
              IWebRequestContextInternal requestContextInternal = requestContext.WebRequestContextInternal(false);
              if (requestContextInternal != null)
                requestContextInternal.RequestRestrictions = requestRestrictions;
              PublicBaseRequestRestrictionsAttribute.SetAnonymousPrincipalAsRequestActor(requestContext, out isAnonymous, out isPublicUser);
            }
            if (!requestContext.ServiceHost.IsProduction | isAnonymous | isPublicUser)
            {
              requestContext.RootContext.Items[RequestContextItemsKeys.SecurityTracking] = (object) new TrackedSecurityCollection(requestContext, isAnonymous | isPublicUser || this.m_enforceDataspaceRestrictionsForMembers);
              requestContext.RequestContextInternal().SetDataspaceIdentifier(publicAccessResult.DataspaceIdentifier);
            }
          }
        }
      }
      if (flag1)
        return;
      base.ApplyRequestRestrictions(requestContext, routeValues);
    }

    private bool VerifyHostRestrictions(IVssRequestContext requestContext) => (TeamFoundationHostTypeHelper.NormalizeHostType(requestContext.ServiceHost.HostType) & this.m_supportedHostTypes) != TeamFoundationHostType.Unknown && (requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment) || requestContext.GetService<IOrganizationPolicyService>().GetPolicy<bool>(requestContext.Elevate(), "Policy.AllowAnonymousAccess", false).EffectiveValue);

    private bool MeetsMinApiRequirement(IDictionary<string, object> routeValues)
    {
      ApiResourceVersion apiResourceVersion;
      return !(this.m_minApiVersion != (Version) null) || !routeValues.TryGetValue<ApiResourceVersion>("apiResourceVersion", out apiResourceVersion) || apiResourceVersion.ApiVersion >= this.m_minApiVersion;
    }

    private static void SetAnonymousPrincipalAsRequestActor(
      IVssRequestContext requestContext,
      out bool isAnonymous,
      out bool isPublicUser)
    {
      IRequestContextInternal requestContextInternal = requestContext.RequestContextInternal();
      if (requestContext.IsAnonymousPrincipal())
      {
        isAnonymous = true;
        isPublicUser = false;
      }
      else
      {
        bool flag = false;
        isAnonymous = false;
        isPublicUser = requestContext.IsPublicUser();
        if (isPublicUser)
          requestContext.RootContext.Items[RequestContextItemsKeys.UseDelegatedS2STokens] = (object) true;
        else
          flag = IdentityHelper.IsOrgUser(requestContext, requestContext.GetUserIdentity());
        List<IRequestActor> source = new List<IRequestActor>((IEnumerable<IRequestActor>) requestContextInternal.Actors);
        IRequestActor requestActor = source.Last<IRequestActor>();
        source.RemoveAt(source.Count - 1);
        List<IRequestActor> requestActorList = source;
        IVssRequestContext context = requestContext;
        IdentityDescriptor descriptor1 = requestActor.Descriptor;
        Guid identifier = requestActor.Identifier;
        IdentityDescriptor descriptor2 = isPublicUser ? (IdentityDescriptor) null : requestActor.Descriptor;
        IdentityDescriptor[] identityDescriptorArray;
        if (!flag)
          identityDescriptorArray = new IdentityDescriptor[1]
          {
            AnonymousAccessConstants.PublicUserSubject.ToDescriptor()
          };
        else
          identityDescriptorArray = new IdentityDescriptor[2]
          {
            AnonymousAccessConstants.PublicUserSubject.ToDescriptor(),
            OrgAccessConstants.OrgUserSubject.ToDescriptor()
          };
        IRequestActor withAdditionalRoles = RequestActor.CreateRequestActorWithAdditionalRoles(context, descriptor1, identifier, descriptor2, identityDescriptorArray);
        requestActorList.Add(withAdditionalRoles);
        requestContextInternal.ClearActors();
        requestContextInternal.Actors = (IReadOnlyList<IRequestActor>) source;
      }
      requestContext.SetLicenseForPublicResource();
    }

    public abstract AllowPublicAccessResult Allow(
      IVssRequestContext requestContext,
      IDictionary<string, object> routeValues);
  }
}
