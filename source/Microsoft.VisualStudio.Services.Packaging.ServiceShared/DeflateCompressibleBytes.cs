// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.DeflateCompressibleBytes
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Utils;
using System;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared
{
  public class DeflateCompressibleBytes
  {
    private string uncompressedBase64String;
    private byte[] uncompressedBytes;
    private byte[] deflatedBytes;
    private string deflatedBase64String;

    private DeflateCompressibleBytes(
      string uncompressedBase64String,
      byte[] uncompressedBytes,
      byte[] deflatedBytes,
      string deflatedBase64String)
    {
      this.uncompressedBase64String = uncompressedBase64String;
      this.uncompressedBytes = uncompressedBytes;
      this.deflatedBytes = deflatedBytes;
      this.deflatedBase64String = deflatedBase64String;
    }

    public static DeflateCompressibleBytes FromUncompressedBase64String(
      string uncompressedBase64String)
    {
      return new DeflateCompressibleBytes(uncompressedBase64String ?? throw new ArgumentNullException(nameof (uncompressedBase64String)), (byte[]) null, (byte[]) null, (string) null);
    }

    public static DeflateCompressibleBytes FromUncompressedBytes(byte[] uncompressedBytes) => new DeflateCompressibleBytes((string) null, uncompressedBytes ?? throw new ArgumentNullException(nameof (uncompressedBytes)), (byte[]) null, (string) null);

    public static DeflateCompressibleBytes FromDeflatedBytes(byte[] deflatedBytes) => new DeflateCompressibleBytes((string) null, (byte[]) null, deflatedBytes ?? throw new ArgumentNullException(nameof (deflatedBytes)), (string) null);

    public static DeflateCompressibleBytes FromDeflatedBase64String(string deflatedBase64String) => new DeflateCompressibleBytes((string) null, (byte[]) null, (byte[]) null, deflatedBase64String ?? throw new ArgumentNullException(nameof (deflatedBase64String)));

    public string AsUncompressedBase64String()
    {
      if (this.uncompressedBase64String == null)
        this.uncompressedBase64String = Convert.ToBase64String(this.AsUncompressedBytes());
      return this.uncompressedBase64String;
    }

    public byte[] AsUncompressedBytes()
    {
      if (this.uncompressedBytes == null)
      {
        if (this.uncompressedBase64String != null)
        {
          this.uncompressedBytes = Convert.FromBase64String(this.uncompressedBase64String);
        }
        else
        {
          if (this.deflatedBytes == null && this.deflatedBase64String == null)
            throw new InvalidOperationException("DeflateCompressibleBytes had all fields null");
          this.uncompressedBytes = CompressionHelper.InflateByteArray(this.AsDeflatedBytes());
        }
      }
      return this.uncompressedBytes;
    }

    public byte[] AsDeflatedBytes()
    {
      if (this.deflatedBytes == null)
      {
        if (this.uncompressedBytes != null || this.uncompressedBase64String != null)
          this.deflatedBytes = CompressionHelper.DeflateByteArray(this.AsUncompressedBytes());
        else
          this.deflatedBytes = this.deflatedBase64String != null ? Convert.FromBase64String(this.deflatedBase64String) : throw new InvalidOperationException("DeflateCompressibleBytes had all fields null");
      }
      return this.deflatedBytes;
    }

    public string AsDeflatedBase64String()
    {
      if (this.deflatedBase64String == null)
        this.deflatedBase64String = Convert.ToBase64String(this.AsDeflatedBytes());
      return this.deflatedBase64String;
    }
  }
}
