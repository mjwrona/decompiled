// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.IdentityExtensions
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.GraphProfile.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Graph;
using Microsoft.VisualStudio.Services.Location.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.Identity
{
  public static class IdentityExtensions
  {
    private const string Area = "IdentityService";
    private const string Layer = "IdentityExtensions";

    public static HashSet<string> GetModifiedProperties(this Microsoft.VisualStudio.Services.Identity.Identity identity)
    {
      HashSet<string> modifiedProperties = (HashSet<string>) null;
      if (identity.HasModifiedProperties)
      {
        foreach (string key in identity.Properties.Keys)
        {
          if (!IdentityAttributeTags.ReadOnlyProperties.Contains(key))
          {
            if (modifiedProperties == null)
              modifiedProperties = new HashSet<string>();
            modifiedProperties.Add(key);
          }
        }
      }
      return modifiedProperties;
    }

    public static Guid GetAadObjectId(this IReadOnlyVssIdentity identity)
    {
      if (identity == null)
        return Guid.Empty;
      return !identity.IsContainer ? AadIdentityHelper.ExtractObjectId(identity) : AadIdentityHelper.ExtractAadGroupId(identity.Descriptor);
    }

    public static Guid StorageKey(
      this Microsoft.VisualStudio.Services.Identity.Identity identity,
      IVssRequestContext requestContext,
      TeamFoundationHostType targetHostType = TeamFoundationHostType.Unknown)
    {
      requestContext.Trace(10008229, TraceLevel.Info, "IdentityService", nameof (IdentityExtensions), "Input ServiceHost is of type {0} and targetHostType is of type {1}.", (object) requestContext.ServiceHost.HostType, (object) targetHostType);
      if (identity == null)
      {
        requestContext.Trace(10008232, TraceLevel.Warning, "IdentityService", nameof (IdentityExtensions), "Identity passed is null. Returning Guid.Empty");
        return Guid.Empty;
      }
      if (requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
      {
        requestContext.Trace(10008233, TraceLevel.Info, "IdentityService", nameof (IdentityExtensions), "ServiceHost is of type Deployment. Returning identity.MasterId - {0}", (object) identity.MasterId);
        return identity.MasterId;
      }
      if (requestContext.ServiceHost.Is(targetHostType))
      {
        requestContext.Trace(10008233, TraceLevel.Info, "IdentityService", nameof (IdentityExtensions), "ServiceHost and targetHostType are of type {0}. Returning identity.Id - {1}", (object) requestContext.ServiceHost.HostType, (object) identity.Id);
        return identity.Id;
      }
      if (targetHostType == TeamFoundationHostType.ProjectCollection)
        throw new UnexpectedHostTypeException(targetHostType);
      bool flag = identity.MasterId == Guid.Empty || targetHostType != TeamFoundationHostType.Application && IdentityHelper.IsUserIdentity(requestContext, (IReadOnlyVssIdentity) identity);
      IVssIdentity vssIdentity = (IVssIdentity) null;
      if (flag)
      {
        IVssRequestContext vssRequestContext = requestContext.To(targetHostType);
        vssIdentity = (IVssIdentity) vssRequestContext.GetService<IdentityService>().ReadIdentities(vssRequestContext, (IList<IdentityDescriptor>) new IdentityDescriptor[1]
        {
          identity.Descriptor
        }, QueryMembership.None, (IEnumerable<string>) null).SingleOrDefault<Microsoft.VisualStudio.Services.Identity.Identity>();
      }
      requestContext.Trace(10008235, TraceLevel.Info, "IdentityService", nameof (IdentityExtensions), "TargetIdentity.Id - {0},  Identity.MasterId - {1}", (object) vssIdentity?.Id, (object) identity.MasterId);
      return vssIdentity == null ? identity.MasterId : vssIdentity.Id;
    }

    internal static string GetPublicDisplayName(
      this Microsoft.VisualStudio.Services.Identity.Identity identity,
      IVssRequestContext requestContext)
    {
      return (identity.Descriptor == (IdentityDescriptor) null || identity.Descriptor.IsBindPendingType()) && (requestContext.IsPublicUser() || requestContext.IsAnonymousPrincipal()) ? FrameworkResources.PublicDisplayNameForBindPendingIdentity() : identity.DisplayName;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static IdentityRef ToIdentityRefPrivate(
      this Microsoft.VisualStudio.Services.Identity.Identity identity,
      IVssRequestContext requestContext,
      bool includeUrls = true,
      bool includeInactive = false,
      IVssRequestContext requiredContextForResourceUriLookUp = null,
      ILocationDataProvider locationDataProvider = null)
    {
      if (identity == null)
        return (IdentityRef) null;
      IdentityRef identityRefPrivate = new IdentityRef()
      {
        Id = identity.Id.ToString(),
        Descriptor = identity.GetSubjectDescriptor(requestContext),
        DisplayName = identity.GetPublicDisplayName(requestContext),
        UniqueName = IdentityHelper.GetUniqueName(identity),
        IsContainer = identity.IsContainer,
        Inactive = includeInactive && !identity.IsActive,
        IsDeletedInOrigin = identity.GetProperty<bool>("IsDeletedInOrigin", false)
      };
      if (includeUrls)
      {
        identityRefPrivate.Url = IdentityHelper.GetIdentityResourceUriString(requiredContextForResourceUriLookUp ?? requestContext, identity.Id, locationDataProvider);
        string graphMemberAvatarUrl = GraphProfileUrlHelper.GetGraphMemberAvatarUrl(requestContext, identityRefPrivate.Descriptor, identity.Id);
        identityRefPrivate.ImageUrl = graphMemberAvatarUrl;
        if (!string.IsNullOrEmpty(graphMemberAvatarUrl))
        {
          identityRefPrivate.Links = new ReferenceLinks();
          identityRefPrivate.Links.AddLink("avatar", graphMemberAvatarUrl, (ISecuredObject) identityRefPrivate);
        }
      }
      return identityRefPrivate;
    }

    public static IdentityRef ToIdentityRef(
      this Microsoft.VisualStudio.Services.Identity.Identity identity,
      IVssRequestContext requestContext,
      bool includeUrls = true,
      bool includeInactive = false)
    {
      requestContext.CheckPermissionToReadPublicIdentityInfo();
      return identity.ToIdentityRefPrivate(requestContext, includeUrls, includeInactive);
    }

    public static IdentityRef[] ToIdentityRefs(
      this IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity> identities,
      IVssRequestContext requestContext,
      bool includeUrls = true)
    {
      if (identities == null)
        return (IdentityRef[]) null;
      int length = identities.Count<Microsoft.VisualStudio.Services.Identity.Identity>();
      IdentityRef[] identityRefs = new IdentityRef[length];
      if (length > 0)
      {
        requestContext.CheckPermissionToReadPublicIdentityInfo();
        ILocationDataProvider locationDataProvider = (ILocationDataProvider) null;
        IVssRequestContext requiredContextForResourceUriLookUp = (IVssRequestContext) null;
        if (includeUrls)
        {
          locationDataProvider = IdentityHelper.GetLocationDataProviderForResourceUriLookup(requestContext);
          requiredContextForResourceUriLookUp = IdentityHelper.GetRequestContextForUriLookup(requestContext);
        }
        int index = 0;
        foreach (Microsoft.VisualStudio.Services.Identity.Identity identity in identities)
        {
          if (identity != null)
            identityRefs[index] = identity.ToIdentityRefPrivate(requestContext, includeUrls, true, requiredContextForResourceUriLookUp, locationDataProvider);
          ++index;
        }
      }
      return identityRefs;
    }

    public static bool IsLocalGroup(IVssRequestContext requestContext, Microsoft.VisualStudio.Services.Identity.Identity identity) => identity?.Descriptor != (IdentityDescriptor) null && identity.IsContainer && string.Equals(identity.Descriptor.IdentityType, "Microsoft.TeamFoundation.Identity", StringComparison.OrdinalIgnoreCase) && string.Equals(identity.GetProperty<string>("SchemaClassName", (string) null), "Group") && !AadIdentityHelper.IsAadGroup(identity.Descriptor);

    public static void MapFromWellKnownIdentifier(
      this IdentityService identityService,
      Microsoft.VisualStudio.Services.Identity.Identity identity)
    {
      identityService.IdentityMapper.MapFromWellKnownIdentifier(identity);
    }

    public static void MapFromWellKnownIdentifiers(
      this IdentityService identityService,
      IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity> identities)
    {
      identityService.IdentityMapper.MapFromWellKnownIdentifiers(identities);
    }

    public static IdentityDescriptor MapFromWellKnownIdentifier(
      this IdentityService identityService,
      IdentityDescriptor descriptor)
    {
      return identityService.IdentityMapper.MapFromWellKnownIdentifier(descriptor);
    }

    public static string MapFromWellKnownIdentifier(
      this IdentityService identityService,
      string identifier)
    {
      return identityService.IdentityMapper.MapFromWellKnownIdentifier(identifier);
    }

    public static void MapToWellKnownIdentifier(
      this IdentityService identityService,
      Microsoft.VisualStudio.Services.Identity.Identity identity)
    {
      identityService.IdentityMapper.MapToWellKnownIdentifier(identity);
    }

    public static void MapToWellKnownIdentifiers(
      this IdentityService identityService,
      IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity> identities)
    {
      identityService.IdentityMapper.MapToWellKnownIdentifiers(identities);
    }

    public static IdentityDescriptor MapToWellKnownIdentifier(
      this IdentityService identityService,
      IdentityDescriptor descriptor)
    {
      return identityService.IdentityMapper.MapToWellKnownIdentifier(descriptor);
    }

    public static string MapToWellKnownIdentifier(
      this IdentityService identityService,
      string identifier)
    {
      return identityService.IdentityMapper.MapToWellKnownIdentifier(identifier);
    }

    internal static IIdentityServiceInternal IdentityServiceInternal(
      this IdentityService identityService)
    {
      return identityService as IIdentityServiceInternal;
    }

    internal static IIdentityServiceInternalRestricted IdentityServiceInternalRestricted(
      this IdentityService identityService,
      bool throwOnFailure = false)
    {
      return identityService as IIdentityServiceInternalRestricted;
    }

    internal static string GetSubjectType(
      this IReadOnlyVssIdentity identity,
      IVssRequestContext requestContext)
    {
      ArgumentUtility.CheckForNull<IReadOnlyVssIdentity>(identity, nameof (identity));
      ArgumentUtility.CheckForNull<IdentityDescriptor>(identity.Descriptor, "Descriptor");
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      return !identity.Descriptor.IsCuidBased() ? identity.Descriptor.GetSubjectTypeForNonCuidBasedIdentity(requestContext) : identity.GetSubjectTypeForCuidBasedIdentity(requestContext);
    }

    internal static string GetSubjectTypeForNonCuidBasedIdentity(
      this IdentityDescriptor identityDescriptor,
      IVssRequestContext requestContext)
    {
      ArgumentUtility.CheckForNull<IdentityDescriptor>(identityDescriptor, nameof (identityDescriptor));
      if (identityDescriptor.IsGroupScopeType())
        return "scp";
      if (identityDescriptor.IsTeamFoundationType())
      {
        if (AadIdentityHelper.IsAadGroup(identityDescriptor))
          return "aadgp";
        if (AadIdentityHelper.IsTfsGroup(identityDescriptor))
          return "vssgp";
        requestContext.TraceDataConditionally(10008227, TraceLevel.Warning, "IdentityService", nameof (IdentityExtensions), "Unknown group identity found", (Func<object>) (() => (object) new
        {
          identityDescriptor = identityDescriptor
        }), nameof (GetSubjectTypeForNonCuidBasedIdentity));
        return "ungrp";
      }
      if (Microsoft.TeamFoundation.Framework.Server.ServicePrincipals.IsServicePrincipal(requestContext, identityDescriptor))
        return "s2s";
      if (identityDescriptor.IsBindPendingType())
        return "bnd";
      if (identityDescriptor.IsWindowsType())
        return "win";
      if (identityDescriptor.IsUnauthenticatedIdentity())
        return "uauth";
      if (identityDescriptor.IsServiceIdentityType())
        return "svc";
      if (identityDescriptor.IsAggregateIdentityType())
        return "agg";
      if (identityDescriptor.IsImportedIdentityType())
        return "imp";
      if (identityDescriptor.IsServerTestIdentityType())
        return "tst";
      if (identityDescriptor.IsSystemLicenseType())
        return "slic";
      if (identityDescriptor.IsSystemScopeType())
        return "sscp";
      if (identityDescriptor.IsSystemCspPartnerType())
        return "scsp";
      if (identityDescriptor.IsSystemPublicAccessType())
        return "spa";
      if (identityDescriptor.IsSystemAccessControlType())
        return "sace";
      if (identityDescriptor.IsClaimsIdentityType())
      {
        if (Guid.TryParse(identityDescriptor.Identifier, out Guid _))
          return "acs";
        requestContext.TraceDataConditionally(10008231, TraceLevel.Warning, "IdentityService", nameof (IdentityExtensions), "Unknown user identity type found", (Func<object>) (() => (object) new
        {
          identityDescriptor = identityDescriptor
        }), nameof (GetSubjectTypeForNonCuidBasedIdentity));
        return "unusr";
      }
      requestContext.TraceDataConditionally(10008230, TraceLevel.Warning, "IdentityService", nameof (IdentityExtensions), "Unknown identity type found", (Func<object>) (() => (object) new
      {
        identityDescriptor = identityDescriptor
      }), nameof (GetSubjectTypeForNonCuidBasedIdentity));
      return "ukn";
    }

    internal static string GetSubjectTypeForCuidBasedIdentity(
      this IReadOnlyVssIdentity identity,
      IVssRequestContext requestContext)
    {
      ArgumentUtility.CheckForNull<IReadOnlyVssIdentity>(identity, nameof (identity));
      ArgumentUtility.CheckForNull<IdentityDescriptor>(identity.Descriptor, "Descriptor");
      if (!identity.Descriptor.IsCuidBased())
        throw new InvalidSubjectTypeException();
      if (identity.IsMsaIdentity())
        return "msa";
      if (identity.Descriptor.IsCspPartnerIdentityType())
        return "csp";
      if (identity.Descriptor.IsAadServicePrincipalType())
        return "aadsp";
      if (identity.IsExternalUser)
        return "aad";
      requestContext.TraceDataConditionally(10008231, TraceLevel.Warning, "IdentityService", nameof (IdentityExtensions), "Unknown cuid based user identity found", (Func<object>) (() => (object) new
      {
        identity = identity
      }), nameof (GetSubjectTypeForCuidBasedIdentity));
      return "unusr";
    }

    internal static bool IsMsaIdentity(this IReadOnlyVssIdentity identity)
    {
      if (identity == null)
        return false;
      if (identity.GetProperty<string>("Domain", string.Empty) == "Windows Live ID")
        return true;
      return identity.Descriptor != (IdentityDescriptor) null && identity.Descriptor.IdentityType == "Microsoft.IdentityModel.Claims.ClaimsIdentity" && !identity.Descriptor.Identifier.Contains("\\") && identity.Descriptor.Identifier.EndsWith("@Live.com", StringComparison.OrdinalIgnoreCase);
    }

    public static Guid Cuid(this IVssIdentity identity, Guid defaultValue = default (Guid))
    {
      Guid property = identity.GetProperty<Guid>("CUID", defaultValue);
      if (property != defaultValue)
        return property;
      Guid cuid = IdentityCuidHelper.ComputeCuid((IVssRequestContext) null, (IReadOnlyVssIdentity) identity);
      if (cuid == new Guid())
        return defaultValue;
      identity.SetProperty("CUID", (object) cuid);
      return cuid;
    }

    public static IReadOnlyDictionary<SubjectDescriptor, Microsoft.VisualStudio.Services.Identity.Identity> LookupIdentities(
      this IdentityService identityService,
      IVssRequestContext requestContext,
      IEnumerable<SubjectDescriptor> subjectDescriptors,
      QueryMembership queryMembership,
      IEnumerable<string> propertyNameFilters,
      bool includeRestrictedVisibility = false)
    {
      if (subjectDescriptors == null)
        return (IReadOnlyDictionary<SubjectDescriptor, Microsoft.VisualStudio.Services.Identity.Identity>) new Dictionary<SubjectDescriptor, Microsoft.VisualStudio.Services.Identity.Identity>();
      if (!requestContext.IsFeatureEnabled("VisualStudio.Services.DisableSubjectDescriptorPermissionCheck"))
      {
        GraphSecurityHelper.CheckPermissionToReadIdentity(requestContext, 1);
        requestContext.RootContext.Items[RequestContextItemsKeys.IsFrameworkIdentityReadPermissionCheckComplete] = (object) true;
      }
      List<SubjectDescriptor> list1 = subjectDescriptors.Distinct<SubjectDescriptor>().ToList<SubjectDescriptor>();
      List<IdentityDescriptor> list2 = list1.Select<SubjectDescriptor, IdentityDescriptor>((Func<SubjectDescriptor, IdentityDescriptor>) (x => x.ToIdentityDescriptor(requestContext))).ToList<IdentityDescriptor>();
      List<IdentityDescriptor> list3 = list2.Where<IdentityDescriptor>((Func<IdentityDescriptor, bool>) (x => x != (IdentityDescriptor) null)).ToList<IdentityDescriptor>();
      IList<Microsoft.VisualStudio.Services.Identity.Identity> identityList = identityService.ReadIdentities(requestContext, (IList<IdentityDescriptor>) list3, queryMembership, propertyNameFilters, includeRestrictedVisibility);
      Dictionary<SubjectDescriptor, Microsoft.VisualStudio.Services.Identity.Identity> dictionary = new Dictionary<SubjectDescriptor, Microsoft.VisualStudio.Services.Identity.Identity>(list2.Count);
      int index1 = 0;
      for (int index2 = 0; index2 < list2.Count; ++index2)
      {
        IdentityDescriptor identityDescriptor = list2[index2];
        SubjectDescriptor key = list1[index2];
        if (identityDescriptor == (IdentityDescriptor) null)
        {
          dictionary.Add(key, (Microsoft.VisualStudio.Services.Identity.Identity) null);
        }
        else
        {
          dictionary.Add(key, identityList[index1]);
          ++index1;
        }
      }
      return (IReadOnlyDictionary<SubjectDescriptor, Microsoft.VisualStudio.Services.Identity.Identity>) dictionary;
    }

    public static bool IsMember(
      this IdentityService identityService,
      IVssRequestContext requestContext,
      SubjectDescriptor groupDescriptor,
      SubjectDescriptor memberDescriptor)
    {
      return identityService.IsMember(requestContext, groupDescriptor.ToIdentityDescriptor(requestContext), memberDescriptor.ToIdentityDescriptor(requestContext));
    }

    public static void DeleteGroup(
      this IdentityService identityService,
      IVssRequestContext requestContext,
      SubjectDescriptor groupDescriptor)
    {
      identityService.DeleteGroup(requestContext, groupDescriptor.ToIdentityDescriptor(requestContext));
    }

    public static bool AddMemberToGroup(
      this IdentityService identityService,
      IVssRequestContext requestContext,
      SubjectDescriptor groupDescriptor,
      SubjectDescriptor memberDescriptor)
    {
      return identityService.AddMemberToGroup(requestContext, groupDescriptor.ToIdentityDescriptor(requestContext), memberDescriptor.ToIdentityDescriptor(requestContext));
    }

    public static bool RemoveMemberFromGroup(
      this IdentityService identityService,
      IVssRequestContext requestContext,
      SubjectDescriptor groupDescriptor,
      SubjectDescriptor memberDescriptor)
    {
      return identityService.RemoveMemberFromGroup(requestContext, groupDescriptor.ToIdentityDescriptor(requestContext), memberDescriptor.ToIdentityDescriptor(requestContext));
    }
  }
}
