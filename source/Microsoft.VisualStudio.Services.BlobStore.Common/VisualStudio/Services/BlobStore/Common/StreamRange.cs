// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Common.StreamRange
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FAFB0281-5CF2-4D3F-992C-49FBB9BEC906
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.BlobStore.Common.dll

using System;
using System.IO;
using System.Net.Http.Headers;

namespace Microsoft.VisualStudio.Services.BlobStore.Common
{
  public struct StreamRange
  {
    public readonly long WholeLength;
    public readonly long FirstBytePositionInclusive;
    public readonly long LastBytePositionInclusive;

    public long Length => this.LastBytePositionInclusive - this.FirstBytePositionInclusive + 1L;

    public StreamRange(ContentRangeHeaderValue rangeHeader)
    {
      if (!rangeHeader.Unit.Equals("bytes", StringComparison.Ordinal))
        throw new ArgumentException("Unsupported range unit: " + rangeHeader.Unit);
      this.WholeLength = rangeHeader.Length.Value;
      long? nullable = rangeHeader.From;
      this.FirstBytePositionInclusive = nullable.Value;
      nullable = rangeHeader.To;
      this.LastBytePositionInclusive = nullable.Value;
    }

    public StreamRange(Stream stream)
    {
      this.WholeLength = stream.Length;
      this.FirstBytePositionInclusive = stream.Position;
      this.LastBytePositionInclusive = this.WholeLength - 1L;
    }

    private StreamRange(long length)
    {
      this.WholeLength = length;
      this.FirstBytePositionInclusive = 0L;
      this.LastBytePositionInclusive = length - 1L;
    }

    public static StreamRange FullRange(long length) => new StreamRange(length);
  }
}
