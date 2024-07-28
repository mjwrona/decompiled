// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.EsrpSourceFileDescription
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using System;

namespace Microsoft.VisualStudio.Services.Gallery.Server
{
  public class EsrpSourceFileDescription
  {
    public readonly string Path;
    public readonly long Size;
    public readonly string Hash;

    public EsrpSourceFileDescription(string path, long size, string hash)
    {
      this.Path = path ?? throw new ArgumentNullException(nameof (path));
      this.Size = size >= 0L ? size : throw new ArgumentOutOfRangeException(nameof (size), "size cannot be negative");
      this.Hash = hash ?? throw new ArgumentNullException(nameof (hash));
      if (hash.Length <= 0)
        throw new ArgumentException("hash length must be greater than 0", nameof (hash));
    }
  }
}
