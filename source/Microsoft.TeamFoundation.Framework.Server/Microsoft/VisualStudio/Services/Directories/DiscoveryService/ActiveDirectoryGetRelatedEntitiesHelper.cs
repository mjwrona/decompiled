// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Directories.DiscoveryService.ActiveDirectoryGetRelatedEntitiesHelper
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Directories.DiscoveryService
{
  internal static class ActiveDirectoryGetRelatedEntitiesHelper
  {
    private static readonly IEnumerable<string> DistinguishedNameProperty = (IEnumerable<string>) new string[1]
    {
      "Distinguishedname"
    };
    private static readonly IEnumerable<string> ManagerProperty = (IEnumerable<string>) new string[1]
    {
      "Manager"
    };
    private const int defaultMaxManagerLevels = 6;
    private const string c_ManagerChainFeature = "VisualStudio.Services.Ad.ManagerChain";

    internal static DirectoryInternalGetRelatedEntitiesResponse GetRelatedEntities(
      IVssRequestContext context,
      DirectoryInternalGetRelatedEntitiesRequest request)
    {
      if (!ActiveDirectoryRequestFilter.AllowRequest(context, (DirectoryInternalRequest) request))
        return new DirectoryInternalGetRelatedEntitiesResponse();
      DirectoryInternalGetRelatedEntitiesResponse relatedEntities = new DirectoryInternalGetRelatedEntitiesResponse()
      {
        Results = (IDictionary<DirectoryEntityIdentifier, DirectoryInternalGetRelatedEntitiesResult>) request.EntityIds.ToDictionary<DirectoryEntityIdentifier, DirectoryEntityIdentifier, DirectoryInternalGetRelatedEntitiesResult>((Func<DirectoryEntityIdentifier, DirectoryEntityIdentifier>) (id => id), (Func<DirectoryEntityIdentifier, DirectoryInternalGetRelatedEntitiesResult>) (id => (DirectoryInternalGetRelatedEntitiesResult) null), (IEqualityComparer<DirectoryEntityIdentifier>) DirectoryEntityIdentifier.Comparer)
      };
      switch (request.Relation)
      {
        case "Manager":
          ActiveDirectoryGetRelatedEntitiesHelper.PopulateDirectManagerChain(context, request.PropertiesToReturn, request.Depth, relatedEntities.Results);
          break;
        case "DirectReports":
          if (request.Depth == 1)
          {
            ActiveDirectoryGetRelatedEntitiesHelper.PopulateDirectReports(context, request.PropertiesToReturn, request.MaxResults, relatedEntities.Results);
            break;
          }
          break;
        case "Member":
          if (request.Depth == 1)
          {
            ActiveDirectoryGetRelatedEntitiesHelper.PopulateDirectMembers(context, request.PropertiesToReturn, relatedEntities.Results, request.MaxResults);
            break;
          }
          break;
      }
      return relatedEntities;
    }

    internal static void PopulateDirectManagerChain(
      IVssRequestContext context,
      IEnumerable<string> properties,
      int depth,
      IDictionary<DirectoryEntityIdentifier, DirectoryInternalGetRelatedEntitiesResult> results)
    {
      Dictionary<DirectoryEntityIdentifier, DirectoryInternalGetEntityResult> results1 = new Dictionary<DirectoryEntityIdentifier, DirectoryInternalGetEntityResult>((IEqualityComparer<DirectoryEntityIdentifier>) DirectoryEntityIdentifier.Comparer);
      foreach (DirectoryEntityIdentifier entityIdentifier in results.Keys.ToList<DirectoryEntityIdentifier>())
      {
        if (DirectoryEntityHelper.IsActiveDirectoryUser(entityIdentifier))
          results1[entityIdentifier] = (DirectoryInternalGetEntityResult) null;
      }
      if (results1.Count == 0)
        return;
      ActiveDirectoryGetEntitiesHelper.PopulateEntitiesFromAd(context, ActiveDirectoryGetRelatedEntitiesHelper.ManagerProperty, (IDictionary<DirectoryEntityIdentifier, DirectoryInternalGetEntityResult>) results1);
      int num = 1;
      if (context.IsFeatureEnabled("VisualStudio.Services.Ad.ManagerChain") && depth > 1)
        num = Math.Min(depth, 6);
      foreach (KeyValuePair<DirectoryEntityIdentifier, DirectoryInternalGetEntityResult> keyValuePair in results1)
      {
        DirectoryEntityIdentifier key = keyValuePair.Key;
        DirectoryInternalGetEntityResult internalGetEntityResult = keyValuePair.Value;
        if (internalGetEntityResult.Entity == null)
        {
          results[key] = new DirectoryInternalGetRelatedEntitiesResult()
          {
            Exception = internalGetEntityResult.Exception
          };
        }
        else
        {
          IDirectoryEntity entity = internalGetEntityResult.Entity;
          List<IDirectoryEntity> directoryEntityList = new List<IDirectoryEntity>();
          for (int index = 0; index < num; ++index)
          {
            string str = entity["Manager"] as string;
            if (!string.IsNullOrEmpty(str))
            {
              IDictionary<string, IDirectoryEntity> directoryEntities = ActiveDirectoryHelper.Instance.GetDirectoryEntities(context, (IList<string>) new string[1]
              {
                str
              }, properties, SearchAttribute.DistinguishedName);
              if (directoryEntities.Count != 0)
              {
                IDirectoryEntity directoryEntity = directoryEntities.First<KeyValuePair<string, IDirectoryEntity>>().Value;
                DirectoryEntityIdentifier decodedId = (DirectoryEntityIdentifier) null;
                if (directoryEntity != null)
                {
                  DirectoryEntityIdentifier.TryParse(directoryEntity.EntityId, out decodedId);
                  if (decodedId != null)
                  {
                    Dictionary<DirectoryEntityIdentifier, DirectoryInternalGetEntityResult> results2 = new Dictionary<DirectoryEntityIdentifier, DirectoryInternalGetEntityResult>((IEqualityComparer<DirectoryEntityIdentifier>) DirectoryEntityIdentifier.Comparer);
                    results2.Add(decodedId, (DirectoryInternalGetEntityResult) null);
                    ActiveDirectoryGetEntitiesHelper.PopulateEntitiesFromAd(context, properties, (IDictionary<DirectoryEntityIdentifier, DirectoryInternalGetEntityResult>) results2);
                    directoryEntityList.Add(results2[decodedId].Entity);
                    entity = results2[decodedId].Entity;
                  }
                  else
                    break;
                }
                else
                  break;
              }
              else
                break;
            }
            else
              break;
          }
          results[key] = new DirectoryInternalGetRelatedEntitiesResult()
          {
            Entities = (IEnumerable<IDirectoryEntity>) directoryEntityList
          };
        }
      }
    }

    internal static void PopulateDirectReports(
      IVssRequestContext context,
      IEnumerable<string> properties,
      int maxResults,
      IDictionary<DirectoryEntityIdentifier, DirectoryInternalGetRelatedEntitiesResult> results)
    {
      Dictionary<DirectoryEntityIdentifier, DirectoryInternalGetEntityResult> results1 = new Dictionary<DirectoryEntityIdentifier, DirectoryInternalGetEntityResult>((IEqualityComparer<DirectoryEntityIdentifier>) DirectoryEntityIdentifier.Comparer);
      foreach (KeyValuePair<DirectoryEntityIdentifier, DirectoryInternalGetRelatedEntitiesResult> result in (IEnumerable<KeyValuePair<DirectoryEntityIdentifier, DirectoryInternalGetRelatedEntitiesResult>>) results)
      {
        if (DirectoryEntityHelper.IsActiveDirectoryUser(result.Key))
          results1.Add(result.Key, (DirectoryInternalGetEntityResult) null);
      }
      if (results1.Count == 0)
        return;
      ActiveDirectoryGetEntitiesHelper.PopulateEntitiesFromAd(context, ActiveDirectoryGetRelatedEntitiesHelper.DistinguishedNameProperty, (IDictionary<DirectoryEntityIdentifier, DirectoryInternalGetEntityResult>) results1);
      Dictionary<DirectoryEntityIdentifier, string> dictionary = new Dictionary<DirectoryEntityIdentifier, string>((IEqualityComparer<DirectoryEntityIdentifier>) DirectoryEntityIdentifier.Comparer);
      foreach (KeyValuePair<DirectoryEntityIdentifier, DirectoryInternalGetEntityResult> keyValuePair in results1)
      {
        DirectoryEntityIdentifier key = keyValuePair.Key;
        DirectoryInternalGetEntityResult internalGetEntityResult = keyValuePair.Value;
        if (internalGetEntityResult.Entity == null)
          results[key] = new DirectoryInternalGetRelatedEntitiesResult()
          {
            Exception = internalGetEntityResult.Exception
          };
        else if (!(internalGetEntityResult.Entity["Distinguishedname"] is string str))
          results[key] = new DirectoryInternalGetRelatedEntitiesResult()
          {
            Entities = (IEnumerable<IDirectoryEntity>) Array.Empty<IDirectoryEntity>()
          };
        else
          dictionary.Add(key, str);
      }
      if (dictionary.Count == 0)
        return;
      foreach (KeyValuePair<DirectoryEntityIdentifier, string> keyValuePair in dictionary)
      {
        DirectoryEntityIdentifier key = keyValuePair.Key;
        string str = keyValuePair.Value;
        try
        {
          string filter = string.Format("(manager={0})", (object) str);
          results[key] = new DirectoryInternalGetRelatedEntitiesResult()
          {
            Entities = (IEnumerable<IDirectoryEntity>) ActiveDirectoryHelper.Instance.SearchAd(context, filter, maxResults, properties)
          };
        }
        catch (Exception ex)
        {
          results[key] = new DirectoryInternalGetRelatedEntitiesResult()
          {
            Exception = (Exception) new DirectoryRelatedEntitiesUnavailableException(ex.Message, ex)
          };
        }
      }
    }

    internal static void PopulateDirectMembers(
      IVssRequestContext context,
      IEnumerable<string> properties,
      IDictionary<DirectoryEntityIdentifier, DirectoryInternalGetRelatedEntitiesResult> results,
      int maxResults)
    {
      Dictionary<DirectoryEntityIdentifier, DirectoryInternalGetEntityResult> results1 = new Dictionary<DirectoryEntityIdentifier, DirectoryInternalGetEntityResult>((IEqualityComparer<DirectoryEntityIdentifier>) DirectoryEntityIdentifier.Comparer);
      foreach (KeyValuePair<DirectoryEntityIdentifier, DirectoryInternalGetRelatedEntitiesResult> result in (IEnumerable<KeyValuePair<DirectoryEntityIdentifier, DirectoryInternalGetRelatedEntitiesResult>>) results)
      {
        if (DirectoryEntityHelper.IsActiveDirectoryGroup(result.Key))
          results1.Add(result.Key, (DirectoryInternalGetEntityResult) null);
      }
      if (results1.Count == 0)
        return;
      ActiveDirectoryGetEntitiesHelper.PopulateEntitiesFromAd(context, ActiveDirectoryGetRelatedEntitiesHelper.DistinguishedNameProperty, (IDictionary<DirectoryEntityIdentifier, DirectoryInternalGetEntityResult>) results1);
      Dictionary<DirectoryEntityIdentifier, string> dictionary = new Dictionary<DirectoryEntityIdentifier, string>((IEqualityComparer<DirectoryEntityIdentifier>) DirectoryEntityIdentifier.Comparer);
      foreach (KeyValuePair<DirectoryEntityIdentifier, DirectoryInternalGetEntityResult> keyValuePair in results1)
      {
        DirectoryEntityIdentifier key = keyValuePair.Key;
        DirectoryInternalGetEntityResult internalGetEntityResult = keyValuePair.Value;
        if (internalGetEntityResult.Entity == null)
          results[key] = new DirectoryInternalGetRelatedEntitiesResult()
          {
            Exception = internalGetEntityResult.Exception
          };
        else if (!(internalGetEntityResult.Entity["Distinguishedname"] is string str))
          results[key] = new DirectoryInternalGetRelatedEntitiesResult()
          {
            Entities = (IEnumerable<IDirectoryEntity>) Array.Empty<IDirectoryEntity>()
          };
        else
          dictionary.Add(key, str);
      }
      if (dictionary.Count == 0)
        return;
      foreach (KeyValuePair<DirectoryEntityIdentifier, string> keyValuePair in dictionary)
      {
        DirectoryEntityIdentifier key = keyValuePair.Key;
        string str = keyValuePair.Value;
        try
        {
          string filter = string.Format("(memberOf={0})", (object) str);
          results[key] = new DirectoryInternalGetRelatedEntitiesResult()
          {
            Entities = (IEnumerable<IDirectoryEntity>) ActiveDirectoryHelper.Instance.SearchAd(context, filter, maxResults, properties)
          };
        }
        catch (Exception ex)
        {
          results[key] = new DirectoryInternalGetRelatedEntitiesResult()
          {
            Exception = (Exception) new DirectoryRelatedEntitiesUnavailableException(ex.Message, ex)
          };
        }
      }
    }
  }
}
