// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.ContentStitcher.FileToZip
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Server.DedupStorage, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9521FAE3-5DB1-49D0-98DB-6A544E3AB730
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.Server.DedupStorage.dll

using System;

namespace Microsoft.VisualStudio.Services.BlobStore.Server.ContentStitcher
{
  [Serializable]
  public sealed class FileToZip
  {
    public FileToZip(string path, string dedupId)
    {
      // ISSUE: reference to a compiler-generated field
      this.\u003CPath\u003Ek__BackingField = path;
      // ISSUE: reference to a compiler-generated field
      this.\u003CDedupId\u003Ek__BackingField = dedupId;
      // ISSUE: explicit constructor call
      base.\u002Ector();
    }

    public string Path { get; set; }

    public string DedupId { get; set; }
  }
}
