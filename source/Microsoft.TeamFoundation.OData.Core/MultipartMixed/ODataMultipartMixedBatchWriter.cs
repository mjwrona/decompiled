// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.MultipartMixed.ODataMultipartMixedBatchWriter
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;

namespace Microsoft.OData.MultipartMixed
{
  internal sealed class ODataMultipartMixedBatchWriter : ODataBatchWriter
  {
    private readonly string batchBoundary;
    private readonly DependsOnIdsTracker dependsOnIdsTracker;
    private string changeSetBoundary;
    private bool batchStartBoundaryWritten;
    private bool changesetStartBoundaryWritten;

    internal ODataMultipartMixedBatchWriter(
      ODataMultipartMixedBatchOutputContext rawOutputContext,
      string batchBoundary)
      : base((ODataOutputContext) rawOutputContext)
    {
      ExceptionUtils.CheckArgumentNotNull<string>(batchBoundary, "batchBoundary is null");
      this.batchBoundary = batchBoundary;
      this.RawOutputContext.InitializeRawValueWriter();
      this.dependsOnIdsTracker = new DependsOnIdsTracker();
    }

    private ODataMultipartMixedBatchOutputContext RawOutputContext => this.OutputContext as ODataMultipartMixedBatchOutputContext;

    private ODataBatchOperationMessage CurrentOperationMessage
    {
      get
      {
        if (this.CurrentOperationRequestMessage != null)
          return this.CurrentOperationRequestMessage.OperationMessage;
        return this.CurrentOperationResponseMessage != null ? this.CurrentOperationResponseMessage.OperationMessage : (ODataBatchOperationMessage) null;
      }
    }

    public override void StreamRequested()
    {
      this.StartBatchOperationContent();
      this.RawOutputContext.FlushBuffers();
      this.DisposeBatchWriterAndSetContentStreamRequestedState();
    }

    public override Task StreamRequestedAsync()
    {
      this.StartBatchOperationContent();
      return this.RawOutputContext.FlushBuffersAsync().FollowOnSuccessWith((Action<Task>) (task => this.DisposeBatchWriterAndSetContentStreamRequestedState()));
    }

    public override void StreamDisposed()
    {
      this.SetState(ODataBatchWriter.BatchWriterState.OperationStreamDisposed);
      this.CurrentOperationRequestMessage = (ODataBatchOperationRequestMessage) null;
      this.CurrentOperationResponseMessage = (ODataBatchOperationResponseMessage) null;
      this.RawOutputContext.InitializeRawValueWriter();
    }

    public override void OnInStreamError()
    {
      this.RawOutputContext.VerifyNotDisposed();
      this.SetState(ODataBatchWriter.BatchWriterState.Error);
      this.RawOutputContext.TextWriter.Flush();
      throw new ODataException(Strings.ODataBatchWriter_CannotWriteInStreamErrorForBatch);
    }

    protected override void FlushSynchronously() => this.RawOutputContext.Flush();

    protected override Task FlushAsynchronously() => this.RawOutputContext.FlushAsync();

    protected override void WriteStartChangesetImplementation(string changeSetId)
    {
      this.WritePendingMessageData(true);
      this.SetState(ODataBatchWriter.BatchWriterState.ChangesetStarted);
      this.changeSetBoundary = ODataMultipartMixedBatchWriterUtils.CreateChangeSetBoundary(this.RawOutputContext.WritingResponse, changeSetId);
      ODataMultipartMixedBatchWriterUtils.WriteStartBoundary(this.RawOutputContext.TextWriter, this.batchBoundary, !this.batchStartBoundaryWritten);
      this.batchStartBoundaryWritten = true;
      ODataMultipartMixedBatchWriterUtils.WriteChangeSetPreamble(this.RawOutputContext.TextWriter, this.changeSetBoundary);
      this.changesetStartBoundaryWritten = false;
      this.dependsOnIdsTracker.ChangeSetStarted();
    }

    protected override IEnumerable<string> GetDependsOnRequestIds(IEnumerable<string> dependsOnIds) => dependsOnIds ?? this.dependsOnIdsTracker.GetDependsOnIds();

    protected override ODataBatchOperationRequestMessage CreateOperationRequestMessageImplementation(
      string method,
      Uri uri,
      string contentId,
      BatchPayloadUriOption payloadUriOption,
      IEnumerable<string> dependsOnIds)
    {
      this.WritePendingMessageData(true);
      ODataBatchOperationRequestMessage messageImplementation = this.BuildOperationRequestMessage(this.RawOutputContext.OutputStream, method, uri, contentId, this.changeSetBoundary, dependsOnIds);
      this.SetState(ODataBatchWriter.BatchWriterState.OperationCreated);
      this.WriteStartBoundaryForOperation();
      if (contentId != null)
        this.dependsOnIdsTracker.AddDependsOnId(contentId);
      ODataMultipartMixedBatchWriterUtils.WriteRequestPreamble(this.RawOutputContext.TextWriter, method, uri, this.RawOutputContext.MessageWriterSettings.BaseUri, this.changeSetBoundary != null, contentId, payloadUriOption);
      return messageImplementation;
    }

    protected override void WriteEndBatchImplementation()
    {
      this.WritePendingMessageData(true);
      this.SetState(ODataBatchWriter.BatchWriterState.BatchCompleted);
      ODataMultipartMixedBatchWriterUtils.WriteEndBoundary(this.RawOutputContext.TextWriter, this.batchBoundary, !this.batchStartBoundaryWritten);
      this.RawOutputContext.TextWriter.WriteLine();
    }

    protected override void WriteEndChangesetImplementation()
    {
      this.WritePendingMessageData(true);
      string changeSetBoundary = this.changeSetBoundary;
      this.SetState(ODataBatchWriter.BatchWriterState.ChangesetCompleted);
      this.dependsOnIdsTracker.ChangeSetEnded();
      this.changeSetBoundary = (string) null;
      ODataMultipartMixedBatchWriterUtils.WriteEndBoundary(this.RawOutputContext.TextWriter, changeSetBoundary, !this.changesetStartBoundaryWritten);
    }

    protected override ODataBatchOperationResponseMessage CreateOperationResponseMessageImplementation(
      string contentId)
    {
      this.WritePendingMessageData(true);
      this.CurrentOperationResponseMessage = this.BuildOperationResponseMessage(this.RawOutputContext.OutputStream, contentId, this.changeSetBoundary);
      this.SetState(ODataBatchWriter.BatchWriterState.OperationCreated);
      this.WriteStartBoundaryForOperation();
      ODataMultipartMixedBatchWriterUtils.WriteResponsePreamble(this.RawOutputContext.TextWriter, this.changeSetBoundary != null, contentId);
      return this.CurrentOperationResponseMessage;
    }

    protected override void VerifyNotDisposed() => this.RawOutputContext.VerifyNotDisposed();

    protected override void WriteStartBatchImplementation() => this.SetState(ODataBatchWriter.BatchWriterState.BatchStarted);

    private void StartBatchOperationContent()
    {
      this.WritePendingMessageData(false);
      this.RawOutputContext.TextWriter.Flush();
    }

    private void DisposeBatchWriterAndSetContentStreamRequestedState()
    {
      this.RawOutputContext.CloseWriter();
      this.SetState(ODataBatchWriter.BatchWriterState.OperationStreamRequested);
    }

    private void WriteStartBoundaryForOperation()
    {
      if (this.changeSetBoundary == null)
      {
        ODataMultipartMixedBatchWriterUtils.WriteStartBoundary(this.RawOutputContext.TextWriter, this.batchBoundary, !this.batchStartBoundaryWritten);
        this.batchStartBoundaryWritten = true;
      }
      else
      {
        ODataMultipartMixedBatchWriterUtils.WriteStartBoundary(this.RawOutputContext.TextWriter, this.changeSetBoundary, !this.changesetStartBoundaryWritten);
        this.changesetStartBoundaryWritten = true;
      }
    }

    private void WritePendingMessageData(bool reportMessageCompleted)
    {
      if (this.CurrentOperationMessage == null)
        return;
      if (this.CurrentOperationResponseMessage != null)
      {
        int statusCode = this.CurrentOperationResponseMessage.StatusCode;
        string statusMessage = HttpUtils.GetStatusMessage(statusCode);
        this.RawOutputContext.TextWriter.WriteLine("{0} {1} {2}", (object) "HTTP/1.1", (object) statusCode, (object) statusMessage);
      }
      IEnumerable<KeyValuePair<string, string>> headers = this.CurrentOperationMessage.Headers;
      if (headers != null)
      {
        foreach (KeyValuePair<string, string> keyValuePair in headers)
          this.RawOutputContext.TextWriter.WriteLine(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}: {1}", new object[2]
          {
            (object) keyValuePair.Key,
            (object) keyValuePair.Value
          }));
      }
      this.RawOutputContext.TextWriter.WriteLine();
      if (!reportMessageCompleted)
        return;
      this.CurrentOperationMessage.PartHeaderProcessingCompleted();
      this.CurrentOperationRequestMessage = (ODataBatchOperationRequestMessage) null;
      this.CurrentOperationResponseMessage = (ODataBatchOperationResponseMessage) null;
    }
  }
}
