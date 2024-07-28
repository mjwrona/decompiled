// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Versioning.VersionUInt64ToPaddedStringConverter
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Common.Internal;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using System;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Versioning
{
  public class VersionUInt64ToPaddedStringConverter : 
    IConverter<ulong, string>,
    IHaveInputType<ulong>,
    IHaveOutputType<string>
  {
    private readonly IConverter<ArraySegment<byte>, string> byteArrayToStringConverter;
    private readonly int byteWidth;
    private readonly ulong maxValue;

    public VersionUInt64ToPaddedStringConverter(
      IConverter<ArraySegment<byte>, string> byteArrayToStringConverter,
      int byteWidth)
    {
      ArgumentUtility.CheckForNull<IConverter<ArraySegment<byte>, string>>(byteArrayToStringConverter, nameof (byteArrayToStringConverter));
      ArgumentUtility.CheckBoundsInclusive(byteWidth, 1, 8, nameof (byteWidth));
      this.byteArrayToStringConverter = byteArrayToStringConverter;
      this.byteWidth = byteWidth;
      this.maxValue = byteWidth == 8 ? ulong.MaxValue : (ulong) ((1L << byteWidth * 8) - 1L);
    }

    public string Convert(ulong input)
    {
      if (input > this.maxValue)
        throw new ArgumentOutOfRangeException(nameof (input), CommonResources.ValueOutOfRange((object) input, (object) nameof (input), (object) 0, (object) this.maxValue));
      return this.byteArrayToStringConverter.Convert(new ArraySegment<byte>(new byte[8]
      {
        (byte) (input >> 56 & (ulong) byte.MaxValue),
        (byte) (input >> 48 & (ulong) byte.MaxValue),
        (byte) (input >> 40 & (ulong) byte.MaxValue),
        (byte) (input >> 32 & (ulong) byte.MaxValue),
        (byte) (input >> 24 & (ulong) byte.MaxValue),
        (byte) (input >> 16 & (ulong) byte.MaxValue),
        (byte) (input >> 8 & (ulong) byte.MaxValue),
        (byte) (input & (ulong) byte.MaxValue)
      }, 8 - this.byteWidth, this.byteWidth));
    }
  }
}
