// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.ODataReaderCore
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Edm;
using Microsoft.OData.Metadata;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.OData
{
  internal abstract class ODataReaderCore : ODataReader, IODataStreamListener
  {
    private readonly ODataInputContext inputContext;
    private readonly bool readingResourceSet;
    private readonly bool readingDelta;
    private readonly Stack<ODataReaderCore.Scope> scopes = new Stack<ODataReaderCore.Scope>();
    private readonly IODataReaderWriterListener listener;
    private int currentResourceDepth;

    protected ODataReaderCore(
      ODataInputContext inputContext,
      bool readingResourceSet,
      bool readingDelta,
      IODataReaderWriterListener listener)
    {
      this.inputContext = inputContext;
      this.readingResourceSet = readingResourceSet;
      this.readingDelta = readingDelta;
      this.listener = listener;
      this.currentResourceDepth = 0;
      this.Version = inputContext.MessageReaderSettings.Version;
    }

    public override sealed ODataReaderState State
    {
      get
      {
        this.inputContext.VerifyNotDisposed();
        return this.scopes.Peek().State;
      }
    }

    public override sealed ODataItem Item
    {
      get
      {
        this.inputContext.VerifyNotDisposed();
        return this.scopes.Peek().Item;
      }
    }

    internal ODataVersion? Version { get; private set; }

    protected ODataResourceSet CurrentResourceSet => (ODataResourceSet) this.Item;

    protected ODataDeltaResourceSet CurrentDeltaResourceSet => (ODataDeltaResourceSet) this.Item;

    protected ODataDeltaLink CurrentDeltaLink => (ODataDeltaLink) this.Item;

    protected ODataDeltaDeletedLink CurrentDeltaDeletedLink => (ODataDeltaDeletedLink) this.Item;

    protected int CurrentResourceDepth => this.currentResourceDepth;

    protected ODataNestedResourceInfo CurrentNestedResourceInfo => (ODataNestedResourceInfo) this.Item;

    protected ODataEntityReferenceLink CurrentEntityReferenceLink => (ODataEntityReferenceLink) this.Item;

    protected IEdmType CurrentResourceType => this.CurrentResourceTypeReference != null ? this.CurrentResourceTypeReference.Definition : (IEdmType) null;

    protected IEdmTypeReference CurrentResourceTypeReference
    {
      get => this.scopes.Peek().ResourceTypeReference;
      set => this.scopes.Peek().ResourceTypeReference = value;
    }

    protected IEdmNavigationSource CurrentNavigationSource => this.scopes.Peek().NavigationSource;

    protected ODataReaderCore.Scope CurrentScope => this.scopes.Peek();

    protected Stack<ODataReaderCore.Scope> Scopes => this.scopes;

    protected ODataReaderCore.Scope ParentScope => this.scopes.Skip<ODataReaderCore.Scope>(1).First<ODataReaderCore.Scope>();

    protected bool IsTopLevel => this.scopes.Count <= 2;

    protected ODataReaderCore.Scope ExpandedLinkContentParentScope
    {
      get
      {
        if (this.scopes.Count > 1)
        {
          ODataReaderCore.Scope contentParentScope = this.scopes.Skip<ODataReaderCore.Scope>(1).First<ODataReaderCore.Scope>();
          if (contentParentScope.State == ODataReaderState.NestedResourceInfoStart)
            return contentParentScope;
        }
        return (ODataReaderCore.Scope) null;
      }
    }

    protected bool IsExpandedLinkContent => this.ExpandedLinkContentParentScope != null;

    protected bool ReadingResourceSet => this.readingResourceSet;

    protected bool ReadingDelta => this.readingDelta;

    protected bool IsReadingNestedPayload => this.listener != null;

    protected ResourceSetWithoutExpectedTypeValidator CurrentResourceSetValidator => this.ParentScope != null ? this.ParentScope.ResourceTypeValidator : (ResourceSetWithoutExpectedTypeValidator) null;

    protected DerivedTypeValidator CurrentDerivedTypeValidator => this.ParentScope != null ? this.ParentScope.DerivedTypeValidator : (DerivedTypeValidator) null;

    public override sealed bool Read()
    {
      this.VerifyCanRead(true);
      return this.InterceptException<bool>(new Func<bool>(this.ReadSynchronously));
    }

    public override sealed Task<bool> ReadAsync()
    {
      this.VerifyCanRead(false);
      return this.ReadAsynchronously().FollowOnFaultWith<bool>((Action<Task<bool>>) (t => this.EnterScope(new ODataReaderCore.Scope(ODataReaderState.Exception, (ODataItem) null, (ODataUri) null))));
    }

    public override sealed Stream CreateReadStream()
    {
      if (this.State != ODataReaderState.Stream)
        throw new ODataException(Strings.ODataReaderCore_CreateReadStreamCalledInInvalidState);
      ODataReaderCore.StreamScope currentScope = this.CurrentScope as ODataReaderCore.StreamScope;
      currentScope.StreamingState = currentScope.StreamingState == ODataReaderCore.StreamingState.None ? ODataReaderCore.StreamingState.Streaming : throw new ODataException(Strings.ODataReaderCore_CreateReadStreamCalledInInvalidState);
      return (Stream) new ODataNotificationStream(this.InterceptException<Stream>(new Func<Stream>(this.CreateReadStreamImplementation)), (IODataStreamListener) this);
    }

    public override sealed TextReader CreateTextReader()
    {
      if (this.State != ODataReaderState.Stream)
        throw new ODataException(Strings.ODataReaderCore_CreateTextReaderCalledInInvalidState);
      ODataReaderCore.StreamScope currentScope = this.CurrentScope as ODataReaderCore.StreamScope;
      currentScope.StreamingState = currentScope.StreamingState == ODataReaderCore.StreamingState.None ? ODataReaderCore.StreamingState.Streaming : throw new ODataException(Strings.ODataReaderCore_CreateReadStreamCalledInInvalidState);
      return (TextReader) new ODataNotificationReader(this.InterceptException<TextReader>(new Func<TextReader>(this.CreateTextReaderImplementation)), (IODataStreamListener) this);
    }

    void IODataStreamListener.StreamRequested()
    {
    }

    Task IODataStreamListener.StreamRequestedAsync() => TaskUtils.GetTaskForSynchronousOperation((Action) (() => ((IODataStreamListener) this).StreamRequested()));

    void IODataStreamListener.StreamDisposed() => (this.CurrentScope as ODataReaderCore.StreamScope).StreamingState = ODataReaderCore.StreamingState.Completed;

    internal ODataReaderCore.Scope SeekScope<T>(int maxDepth) where T : ODataReaderCore.Scope
    {
      int num = 1;
      foreach (ODataReaderCore.Scope scope in this.scopes)
      {
        if (num > maxDepth)
          return (ODataReaderCore.Scope) null;
        if (scope is T)
          return scope;
        ++num;
      }
      return (ODataReaderCore.Scope) null;
    }

    protected abstract bool ReadAtStartImplementation();

    protected abstract bool ReadAtResourceSetStartImplementation();

    protected abstract bool ReadAtResourceSetEndImplementation();

    protected abstract bool ReadAtResourceStartImplementation();

    protected abstract bool ReadAtResourceEndImplementation();

    protected virtual bool ReadAtPrimitiveImplementation() => throw new NotImplementedException();

    protected virtual bool ReadAtNestedPropertyInfoImplementation() => throw new NotImplementedException();

    protected virtual bool ReadAtStreamImplementation() => throw new NotImplementedException();

    protected virtual Stream CreateReadStreamImplementation() => throw new NotImplementedException();

    protected virtual TextReader CreateTextReaderImplementation() => throw new NotImplementedException();

    protected abstract bool ReadAtNestedResourceInfoStartImplementation();

    protected abstract bool ReadAtNestedResourceInfoEndImplementation();

    protected abstract bool ReadAtEntityReferenceLink();

    protected virtual bool ReadAtDeltaResourceSetStartImplementation() => throw new NotImplementedException();

    protected virtual bool ReadAtDeltaResourceSetEndImplementation() => throw new NotImplementedException();

    protected virtual bool ReadAtDeletedResourceStartImplementation() => throw new NotImplementedException();

    protected virtual bool ReadAtDeletedResourceEndImplementation() => throw new NotImplementedException();

    protected virtual bool ReadAtDeltaLinkImplementation() => throw new NotImplementedException();

    protected virtual bool ReadAtDeltaDeletedLinkImplementation() => throw new NotImplementedException();

    protected void EnterScope(ODataReaderCore.Scope scope)
    {
      if ((scope.State == ODataReaderState.ResourceSetStart || scope.State == ODataReaderState.DeltaResourceSetStart) && this.inputContext.Model.IsUserModel())
        scope.ResourceTypeValidator = new ResourceSetWithoutExpectedTypeValidator(scope.ResourceType);
      if (scope.State == ODataReaderState.ResourceSetStart || scope.State == ODataReaderState.DeltaResourceSetStart)
        scope.DerivedTypeValidator = this.CurrentScope.DerivedTypeValidator;
      this.scopes.Push(scope);
      if (this.listener == null)
        return;
      if (scope.State == ODataReaderState.Exception)
      {
        this.listener.OnException();
      }
      else
      {
        if (scope.State != ODataReaderState.Completed)
          return;
        this.listener.OnCompleted();
      }
    }

    protected void ReplaceScope(ODataReaderCore.Scope scope)
    {
      this.scopes.Pop();
      this.EnterScope(scope);
    }

    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "state", Justification = "Used in debug builds in assertions.")]
    [SuppressMessage("Microsoft.Performance", "CA1804:RemoveUnusedLocals", MessageId = "scope", Justification = "Used in debug builds in assertions.")]
    protected void PopScope(ODataReaderState state) => this.scopes.Pop();

    protected void EndEntry(ODataReaderCore.Scope scope)
    {
      this.scopes.Pop();
      this.EnterScope(scope);
    }

    protected void ApplyResourceTypeNameFromPayload(string resourceTypeNameFromPayload)
    {
      ODataTypeAnnotation typeAnnotation;
      IEdmStructuredTypeReference targetType = (IEdmStructuredTypeReference) this.inputContext.MessageReaderSettings.Validator.ResolvePayloadTypeNameAndComputeTargetType(EdmTypeKind.None, new bool?(true), (IEdmType) null, this.CurrentResourceTypeReference, resourceTypeNameFromPayload, this.inputContext.Model, (Func<EdmTypeKind>) (() => EdmTypeKind.Entity), out EdmTypeKind _, out typeAnnotation);
      ODataResourceBase odataResourceBase = this.Item as ODataResourceBase;
      if (targetType != null)
      {
        IEdmStructuredType type = targetType.StructuredDefinition();
        odataResourceBase.TypeName = type.FullTypeName();
        if (typeAnnotation != null)
          odataResourceBase.TypeAnnotation = typeAnnotation;
      }
      else if (resourceTypeNameFromPayload != null)
        odataResourceBase.TypeName = resourceTypeNameFromPayload;
      this.CurrentResourceTypeReference = (IEdmTypeReference) targetType;
    }

    protected bool ReadSynchronously() => this.ReadImplementation();

    protected virtual Task<bool> ReadAsynchronously() => TaskUtils.GetTaskForSynchronousOperation<bool>(new Func<bool>(this.ReadImplementation));

    protected void IncreaseResourceDepth()
    {
      ++this.currentResourceDepth;
      if (this.currentResourceDepth > this.inputContext.MessageReaderSettings.MessageQuotas.MaxNestingDepth)
        throw new ODataException(Strings.ValidationUtils_MaxDepthOfNestedEntriesExceeded((object) this.inputContext.MessageReaderSettings.MessageQuotas.MaxNestingDepth));
    }

    protected void DecreaseResourceDepth() => --this.currentResourceDepth;

    private bool ReadImplementation()
    {
      switch (this.State)
      {
        case ODataReaderState.Start:
          return this.ReadAtStartImplementation();
        case ODataReaderState.ResourceSetStart:
          return this.ReadAtResourceSetStartImplementation();
        case ODataReaderState.ResourceSetEnd:
          return this.ReadAtResourceSetEndImplementation();
        case ODataReaderState.ResourceStart:
          this.IncreaseResourceDepth();
          return this.ReadAtResourceStartImplementation();
        case ODataReaderState.ResourceEnd:
          this.DecreaseResourceDepth();
          return this.ReadAtResourceEndImplementation();
        case ODataReaderState.NestedResourceInfoStart:
          return this.ReadAtNestedResourceInfoStartImplementation();
        case ODataReaderState.NestedResourceInfoEnd:
          return this.ReadAtNestedResourceInfoEndImplementation();
        case ODataReaderState.EntityReferenceLink:
          return this.ReadAtEntityReferenceLink();
        case ODataReaderState.Exception:
        case ODataReaderState.Completed:
          throw new ODataException(Strings.ODataReaderCore_NoReadCallsAllowed((object) this.State));
        case ODataReaderState.Primitive:
          return this.ReadAtPrimitiveImplementation();
        case ODataReaderState.DeltaResourceSetStart:
          return this.ReadAtDeltaResourceSetStartImplementation();
        case ODataReaderState.DeltaResourceSetEnd:
          return this.ReadAtDeltaResourceSetEndImplementation();
        case ODataReaderState.DeletedResourceStart:
          return this.ReadAtDeletedResourceStartImplementation();
        case ODataReaderState.DeletedResourceEnd:
          return this.ReadAtDeletedResourceEndImplementation();
        case ODataReaderState.DeltaLink:
          return this.ReadAtDeltaLinkImplementation();
        case ODataReaderState.DeltaDeletedLink:
          return this.ReadAtDeltaDeletedLinkImplementation();
        case ODataReaderState.NestedProperty:
          return this.ReadAtNestedPropertyInfoImplementation();
        case ODataReaderState.Stream:
          return this.ReadAtStreamImplementation();
        default:
          throw new ODataException(Strings.General_InternalError((object) InternalErrorCodes.ODataReaderCore_ReadImplementation));
      }
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
          this.EnterScope(new ODataReaderCore.Scope(ODataReaderState.Exception, (ODataItem) null, (ODataUri) null));
        throw;
      }
    }

    private void VerifyCanRead(bool synchronousCall)
    {
      this.inputContext.VerifyNotDisposed();
      this.VerifyCallAllowed(synchronousCall);
      if (this.State == ODataReaderState.Exception || this.State == ODataReaderState.Completed)
        throw new ODataException(Strings.ODataReaderCore_ReadOrReadAsyncCalledInInvalidState((object) this.State));
      if (this.State == ODataReaderState.Stream && (this.CurrentScope as ODataReaderCore.StreamScope).StreamingState != ODataReaderCore.StreamingState.Completed)
        throw new ODataException(Strings.ODataReaderCore_ReadCalledWithOpenStream);
    }

    private void VerifyCallAllowed(bool synchronousCall)
    {
      if (synchronousCall)
      {
        if (!this.inputContext.Synchronous)
          throw new ODataException(Strings.ODataReaderCore_SyncCallOnAsyncReader);
      }
      else if (this.inputContext.Synchronous)
        throw new ODataException(Strings.ODataReaderCore_AsyncCallOnSyncReader);
    }

    internal enum StreamingState
    {
      None,
      Streaming,
      Completed,
    }

    protected internal class Scope
    {
      private readonly ODataReaderState state;
      private readonly ODataItem item;
      private readonly ODataUri odataUri;
      private ResourceSetWithoutExpectedTypeValidator resourceTypeValidator;

      [SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily", Justification = "Debug.Assert check only.")]
      internal Scope(ODataReaderState state, ODataItem item, ODataUri odataUri)
      {
        this.state = state;
        this.item = item;
        this.odataUri = odataUri;
      }

      [SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily", Justification = "Debug.Assert check only.")]
      internal Scope(
        ODataReaderState state,
        ODataItem item,
        IEdmNavigationSource navigationSource,
        IEdmTypeReference expectedResourceTypeReference,
        ODataUri odataUri)
        : this(state, item, odataUri)
      {
        this.NavigationSource = navigationSource;
        this.ResourceTypeReference = expectedResourceTypeReference;
      }

      internal ODataReaderState State => this.state;

      internal ODataItem Item => this.item;

      internal ODataUri ODataUri => this.odataUri;

      internal IEdmNavigationSource NavigationSource { get; set; }

      internal IEdmType ResourceType => this.ResourceTypeReference != null ? this.ResourceTypeReference.Definition : (IEdmType) null;

      internal IEdmTypeReference ResourceTypeReference { get; set; }

      internal ResourceSetWithoutExpectedTypeValidator ResourceTypeValidator
      {
        get => this.resourceTypeValidator;
        set => this.resourceTypeValidator = value;
      }

      internal DerivedTypeValidator DerivedTypeValidator { get; set; }
    }

    protected internal class StreamScope : ODataReaderCore.Scope
    {
      internal StreamScope(
        ODataReaderState state,
        ODataItem item,
        IEdmNavigationSource navigationSource,
        IEdmTypeReference expectedResourceType,
        ODataUri odataUri)
        : base(state, item, navigationSource, expectedResourceType, odataUri)
      {
        this.StreamingState = ODataReaderCore.StreamingState.None;
      }

      internal ODataReaderCore.StreamingState StreamingState { get; set; }
    }
  }
}
