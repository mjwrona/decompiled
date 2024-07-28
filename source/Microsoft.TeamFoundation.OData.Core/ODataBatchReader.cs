// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.ODataBatchReader
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
  public abstract class ODataBatchReader : IODataStreamListener
  {
    internal readonly ODataBatchPayloadUriConverter PayloadUriConverter;
    private readonly ODataInputContext inputContext;
    private readonly bool synchronous;
    private readonly IServiceProvider container;
    private ODataBatchReaderState batchReaderState;
    private uint currentBatchSize;
    private uint currentChangeSetSize;
    private ODataBatchReader.OperationState operationState;
    private bool isInChangeset;
    private string contentIdToAddOnNextRead;

    protected ODataBatchReader(ODataInputContext inputContext, bool synchronous)
    {
      this.inputContext = inputContext;
      this.container = inputContext.Container;
      this.synchronous = synchronous;
      this.PayloadUriConverter = new ODataBatchPayloadUriConverter(inputContext.PayloadUriConverter);
    }

    public ODataBatchReaderState State
    {
      get
      {
        this.inputContext.VerifyNotDisposed();
        return this.batchReaderState;
      }
      private set => this.batchReaderState = value;
    }

    public string CurrentGroupId => this.GetCurrentGroupIdImplementation();

    protected ODataInputContext InputContext => this.inputContext;

    private ODataBatchReader.OperationState ReaderOperationState
    {
      get => this.operationState;
      set => this.operationState = value;
    }

    public bool Read()
    {
      this.VerifyCanRead(true);
      return this.InterceptException<bool>(new Func<bool>(this.ReadSynchronously));
    }

    public Task<bool> ReadAsync()
    {
      this.VerifyCanRead(false);
      return this.ReadAsynchronously().FollowOnFaultWith<bool>((Action<Task<bool>>) (t => this.State = ODataBatchReaderState.Exception));
    }

    public ODataBatchOperationRequestMessage CreateOperationRequestMessage()
    {
      this.VerifyCanCreateOperationRequestMessage(true);
      ODataBatchOperationRequestMessage operationRequestMessage = this.InterceptException<ODataBatchOperationRequestMessage>(new Func<ODataBatchOperationRequestMessage>(this.CreateOperationRequestMessageImplementation));
      this.ReaderOperationState = ODataBatchReader.OperationState.MessageCreated;
      this.contentIdToAddOnNextRead = operationRequestMessage.ContentId;
      return operationRequestMessage;
    }

    public Task<ODataBatchOperationRequestMessage> CreateOperationRequestMessageAsync()
    {
      this.VerifyCanCreateOperationRequestMessage(false);
      return TaskUtils.GetTaskForSynchronousOperation<ODataBatchOperationRequestMessage>(new Func<ODataBatchOperationRequestMessage>(this.CreateOperationRequestMessageImplementation)).FollowOnSuccessWithTask<ODataBatchOperationRequestMessage, ODataBatchOperationRequestMessage>((Func<Task<ODataBatchOperationRequestMessage>, Task<ODataBatchOperationRequestMessage>>) (t =>
      {
        this.ReaderOperationState = ODataBatchReader.OperationState.MessageCreated;
        this.contentIdToAddOnNextRead = t.Result.ContentId;
        return t;
      })).FollowOnFaultWith<ODataBatchOperationRequestMessage>((Action<Task<ODataBatchOperationRequestMessage>>) (t => this.State = ODataBatchReaderState.Exception));
    }

    public ODataBatchOperationResponseMessage CreateOperationResponseMessage()
    {
      this.VerifyCanCreateOperationResponseMessage(true);
      ODataBatchOperationResponseMessage operationResponseMessage = this.InterceptException<ODataBatchOperationResponseMessage>(new Func<ODataBatchOperationResponseMessage>(this.CreateOperationResponseMessageImplementation));
      this.ReaderOperationState = ODataBatchReader.OperationState.MessageCreated;
      return operationResponseMessage;
    }

    public Task<ODataBatchOperationResponseMessage> CreateOperationResponseMessageAsync()
    {
      this.VerifyCanCreateOperationResponseMessage(false);
      return TaskUtils.GetTaskForSynchronousOperation<ODataBatchOperationResponseMessage>(new Func<ODataBatchOperationResponseMessage>(this.CreateOperationResponseMessageImplementation)).FollowOnSuccessWithTask<ODataBatchOperationResponseMessage, ODataBatchOperationResponseMessage>((Func<Task<ODataBatchOperationResponseMessage>, Task<ODataBatchOperationResponseMessage>>) (t =>
      {
        this.ReaderOperationState = ODataBatchReader.OperationState.MessageCreated;
        return t;
      })).FollowOnFaultWith<ODataBatchOperationResponseMessage>((Action<Task<ODataBatchOperationResponseMessage>>) (t => this.State = ODataBatchReaderState.Exception));
    }

    void IODataStreamListener.StreamRequested() => this.operationState = ODataBatchReader.OperationState.StreamRequested;

    Task IODataStreamListener.StreamRequestedAsync()
    {
      this.operationState = ODataBatchReader.OperationState.StreamRequested;
      return TaskUtils.CompletedTask;
    }

    void IODataStreamListener.StreamDisposed() => this.operationState = ODataBatchReader.OperationState.StreamDisposed;

    protected void ThrowODataException(string errorMessage)
    {
      this.State = ODataBatchReaderState.Exception;
      throw new ODataException(errorMessage);
    }

    [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "A method for consistency with the rest of the API.")]
    protected virtual string GetCurrentGroupIdImplementation() => (string) null;

    protected abstract ODataBatchOperationRequestMessage CreateOperationRequestMessageImplementation();

    protected abstract ODataBatchOperationResponseMessage CreateOperationResponseMessageImplementation();

    protected abstract ODataBatchReaderState ReadAtStartImplementation();

    protected abstract ODataBatchReaderState ReadAtOperationImplementation();

    protected abstract ODataBatchReaderState ReadAtChangesetStartImplementation();

    protected abstract ODataBatchReaderState ReadAtChangesetEndImplementation();

    protected ODataBatchOperationRequestMessage BuildOperationRequestMessage(
      Func<Stream> streamCreatorFunc,
      string method,
      Uri requestUri,
      ODataBatchOperationHeaders headers,
      string contentId,
      string groupId,
      IEnumerable<string> dependsOnRequestIds,
      bool dependsOnIdsValidationRequired)
    {
      if (dependsOnRequestIds != null & dependsOnIdsValidationRequired)
      {
        foreach (string dependsOnRequestId in dependsOnRequestIds)
        {
          if (!this.PayloadUriConverter.ContainsContentId(dependsOnRequestId))
            throw new ODataException(Strings.ODataBatchReader_DependsOnIdNotFound((object) dependsOnRequestId, (object) contentId));
        }
      }
      Uri operationRequestUri = ODataBatchUtils.CreateOperationRequestUri(requestUri, this.inputContext.MessageReaderSettings.BaseUri, (IODataPayloadUriConverter) this.PayloadUriConverter);
      ODataBatchUtils.ValidateReferenceUri(requestUri, dependsOnRequestIds, this.inputContext.MessageReaderSettings.BaseUri);
      return new ODataBatchOperationRequestMessage(streamCreatorFunc, method, operationRequestUri, headers, (IODataStreamListener) this, contentId, (IODataPayloadUriConverter) this.PayloadUriConverter, false, this.container, dependsOnRequestIds, groupId);
    }

    protected ODataBatchOperationResponseMessage BuildOperationResponseMessage(
      Func<Stream> streamCreatorFunc,
      int statusCode,
      ODataBatchOperationHeaders headers,
      string contentId,
      string groupId)
    {
      return new ODataBatchOperationResponseMessage(streamCreatorFunc, headers, (IODataStreamListener) this, contentId, this.PayloadUriConverter.BatchMessagePayloadUriConverter, false, this.container, groupId)
      {
        StatusCode = statusCode
      };
    }

    private void IncreaseBatchSize()
    {
      if ((long) this.currentBatchSize == (long) this.inputContext.MessageReaderSettings.MessageQuotas.MaxPartsPerBatch)
        throw new ODataException(Strings.ODataBatchReader_MaxBatchSizeExceeded((object) this.inputContext.MessageReaderSettings.MessageQuotas.MaxPartsPerBatch));
      ++this.currentBatchSize;
    }

    private void IncreaseChangesetSize()
    {
      if ((long) this.currentChangeSetSize == (long) this.inputContext.MessageReaderSettings.MessageQuotas.MaxOperationsPerChangeset)
        throw new ODataException(Strings.ODataBatchReader_MaxChangeSetSizeExceeded((object) this.inputContext.MessageReaderSettings.MessageQuotas.MaxOperationsPerChangeset));
      ++this.currentChangeSetSize;
    }

    private void ResetChangesetSize() => this.currentChangeSetSize = 0U;

    private bool ReadSynchronously() => this.ReadImplementation();

    [SuppressMessage("Microsoft.MSInternal", "CA908:AvoidTypesThatRequireJitCompilationInPrecompiledAssemblies", Justification = "API design calls for a bool being returned from the task here.")]
    private Task<bool> ReadAsynchronously() => TaskUtils.GetTaskForSynchronousOperation<bool>(new Func<bool>(this.ReadImplementation));

    private bool ReadImplementation()
    {
      switch (this.State)
      {
        case ODataBatchReaderState.Initial:
          this.State = this.ReadAtStartImplementation();
          break;
        case ODataBatchReaderState.Operation:
          this.ReaderOperationState = this.ReaderOperationState != ODataBatchReader.OperationState.None ? ODataBatchReader.OperationState.None : throw new ODataException(Strings.ODataBatchReader_NoMessageWasCreatedForOperation);
          if (this.contentIdToAddOnNextRead != null)
          {
            if (this.PayloadUriConverter.ContainsContentId(this.contentIdToAddOnNextRead))
              throw new ODataException(Strings.ODataBatchReader_DuplicateContentIDsNotAllowed((object) this.contentIdToAddOnNextRead));
            this.PayloadUriConverter.AddContentId(this.contentIdToAddOnNextRead);
            this.contentIdToAddOnNextRead = (string) null;
          }
          if (this.isInChangeset)
            this.IncreaseChangesetSize();
          else
            this.IncreaseBatchSize();
          this.State = this.ReadAtOperationImplementation();
          break;
        case ODataBatchReaderState.ChangesetStart:
          if (this.isInChangeset)
            this.ThrowODataException(Strings.ODataBatchReaderStream_NestedChangesetsAreNotSupported);
          this.IncreaseBatchSize();
          this.State = this.ReadAtChangesetStartImplementation();
          ODataVersion? version = this.inputContext.MessageReaderSettings.Version;
          ODataVersion odataVersion = ODataVersion.V4;
          if (version.GetValueOrDefault() <= odataVersion & version.HasValue)
            this.PayloadUriConverter.Reset();
          this.isInChangeset = true;
          break;
        case ODataBatchReaderState.ChangesetEnd:
          this.ResetChangesetSize();
          this.isInChangeset = false;
          this.State = this.ReadAtChangesetEndImplementation();
          break;
        case ODataBatchReaderState.Completed:
        case ODataBatchReaderState.Exception:
          throw new ODataException(Strings.General_InternalError((object) InternalErrorCodes.ODataBatchReader_ReadImplementation));
        default:
          throw new ODataException(Strings.General_InternalError((object) InternalErrorCodes.ODataBatchReader_ReadImplementation));
      }
      return this.State != ODataBatchReaderState.Completed && this.State != ODataBatchReaderState.Exception;
    }

    private void VerifyCanCreateOperationRequestMessage(bool synchronousCall)
    {
      this.VerifyReaderReady();
      this.VerifyCallAllowed(synchronousCall);
      if (this.inputContext.ReadingResponse)
        this.ThrowODataException(Strings.ODataBatchReader_CannotCreateRequestOperationWhenReadingResponse);
      if (this.State != ODataBatchReaderState.Operation)
        this.ThrowODataException(Strings.ODataBatchReader_InvalidStateForCreateOperationRequestMessage((object) this.State));
      if (this.operationState == ODataBatchReader.OperationState.None)
        return;
      this.ThrowODataException(Strings.ODataBatchReader_OperationRequestMessageAlreadyCreated);
    }

    private void VerifyCanCreateOperationResponseMessage(bool synchronousCall)
    {
      this.VerifyReaderReady();
      this.VerifyCallAllowed(synchronousCall);
      if (!this.inputContext.ReadingResponse)
        this.ThrowODataException(Strings.ODataBatchReader_CannotCreateResponseOperationWhenReadingRequest);
      if (this.State != ODataBatchReaderState.Operation)
        this.ThrowODataException(Strings.ODataBatchReader_InvalidStateForCreateOperationResponseMessage((object) this.State));
      if (this.operationState == ODataBatchReader.OperationState.None)
        return;
      this.ThrowODataException(Strings.ODataBatchReader_OperationResponseMessageAlreadyCreated);
    }

    private void VerifyCanRead(bool synchronousCall)
    {
      this.VerifyReaderReady();
      this.VerifyCallAllowed(synchronousCall);
      if (this.State == ODataBatchReaderState.Exception || this.State == ODataBatchReaderState.Completed)
        throw new ODataException(Strings.ODataBatchReader_ReadOrReadAsyncCalledInInvalidState((object) this.State));
    }

    private void VerifyReaderReady()
    {
      this.inputContext.VerifyNotDisposed();
      if (this.operationState == ODataBatchReader.OperationState.StreamRequested)
        throw new ODataException(Strings.ODataBatchReader_CannotUseReaderWhileOperationStreamActive);
    }

    private void VerifyCallAllowed(bool synchronousCall)
    {
      if (synchronousCall)
      {
        if (!this.synchronous)
          throw new ODataException(Strings.ODataBatchReader_SyncCallOnAsyncReader);
      }
      else if (this.synchronous)
        throw new ODataException(Strings.ODataBatchReader_AsyncCallOnSyncReader);
    }

    private T InterceptException<T>(Func<T> action)
    {
      try
      {
        return action();
      }
      catch (Exception ex)
      {
        if (ExceptionUtils.IsCatchableExceptionType(ex))
          this.State = ODataBatchReaderState.Exception;
        throw;
      }
    }

    private enum OperationState
    {
      None,
      MessageCreated,
      StreamRequested,
      StreamDisposed,
    }
  }
}
