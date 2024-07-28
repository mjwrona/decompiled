// Decompiled with JetBrains decompiler
// Type: Microsoft.DataDeduplication.Interop.IDedupDataPortManager
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FAFB0281-5CF2-4D3F-992C-49FBB9BEC906
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.BlobStore.Common.dll

using System;
using System.Runtime.InteropServices;

namespace Microsoft.DataDeduplication.Interop
{
  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [Guid("44677452-b90a-445e-8192-cdcfe81511fb")]
  [CLSCompliant(false)]
  public interface IDedupDataPortManager
  {
    void GetConfiguration(
      out uint minChunkSize,
      out uint maxChunkSize,
      out DedupChunkingAlgorithm chunking,
      out DedupHashingAlgorithm hashing,
      out DedupCompressionAlgorithm compression);

    void GetVolumeStatus(uint options, [MarshalAs(UnmanagedType.BStr)] string path, out DedupDataPortVolumeStatus status);

    void GetVolumeDataPort(uint options, [MarshalAs(UnmanagedType.BStr)] string path, [MarshalAs(UnmanagedType.Interface)] out IDedupDataPort dataPort);
  }
}
