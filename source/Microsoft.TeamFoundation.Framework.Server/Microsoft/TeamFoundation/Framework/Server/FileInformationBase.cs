// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.FileInformationBase
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public abstract class FileInformationBase
  {
    private byte[] m_hashValue;

    public FileInformationBase(string repositoryGuid, string fileIdentifier)
      : this(repositoryGuid, fileIdentifier, (byte[]) null)
    {
    }

    public FileInformationBase(string repositoryGuid, string fileIdentifier, byte[] hashValue)
    {
      this.RepositoryGuid = repositoryGuid;
      this.FileIdentifier = fileIdentifier;
      this.HashValue = hashValue;
      this.RepositoryId = new Guid(repositoryGuid);
    }

    public FileInformationBase(Guid repositoryId, string fileIdentifier, byte[] hashValue)
    {
      this.RepositoryId = repositoryId;
      this.RepositoryGuid = repositoryId.ToString();
      this.FileIdentifier = fileIdentifier;
      this.HashValue = hashValue;
    }

    public override string ToString() => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "FileInformation\r\n[\r\n    RepositoryId = {0}\r\n    FileId       = {1}\r\n    ContentType  = {2}\r\n    HashValue    = {3}\r\n]", (object) this.RepositoryId, (object) this.FileIdentifier, (object) this.ContentType, (object) (this.HashValueString ?? "null"));

    public string ContentType { get; set; }

    public byte[] HashValue
    {
      get => this.m_hashValue;
      set
      {
        this.m_hashValue = value;
        if (this.m_hashValue != null)
          this.HashValueString = Convert.ToBase64String(this.HashValue);
        else
          this.HashValueString = (string) null;
      }
    }

    public string HashValueString { get; private set; }

    public Guid RepositoryId { get; set; }

    public string RepositoryGuid { get; set; }

    public string FileIdentifier { get; set; }

    public long Length { get; set; }

    public long UncompressedLength { get; set; }

    protected abstract string GetRelativePath();

    internal string GetFilePath() => Path.Combine(this.RepositoryGuid, this.GetRelativePath());

    protected static string ComputeFilePathFromInteger(int fileIdentifier, string filenameSuffix = ".pxy")
    {
      StringBuilder sb1 = new StringBuilder(2);
      StringBuilder sb2 = new StringBuilder(10);
      byte byteValue1 = (byte) (fileIdentifier >> 24);
      if (byteValue1 > (byte) 0)
        FileInformationBase.ByteToHex(byteValue1, sb1);
      byte byteValue2 = (byte) (fileIdentifier >> 16);
      if (byteValue2 > (byte) 0)
        FileInformationBase.ByteToHex(byteValue2, sb2);
      byte byteValue3 = (byte) (fileIdentifier >> 8);
      if (byteValue3 > (byte) 0 || byteValue3 == (byte) 0 && sb2.Length > 0)
        FileInformationBase.ByteToHex(byteValue3, sb2);
      byte byteValue4 = (byte) fileIdentifier;
      FileInformationBase.ByteToHex(byteValue4, sb1);
      FileInformationBase.ByteToHex(byteValue4, sb2);
      if (!string.IsNullOrEmpty(filenameSuffix))
        sb2.Append(filenameSuffix);
      return Path.Combine(sb1.ToString(), sb2.ToString());
    }

    internal string GetTempFilePath()
    {
      Guid guid = Guid.NewGuid();
      return Path.Combine(this.RepositoryId.ToString(), string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}\\{1:X}", (object) "Downloads", (object) guid.ToString("N")));
    }

    private static void ByteToHex(byte byteValue, StringBuilder sb)
    {
      string str = "0123456789ABCDEF";
      sb.Append(str[(int) byteValue >> 4]);
      sb.Append(str[(int) byteValue & 15]);
    }

    internal struct FileHeader
    {
      public Guid Signature;
      public short VersionStamp;
      public short HashValueSize;
      public short HashValueOffset;
      public short ContentTypeLength;
      public short ContentTypeOffset;
      public short ContentOffset;
      public uint CRC32;
      public long UncompressedLength;
      private short m_contentStart;
      private byte[] m_hashValue;
      private string m_contentType;
      public static readonly Guid SignatureReference = new Guid("25d26d14-18d2-47f0-b491-3501ae19817f");
      private const short HeaderVersion10 = 10;
      private const short HeaderVersion11 = 11;
      private const short HeaderVersion12 = 12;
      public const short HeaderSize = 256;
      private static readonly int SignatureOffset = (int) Marshal.OffsetOf(typeof (FileInformationBase.FileHeader), nameof (Signature));
      private static readonly int VersionStampOffset = (int) Marshal.OffsetOf(typeof (FileInformationBase.FileHeader), nameof (VersionStamp));
      private static readonly int HashValueSizeOffset = (int) Marshal.OffsetOf(typeof (FileInformationBase.FileHeader), nameof (HashValueSize));
      private static readonly int HashValueOffsetOffset = (int) Marshal.OffsetOf(typeof (FileInformationBase.FileHeader), nameof (HashValueOffset));
      private static readonly int ContentTypeLengthOffset = (int) Marshal.OffsetOf(typeof (FileInformationBase.FileHeader), nameof (ContentTypeLength));
      private static readonly int ContentTypeOffsetOffset = (int) Marshal.OffsetOf(typeof (FileInformationBase.FileHeader), nameof (ContentTypeOffset));
      private static readonly int ContentOffsetOffset = (int) Marshal.OffsetOf(typeof (FileInformationBase.FileHeader), nameof (ContentOffset));
      private static readonly int CRC32Offset = (int) Marshal.OffsetOf(typeof (FileInformationBase.FileHeader), nameof (CRC32));
      private static readonly int Version10ContentStartOffset = FileInformationBase.FileHeader.CRC32Offset;
      private static readonly int UncompressedLengthOffset = (int) Marshal.OffsetOf(typeof (FileInformationBase.FileHeader), nameof (UncompressedLength));
      private static readonly int Version11ContentStartOffset = FileInformationBase.FileHeader.UncompressedLengthOffset;
      private static readonly int ContentStartOffset = (int) Marshal.OffsetOf(typeof (FileInformationBase.FileHeader), nameof (m_contentStart));

      public string ContentType => this.m_contentType;

      public byte[] HashValue => this.m_hashValue;

      public static byte[] GetBytes(
        byte[] hashValue,
        string contentType,
        uint crc32,
        long uncompressedLength)
      {
        FileInformationBase.FileHeader fileHeader = new FileInformationBase.FileHeader();
        byte[] bytes1 = new byte[256];
        fileHeader.Signature = FileInformationBase.FileHeader.SignatureReference;
        fileHeader.VersionStamp = (short) 12;
        byte[] bytes2 = Encoding.UTF8.GetBytes(contentType);
        fileHeader.ContentOffset = (short) bytes1.Length;
        fileHeader.HashValueOffset = (short) FileInformationBase.FileHeader.ContentStartOffset;
        fileHeader.HashValueSize = (short) hashValue.Length;
        fileHeader.ContentTypeOffset = (short) ((int) fileHeader.HashValueOffset + (int) fileHeader.HashValueSize);
        fileHeader.ContentTypeLength = (short) bytes2.Length;
        fileHeader.CRC32 = crc32;
        fileHeader.UncompressedLength = uncompressedLength;
        FileInformationBase.FileHeader.Write(fileHeader.Signature, bytes1, FileInformationBase.FileHeader.SignatureOffset);
        FileInformationBase.FileHeader.Write(fileHeader.VersionStamp, bytes1, FileInformationBase.FileHeader.VersionStampOffset);
        FileInformationBase.FileHeader.Write(fileHeader.HashValueSize, bytes1, FileInformationBase.FileHeader.HashValueSizeOffset);
        FileInformationBase.FileHeader.Write(fileHeader.HashValueOffset, bytes1, FileInformationBase.FileHeader.HashValueOffsetOffset);
        FileInformationBase.FileHeader.Write(fileHeader.ContentTypeLength, bytes1, FileInformationBase.FileHeader.ContentTypeLengthOffset);
        FileInformationBase.FileHeader.Write(fileHeader.ContentTypeOffset, bytes1, FileInformationBase.FileHeader.ContentTypeOffsetOffset);
        FileInformationBase.FileHeader.Write(fileHeader.ContentOffset, bytes1, FileInformationBase.FileHeader.ContentOffsetOffset);
        FileInformationBase.FileHeader.Write(fileHeader.CRC32, bytes1, FileInformationBase.FileHeader.CRC32Offset);
        FileInformationBase.FileHeader.Write(fileHeader.UncompressedLength, bytes1, FileInformationBase.FileHeader.UncompressedLengthOffset);
        Array.Copy((Array) hashValue, 0, (Array) bytes1, (int) fileHeader.HashValueOffset, hashValue.Length);
        Array.Copy((Array) bytes2, 0, (Array) bytes1, (int) fileHeader.ContentTypeOffset, bytes2.Length);
        return bytes1;
      }

      public FileHeader(byte[] header)
      {
        this.m_contentStart = (short) 0;
        this.Signature = header.Length >= 256 ? FileInformationBase.FileHeader.ReadGuid(header, FileInformationBase.FileHeader.SignatureOffset) : throw new CorruptHeadersException("Size does not match");
        if (this.Signature != FileInformationBase.FileHeader.SignatureReference)
          throw new CorruptHeadersException("Signature does not match");
        this.VersionStamp = BitConverter.ToInt16(header, FileInformationBase.FileHeader.VersionStampOffset);
        if (this.VersionStamp < (short) 10)
          throw new CorruptHeadersException("Header version does not make sense");
        this.HashValueSize = BitConverter.ToInt16(header, FileInformationBase.FileHeader.HashValueSizeOffset);
        if (this.HashValueSize > (short) 256)
          throw new CorruptHeadersException("Hash Value size is greater than 256 bytes");
        this.HashValueOffset = BitConverter.ToInt16(header, FileInformationBase.FileHeader.HashValueOffsetOffset);
        if (this.HashValueOffset > (short) 256 || this.VersionStamp == (short) 10 && (int) this.HashValueOffset < FileInformationBase.FileHeader.Version10ContentStartOffset || this.VersionStamp == (short) 11 && (int) this.HashValueOffset < FileInformationBase.FileHeader.Version11ContentStartOffset || this.VersionStamp == (short) 12 && (int) this.HashValueOffset < FileInformationBase.FileHeader.ContentStartOffset)
          throw new CorruptHeadersException("HashValueOffset is out of bounds");
        this.ContentTypeLength = BitConverter.ToInt16(header, FileInformationBase.FileHeader.ContentTypeLengthOffset);
        if (this.ContentTypeLength >= (short) 256)
          throw new CorruptHeadersException("ContentTypeLength is too large");
        this.ContentTypeOffset = BitConverter.ToInt16(header, FileInformationBase.FileHeader.ContentTypeOffsetOffset);
        if (this.ContentTypeOffset >= (short) 256 || (int) this.ContentTypeOffset < (int) this.HashValueOffset)
          throw new CorruptHeadersException("ContentType Offset is out of bounds");
        this.ContentOffset = BitConverter.ToInt16(header, FileInformationBase.FileHeader.ContentOffsetOffset);
        if (this.ContentOffset < (short) 256)
          throw new CorruptHeadersException("Content Offset is less than header size");
        this.CRC32 = this.VersionStamp < (short) 11 ? 0U : BitConverter.ToUInt32(header, FileInformationBase.FileHeader.CRC32Offset);
        this.UncompressedLength = this.VersionStamp < (short) 12 ? 0L : BitConverter.ToInt64(header, FileInformationBase.FileHeader.UncompressedLengthOffset);
        this.m_hashValue = new byte[(int) this.HashValueSize];
        Array.Copy((Array) header, (int) this.HashValueOffset, (Array) this.m_hashValue, 0, (int) this.HashValueSize);
        this.m_contentType = Encoding.UTF8.GetString(header, (int) this.ContentTypeOffset, (int) this.ContentTypeLength);
      }

      private static int Write(short value, byte[] buffer, int offset)
      {
        buffer[offset] = (byte) value;
        buffer[offset + 1] = (byte) ((uint) value >> 8);
        return 2;
      }

      private static int Write(uint value, byte[] buffer, int offset)
      {
        buffer[offset] = (byte) value;
        buffer[offset + 1] = (byte) (value >> 8);
        buffer[offset + 2] = (byte) (value >> 16);
        buffer[offset + 3] = (byte) (value >> 24);
        return 4;
      }

      private static int Write(long value, byte[] buffer, int offset)
      {
        buffer[offset] = (byte) value;
        buffer[offset + 1] = (byte) (value >> 8);
        buffer[offset + 2] = (byte) (value >> 16);
        buffer[offset + 3] = (byte) (value >> 24);
        buffer[offset + 4] = (byte) (value >> 32);
        buffer[offset + 5] = (byte) (value >> 40);
        buffer[offset + 6] = (byte) (value >> 48);
        buffer[offset + 7] = (byte) (value >> 56);
        return 8;
      }

      public static unsafe int Write(Guid value, byte[] buffer, int offset)
      {
        byte* numPtr = (byte*) &value;
        for (int index = 0; index < 16; index += 4)
        {
          buffer[offset] = numPtr[index];
          buffer[offset + 1] = numPtr[index + 1];
          buffer[offset + 2] = numPtr[index + 2];
          buffer[offset + 3] = numPtr[index + 3];
          offset += 4;
        }
        return 16;
      }

      public static unsafe Guid ReadGuid(byte[] buffer, int offset)
      {
        if (offset < 0 || offset > buffer.Length - sizeof (Guid))
          throw new ArgumentOutOfRangeException(nameof (offset));
        Guid guid;
        byte* numPtr = (byte*) &guid;
        for (int index = 0; index < sizeof (Guid); index += 4)
        {
          numPtr[index] = buffer[offset];
          numPtr[index + 1] = buffer[offset + 1];
          numPtr[index + 2] = buffer[offset + 2];
          numPtr[index + 3] = buffer[offset + 3];
          offset += 4;
        }
        return guid;
      }
    }
  }
}
