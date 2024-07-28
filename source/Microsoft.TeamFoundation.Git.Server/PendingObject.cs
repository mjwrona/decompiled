// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.PendingObject
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

namespace Microsoft.TeamFoundation.Git.Server
{
  internal class PendingObject
  {
    public readonly Sha1Id ObjectId;
    public readonly GitPackObjectType ObjectType;
    public readonly long Offset;
    public readonly long Length;

    public PendingObject(Sha1Id objectId, GitPackObjectType objectType, long offset, long length)
    {
      this.ObjectId = objectId;
      this.ObjectType = objectType;
      this.Offset = offset;
      this.Length = length;
    }
  }
}
