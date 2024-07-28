// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.Common.BasicBlobMetadata
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CB48D0BF-32A2-483C-A1D4-2F10DEBB3D56
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.Server.Common.dll

using System;

namespace Microsoft.VisualStudio.Services.BlobStore.Server.Common
{
  public struct BasicBlobMetadata
  {
    public readonly string Name;
    public readonly ulong Size;
    public readonly DateTimeOffset? LastModified;

    public BasicBlobMetadata(string name, long size, DateTimeOffset? lastModified)
    {
      if (size < 0L)
        throw new ArgumentOutOfRangeException(string.Format("Size of blob is less than zero: {0}", (object) size));
      this.Name = name;
      this.Size = (ulong) size;
      this.LastModified = lastModified;
    }
  }
}
