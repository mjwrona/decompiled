// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Directories.DiscoveryService.GitHubDirectoryConvertKeysHelper
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Directories.DiscoveryService
{
  internal static class GitHubDirectoryConvertKeysHelper
  {
    internal static DirectoryInternalConvertKeysResponse ConvertKeys(
      IVssRequestContext context,
      DirectoryInternalConvertKeysRequest request)
    {
      Dictionary<string, DirectoryInternalConvertKeyResult> dictionary = request.Keys.ToDictionary<string, string, DirectoryInternalConvertKeyResult>((Func<string, string>) (key => key), (Func<string, DirectoryInternalConvertKeyResult>) (key => (DirectoryInternalConvertKeyResult) null));
      if (GitHubDirectoryRequestFilter.AllowRequest(context, (DirectoryInternalRequest) request))
      {
        switch (request.ConvertFrom)
        {
          case "DirectoryEntityIdentifier":
            if (request.ConvertTo == "GitHubIdentifier")
            {
              GitHubDirectoryConvertKeysHelper.PopulateEntityIdToGitHubIdDictionary(context, (IDictionary<string, DirectoryInternalConvertKeyResult>) dictionary);
              break;
            }
            break;
          case "GitHubIdentifier":
            if (!(request.ConvertTo == "DirectoryEntityIdentifier"))
              throw new NotSupportedException();
            GitHubDirectoryConvertKeysHelper.PopulateGitHubIdToEntityIdDictionary(context, (IDictionary<string, DirectoryInternalConvertKeyResult>) dictionary);
            break;
        }
      }
      return new DirectoryInternalConvertKeysResponse()
      {
        Results = (IDictionary<string, DirectoryInternalConvertKeyResult>) dictionary
      };
    }

    private static void PopulateEntityIdToGitHubIdDictionary(
      IVssRequestContext context,
      IDictionary<string, DirectoryInternalConvertKeyResult> results)
    {
      List<string> list = results.Keys.ToList<string>();
      List<KeyValuePair<string, Guid>> source = new List<KeyValuePair<string, Guid>>();
      foreach (string str in list)
      {
        DirectoryEntityIdentifier decodedId;
        if (DirectoryEntityIdentifier.TryParse(str, out decodedId) && decodedId.Version == 1)
        {
          DirectoryEntityIdentifierV1 entityIdentifierV1 = (DirectoryEntityIdentifierV1) decodedId;
          switch (entityIdentifierV1.Source)
          {
            case "ims":
              Guid result;
              if (Guid.TryParseExact(entityIdentifierV1.Id, "N", out result))
              {
                source.Add(new KeyValuePair<string, Guid>(str, result));
                continue;
              }
              continue;
            case "ghb":
              results[str] = new DirectoryInternalConvertKeyResult()
              {
                Key = entityIdentifierV1.Id
              };
              continue;
            default:
              continue;
          }
        }
      }
      if (source.Count <= 0)
        return;
      IList<Microsoft.VisualStudio.Services.Identity.Identity> identityList = context.GetService<IdentityService>().ReadIdentities(context, (IList<Guid>) source.Select<KeyValuePair<string, Guid>, Guid>((Func<KeyValuePair<string, Guid>, Guid>) (kvp => kvp.Value)).ToList<Guid>(), QueryMembership.None, (IEnumerable<string>) null);
      for (int index = 0; index < source.Count; ++index)
      {
        Microsoft.VisualStudio.Services.Identity.Identity identity = identityList[index];
        if (identity != null)
        {
          SocialDescriptor socialDescriptor1 = identity.SocialDescriptor;
          VssStringComparer socialType1 = VssStringComparer.SocialType;
          SocialDescriptor socialDescriptor2 = identity.SocialDescriptor;
          string socialType2 = socialDescriptor2.SocialType;
          if (socialType1.Compare("ghb", socialType2) == 0)
          {
            string key = source[index].Key;
            socialDescriptor2 = identity.SocialDescriptor;
            string identifier = socialDescriptor2.Identifier;
            results[key] = new DirectoryInternalConvertKeyResult()
            {
              Key = identifier
            };
          }
        }
      }
    }

    private static void PopulateGitHubIdToEntityIdDictionary(
      IVssRequestContext context,
      IDictionary<string, DirectoryInternalConvertKeyResult> results)
    {
      foreach (string str1 in results.Keys.ToList<string>())
      {
        GitHubDirectoryHelper.Instance.GetUserById(context, str1);
        string str2 = GitHubDirectoryEntityIdentifierHelper.CreateUserEntityId(str1).Encode();
        results[str1] = new DirectoryInternalConvertKeyResult()
        {
          Key = str2
        };
      }
    }
  }
}
