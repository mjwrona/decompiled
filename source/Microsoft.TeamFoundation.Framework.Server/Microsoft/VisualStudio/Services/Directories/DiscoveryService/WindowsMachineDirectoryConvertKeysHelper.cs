// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Directories.DiscoveryService.WindowsMachineDirectoryConvertKeysHelper
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;

namespace Microsoft.VisualStudio.Services.Directories.DiscoveryService
{
  internal static class WindowsMachineDirectoryConvertKeysHelper
  {
    internal static DirectoryInternalConvertKeysResponse ConvertKeys(
      IVssRequestContext context,
      DirectoryInternalConvertKeysRequest request)
    {
      if (!WindowsMachineDirectoryRequestFilter.AllowRequest(context, (DirectoryInternalRequest) request))
        return new DirectoryInternalConvertKeysResponse();
      DirectoryInternalConvertKeysResponse convertKeysResponse = new DirectoryInternalConvertKeysResponse()
      {
        Results = (IDictionary<string, DirectoryInternalConvertKeyResult>) request.Keys.ToDictionary<string, string, DirectoryInternalConvertKeyResult>((Func<string, string>) (id => id), (Func<string, DirectoryInternalConvertKeyResult>) (id => (DirectoryInternalConvertKeyResult) null), (IEqualityComparer<string>) VssStringComparer.DirectoryKeyStringComparer)
      };
      switch (request.ConvertFrom)
      {
        case "DirectoryEntityIdentifier":
          if (request.ConvertTo == "WindowsMachineDirectoryObjectIdentifier")
          {
            WindowsMachineDirectoryConvertKeysHelper.PopulateConvertKeyResultsDirToWindowsMachineDirectory(context, convertKeysResponse.Results);
            break;
          }
          break;
        case "WindowsMachineDirectoryObjectIdentifier":
          if (request.ConvertTo == "DirectoryEntityIdentifier")
          {
            WindowsMachineDirectoryConvertKeysHelper.PopulateConvertKeyResultsWindowsMachineDirectoryToDir(context, convertKeysResponse.Results);
            break;
          }
          break;
      }
      return convertKeysResponse;
    }

    private static void PopulateConvertKeyResultsDirToWindowsMachineDirectory(
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
            case "wmd":
              SecurityIdentifier objectSid;
              if (WindowsMachineDirectoryHelper.TryGetObjectSid(entityId, out objectSid))
              {
                results[key] = new DirectoryInternalConvertKeyResult()
                {
                  Key = objectSid.ToString()
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

    private static void PopulateConvertKeyResultsWindowsMachineDirectoryToDir(
      IVssRequestContext context,
      IDictionary<string, DirectoryInternalConvertKeyResult> results)
    {
      List<SecurityIdentifier> sids = new List<SecurityIdentifier>();
      foreach (string str in results.Keys.ToList<string>())
      {
        SecurityIdentifier sid;
        if (!WindowsMachineDirectoryConvertKeysHelper.TryGetSecurityIdentifier(str, out sid))
          results[str] = new DirectoryInternalConvertKeyResult()
          {
            Exception = (Exception) new DirectoryKeyNotFoundException(FrameworkResources.UnableToResolveObjectSid((object) str))
          };
        else
          sids.Add(sid);
      }
      if (sids.Count == 0)
        return;
      IDictionary<SecurityIdentifier, IDirectoryEntity> directoryEntities = WindowsMachineDirectoryHelper.Instance.GetDirectoryEntities(context, (IEnumerable<SecurityIdentifier>) sids, (IEnumerable<string>) null);
      foreach (SecurityIdentifier key in sids)
      {
        IDirectoryEntity directoryEntity;
        if (directoryEntities.TryGetValue(key, out directoryEntity) && directoryEntity != null)
          results[key.ToString()] = new DirectoryInternalConvertKeyResult()
          {
            Key = directoryEntity.EntityId
          };
        else
          results[key.ToString()] = new DirectoryInternalConvertKeyResult()
          {
            Exception = (Exception) new DirectoryKeyNotFoundException(FrameworkResources.UnableToResolveObjectSid((object) key.ToString()))
          };
      }
    }

    private static bool TryGetSecurityIdentifier(string id, out SecurityIdentifier sid)
    {
      sid = (SecurityIdentifier) null;
      try
      {
        sid = new SecurityIdentifier(id);
      }
      catch (Exception ex)
      {
        return false;
      }
      return true;
    }
  }
}
