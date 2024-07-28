// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.ITfsGitPackIndexExtensions
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Git.Server
{
  internal static class ITfsGitPackIndexExtensions
  {
    public static bool TryLookupObject(
      this IGitPackIndex index,
      Sha1Id objectId,
      out GitPackIndexEntry entry)
    {
      int index1;
      if (!index.ObjectIds.TryGetIndex(objectId, out index1))
      {
        entry = new GitPackIndexEntry();
        return false;
      }
      entry = index.Entries[index1];
      return true;
    }

    public static GitPackIndexEntry LookupObject(this IGitPackIndex index, Sha1Id objectId)
    {
      GitPackIndexEntry entry;
      if (!index.TryLookupObject(objectId, out entry))
        throw new GitObjectDoesNotExistException(objectId);
      return entry;
    }

    public static IEnumerable<Sha1Id> GetObjectIdsByType(
      this IGitPackIndex index,
      GitPackObjectType type)
    {
      int indexPosition = 0;
      foreach (GitPackIndexEntry entry in (IEnumerable<GitPackIndexEntry>) index.Entries)
      {
        if (entry.ObjectType == type)
          yield return index.ObjectIds[indexPosition];
        ++indexPosition;
      }
    }
  }
}
