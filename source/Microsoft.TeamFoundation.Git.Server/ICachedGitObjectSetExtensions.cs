// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.ICachedGitObjectSetExtensions
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Git.Server.TfsGitObjects;

namespace Microsoft.TeamFoundation.Git.Server
{
  internal static class ICachedGitObjectSetExtensions
  {
    public static bool TryGetObjectCoreFromCache<TGitObjectCore>(
      this ICachedGitObjectSet objectDB,
      Sha1Id objectId,
      out TGitObjectCore value)
      where TGitObjectCore : class, IGitObjectCore
    {
      IGitObjectCore gitObjectCore;
      if (!objectDB.TryGetObjectCoreFromCache(objectId, out gitObjectCore))
      {
        value = default (TGitObjectCore);
        return false;
      }
      value = gitObjectCore as TGitObjectCore;
      return (object) value != null;
    }
  }
}
