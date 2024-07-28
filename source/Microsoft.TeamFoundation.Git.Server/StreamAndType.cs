// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.StreamAndType
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using System.IO;

namespace Microsoft.TeamFoundation.Git.Server
{
  internal struct StreamAndType
  {
    public readonly Stream Stream;
    public readonly GitPackObjectType PackType;
    public readonly Sha1Id? BaseObjectId;

    public StreamAndType(Stream stream, GitPackObjectType packType, Sha1Id? baseObjectId)
    {
      this.Stream = stream;
      this.PackType = packType;
      this.BaseObjectId = baseObjectId;
    }
  }
}
