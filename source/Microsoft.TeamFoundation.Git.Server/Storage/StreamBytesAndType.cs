// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.Storage.StreamBytesAndType
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Git.Server.Streams;
using System.IO;

namespace Microsoft.TeamFoundation.Git.Server.Storage
{
  internal readonly struct StreamBytesAndType
  {
    private readonly object m_content;
    public readonly GitPackObjectType PackType;
    public readonly int Length;

    public StreamBytesAndType(byte[] content, GitPackObjectType packType)
    {
      this.m_content = (object) content;
      this.PackType = packType;
      this.Length = content.Length;
    }

    public StreamBytesAndType(ConcatMemoryStream.Shards content, GitPackObjectType packType)
    {
      this.m_content = (object) content;
      this.PackType = packType;
      this.Length = content.Length;
    }

    public Stream GetStream() => this.m_content is byte[] content ? (Stream) new MemoryStream(content, 0, this.Length, false, true) : (Stream) new ConcatMemoryStream((ConcatMemoryStream.Shards) this.m_content);
  }
}
