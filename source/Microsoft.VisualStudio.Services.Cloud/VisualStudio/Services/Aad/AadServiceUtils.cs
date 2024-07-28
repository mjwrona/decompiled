// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Aad.AadServiceUtils
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Aad
{
  public static class AadServiceUtils
  {
    private const string c_defaultADOAdminRoleTemplateIds = "e3973bdf-4987-49ae-837a-ba8e231c7286";
    private const string IncludeIndirectMembershipsInRoleChecks = "VisualStudio.Services.Aad.IncludeIndirectMembershipsInRoleChecks";

    internal static void ValidateId<T>(T id, AadServiceUtils.IdentifierType type = AadServiceUtils.IdentifierType.Unknown, string name = null)
    {
      if ((object) id is Guid)
      {
        if (id.Equals((object) Guid.Empty))
          throw new ArgumentException("Identifier cannot equal Guid.Empty.", name);
      }
      else
      {
        IdentityDescriptor descriptor = (object) id as IdentityDescriptor;
        if (!(descriptor != (IdentityDescriptor) null))
          throw new ArgumentException("Identifier is an unsupported type: " + id?.ToString(), name);
        if (type == AadServiceUtils.IdentifierType.Group && !AadIdentityHelper.IsAadGroup(descriptor))
          throw new ArgumentException("Identifier is not an AAD group descriptor.", name);
      }
    }

    internal static void ValidateIds<T>(
      IEnumerable<T> ids,
      AadServiceUtils.IdentifierType type = AadServiceUtils.IdentifierType.Unknown,
      string name = null)
    {
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) ids, name);
      switch (ids)
      {
        case IEnumerable<Guid> source1:
          if (!source1.Contains<Guid>(Guid.Empty))
            break;
          throw new ArgumentException("Identifiers cannot contain Guid.Empty.", name);
        case IEnumerable<IdentityDescriptor> source2:
          if (source2.Contains<IdentityDescriptor>((IdentityDescriptor) null))
            throw new ArgumentException("Identifiers cannot contain null.", name);
          if (type != AadServiceUtils.IdentifierType.Group)
            break;
          using (IEnumerator<IdentityDescriptor> enumerator = source2.GetEnumerator())
          {
            while (enumerator.MoveNext())
            {
              if (!AadIdentityHelper.IsAadGroup(enumerator.Current))
                throw new ArgumentException("Identifier is not an AAD group descriptor.", name);
            }
            break;
          }
        default:
          throw new ArgumentException("Identifiers is an unsupported type: " + ids?.ToString(), name);
      }
    }

    internal static IEnumerable<KeyValuePair<T, Guid>> MapIds<T>(
      IVssRequestContext context,
      IEnumerable<T> ids,
      AadServiceUtils.IdentifierType type = AadServiceUtils.IdentifierType.Unknown)
    {
      switch (ids)
      {
        case IEnumerable<Guid> source:
          return (IEnumerable<KeyValuePair<T, Guid>>) source.Select<Guid, KeyValuePair<Guid, Guid>>((Func<Guid, KeyValuePair<Guid, Guid>>) (guid => new KeyValuePair<Guid, Guid>(guid, guid)));
        case IEnumerable<IdentityDescriptor> identityDescriptors:
          type = AadServiceUtils.ResolveIdentifierType(context, identityDescriptors, type);
          if (type != AadServiceUtils.IdentifierType.User)
          {
            if (type == AadServiceUtils.IdentifierType.Group)
              return (IEnumerable<KeyValuePair<T, Guid>>) identityDescriptors.Select<IdentityDescriptor, KeyValuePair<IdentityDescriptor, Guid>>((Func<IdentityDescriptor, KeyValuePair<IdentityDescriptor, Guid>>) (d => new KeyValuePair<IdentityDescriptor, Guid>(d, AadIdentityHelper.ExtractAadGroupId(d)))).ToList<KeyValuePair<IdentityDescriptor, Guid>>();
            throw new AadInternalException("Unsupported identifier type: " + type.ToString());
          }
          IList<Microsoft.VisualStudio.Services.Identity.Identity> second;
          if (context.ServiceHost.Is(TeamFoundationHostType.Deployment))
          {
            context.TraceSerializedConditionally(1035001, TraceLevel.Info, "VisualStudio.Services.Aad", "Service", "Trying to get ObjectId with descriptor in deployment level. ");
            second = context.GetService<IdentityService>().ReadIdentities(context, (IList<IdentityDescriptor>) identityDescriptors.ToList<IdentityDescriptor>(), QueryMembership.None, (IEnumerable<string>) null);
          }
          else
            second = (IList<Microsoft.VisualStudio.Services.Identity.Identity>) context.GetService<IdentityRetrievalService>().GetMaterializedUsers(context, (IEnumerable<IdentityDescriptor>) identityDescriptors.ToList<IdentityDescriptor>()).Values.ToList<Microsoft.VisualStudio.Services.Identity.Identity>();
          // ISSUE: reference to a compiler-generated field
          // ISSUE: reference to a compiler-generated field
          return (IEnumerable<KeyValuePair<T, Guid>>) identityDescriptors.Zip<IdentityDescriptor, Microsoft.VisualStudio.Services.Identity.Identity, KeyValuePair<IdentityDescriptor, Guid>>((IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) second, AadServiceUtils.\u003C\u003EO.\u003C0\u003E__GetUserId ?? (AadServiceUtils.\u003C\u003EO.\u003C0\u003E__GetUserId = new Func<IdentityDescriptor, Microsoft.VisualStudio.Services.Identity.Identity, KeyValuePair<IdentityDescriptor, Guid>>(AadServiceUtils.GetUserId))).ToList<KeyValuePair<IdentityDescriptor, Guid>>();
        default:
          throw new AadInternalException("Unsupported identifier type: " + typeof (T)?.ToString());
      }
    }

    private static AadServiceUtils.IdentifierType ResolveIdentifierType(
      IVssRequestContext context,
      IEnumerable<IdentityDescriptor> descriptors,
      AadServiceUtils.IdentifierType type)
    {
      if (type != AadServiceUtils.IdentifierType.Unknown)
        return type;
      IdentityDescriptor descriptor = descriptors.First<IdentityDescriptor>();
      if (AadIdentityHelper.IsAadUser(descriptor))
        return AadServiceUtils.IdentifierType.User;
      if (AadIdentityHelper.IsAadGroup(descriptor))
        return AadServiceUtils.IdentifierType.Group;
      throw new AadIdentityException("Identity descriptor is not an AAD user or group.")
      {
        Identity = descriptor
      };
    }

    private static KeyValuePair<IdentityDescriptor, Guid> GetUserId(
      IdentityDescriptor descriptor,
      Microsoft.VisualStudio.Services.Identity.Identity identity)
    {
      string input = identity != null ? identity.GetProperty<string>("http://schemas.microsoft.com/identity/claims/objectidentifier", (string) null) : throw new AadIdentityException("Identity not found.")
      {
        Identity = descriptor
      };
      if (string.IsNullOrWhiteSpace(input))
        throw new AadIdentityException("Identity missing AAD object ID.")
        {
          Identity = descriptor
        };
      Guid result;
      if (!Guid.TryParse(input, out result))
        throw new AadIdentityException("Identity has an AAD object ID but it is not a GUID.")
        {
          Identity = descriptor
        };
      return new KeyValuePair<IdentityDescriptor, Guid>(descriptor, result);
    }

    internal static IDictionary<K, V> ConvertValues<K, V>(
      IVssRequestContext context,
      IDictionary<K, Guid> ids,
      IDictionary<Guid, V> values)
    {
      return (IDictionary<K, V>) ids.ToDictionary<KeyValuePair<K, Guid>, K, V>((Func<KeyValuePair<K, Guid>, K>) (kvp => kvp.Key), (Func<KeyValuePair<K, Guid>, V>) (kvp => AadServiceUtils.GetValueOrDefault<Guid, V>(values, kvp.Value)));
    }

    internal static V GetValueOrDefault<K, V>(IDictionary<K, V> values, K key)
    {
      V valueOrDefault;
      values.TryGetValue(key, out valueOrDefault);
      return valueOrDefault;
    }

    public static AadObjectType GetAadObjectType<TObject>() where TObject : AadObject
    {
      if (typeof (AadUser) == typeof (TObject))
        return AadObjectType.User;
      return typeof (AadGroup) == typeof (TObject) ? AadObjectType.Group : AadObjectType.Unknown;
    }

    public static IDictionary<Guid, bool> AreObjectsVirtuallyInScope(
      IVssRequestContext requestContext,
      AadObjectType objectType,
      IEnumerable<Guid> objectIds)
    {
      ArgumentUtility.CheckForNull<IEnumerable<Guid>>(objectIds, nameof (objectIds));
      objectIds = (IEnumerable<Guid>) objectIds.Distinct<Guid>().ToList<Guid>();
      IDictionary<Guid, List<Microsoft.VisualStudio.Services.Identity.Identity>> ancestorsGroups = AadServiceUtils.GetAncestorAadGroupsInScope(requestContext, objectType, objectIds);
      return (IDictionary<Guid, bool>) objectIds.ToDictionary<Guid, Guid, bool>((Func<Guid, Guid>) (x => x), (Func<Guid, bool>) (x => ancestorsGroups.ContainsKey(x) && !ancestorsGroups[x].IsNullOrEmpty<Microsoft.VisualStudio.Services.Identity.Identity>()));
    }

    public static IDictionary<Guid, List<Microsoft.VisualStudio.Services.Identity.Identity>> GetAncestorAadGroupsInScope(
      IVssRequestContext requestContext,
      AadObjectType objectType,
      IEnumerable<Guid> objectIds)
    {
      ArgumentUtility.CheckForNull<IEnumerable<Guid>>(objectIds, nameof (objectIds));
      objectIds = (IEnumerable<Guid>) objectIds.Distinct<Guid>().ToList<Guid>();
      if (requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment) || objectIds.Count<Guid>() == 0)
        return (IDictionary<Guid, List<Microsoft.VisualStudio.Services.Identity.Identity>>) objectIds.ToDictionary<Guid, Guid, List<Microsoft.VisualStudio.Services.Identity.Identity>>((Func<Guid, Guid>) (x => x), (Func<Guid, List<Microsoft.VisualStudio.Services.Identity.Identity>>) (x => (List<Microsoft.VisualStudio.Services.Identity.Identity>) null));
      AadService service = requestContext.GetService<AadService>();
      List<Guid> enumerable = (List<Guid>) null;
      switch (objectType)
      {
        case AadObjectType.User:
          enumerable = service.GetUsersWithIds<Guid>(requestContext.Elevate(), new GetUsersWithIdsRequest<Guid>()
          {
            Identifiers = objectIds
          }).Users.Where<KeyValuePair<Guid, AadUser>>((Func<KeyValuePair<Guid, AadUser>, bool>) (x => x.Value != null)).Select<KeyValuePair<Guid, AadUser>, Guid>((Func<KeyValuePair<Guid, AadUser>, Guid>) (x => x.Key)).ToList<Guid>();
          break;
        case AadObjectType.Group:
          enumerable = service.GetGroupsWithIds<Guid>(requestContext.Elevate(), new GetGroupsWithIdsRequest<Guid>()
          {
            Identifiers = objectIds
          }).Groups.Where<KeyValuePair<Guid, AadGroup>>((Func<KeyValuePair<Guid, AadGroup>, bool>) (x => x.Value != null)).Select<KeyValuePair<Guid, AadGroup>, Guid>((Func<KeyValuePair<Guid, AadGroup>, Guid>) (x => x.Key)).ToList<Guid>();
          break;
        case AadObjectType.ServicePrincipal:
          enumerable = service.GetServicePrincipalsByIds(requestContext.Elevate(), new GetServicePrincipalsByIdsRequest()
          {
            Identifiers = objectIds
          }).ServicePrincipals.Where<KeyValuePair<Guid, AadServicePrincipal>>((Func<KeyValuePair<Guid, AadServicePrincipal>, bool>) (x => x.Value != null)).Select<KeyValuePair<Guid, AadServicePrincipal>, Guid>((Func<KeyValuePair<Guid, AadServicePrincipal>, Guid>) (x => x.Key)).ToList<Guid>();
          break;
      }
      if (enumerable.IsNullOrEmpty<Guid>())
        return (IDictionary<Guid, List<Microsoft.VisualStudio.Services.Identity.Identity>>) objectIds.ToDictionary<Guid, Guid, List<Microsoft.VisualStudio.Services.Identity.Identity>>((Func<Guid, Guid>) (x => x), (Func<Guid, List<Microsoft.VisualStudio.Services.Identity.Identity>>) (x => (List<Microsoft.VisualStudio.Services.Identity.Identity>) null));
      IDictionary<Guid, ISet<Guid>> ancestors = service.GetAncestorIds<Guid>(requestContext.Elevate(), new GetAncestorIdsRequest<Guid>()
      {
        ObjectType = objectType,
        Identifiers = (IEnumerable<Guid>) enumerable,
        Expand = -1
      }).Ancestors;
      HashSet<IdentityDescriptor> source1 = new HashSet<IdentityDescriptor>((IEqualityComparer<IdentityDescriptor>) IdentityDescriptorComparer.Instance);
      foreach (ISet<Guid> source2 in ancestors.Values.Where<ISet<Guid>>((Func<ISet<Guid>, bool>) (x => x != null)))
        source1.UnionWith(source2.Select<Guid, IdentityDescriptor>((Func<Guid, IdentityDescriptor>) (groupId => IdentityHelper.CreateDescriptorFromSid(SidIdentityHelper.ConstructAadGroupSid(groupId)))));
      Dictionary<Guid, Microsoft.VisualStudio.Services.Identity.Identity> aadGroups = requestContext.GetService<IdentityService>().ReadIdentities(requestContext.Elevate(), (IList<IdentityDescriptor>) source1.ToArray<IdentityDescriptor>(), QueryMembership.None, (IEnumerable<string>) null).Where<Microsoft.VisualStudio.Services.Identity.Identity>((Func<Microsoft.VisualStudio.Services.Identity.Identity, bool>) (x => x != null && x.IsActive)).ToDictionary<Microsoft.VisualStudio.Services.Identity.Identity, Guid>((Func<Microsoft.VisualStudio.Services.Identity.Identity, Guid>) (x => AadIdentityHelper.ExtractAadGroupId(x.Descriptor)));
      return (IDictionary<Guid, List<Microsoft.VisualStudio.Services.Identity.Identity>>) objectIds.ToDictionary<Guid, Guid, List<Microsoft.VisualStudio.Services.Identity.Identity>>((Func<Guid, Guid>) (x => x), (Func<Guid, List<Microsoft.VisualStudio.Services.Identity.Identity>>) (x => !ancestors.ContainsKey(x) || ancestors[x].IsNullOrEmpty<Guid>() ? new List<Microsoft.VisualStudio.Services.Identity.Identity>() : ancestors[x].Where<Guid>((Func<Guid, bool>) (y => aadGroups.ContainsKey(y))).Select<Guid, Microsoft.VisualStudio.Services.Identity.Identity>((Func<Guid, Microsoft.VisualStudio.Services.Identity.Identity>) (y => aadGroups[y])).ToList<Microsoft.VisualStudio.Services.Identity.Identity>()));
    }

    public static bool IsAadGroup(IdentityDescriptor descriptor) => AadIdentityHelper.IsAadGroup(descriptor);

    public static AadUser GetAadUser(
      IVssRequestContext requestContext,
      IdentityDescriptor descriptor,
      Guid targetTenantId)
    {
      IVssRequestContext context = requestContext.To(TeamFoundationHostType.Deployment).Elevate();
      requestContext.GetService<IdentityService>();
      string identityUpn = AadIdentityHelper.GetIdentityUpn(descriptor);
      if (!descriptor.IsAadUserType() || identityUpn == null)
        return (AadUser) null;
      AadService service = context.GetService<AadService>();
      GetUsersRequest getUsersRequest = new GetUsersRequest();
      getUsersRequest.UserPrincipalNamePrefixes = (IEnumerable<string>) new string[1]
      {
        identityUpn
      };
      getUsersRequest.MailPrefixes = (IEnumerable<string>) new string[1]
      {
        identityUpn
      };
      getUsersRequest.ToTenant = targetTenantId.ToString();
      GetUsersRequest request = getUsersRequest;
      IEnumerable<AadUser> users = service.GetUsers(context, request).Users;
      return users == null ? (AadUser) null : users.FirstOrDefault<AadUser>();
    }

    public static bool IsUserActiveInAAD(
      IVssRequestContext requestContext,
      IdentityDescriptor descriptor,
      Guid targetTenantId)
    {
      if (targetTenantId == Guid.Empty)
        return false;
      IVssRequestContext context1 = requestContext.To(TeamFoundationHostType.Deployment).Elevate();
      Microsoft.VisualStudio.Services.Identity.Identity readIdentity = requestContext.GetService<IdentityService>().ReadIdentities(requestContext, (IList<IdentityDescriptor>) new IdentityDescriptor[1]
      {
        descriptor
      }, QueryMembership.None, (IEnumerable<string>) null)?[0];
      Guid aadObjectId = readIdentity.GetAadObjectId();
      if (!AadIdentityHelper.IsAadUser((IReadOnlyVssIdentity) readIdentity) || !(aadObjectId != Guid.Empty))
        return false;
      AadService service = context1.GetService<AadService>();
      List<Guid> guidList = new List<Guid>() { aadObjectId };
      GetUsersWithIdsRequest<Guid> usersWithIdsRequest1 = new GetUsersWithIdsRequest<Guid>();
      usersWithIdsRequest1.Identifiers = (IEnumerable<Guid>) guidList;
      usersWithIdsRequest1.ToTenant = targetTenantId.ToString();
      GetUsersWithIdsRequest<Guid> usersWithIdsRequest2 = usersWithIdsRequest1;
      IVssRequestContext context2 = context1;
      GetUsersWithIdsRequest<Guid> request = usersWithIdsRequest2;
      AadUser user = service.GetUsersWithIds<Guid>(context2, request).Users[aadObjectId];
      return user != null && user.AccountEnabled;
    }

    public static bool IsAzureDevOpsAdministrator(
      IVssRequestContext requestContext,
      Guid userObjectId,
      Guid targetTenantId)
    {
      if (targetTenantId == Guid.Empty || userObjectId == Guid.Empty)
        return false;
      IVssRequestContext requestContext1 = requestContext.To(TeamFoundationHostType.Deployment).Elevate();
      List<AadDirectoryRole> devOpsAdminRoles = AadServiceUtils.GetAzureDevOpsAdminRoles(requestContext1, targetTenantId);
      if (devOpsAdminRoles == null)
        return false;
      if (requestContext.IsFeatureEnabled("VisualStudio.Services.Aad.IncludeIndirectMembershipsInRoleChecks"))
      {
        ISet<Guid> userRolesAndGroups = AadServiceUtils.GetUserRolesAndGroups(requestContext1, targetTenantId, userObjectId);
        foreach (AadDirectoryRole aadDirectoryRole in devOpsAdminRoles)
        {
          if (userRolesAndGroups.Contains(aadDirectoryRole.ObjectId))
            return true;
        }
      }
      foreach (AadDirectoryRole aadDirectoryRole in devOpsAdminRoles)
      {
        ISet<AadObject> roleMembers = AadServiceUtils.GetRoleMembers(requestContext1, targetTenantId, aadDirectoryRole.ObjectId);
        if (roleMembers != null && roleMembers.Any<AadObject>((Func<AadObject, bool>) (m => m.ObjectId == userObjectId)))
          return true;
      }
      return false;
    }

    public static List<AadUser> GetAzureDevOpsAdministrators(
      IVssRequestContext requestContext,
      Guid targetTenantId)
    {
      List<AadUser> opsAdministrators = new List<AadUser>();
      if (targetTenantId == Guid.Empty)
        return opsAdministrators;
      IVssRequestContext requestContext1 = requestContext.To(TeamFoundationHostType.Deployment).Elevate();
      AadDirectoryRole aadDirectoryRole = AadServiceUtils.GetAzureDevOpsAdminRoles(requestContext1, targetTenantId).FirstOrDefault<AadDirectoryRole>();
      return aadDirectoryRole == null ? opsAdministrators : (!requestContext.IsFeatureEnabled("VisualStudio.Services.Aad.IncludeIndirectMembershipsInRoleChecks") ? AadServiceUtils.GetRoleMembers(requestContext1, targetTenantId, aadDirectoryRole.ObjectId).OfType<AadUser>().ToList<AadUser>() : AadServiceUtils.GetTransitiveRoleMembers(requestContext1, targetTenantId, aadDirectoryRole.ObjectId));
    }

    public static List<AadUser> GetTransitiveRoleMembers(
      IVssRequestContext requestContext,
      Guid targetTenantId,
      Guid roleObjectId)
    {
      ArgumentUtility.CheckForEmptyGuid(targetTenantId, nameof (targetTenantId));
      ArgumentUtility.CheckForEmptyGuid(roleObjectId, nameof (roleObjectId));
      ISet<AadObject> roleMembers = AadServiceUtils.GetRoleMembers(requestContext, targetTenantId, roleObjectId);
      List<AadUser> transitiveRoleMembers = new List<AadUser>();
      foreach (AadObject aadObject in (IEnumerable<AadObject>) roleMembers)
      {
        if (!(aadObject is AadUser))
        {
          if (aadObject is AadGroup)
          {
            IEnumerable<AadUser> groupMembers = AadServiceUtils.GetGroupMembers(requestContext, targetTenantId, (aadObject as AadGroup).ObjectId);
            transitiveRoleMembers.AddRange(groupMembers);
          }
        }
        else
          transitiveRoleMembers.Add(aadObject as AadUser);
      }
      return transitiveRoleMembers;
    }

    private static List<AadDirectoryRole> GetAzureDevOpsAdminRoles(
      IVssRequestContext requestContext,
      Guid targetTenantId)
    {
      ArgumentUtility.CheckForEmptyGuid(targetTenantId, nameof (targetTenantId));
      AadService service = requestContext.GetService<AadService>();
      GetDirectoryRolesRequest directoryRolesRequest1 = new GetDirectoryRolesRequest();
      directoryRolesRequest1.ToTenant = targetTenantId.ToString();
      GetDirectoryRolesRequest directoryRolesRequest2 = directoryRolesRequest1;
      IVssRequestContext context = requestContext;
      GetDirectoryRolesRequest request = directoryRolesRequest2;
      GetDirectoryRolesResponse directoryRoles1 = service.GetDirectoryRoles(context, request);
      IEnumerable<AadDirectoryRole> source;
      if (directoryRoles1 == null)
      {
        source = (IEnumerable<AadDirectoryRole>) null;
      }
      else
      {
        IEnumerable<AadDirectoryRole> directoryRoles2 = directoryRoles1.DirectoryRoles;
        source = directoryRoles2 != null ? directoryRoles2.Where<AadDirectoryRole>((Func<AadDirectoryRole, bool>) (role => "e3973bdf-4987-49ae-837a-ba8e231c7286".Equals(role.RoleTemplateId, StringComparison.OrdinalIgnoreCase))) : (IEnumerable<AadDirectoryRole>) null;
      }
      return source == null ? (List<AadDirectoryRole>) null : source.ToList<AadDirectoryRole>();
    }

    private static ISet<AadObject> GetRoleMembers(
      IVssRequestContext requestContext,
      Guid targetTenantId,
      Guid roleObjectId)
    {
      ArgumentUtility.CheckForEmptyGuid(targetTenantId, nameof (targetTenantId));
      ArgumentUtility.CheckForEmptyGuid(roleObjectId, nameof (roleObjectId));
      AadService service = requestContext.GetService<AadService>();
      GetDirectoryRoleMembersRequest roleMembersRequest1 = new GetDirectoryRoleMembersRequest();
      roleMembersRequest1.DirectoryRoleObjectId = roleObjectId;
      roleMembersRequest1.ToTenant = targetTenantId.ToString();
      GetDirectoryRoleMembersRequest roleMembersRequest2 = roleMembersRequest1;
      IVssRequestContext context = requestContext;
      GetDirectoryRoleMembersRequest request = roleMembersRequest2;
      return service.GetDirectoryRoleMembers(context, request).Members;
    }

    private static ISet<Guid> GetUserRolesAndGroups(
      IVssRequestContext requestContext,
      Guid targetTenantId,
      Guid identityObjectId)
    {
      ArgumentUtility.CheckForEmptyGuid(targetTenantId, nameof (targetTenantId));
      ArgumentUtility.CheckForEmptyGuid(identityObjectId, nameof (identityObjectId));
      GetUserRolesAndGroupsRequest andGroupsRequest = new GetUserRolesAndGroupsRequest();
      andGroupsRequest.UserObjectId = identityObjectId;
      andGroupsRequest.ToTenant = targetTenantId.ToString();
      GetUserRolesAndGroupsRequest request = andGroupsRequest;
      return requestContext.GetService<AadService>().GetUserRolesAndGroups(requestContext, request).Members;
    }

    private static IEnumerable<AadUser> GetGroupMembers(
      IVssRequestContext requestContext,
      Guid targetTenantId,
      Guid groupObjectId)
    {
      ArgumentUtility.CheckForEmptyGuid(targetTenantId, nameof (targetTenantId));
      ArgumentUtility.CheckForEmptyGuid(groupObjectId, nameof (groupObjectId));
      GetDescendantsRequest<Guid> descendantsRequest = new GetDescendantsRequest<Guid>();
      descendantsRequest.Identifier = groupObjectId;
      descendantsRequest.ToTenant = targetTenantId.ToString();
      GetDescendantsRequest<Guid> request = descendantsRequest;
      return requestContext.GetService<AadService>().GetDescendants<Guid>(requestContext, request).Descendants.OfType<AadUser>();
    }

    internal enum IdentifierType
    {
      Unknown,
      User,
      Group,
      AadServicePrincipal,
    }
  }
}
