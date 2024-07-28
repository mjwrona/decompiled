// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.IGitObjectSetExtensions
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.SourceControl.WebApi;
using System;
using System.IO;

namespace Microsoft.TeamFoundation.Git.Server
{
  public static class IGitObjectSetExtensions
  {
    public static Stream GetContent(
      this IGitObjectSet objectSet,
      Sha1Id objectId,
      out GitObjectType objectType)
    {
      Stream content;
      if (!objectSet.TryGetContent(objectId, out content, out objectType))
        throw new GitObjectDoesNotExistException(objectId);
      return content;
    }

    public static Stream GetContentAndCheckType(
      this IGitObjectSet objectSet,
      Sha1Id objectId,
      GitObjectType objectType)
    {
      GitObjectType objectType1;
      Stream stream = objectSet.GetContent(objectId, out objectType1);
      try
      {
        if (objectType1 != objectType)
          throw new GitUnexpectedObjectTypeException(objectType, objectId, objectType1);
        Stream contentAndCheckType = stream;
        stream = (Stream) null;
        return contentAndCheckType;
      }
      finally
      {
        stream?.Dispose();
      }
    }

    public static GitObjectType LookupObjectType(this IGitObjectSet objectSet, Sha1Id objectId)
    {
      int num = (int) objectSet.TryLookupObjectType(objectId);
      return num != 0 ? (GitObjectType) num : throw new GitObjectDoesNotExistException(objectId);
    }

    public static TfsGitObject LookupObject(this IGitObjectSet objectSet, Sha1Id objectId) => objectSet.TryLookupObject(objectId) ?? throw new GitObjectDoesNotExistException(objectId);

    public static TGitObject LookupObject<TGitObject>(this IGitObjectSet objectSet, Sha1Id objectId) where TGitObject : TfsGitObject => objectSet.TryLookupObject<TGitObject>(objectId) ?? throw new GitObjectDoesNotExistException(objectId);

    public static TGitObject TryLookupObject<TGitObject>(
      this IGitObjectSet objectSet,
      Sha1Id objectId)
      where TGitObject : TfsGitObject
    {
      TfsGitObject tfsGitObject = objectSet.TryLookupObject(objectId);
      if (tfsGitObject == null)
        return default (TGitObject);
      return tfsGitObject is TGitObject gitObject ? gitObject : throw new GitUnexpectedObjectTypeException(GitObjectTypeExtensions.GetPackType(typeof (TGitObject)).GetObjectType(), objectId, tfsGitObject.ObjectType);
    }

    public static GitObjectType GetResolvableType(
      this IGitObjectSet objectSet,
      Sha1Id newObjectId,
      out GitObjectType baseObjectType)
    {
      baseObjectType = GitObjectType.Bad;
      if (newObjectId.IsEmpty)
        return GitObjectType.Bad;
      TfsGitObject gitObject = objectSet.TryLookupObject(newObjectId);
      if (gitObject == null)
        return GitObjectType.Bad;
      baseObjectType = gitObject.ObjectType;
      return gitObject.GetResolvableObjectType();
    }

    public static TfsGitObject TryLookupObject(this IGitObjectSet gitObjectSet, Sha1Id objectId)
    {
      switch (gitObjectSet.TryLookupObjectType(objectId))
      {
        case GitObjectType.Bad:
          return (TfsGitObject) null;
        case GitObjectType.Commit:
          return (TfsGitObject) new TfsGitCommit((ICachedGitObjectSet) gitObjectSet, objectId);
        case GitObjectType.Tree:
          return (TfsGitObject) new TfsGitTree((ICachedGitObjectSet) gitObjectSet, objectId);
        case GitObjectType.Blob:
          return (TfsGitObject) new TfsGitBlob((ICachedGitObjectSet) gitObjectSet, objectId);
        case GitObjectType.Tag:
          return (TfsGitObject) new TfsGitTag((ICachedGitObjectSet) gitObjectSet, objectId);
        default:
          throw new InvalidOperationException();
      }
    }
  }
}
