// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.GitLfsObject
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

namespace Microsoft.TeamFoundation.Git.Server
{
  internal class GitLfsObject
  {
    public GitLfsObject(Sha256Id objectId, long size)
    {
      this.ObjectId = objectId;
      this.Size = size;
    }

    public GitLfsObject(Sha256Id objectId, int objectIndex, long size)
    {
      this.ObjectId = objectId;
      this.ObjectIndex = objectIndex;
      this.Size = size;
    }

    public Sha256Id ObjectId { get; set; }

    public int ObjectIndex { get; set; }

    public long Size { get; set; }
  }
}
