// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Common.Utils.CrawlerHelpers
// Assembly: Microsoft.VisualStudio.Services.Search.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8E09DCBA-148E-4EB7-BB73-B53B030BE93E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Common.dll

using Microsoft.VisualStudio.Services.Search.WebApi;
using System;
using System.IO;
using System.Text;

namespace Microsoft.VisualStudio.Services.Search.Common.Utils
{
  public static class CrawlerHelpers
  {
    private const int BufferLengthInBytes = 16384;
    public static readonly StringComparer GitPathStringComparer = StringComparer.Ordinal;
    internal static StringComparison GitPathStringComparison = StringComparison.Ordinal;

    internal static int GitPathStringCompare(string firstString, string secondString) => string.CompareOrdinal(firstString, secondString);

    public static byte[] GetDummyByteContent() => new byte[1];

    public static string GetDummyContent(FileTypeEnum fileType)
    {
      switch (fileType)
      {
        case FileTypeEnum.LARGE:
          return "SD-Indexing : File Size is too large.";
        case FileTypeEnum.UNSUPPORTED_EXTENSION:
          return "SD-Indexing : Unsupported extension.";
        case FileTypeEnum.BINARY:
          return "SD-Indexing : Detected as Binary.";
        case FileTypeEnum.REGULAR:
          throw new NotSupportedException("Generating Dummy content for Regular Files not supported");
        case FileTypeEnum.INVALID_FILE_DETAILS:
          return "SD-Crawler : Invalid File details";
        case FileTypeEnum.OTHER:
          return "SD-Indexing : Something went wrong while indexing";
        default:
          throw new NotImplementedException("Unrecognized type of file");
      }
    }

    public static byte[] GetByteContentForNonBinaryStream(
      Stream blobStream,
      out bool isBinaryContent,
      out bool isFileSizeSupported,
      long maxBytesToRead,
      bool dummyConentForBinary = true)
    {
      if (blobStream == null)
        throw new ArgumentNullException(nameof (blobStream));
      byte[] buffer = new byte[16384];
      int num = 0;
      isBinaryContent = false;
      isFileSizeSupported = true;
      using (MemoryStream memoryStream = new MemoryStream())
      {
        int count;
        while ((count = blobStream.Read(buffer, 0, buffer.Length)) > 0)
        {
          memoryStream.Write(buffer, 0, count);
          num += count;
          if ((long) num > maxBytesToRead)
          {
            isFileSizeSupported = false;
            return CrawlerHelpers.GetDummyByteContent();
          }
        }
        memoryStream.Seek(0L, SeekOrigin.Begin);
        if (FileTypeUtil.DetermineEncoding((Stream) memoryStream, true, Encoding.Default, maxBytesToRead, out bool _) == null)
          isBinaryContent = true;
        if (isBinaryContent & dummyConentForBinary)
          return CrawlerHelpers.GetDummyByteContent();
        memoryStream.Seek(0L, SeekOrigin.Begin);
        return memoryStream.ToArray();
      }
    }
  }
}
