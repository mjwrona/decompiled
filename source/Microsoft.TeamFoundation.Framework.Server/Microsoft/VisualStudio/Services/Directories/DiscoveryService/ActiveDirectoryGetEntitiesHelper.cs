// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Directories.DiscoveryService.ActiveDirectoryGetEntitiesHelper
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Directories.DiscoveryService
{
  internal static class ActiveDirectoryGetEntitiesHelper
  {
    internal static DirectoryInternalGetEntitiesResponse GetEntities(
      IVssRequestContext context,
      DirectoryInternalGetEntitiesRequest request)
    {
      if (!ActiveDirectoryRequestFilter.AllowRequest(context, (DirectoryInternalRequest) request))
        return new DirectoryInternalGetEntitiesResponse();
      DirectoryInternalGetEntitiesResponse entities = new DirectoryInternalGetEntitiesResponse()
      {
        Results = (IDictionary<DirectoryEntityIdentifier, DirectoryInternalGetEntityResult>) request.EntityIds.ToDictionary<DirectoryEntityIdentifier, DirectoryEntityIdentifier, DirectoryInternalGetEntityResult>((Func<DirectoryEntityIdentifier, DirectoryEntityIdentifier>) (id => id), (Func<DirectoryEntityIdentifier, DirectoryInternalGetEntityResult>) (id => (DirectoryInternalGetEntityResult) null), (IEqualityComparer<DirectoryEntityIdentifier>) DirectoryEntityIdentifier.Comparer)
      };
      ActiveDirectoryGetEntitiesHelper.PopulateEntitiesFromAd(context, request.PropertiesToReturn, entities.Results);
      return entities;
    }

    internal static void PopulateEntitiesFromAd(
      IVssRequestContext context,
      IEnumerable<string> propertiesToReturn,
      IDictionary<DirectoryEntityIdentifier, DirectoryInternalGetEntityResult> results)
    {
      Dictionary<DirectoryEntityIdentifier, string> dictionary = new Dictionary<DirectoryEntityIdentifier, string>((IEqualityComparer<DirectoryEntityIdentifier>) DirectoryEntityIdentifier.Comparer);
      foreach (DirectoryEntityIdentifier entityIdentifier in results.Keys.ToList<DirectoryEntityIdentifier>())
      {
        string objectSid;
        if (ActiveDirectoryHelper.TryGetObjectSid(entityIdentifier, out objectSid))
          dictionary.Add(entityIdentifier, objectSid);
        else
          results[entityIdentifier] = new DirectoryInternalGetEntityResult()
          {
            Exception = (Exception) new DirectoryEntityNotFoundException(FrameworkResources.UnableToResolveObjectSid((object) entityIdentifier.ToString()))
          };
      }
      if (dictionary.Count == 0)
        return;
      try
      {
        IDictionary<string, IDirectoryEntity> directoryEntities = ActiveDirectoryHelper.Instance.GetDirectoryEntities(context, (IList<string>) dictionary.Values.ToList<string>(), propertiesToReturn);
        foreach (KeyValuePair<DirectoryEntityIdentifier, string> keyValuePair in dictionary)
        {
          DirectoryEntityIdentifier key1 = keyValuePair.Key;
          string key2 = keyValuePair.Value;
          IDirectoryEntity directoryEntity = (IDirectoryEntity) null;
          if (directoryEntities.TryGetValue(key2, out directoryEntity) && directoryEntity != null)
            results[key1] = new DirectoryInternalGetEntityResult()
            {
              Entity = directoryEntity
            };
          else
            results[key1] = new DirectoryInternalGetEntityResult()
            {
              Exception = (Exception) new DirectoryEntityNotFoundException()
            };
        }
      }
      catch (Exception ex)
      {
        foreach (DirectoryEntityIdentifier key in dictionary.Keys)
        {
          if (results[key] == null)
            results[key] = new DirectoryInternalGetEntityResult()
            {
              Exception = (Exception) new DirectoryEntityNotFoundException(ex.Message, ex)
            };
        }
      }
    }
  }
}
