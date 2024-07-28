// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.TfsGitTreeEntry
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.SourceControl.WebApi;
using System;

namespace Microsoft.TeamFoundation.Git.Server
{
  public class TfsGitTreeEntry
  {
    private readonly ICachedGitObjectSet m_objectSet;
    private string m_name;
    private TfsGitObject m_object;

    internal TfsGitTreeEntry(ICachedGitObjectSet objectSet, TreeParser.Entry entry)
    {
      this.m_objectSet = objectSet;
      this.NameBytes = entry.NameBytes;
      this.PackType = entry.PackType;
      this.ObjectId = entry.ObjectId;
      this.Mode = entry.Mode;
    }

    internal int GitCompareTo(TfsGitTreeEntry other) => TreeParser.GitCompareBytes(this.NameBytes, this.PackType, other.NameBytes, other.PackType);

    public string Name
    {
      get
      {
        if (this.m_name == null)
          this.m_name = TreeParser.NameBytesToString(this.NameBytes);
        return this.m_name;
      }
    }

    internal ArraySegment<byte> NameBytes { get; }

    public GitObjectType ObjectType => this.PackType.GetObjectType();

    internal GitPackObjectType PackType { get; }

    public Sha1Id ObjectId { get; }

    public StatMode Mode { get; }

    public TfsGitObject Object
    {
      get
      {
        TfsGitObject tfsGitObject = this.m_object;
        if (tfsGitObject == null)
        {
          if (GitPackObjectType.Blob == this.PackType)
            tfsGitObject = (TfsGitObject) new TfsGitBlob(this.m_objectSet, this.ObjectId);
          else if (GitPackObjectType.Tree == this.PackType)
            tfsGitObject = (TfsGitObject) new TfsGitTree(this.m_objectSet, this.ObjectId);
          else if (GitPackObjectType.Commit == this.PackType)
            tfsGitObject = (TfsGitObject) new TfsGitCommit(this.m_objectSet, this.ObjectId);
          this.m_object = tfsGitObject;
        }
        return tfsGitObject;
      }
    }
  }
}
