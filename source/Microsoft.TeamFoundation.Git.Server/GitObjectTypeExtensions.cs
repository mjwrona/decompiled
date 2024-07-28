// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.GitObjectTypeExtensions
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.SourceControl.WebApi;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Git.Server
{
  internal static class GitObjectTypeExtensions
  {
    private static readonly IReadOnlyDictionary<GitObjectType, GitPackObjectType> s_objectTypeToPackType = (IReadOnlyDictionary<GitObjectType, GitPackObjectType>) new Dictionary<GitObjectType, GitPackObjectType>()
    {
      [GitObjectType.Blob] = GitPackObjectType.Blob,
      [GitObjectType.Commit] = GitPackObjectType.Commit,
      [GitObjectType.Tag] = GitPackObjectType.Tag,
      [GitObjectType.Tree] = GitPackObjectType.Tree
    };
    private static readonly IReadOnlyDictionary<GitPackObjectType, GitObjectType> s_packTypeToObjectType = (IReadOnlyDictionary<GitPackObjectType, GitObjectType>) new Dictionary<GitPackObjectType, GitObjectType>()
    {
      [GitPackObjectType.Blob] = GitObjectType.Blob,
      [GitPackObjectType.Commit] = GitObjectType.Commit,
      [GitPackObjectType.Tag] = GitObjectType.Tag,
      [GitPackObjectType.Tree] = GitObjectType.Tree
    };
    private static readonly IReadOnlyDictionary<GitPackObjectType, string> s_packTypeToString = (IReadOnlyDictionary<GitPackObjectType, string>) new Dictionary<GitPackObjectType, string>()
    {
      [GitPackObjectType.Blob] = "blob",
      [GitPackObjectType.Commit] = "commit",
      [GitPackObjectType.Tag] = "tag",
      [GitPackObjectType.Tree] = "tree"
    };
    private static readonly IReadOnlyDictionary<string, GitPackObjectType> s_stringToPackType = (IReadOnlyDictionary<string, GitPackObjectType>) new Dictionary<string, GitPackObjectType>()
    {
      ["blob"] = GitPackObjectType.Blob,
      ["commit"] = GitPackObjectType.Commit,
      ["tag"] = GitPackObjectType.Tag,
      ["tree"] = GitPackObjectType.Tree
    };
    private static readonly IReadOnlyDictionary<Type, GitPackObjectType> s_tfsTypeToPackObjectType = (IReadOnlyDictionary<Type, GitPackObjectType>) new Dictionary<Type, GitPackObjectType>()
    {
      [typeof (TfsGitBlob)] = GitPackObjectType.Blob,
      [typeof (TfsGitCommit)] = GitPackObjectType.Commit,
      [typeof (TfsGitTag)] = GitPackObjectType.Tag,
      [typeof (TfsGitTree)] = GitPackObjectType.Tree
    };

    internal static GitObjectType GetObjectType(this GitPackObjectType packType)
    {
      GitObjectType objectType;
      if (!GitObjectTypeExtensions.s_packTypeToObjectType.TryGetValue(packType, out objectType))
        throw new InvalidOperationException(Resources.Format("InvalidPackType", (object) packType.ToString()));
      return objectType;
    }

    internal static GitPackObjectType GetPackType(this GitObjectType objectType)
    {
      GitPackObjectType packType;
      if (!objectType.TryGetPackType(out packType))
        throw new InvalidOperationException(Resources.Format("InvalidObjectType", (object) packType.ToString()));
      return packType;
    }

    internal static GitPackObjectType GetPackType(string gitType)
    {
      GitPackObjectType packType;
      if (!GitObjectTypeExtensions.s_stringToPackType.TryGetValue(gitType, out packType))
        throw new InvalidOperationException("'" + gitType + "' doesn't map to a GitPackObjectType");
      return packType;
    }

    internal static GitPackObjectType GetPackType(Type tfsGitObjectType)
    {
      GitPackObjectType packType;
      GitObjectTypeExtensions.s_tfsTypeToPackObjectType.TryGetValue(tfsGitObjectType, out packType);
      return packType;
    }

    internal static string ToGitString(this GitPackObjectType packType)
    {
      string gitString;
      if (!GitObjectTypeExtensions.s_packTypeToString.TryGetValue(packType, out gitString))
        throw new InvalidOperationException();
      return gitString;
    }

    public static StatMode ToMode(this GitPackObjectType packType)
    {
      switch (packType)
      {
        case GitPackObjectType.Commit:
          return StatMode.S_IFBLK | StatMode.S_IFREG;
        case GitPackObjectType.Tree:
          return StatMode.S_IFDIR;
        case GitPackObjectType.Blob:
          return StatMode.S_644 | StatMode.S_IFREG;
        default:
          throw new InvalidOperationException("You can only add blobs, commits, or trees to tree objects.");
      }
    }

    internal static bool TryGetPackType(
      this GitObjectType objectType,
      out GitPackObjectType packType)
    {
      return GitObjectTypeExtensions.s_objectTypeToPackType.TryGetValue(objectType, out packType);
    }
  }
}
