// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.Authorization.RequestActor
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.Organization;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.TeamFoundation.Framework.Server.Authorization
{
  internal class RequestActor : IRequestActor
  {
    private Dictionary<SubjectType, EvaluationPrincipal> m_principals;
    private const string c_area = "Authorization";
    private const string c_layer = "RequestActor";

    private RequestActor(IdentityDescriptor descriptor, Guid identifier, bool identityIsPrincipal = true)
    {
      this.Descriptor = descriptor;
      this.Identifier = identifier;
      this.m_principals = new Dictionary<SubjectType, EvaluationPrincipal>();
      if (!identityIsPrincipal)
        return;
      this.m_principals.Add(SubjectType.Identity, new EvaluationPrincipal(descriptor));
    }

    internal static IRequestActor CreateRequestActor(
      IVssRequestContext context,
      IdentityDescriptor descriptor,
      Guid identifier,
      bool identityIsPrincipal = true)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(context, nameof (context));
      ArgumentUtility.CheckForNull<IdentityDescriptor>(descriptor, nameof (descriptor));
      RequestActor actor = new RequestActor(descriptor, identifier, identityIsPrincipal);
      if (descriptor.IsCspPartnerIdentityType())
      {
        actor.m_principals.Clear();
        RequestActor.ApplyCspPrincipal(context, (IRequestActor) actor, descriptor);
      }
      return (IRequestActor) actor;
    }

    internal static IRequestActor CreateAnonymousRequestActor(IVssRequestContext context)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(context, nameof (context));
      RequestActor anonymousRequestActor = new RequestActor(UserWellKnownIdentityDescriptors.AnonymousPrincipal, AnonymousAccessConstants.AnonymousSubjectId, false);
      EvaluationPrincipal principal = new EvaluationPrincipal(UserWellKnownIdentityDescriptors.AnonymousPrincipal, (IdentityDescriptor) null, (IEnumerable<IdentityDescriptor>) new IdentityDescriptor[1]
      {
        UserWellKnownIdentityDescriptors.AnonymousPrincipal
      });
      anonymousRequestActor.TryAppendPrincipal(SubjectType.Identity, principal);
      return (IRequestActor) anonymousRequestActor;
    }

    internal static IRequestActor CreateRequestActorWithMultipleIdentitySubjects(
      IVssRequestContext context,
      IdentityDescriptor primaryDescriptor,
      Guid primaryIdentifier,
      IdentityDescriptor additionalDescriptor)
    {
      return RequestActor.CreateRequestActorWithAdditionalRoles(context, primaryDescriptor, primaryIdentifier, primaryDescriptor, additionalDescriptor);
    }

    internal static IRequestActor CreateRequestActorWithAdditionalRoles(
      IVssRequestContext context,
      IdentityDescriptor primaryDescriptor,
      Guid primaryIdentifier,
      IdentityDescriptor membershipDescriptor,
      params IdentityDescriptor[] additionalDescriptors)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(context, nameof (context));
      ArgumentUtility.CheckForNull<IdentityDescriptor>(primaryDescriptor, nameof (primaryDescriptor));
      RequestActor withAdditionalRoles = new RequestActor(primaryDescriptor, primaryIdentifier, false);
      withAdditionalRoles.TryAppendPrincipal(SubjectType.Identity, new EvaluationPrincipal(primaryDescriptor, membershipDescriptor, (IEnumerable<IdentityDescriptor>) additionalDescriptors));
      return (IRequestActor) withAdditionalRoles;
    }

    public IdentityDescriptor Descriptor { get; private set; }

    public Guid Identifier { get; private set; }

    public bool TryAppendPrincipal(SubjectType principalType, EvaluationPrincipal principal)
    {
      if (this.m_principals.ContainsKey(principalType))
        return false;
      this.m_principals.Add(principalType, principal);
      return true;
    }

    public bool TryReplacePrincipal(SubjectType subjectType, EvaluationPrincipal principal)
    {
      if (!this.m_principals.Remove(subjectType))
        return false;
      this.m_principals.Add(subjectType, principal);
      return true;
    }

    public bool TryGetPrincipal(SubjectType subjectType, out EvaluationPrincipal principal)
    {
      principal = (EvaluationPrincipal) null;
      return this.m_principals.TryGetValue(subjectType, out principal);
    }

    public IReadOnlyDictionary<SubjectType, EvaluationPrincipal> Principals => (IReadOnlyDictionary<SubjectType, EvaluationPrincipal>) this.m_principals;

    private static void ApplyCspPrincipal(
      IVssRequestContext requestContext,
      IRequestActor actor,
      IdentityDescriptor descriptor)
    {
      RequestActor.ValidateCspUserIsAllowedOnCurrentHost(requestContext, descriptor);
      IVssRequestContext vssRequestContext1 = requestContext.Elevate();
      Microsoft.VisualStudio.Services.Identity.Identity cspIdentity = vssRequestContext1.GetService<IdentityService>().ReadIdentities(vssRequestContext1, (IList<IdentityDescriptor>) new List<IdentityDescriptor>()
      {
        descriptor
      }, QueryMembership.None, (IEnumerable<string>) null).FirstOrDefault<Microsoft.VisualStudio.Services.Identity.Identity>();
      if (cspIdentity == null)
        throw new IdentityNotFoundException(descriptor);
      IVssRequestContext vssRequestContext2 = vssRequestContext1.To(TeamFoundationHostType.Deployment);
      SecuritySubjectEntry securitySubjectEntry = vssRequestContext2.GetService<IVssSecuritySubjectService>().GetSecuritySubjectEntry(vssRequestContext2, RequestActor.GetCspSubjectEntryId(cspIdentity));
      if (securitySubjectEntry == null || securitySubjectEntry.SubjectType != SecuritySubjectType.CspPartner)
        throw new FailedToCreateRequestActorException(string.Format("Failed to find the corresponding security subject entry for CSP identity {0}, meta type {1} and security subject type {2}.", (object) descriptor, (object) cspIdentity.MetaType, (object) securitySubjectEntry?.SubjectType));
      if (!actor.TryAppendPrincipal(SubjectType.CspPartner, new EvaluationPrincipal(securitySubjectEntry.ToDescriptor())))
        throw new FailedToCreateRequestActorException(string.Format("Failed to apply CSP principal: {0} for identity: {1}.", (object) securitySubjectEntry, (object) descriptor));
      requestContext.Trace(60494, TraceLevel.Info, "Authorization", nameof (RequestActor), string.Format("Applied CSP security subject: {0} for identity: {1}.", (object) securitySubjectEntry.SubjectType, (object) descriptor));
    }

    private static void ValidateCspUserIsAllowedOnCurrentHost(
      IVssRequestContext requestContext,
      IdentityDescriptor descriptor)
    {
      if (requestContext.ExecutionEnvironment.IsOnPremisesDeployment)
        throw new FailedToCreateRequestActorException("The CSP(Cloud Solution Provider) feature is not allowed in OnPrem environment.");
      Guid identityTenantId = AadIdentityHelper.GetIdentityTenantId(descriptor);
      if (identityTenantId == Guid.Empty)
        throw new InvalidCspIdentityException(string.Format("Found CSP identity: {0} whose tenant Id is empty guid.", (object) descriptor));
      if (RequestActor.IsAnonymousRequest(requestContext) || requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
        return;
      Guid organizationAadTenantId = requestContext.GetOrganizationAadTenantId();
      if (organizationAadTenantId != identityTenantId)
        throw new UnauthorizedAccessException(FrameworkResources.CspIdentityAccessDenied_TenantsNotMatch((object) descriptor, (object) identityTenantId, (object) requestContext.ServiceHost, (object) organizationAadTenantId));
    }

    private static bool IsAnonymousRequest(IVssRequestContext requestContext)
    {
      IWebRequestContextInternal requestContextInternal = requestContext.WebRequestContextInternal(false);
      return (requestContextInternal?.RequestRestrictions != null ? (int) requestContextInternal.RequestRestrictions.RequiredAuthentication : 18) == 1;
    }

    private static Guid GetCspSubjectEntryId(Microsoft.VisualStudio.Services.Identity.Identity cspIdentity)
    {
      switch (cspIdentity.MetaType)
      {
        case IdentityMetaType.CompanyAdministrator:
          return CspConstants.CspPartnerSubjectStoreIds.CompanyAdministrator;
        case IdentityMetaType.HelpdeskAdministrator:
          return CspConstants.CspPartnerSubjectStoreIds.HelpdeskAdministrator;
        default:
          return Guid.Empty;
      }
    }
  }
}
