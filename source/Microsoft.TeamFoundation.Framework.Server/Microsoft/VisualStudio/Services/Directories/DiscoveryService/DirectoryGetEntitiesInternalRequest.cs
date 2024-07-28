// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Directories.DiscoveryService.DirectoryGetEntitiesInternalRequest
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Directories.DiscoveryService
{
  public class DirectoryGetEntitiesInternalRequest : DirectoryRequest
  {
    private static readonly DirectoryGetEntityResult s_defaultResult = new DirectoryGetEntityResult()
    {
      Exception = (Exception) new DirectoryEntityUnavailableException()
    };

    public IEnumerable<string> EntityIds { get; set; }

    public IEnumerable<string> PropertiesToReturn { get; set; }

    internal override DirectoryResponse Execute(
      IVssRequestContext context,
      IEnumerable<IDirectory> directories)
    {
      context.TraceConditionally(15003002, TraceLevel.Verbose, "VisualStudio.Services.DirectoryDiscovery", "Service", (Func<string>) (() => "DirectoryGetEntitiesRequest Execute" + string.Join(";", new string[12]
      {
        context.ActivityId.ToString(),
        context.UniqueIdentifier.ToString(),
        context.ServiceName,
        context.ServiceHost.ToString(),
        context.GetAuthenticatedId().ToString(),
        context.GetUserId().ToString(),
        context.IsSystemContext.ToString(),
        context.IsServicingContext.ToString(),
        context.IsUserContext.ToString(),
        string.Join("-", this.EntityIds),
        string.Join("-", this.Directories),
        string.Join<IDirectory>("-", directories)
      })));
      string[] strArray = this.SanitizeDirectories();
      IEnumerable<string> entityIds = this.GetEntityIds();
      IEnumerable<string> propertiesToReturn = this.GetPropertiesToReturn();
      IList<KeyValuePair<string, DirectoryEntityIdentifier>> source = DirectoryEntityIdentifier.TryParse(entityIds);
      Dictionary<DirectoryEntityIdentifier, DirectoryGetEntityResult> decodedIdToResult = source.Where<KeyValuePair<string, DirectoryEntityIdentifier>>((Func<KeyValuePair<string, DirectoryEntityIdentifier>, bool>) (kvp => kvp.Value != null)).ToDictionary<KeyValuePair<string, DirectoryEntityIdentifier>, DirectoryEntityIdentifier, DirectoryGetEntityResult>((Func<KeyValuePair<string, DirectoryEntityIdentifier>, DirectoryEntityIdentifier>) (kvp => kvp.Value), (Func<KeyValuePair<string, DirectoryEntityIdentifier>, DirectoryGetEntityResult>) (kvp => DirectoryGetEntitiesInternalRequest.s_defaultResult));
      List<DirectoryEntityIdentifier> list = decodedIdToResult.Select<KeyValuePair<DirectoryEntityIdentifier, DirectoryGetEntityResult>, DirectoryEntityIdentifier>((Func<KeyValuePair<DirectoryEntityIdentifier, DirectoryGetEntityResult>, DirectoryEntityIdentifier>) (kvp => kvp.Key)).ToList<DirectoryEntityIdentifier>();
      if (list.Count > 0)
      {
        foreach (IDirectory directory in directories)
        {
          IVssRequestContext context1 = context;
          DirectoryInternalGetEntitiesRequest request = new DirectoryInternalGetEntitiesRequest();
          request.Directories = (IEnumerable<string>) strArray;
          request.EntityIds = (IEnumerable<DirectoryEntityIdentifier>) list;
          request.PropertiesToReturn = propertiesToReturn;
          DirectoryInternalGetEntitiesResponse entities = directory.GetEntities(context1, request);
          if (entities.Results != null)
          {
            foreach (DirectoryEntityIdentifier key in list)
            {
              DirectoryInternalGetEntityResult result = entities.Results[key];
              if (result != null)
              {
                DirectoryGetEntityResult directoryGetEntityResult = decodedIdToResult[key];
                decodedIdToResult[key] = new DirectoryGetEntityResult()
                {
                  Entity = DirectoryEntityMerger.MergeEntities(directoryGetEntityResult.Entity, result.Entity),
                  Exception = result.Exception
                };
              }
            }
          }
        }
      }
      return (DirectoryResponse) new DirectoryGetEntitiesResponse()
      {
        Results = (IDictionary<string, DirectoryGetEntityResult>) source.ToDictionary<KeyValuePair<string, DirectoryEntityIdentifier>, string, DirectoryGetEntityResult>((Func<KeyValuePair<string, DirectoryEntityIdentifier>, string>) (kvp => kvp.Key), (Func<KeyValuePair<string, DirectoryEntityIdentifier>, DirectoryGetEntityResult>) (kvp =>
        {
          DirectoryGetEntityResult directoryGetEntityResult = (DirectoryGetEntityResult) null;
          return kvp.Value == null || !decodedIdToResult.TryGetValue(kvp.Value, out directoryGetEntityResult) ? (DirectoryGetEntityResult) null : directoryGetEntityResult;
        }))
      };
    }

    private IEnumerable<string> GetEntityIds() => this.EntityIds == null ? (IEnumerable<string>) Array.Empty<string>() : (IEnumerable<string>) new HashSet<string>(this.EntityIds);

    private IEnumerable<string> GetPropertiesToReturn() => this.PropertiesToReturn == null ? (IEnumerable<string>) Array.Empty<string>() : (IEnumerable<string>) new HashSet<string>(this.PropertiesToReturn);
  }
}
