// Decompiled with JetBrains decompiler
// Type: Microsoft.DataDeduplication.Interop.DedupStream
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FAFB0281-5CF2-4D3F-992C-49FBB9BEC906
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.BlobStore.Common.dll

using System;
using System.Runtime.InteropServices;

namespace Microsoft.DataDeduplication.Interop
{
  [CLSCompliant(false)]
  public struct DedupStream
  {
    [MarshalAs(UnmanagedType.BStr)]
    public string Path;
    public ulong Offset;
    public ulong Length;
    public uint ChunkCount;
  }
}
