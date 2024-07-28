// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.TfsGitTag
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Git.Server.TfsGitObjects;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using System;
using System.Collections.Generic;
using System.IO;

namespace Microsoft.TeamFoundation.Git.Server
{
  public sealed class TfsGitTag : TfsGitObject
  {
    private TagCore m_core;

    internal TfsGitTag(ICachedGitObjectSet objectSet, Sha1Id objectId)
      : base(objectSet, objectId)
    {
    }

    public override GitObjectType ObjectType => GitObjectType.Tag;

    internal override GitPackObjectType PackType => GitPackObjectType.Tag;

    internal override IEnumerable<Sha1Id> ReferencedObjectIds
    {
      get
      {
        this.EnsureCore();
        return (IEnumerable<Sha1Id>) new Sha1Id[1]
        {
          this.m_core.ReferencedObject.ObjectId
        };
      }
    }

    public string GetName()
    {
      this.EnsureCore();
      return this.m_core.Name;
    }

    public IdentityAndDate GetTagger()
    {
      this.EnsureCore();
      return this.m_core.Tagger;
    }

    public TfsGitObject GetReferencedObject()
    {
      this.EnsureCore();
      ObjectIdAndType referencedObject = this.m_core.ReferencedObject;
      switch (referencedObject.ObjectType)
      {
        case GitPackObjectType.Commit:
          return (TfsGitObject) new TfsGitCommit(this.ObjectSet, referencedObject.ObjectId);
        case GitPackObjectType.Tree:
          return (TfsGitObject) new TfsGitTree(this.ObjectSet, referencedObject.ObjectId);
        case GitPackObjectType.Blob:
          return (TfsGitObject) new TfsGitBlob(this.ObjectSet, referencedObject.ObjectId);
        case GitPackObjectType.Tag:
          return (TfsGitObject) new TfsGitTag(this.ObjectSet, referencedObject.ObjectId);
        default:
          throw new Exception("Invalid object type for tag target");
      }
    }

    public string GetComment()
    {
      this.EnsureCore();
      return this.m_core.Comment;
    }

    private void EnsureCore()
    {
      if (this.m_core != null || this.ObjectSet.TryGetObjectCoreFromCache<TagCore>(this.ObjectId, out this.m_core))
        return;
      using (Stream content = this.GetContent())
        this.m_core = TagCore.Parse(content);
      this.ObjectSet.TryCacheObjectCore(this.ObjectId, (IGitObjectCore) this.m_core);
    }
  }
}
