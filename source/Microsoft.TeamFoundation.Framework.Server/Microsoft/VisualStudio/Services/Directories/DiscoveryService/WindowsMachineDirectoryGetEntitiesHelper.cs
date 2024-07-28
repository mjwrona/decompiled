// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Directories.DiscoveryService.WindowsMachineDirectoryGetEntitiesHelper
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;

namespace Microsoft.VisualStudio.Services.Directories.DiscoveryService
{
  internal static class WindowsMachineDirectoryGetEntitiesHelper
  {
    internal static DirectoryInternalGetEntitiesResponse GetEntities(
      IVssRequestContext context,
      DirectoryInternalGetEntitiesRequest request)
    {
      if (!WindowsMachineDirectoryRequestFilter.AllowRequest(context, (DirectoryInternalRequest) request))
        return new DirectoryInternalGetEntitiesResponse();
      DirectoryInternalGetEntitiesResponse entities = new DirectoryInternalGetEntitiesResponse()
      {
        Results = (IDictionary<DirectoryEntityIdentifier, DirectoryInternalGetEntityResult>) request.EntityIds.ToDictionary<DirectoryEntityIdentifier, DirectoryEntityIdentifier, DirectoryInternalGetEntityResult>((Func<DirectoryEntityIdentifier, DirectoryEntityIdentifier>) (id => id), (Func<DirectoryEntityIdentifier, DirectoryInternalGetEntityResult>) (id => (DirectoryInternalGetEntityResult) null), (IEqualityComparer<DirectoryEntityIdentifier>) DirectoryEntityIdentifier.Comparer)
      };
      WindowsMachineDirectoryGetEntitiesHelper.PopulateEntitiesFromMd(context, request.PropertiesToReturn, entities.Results);
      return entities;
    }

    internal static void PopulateEntitiesFromMd(
      IVssRequestContext context,
      IEnumerable<string> propertiesToReturn,
      IDictionary<DirectoryEntityIdentifier, DirectoryInternalGetEntityResult> results)
    {
      Dictionary<DirectoryEntityIdentifier, SecurityIdentifier> dictionary = new Dictionary<DirectoryEntityIdentifier, SecurityIdentifier>((IEqualityComparer<DirectoryEntityIdentifier>) DirectoryEntityIdentifier.Comparer);
      foreach (DirectoryEntityIdentifier entityIdentifier in results.Keys.ToList<DirectoryEntityIdentifier>())
      {
        SecurityIdentifier objectSid;
        if (!WindowsMachineDirectoryHelper.TryGetObjectSid(entityIdentifier, out objectSid))
          results[entityIdentifier] = new DirectoryInternalGetEntityResult()
          {
            Exception = (Exception) new DirectoryEntityNotFoundException(FrameworkResources.UnableToResolveObjectSid((object) entityIdentifier.ToString()))
          };
        else
          dictionary.Add(entityIdentifier, objectSid);
      }
      if (dictionary.Count == 0)
        return;
      try
      {
        IDictionary<SecurityIdentifier, IDirectoryEntity> directoryEntities = WindowsMachineDirectoryHelper.Instance.GetDirectoryEntities(context, (IEnumerable<SecurityIdentifier>) dictionary.Values, propertiesToReturn);
        foreach (KeyValuePair<DirectoryEntityIdentifier, SecurityIdentifier> keyValuePair in dictionary)
        {
          DirectoryEntityIdentifier key1 = keyValuePair.Key;
          SecurityIdentifier key2 = keyValuePair.Value;
          IDirectoryEntity directoryEntity;
          if (!directoryEntities.TryGetValue(key2, out directoryEntity) || directoryEntity == null)
            results[key1] = new DirectoryInternalGetEntityResult()
            {
              Exception = (Exception) new DirectoryEntityNotFoundException()
            };
          else
            results[key1] = new DirectoryInternalGetEntityResult()
            {
              Entity = directoryEntity
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
