// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.ODataBatchWriter
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading.Tasks;

namespace Microsoft.OData
{
  public abstract class ODataBatchWriter : IODataStreamListener, IODataOutputInStreamErrorListener
  {
    private readonly ODataOutputContext outputContext;
    private readonly ODataBatchPayloadUriConverter payloadUriConverter;
    private readonly IServiceProvider container;
    private ODataBatchWriter.BatchWriterState state;
    private ODataBatchOperationRequestMessage currentOperationRequestMessage;
    private ODataBatchOperationResponseMessage currentOperationResponseMessage;
    private string currentOperationContentId;
    private uint currentBatchSize;
    private uint currentChangeSetSize;
    private bool isInChangset;

    internal ODataBatchWriter(ODataOutputContext outputContext)
    {
      this.outputContext = outputContext;
      this.container = outputContext.Container;
      this.payloadUriConverter = new ODataBatchPayloadUriConverter(outputContext.PayloadUriConverter);
    }

    protected ODataBatchOperationRequestMessage CurrentOperationRequestMessage
    {
      get => this.currentOperationRequestMessage;
      set => this.currentOperationRequestMessage = value;
    }

    protected ODataBatchOperationResponseMessage CurrentOperationResponseMessage
    {
      get => this.currentOperationResponseMessage;
      set => this.currentOperationResponseMessage = value;
    }

    protected ODataOutputContext OutputContext => this.outputContext;

    public void WriteStartBatch()
    {
      this.VerifyCanWriteStartBatch(true);
      this.WriteStartBatchImplementation();
    }

    public Task WriteStartBatchAsync()
    {
      this.VerifyCanWriteStartBatch(false);
      return TaskUtils.GetTaskForSynchronousOperation(new Action(this.WriteStartBatchImplementation));
    }

    public void WriteEndBatch()
    {
      this.VerifyCanWriteEndBatch(true);
      this.WriteEndBatchImplementation();
      this.Flush();
    }

    public Task WriteEndBatchAsync()
    {
      this.VerifyCanWriteEndBatch(false);
      return TaskUtils.GetTaskForSynchronousOperation(new Action(this.WriteEndBatchImplementation)).FollowOnSuccessWithTask((Func<Task, Task>) (task => this.FlushAsync()));
    }

    public void WriteStartChangeset() => this.WriteStartChangeset(Guid.NewGuid().ToString());

    public void WriteStartChangeset(string changesetId)
    {
      ExceptionUtils.CheckArgumentNotNull<string>(changesetId, nameof (changesetId));
      this.VerifyCanWriteStartChangeset(true);
      this.WriteStartChangesetImplementation(changesetId);
      this.FinishWriteStartChangeset();
    }

    public Task WriteStartChangesetAsync() => this.WriteStartChangesetAsync(Guid.NewGuid().ToString());

    public Task WriteStartChangesetAsync(string changesetId)
    {
      ExceptionUtils.CheckArgumentNotNull<string>(changesetId, nameof (changesetId));
      this.VerifyCanWriteStartChangeset(false);
      return TaskUtils.GetTaskForSynchronousOperation((Action) (() => this.WriteStartChangesetImplementation(changesetId))).FollowOnSuccessWith((Action<Task>) (t => this.FinishWriteStartChangeset()));
    }

    public void WriteEndChangeset()
    {
      this.VerifyCanWriteEndChangeset(true);
      this.WriteEndChangesetImplementation();
      this.FinishWriteEndChangeset();
    }

    public Task WriteEndChangesetAsync()
    {
      this.VerifyCanWriteEndChangeset(false);
      return TaskUtils.GetTaskForSynchronousOperation(new Action(this.WriteEndChangesetImplementation)).FollowOnSuccessWith((Action<Task>) (t => this.FinishWriteEndChangeset()));
    }

    public ODataBatchOperationRequestMessage CreateOperationRequestMessage(
      string method,
      Uri uri,
      string contentId)
    {
      return this.CreateOperationRequestMessage(method, uri, contentId, BatchPayloadUriOption.AbsoluteUri);
    }

    public ODataBatchOperationRequestMessage CreateOperationRequestMessage(
      string method,
      Uri uri,
      string contentId,
      BatchPayloadUriOption payloadUriOption)
    {
      return this.CreateOperationRequestMessage(method, uri, contentId, payloadUriOption, (IEnumerable<string>) null);
    }

    public ODataBatchOperationRequestMessage CreateOperationRequestMessage(
      string method,
      Uri uri,
      string contentId,
      BatchPayloadUriOption payloadUriOption,
      IEnumerable<string> dependsOnIds)
    {
      this.VerifyCanCreateOperationRequestMessage(true, method, uri, contentId);
      return this.CreateOperationRequestMessageInternal(method, uri, contentId, payloadUriOption, dependsOnIds);
    }

    public Task<ODataBatchOperationRequestMessage> CreateOperationRequestMessageAsync(
      string method,
      Uri uri,
      string contentId)
    {
      return this.CreateOperationRequestMessageAsync(method, uri, contentId, BatchPayloadUriOption.AbsoluteUri);
    }

    public Task<ODataBatchOperationRequestMessage> CreateOperationRequestMessageAsync(
      string method,
      Uri uri,
      string contentId,
      BatchPayloadUriOption payloadUriOption)
    {
      return this.CreateOperationRequestMessageAsync(method, uri, contentId, payloadUriOption, (IList<string>) null);
    }

    public Task<ODataBatchOperationRequestMessage> CreateOperationRequestMessageAsync(
      string method,
      Uri uri,
      string contentId,
      BatchPayloadUriOption payloadUriOption,
      IList<string> dependsOnIds)
    {
      this.VerifyCanCreateOperationRequestMessage(false, method, uri, contentId);
      return TaskUtils.GetTaskForSynchronousOperation<ODataBatchOperationRequestMessage>((Func<ODataBatchOperationRequestMessage>) (() => this.CreateOperationRequestMessageInternal(method, uri, contentId, payloadUriOption, (IEnumerable<string>) dependsOnIds)));
    }

    public ODataBatchOperationResponseMessage CreateOperationResponseMessage(string contentId)
    {
      this.VerifyCanCreateOperationResponseMessage(true);
      return this.CreateOperationResponseMessageImplementation(contentId);
    }

    public Task<ODataBatchOperationResponseMessage> CreateOperationResponseMessageAsync(
      string contentId)
    {
      this.VerifyCanCreateOperationResponseMessage(false);
      return TaskUtils.GetTaskForSynchronousOperation<ODataBatchOperationResponseMessage>((Func<ODataBatchOperationResponseMessage>) (() => this.CreateOperationResponseMessageImplementation(contentId)));
    }

    public void Flush()
    {
      this.VerifyCanFlush(true);
      try
      {
        this.FlushSynchronously();
      }
      catch
      {
        this.SetState(ODataBatchWriter.BatchWriterState.Error);
        throw;
      }
    }

    public Task FlushAsync()
    {
      this.VerifyCanFlush(false);
      return this.FlushAsynchronously().FollowOnFaultWith((Action<Task>) (t => this.SetState(ODataBatchWriter.BatchWriterState.Error)));
    }

    public abstract void StreamRequested();

    public abstract Task StreamRequestedAsync();

    public abstract void StreamDisposed();

    public abstract void OnInStreamError();

    protected abstract void FlushSynchronously();

    protected abstract Task FlushAsynchronously();

    protected abstract void WriteEndBatchImplementation();

    protected abstract void WriteStartChangesetImplementation(string groupOrChangesetId);

    protected abstract void WriteEndChangesetImplementation();

    protected abstract ODataBatchOperationResponseMessage CreateOperationResponseMessageImplementation(
      string contentId);

    protected abstract ODataBatchOperationRequestMessage CreateOperationRequestMessageImplementation(
      string method,
      Uri uri,
      string contentId,
      BatchPayloadUriOption payloadUriOption,
      IEnumerable<string> dependsOnIds);

    protected void SetState(ODataBatchWriter.BatchWriterState newState)
    {
      this.InterceptException((Action) (() => this.ValidateTransition(newState)));
      this.state = newState;
    }

    protected abstract void VerifyNotDisposed();

    protected abstract void WriteStartBatchImplementation();

    protected abstract IEnumerable<string> GetDependsOnRequestIds(IEnumerable<string> dependsOnIds);

    protected ODataBatchOperationRequestMessage BuildOperationRequestMessage(
      Stream outputStream,
      string method,
      Uri uri,
      string contentId,
      string groupId,
      IEnumerable<string> dependsOnIds)
    {
      IEnumerable<string> dependsOnRequestIds1 = this.GetDependsOnRequestIds(dependsOnIds);
      if (dependsOnIds != null)
      {
        foreach (string str in dependsOnRequestIds1)
        {
          if (!this.payloadUriConverter.ContainsContentId(str))
            throw new ODataException(Strings.ODataBatchReader_DependsOnIdNotFound((object) str, (object) contentId));
        }
      }
      IEnumerable<string> dependsOnRequestIds2 = dependsOnIds == null ? this.payloadUriConverter.ContentIdCache : dependsOnRequestIds1;
      ODataBatchUtils.ValidateReferenceUri(uri, dependsOnRequestIds2, this.outputContext.MessageWriterSettings.BaseUri);
      return new ODataBatchOperationRequestMessage((Func<Stream>) (() => (Stream) ODataBatchUtils.CreateBatchOperationWriteStream(outputStream, (IODataStreamListener) this)), method, uri, (ODataBatchOperationHeaders) null, (IODataStreamListener) this, contentId, (IODataPayloadUriConverter) this.payloadUriConverter, true, this.container, dependsOnIds, groupId);
    }

    protected ODataBatchOperationResponseMessage BuildOperationResponseMessage(
      Stream outputStream,
      string contentId,
      string groupId)
    {
      return new ODataBatchOperationResponseMessage((Func<Stream>) (() => (Stream) ODataBatchUtils.CreateBatchOperationWriteStream(outputStream, (IODataStreamListener) this)), (ODataBatchOperationHeaders) null, (IODataStreamListener) this, contentId, this.payloadUriConverter.BatchMessagePayloadUriConverter, true, this.container, groupId);
    }

    private void InterceptException(Action action)
    {
      try
      {
        action();
      }
      catch
      {
        if (!ODataBatchWriter.IsErrorState(this.state))
          this.SetState(ODataBatchWriter.BatchWriterState.Error);
        throw;
      }
    }

    private void ThrowODataException(string errorMessage)
    {
      this.SetState(ODataBatchWriter.BatchWriterState.Error);
      throw new ODataException(errorMessage);
    }

    private ODataBatchOperationRequestMessage CreateOperationRequestMessageInternal(
      string method,
      Uri uri,
      string contentId,
      BatchPayloadUriOption payloadUriOption,
      IEnumerable<string> dependsOnIds)
    {
      if (!this.isInChangset)
        this.InterceptException(new Action(this.IncreaseBatchSize));
      else
        this.InterceptException(new Action(this.IncreaseChangeSetSize));
      if (this.currentOperationContentId != null)
        this.payloadUriConverter.AddContentId(this.currentOperationContentId);
      this.InterceptException((Action) (() => uri = ODataBatchUtils.CreateOperationRequestUri(uri, this.outputContext.MessageWriterSettings.BaseUri, (IODataPayloadUriConverter) this.payloadUriConverter)));
      this.CurrentOperationRequestMessage = this.CreateOperationRequestMessageImplementation(method, uri, contentId, payloadUriOption, dependsOnIds);
      if (!this.isInChangset)
      {
        ODataVersion? version = this.outputContext.MessageWriterSettings.Version;
        ODataVersion odataVersion = ODataVersion.V4;
        if (!(version.GetValueOrDefault() > odataVersion & version.HasValue))
          goto label_8;
      }
      this.RememberContentIdHeader(this.CurrentOperationRequestMessage.ContentId);
label_8:
      return this.CurrentOperationRequestMessage;
    }

    private void FinishWriteStartChangeset()
    {
      ODataVersion? version = this.outputContext.MessageWriterSettings.Version;
      ODataVersion odataVersion = ODataVersion.V4;
      if (version.GetValueOrDefault() <= odataVersion & version.HasValue)
        this.payloadUriConverter.Reset();
      this.ResetChangeSetSize();
      this.InterceptException(new Action(this.IncreaseBatchSize));
      this.isInChangset = true;
    }

    private void FinishWriteEndChangeset()
    {
      ODataVersion? version = this.outputContext.MessageWriterSettings.Version;
      ODataVersion odataVersion = ODataVersion.V4;
      if (version.GetValueOrDefault() <= odataVersion & version.HasValue)
        this.currentOperationContentId = (string) null;
      this.isInChangset = false;
    }

    private void RememberContentIdHeader(string contentId) => this.currentOperationContentId = contentId == null || !this.payloadUriConverter.ContainsContentId(contentId) ? contentId : throw new ODataException(Strings.ODataBatchWriter_DuplicateContentIDsNotAllowed((object) contentId));

    private void IncreaseBatchSize()
    {
      ++this.currentBatchSize;
      if ((long) this.currentBatchSize > (long) this.outputContext.MessageWriterSettings.MessageQuotas.MaxPartsPerBatch)
        throw new ODataException(Strings.ODataBatchWriter_MaxBatchSizeExceeded((object) this.outputContext.MessageWriterSettings.MessageQuotas.MaxPartsPerBatch));
    }

    private void IncreaseChangeSetSize()
    {
      ++this.currentChangeSetSize;
      if ((long) this.currentChangeSetSize > (long) this.outputContext.MessageWriterSettings.MessageQuotas.MaxOperationsPerChangeset)
        throw new ODataException(Strings.ODataBatchWriter_MaxChangeSetSizeExceeded((object) this.outputContext.MessageWriterSettings.MessageQuotas.MaxOperationsPerChangeset));
    }

    private void ResetChangeSetSize() => this.currentChangeSetSize = 0U;

    private void VerifyCallAllowed(bool synchronousCall)
    {
      if (synchronousCall)
      {
        if (!this.outputContext.Synchronous)
          throw new ODataException(Strings.ODataBatchWriter_SyncCallOnAsyncWriter);
      }
      else if (this.outputContext.Synchronous)
        throw new ODataException(Strings.ODataBatchWriter_AsyncCallOnSyncWriter);
    }

    private void VerifyCanCreateOperationRequestMessage(
      bool synchronousCall,
      string method,
      Uri uri,
      string contentId)
    {
      ExceptionUtils.CheckArgumentStringNotNullOrEmpty(method, nameof (method));
      ExceptionUtils.CheckArgumentNotNull<Uri>(uri, nameof (uri));
      if (this.isInChangset)
      {
        if (HttpUtils.IsQueryMethod(method))
          this.ThrowODataException(Strings.ODataBatch_InvalidHttpMethodForChangeSetRequest((object) method));
        if (string.IsNullOrEmpty(contentId))
          this.ThrowODataException(Strings.ODataBatchOperationHeaderDictionary_KeyNotFound((object) "Content-ID"));
      }
      this.ValidateWriterReady();
      this.VerifyCallAllowed(synchronousCall);
      if (!this.outputContext.WritingResponse)
        return;
      this.ThrowODataException(Strings.ODataBatchWriter_CannotCreateRequestOperationWhenWritingResponse);
    }

    private void VerifyCanFlush(bool synchronousCall)
    {
      this.VerifyNotDisposed();
      this.VerifyCallAllowed(synchronousCall);
      if (this.state != ODataBatchWriter.BatchWriterState.OperationStreamRequested)
        return;
      this.ThrowODataException(Strings.ODataBatchWriter_FlushOrFlushAsyncCalledInStreamRequestedState);
    }

    private void ValidateWriterReady()
    {
      this.VerifyNotDisposed();
      if (this.state == ODataBatchWriter.BatchWriterState.OperationStreamRequested)
        throw new ODataException(Strings.ODataBatchWriter_InvalidTransitionFromOperationContentStreamRequested);
    }

    private static bool IsErrorState(ODataBatchWriter.BatchWriterState state) => state == ODataBatchWriter.BatchWriterState.Error;

    private void VerifyCanWriteStartBatch(bool synchronousCall)
    {
      this.ValidateWriterReady();
      this.VerifyCallAllowed(synchronousCall);
    }

    private void VerifyCanWriteEndBatch(bool synchronousCall)
    {
      this.ValidateWriterReady();
      this.VerifyCallAllowed(synchronousCall);
    }

    private void VerifyCanWriteStartChangeset(bool synchronousCall)
    {
      this.ValidateWriterReady();
      this.VerifyCallAllowed(synchronousCall);
    }

    private void VerifyCanWriteEndChangeset(bool synchronousCall)
    {
      this.ValidateWriterReady();
      this.VerifyCallAllowed(synchronousCall);
    }

    private void VerifyCanCreateOperationResponseMessage(bool synchronousCall)
    {
      this.ValidateWriterReady();
      this.VerifyCallAllowed(synchronousCall);
      if (this.outputContext.WritingResponse)
        return;
      this.ThrowODataException(Strings.ODataBatchWriter_CannotCreateResponseOperationWhenWritingRequest);
    }

    [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Validating the transition in the state machine should stay in a single method.")]
    private void ValidateTransition(ODataBatchWriter.BatchWriterState newState)
    {
      if (!ODataBatchWriter.IsErrorState(this.state) && ODataBatchWriter.IsErrorState(newState))
        return;
      switch (newState)
      {
        case ODataBatchWriter.BatchWriterState.ChangesetStarted:
          if (this.isInChangset)
            throw new ODataException(Strings.ODataBatchWriter_CannotStartChangeSetWithActiveChangeSet);
          break;
        case ODataBatchWriter.BatchWriterState.ChangesetCompleted:
          if (!this.isInChangset)
            throw new ODataException(Strings.ODataBatchWriter_CannotCompleteChangeSetWithoutActiveChangeSet);
          break;
        case ODataBatchWriter.BatchWriterState.BatchCompleted:
          if (this.isInChangset)
            throw new ODataException(Strings.ODataBatchWriter_CannotCompleteBatchWithActiveChangeSet);
          break;
      }
      switch (this.state)
      {
        case ODataBatchWriter.BatchWriterState.Start:
          if (newState == ODataBatchWriter.BatchWriterState.BatchStarted)
            break;
          throw new ODataException(Strings.ODataBatchWriter_InvalidTransitionFromStart);
        case ODataBatchWriter.BatchWriterState.BatchStarted:
          if (newState == ODataBatchWriter.BatchWriterState.ChangesetStarted || newState == ODataBatchWriter.BatchWriterState.OperationCreated || newState == ODataBatchWriter.BatchWriterState.BatchCompleted)
            break;
          throw new ODataException(Strings.ODataBatchWriter_InvalidTransitionFromBatchStarted);
        case ODataBatchWriter.BatchWriterState.ChangesetStarted:
          if (newState == ODataBatchWriter.BatchWriterState.OperationCreated || newState == ODataBatchWriter.BatchWriterState.ChangesetCompleted)
            break;
          throw new ODataException(Strings.ODataBatchWriter_InvalidTransitionFromChangeSetStarted);
        case ODataBatchWriter.BatchWriterState.OperationCreated:
          if (newState == ODataBatchWriter.BatchWriterState.OperationCreated || newState == ODataBatchWriter.BatchWriterState.OperationStreamRequested || newState == ODataBatchWriter.BatchWriterState.ChangesetStarted || newState == ODataBatchWriter.BatchWriterState.ChangesetCompleted || newState == ODataBatchWriter.BatchWriterState.BatchCompleted)
            break;
          throw new ODataException(Strings.ODataBatchWriter_InvalidTransitionFromOperationCreated);
        case ODataBatchWriter.BatchWriterState.OperationStreamRequested:
          if (newState == ODataBatchWriter.BatchWriterState.OperationStreamDisposed)
            break;
          throw new ODataException(Strings.ODataBatchWriter_InvalidTransitionFromOperationContentStreamRequested);
        case ODataBatchWriter.BatchWriterState.OperationStreamDisposed:
          if (newState == ODataBatchWriter.BatchWriterState.OperationCreated || newState == ODataBatchWriter.BatchWriterState.ChangesetStarted || newState == ODataBatchWriter.BatchWriterState.ChangesetCompleted || newState == ODataBatchWriter.BatchWriterState.BatchCompleted)
            break;
          throw new ODataException(Strings.ODataBatchWriter_InvalidTransitionFromOperationContentStreamDisposed);
        case ODataBatchWriter.BatchWriterState.ChangesetCompleted:
          if (newState == ODataBatchWriter.BatchWriterState.BatchCompleted || newState == ODataBatchWriter.BatchWriterState.ChangesetStarted || newState == ODataBatchWriter.BatchWriterState.OperationCreated)
            break;
          throw new ODataException(Strings.ODataBatchWriter_InvalidTransitionFromChangeSetCompleted);
        case ODataBatchWriter.BatchWriterState.BatchCompleted:
          throw new ODataException(Strings.ODataBatchWriter_InvalidTransitionFromBatchCompleted);
        case ODataBatchWriter.BatchWriterState.Error:
          if (newState == ODataBatchWriter.BatchWriterState.Error)
            break;
          throw new ODataException(Strings.ODataWriterCore_InvalidTransitionFromError((object) this.state.ToString(), (object) newState.ToString()));
        default:
          throw new ODataException(Strings.General_InternalError((object) InternalErrorCodes.ODataBatchWriter_ValidateTransition_UnreachableCodePath));
      }
    }

    protected enum BatchWriterState
    {
      Start,
      BatchStarted,
      ChangesetStarted,
      OperationCreated,
      OperationStreamRequested,
      OperationStreamDisposed,
      ChangesetCompleted,
      BatchCompleted,
      Error,
    }
  }
}
