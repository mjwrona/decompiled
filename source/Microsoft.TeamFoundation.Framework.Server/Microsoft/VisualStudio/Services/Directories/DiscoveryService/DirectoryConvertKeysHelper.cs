// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Directories.DiscoveryService.DirectoryConvertKeysHelper
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;

namespace Microsoft.VisualStudio.Services.Directories.DiscoveryService
{
  internal static class DirectoryConvertKeysHelper
  {
    internal static bool TryGetVsid(
      DirectoryEntityIdentifierV1 directoryEntityIdentifierV1,
      out Guid vsid)
    {
      vsid = Guid.Empty;
      return Guid.TryParseExact(directoryEntityIdentifierV1.Id, "N", out vsid);
    }

    internal static void PopulateConvertKeyResultsDirToActiveOrWindowsMachineDirectory(
      IVssRequestContext context,
      IList<KeyValuePair<string, Guid>> keyToVsid,
      IDictionary<string, DirectoryInternalConvertKeyResult> results)
    {
      if (keyToVsid.Count == 0)
        return;
      List<Guid> list = keyToVsid.Select<KeyValuePair<string, Guid>, Guid>((Func<KeyValuePair<string, Guid>, Guid>) (kvp => kvp.Value)).ToList<Guid>();
      IDictionary<Guid, Microsoft.VisualStudio.Services.Identity.Identity> dictionary = DirectoryConvertKeysHelper.ReadIdentities(context, (IList<Guid>) list);
      foreach (KeyValuePair<string, Guid> keyValuePair in (IEnumerable<KeyValuePair<string, Guid>>) keyToVsid)
      {
        Guid key1 = keyValuePair.Value;
        string key2 = keyValuePair.Key;
        Microsoft.VisualStudio.Services.Identity.Identity resultIdentity = (Microsoft.VisualStudio.Services.Identity.Identity) null;
        if (!dictionary.TryGetValue(key1, out resultIdentity))
          results[key2] = new DirectoryInternalConvertKeyResult()
          {
            Exception = (Exception) new DirectoryKeyNotFoundException()
          };
        else
          results[keyValuePair.Key] = DirectoryConvertKeysHelper.CreateDirectoryInternalConvertKeyResult(resultIdentity);
      }
    }

    private static IDictionary<Guid, Microsoft.VisualStudio.Services.Identity.Identity> ReadIdentities(
      IVssRequestContext context,
      IList<Guid> vsids)
    {
      Dictionary<Guid, Microsoft.VisualStudio.Services.Identity.Identity> dictionary = new Dictionary<Guid, Microsoft.VisualStudio.Services.Identity.Identity>();
      if (vsids.Count == 0)
        return (IDictionary<Guid, Microsoft.VisualStudio.Services.Identity.Identity>) dictionary;
      IList<Microsoft.VisualStudio.Services.Identity.Identity> identityList = context.GetService<IdentityService>().ReadIdentities(context, vsids, QueryMembership.None, (IEnumerable<string>) null);
      int index = 0;
      foreach (Guid vsid in (IEnumerable<Guid>) vsids)
      {
        Microsoft.VisualStudio.Services.Identity.Identity identity = identityList[index];
        if (identity != null)
          dictionary[vsid] = identity;
        ++index;
      }
      return (IDictionary<Guid, Microsoft.VisualStudio.Services.Identity.Identity>) dictionary;
    }

    private static DirectoryInternalConvertKeyResult CreateDirectoryInternalConvertKeyResult(
      Microsoft.VisualStudio.Services.Identity.Identity resultIdentity)
    {
      try
      {
        SecurityIdentifier securityIdentifier = TFCommonUtil.CheckSid(resultIdentity.Descriptor.Identifier, "Identifier");
        return new DirectoryInternalConvertKeyResult()
        {
          Key = securityIdentifier.ToString()
        };
      }
      catch (Exception ex)
      {
        return new DirectoryInternalConvertKeyResult()
        {
          Exception = (Exception) new DirectoryConvertKeyFailedException(ex.Message, ex)
        };
      }
    }
  }
}
