// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Directories.DiscoveryService.DirectoryGetRelatedEntitiesRequest
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Directories.DiscoveryService
{
  public class DirectoryGetRelatedEntitiesRequest : DirectoryPagedRequest
  {
    private const int absoluteMinResults = 1;
    private const int absoluteMaxResults = 200;

    public IEnumerable<string> EntityIds { get; set; }

    public string Relation { get; set; }

    public int Depth { get; set; }

    public IEnumerable<string> PropertiesToReturn { get; set; }

    internal override DirectoryResponse Execute(
      IVssRequestContext context,
      IEnumerable<IDirectory> directories)
    {
      string[] strArray = this.SanitizeAndFilterDirectories(context);
      string[] source1 = this.SanitizePreferences();
      IEnumerable<string> encodedIds = this.SanitizeEntityIds();
      IEnumerable<string> strings = this.SanitizePropertiesToReturn();
      int maxResults = this.GetMaxResults();
      IList<KeyValuePair<string, DirectoryEntityIdentifier>> source2 = DirectoryEntityIdentifier.TryParse(encodedIds);
      Dictionary<DirectoryEntityIdentifier, DirectoryGetRelatedEntitiesRequest.Result> decodedIdToResult = source2.Where<KeyValuePair<string, DirectoryEntityIdentifier>>((Func<KeyValuePair<string, DirectoryEntityIdentifier>, bool>) (kvp => kvp.Value != null)).ToDictionary<KeyValuePair<string, DirectoryEntityIdentifier>, DirectoryEntityIdentifier, DirectoryGetRelatedEntitiesRequest.Result>((Func<KeyValuePair<string, DirectoryEntityIdentifier>, DirectoryEntityIdentifier>) (kvp => kvp.Value), (Func<KeyValuePair<string, DirectoryEntityIdentifier>, DirectoryGetRelatedEntitiesRequest.Result>) (kvp => new DirectoryGetRelatedEntitiesRequest.Result()));
      bool partialSuccessPreferred = ((IEnumerable<string>) source1).Contains<string>("PartialSuccessOverFailure");
      foreach (IDirectory directory in directories)
      {
        List<DirectoryEntityIdentifier> list = decodedIdToResult.Where<KeyValuePair<DirectoryEntityIdentifier, DirectoryGetRelatedEntitiesRequest.Result>>((Func<KeyValuePair<DirectoryEntityIdentifier, DirectoryGetRelatedEntitiesRequest.Result>, bool>) (kvp => kvp.Value.Exception == null | partialSuccessPreferred)).Select<KeyValuePair<DirectoryEntityIdentifier, DirectoryGetRelatedEntitiesRequest.Result>, DirectoryEntityIdentifier>((Func<KeyValuePair<DirectoryEntityIdentifier, DirectoryGetRelatedEntitiesRequest.Result>, DirectoryEntityIdentifier>) (kvp => kvp.Key)).ToList<DirectoryEntityIdentifier>();
        IVssRequestContext context1 = context;
        DirectoryInternalGetRelatedEntitiesRequest request = new DirectoryInternalGetRelatedEntitiesRequest();
        request.Directories = (IEnumerable<string>) strArray;
        request.EntityIds = (IEnumerable<DirectoryEntityIdentifier>) list;
        request.Relation = this.Relation;
        request.Depth = this.Depth;
        request.MaxResults = maxResults;
        request.PropertiesToReturn = strings;
        request.PagingToken = this.PagingToken;
        DirectoryInternalGetRelatedEntitiesResponse relatedEntities = directory.GetRelatedEntities(context1, request);
        if (relatedEntities.Results != null)
        {
          foreach (KeyValuePair<DirectoryEntityIdentifier, DirectoryInternalGetRelatedEntitiesResult> result1 in (IEnumerable<KeyValuePair<DirectoryEntityIdentifier, DirectoryInternalGetRelatedEntitiesResult>>) relatedEntities.Results)
          {
            DirectoryEntityIdentifier key = result1.Key;
            DirectoryInternalGetRelatedEntitiesResult relatedEntitiesResult = result1.Value;
            if (relatedEntitiesResult != null)
            {
              if (relatedEntitiesResult.Entities != null)
              {
                DirectoryGetRelatedEntitiesRequest.Result result2 = decodedIdToResult[key];
                result2.PagingToken = relatedEntitiesResult.PagingToken;
                if (result2.Entities == null)
                {
                  result2.Entities = new Dictionary<string, IDirectoryEntity>();
                  foreach (IDirectoryEntity entity in relatedEntitiesResult.Entities)
                    result2.Entities[entity.EntityId] = entity;
                }
                else
                {
                  foreach (IDirectoryEntity entity in relatedEntitiesResult.Entities)
                  {
                    IDirectoryEntity x = !result2.Entities.TryGetValue(entity.EntityId, out x) ? entity : DirectoryEntityMerger.MergeEntities(x, entity);
                    result2.Entities[x.EntityId] = x;
                  }
                }
              }
              else if (relatedEntitiesResult.Exception != null)
                decodedIdToResult[key].Exception = relatedEntitiesResult.Exception;
            }
          }
        }
      }
      return (DirectoryResponse) new DirectoryGetRelatedEntitiesResponse()
      {
        Results = (IDictionary<string, DirectoryGetRelatedEntitiesResult>) source2.ToDictionary<KeyValuePair<string, DirectoryEntityIdentifier>, string, DirectoryGetRelatedEntitiesResult>((Func<KeyValuePair<string, DirectoryEntityIdentifier>, string>) (kvp => kvp.Key), (Func<KeyValuePair<string, DirectoryEntityIdentifier>, DirectoryGetRelatedEntitiesResult>) (kvp =>
        {
          DirectoryGetRelatedEntitiesRequest.Result result;
          if (kvp.Value != null && decodedIdToResult.TryGetValue(kvp.Value, out result))
          {
            IList<IDirectoryEntity> directoryEntityList = (IList<IDirectoryEntity>) Array.Empty<IDirectoryEntity>();
            Exception exception = (Exception) null;
            string str = (string) null;
            if (result != null)
            {
              if (result.Entities != null)
                directoryEntityList = (IList<IDirectoryEntity>) result.Entities.Values.ToList<IDirectoryEntity>();
              else if (result.Exception != null)
                exception = result.Exception;
              str = result.PagingToken;
            }
            return new DirectoryGetRelatedEntitiesResult()
            {
              Entities = (IEnumerable<IDirectoryEntity>) directoryEntityList,
              Exception = exception,
              PagingToken = str
            };
          }
          return new DirectoryGetRelatedEntitiesResult()
          {
            Exception = (Exception) new DirectoryRelatedEntitiesUnavailableException()
          };
        }))
      };
    }

    private int GetMaxResults()
    {
      if (!this.MaxResults.HasValue)
        return 200;
      int num = this.MaxResults.Value;
      return 1 >= num || num > 200 ? 200 : num;
    }

    private IEnumerable<string> SanitizeEntityIds() => this.EntityIds == null ? (IEnumerable<string>) Array.Empty<string>() : (IEnumerable<string>) new HashSet<string>(this.EntityIds);

    private IEnumerable<string> SanitizePropertiesToReturn() => this.PropertiesToReturn == null ? (IEnumerable<string>) Array.Empty<string>() : (IEnumerable<string>) new HashSet<string>(this.PropertiesToReturn);

    private class Result
    {
      internal Dictionary<string, IDirectoryEntity> Entities { get; set; }

      internal Exception Exception { get; set; }

      internal virtual string PagingToken { get; set; }

      internal bool HasMoreResults => this.PagingToken != null;
    }
  }
}
