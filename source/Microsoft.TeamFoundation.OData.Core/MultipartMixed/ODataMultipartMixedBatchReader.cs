// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.MultipartMixed.ODataMultipartMixedBatchReader
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;

namespace Microsoft.OData.MultipartMixed
{
  internal sealed class ODataMultipartMixedBatchReader : ODataBatchReader
  {
    private readonly ODataMultipartMixedBatchReaderStream batchStream;
    private readonly DependsOnIdsTracker dependsOnIdsTracker;
    private string currentContentId;

    internal ODataMultipartMixedBatchReader(
      ODataMultipartMixedBatchInputContext inputContext,
      string batchBoundary,
      Encoding batchEncoding,
      bool synchronous)
      : base((ODataInputContext) inputContext, synchronous)
    {
      this.batchStream = new ODataMultipartMixedBatchReaderStream(this.MultipartMixedBatchInputContext, batchBoundary, batchEncoding);
      this.dependsOnIdsTracker = new DependsOnIdsTracker();
    }

    private ODataMultipartMixedBatchInputContext MultipartMixedBatchInputContext => this.InputContext as ODataMultipartMixedBatchInputContext;

    protected override ODataBatchOperationRequestMessage CreateOperationRequestMessageImplementation()
    {
      string httpMethod;
      Uri requestUri;
      this.ParseRequestLine(this.batchStream.ReadFirstNonEmptyLine(), out httpMethod, out requestUri);
      ODataBatchOperationHeaders headers = this.batchStream.ReadHeaders();
      if (this.batchStream.ChangeSetBoundary != null)
      {
        if (this.currentContentId == null)
        {
          headers.TryGetValue("Content-ID", out this.currentContentId);
          if (this.currentContentId == null)
            throw new ODataException(Strings.ODataBatchOperationHeaderDictionary_KeyNotFound((object) "Content-ID"));
        }
      }
      else
      {
        ODataVersion? version = this.InputContext.MessageReaderSettings.Version;
        ODataVersion odataVersion = ODataVersion.V4;
        if (version.GetValueOrDefault() <= odataVersion & version.HasValue)
          this.PayloadUriConverter.Reset();
      }
      ODataBatchOperationRequestMessage messageImplementation = this.BuildOperationRequestMessage((Func<Stream>) (() => (Stream) ODataBatchUtils.CreateBatchOperationReadStream((ODataBatchReaderStream) this.batchStream, headers, (IODataStreamListener) this)), httpMethod, requestUri, headers, this.currentContentId, this.batchStream.ChangeSetBoundary, this.dependsOnIdsTracker.GetDependsOnIds(), false);
      if (this.currentContentId != null)
        this.dependsOnIdsTracker.AddDependsOnId(this.currentContentId);
      this.currentContentId = (string) null;
      return messageImplementation;
    }

    protected override ODataBatchOperationResponseMessage CreateOperationResponseMessageImplementation()
    {
      int responseLine = this.ParseResponseLine(this.batchStream.ReadFirstNonEmptyLine());
      ODataBatchOperationHeaders headers = this.batchStream.ReadHeaders();
      if (this.currentContentId == null)
        headers.TryGetValue("Content-ID", out this.currentContentId);
      ODataBatchOperationResponseMessage messageImplementation = this.BuildOperationResponseMessage((Func<Stream>) (() => (Stream) ODataBatchUtils.CreateBatchOperationReadStream((ODataBatchReaderStream) this.batchStream, headers, (IODataStreamListener) this)), responseLine, headers, this.currentContentId, (string) null);
      this.currentContentId = (string) null;
      return messageImplementation;
    }

    protected override ODataBatchReaderState ReadAtStartImplementation() => this.SkipToNextPartAndReadHeaders();

    protected override ODataBatchReaderState ReadAtOperationImplementation() => this.SkipToNextPartAndReadHeaders();

    protected override ODataBatchReaderState ReadAtChangesetStartImplementation()
    {
      if (this.batchStream.ChangeSetBoundary == null)
        this.ThrowODataException(Strings.ODataBatchReader_ReaderStreamChangesetBoundaryCannotBeNull);
      this.dependsOnIdsTracker.ChangeSetStarted();
      return this.SkipToNextPartAndReadHeaders();
    }

    protected override ODataBatchReaderState ReadAtChangesetEndImplementation()
    {
      this.batchStream.ResetChangeSetBoundary();
      this.dependsOnIdsTracker.ChangeSetEnded();
      return this.SkipToNextPartAndReadHeaders();
    }

    private ODataBatchReaderState GetEndBoundaryState()
    {
      switch (this.State)
      {
        case ODataBatchReaderState.Initial:
          return ODataBatchReaderState.Completed;
        case ODataBatchReaderState.Operation:
          return this.batchStream.ChangeSetBoundary != null ? ODataBatchReaderState.ChangesetEnd : ODataBatchReaderState.Completed;
        case ODataBatchReaderState.ChangesetStart:
          return ODataBatchReaderState.ChangesetEnd;
        case ODataBatchReaderState.ChangesetEnd:
          return ODataBatchReaderState.Completed;
        case ODataBatchReaderState.Completed:
          throw new ODataException(Strings.General_InternalError((object) InternalErrorCodes.ODataBatchReader_GetEndBoundary_Completed));
        case ODataBatchReaderState.Exception:
          throw new ODataException(Strings.General_InternalError((object) InternalErrorCodes.ODataBatchReader_GetEndBoundary_Exception));
        default:
          throw new ODataException(Strings.General_InternalError((object) InternalErrorCodes.ODataBatchReader_GetEndBoundary_UnknownValue));
      }
    }

    private void ParseRequestLine(string requestLine, out string httpMethod, out Uri requestUri)
    {
      int length = requestLine.IndexOf(' ');
      if (length <= 0 || requestLine.Length - 3 <= length)
        throw new ODataException(Strings.ODataBatchReaderStream_InvalidRequestLine((object) requestLine));
      int num = requestLine.LastIndexOf(' ');
      if (num < 0 || num - length - 1 <= 0 || requestLine.Length - 1 <= num)
        throw new ODataException(Strings.ODataBatchReaderStream_InvalidRequestLine((object) requestLine));
      httpMethod = requestLine.Substring(0, length);
      string uriString = requestLine.Substring(length + 1, num - length - 1);
      string str = requestLine.Substring(num + 1);
      if (string.CompareOrdinal("HTTP/1.1", str) != 0)
        throw new ODataException(Strings.ODataBatchReaderStream_InvalidHttpVersionSpecified((object) str, (object) "HTTP/1.1"));
      HttpUtils.ValidateHttpMethod(httpMethod);
      if (this.batchStream.ChangeSetBoundary != null && HttpUtils.IsQueryMethod(httpMethod))
        throw new ODataException(Strings.ODataBatch_InvalidHttpMethodForChangeSetRequest((object) httpMethod));
      requestUri = new Uri(uriString, UriKind.RelativeOrAbsolute);
    }

    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "'this' is used when built in debug")]
    private int ParseResponseLine(string responseLine)
    {
      int length = responseLine.IndexOf(' ');
      if (length <= 0 || responseLine.Length - 3 <= length)
        throw new ODataException(Strings.ODataBatchReaderStream_InvalidResponseLine((object) responseLine));
      int num = responseLine.IndexOf(' ', length + 1);
      if (num < 0 || num - length - 1 <= 0 || responseLine.Length - 1 <= num)
        throw new ODataException(Strings.ODataBatchReaderStream_InvalidResponseLine((object) responseLine));
      string str1 = responseLine.Substring(0, length);
      string str2 = responseLine.Substring(length + 1, num - length - 1);
      if (string.CompareOrdinal("HTTP/1.1", str1) != 0)
        throw new ODataException(Strings.ODataBatchReaderStream_InvalidHttpVersionSpecified((object) str1, (object) "HTTP/1.1"));
      int result;
      if (!int.TryParse(str2, out result))
        throw new ODataException(Strings.ODataBatchReaderStream_NonIntegerHttpStatusCode((object) str2));
      return result;
    }

    private ODataBatchReaderState SkipToNextPartAndReadHeaders()
    {
      bool isEndBoundary;
      bool isParentBoundary;
      if (!this.batchStream.SkipToBoundary(out isEndBoundary, out isParentBoundary))
        return this.batchStream.ChangeSetBoundary == null ? ODataBatchReaderState.Completed : ODataBatchReaderState.ChangesetEnd;
      ODataBatchReaderState partAndReadHeaders;
      if (isEndBoundary | isParentBoundary)
      {
        partAndReadHeaders = this.GetEndBoundaryState();
      }
      else
      {
        bool flag1 = this.batchStream.ChangeSetBoundary != null;
        bool flag2 = this.batchStream.ProcessPartHeader(out this.currentContentId);
        partAndReadHeaders = !flag1 ? (flag2 ? ODataBatchReaderState.ChangesetStart : ODataBatchReaderState.Operation) : ODataBatchReaderState.Operation;
      }
      return partAndReadHeaders;
    }
  }
}
