// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Diff.DiffLineTokenizer
// Assembly: Microsoft.TeamFoundation.Diff, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F647AACF-6EF1-4C0C-AB27-20317A054A39
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Diff.dll

using System.IO;
using System.Text;

namespace Microsoft.TeamFoundation.Diff
{
  public class DiffLineTokenizer
  {
    private const int LineSeparator = 8232;
    private const int ParagraphSeparator = 8233;
    private const int NextLine = 133;
    private bool m_isUnicodeEncoding;
    private int m_bufferSize;
    private int m_numCharsInBuffer;
    private int m_currBufferPos;
    private char[] m_charBuffer;
    private StringBuilder m_stringBuilder;
    private StreamReader m_streamReader;

    public DiffLineTokenizer(Stream stream, Encoding encoding)
    {
      this.m_streamReader = encoding != null ? new StreamReader(stream, encoding, true) : new StreamReader(stream, true);
      int codePage = this.m_streamReader.CurrentEncoding.CodePage;
      if (codePage == Encoding.Unicode.CodePage || codePage == Encoding.BigEndianUnicode.CodePage || codePage == Encoding.UTF32.CodePage || codePage == Encoding.UTF7.CodePage || codePage == Encoding.UTF8.CodePage)
        this.m_isUnicodeEncoding = true;
      this.m_bufferSize = this.m_streamReader.CurrentEncoding.GetMaxCharCount(1024);
      this.m_charBuffer = new char[this.m_bufferSize];
      this.m_stringBuilder = new StringBuilder(80);
    }

    private int FillBuffer()
    {
      this.m_currBufferPos = 0;
      this.m_numCharsInBuffer = this.m_streamReader.Read(this.m_charBuffer, 0, this.m_bufferSize);
      return this.m_numCharsInBuffer;
    }

    public string NextLineToken(out EndOfLineTerminator endOfLine)
    {
      this.m_stringBuilder.Length = 0;
      endOfLine = EndOfLineTerminator.None;
      if (this.m_currBufferPos == this.m_numCharsInBuffer && this.FillBuffer() == 0)
        return (string) null;
      do
      {
        int currBufferPos = this.m_currBufferPos;
        do
        {
          char ch = this.m_charBuffer[currBufferPos];
          switch (ch)
          {
            case '\n':
            case '\r':
              switch (ch)
              {
                case '\n':
                  endOfLine = EndOfLineTerminator.LineFeed;
                  break;
                case '\r':
                  endOfLine = EndOfLineTerminator.CarriageReturn;
                  break;
                case '\u0085':
                  endOfLine = EndOfLineTerminator.NextLine;
                  break;
                case '\u2028':
                  endOfLine = EndOfLineTerminator.LineSeparator;
                  break;
                case '\u2029':
                  endOfLine = EndOfLineTerminator.ParagraphSeparator;
                  break;
              }
              string str;
              if (this.m_stringBuilder.Length > 0)
              {
                this.m_stringBuilder.Append(this.m_charBuffer, this.m_currBufferPos, currBufferPos - this.m_currBufferPos);
                str = this.m_stringBuilder.ToString();
              }
              else
                str = new string(this.m_charBuffer, this.m_currBufferPos, currBufferPos - this.m_currBufferPos);
              this.m_currBufferPos = currBufferPos + 1;
              if (ch == '\r' && (this.m_currBufferPos < this.m_numCharsInBuffer || this.FillBuffer() > 0) && this.m_charBuffer[this.m_currBufferPos] == '\n')
              {
                endOfLine = EndOfLineTerminator.CarriageReturnLineFeed;
                ++this.m_currBufferPos;
              }
              return str;
            default:
              if (!this.m_isUnicodeEncoding || ch != '\u2028' && ch != '\u2029' && ch != '\u0085')
              {
                ++currBufferPos;
                continue;
              }
              goto case '\n';
          }
        }
        while (currBufferPos < this.m_numCharsInBuffer);
        this.m_stringBuilder.Append(this.m_charBuffer, this.m_currBufferPos, this.m_numCharsInBuffer - this.m_currBufferPos);
      }
      while (this.FillBuffer() > 0);
      return this.m_stringBuilder.ToString();
    }
  }
}
