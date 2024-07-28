// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.BufferPoolsProvider
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public abstract class BufferPoolsProvider
  {
    protected const int Size2K = 2048;
    protected const int Size4K = 4096;
    protected const int Size8K = 8192;
    protected const int Size16K = 16384;
    protected const int Size32K = 32768;
    protected const int Size64K = 65536;
    protected const int Size128K = 131072;
    protected const int Size256K = 262144;
    protected const int Size512K = 524288;
    protected const int Size1M = 1048576;
    protected const int Size2M = 2097152;
    private Lazy<BufferPool[]> m_pool;

    internal BufferPool[] BufferPools => this.m_pool.Value;

    protected BufferPoolsProvider() => this.m_pool = new Lazy<BufferPool[]>((Func<BufferPool[]>) (() => this.CreateBufferPools()));

    protected abstract BufferPool[] CreateBufferPools();
  }
}
