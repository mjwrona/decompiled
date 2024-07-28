// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.Common.BlobStoreBufferPoolsProvider
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CB48D0BF-32A2-483C-A1D4-2F10DEBB3D56
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Runtime.InteropServices;

namespace Microsoft.VisualStudio.Services.BlobStore.Server.Common
{
  public class BlobStoreBufferPoolsProvider : BufferPoolsProvider
  {
    public static readonly BufferPoolsProvider Instance = (BufferPoolsProvider) new BlobStoreBufferPoolsProvider();
    private readonly int total2MBuffers;

    private BlobStoreBufferPoolsProvider() => this.total2MBuffers = BlobStoreBufferPoolsProvider.GetCountOf2MBuffers();

    protected override BufferPool[] CreateBufferPools() => new BufferPool[8]
    {
      new BufferPool(16384, 32),
      new BufferPool(32768, 32),
      new BufferPool(65536, 32),
      new BufferPool(131072, 32),
      new BufferPool(262144, 16),
      new BufferPool(524288, 8),
      new BufferPool(1048576, 8),
      new BufferPool(2097152, this.total2MBuffers)
    };

    private static int GetCountOf2MBuffers()
    {
      long TotalMemoryInKilobytes = 0;
      BlobStoreBufferPoolsProvider.GetPhysicallyInstalledSystemMemory(out TotalMemoryInKilobytes);
      int countOf2Mbuffers = (int) Math.Max(Math.Floor((double) (TotalMemoryInKilobytes / 1024L / 30L / 4L / 2L)), 8.0);
      if ((countOf2Mbuffers & 1) == 1)
        ++countOf2Mbuffers;
      return countOf2Mbuffers;
    }

    [DllImport("kernel32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool GetPhysicallyInstalledSystemMemory(out long TotalMemoryInKilobytes);
  }
}
