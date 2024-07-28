// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Directories.DiscoveryService.DirectoryGetRelatedEntityIdsRequest
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Directories.DiscoveryService
{
  public class DirectoryGetRelatedEntityIdsRequest : DirectoryPagedRequest
  {
    private const int absoluteMinResults = 1;
    private const int absoluteMaxResults = 100;

    public IEnumerable<string> EntityIds { get; set; }

    public string Relation { get; set; }

    public int Depth { get; set; }

    internal override DirectoryResponse Execute(
      IVssRequestContext context,
      IEnumerable<IDirectory> directories)
    {
      string[] strArray = this.SanitizeAndFilterDirectories(context);
      string[] source1 = this.SanitizePreferences();
      IEnumerable<string> encodedIds = this.SanitizeEntityIds();
      int maxResults = this.GetMaxResults();
      IList<KeyValuePair<string, DirectoryEntityIdentifier>> source2 = DirectoryEntityIdentifier.TryParse(encodedIds);
      Dictionary<DirectoryEntityIdentifier, DirectoryGetRelatedEntityIdsRequest.Result> decodedIdToResult = source2.Where<KeyValuePair<string, DirectoryEntityIdentifier>>((Func<KeyValuePair<string, DirectoryEntityIdentifier>, bool>) (kvp => kvp.Value != null)).ToDictionary<KeyValuePair<string, DirectoryEntityIdentifier>, DirectoryEntityIdentifier, DirectoryGetRelatedEntityIdsRequest.Result>((Func<KeyValuePair<string, DirectoryEntityIdentifier>, DirectoryEntityIdentifier>) (kvp => kvp.Value), (Func<KeyValuePair<string, DirectoryEntityIdentifier>, DirectoryGetRelatedEntityIdsRequest.Result>) (kvp => new DirectoryGetRelatedEntityIdsRequest.Result()));
      bool partialSuccessPreferred = ((IEnumerable<string>) source1).Contains<string>("PartialSuccessOverFailure");
      foreach (IDirectory directory in directories)
      {
        List<DirectoryEntityIdentifier> list = decodedIdToResult.Where<KeyValuePair<DirectoryEntityIdentifier, DirectoryGetRelatedEntityIdsRequest.Result>>((Func<KeyValuePair<DirectoryEntityIdentifier, DirectoryGetRelatedEntityIdsRequest.Result>, bool>) (kvp => kvp.Value.Exception == null | partialSuccessPreferred)).Select<KeyValuePair<DirectoryEntityIdentifier, DirectoryGetRelatedEntityIdsRequest.Result>, DirectoryEntityIdentifier>((Func<KeyValuePair<DirectoryEntityIdentifier, DirectoryGetRelatedEntityIdsRequest.Result>, DirectoryEntityIdentifier>) (kvp => kvp.Key)).ToList<DirectoryEntityIdentifier>();
        IVssRequestContext context1 = context;
        DirectoryInternalGetRelatedEntityIdsRequest request = new DirectoryInternalGetRelatedEntityIdsRequest();
        request.Directories = (IEnumerable<string>) strArray;
        request.EntityIds = (IEnumerable<DirectoryEntityIdentifier>) list;
        request.Relation = this.Relation;
        request.Depth = this.Depth;
        request.MaxResults = maxResults;
        DirectoryInternalGetRelatedEntityIdsResponse relatedEntityIds = directory.GetRelatedEntityIds(context1, request);
        if (relatedEntityIds.Results != null)
        {
          foreach (KeyValuePair<DirectoryEntityIdentifier, DirectoryInternalGetRelatedEntityIdsResult> result1 in (IEnumerable<KeyValuePair<DirectoryEntityIdentifier, DirectoryInternalGetRelatedEntityIdsResult>>) relatedEntityIds.Results)
          {
            DirectoryEntityIdentifier key = result1.Key;
            DirectoryInternalGetRelatedEntityIdsResult relatedEntityIdsResult = result1.Value;
            if (relatedEntityIdsResult != null)
            {
              if (relatedEntityIdsResult.EntityIds != null)
              {
                DirectoryGetRelatedEntityIdsRequest.Result result2 = decodedIdToResult[key];
                if (result2.EntityIds == null)
                {
                  result2.EntityIds = new HashSet<string>(relatedEntityIdsResult.EntityIds);
                }
                else
                {
                  foreach (string entityId in relatedEntityIdsResult.EntityIds)
                    result2.EntityIds.Add(entityId);
                }
              }
              else if (relatedEntityIdsResult.Exception != null)
                decodedIdToResult[key].Exception = relatedEntityIdsResult.Exception;
            }
          }
        }
      }
      return (DirectoryResponse) new DirectoryGetRelatedEntityIdsResponse()
      {
        Results = (IDictionary<string, DirectoryGetRelatedEntityIdsResult>) source2.ToDictionary<KeyValuePair<string, DirectoryEntityIdentifier>, string, DirectoryGetRelatedEntityIdsResult>((Func<KeyValuePair<string, DirectoryEntityIdentifier>, string>) (kvp => kvp.Key), (Func<KeyValuePair<string, DirectoryEntityIdentifier>, DirectoryGetRelatedEntityIdsResult>) (kvp =>
        {
          DirectoryGetRelatedEntityIdsRequest.Result result;
          if (kvp.Value != null && decodedIdToResult.TryGetValue(kvp.Value, out result))
            return new DirectoryGetRelatedEntityIdsResult()
            {
              EntityIds = (IEnumerable<string>) result.EntityIds,
              Exception = result.Exception
            };
          return new DirectoryGetRelatedEntityIdsResult()
          {
            Exception = (Exception) new DirectoryRelatedEntityIdsUnavailableException()
          };
        }))
      };
    }

    private int GetMaxResults()
    {
      if (!this.MaxResults.HasValue)
        return 100;
      int num = this.MaxResults.Value;
      return 1 >= num || num > 100 ? 100 : num;
    }

    private IEnumerable<string> SanitizeEntityIds() => this.EntityIds == null ? (IEnumerable<string>) Array.Empty<string>() : (IEnumerable<string>) new HashSet<string>(this.EntityIds);

    private class Result
    {
      internal HashSet<string> EntityIds { get; set; }

      internal Exception Exception { get; set; }
    }
  }
}
