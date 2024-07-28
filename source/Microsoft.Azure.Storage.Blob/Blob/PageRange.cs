// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Storage.Blob.PageRange
// Assembly: Microsoft.Azure.Storage.Blob, Version=11.2.3.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: A04A3512-352A-442F-A95B-BC1B94EF8840
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Storage.Blob.dll

using System;
using System.Globalization;

namespace Microsoft.Azure.Storage.Blob
{
  public class PageRange
  {
    public PageRange(long start, long end)
    {
      this.StartOffset = start;
      this.EndOffset = end;
    }

    public long StartOffset { get; internal set; }

    public long EndOffset { get; internal set; }

    public override string ToString() => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "bytes={0}-{1}", (object) this.StartOffset, (object) this.EndOffset);
  }
}
