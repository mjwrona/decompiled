// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Directories.DiscoveryService.AzureActiveDirectoryConvertKeysHelper
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Aad;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Directories.DiscoveryService
{
  internal static class AzureActiveDirectoryConvertKeysHelper
  {
    internal static DirectoryInternalConvertKeysResponse ConvertKeys(
      IVssRequestContext context,
      DirectoryInternalConvertKeysRequest request)
    {
      Dictionary<string, DirectoryInternalConvertKeyResult> dictionary = request.Keys.ToDictionary<string, string, DirectoryInternalConvertKeyResult>((Func<string, string>) (key => key), (Func<string, DirectoryInternalConvertKeyResult>) (key => (DirectoryInternalConvertKeyResult) null));
      if (AzureActiveDirectoryRequestFilter.AllowRequest(context, (DirectoryInternalRequest) request))
      {
        switch (request.ConvertFrom)
        {
          case "DirectoryEntityIdentifier":
            if (request.ConvertTo == "AzureActiveDirectoryObjectIdentifier")
            {
              AzureActiveDirectoryConvertKeysHelper.ConvertDirToAad(context, (IDictionary<string, DirectoryInternalConvertKeyResult>) dictionary);
              break;
            }
            break;
          case "AzureActiveDirectoryObjectIdentifier":
            switch (request.ConvertTo)
            {
              case "DirectoryEntityIdentifier":
                AzureActiveDirectoryConvertKeysHelper.ConvertAadToDir(context, (IDictionary<string, DirectoryInternalConvertKeyResult>) dictionary);
                break;
              case "VisualStudioIdentifier":
                AzureActiveDirectoryConvertKeysHelper.ConvertAadToVsid(context, (IDictionary<string, DirectoryInternalConvertKeyResult>) dictionary);
                break;
              case "AzureActiveDirectoryObjectIdentifier":
                AzureActiveDirectoryConvertKeysHelper.ConvertAadToAad(context, (IDictionary<string, DirectoryInternalConvertKeyResult>) dictionary);
                break;
              case "SignInAddress":
                AzureActiveDirectoryConvertKeysHelper.ConvertAadToSignInAddress(context, (IDictionary<string, DirectoryInternalConvertKeyResult>) dictionary);
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

    private static void ConvertDirToAad(
      IVssRequestContext context,
      IDictionary<string, DirectoryInternalConvertKeyResult> results)
    {
      List<KeyValuePair<string, DirectoryEntityIdentifier>> list1 = DirectoryEntityIdentifier.TryParse((IEnumerable<string>) results.Keys).Where<KeyValuePair<string, DirectoryEntityIdentifier>>((Func<KeyValuePair<string, DirectoryEntityIdentifier>, bool>) (kvp => kvp.Value != null)).ToList<KeyValuePair<string, DirectoryEntityIdentifier>>();
      List<KeyValuePair<string, Guid>> source = new List<KeyValuePair<string, Guid>>(list1.Count);
      foreach (KeyValuePair<string, DirectoryEntityIdentifier> keyValuePair in list1)
      {
        string key = keyValuePair.Key;
        DirectoryEntityIdentifier entityIdentifier = keyValuePair.Value;
        if (entityIdentifier.Version == 1)
        {
          DirectoryEntityIdentifierV1 entityIdentifierV1 = (DirectoryEntityIdentifierV1) entityIdentifier;
          switch (entityIdentifierV1.Source)
          {
            case "ims":
              Guid result1;
              if (Guid.TryParseExact(entityIdentifierV1.Id, "N", out result1))
              {
                source.Add(new KeyValuePair<string, Guid>(key, result1));
                continue;
              }
              continue;
            case "aad":
              Guid result2;
              if (Guid.TryParseExact(entityIdentifierV1.Id, "N", out result2))
              {
                results[key] = new DirectoryInternalConvertKeyResult()
                {
                  Key = result2.ToString()
                };
                continue;
              }
              continue;
            default:
              continue;
          }
        }
      }
      if (source.Count <= 0)
        return;
      List<Guid> list2 = source.Select<KeyValuePair<string, Guid>, Guid>((Func<KeyValuePair<string, Guid>, Guid>) (kvp => kvp.Value)).ToList<Guid>();
      IList<Microsoft.VisualStudio.Services.Identity.Identity> identityList = context.GetService<IdentityService>().ReadIdentities(context, (IList<Guid>) list2, QueryMembership.None, (IEnumerable<string>) null);
      for (int index = 0; index < source.Count; ++index)
      {
        Microsoft.VisualStudio.Services.Identity.Identity aadIdentity = identityList[index];
        if (aadIdentity != null)
        {
          Guid objectId = AadIdentityHelper.ExtractObjectId((IReadOnlyVssIdentity) aadIdentity);
          if (objectId != Guid.Empty)
          {
            string key = source[index].Key;
            string str = objectId.ToString();
            results[key] = new DirectoryInternalConvertKeyResult()
            {
              Key = str
            };
          }
        }
      }
    }

    private static void ConvertAadToDir(
      IVssRequestContext context,
      IDictionary<string, DirectoryInternalConvertKeyResult> results)
    {
      IList<KeyValuePair<string, Guid>> oids = AzureActiveDirectoryConvertKeysHelper.ConvertKeysToOids((IEnumerable<string>) results.Keys);
      List<Guid> list = oids.Select<KeyValuePair<string, Guid>, Guid>((Func<KeyValuePair<string, Guid>, Guid>) (kvp => kvp.Value)).ToList<Guid>();
      AadService service = context.GetService<AadService>();
      IDictionary<Guid, AadUser> users = service.GetUsersWithIds<Guid>(context, new GetUsersWithIdsRequest<Guid>()
      {
        Identifiers = (IEnumerable<Guid>) list
      }).Users;
      IDictionary<Guid, AadGroup> groups = service.GetGroupsWithIds<Guid>(context, new GetGroupsWithIdsRequest<Guid>()
      {
        Identifiers = (IEnumerable<Guid>) list
      }).Groups;
      foreach (KeyValuePair<string, Guid> keyValuePair in (IEnumerable<KeyValuePair<string, Guid>>) oids)
      {
        string key1 = keyValuePair.Key;
        Guid key2 = keyValuePair.Value;
        AadUser aadUser;
        if (users.TryGetValue(key2, out aadUser))
        {
          string str = AzureActiveDirectoryEntityIdentifierHelper.CreateUserId(aadUser.ObjectId).Encode();
          results[key1] = new DirectoryInternalConvertKeyResult()
          {
            Key = str
          };
        }
        else
        {
          AadGroup aadGroup;
          if (groups.TryGetValue(key2, out aadGroup))
          {
            string str = AzureActiveDirectoryEntityIdentifierHelper.CreateGroupId(aadGroup.ObjectId).Encode();
            results[key1] = new DirectoryInternalConvertKeyResult()
            {
              Key = str
            };
          }
        }
      }
    }

    private static void ConvertAadToVsid(
      IVssRequestContext context,
      IDictionary<string, DirectoryInternalConvertKeyResult> results)
    {
      IList<KeyValuePair<string, Guid>> oids = AzureActiveDirectoryConvertKeysHelper.ConvertKeysToOids((IEnumerable<string>) results.Keys);
      List<Guid> list1 = oids.Select<KeyValuePair<string, Guid>, Guid>((Func<KeyValuePair<string, Guid>, Guid>) (kvp => kvp.Value)).ToList<Guid>();
      AadService service1 = context.GetService<AadService>();
      IDictionary<Guid, AadUser> users = service1.GetUsersWithIds<Guid>(context, new GetUsersWithIdsRequest<Guid>()
      {
        Identifiers = (IEnumerable<Guid>) list1
      }).Users;
      IDictionary<Guid, AadGroup> groups = service1.GetGroupsWithIds<Guid>(context, new GetGroupsWithIdsRequest<Guid>()
      {
        Identifiers = (IEnumerable<Guid>) list1
      }).Groups;
      List<KeyValuePair<string, IdentityDescriptor>> source = new List<KeyValuePair<string, IdentityDescriptor>>(oids.Count);
      Guid tenantId;
      if (!DirectoryUtils.TryGetOrganizationTenantId(context, out tenantId))
        return;
      string domain = tenantId.ToString();
      foreach (KeyValuePair<string, Guid> keyValuePair in (IEnumerable<KeyValuePair<string, Guid>>) oids)
      {
        string key = keyValuePair.Key;
        Guid guid = keyValuePair.Value;
        AadUser aadUser;
        if (users.TryGetValue(guid, out aadUser))
        {
          IdentityDescriptor descriptorFromAccountName = IdentityHelper.CreateDescriptorFromAccountName(domain, aadUser.SignInAddress);
          source.Add(new KeyValuePair<string, IdentityDescriptor>(key, descriptorFromAccountName));
        }
        else if (groups.TryGetValue(guid, out AadGroup _))
        {
          IdentityDescriptor descriptorFromSid = IdentityHelper.CreateDescriptorFromSid(SidIdentityHelper.ConstructAadGroupSid(guid));
          source.Add(new KeyValuePair<string, IdentityDescriptor>(key, descriptorFromSid));
        }
      }
      IdentityService service2 = context.GetService<IdentityService>();
      List<IdentityDescriptor> list2 = source.Select<KeyValuePair<string, IdentityDescriptor>, IdentityDescriptor>((Func<KeyValuePair<string, IdentityDescriptor>, IdentityDescriptor>) (kvp => kvp.Value)).ToList<IdentityDescriptor>();
      IVssRequestContext requestContext = context;
      List<IdentityDescriptor> descriptors = list2;
      IList<Microsoft.VisualStudio.Services.Identity.Identity> identityList = service2.ReadIdentities(requestContext, (IList<IdentityDescriptor>) descriptors, QueryMembership.None, (IEnumerable<string>) null);
      for (int index = 0; index < source.Count; ++index)
      {
        string key = source[index].Key;
        Microsoft.VisualStudio.Services.Identity.Identity identity = identityList[index];
        if (identity != null)
        {
          string str = identity.Id.ToString();
          results[key] = new DirectoryInternalConvertKeyResult()
          {
            Key = str
          };
        }
      }
    }

    private static void ConvertAadToAad(
      IVssRequestContext context,
      IDictionary<string, DirectoryInternalConvertKeyResult> results)
    {
      IList<KeyValuePair<string, Guid>> oids = AzureActiveDirectoryConvertKeysHelper.ConvertKeysToOids((IEnumerable<string>) results.Keys);
      List<Guid> list = oids.Select<KeyValuePair<string, Guid>, Guid>((Func<KeyValuePair<string, Guid>, Guid>) (kvp => kvp.Value)).ToList<Guid>();
      AadService service = context.GetService<AadService>();
      IDictionary<Guid, AadUser> users = service.GetUsersWithIds<Guid>(context, new GetUsersWithIdsRequest<Guid>()
      {
        Identifiers = (IEnumerable<Guid>) list
      }).Users;
      IDictionary<Guid, AadGroup> groups = service.GetGroupsWithIds<Guid>(context, new GetGroupsWithIdsRequest<Guid>()
      {
        Identifiers = (IEnumerable<Guid>) list
      }).Groups;
      foreach (KeyValuePair<string, Guid> keyValuePair in (IEnumerable<KeyValuePair<string, Guid>>) oids)
      {
        string key1 = keyValuePair.Key;
        Guid key2 = keyValuePair.Value;
        if (users.TryGetValue(key2, out AadUser _))
        {
          string str = key2.ToString();
          results[key1] = new DirectoryInternalConvertKeyResult()
          {
            Key = str
          };
        }
        else if (groups.TryGetValue(key2, out AadGroup _))
        {
          string str = key2.ToString();
          results[key1] = new DirectoryInternalConvertKeyResult()
          {
            Key = str
          };
        }
      }
    }

    private static void ConvertAadToSignInAddress(
      IVssRequestContext context,
      IDictionary<string, DirectoryInternalConvertKeyResult> results)
    {
      IList<KeyValuePair<string, Guid>> oids = AzureActiveDirectoryConvertKeysHelper.ConvertKeysToOids((IEnumerable<string>) results.Keys);
      List<Guid> list = oids.Select<KeyValuePair<string, Guid>, Guid>((Func<KeyValuePair<string, Guid>, Guid>) (kvp => kvp.Value)).ToList<Guid>();
      IDictionary<Guid, AadUser> users = context.GetService<AadService>().GetUsersWithIds<Guid>(context, new GetUsersWithIdsRequest<Guid>()
      {
        Identifiers = (IEnumerable<Guid>) list
      }).Users;
      foreach (KeyValuePair<string, Guid> keyValuePair in (IEnumerable<KeyValuePair<string, Guid>>) oids)
      {
        string key1 = keyValuePair.Key;
        Guid key2 = keyValuePair.Value;
        AadUser aadUser;
        if (users.TryGetValue(key2, out aadUser))
        {
          string signInAddress = aadUser.SignInAddress;
          results[key1] = new DirectoryInternalConvertKeyResult()
          {
            Key = signInAddress
          };
        }
      }
    }

    private static IList<KeyValuePair<string, Guid>> ConvertKeysToOids(IEnumerable<string> keys) => (IList<KeyValuePair<string, Guid>>) keys.Select<string, KeyValuePair<string, Guid>>((Func<string, KeyValuePair<string, Guid>>) (key =>
    {
      Guid result;
      Guid.TryParse(key, out result);
      return new KeyValuePair<string, Guid>(key, result);
    })).Where<KeyValuePair<string, Guid>>((Func<KeyValuePair<string, Guid>, bool>) (kvp => kvp.Value != Guid.Empty)).ToList<KeyValuePair<string, Guid>>();
  }
}
