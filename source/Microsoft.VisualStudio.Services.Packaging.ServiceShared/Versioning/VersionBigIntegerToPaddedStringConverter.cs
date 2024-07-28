// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Versioning.VersionBigIntegerToPaddedStringConverter
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.BlobStore.Common;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using System;
using System.Numerics;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Versioning
{
  public class VersionBigIntegerToPaddedStringConverter : 
    IConverter<BigInteger, string>,
    IHaveInputType<BigInteger>,
    IHaveOutputType<string>
  {
    private const int IntMaxBytes = 6;
    private readonly IConverter<ArraySegment<byte>, string> byteArrayToStringConverter;

    public VersionBigIntegerToPaddedStringConverter(
      IConverter<ArraySegment<byte>, string> byteArrayToStringConverter)
    {
      this.byteArrayToStringConverter = byteArrayToStringConverter;
    }

    public string Convert(BigInteger input)
    {
      byte[] byteArray = input.ToByteArray();
      if (byteArray.Length > 6)
        throw new InvalidVersionException(Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Error_VersionIntegerTooLarge());
      Array.Reverse((Array) byteArray);
      byte[] numArray = new byte[6];
      Array.Copy((Array) byteArray, 0, (Array) numArray, 6 - byteArray.Length, byteArray.Length);
      return this.byteArrayToStringConverter.Convert(numArray.AsArraySegment());
    }
  }
}
