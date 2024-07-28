// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.ITfsGitContentDBExtensions
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using System.IO;

namespace Microsoft.TeamFoundation.Git.Server
{
  internal static class ITfsGitContentDBExtensions
  {
    public static Stream GetContent(this IContentDB contentDB, Sha1Id objectId)
    {
      Stream content;
      if (!contentDB.TryLookupObjectAndGetContent(objectId, out GitPackObjectType _, out content))
        throw new GitObjectDoesNotExistException(objectId);
      return content;
    }

    public static void LookupObject<TRawKey>(
      this ITfsGitContentDB<TRawKey> contentDB,
      Sha1Id objectId,
      out GitPackObjectType packType,
      out TRawKey rawKey)
    {
      if (!contentDB.TryLookupObject(objectId, out packType, out rawKey))
        throw new GitObjectDoesNotExistException(objectId);
    }

    public static Stream LookupObjectAndGetContent(
      this IContentDB contentDB,
      Sha1Id objectId,
      out GitPackObjectType packType)
    {
      Stream content;
      if (!contentDB.TryLookupObjectAndGetContent(objectId, out packType, out content))
        throw new GitObjectDoesNotExistException(objectId);
      return content;
    }
  }
}
