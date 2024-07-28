// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Directories.DiscoveryService.WindowsMachineDirectoryGetRelatedEntitiesHelper
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Linq;
using System.Security.Principal;

namespace Microsoft.VisualStudio.Services.Directories.DiscoveryService
{
  internal static class WindowsMachineDirectoryGetRelatedEntitiesHelper
  {
    private const string Members = "Members";

    internal static DirectoryInternalGetRelatedEntitiesResponse GetRelatedEntities(
      IVssRequestContext context,
      DirectoryInternalGetRelatedEntitiesRequest request)
    {
      if (!WindowsMachineDirectoryRequestFilter.AllowRequest(context, (DirectoryInternalRequest) request))
        return new DirectoryInternalGetRelatedEntitiesResponse();
      DirectoryInternalGetRelatedEntitiesResponse relatedEntities = new DirectoryInternalGetRelatedEntitiesResponse()
      {
        Results = (IDictionary<DirectoryEntityIdentifier, DirectoryInternalGetRelatedEntitiesResult>) request.EntityIds.ToDictionary<DirectoryEntityIdentifier, DirectoryEntityIdentifier, DirectoryInternalGetRelatedEntitiesResult>((Func<DirectoryEntityIdentifier, DirectoryEntityIdentifier>) (id => id), (Func<DirectoryEntityIdentifier, DirectoryInternalGetRelatedEntitiesResult>) (id => (DirectoryInternalGetRelatedEntitiesResult) null), (IEqualityComparer<DirectoryEntityIdentifier>) DirectoryEntityIdentifier.Comparer)
      };
      if (request.Relation == "Member" && request.Depth == 1)
        WindowsMachineDirectoryGetRelatedEntitiesHelper.PopulateDirectMembers(context, request.PropertiesToReturn, request.MaxResults, relatedEntities.Results);
      return relatedEntities;
    }

    internal static void PopulateDirectMembers(
      IVssRequestContext context,
      IEnumerable<string> properties,
      int maxResults,
      IDictionary<DirectoryEntityIdentifier, DirectoryInternalGetRelatedEntitiesResult> results)
    {
      HashSet<string> propertiesToReturn = WindowsMachineDirectoryHelper.GetAllPropertiesToReturn(properties);
      Dictionary<DirectoryEntityIdentifier, SecurityIdentifier> dictionary = new Dictionary<DirectoryEntityIdentifier, SecurityIdentifier>((IEqualityComparer<DirectoryEntityIdentifier>) DirectoryEntityIdentifier.Comparer);
      foreach (DirectoryEntityIdentifier entityIdentifier in results.Keys.ToList<DirectoryEntityIdentifier>())
      {
        if (WindowsMachineDirectoryEntityHelper.IsWindowsMachineDirectoryGroup(entityIdentifier))
        {
          SecurityIdentifier objectSid;
          if (!WindowsMachineDirectoryHelper.TryGetObjectSid(entityIdentifier, out objectSid))
            results[entityIdentifier] = new DirectoryInternalGetRelatedEntitiesResult()
            {
              Exception = (Exception) new DirectoryEntityNotFoundException(FrameworkResources.UnableToResolveObjectSid((object) entityIdentifier.ToString()))
            };
          else
            dictionary[entityIdentifier] = objectSid;
        }
      }
      IDictionary<SecurityIdentifier, DirectoryEntry> directoryEntries = WindowsMachineDirectoryHelper.Instance.GetDirectoryEntries(context, (IEnumerable<SecurityIdentifier>) dictionary.Values, "group");
      foreach (KeyValuePair<DirectoryEntityIdentifier, SecurityIdentifier> keyValuePair in dictionary)
      {
        DirectoryEntityIdentifier key1 = keyValuePair.Key;
        SecurityIdentifier key2 = keyValuePair.Value;
        DirectoryEntry directoryEntry;
        if (!directoryEntries.TryGetValue(key2, out directoryEntry) || directoryEntry == null)
        {
          results[key1] = new DirectoryInternalGetRelatedEntitiesResult()
          {
            Exception = (Exception) new DirectoryEntityNotFoundException(FrameworkResources.UnableToRetrieveDirectoryEntry((object) key1.ToString()))
          };
        }
        else
        {
          using (directoryEntry)
          {
            if (!(directoryEntry.Invoke("Members") is IEnumerable members))
            {
              results[key1] = new DirectoryInternalGetRelatedEntitiesResult()
              {
                Exception = (Exception) new DirectoryEntityNotFoundException(FrameworkResources.UnableToRetrieveDirectoryEntry((object) key1.ToString()))
              };
            }
            else
            {
              IEnumerable<IDirectoryEntity> directoryEntities = WindowsMachineDirectoryGetRelatedEntitiesHelper.GetDirectoryEntities(members, propertiesToReturn, maxResults);
              results[key1] = new DirectoryInternalGetRelatedEntitiesResult()
              {
                Entities = directoryEntities
              };
            }
          }
        }
      }
    }

    private static IEnumerable<IDirectoryEntity> GetDirectoryEntities(
      IEnumerable members,
      HashSet<string> allPropertiesToReturn,
      int maxResults)
    {
      SortedSet<IDirectoryEntity> directoryEntities = new SortedSet<IDirectoryEntity>(DirectoryEntityComparer.DisplayName);
      foreach (object member in members)
      {
        if (directoryEntities.Count < maxResults)
        {
          using (DirectoryEntry directoryEntry = DirectoryEntryFactory.CreateDirectoryEntry(member))
          {
            IDirectoryEntity directoryEntity = WindowsMachineDirectoryHelper.CreateDirectoryEntity(directoryEntry, (IEnumerable<string>) allPropertiesToReturn);
            if (directoryEntity != null)
              directoryEntities.Add(directoryEntity);
          }
        }
        else
          break;
      }
      return (IEnumerable<IDirectoryEntity>) directoryEntities;
    }
  }
}
