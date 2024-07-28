// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Directories.DiscoveryService.ActiveDirectoryGetRelatedEntityIdsHelper
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Directories.DiscoveryService
{
  internal static class ActiveDirectoryGetRelatedEntityIdsHelper
  {
    internal static DirectoryInternalGetRelatedEntityIdsResponse GetRelatedEntityIds(
      IVssRequestContext context,
      DirectoryInternalGetRelatedEntityIdsRequest request)
    {
      if (!ActiveDirectoryRequestFilter.AllowRequest(context, (DirectoryInternalRequest) request))
        return new DirectoryInternalGetRelatedEntityIdsResponse();
      DirectoryInternalGetRelatedEntityIdsResponse relatedEntityIds = new DirectoryInternalGetRelatedEntityIdsResponse()
      {
        Results = (IDictionary<DirectoryEntityIdentifier, DirectoryInternalGetRelatedEntityIdsResult>) request.EntityIds.ToDictionary<DirectoryEntityIdentifier, DirectoryEntityIdentifier, DirectoryInternalGetRelatedEntityIdsResult>((Func<DirectoryEntityIdentifier, DirectoryEntityIdentifier>) (id => id), (Func<DirectoryEntityIdentifier, DirectoryInternalGetRelatedEntityIdsResult>) (id => (DirectoryInternalGetRelatedEntityIdsResult) null), (IEqualityComparer<DirectoryEntityIdentifier>) DirectoryEntityIdentifier.Comparer)
      };
      switch (request.Relation)
      {
        case "Manager":
          ActiveDirectoryGetRelatedEntityIdsHelper.GetDirectManagerIds(context, request.Depth, relatedEntityIds.Results);
          break;
        case "Member":
          ActiveDirectoryGetRelatedEntityIdsHelper.GetDirectMembersIds(context, relatedEntityIds.Results, request.MaxResults);
          break;
      }
      return relatedEntityIds;
    }

    private static void GetDirectManagerIds(
      IVssRequestContext context,
      int depth,
      IDictionary<DirectoryEntityIdentifier, DirectoryInternalGetRelatedEntityIdsResult> results)
    {
      Dictionary<DirectoryEntityIdentifier, DirectoryInternalGetRelatedEntitiesResult> dictionary = results.ToDictionary<KeyValuePair<DirectoryEntityIdentifier, DirectoryInternalGetRelatedEntityIdsResult>, DirectoryEntityIdentifier, DirectoryInternalGetRelatedEntitiesResult>((Func<KeyValuePair<DirectoryEntityIdentifier, DirectoryInternalGetRelatedEntityIdsResult>, DirectoryEntityIdentifier>) (kvp => kvp.Key), (Func<KeyValuePair<DirectoryEntityIdentifier, DirectoryInternalGetRelatedEntityIdsResult>, DirectoryInternalGetRelatedEntitiesResult>) (kvp => (DirectoryInternalGetRelatedEntitiesResult) null));
      ActiveDirectoryGetRelatedEntitiesHelper.PopulateDirectManagerChain(context, (IEnumerable<string>) new string[0], depth, (IDictionary<DirectoryEntityIdentifier, DirectoryInternalGetRelatedEntitiesResult>) dictionary);
      foreach (KeyValuePair<DirectoryEntityIdentifier, DirectoryInternalGetRelatedEntitiesResult> keyValuePair in dictionary)
      {
        DirectoryEntityIdentifier key = keyValuePair.Key;
        DirectoryInternalGetRelatedEntitiesResult directoryInternalGetRelatedEntitiesResult = keyValuePair.Value;
        results[key] = ActiveDirectoryGetRelatedEntityIdsHelper.ToDirectoryInternalGetRelatedEntityIdsResult(directoryInternalGetRelatedEntitiesResult);
      }
    }

    private static void GetDirectMembersIds(
      IVssRequestContext context,
      IDictionary<DirectoryEntityIdentifier, DirectoryInternalGetRelatedEntityIdsResult> results,
      int maxResults)
    {
      Dictionary<DirectoryEntityIdentifier, DirectoryInternalGetRelatedEntitiesResult> dictionary = results.ToDictionary<KeyValuePair<DirectoryEntityIdentifier, DirectoryInternalGetRelatedEntityIdsResult>, DirectoryEntityIdentifier, DirectoryInternalGetRelatedEntitiesResult>((Func<KeyValuePair<DirectoryEntityIdentifier, DirectoryInternalGetRelatedEntityIdsResult>, DirectoryEntityIdentifier>) (kvp => kvp.Key), (Func<KeyValuePair<DirectoryEntityIdentifier, DirectoryInternalGetRelatedEntityIdsResult>, DirectoryInternalGetRelatedEntitiesResult>) (kvp => (DirectoryInternalGetRelatedEntitiesResult) null));
      ActiveDirectoryGetRelatedEntitiesHelper.PopulateDirectMembers(context, (IEnumerable<string>) new string[0], (IDictionary<DirectoryEntityIdentifier, DirectoryInternalGetRelatedEntitiesResult>) dictionary, maxResults);
      foreach (KeyValuePair<DirectoryEntityIdentifier, DirectoryInternalGetRelatedEntitiesResult> keyValuePair in dictionary)
      {
        DirectoryEntityIdentifier key = keyValuePair.Key;
        DirectoryInternalGetRelatedEntitiesResult directoryInternalGetRelatedEntitiesResult = keyValuePair.Value;
        results[key] = ActiveDirectoryGetRelatedEntityIdsHelper.ToDirectoryInternalGetRelatedEntityIdsResult(directoryInternalGetRelatedEntitiesResult);
      }
    }

    private static DirectoryInternalGetRelatedEntityIdsResult ToDirectoryInternalGetRelatedEntityIdsResult(
      this DirectoryInternalGetRelatedEntitiesResult directoryInternalGetRelatedEntitiesResult)
    {
      if (directoryInternalGetRelatedEntitiesResult == null)
        return (DirectoryInternalGetRelatedEntityIdsResult) null;
      return new DirectoryInternalGetRelatedEntityIdsResult()
      {
        EntityIds = directoryInternalGetRelatedEntitiesResult.Entities == null ? (IEnumerable<string>) null : directoryInternalGetRelatedEntitiesResult.Entities.Select<IDirectoryEntity, string>((Func<IDirectoryEntity, string>) (directoryEntity => directoryEntity.EntityId)),
        Exception = directoryInternalGetRelatedEntitiesResult.Exception
      };
    }
  }
}
