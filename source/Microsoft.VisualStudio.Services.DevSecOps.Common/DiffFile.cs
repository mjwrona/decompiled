// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.DevSecOps.Common.DiffFile
// Assembly: Microsoft.VisualStudio.Services.DevSecOps.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 072F1303-F456-426E-A1CB-C0838641751B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.DevSecOps.Common.dll

using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Microsoft.VisualStudio.Services.DevSecOps.Common
{
  public class DiffFile : List<DiffLine>
  {
    private Encoding m_encoding;
    private const int AverageLinesInSequence = 1024;

    private DiffFile(int capacity, Encoding encoding, bool containsPreamble)
      : base(capacity)
    {
      this.m_encoding = encoding;
      this.HasPreamble = this.m_encoding.GetPreamble().Length != 0 & containsPreamble;
    }

    public bool HasPreamble { get; set; }

    public Encoding Encoding => this.m_encoding;

    public static DiffFile Create(string filePath, Encoding fileEncoding, DiffOptions diffOptions)
    {
      using (FileStream fileStream = File.OpenRead(filePath))
        return DiffFile.Create((Stream) fileStream, fileEncoding, diffOptions);
    }

    public static DiffFile Create(Stream stream, Encoding encoding, DiffOptions diffOptions)
    {
      bool containsPreamble = false;
      if (stream.CanSeek)
      {
        long position = stream.Position;
        encoding = FileTypeUtil.DetermineEncoding(stream, true, encoding, (diffOptions.Flags & DiffOptionFlags.ScanFullFileForEncodingDetection) != 0, out containsPreamble) ?? encoding;
        stream.Position = position;
      }
      Encoding encoding1 = encoding;
      if ((diffOptions.Flags & DiffOptionFlags.ThrowIfDetectedEncodingMismatch) != DiffOptionFlags.None)
      {
        encoding1 = (Encoding) encoding1.Clone();
        encoding1.DecoderFallback = DecoderFallback.ExceptionFallback;
      }
      DiffFile diffFile = new DiffFile(1024, encoding1, containsPreamble);
      DiffLineTokenizer diffLineTokenizer = new DiffLineTokenizer(stream, encoding1);
      EndOfLineTerminator endOfLine;
      for (string content = diffLineTokenizer.NextLineToken(out endOfLine); content != null; content = diffLineTokenizer.NextLineToken(out endOfLine))
        diffFile.Add(new DiffLine(content, endOfLine, diffOptions));
      return diffFile;
    }
  }
}
