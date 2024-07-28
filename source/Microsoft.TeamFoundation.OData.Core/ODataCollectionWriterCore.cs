// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.ODataCollectionWriterCore
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Edm;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.OData
{
  internal abstract class ODataCollectionWriterCore : 
    ODataCollectionWriter,
    IODataOutputInStreamErrorListener
  {
    private readonly ODataOutputContext outputContext;
    private readonly IODataReaderWriterListener listener;
    private readonly Stack<ODataCollectionWriterCore.Scope> scopes = new Stack<ODataCollectionWriterCore.Scope>();
    private readonly IEdmTypeReference expectedItemType;
    private IDuplicatePropertyNameChecker duplicatePropertyNameChecker;
    private CollectionWithoutExpectedTypeValidator collectionValidator;

    protected ODataCollectionWriterCore(
      ODataOutputContext outputContext,
      IEdmTypeReference itemTypeReference)
      : this(outputContext, itemTypeReference, (IODataReaderWriterListener) null)
    {
    }

    protected ODataCollectionWriterCore(
      ODataOutputContext outputContext,
      IEdmTypeReference expectedItemType,
      IODataReaderWriterListener listener)
    {
      this.outputContext = outputContext;
      this.expectedItemType = expectedItemType;
      this.listener = listener;
      this.scopes.Push(new ODataCollectionWriterCore.Scope(ODataCollectionWriterCore.CollectionWriterState.Start, (object) null));
    }

    protected ODataCollectionWriterCore.CollectionWriterState State => this.scopes.Peek().State;

    protected IDuplicatePropertyNameChecker DuplicatePropertyNameChecker => this.duplicatePropertyNameChecker ?? (this.duplicatePropertyNameChecker = this.outputContext.MessageWriterSettings.Validator.CreateDuplicatePropertyNameChecker());

    protected CollectionWithoutExpectedTypeValidator CollectionValidator => this.collectionValidator;

    protected IEdmTypeReference ItemTypeReference => this.expectedItemType;

    public override sealed void Flush()
    {
      this.VerifyCanFlush(true);
      try
      {
        this.FlushSynchronously();
      }
      catch
      {
        this.ReplaceScope(ODataCollectionWriterCore.CollectionWriterState.Error, (ODataItem) null);
        throw;
      }
    }

    public override sealed Task FlushAsync()
    {
      this.VerifyCanFlush(false);
      return this.FlushAsynchronously().FollowOnFaultWith((Action<Task>) (t => this.ReplaceScope(ODataCollectionWriterCore.CollectionWriterState.Error, (ODataItem) null)));
    }

    public override sealed void WriteStart(ODataCollectionStart collectionStart)
    {
      this.VerifyCanWriteStart(true, collectionStart);
      this.WriteStartImplementation(collectionStart);
    }

    public override sealed Task WriteStartAsync(ODataCollectionStart collection)
    {
      this.VerifyCanWriteStart(false, collection);
      return TaskUtils.GetTaskForSynchronousOperation((Action) (() => this.WriteStartImplementation(collection)));
    }

    public override sealed void WriteItem(object item)
    {
      this.VerifyCanWriteItem(true);
      this.WriteItemImplementation(item);
    }

    public override sealed Task WriteItemAsync(object item)
    {
      this.VerifyCanWriteItem(false);
      return TaskUtils.GetTaskForSynchronousOperation((Action) (() => this.WriteItemImplementation(item)));
    }

    public override sealed void WriteEnd()
    {
      this.VerifyCanWriteEnd(true);
      this.WriteEndImplementation();
      if (this.scopes.Peek().State != ODataCollectionWriterCore.CollectionWriterState.Completed)
        return;
      this.Flush();
    }

    public override sealed Task WriteEndAsync()
    {
      this.VerifyCanWriteEnd(false);
      return TaskUtils.GetTaskForSynchronousOperation(new Action(this.WriteEndImplementation)).FollowOnSuccessWithTask((Func<Task, Task>) (task => this.scopes.Peek().State == ODataCollectionWriterCore.CollectionWriterState.Completed ? this.FlushAsync() : TaskUtils.CompletedTask));
    }

    void IODataOutputInStreamErrorListener.OnInStreamError()
    {
      this.VerifyNotDisposed();
      if (this.State == ODataCollectionWriterCore.CollectionWriterState.Completed)
        throw new ODataException(Strings.ODataWriterCore_InvalidTransitionFromCompleted((object) this.State.ToString(), (object) ODataCollectionWriterCore.CollectionWriterState.Error.ToString()));
      this.StartPayloadInStartState();
      this.EnterScope(ODataCollectionWriterCore.CollectionWriterState.Error, this.scopes.Peek().Item);
    }

    protected static bool IsErrorState(
      ODataCollectionWriterCore.CollectionWriterState state)
    {
      return state == ODataCollectionWriterCore.CollectionWriterState.Error;
    }

    protected abstract void VerifyNotDisposed();

    protected abstract void FlushSynchronously();

    protected abstract Task FlushAsynchronously();

    protected abstract void StartPayload();

    protected abstract void EndPayload();

    protected abstract void StartCollection(ODataCollectionStart collectionStart);

    protected abstract void EndCollection();

    protected abstract void WriteCollectionItem(
      object item,
      IEdmTypeReference expectedItemTypeReference);

    private void VerifyCanWriteStart(bool synchronousCall, ODataCollectionStart collectionStart)
    {
      ExceptionUtils.CheckArgumentNotNull<ODataCollectionStart>(collectionStart, "collection");
      this.VerifyNotDisposed();
      this.VerifyCallAllowed(synchronousCall);
    }

    private void WriteStartImplementation(ODataCollectionStart collectionStart)
    {
      this.StartPayloadInStartState();
      this.EnterScope(ODataCollectionWriterCore.CollectionWriterState.Collection, (object) collectionStart);
      this.InterceptException((Action) (() =>
      {
        if (this.expectedItemType == null)
          this.collectionValidator = new CollectionWithoutExpectedTypeValidator((string) null);
        this.StartCollection(collectionStart);
      }));
    }

    private void VerifyCanWriteItem(bool synchronousCall)
    {
      this.VerifyNotDisposed();
      this.VerifyCallAllowed(synchronousCall);
    }

    private void WriteItemImplementation(object item)
    {
      if (this.scopes.Peek().State != ODataCollectionWriterCore.CollectionWriterState.Item)
        this.EnterScope(ODataCollectionWriterCore.CollectionWriterState.Item, item);
      this.InterceptException((Action) (() =>
      {
        ValidationUtils.ValidateCollectionItem(item, true);
        this.WriteCollectionItem(item, this.expectedItemType);
      }));
    }

    private void VerifyCanWriteEnd(bool synchronousCall)
    {
      this.VerifyNotDisposed();
      this.VerifyCallAllowed(synchronousCall);
    }

    private void WriteEndImplementation() => this.InterceptException((Action) (() =>
    {
      ODataCollectionWriterCore.Scope scope = this.scopes.Peek();
      switch (scope.State)
      {
        case ODataCollectionWriterCore.CollectionWriterState.Start:
        case ODataCollectionWriterCore.CollectionWriterState.Completed:
        case ODataCollectionWriterCore.CollectionWriterState.Error:
          throw new ODataException(Strings.ODataCollectionWriterCore_WriteEndCalledInInvalidState((object) scope.State.ToString()));
        case ODataCollectionWriterCore.CollectionWriterState.Collection:
          this.EndCollection();
          break;
        case ODataCollectionWriterCore.CollectionWriterState.Item:
          this.LeaveScope();
          this.EndCollection();
          break;
        default:
          throw new ODataException(Strings.General_InternalError((object) InternalErrorCodes.ODataCollectionWriterCore_WriteEnd_UnreachableCodePath));
      }
      this.LeaveScope();
    }));

    private void VerifyCanFlush(bool synchronousCall)
    {
      this.VerifyNotDisposed();
      this.VerifyCallAllowed(synchronousCall);
    }

    private void VerifyCallAllowed(bool synchronousCall)
    {
      if (synchronousCall)
      {
        if (!this.outputContext.Synchronous)
          throw new ODataException(Strings.ODataCollectionWriterCore_SyncCallOnAsyncWriter);
      }
      else if (this.outputContext.Synchronous)
        throw new ODataException(Strings.ODataCollectionWriterCore_AsyncCallOnSyncWriter);
    }

    private void StartPayloadInStartState()
    {
      if (this.scopes.Peek().State != ODataCollectionWriterCore.CollectionWriterState.Start)
        return;
      this.InterceptException(new Action(this.StartPayload));
    }

    private void InterceptException(Action action)
    {
      try
      {
        action();
      }
      catch
      {
        if (!ODataCollectionWriterCore.IsErrorState(this.State))
          this.EnterScope(ODataCollectionWriterCore.CollectionWriterState.Error, this.scopes.Peek().Item);
        throw;
      }
    }

    private void NotifyListener(
      ODataCollectionWriterCore.CollectionWriterState newState)
    {
      if (this.listener == null)
        return;
      if (ODataCollectionWriterCore.IsErrorState(newState))
      {
        this.listener.OnException();
      }
      else
      {
        if (newState != ODataCollectionWriterCore.CollectionWriterState.Completed)
          return;
        this.listener.OnCompleted();
      }
    }

    private void EnterScope(
      ODataCollectionWriterCore.CollectionWriterState newState,
      object item)
    {
      this.InterceptException((Action) (() => this.ValidateTransition(newState)));
      this.scopes.Push(new ODataCollectionWriterCore.Scope(newState, item));
      this.NotifyListener(newState);
    }

    private void LeaveScope()
    {
      this.scopes.Pop();
      if (this.scopes.Count != 1)
        return;
      this.scopes.Pop();
      this.scopes.Push(new ODataCollectionWriterCore.Scope(ODataCollectionWriterCore.CollectionWriterState.Completed, (object) null));
      this.InterceptException(new Action(this.EndPayload));
      this.NotifyListener(ODataCollectionWriterCore.CollectionWriterState.Completed);
    }

    private void ReplaceScope(
      ODataCollectionWriterCore.CollectionWriterState newState,
      ODataItem item)
    {
      this.ValidateTransition(newState);
      this.scopes.Pop();
      this.scopes.Push(new ODataCollectionWriterCore.Scope(newState, (object) item));
      this.NotifyListener(newState);
    }

    private void ValidateTransition(
      ODataCollectionWriterCore.CollectionWriterState newState)
    {
      if (!ODataCollectionWriterCore.IsErrorState(this.State) && ODataCollectionWriterCore.IsErrorState(newState))
        return;
      switch (this.State)
      {
        case ODataCollectionWriterCore.CollectionWriterState.Start:
          if (newState == ODataCollectionWriterCore.CollectionWriterState.Collection || newState == ODataCollectionWriterCore.CollectionWriterState.Completed)
            break;
          throw new ODataException(Strings.ODataCollectionWriterCore_InvalidTransitionFromStart((object) this.State.ToString(), (object) newState.ToString()));
        case ODataCollectionWriterCore.CollectionWriterState.Collection:
          if (newState == ODataCollectionWriterCore.CollectionWriterState.Item || newState == ODataCollectionWriterCore.CollectionWriterState.Completed)
            break;
          throw new ODataException(Strings.ODataCollectionWriterCore_InvalidTransitionFromCollection((object) this.State.ToString(), (object) newState.ToString()));
        case ODataCollectionWriterCore.CollectionWriterState.Item:
          if (newState == ODataCollectionWriterCore.CollectionWriterState.Completed)
            break;
          throw new ODataException(Strings.ODataCollectionWriterCore_InvalidTransitionFromItem((object) this.State.ToString(), (object) newState.ToString()));
        case ODataCollectionWriterCore.CollectionWriterState.Completed:
          throw new ODataException(Strings.ODataWriterCore_InvalidTransitionFromCompleted((object) this.State.ToString(), (object) newState.ToString()));
        case ODataCollectionWriterCore.CollectionWriterState.Error:
          if (newState == ODataCollectionWriterCore.CollectionWriterState.Error)
            break;
          throw new ODataException(Strings.ODataWriterCore_InvalidTransitionFromError((object) this.State.ToString(), (object) newState.ToString()));
        default:
          throw new ODataException(Strings.General_InternalError((object) InternalErrorCodes.ODataCollectionWriterCore_ValidateTransition_UnreachableCodePath));
      }
    }

    internal enum CollectionWriterState
    {
      Start,
      Collection,
      Item,
      Completed,
      Error,
    }

    private sealed class Scope
    {
      private readonly ODataCollectionWriterCore.CollectionWriterState state;
      private readonly object item;

      public Scope(
        ODataCollectionWriterCore.CollectionWriterState state,
        object item)
      {
        this.state = state;
        this.item = item;
      }

      public ODataCollectionWriterCore.CollectionWriterState State => this.state;

      public object Item => this.item;
    }
  }
}
