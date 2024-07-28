// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.GitPackDeserializerObjectParserTrait
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.SourceControl.WebApi;
using System;
using System.Collections.Generic;
using System.IO;

namespace Microsoft.TeamFoundation.Git.Server
{
  internal sealed class GitPackDeserializerObjectParserTrait : IGitPackDeserializerTrait
  {
    private readonly TreeFsckOptions m_fsckOptions;
    private readonly CommitParserOptions m_commitParserOptions;
    private readonly bool m_packHasShallows;
    private readonly HashSet<ObjectIdAndType> m_requiredObjects;
    private readonly HashSet<ObjectIdAndType> m_seenObjects;

    public GitPackDeserializerObjectParserTrait(
      TreeFsckOptions fsckOptions,
      CommitParserOptions commitOptions,
      bool packHasShallows)
    {
      this.m_fsckOptions = fsckOptions;
      this.m_commitParserOptions = commitOptions;
      this.m_packHasShallows = packHasShallows;
      this.CommitHandler = new EventCommitParserHandler();
      this.CommitHandler.Tree += (EventCommitParserHandler.TreeEvent) (tree => OnReferencedObject(new ObjectIdAndType(tree, GitPackObjectType.Tree)));
      this.CommitHandler.Parent += (EventCommitParserHandler.ParentEvent) (parent => OnReferencedObject(new ObjectIdAndType(parent, GitPackObjectType.Commit)));
      this.TagHandler = new EventTagParserHandler();
      this.TagHandler.ReferencedObject += (EventTagParserHandler.ReferencedObjectEvent) (o => OnReferencedObject(o));
      this.TreeEntryHandler += (TreeParserEntryHandler) (entry =>
      {
        if (entry.PackType == GitPackObjectType.Commit)
          return;
        OnReferencedObject(new ObjectIdAndType(entry.ObjectId, entry.PackType));
      });
      this.m_requiredObjects = new HashSet<ObjectIdAndType>();
      this.m_seenObjects = new HashSet<ObjectIdAndType>();

      void OnReferencedObject(ObjectIdAndType o)
      {
        if (this.m_seenObjects.Contains(o))
          return;
        this.m_requiredObjects.Add(o);
      }
    }

    public static GitPackDeserializerObjectParserTrait CreateForOdbFsck() => new GitPackDeserializerObjectParserTrait(TreeFsckOptions.None, CommitParserOptions.None, false);

    public void AddToDeserializer(GitPackDeserializer deserializer)
    {
      this.Deserializer = deserializer;
      this.Deserializer.ObjectContent += new GitPackDeserializer.ObjectContentHandler(this.Deserializer_ObjectContent);
      this.Deserializer.ObjectInfo += new GitPackDeserializer.ObjectInfoHandler(this.Deserializer_ObjectInfo);
      this.Deserializer.DeserializationComplete += new GitPackDeserializer.DeserializationCompleteHandler(this.Deserializer_DeserializationComplete);
    }

    public GitPackDeserializer Deserializer { get; private set; }

    public EventCommitParserHandler CommitHandler { get; }

    public EventTagParserHandler TagHandler { get; }

    public event TreeParserEntryHandler TreeEntryHandler;

    public event GitPackDeserializerObjectParserTrait.ObjectInfoHandler ObjectInfo;

    public int BlobCount { get; private set; }

    public int CommitCount { get; private set; }

    public int TagCount { get; private set; }

    public int TreeCount { get; private set; }

    public int TotalObjectCount => this.BlobCount + this.CommitCount + this.TagCount + this.TreeCount;

    public int ObjectsThatAlreadyExistedCount { get; private set; }

    public long TotalExistedBytes { get; private set; }

    public long TotalCommitBytes { get; private set; }

    public long TotalTagBytes { get; private set; }

    public long TotalTreeBytes { get; private set; }

    public ISet<ObjectIdAndType> RequiredObjects => (ISet<ObjectIdAndType>) this.m_requiredObjects;

    public ISet<ObjectIdAndType> SeenObjects => (ISet<ObjectIdAndType>) this.m_seenObjects;

    private void Deserializer_ObjectContent(
      GitPackDeserializerProgress progress,
      Stream content,
      long objectLength,
      GitPackObjectType type)
    {
      switch (type)
      {
        case GitPackObjectType.Commit:
          CommitParser.ParseMetadata(content, objectLength, this.m_commitParserOptions, (ICommitParserHandler) this.CommitHandler);
          ++this.CommitCount;
          break;
        case GitPackObjectType.Tree:
          foreach (TreeParser.Entry entry in TreeParser.Parse(content, objectLength, this.m_fsckOptions))
          {
            TreeParserEntryHandler treeEntryHandler = this.TreeEntryHandler;
            if (treeEntryHandler != null)
              treeEntryHandler(entry);
          }
          ++this.TreeCount;
          break;
        case GitPackObjectType.Blob:
          ++this.BlobCount;
          break;
        case GitPackObjectType.Tag:
          TagParser.Parse(content, (ITagParserHandler) this.TagHandler);
          ++this.TagCount;
          break;
        default:
          throw new InvalidOperationException("Unknown object type!");
      }
    }

    private void Deserializer_ObjectInfo(
      GitPackDeserializerProgress progress,
      Sha1Id objectId,
      GitPackObjectType objectType,
      long objectLength,
      long offsetInPack,
      long lengthInPack)
    {
      this.m_requiredObjects.Remove(new ObjectIdAndType(objectId, objectType));
      bool isFirstInPack;
      if ((isFirstInPack = this.m_seenObjects.Add(new ObjectIdAndType(objectId, objectType))) && this.Deserializer.BaseObjects.TryLookupObjectType(objectId) != GitObjectType.Bad)
      {
        ++this.ObjectsThatAlreadyExistedCount;
        this.TotalExistedBytes += lengthInPack;
      }
      switch (objectType)
      {
        case GitPackObjectType.Commit:
          this.TotalCommitBytes += lengthInPack;
          break;
        case GitPackObjectType.Tree:
          this.TotalTreeBytes += lengthInPack;
          break;
        case GitPackObjectType.Tag:
          this.TotalTagBytes += lengthInPack;
          break;
      }
      GitPackDeserializerObjectParserTrait.ObjectInfoHandler objectInfo = this.ObjectInfo;
      if (objectInfo == null)
        return;
      objectInfo(progress, objectId, objectType, objectLength, offsetInPack, lengthInPack, isFirstInPack);
    }

    private void Deserializer_DeserializationComplete(
      Sha1Id packHash,
      Stream packStream,
      long packLength)
    {
      foreach (ObjectIdAndType requiredObject in this.m_requiredObjects)
      {
        GitPackObjectType packType;
        if (!this.Deserializer.BaseObjects.TryLookupObjectType(requiredObject.ObjectId).TryGetPackType(out packType) || requiredObject.ObjectType != GitPackObjectType.None && packType != requiredObject.ObjectType)
          throw new GitMissingReferencedObjectException(requiredObject.ObjectId, this.m_packHasShallows);
      }
    }

    public delegate void ObjectInfoHandler(
      GitPackDeserializerProgress progress,
      Sha1Id objectId,
      GitPackObjectType objectType,
      long objectLength,
      long offsetInPack,
      long lengthInPack,
      bool isFirstInPack);
  }
}
