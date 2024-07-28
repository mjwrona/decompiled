// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.SmartRouter.Common.ServerNodeWithHash
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using System;


#nullable enable
namespace Microsoft.VisualStudio.Services.Cloud.SmartRouter.Common
{
  public class ServerNodeWithHash
  {
    public ServerNodeWithHash(ServerNode server, byte[] hash, string affinityCookie)
    {
      this.Server = server.CheckArgumentIsNotNull<ServerNode>(nameof (server));
      this.Hash = new ServerNodeWithHash.HashBytes(hash.CheckArgumentIsNotNull<byte[]>(nameof (hash)));
      this.AffinityCookie = affinityCookie.CheckArgumentIsNotNullOrEmpty(nameof (affinityCookie));
    }

    public ServerNode Server { get; }

    public ServerNodeWithHash.HashBytes Hash { get; }

    public string AffinityCookie { get; }

    public struct HashBytes
    {
      private readonly byte[] m_bytes;

      public HashBytes(byte[] bytes) => this.m_bytes = bytes;

      public byte[] ToArray()
      {
        int length = this.m_bytes.Length;
        byte[] dst = new byte[length];
        Buffer.BlockCopy((Array) this.m_bytes, 0, (Array) dst, 0, length);
        return dst;
      }

      public bool IsDefaultOrEmpty => this.m_bytes == null || this.m_bytes.Length == 0;
    }
  }
}
