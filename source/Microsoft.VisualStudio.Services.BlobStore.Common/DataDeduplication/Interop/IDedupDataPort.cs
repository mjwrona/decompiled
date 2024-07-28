// Decompiled with JetBrains decompiler
// Type: Microsoft.DataDeduplication.Interop.IDedupDataPort
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FAFB0281-5CF2-4D3F-992C-49FBB9BEC906
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.BlobStore.Common.dll

using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

namespace Microsoft.DataDeduplication.Interop
{
  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [Guid("7963d734-40a9-4ea3-bbf6-5a89d26f7ae8")]
  [CLSCompliant(false)]
  public interface IDedupDataPort
  {
    void GetStatus(out DedupDataPortVolumeStatus status, out uint maintenanceMB);

    void LookupChunks(uint count, [MarshalAs(UnmanagedType.LPArray)] DedupHash[] hashes, out Guid requestId);

    void InsertChunks(
      uint chunkCount,
      [MarshalAs(UnmanagedType.LPArray)] DedupChunk[] chunkMetadata,
      uint dataByteCount,
      [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 2)] byte[] chunkData,
      out Guid requestId);

    void InsertChunksWithStream(
      uint chunkCount,
      [MarshalAs(UnmanagedType.LPArray)] DedupChunk[] chunkMetadata,
      uint dataByteCount,
      [MarshalAs(UnmanagedType.Interface)] IStream chunkDataStream,
      out Guid requestId);

    void CommitStreams(
      uint streamCount,
      [MarshalAs(UnmanagedType.LPArray)] DedupStream[] streams,
      uint entryCount,
      [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 2)] DedupStreamEntry[] entries,
      out Guid requestId);

    void CommitStreamsWithStream(
      uint streamCount,
      [MarshalAs(UnmanagedType.LPArray)] DedupStream[] streams,
      uint entryCount,
      [MarshalAs(UnmanagedType.Interface)] IStream streamEntriesStream,
      out Guid requestId);

    void GetStreams(uint streamCount, [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.BStr)] string[] streamPaths, out Guid requestId);

    void GetStreamsResults(
      Guid requestId,
      uint waitMs,
      uint entryIndex,
      out uint streamCount,
      [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 3)] out DedupStream[] streams,
      out uint entryCount,
      [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 5)] out DedupStreamEntry[] entries,
      out DedupDataPortRequestStatus status,
      [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 3)] out int[] itemResults);

    void GetChunks(uint count, [MarshalAs(UnmanagedType.LPArray)] DedupHash[] hashes, out Guid requestId);

    void GetChunksResults(
      Guid requestId,
      uint waitMs,
      uint index,
      out uint chunkCount,
      [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 3)] out DedupChunk[] chunkMetadata,
      out uint dataByteCount,
      [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 5)] out byte[] chunkData,
      out DedupDataPortRequestStatus status,
      [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 3)] out int[] itemResults);

    void GetRequestStatus(Guid requestId, out DedupDataPortRequestStatus status);

    void GetRequestResults(
      Guid requestId,
      uint waitMs,
      out int batchResult,
      out uint batchCount,
      out DedupDataPortRequestStatus status,
      [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 3)] out int[] itemResults);
  }
}
