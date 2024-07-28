// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.PublicRepositories.PubCacheProtobufDocumentFormattingHelper
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Google.Protobuf;
using System;


#nullable enable
namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.PublicRepositories
{
  internal class PubCacheProtobufDocumentFormattingHelper
  {
    private const byte MinSupportedFileFormatVersion = 1;
    private const byte MaxSupportedFileFormatVersion = 1;
    private static readonly unsafe ReadOnlyMemory<byte> CorrectFileSignature = (ReadOnlyMemory<byte>) new ReadOnlySpan<byte>((void*) &\u003CPrivateImplementationDetails\u003E.\u00340DAFD81DBF9FF3D6B7F4294FCE552118FD3AA1E636AFF8E94F52FC34BBE2994, 4).ToArray();
    private static readonly int SignatureOffset = 0;
    private static readonly int SignatureLength = PubCacheProtobufDocumentFormattingHelper.CorrectFileSignature.Length;
    private static readonly int VersionOffset = PubCacheProtobufDocumentFormattingHelper.SignatureOffset + PubCacheProtobufDocumentFormattingHelper.SignatureLength;
    private static readonly int VersionLength = 1;
    private static readonly int FileHeaderLength = PubCacheProtobufDocumentFormattingHelper.VersionOffset + PubCacheProtobufDocumentFormattingHelper.VersionLength;
    public static readonly int ContentOffset = PubCacheProtobufDocumentFormattingHelper.FileHeaderLength;
    private static readonly int MinimumFileLength = PubCacheProtobufDocumentFormattingHelper.FileHeaderLength;

    public static int ValidateHeader(byte[] buffer)
    {
      if (buffer.Length < PubCacheProtobufDocumentFormattingHelper.MinimumFileLength)
        throw new InvalidOperationException(string.Format("Public repo cache file is too small ({0} bytes) to be valid (minimum {1} bytes).", (object) buffer.Length, (object) PubCacheProtobufDocumentFormattingHelper.MinimumFileLength));
      if (buffer.AsSpan<byte>(PubCacheProtobufDocumentFormattingHelper.SignatureOffset, PubCacheProtobufDocumentFormattingHelper.SignatureLength).SequenceCompareTo<byte>(PubCacheProtobufDocumentFormattingHelper.CorrectFileSignature.Span) != 0)
        throw new InvalidOperationException("Public repo cache file begins with incorrect signature.");
      byte num = buffer[PubCacheProtobufDocumentFormattingHelper.VersionOffset];
      if (num < (byte) 1 || num > (byte) 1)
        throw new InvalidOperationException(string.Format("Public repo cache file format version {0} is outside the supported range [{1} .. {2}].", (object) num, (object) (byte) 1, (object) (byte) 1));
      return PubCacheProtobufDocumentFormattingHelper.ContentOffset;
    }

    public static int CalculateBufferSize(IMessage doc) => PubCacheProtobufDocumentFormattingHelper.FileHeaderLength + doc.CalculateSize();

    public static int WriteHeader(byte[] buffer)
    {
      PubCacheProtobufDocumentFormattingHelper.CorrectFileSignature.CopyTo(buffer.AsMemory<byte>(PubCacheProtobufDocumentFormattingHelper.SignatureOffset, PubCacheProtobufDocumentFormattingHelper.SignatureLength));
      buffer[PubCacheProtobufDocumentFormattingHelper.VersionOffset] = (byte) 1;
      return PubCacheProtobufDocumentFormattingHelper.ContentOffset;
    }
  }
}
