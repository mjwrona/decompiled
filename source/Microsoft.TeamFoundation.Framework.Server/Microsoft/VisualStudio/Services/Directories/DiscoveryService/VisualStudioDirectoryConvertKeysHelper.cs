// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Directories.DiscoveryService.VisualStudioDirectoryConvertKeysHelper
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Directories.DiscoveryService
{
  internal static class VisualStudioDirectoryConvertKeysHelper
  {
    internal static DirectoryInternalConvertKeysResponse ConvertKeys(
      IVssRequestContext context,
      DirectoryInternalConvertKeysRequest request)
    {
      Dictionary<string, DirectoryInternalConvertKeyResult> dictionary = request.Keys.ToDictionary<string, string, DirectoryInternalConvertKeyResult>((Func<string, string>) (key => key), (Func<string, DirectoryInternalConvertKeyResult>) (key => (DirectoryInternalConvertKeyResult) null));
      if (VisualStudioDirectoryRequestFilter.AllowRequest(context, (DirectoryInternalRequest) request))
      {
        switch (request.ConvertFrom)
        {
          case "DirectoryEntityIdentifier":
            if (request.ConvertTo == "VisualStudioIdentifier")
            {
              VisualStudioDirectoryConvertKeysHelper.ConvertDirToVsid(context, (IDictionary<string, DirectoryInternalConvertKeyResult>) dictionary);
              break;
            }
            break;
          case "VisualStudioIdentifier":
            switch (request.ConvertTo)
            {
              case "DirectoryEntityIdentifier":
                VisualStudioDirectoryConvertKeysHelper.ConvertVsidToDir(context, (IDictionary<string, DirectoryInternalConvertKeyResult>) dictionary);
                break;
              case "VisualStudioIdentifier":
                VisualStudioDirectoryConvertKeysHelper.ConvertVsidToVsid(context, (IDictionary<string, DirectoryInternalConvertKeyResult>) dictionary);
                break;
              case "AzureActiveDirectoryObjectIdentifier":
                VisualStudioDirectoryConvertKeysHelper.ConvertVsidToAad(context, (IDictionary<string, DirectoryInternalConvertKeyResult>) dictionary);
                break;
              case "GitHubIdentifier":
                VisualStudioDirectoryConvertKeysHelper.ConvertVsidToGitHubId(context, (IDictionary<string, DirectoryInternalConvertKeyResult>) dictionary);
                break;
              case "ActiveDirectoryObjectIdentifier":
              case "WindowsMachineDirectoryObjectIdentifier":
                VisualStudioDirectoryConvertKeysHelper.ConvertVsidToWindowsIdentifier(context, (IDictionary<string, DirectoryInternalConvertKeyResult>) dictionary);
                break;
            }
            break;
          case "SubjectDescriptor":
            if (request.ConvertTo == "DirectoryEntityIdentifier")
            {
              VisualStudioDirectoryConvertKeysHelper.ConvertSubjectDescriptorToDir(context, (IDictionary<string, DirectoryInternalConvertKeyResult>) dictionary);
              break;
            }
            break;
        }
      }
      return new DirectoryInternalConvertKeysResponse()
      {
        Results = (IDictionary<string, DirectoryInternalConvertKeyResult>) dictionary
      };
    }

    private static void ConvertDirToVsid(
      IVssRequestContext context,
      IDictionary<string, DirectoryInternalConvertKeyResult> results)
    {
      List<KeyValuePair<string, DirectoryEntityIdentifier>> list1 = DirectoryEntityIdentifier.TryParse((IEnumerable<string>) results.Keys).Where<KeyValuePair<string, DirectoryEntityIdentifier>>((Func<KeyValuePair<string, DirectoryEntityIdentifier>, bool>) (kvp => kvp.Value != null)).ToList<KeyValuePair<string, DirectoryEntityIdentifier>>();
      List<DirectoryEntityIdentifier> list2 = list1.Select<KeyValuePair<string, DirectoryEntityIdentifier>, DirectoryEntityIdentifier>((Func<KeyValuePair<string, DirectoryEntityIdentifier>, DirectoryEntityIdentifier>) (kvp => kvp.Value)).ToList<DirectoryEntityIdentifier>();
      IDictionary<DirectoryEntityIdentifier, Guid> dictionary = VisualStudioDirectoryVsidResolver.Instance.ResolveVsids(context, (IEnumerable<DirectoryEntityIdentifier>) list2);
      foreach (KeyValuePair<string, DirectoryEntityIdentifier> keyValuePair in list1)
      {
        Guid guid;
        if (dictionary.TryGetValue(keyValuePair.Value, out guid) && guid != Guid.Empty)
          results[keyValuePair.Key] = new DirectoryInternalConvertKeyResult()
          {
            Key = guid.ToString()
          };
        else
          results[keyValuePair.Key] = new DirectoryInternalConvertKeyResult()
          {
            Exception = (Exception) new DirectoryKeyNotFoundException("No VSID for key: " + keyValuePair.Key)
          };
      }
    }

    private static void ConvertVsidToDir(
      IVssRequestContext context,
      IDictionary<string, DirectoryInternalConvertKeyResult> results)
    {
      foreach (KeyValuePair<string, Microsoft.VisualStudio.Services.Identity.Identity> identitiesForKey in (IEnumerable<KeyValuePair<string, Microsoft.VisualStudio.Services.Identity.Identity>>) VisualStudioDirectoryConvertKeysHelper.ReadIdentitiesForKeys(context, (IEnumerable<string>) results.Keys))
        results[identitiesForKey.Key] = new DirectoryInternalConvertKeyResult()
        {
          Key = VisualStudioDirectoryEntityConverter.ConvertIdentity(context, identitiesForKey.Value).EntityId
        };
    }

    private static void ConvertVsidToVsid(
      IVssRequestContext context,
      IDictionary<string, DirectoryInternalConvertKeyResult> results)
    {
      foreach (KeyValuePair<string, Microsoft.VisualStudio.Services.Identity.Identity> identitiesForKey in (IEnumerable<KeyValuePair<string, Microsoft.VisualStudio.Services.Identity.Identity>>) VisualStudioDirectoryConvertKeysHelper.ReadIdentitiesForKeys(context, (IEnumerable<string>) results.Keys))
        results[identitiesForKey.Key] = new DirectoryInternalConvertKeyResult()
        {
          Key = identitiesForKey.Value.Id.ToString()
        };
    }

    private static void ConvertVsidToGitHubId(
      IVssRequestContext context,
      IDictionary<string, DirectoryInternalConvertKeyResult> results)
    {
      foreach (KeyValuePair<string, Microsoft.VisualStudio.Services.Identity.Identity> identitiesForKey in (IEnumerable<KeyValuePair<string, Microsoft.VisualStudio.Services.Identity.Identity>>) VisualStudioDirectoryConvertKeysHelper.ReadIdentitiesForKeys(context, (IEnumerable<string>) results.Keys))
      {
        string key = identitiesForKey.Key;
        Microsoft.VisualStudio.Services.Identity.Identity identity = identitiesForKey.Value;
        if (identity != null)
        {
          SocialDescriptor socialDescriptor = identity.SocialDescriptor;
          if (VssStringComparer.SocialType.Compare("ghb", identity.SocialDescriptor.SocialType) == 0)
          {
            results[key] = new DirectoryInternalConvertKeyResult()
            {
              Key = identity.SocialDescriptor.Identifier
            };
            continue;
          }
        }
        results[key] = new DirectoryInternalConvertKeyResult()
        {
          Exception = (Exception) new DirectoryKeyNotFoundException("No GitHub identifier for VSID: " + key)
        };
      }
    }

    private static void ConvertVsidToAad(
      IVssRequestContext context,
      IDictionary<string, DirectoryInternalConvertKeyResult> results)
    {
      foreach (KeyValuePair<string, Microsoft.VisualStudio.Services.Identity.Identity> identitiesForKey in (IEnumerable<KeyValuePair<string, Microsoft.VisualStudio.Services.Identity.Identity>>) VisualStudioDirectoryConvertKeysHelper.ReadIdentitiesForKeys(context, (IEnumerable<string>) results.Keys))
      {
        Guid objectId = AadIdentityHelper.ExtractObjectId((IReadOnlyVssIdentity) identitiesForKey.Value);
        if (objectId != Guid.Empty)
          results[identitiesForKey.Key] = new DirectoryInternalConvertKeyResult()
          {
            Key = objectId.ToString()
          };
        else
          results[identitiesForKey.Key] = new DirectoryInternalConvertKeyResult()
          {
            Exception = (Exception) new DirectoryKeyNotFoundException("No OID for key: " + identitiesForKey.Key)
          };
      }
    }

    private static void ConvertVsidToWindowsIdentifier(
      IVssRequestContext context,
      IDictionary<string, DirectoryInternalConvertKeyResult> results)
    {
      foreach (KeyValuePair<string, Microsoft.VisualStudio.Services.Identity.Identity> identitiesForKey in (IEnumerable<KeyValuePair<string, Microsoft.VisualStudio.Services.Identity.Identity>>) VisualStudioDirectoryConvertKeysHelper.ReadIdentitiesForKeys(context, (IEnumerable<string>) results.Keys))
      {
        string identifier = identitiesForKey.Value?.Descriptor.Identifier;
        if (!string.IsNullOrWhiteSpace(identifier))
          results[identitiesForKey.Key] = new DirectoryInternalConvertKeyResult()
          {
            Key = identifier
          };
        else
          results[identitiesForKey.Key] = new DirectoryInternalConvertKeyResult()
          {
            Exception = (Exception) new DirectoryKeyNotFoundException("No SID for key: " + identitiesForKey.Key)
          };
      }
    }

    private static void ConvertSubjectDescriptorToDir(
      IVssRequestContext context,
      IDictionary<string, DirectoryInternalConvertKeyResult> results)
    {
      ICollection<string> keys = results.Keys;
      if (keys.IsNullOrEmpty<string>())
        return;
      List<SubjectDescriptor> list = keys.Select<string, SubjectDescriptor>((Func<string, SubjectDescriptor>) (x => SubjectDescriptor.FromString(x))).ToList<SubjectDescriptor>();
      Dictionary<SubjectDescriptor, Microsoft.VisualStudio.Services.Identity.Identity> identitiesDict = context.GetService<IdentityService>().ReadIdentities(context, (IList<SubjectDescriptor>) list, QueryMembership.None, (IEnumerable<string>) null).Where<Microsoft.VisualStudio.Services.Identity.Identity>((Func<Microsoft.VisualStudio.Services.Identity.Identity, bool>) (x => x != null)).ToDedupedDictionary<Microsoft.VisualStudio.Services.Identity.Identity, SubjectDescriptor, Microsoft.VisualStudio.Services.Identity.Identity>((Func<Microsoft.VisualStudio.Services.Identity.Identity, SubjectDescriptor>) (x => x.SubjectDescriptor), (Func<Microsoft.VisualStudio.Services.Identity.Identity, Microsoft.VisualStudio.Services.Identity.Identity>) (x => x));
      Microsoft.VisualStudio.Services.Identity.Identity identity;
      foreach (KeyValuePair<SubjectDescriptor, Microsoft.VisualStudio.Services.Identity.Identity> deduped in list.ToDedupedDictionary<SubjectDescriptor, SubjectDescriptor, Microsoft.VisualStudio.Services.Identity.Identity>((Func<SubjectDescriptor, SubjectDescriptor>) (x => x), (Func<SubjectDescriptor, Microsoft.VisualStudio.Services.Identity.Identity>) (x => !identitiesDict.TryGetValue(x, out identity) ? (Microsoft.VisualStudio.Services.Identity.Identity) null : identity)))
      {
        if (deduped.Value == null)
          results[(string) deduped.Key] = new DirectoryInternalConvertKeyResult()
          {
            Exception = (Exception) new DirectoryKeyNotFoundException("No identity found for key: " + (string) deduped.Key)
          };
        else
          results[(string) deduped.Key] = new DirectoryInternalConvertKeyResult()
          {
            Key = VisualStudioDirectoryEntityConverter.ConvertIdentity(context, deduped.Value).EntityId
          };
      }
    }

    private static IList<KeyValuePair<string, Microsoft.VisualStudio.Services.Identity.Identity>> ReadIdentitiesForKeys(
      IVssRequestContext context,
      IEnumerable<string> keys)
    {
      List<KeyValuePair<string, Guid>> keyToVsid = keys.Select<string, KeyValuePair<string, Guid>>((Func<string, KeyValuePair<string, Guid>>) (key =>
      {
        Guid result;
        Guid.TryParse(key, out result);
        return new KeyValuePair<string, Guid>(key, result);
      })).Where<KeyValuePair<string, Guid>>((Func<KeyValuePair<string, Guid>, bool>) (kvp => kvp.Value != Guid.Empty)).ToList<KeyValuePair<string, Guid>>();
      List<Guid> list = keyToVsid.Select<KeyValuePair<string, Guid>, Guid>((Func<KeyValuePair<string, Guid>, Guid>) (kvp => kvp.Value)).ToList<Guid>();
      IList<Microsoft.VisualStudio.Services.Identity.Identity> identities = context.GetService<IdentityService>().ReadIdentities(context, (IList<Guid>) list, QueryMembership.None, (IEnumerable<string>) null);
      return (IList<KeyValuePair<string, Microsoft.VisualStudio.Services.Identity.Identity>>) Enumerable.Range(0, keyToVsid.Count).Select<int, KeyValuePair<string, Microsoft.VisualStudio.Services.Identity.Identity>>((Func<int, KeyValuePair<string, Microsoft.VisualStudio.Services.Identity.Identity>>) (i => new KeyValuePair<string, Microsoft.VisualStudio.Services.Identity.Identity>(keyToVsid[i].Key, identities[i]))).Where<KeyValuePair<string, Microsoft.VisualStudio.Services.Identity.Identity>>((Func<KeyValuePair<string, Microsoft.VisualStudio.Services.Identity.Identity>, bool>) (kvp => kvp.Value != null)).ToList<KeyValuePair<string, Microsoft.VisualStudio.Services.Identity.Identity>>();
    }
  }
}
