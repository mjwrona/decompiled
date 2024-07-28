// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Storage.File.FileRange
// Assembly: Microsoft.Azure.Storage.File, Version=11.2.3.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: C68E95B0-8DFB-410C-8E70-706406D1A279
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Storage.File.dll

using System;
using System.Globalization;

namespace Microsoft.Azure.Storage.File
{
  public sealed class FileRange
  {
    public FileRange(long start, long end)
    {
      this.StartOffset = start;
      this.EndOffset = end;
    }

    public long StartOffset { get; internal set; }

    public long EndOffset { get; internal set; }

    public override string ToString() => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "bytes={0}-{1}", (object) this.StartOffset, (object) this.EndOffset);
  }
}
