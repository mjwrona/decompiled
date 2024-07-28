// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Common.HashingStream
// Assembly: Microsoft.TeamFoundation.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E643B42-FE11-4FC2-A9D6-79417E26CF92
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Common.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.ComponentModel;
using System.IO;
using System.Security.Cryptography;

namespace Microsoft.TeamFoundation.Framework.Common
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class HashingStream : Stream
  {
    private readonly Stream m_stream;
    private readonly HashAlgorithm m_hash;
    private readonly bool m_leaveOpen;
    private bool m_closed;
    private bool m_transformedFinalBlock;
    private static readonly byte[] s_emptyByteArray = Array.Empty<byte>();

    public HashingStream(Stream stream, HashAlgorithm hash, bool leaveOpen = false)
    {
      ArgumentUtility.CheckForNull<Stream>(stream, nameof (stream));
      ArgumentUtility.CheckForNull<HashAlgorithm>(hash, nameof (hash));
      this.m_stream = stream;
      this.m_hash = hash;
      this.m_leaveOpen = leaveOpen;
    }

    public override void Close()
    {
      if (!this.m_closed)
      {
        this.m_closed = true;
        this.m_hash.Dispose();
        if (!this.m_leaveOpen)
          this.m_stream.Close();
      }
      base.Close();
    }

    public byte[] HashValue
    {
      get
      {
        if (!this.m_transformedFinalBlock)
        {
          this.m_hash.TransformFinalBlock(HashingStream.s_emptyByteArray, 0, 0);
          this.m_transformedFinalBlock = true;
        }
        return this.m_hash.Hash;
      }
    }

    public override bool CanRead => true;

    public override bool CanSeek => false;

    public override bool CanWrite => false;

    public override long Length => this.m_stream.Length;

    public override long Position
    {
      get => this.m_stream.Position;
      set => throw new NotImplementedException();
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
      int inputCount = this.m_stream.Read(buffer, offset, count);
      this.m_hash.TransformBlock(buffer, offset, inputCount, (byte[]) null, 0);
      return inputCount;
    }

    public override void Flush() => throw new NotImplementedException();

    public override long Seek(long offset, SeekOrigin origin) => throw new NotImplementedException();

    public override void SetLength(long value) => throw new NotImplementedException();

    public override void Write(byte[] buffer, int offset, int count) => throw new NotImplementedException();
  }
}
