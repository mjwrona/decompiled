// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.Native.LibGit2Extensions
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using LibGit2Sharp;
using System;

namespace Microsoft.TeamFoundation.Git.Server.Native
{
  internal static class LibGit2Extensions
  {
    public static GitPackObjectType ToOurObjectType(this ObjectType type)
    {
      switch (type)
      {
        case ObjectType.Commit:
          return GitPackObjectType.Commit;
        case ObjectType.Tree:
          return GitPackObjectType.Tree;
        case ObjectType.Blob:
          return GitPackObjectType.Blob;
        case ObjectType.Tag:
          return GitPackObjectType.Tag;
        default:
          throw new ArgumentException("Unsupported object type {0}", type.ToString());
      }
    }
  }
}
