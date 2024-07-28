// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.ODataCollectionReaderCore
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Edm;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace Microsoft.OData
{
  internal abstract class ODataCollectionReaderCore : ODataCollectionReader
  {
    private readonly ODataInputContext inputContext;
    private readonly Stack<ODataCollectionReaderCore.Scope> scopes = new Stack<ODataCollectionReaderCore.Scope>();
    private readonly IODataReaderWriterListener listener;
    private CollectionWithoutExpectedTypeValidator collectionValidator;
    private IEdmTypeReference expectedItemTypeReference;

    protected ODataCollectionReaderCore(
      ODataInputContext inputContext,
      IEdmTypeReference expectedItemTypeReference,
      IODataReaderWriterListener listener)
    {
      this.inputContext = inputContext;
      this.expectedItemTypeReference = expectedItemTypeReference;
      if (this.expectedItemTypeReference == null)
        this.collectionValidator = new CollectionWithoutExpectedTypeValidator((string) null);
      this.listener = listener;
      this.EnterScope(ODataCollectionReaderState.Start, (object) null);
    }

    public override sealed ODataCollectionReaderState State
    {
      get
      {
        this.inputContext.VerifyNotDisposed();
        return this.scopes.Peek().State;
      }
    }

    public override sealed object Item
    {
      get
      {
        this.inputContext.VerifyNotDisposed();
        return this.scopes.Peek().Item;
      }
    }

    protected IEdmTypeReference ExpectedItemTypeReference
    {
      get => this.expectedItemTypeReference;
      set
      {
        ExceptionUtils.CheckArgumentNotNull<IEdmTypeReference>(value, nameof (value));
        if (this.expectedItemTypeReference == value)
          return;
        this.expectedItemTypeReference = value;
        this.collectionValidator = (CollectionWithoutExpectedTypeValidator) null;
      }
    }

    protected CollectionWithoutExpectedTypeValidator CollectionValidator => this.collectionValidator;

    protected bool IsReadingNestedPayload => this.listener != null;

    public override sealed bool Read()
    {
      this.VerifyCanRead(true);
      return this.InterceptException<bool>(new Func<bool>(this.ReadSynchronously));
    }

    public override sealed Task<bool> ReadAsync()
    {
      this.VerifyCanRead(false);
      return this.ReadAsynchronously().FollowOnFaultWith<bool>((Action<Task<bool>>) (t => this.EnterScope(ODataCollectionReaderState.Exception, (object) null)));
    }

    protected bool ReadImplementation()
    {
      switch (this.State)
      {
        case ODataCollectionReaderState.Start:
          return this.ReadAtStartImplementation();
        case ODataCollectionReaderState.CollectionStart:
          return this.ReadAtCollectionStartImplementation();
        case ODataCollectionReaderState.Value:
          return this.ReadAtValueImplementation();
        case ODataCollectionReaderState.CollectionEnd:
          return this.ReadAtCollectionEndImplementation();
        default:
          throw new ODataException(Strings.General_InternalError((object) InternalErrorCodes.ODataCollectionReaderCore_ReadImplementation));
      }
    }

    protected abstract bool ReadAtStartImplementation();

    protected abstract bool ReadAtCollectionStartImplementation();

    protected abstract bool ReadAtValueImplementation();

    protected abstract bool ReadAtCollectionEndImplementation();

    protected bool ReadSynchronously() => this.ReadImplementation();

    protected virtual Task<bool> ReadAsynchronously() => TaskUtils.GetTaskForSynchronousOperation<bool>(new Func<bool>(this.ReadImplementation));

    protected void EnterScope(ODataCollectionReaderState state, object item) => this.EnterScope(state, item, false);

    protected void EnterScope(
      ODataCollectionReaderState state,
      object item,
      bool isCollectionElementEmpty)
    {
      if (state == ODataCollectionReaderState.Value)
        ValidationUtils.ValidateCollectionItem(item, true);
      this.scopes.Push(new ODataCollectionReaderCore.Scope(state, item, isCollectionElementEmpty));
      if (this.listener == null)
        return;
      if (state == ODataCollectionReaderState.Exception)
      {
        this.listener.OnException();
      }
      else
      {
        if (state != ODataCollectionReaderState.Completed)
          return;
        this.listener.OnCompleted();
      }
    }

    protected void ReplaceScope(ODataCollectionReaderState state, object item)
    {
      if (state == ODataCollectionReaderState.Value)
        ValidationUtils.ValidateCollectionItem(item, true);
      this.scopes.Pop();
      this.EnterScope(state, item);
    }

    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "state", Justification = "Used in debug builds in assertions.")]
    [SuppressMessage("Microsoft.Performance", "CA1804:RemoveUnusedLocals", MessageId = "scope", Justification = "Used in debug builds in assertions.")]
    protected void PopScope(ODataCollectionReaderState state) => this.scopes.Pop();

    private T InterceptException<T>(Func<T> action)
    {
      try
      {
        return action();
      }
      catch (Exception ex)
      {
        if (ExceptionUtils.IsCatchableExceptionType(ex))
          this.EnterScope(ODataCollectionReaderState.Exception, (object) null);
        throw;
      }
    }

    private void VerifyCanRead(bool synchronousCall)
    {
      this.inputContext.VerifyNotDisposed();
      this.VerifyCallAllowed(synchronousCall);
      if (this.State == ODataCollectionReaderState.Exception || this.State == ODataCollectionReaderState.Completed)
        throw new ODataException(Strings.ODataCollectionReaderCore_ReadOrReadAsyncCalledInInvalidState((object) this.State));
    }

    private void VerifyCallAllowed(bool synchronousCall)
    {
      if (synchronousCall)
        this.VerifySynchronousCallAllowed();
      else
        this.VerifyAsynchronousCallAllowed();
    }

    private void VerifySynchronousCallAllowed()
    {
      if (!this.inputContext.Synchronous)
        throw new ODataException(Strings.ODataCollectionReaderCore_SyncCallOnAsyncReader);
    }

    private void VerifyAsynchronousCallAllowed()
    {
      if (this.inputContext.Synchronous)
        throw new ODataException(Strings.ODataCollectionReaderCore_AsyncCallOnSyncReader);
    }

    protected sealed class Scope
    {
      private readonly ODataCollectionReaderState state;
      private readonly object item;
      [SuppressMessage("Microsoft.Performance", "CA1823", Justification = "isCollectionElementEmpty is used in debug.")]
      private readonly bool isCollectionElementEmpty;

      [SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily", Justification = "Debug.Assert check only.")]
      public Scope(ODataCollectionReaderState state, object item)
        : this(state, item, false)
      {
      }

      [SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily", Justification = "Debug.Assert check only.")]
      public Scope(ODataCollectionReaderState state, object item, bool isCollectionElementEmpty)
      {
        this.state = state;
        this.item = item;
        this.isCollectionElementEmpty = isCollectionElementEmpty;
      }

      public ODataCollectionReaderState State => this.state;

      public object Item => this.item;
    }
  }
}
