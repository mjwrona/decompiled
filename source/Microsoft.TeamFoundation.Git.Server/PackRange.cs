// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.PackRange
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

namespace Microsoft.TeamFoundation.Git.Server
{
  internal class PackRange
  {
    public PackRange(long offset, long length, int objectCount)
    {
      this.Offset = offset;
      this.Length = length;
      this.ObjectCount = objectCount;
      this.PackId = StorageUtils.CreateUniqueId();
      this.PackName = StorageUtils.GetPackFileName(this.PackId);
    }

    public long Offset { get; }

    public long Length { get; }

    public int ObjectCount { get; }

    public Sha1Id PackId { get; }

    public string PackName { get; }
  }
}
