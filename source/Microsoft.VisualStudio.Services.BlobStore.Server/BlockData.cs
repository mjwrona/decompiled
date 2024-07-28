// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.BlockData
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D3AB5C9B-EB54-4477-A304-63BB297414A3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.Server.dll

using Microsoft.VisualStudio.Services.BlobStore.Server.Common;

namespace Microsoft.VisualStudio.Services.BlobStore.Server
{
  internal class BlockData
  {
    public BlockInfo Info { get; set; }

    public byte[] Bytes { get; set; }
  }
}
