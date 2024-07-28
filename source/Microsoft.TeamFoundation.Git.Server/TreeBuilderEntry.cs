// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.TreeBuilderEntry
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

namespace Microsoft.TeamFoundation.Git.Server
{
  internal sealed class TreeBuilderEntry
  {
    public TreeBuilderEntry(TfsGitTreeEntry entry)
      : this(entry.PackType, entry.Name, entry.ObjectId, entry.Mode)
    {
    }

    public TreeBuilderEntry(GitPackObjectType packType, string name, Sha1Id objectId)
      : this(packType, name, objectId, packType.ToMode())
    {
    }

    private TreeBuilderEntry(
      GitPackObjectType packType,
      string name,
      Sha1Id objectId,
      StatMode mode)
    {
      this.Mode = mode;
      this.NameBytes = GitEncodingUtil.SafeUtf8NoBom.GetBytes(name);
      this.ObjectId = objectId;
      this.PackType = packType;
      this.Name = name;
    }

    internal static TreeBuilderEntry TEST_Create(
      GitPackObjectType packType,
      string name,
      Sha1Id objectId,
      StatMode mode)
    {
      return new TreeBuilderEntry(packType, name, objectId, mode);
    }

    public TreeBuilderEntry CopyAndModify(GitPackObjectType packType, Sha1Id objectId) => new TreeBuilderEntry(packType, this.Name, objectId, this.Mode);

    public StatMode Mode { get; }

    internal byte[] NameBytes { get; }

    public Sha1Id ObjectId { get; }

    public GitPackObjectType PackType { get; }

    public string Name { get; }
  }
}
