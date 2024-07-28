// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Directories.DiscoveryService.AzureActiveDirectoryGetEntitiesHelper
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Aad;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Directories.DiscoveryService
{
  internal static class AzureActiveDirectoryGetEntitiesHelper
  {
    internal static DirectoryInternalGetEntitiesResponse GetEntities(
      IVssRequestContext context,
      DirectoryInternalGetEntitiesRequest request)
    {
      DirectoryInternalGetEntitiesResponse entities = new DirectoryInternalGetEntitiesResponse()
      {
        Results = (IDictionary<DirectoryEntityIdentifier, DirectoryInternalGetEntityResult>) request.EntityIds.ToDictionary<DirectoryEntityIdentifier, DirectoryEntityIdentifier, DirectoryInternalGetEntityResult>((Func<DirectoryEntityIdentifier, DirectoryEntityIdentifier>) (id => id), (Func<DirectoryEntityIdentifier, DirectoryInternalGetEntityResult>) (id => (DirectoryInternalGetEntityResult) null), (IEqualityComparer<DirectoryEntityIdentifier>) DirectoryEntityIdentifier.Comparer)
      };
      if (AzureActiveDirectoryRequestFilter.AllowRequest(context, (DirectoryInternalRequest) request))
      {
        AzureActiveDirectoryGetEntitiesHelper.ResolveAadMembersIds(context, request.PropertiesToReturn, entities.Results);
        AzureActiveDirectoryGetEntitiesHelper.ResolveAadGroupIds(context, request.PropertiesToReturn, entities.Results);
      }
      return entities;
    }

    private static void ResolveAadMembersIds(
      IVssRequestContext context,
      IEnumerable<string> properties,
      IDictionary<DirectoryEntityIdentifier, DirectoryInternalGetEntityResult> results)
    {
      List<KeyValuePair<DirectoryEntityIdentifier, Guid>> source = new List<KeyValuePair<DirectoryEntityIdentifier, Guid>>(results.Count);
      foreach (KeyValuePair<DirectoryEntityIdentifier, DirectoryInternalGetEntityResult> result in (IEnumerable<KeyValuePair<DirectoryEntityIdentifier, DirectoryInternalGetEntityResult>>) results)
      {
        if (result.Value == null)
        {
          Guid oid;
          if (AzureActiveDirectoryEntityIdentifierHelper.TryGetUserOid(result.Key, out oid))
            source.Add(new KeyValuePair<DirectoryEntityIdentifier, Guid>(result.Key, oid));
          else if (AzureActiveDirectoryEntityIdentifierHelper.TryGetServicePrincipalOid(result.Key, out oid))
            source.Add(new KeyValuePair<DirectoryEntityIdentifier, Guid>(result.Key, oid));
        }
      }
      if (source.Count == 0)
        return;
      AadService service = context.GetService<AadService>();
      GetUsersWithIdsResponse<Guid> usersWithIds = service.GetUsersWithIds<Guid>(context, new GetUsersWithIdsRequest<Guid>()
      {
        Identifiers = (IEnumerable<Guid>) source.Select<KeyValuePair<DirectoryEntityIdentifier, Guid>, Guid>((Func<KeyValuePair<DirectoryEntityIdentifier, Guid>, Guid>) (kvp => kvp.Value)).ToList<Guid>()
      });
      GetServicePrincipalsByIdsResponse principalsByIdsResponse = (GetServicePrincipalsByIdsResponse) null;
      if (AadServicePrincipalConfigurationHelper.Instance.IsGroupRulesForServicePrincipalsEnabled(context))
        principalsByIdsResponse = service.GetServicePrincipalsByIds(context, new GetServicePrincipalsByIdsRequest()
        {
          Identifiers = (IEnumerable<Guid>) source.Select<KeyValuePair<DirectoryEntityIdentifier, Guid>, Guid>((Func<KeyValuePair<DirectoryEntityIdentifier, Guid>, Guid>) (kvp => kvp.Value)).ToList<Guid>()
        });
      foreach (KeyValuePair<DirectoryEntityIdentifier, Guid> keyValuePair in source)
      {
        DirectoryEntityIdentifier key = keyValuePair.Key;
        AadUser valueOrDefault1 = usersWithIds != null ? usersWithIds.Users.GetValueOrDefault<Guid, AadUser>(keyValuePair.Value) : (AadUser) null;
        AadServicePrincipal valueOrDefault2 = principalsByIdsResponse != null ? principalsByIdsResponse.ServicePrincipals.GetValueOrDefault<Guid, AadServicePrincipal>(keyValuePair.Value) : (AadServicePrincipal) null;
        if (valueOrDefault1 != null)
          results[key] = new DirectoryInternalGetEntityResult()
          {
            Entity = (IDirectoryEntity) AzureActiveDirectoryEntityConverter.ConvertUser(context, valueOrDefault1, properties)
          };
        else if (valueOrDefault2 != null)
          results[key] = new DirectoryInternalGetEntityResult()
          {
            Entity = (IDirectoryEntity) AzureActiveDirectoryEntityConverter.ConvertServicePrincipal(context, valueOrDefault2, properties)
          };
        else
          results[key] = new DirectoryInternalGetEntityResult()
          {
            Exception = (Exception) new DirectoryEntityNotFoundException()
          };
      }
    }

    private static void ResolveAadGroupIds(
      IVssRequestContext context,
      IEnumerable<string> properties,
      IDictionary<DirectoryEntityIdentifier, DirectoryInternalGetEntityResult> results)
    {
      List<KeyValuePair<DirectoryEntityIdentifier, Guid>> source1 = new List<KeyValuePair<DirectoryEntityIdentifier, Guid>>(results.Count);
      foreach (KeyValuePair<DirectoryEntityIdentifier, DirectoryInternalGetEntityResult> result in (IEnumerable<KeyValuePair<DirectoryEntityIdentifier, DirectoryInternalGetEntityResult>>) results)
      {
        Guid oid;
        if (result.Value == null && AzureActiveDirectoryEntityIdentifierHelper.TryGetGroupOid(result.Key, out oid))
          source1.Add(new KeyValuePair<DirectoryEntityIdentifier, Guid>(result.Key, oid));
      }
      if (source1.Count == 0)
        return;
      AadService service = context.GetService<AadService>();
      GetGroupsWithIdsResponse<Guid> groupsWithIds = service.GetGroupsWithIds<Guid>(context, new GetGroupsWithIdsRequest<Guid>()
      {
        Identifiers = (IEnumerable<Guid>) source1.Select<KeyValuePair<DirectoryEntityIdentifier, Guid>, Guid>((Func<KeyValuePair<DirectoryEntityIdentifier, Guid>, Guid>) (kvp => kvp.Value)).ToList<Guid>()
      });
      List<KeyValuePair<DirectoryEntityIdentifier, Guid>> source2 = new List<KeyValuePair<DirectoryEntityIdentifier, Guid>>(source1.Count);
      foreach (KeyValuePair<DirectoryEntityIdentifier, Guid> keyValuePair in source1)
      {
        DirectoryEntityIdentifier key = keyValuePair.Key;
        AadGroup group = groupsWithIds.Groups[keyValuePair.Value];
        if (group != null)
          results[key] = new DirectoryInternalGetEntityResult()
          {
            Entity = (IDirectoryEntity) AzureActiveDirectoryEntityConverter.ConvertGroup(context, group, properties)
          };
        else
          source2.Add(keyValuePair);
      }
      if (!source2.Any<KeyValuePair<DirectoryEntityIdentifier, Guid>>())
        return;
      GetDirectoryRolesWithIdsResponse directoryRolesWithIds = service.GetDirectoryRolesWithIds(context, new GetDirectoryRolesWithIdsRequest()
      {
        Identifiers = (IEnumerable<Guid>) source2.Select<KeyValuePair<DirectoryEntityIdentifier, Guid>, Guid>((Func<KeyValuePair<DirectoryEntityIdentifier, Guid>, Guid>) (kvp => kvp.Value)).ToList<Guid>()
      });
      foreach (KeyValuePair<DirectoryEntityIdentifier, Guid> keyValuePair in source2)
      {
        DirectoryEntityIdentifier key = keyValuePair.Key;
        AadDirectoryRole directoryRole = directoryRolesWithIds.DirectoryRoles[keyValuePair.Value];
        if (directoryRole != null)
          results[key] = new DirectoryInternalGetEntityResult()
          {
            Entity = AzureActiveDirectoryEntityConverter.ConvertObject(context, (AadObject) directoryRole, properties)
          };
        else
          results[key] = new DirectoryInternalGetEntityResult()
          {
            Exception = (Exception) new DirectoryEntityNotFoundException()
          };
      }
    }
  }
}
