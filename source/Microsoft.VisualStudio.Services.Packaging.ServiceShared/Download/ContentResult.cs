// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Download.ContentResult
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using System;
using System.IO;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Download
{
  public class ContentResult
  {
    public ContentResult(Uri redirectToUri) => this.RedirectToUri = redirectToUri ?? throw new ArgumentNullException(nameof (redirectToUri));

    public ContentResult(Stream stream, string fileName)
    {
      if (string.IsNullOrWhiteSpace(fileName))
        throw new ArgumentException("Value cannot be null or whitespace.", nameof (fileName));
      this.Stream = stream ?? throw new ArgumentNullException(nameof (stream));
      this.FileName = fileName;
    }

    public Uri RedirectToUri { get; }

    public Stream Stream { get; }

    public string FileName { get; }
  }
}
