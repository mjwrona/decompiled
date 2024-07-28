// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.ODataAsynchronousReader
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.OData
{
  public sealed class ODataAsynchronousReader
  {
    private readonly ODataRawInputContext rawInputContext;
    private readonly IServiceProvider container;

    internal ODataAsynchronousReader(ODataRawInputContext rawInputContext, Encoding encoding)
    {
      if (encoding != null)
        ReaderValidationUtils.ValidateEncodingSupportedInAsync(encoding);
      this.rawInputContext = rawInputContext;
      this.container = rawInputContext.Container;
    }

    public ODataAsynchronousResponseMessage CreateResponseMessage()
    {
      this.VerifyCanCreateResponseMessage(true);
      return this.CreateResponseMessageImplementation();
    }

    public Task<ODataAsynchronousResponseMessage> CreateResponseMessageAsync()
    {
      this.VerifyCanCreateResponseMessage(false);
      return TaskUtils.GetTaskForSynchronousOperation<ODataAsynchronousResponseMessage>((Func<ODataAsynchronousResponseMessage>) (() => this.CreateResponseMessageImplementation()));
    }

    private void ValidateReaderNotDisposed() => this.rawInputContext.VerifyNotDisposed();

    private void VerifyCallAllowed(bool synchronousCall)
    {
      if (synchronousCall)
      {
        if (!this.rawInputContext.Synchronous)
          throw new ODataException(Strings.ODataAsyncReader_SyncCallOnAsyncReader);
      }
      else if (this.rawInputContext.Synchronous)
        throw new ODataException(Strings.ODataAsyncReader_AsyncCallOnSyncReader);
    }

    private void VerifyCanCreateResponseMessage(bool synchronousCall)
    {
      this.ValidateReaderNotDisposed();
      this.VerifyCallAllowed(synchronousCall);
      if (!this.rawInputContext.ReadingResponse)
        throw new ODataException(Strings.ODataAsyncReader_CannotCreateResponseWhenNotReadingResponse);
    }

    private ODataAsynchronousResponseMessage CreateResponseMessageImplementation()
    {
      int statusCode;
      IDictionary<string, string> headers;
      this.ReadInnerEnvelope(out statusCode, out headers);
      return ODataAsynchronousResponseMessage.CreateMessageForReading(this.rawInputContext.Stream, statusCode, headers, this.container);
    }

    private void ReadInnerEnvelope(out int statusCode, out IDictionary<string, string> headers)
    {
      string responseLine = this.ReadFirstNonEmptyLine();
      statusCode = ODataAsynchronousReader.ParseResponseLine(responseLine);
      headers = this.ReadHeaders();
    }

    private string ReadFirstNonEmptyLine()
    {
      string str = this.ReadLine();
      while (str.Length == 0)
        str = this.ReadLine();
      return str;
    }

    private static int ParseResponseLine(string responseLine)
    {
      int length = responseLine.Length != 0 ? responseLine.IndexOf(' ') : throw new ODataException(Strings.ODataAsyncReader_InvalidResponseLine((object) responseLine));
      if (length <= 0 || responseLine.Length - 3 <= length)
        throw new ODataException(Strings.ODataAsyncReader_InvalidResponseLine((object) responseLine));
      int num = responseLine.IndexOf(' ', length + 1);
      if (num < 0 || num - length - 1 <= 0 || responseLine.Length - 1 <= num)
        throw new ODataException(Strings.ODataAsyncReader_InvalidResponseLine((object) responseLine));
      string str1 = responseLine.Substring(0, length);
      string str2 = responseLine.Substring(length + 1, num - length - 1);
      if (string.CompareOrdinal("HTTP/1.1", str1) != 0)
        throw new ODataException(Strings.ODataAsyncReader_InvalidHttpVersionSpecified((object) str1, (object) "HTTP/1.1"));
      int result;
      if (!int.TryParse(str2, out result))
        throw new ODataException(Strings.ODataAsyncReader_NonIntegerHttpStatusCode((object) str2));
      return result;
    }

    private IDictionary<string, string> ReadHeaders()
    {
      Dictionary<string, string> dictionary = new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.Ordinal);
      for (string headerLine = this.ReadLine(); !string.IsNullOrEmpty(headerLine); headerLine = this.ReadLine())
      {
        string headerName;
        string headerValue;
        ODataAsynchronousReader.ValidateHeaderLine(headerLine, out headerName, out headerValue);
        if (dictionary.ContainsKey(headerName))
          throw new ODataException(Strings.ODataAsyncReader_DuplicateHeaderFound((object) headerName));
        dictionary.Add(headerName, headerValue);
      }
      return (IDictionary<string, string>) dictionary;
    }

    private static void ValidateHeaderLine(
      string headerLine,
      out string headerName,
      out string headerValue)
    {
      int length = headerLine.IndexOf(':');
      headerName = length > 0 ? headerLine.Substring(0, length).Trim() : throw new ODataException(Strings.ODataAsyncReader_InvalidHeaderSpecified((object) headerLine));
      headerValue = headerLine.Substring(length + 1).Trim();
    }

    private string ReadLine()
    {
      StringBuilder stringBuilder = new StringBuilder();
      for (int index = this.ReadByte(); index != -1; index = this.ReadByte())
      {
        if (index == 10)
          throw new ODataException(Strings.ODataAsyncReader_InvalidNewLineEncountered((object) '\n'));
        if (index == 13)
        {
          if (this.ReadByte() != 10)
            throw new ODataException(Strings.ODataAsyncReader_InvalidNewLineEncountered((object) '\r'));
          return stringBuilder.ToString();
        }
        stringBuilder.Append((char) index);
      }
      throw new ODataException(Strings.ODataAsyncReader_UnexpectedEndOfInput);
    }

    private int ReadByte() => this.rawInputContext.Stream.ReadByte();
  }
}
