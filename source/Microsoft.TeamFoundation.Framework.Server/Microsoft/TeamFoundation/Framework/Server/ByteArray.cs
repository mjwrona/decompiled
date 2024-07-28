// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ByteArray
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class ByteArray : IDisposable
  {
    private byte[] m_bytes;
    private BufferPool m_pool;
    private int m_sizeRequested;

    public ByteArray(int sizeRequested)
      : this(sizeRequested, DefaultBufferPoolsProvider.Instance)
    {
    }

    public ByteArray(int sizeRequested, BufferPoolsProvider poolProvider)
    {
      this.m_sizeRequested = sizeRequested >= 0 ? sizeRequested : throw new ArgumentOutOfRangeException(nameof (sizeRequested));
      if (sizeRequested > 1024)
        this.m_pool = ByteArray.FindBufferPool(poolProvider.BufferPools, sizeRequested);
      if (this.m_pool != null)
        this.m_bytes = this.m_pool.New();
      else
        this.m_bytes = new byte[sizeRequested];
    }

    public void Dispose()
    {
      if (this.m_bytes != null && this.m_pool != null)
        this.m_pool.Release(this.m_bytes);
      GC.SuppressFinalize((object) this);
    }

    public int SizeRequested => this.m_sizeRequested;

    public byte[] Bytes => this.m_bytes;

    private static BufferPool FindBufferPool(BufferPool[] pools, int sizeRequested)
    {
      for (int index = 0; index < pools.Length; ++index)
      {
        if (pools[index].BufferSize >= sizeRequested)
          return pools[index];
      }
      return (BufferPool) null;
    }
  }
}
