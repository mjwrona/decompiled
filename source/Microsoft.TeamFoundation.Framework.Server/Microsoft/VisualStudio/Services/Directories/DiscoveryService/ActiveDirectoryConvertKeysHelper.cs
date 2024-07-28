// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Directories.DiscoveryService.ActiveDirectoryConvertKeysHelper
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Directories.DiscoveryService
{
  internal static class ActiveDirectoryConvertKeysHelper
  {
    internal static DirectoryInternalConvertKeysResponse ConvertKeys(
      IVssRequestContext context,
      DirectoryInternalConvertKeysRequest request)
    {
      if (!ActiveDirectoryRequestFilter.AllowRequest(context, (DirectoryInternalRequest) request))
        return new DirectoryInternalConvertKeysResponse();
      DirectoryInternalConvertKeysResponse convertKeysResponse = new DirectoryInternalConvertKeysResponse()
      {
        Results = (IDictionary<string, DirectoryInternalConvertKeyResult>) request.Keys.ToDictionary<string, string, DirectoryInternalConvertKeyResult>((Func<string, string>) (id => id), (Func<string, DirectoryInternalConvertKeyResult>) (id => (DirectoryInternalConvertKeyResult) null), (IEqualityComparer<string>) VssStringComparer.DirectoryKeyStringComparer)
      };
      switch (request.ConvertFrom)
      {
        case "DirectoryEntityIdentifier":
          if (request.ConvertTo == "ActiveDirectoryObjectIdentifier")
          {
            ActiveDirectoryConvertKeysHelper.PopulateConvertKeyResultsDirToAd(context, convertKeysResponse.Results);
            break;
          }
          break;
        case "ActiveDirectoryObjectIdentifier":
          if (request.ConvertTo == "DirectoryEntityIdentifier")
          {
            ActiveDirectoryConvertKeysHelper.PopulateConvertKeyResultsAdToDir(context, convertKeysResponse.Results);
            break;
          }
          break;
      }
      return convertKeysResponse;
    }

    private static void PopulateConvertKeyResultsDirToAd(
      IVssRequestContext context,
      IDictionary<string, DirectoryInternalConvertKeyResult> results)
    {
      List<KeyValuePair<string, DirectoryEntityIdentifier>> list = DirectoryEntityIdentifier.TryParse((IEnumerable<string>) results.Keys).Where<KeyValuePair<string, DirectoryEntityIdentifier>>((Func<KeyValuePair<string, DirectoryEntityIdentifier>, bool>) (kvp => kvp.Value != null)).ToList<KeyValuePair<string, DirectoryEntityIdentifier>>();
      List<KeyValuePair<string, Guid>> keyToVsid = new List<KeyValuePair<string, Guid>>(list.Count);
      foreach (KeyValuePair<string, DirectoryEntityIdentifier> keyValuePair in list)
      {
        string key = keyValuePair.Key;
        DirectoryEntityIdentifier entityId = keyValuePair.Value;
        if (entityId.Version == 1 && entityId is DirectoryEntityIdentifierV1 directoryEntityIdentifierV1)
        {
          switch (directoryEntityIdentifierV1.Source)
          {
            case "ims":
              Guid vsid;
              if (DirectoryConvertKeysHelper.TryGetVsid(directoryEntityIdentifierV1, out vsid))
              {
                keyToVsid.Add(new KeyValuePair<string, Guid>(key, vsid));
                continue;
              }
              continue;
            case "ad":
              string objectSid;
              if (ActiveDirectoryHelper.TryGetObjectSid(entityId, out objectSid))
              {
                results[key] = new DirectoryInternalConvertKeyResult()
                {
                  Key = objectSid
                };
                continue;
              }
              continue;
            default:
              continue;
          }
        }
      }
      DirectoryConvertKeysHelper.PopulateConvertKeyResultsDirToActiveOrWindowsMachineDirectory(context, (IList<KeyValuePair<string, Guid>>) keyToVsid, results);
    }

    private static void PopulateConvertKeyResultsAdToDir(
      IVssRequestContext context,
      IDictionary<string, DirectoryInternalConvertKeyResult> results)
    {
      foreach (KeyValuePair<string, IDirectoryEntity> directoryEntity in (IEnumerable<KeyValuePair<string, IDirectoryEntity>>) ActiveDirectoryHelper.Instance.GetDirectoryEntities(context, (IList<string>) results.Keys.ToList<string>(), (IEnumerable<string>) null))
      {
        string key = directoryEntity.Key;
        if (directoryEntity.Value != null)
          results[key] = new DirectoryInternalConvertKeyResult()
          {
            Key = directoryEntity.Value.EntityId
          };
        else
          results[key] = new DirectoryInternalConvertKeyResult()
          {
            Exception = (Exception) new DirectoryKeyNotFoundException()
          };
      }
    }
  }
}
