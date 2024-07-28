// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.OnPrem.SqlBlockInfo
// Assembly: Microsoft.VisualStudio.Services.BlobStore.OnPrem, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EA52CF3A-8E8F-49A1-8A12-783B16F9478A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.BlobStore.OnPrem.dll

using Microsoft.VisualStudio.Services.BlobStore.Common;

namespace Microsoft.VisualStudio.Services.BlobStore.OnPrem
{
  internal class SqlBlockInfo
  {
    internal BlobBlockHash BlockHash { get; set; }

    internal int BlockFileId { get; set; }

    internal long BlockFileLength { get; set; }

    internal bool IsCommitted { get; set; }
  }
}
