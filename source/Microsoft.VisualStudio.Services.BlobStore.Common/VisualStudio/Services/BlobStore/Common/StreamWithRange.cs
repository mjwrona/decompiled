// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Common.StreamWithRange
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FAFB0281-5CF2-4D3F-992C-49FBB9BEC906
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.BlobStore.Common.dll

using System;
using System.IO;

namespace Microsoft.VisualStudio.Services.BlobStore.Common
{
  public struct StreamWithRange : IDisposable
  {
    public readonly Stream Stream;
    public readonly StreamRange Range;

    public StreamWithRange(Stream stream)
      : this(stream, new StreamRange(stream))
    {
    }

    public StreamWithRange(Stream stream, StreamRange range)
    {
      this.Stream = stream;
      this.Range = range;
    }

    public void Dispose() => this.Stream.Dispose();
  }
}
