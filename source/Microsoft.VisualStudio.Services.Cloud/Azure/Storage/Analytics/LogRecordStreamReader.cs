// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Storage.Analytics.LogRecordStreamReader
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using System;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;

namespace Microsoft.Azure.Storage.Analytics
{
  internal class LogRecordStreamReader : IDisposable
  {
    public const char FieldDelimiter = ';';
    public const char RecordDelimiter = '\n';
    public const char QuoteChar = '"';
    private Encoding encoding;
    private StreamReader reader;
    private long position;
    private bool isFirstFieldInRecord;

    public LogRecordStreamReader(Stream stream, int bufferSize)
    {
      this.encoding = (Encoding) new UTF8Encoding(false);
      this.reader = new StreamReader(stream, this.encoding, false, bufferSize);
      this.position = 0L;
      this.isFirstFieldInRecord = true;
    }

    public bool IsEndOfFile => this.reader.EndOfStream;

    public long Position => this.position;

    public bool HasMoreFieldsInRecord() => this.TryPeekDelimiter(';');

    public string ReadString()
    {
      string str = this.ReadField(false);
      return string.IsNullOrEmpty(str) ? (string) null : str;
    }

    public string ReadQuotedString()
    {
      string str = this.ReadField(true);
      return string.IsNullOrEmpty(str) ? (string) null : str;
    }

    public void EndCurrentRecord()
    {
      this.ReadDelimiter('\n');
      this.isFirstFieldInRecord = true;
    }

    public bool? ReadBool()
    {
      string str = this.ReadField(false);
      return string.IsNullOrEmpty(str) ? new bool?() : new bool?(bool.Parse(str));
    }

    public DateTimeOffset? ReadDateTimeOffset(string format)
    {
      string input = this.ReadField(false);
      if (string.IsNullOrEmpty(input))
        return new DateTimeOffset?();
      DateTimeOffset result;
      if (DateTimeOffset.TryParseExact(input, format, (IFormatProvider) CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal, out result))
        return new DateTimeOffset?(result);
      throw new InvalidOperationException("Error parsing log record: could not parse '" + input + "' using format: " + format);
    }

    public TimeSpan? ReadTimeSpanInMS()
    {
      string s = this.ReadField(false);
      return string.IsNullOrEmpty(s) ? new TimeSpan?() : new TimeSpan?(new TimeSpan(0, 0, 0, 0, int.Parse(s, NumberStyles.None, (IFormatProvider) CultureInfo.InvariantCulture)));
    }

    public double? ReadDouble()
    {
      string s = this.ReadField(false);
      return string.IsNullOrEmpty(s) ? new double?() : new double?(double.Parse(s, NumberStyles.AllowDecimalPoint, (IFormatProvider) CultureInfo.InvariantCulture));
    }

    public Guid? ReadGuid()
    {
      string input = this.ReadField(false);
      return string.IsNullOrEmpty(input) ? new Guid?() : new Guid?(Guid.ParseExact(input, "D"));
    }

    public int? ReadInt()
    {
      string s = this.ReadField(false);
      return string.IsNullOrEmpty(s) ? new int?() : new int?(int.Parse(s, NumberStyles.None, (IFormatProvider) CultureInfo.InvariantCulture));
    }

    public long? ReadLong()
    {
      string s = this.ReadField(false);
      return string.IsNullOrEmpty(s) ? new long?() : new long?(long.Parse(s, NumberStyles.None, (IFormatProvider) CultureInfo.InvariantCulture));
    }

    public Uri ReadUri()
    {
      string str = this.ReadField(true);
      return string.IsNullOrEmpty(str) ? (Uri) null : new Uri(WebUtility.HtmlDecode(str));
    }

    private void ReadDelimiter(char delimiterToRead)
    {
      this.EnsureNotEndOfFile();
      long position = this.position;
      int num = this.reader.Read();
      if (num == -1 || (int) (ushort) num != (int) delimiterToRead)
        throw new InvalidOperationException(string.Format("Error parsing log record: expected the delimiter '{0}', but read '{1}' at position '{2}'.", (object) delimiterToRead, (object) (char) num, (object) position));
      ++this.position;
    }

    private bool TryPeekDelimiter(char delimiterToRead)
    {
      this.EnsureNotEndOfFile();
      int num = this.reader.Peek();
      return num != -1 && (int) (ushort) num == (int) delimiterToRead;
    }

    private void EnsureNotEndOfFile()
    {
      if (this.IsEndOfFile)
        throw new EndOfStreamException(string.Format("Error parsing log record: unexpected end of stream at position '{0}'.", (object) this.Position));
    }

    private string ReadField(bool isQuotedString)
    {
      if (!this.isFirstFieldInRecord)
        this.ReadDelimiter(';');
      else
        this.isFirstFieldInRecord = false;
      StringBuilder stringBuilder = new StringBuilder();
      bool flag1 = false;
      bool flag2 = false;
      while (true)
      {
        char ch;
        do
        {
          this.EnsureNotEndOfFile();
          ch = (char) this.reader.Peek();
          if (!(!isQuotedString | flag2) && stringBuilder.Length != 0 || ch != ';' && ch != '\n')
          {
            if (flag2)
              throw new InvalidOperationException(string.Format("Error parsing log record: unexpected quote character found. String so far: '{0}'. Character read: '{1}'", (object) stringBuilder.ToString(), (object) ch));
            this.reader.Read();
            stringBuilder.Append(ch);
            ++this.position;
          }
          else
            goto label_15;
        }
        while (ch != '"');
        if (isQuotedString)
        {
          if (stringBuilder.Length == 1)
            flag1 = true;
          else if (flag1)
            flag2 = true;
          else
            goto label_14;
        }
        else
          break;
      }
      throw new InvalidOperationException(string.Format("Error parsing log record: unexpected quote character found. String so far: '{0}'. Character read: '{1}'", (object) stringBuilder.ToString(), (object) '"'));
label_14:
      throw new InvalidOperationException(string.Format("Error parsing log record: unexpected quote character found. String so far: '{0}'. Character read: '{1}'", (object) stringBuilder.ToString(), (object) '"'));
label_15:
      return !isQuotedString || stringBuilder.Length == 0 ? stringBuilder.ToString() : stringBuilder.ToString(1, stringBuilder.Length - 2);
    }

    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize((object) this);
    }

    protected virtual void Dispose(bool disposing)
    {
      if (!disposing)
        return;
      this.reader.Close();
    }

    ~LogRecordStreamReader() => this.Dispose(false);
  }
}
