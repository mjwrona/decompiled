// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.ODataWriterCore
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Vocabularies;
using Microsoft.OData.Evaluation;
using Microsoft.OData.Metadata;
using Microsoft.OData.UriParser;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.OData
{
  internal abstract class ODataWriterCore : 
    ODataWriter,
    IODataOutputInStreamErrorListener,
    IODataStreamListener
  {
    protected readonly IWriterValidator WriterValidator;
    private readonly ODataOutputContext outputContext;
    private readonly bool writingResourceSet;
    private readonly bool writingDelta;
    private readonly IODataReaderWriterListener listener;
    private readonly ODataWriterCore.ScopeStack scopeStack = new ODataWriterCore.ScopeStack();
    private int currentResourceDepth;

    protected ODataWriterCore(
      ODataOutputContext outputContext,
      IEdmNavigationSource navigationSource,
      IEdmStructuredType resourceType,
      bool writingResourceSet,
      bool writingDelta = false,
      IODataReaderWriterListener listener = null)
    {
      this.outputContext = outputContext;
      this.writingResourceSet = writingResourceSet;
      this.writingDelta = writingDelta || !writingResourceSet && !outputContext.WritingResponse;
      this.WriterValidator = outputContext.WriterValidator;
      this.Version = outputContext.MessageWriterSettings.Version;
      if (navigationSource != null && resourceType == null)
        resourceType = (IEdmStructuredType) this.outputContext.EdmTypeResolver.GetElementType(navigationSource);
      ODataUri odataUri = outputContext.MessageWriterSettings.ODataUri.Clone();
      if (!writingResourceSet && odataUri != null && odataUri.Path != null)
        odataUri.Path = odataUri.Path.TrimEndingKeySegment();
      this.listener = listener;
      this.scopeStack.Push(new ODataWriterCore.Scope(ODataWriterCore.WriterState.Start, (ODataItem) null, navigationSource, (IEdmType) resourceType, false, outputContext.MessageWriterSettings.SelectedProperties, odataUri));
      this.CurrentScope.DerivedTypeConstraints = this.outputContext.Model.GetDerivedTypeConstraints(navigationSource);
    }

    internal ODataVersion? Version { get; private set; }

    protected ODataWriterCore.Scope CurrentScope => this.scopeStack.Peek();

    protected ODataWriterCore.WriterState State => this.CurrentScope.State;

    protected bool SkipWriting => this.CurrentScope.SkipWriting;

    protected bool IsTopLevel => this.scopeStack.Count == 2;

    protected int ScopeLevel => this.scopeStack.Count;

    protected ODataNestedResourceInfo ParentNestedResourceInfo
    {
      get
      {
        ODataWriterCore.Scope parentOrNull = this.scopeStack.ParentOrNull;
        return parentOrNull != null ? parentOrNull.Item as ODataNestedResourceInfo : (ODataNestedResourceInfo) null;
      }
    }

    protected ODataNestedResourceInfo BelongingNestedResourceInfo
    {
      get
      {
        ODataWriterCore.Scope parentOrNull = this.scopeStack.ParentOrNull;
        switch (parentOrNull)
        {
          case ODataWriterCore.NestedResourceInfoScope _:
            return parentOrNull.Item as ODataNestedResourceInfo;
          case ODataWriterCore.ResourceSetBaseScope _:
            ODataWriterCore.Scope parentOfParent = this.scopeStack.ParentOfParent;
            return parentOfParent != null ? parentOfParent.Item as ODataNestedResourceInfo : (ODataNestedResourceInfo) null;
          default:
            return (ODataNestedResourceInfo) null;
        }
      }
    }

    protected IEdmStructuredType ParentResourceType => this.scopeStack.Parent.ResourceType;

    protected IEdmNavigationSource ParentResourceNavigationSource => this.scopeStack.Parent?.NavigationSource;

    protected ODataWriterCore.Scope ParentScope => this.scopeStack.Scopes.Skip<ODataWriterCore.Scope>(1).First<ODataWriterCore.Scope>();

    protected int ResourceSetScopeResourceCount => ((ODataWriterCore.ResourceSetBaseScope) this.CurrentScope).ResourceCount;

    protected IDuplicatePropertyNameChecker DuplicatePropertyNameChecker
    {
      get
      {
        ODataWriterCore.ResourceBaseScope resourceBaseScope;
        switch (this.State)
        {
          case ODataWriterCore.WriterState.Resource:
          case ODataWriterCore.WriterState.DeletedResource:
            resourceBaseScope = (ODataWriterCore.ResourceBaseScope) this.CurrentScope;
            break;
          case ODataWriterCore.WriterState.NestedResourceInfo:
          case ODataWriterCore.WriterState.NestedResourceInfoWithContent:
          case ODataWriterCore.WriterState.Property:
            resourceBaseScope = (ODataWriterCore.ResourceBaseScope) this.scopeStack.Parent;
            break;
          default:
            throw new ODataException(Strings.General_InternalError((object) InternalErrorCodes.ODataWriterCore_PropertyAndAnnotationCollector));
        }
        return resourceBaseScope.DuplicatePropertyNameChecker;
      }
    }

    protected IEdmStructuredType ResourceType => this.CurrentScope.ResourceType;

    protected ODataWriterCore.NestedResourceInfoScope ParentNestedResourceInfoScope
    {
      get
      {
        ODataWriterCore.Scope scope = this.scopeStack.Parent;
        if (scope.State == ODataWriterCore.WriterState.Start)
          return (ODataWriterCore.NestedResourceInfoScope) null;
        if (scope.State == ODataWriterCore.WriterState.ResourceSet || scope.State == ODataWriterCore.WriterState.DeltaResourceSet)
        {
          scope = this.scopeStack.ParentOfParent;
          if (scope.State == ODataWriterCore.WriterState.Start || scope.State == ODataWriterCore.WriterState.ResourceSet && scope.ResourceType != null && scope.ResourceType.TypeKind == EdmTypeKind.Untyped)
            return (ODataWriterCore.NestedResourceInfoScope) null;
        }
        return scope.State == ODataWriterCore.WriterState.NestedResourceInfoWithContent ? (ODataWriterCore.NestedResourceInfoScope) scope : throw new ODataException(Strings.General_InternalError((object) InternalErrorCodes.ODataWriterCore_ParentNestedResourceInfoScope));
      }
    }

    private ResourceSetWithoutExpectedTypeValidator CurrentResourceSetValidator => this.ParentScope is ODataWriterCore.ResourceSetBaseScope parentScope ? parentScope.ResourceTypeValidator : (ResourceSetWithoutExpectedTypeValidator) null;

    public override sealed void Flush()
    {
      this.VerifyCanFlush(true);
      try
      {
        this.FlushSynchronously();
      }
      catch
      {
        this.EnterScope(ODataWriterCore.WriterState.Error, (ODataItem) null);
        throw;
      }
    }

    public override sealed Task FlushAsync()
    {
      this.VerifyCanFlush(false);
      return this.FlushAsynchronously().FollowOnFaultWith((Action<Task>) (t => this.EnterScope(ODataWriterCore.WriterState.Error, (ODataItem) null)));
    }

    public override sealed void WriteStart(ODataResourceSet resourceSet)
    {
      this.VerifyCanWriteStartResourceSet(true, resourceSet);
      this.WriteStartResourceSetImplementation(resourceSet);
    }

    public override sealed Task WriteStartAsync(ODataResourceSet resourceSet)
    {
      this.VerifyCanWriteStartResourceSet(false, resourceSet);
      return TaskUtils.GetTaskForSynchronousOperation((Action) (() => this.WriteStartResourceSetImplementation(resourceSet)));
    }

    public override sealed void WriteStart(ODataDeltaResourceSet deltaResourceSet)
    {
      this.VerifyCanWriteStartDeltaResourceSet(true, deltaResourceSet);
      this.WriteStartDeltaResourceSetImplementation(deltaResourceSet);
    }

    public override sealed Task WriteStartAsync(ODataDeltaResourceSet deltaResourceSet)
    {
      this.VerifyCanWriteStartDeltaResourceSet(false, deltaResourceSet);
      return TaskUtils.GetTaskForSynchronousOperation((Action) (() => this.WriteStartDeltaResourceSetImplementation(deltaResourceSet)));
    }

    public override sealed void WriteStart(ODataResource resource)
    {
      this.VerifyCanWriteStartResource(true, resource);
      this.WriteStartResourceImplementation(resource);
    }

    public override sealed Task WriteStartAsync(ODataResource resource)
    {
      this.VerifyCanWriteStartResource(false, resource);
      return TaskUtils.GetTaskForSynchronousOperation((Action) (() => this.WriteStartResourceImplementation(resource)));
    }

    public override sealed void WriteStart(ODataDeletedResource deletedResource)
    {
      this.VerifyCanWriteStartDeletedResource(true, deletedResource);
      this.WriteStartDeletedResourceImplementation(deletedResource);
    }

    public override sealed Task WriteStartAsync(ODataDeletedResource deletedResource)
    {
      this.VerifyCanWriteStartDeletedResource(false, deletedResource);
      return TaskUtils.GetTaskForSynchronousOperation((Action) (() => this.WriteStartDeletedResourceImplementation(deletedResource)));
    }

    public override void WriteDeltaLink(ODataDeltaLink deltaLink)
    {
      this.VerifyCanWriteLink(true, (ODataDeltaLinkBase) deltaLink);
      this.WriteDeltaLinkImplementation((ODataDeltaLinkBase) deltaLink);
    }

    public override Task WriteDeltaLinkAsync(ODataDeltaLink deltaLink)
    {
      this.VerifyCanWriteLink(false, (ODataDeltaLinkBase) deltaLink);
      return TaskUtils.GetTaskForSynchronousOperation((Action) (() => this.WriteDeltaLinkImplementation((ODataDeltaLinkBase) deltaLink)));
    }

    public override void WriteDeltaDeletedLink(ODataDeltaDeletedLink deltaLink)
    {
      this.VerifyCanWriteLink(true, (ODataDeltaLinkBase) deltaLink);
      this.WriteDeltaLinkImplementation((ODataDeltaLinkBase) deltaLink);
    }

    public override Task WriteDeltaDeletedLinkAsync(ODataDeltaDeletedLink deltaLink)
    {
      this.VerifyCanWriteLink(false, (ODataDeltaLinkBase) deltaLink);
      return TaskUtils.GetTaskForSynchronousOperation((Action) (() => this.WriteDeltaLinkImplementation((ODataDeltaLinkBase) deltaLink)));
    }

    public override sealed void WritePrimitive(ODataPrimitiveValue primitiveValue)
    {
      this.VerifyCanWritePrimitive(true, primitiveValue);
      this.WritePrimitiveValueImplementation(primitiveValue);
    }

    public override sealed Task WritePrimitiveAsync(ODataPrimitiveValue primitiveValue)
    {
      this.VerifyCanWritePrimitive(false, primitiveValue);
      return TaskUtils.GetTaskForSynchronousOperation((Action) (() => this.WritePrimitiveValueImplementation(primitiveValue)));
    }

    public override sealed void WriteStart(ODataPropertyInfo primitiveProperty)
    {
      this.VerifyCanWriteProperty(true, primitiveProperty);
      this.WriteStartPropertyImplementation(primitiveProperty);
    }

    public override sealed Task WriteStartAsync(ODataProperty primitiveProperty)
    {
      this.VerifyCanWriteProperty(false, (ODataPropertyInfo) primitiveProperty);
      return TaskUtils.GetTaskForSynchronousOperation((Action) (() => this.WriteStartPropertyImplementation((ODataPropertyInfo) primitiveProperty)));
    }

    public override sealed Stream CreateBinaryWriteStream()
    {
      this.VerifyCanCreateWriteStream(true);
      return this.CreateWriteStreamImplementation();
    }

    public override sealed Task<Stream> CreateBinaryWriteStreamAsync()
    {
      this.VerifyCanCreateWriteStream(false);
      return TaskUtils.GetTaskForSynchronousOperation<Stream>((Func<Stream>) (() => this.CreateWriteStreamImplementation()));
    }

    public override sealed TextWriter CreateTextWriter()
    {
      this.VerifyCanCreateTextWriter(true);
      return this.CreateTextWriterImplementation();
    }

    public override sealed Task<TextWriter> CreateTextWriterAsync()
    {
      this.VerifyCanCreateWriteStream(false);
      return TaskUtils.GetTaskForSynchronousOperation<TextWriter>((Func<TextWriter>) (() => this.CreateTextWriterImplementation()));
    }

    public override sealed void WriteStart(ODataNestedResourceInfo nestedResourceInfo)
    {
      this.VerifyCanWriteStartNestedResourceInfo(true, nestedResourceInfo);
      this.WriteStartNestedResourceInfoImplementation(nestedResourceInfo);
    }

    public override sealed Task WriteStartAsync(ODataNestedResourceInfo nestedResourceInfo)
    {
      this.VerifyCanWriteStartNestedResourceInfo(false, nestedResourceInfo);
      return TaskUtils.GetTaskForSynchronousOperation((Action) (() => this.WriteStartNestedResourceInfoImplementation(nestedResourceInfo)));
    }

    public override sealed void WriteEnd()
    {
      this.VerifyCanWriteEnd(true);
      this.WriteEndImplementation();
      if (this.CurrentScope.State != ODataWriterCore.WriterState.Completed)
        return;
      this.Flush();
    }

    public override sealed Task WriteEndAsync()
    {
      this.VerifyCanWriteEnd(false);
      return TaskUtils.GetTaskForSynchronousOperation(new Action(this.WriteEndImplementation)).FollowOnSuccessWithTask((Func<Task, Task>) (task => this.CurrentScope.State == ODataWriterCore.WriterState.Completed ? this.FlushAsync() : TaskUtils.CompletedTask));
    }

    public override sealed void WriteEntityReferenceLink(
      ODataEntityReferenceLink entityReferenceLink)
    {
      this.VerifyCanWriteEntityReferenceLink(entityReferenceLink, true);
      this.WriteEntityReferenceLinkImplementation(entityReferenceLink);
    }

    public override sealed Task WriteEntityReferenceLinkAsync(
      ODataEntityReferenceLink entityReferenceLink)
    {
      this.VerifyCanWriteEntityReferenceLink(entityReferenceLink, false);
      return TaskUtils.GetTaskForSynchronousOperation((Action) (() => this.WriteEntityReferenceLinkImplementation(entityReferenceLink)));
    }

    void IODataOutputInStreamErrorListener.OnInStreamError()
    {
      this.VerifyNotDisposed();
      if (this.State == ODataWriterCore.WriterState.Completed)
        throw new ODataException(Strings.ODataWriterCore_InvalidTransitionFromCompleted((object) this.State.ToString(), (object) ODataWriterCore.WriterState.Error.ToString()));
      this.StartPayloadInStartState();
      this.EnterScope(ODataWriterCore.WriterState.Error, this.CurrentScope.Item);
    }

    void IODataStreamListener.StreamRequested()
    {
    }

    Task IODataStreamListener.StreamRequestedAsync() => TaskUtils.GetTaskForSynchronousOperation((Action) (() => ((IODataStreamListener) this).StreamRequested()));

    void IODataStreamListener.StreamDisposed()
    {
      if (this.State == ODataWriterCore.WriterState.Stream)
        this.EndBinaryStream();
      else if (this.State == ODataWriterCore.WriterState.String)
        this.EndTextWriter();
      this.LeaveScope();
    }

    protected ODataWriterCore.ResourceScope GetParentResourceScope()
    {
      ODataWriterCore.ScopeStack scopeStack = new ODataWriterCore.ScopeStack();
      ODataWriterCore.Scope parentResourceScope = (ODataWriterCore.Scope) null;
      if (this.scopeStack.Count > 0)
        scopeStack.Push(this.scopeStack.Pop());
      while (this.scopeStack.Count > 0)
      {
        ODataWriterCore.Scope scope = this.scopeStack.Pop();
        scopeStack.Push(scope);
        if (scope is ODataWriterCore.ResourceScope)
        {
          parentResourceScope = scope;
          break;
        }
      }
      while (scopeStack.Count > 0)
        this.scopeStack.Push(scopeStack.Pop());
      return parentResourceScope as ODataWriterCore.ResourceScope;
    }

    protected static bool IsErrorState(ODataWriterCore.WriterState state) => state == ODataWriterCore.WriterState.Error;

    protected abstract void VerifyNotDisposed();

    protected abstract void FlushSynchronously();

    protected abstract Task FlushAsynchronously();

    protected abstract void StartPayload();

    protected abstract void StartResource(ODataResource resource);

    protected abstract void EndResource(ODataResource resource);

    protected virtual void StartProperty(ODataPropertyInfo property) => throw new NotImplementedException();

    protected virtual void EndProperty(ODataPropertyInfo property) => throw new NotImplementedException();

    protected abstract void StartResourceSet(ODataResourceSet resourceSet);

    protected virtual void StartDeltaResourceSet(ODataDeltaResourceSet deltaResourceSet) => throw new NotImplementedException();

    protected virtual void StartDeletedResource(ODataDeletedResource deletedEntry) => throw new NotImplementedException();

    protected virtual void StartDeltaLink(ODataDeltaLinkBase deltaLink) => throw new NotImplementedException();

    protected virtual Stream StartBinaryStream() => throw new NotImplementedException();

    protected virtual void EndBinaryStream() => throw new NotImplementedException();

    protected virtual TextWriter StartTextWriter() => throw new NotImplementedException();

    protected virtual void EndTextWriter() => throw new NotImplementedException();

    protected abstract void EndPayload();

    protected abstract void EndResourceSet(ODataResourceSet resourceSet);

    protected virtual void EndDeltaResourceSet(ODataDeltaResourceSet deltaResourceSet) => throw new NotImplementedException();

    protected virtual void EndDeletedResource(ODataDeletedResource deletedResource) => throw new NotImplementedException();

    protected virtual void WritePrimitiveValue(ODataPrimitiveValue primitiveValue) => throw new NotImplementedException();

    protected abstract void WriteDeferredNestedResourceInfo(
      ODataNestedResourceInfo nestedResourceInfo);

    protected abstract void StartNestedResourceInfoWithContent(
      ODataNestedResourceInfo nestedResourceInfo);

    protected abstract void EndNestedResourceInfoWithContent(
      ODataNestedResourceInfo nestedResourceInfo);

    protected abstract void WriteEntityReferenceInNavigationLinkContent(
      ODataNestedResourceInfo parentNestedResourceInfo,
      ODataEntityReferenceLink entityReferenceLink);

    protected abstract ODataWriterCore.ResourceSetScope CreateResourceSetScope(
      ODataResourceSet resourceSet,
      IEdmNavigationSource navigationSource,
      IEdmType itemType,
      bool skipWriting,
      SelectedPropertiesNode selectedProperties,
      ODataUri odataUri,
      bool isUndeclared);

    protected virtual ODataWriterCore.DeltaResourceSetScope CreateDeltaResourceSetScope(
      ODataDeltaResourceSet deltaResourceSet,
      IEdmNavigationSource navigationSource,
      IEdmStructuredType resourceType,
      bool skipWriting,
      SelectedPropertiesNode selectedProperties,
      ODataUri odataUri,
      bool isUndeclared)
    {
      throw new NotImplementedException();
    }

    protected abstract ODataWriterCore.ResourceScope CreateResourceScope(
      ODataResource resource,
      IEdmNavigationSource navigationSource,
      IEdmStructuredType resourceType,
      bool skipWriting,
      SelectedPropertiesNode selectedProperties,
      ODataUri odataUri,
      bool isUndeclared);

    protected virtual ODataWriterCore.DeletedResourceScope CreateDeletedResourceScope(
      ODataDeletedResource resource,
      IEdmNavigationSource navigationSource,
      IEdmEntityType resourceType,
      bool skipWriting,
      SelectedPropertiesNode selectedProperties,
      ODataUri odataUri,
      bool isUndeclared)
    {
      throw new NotImplementedException();
    }

    protected virtual ODataWriterCore.PropertyInfoScope CreatePropertyInfoScope(
      ODataPropertyInfo property,
      IEdmNavigationSource navigationSource,
      IEdmStructuredType resourceType,
      SelectedPropertiesNode selectedProperties,
      ODataUri odataUri)
    {
      throw new NotImplementedException();
    }

    protected virtual ODataWriterCore.DeltaLinkScope CreateDeltaLinkScope(
      ODataDeltaLinkBase link,
      IEdmNavigationSource navigationSource,
      IEdmEntityType entityType,
      SelectedPropertiesNode selectedProperties,
      ODataUri odataUri)
    {
      throw new NotImplementedException();
    }

    protected ODataResourceSerializationInfo GetResourceSerializationInfo(ODataResourceBase resource)
    {
      ODataResourceSerializationInfo serializationInfo = resource == null ? (ODataResourceSerializationInfo) null : resource.SerializationInfo;
      if (serializationInfo != null)
        return serializationInfo;
      return this.CurrentScope.Item is ODataResourceSetBase odataResourceSetBase ? odataResourceSetBase.SerializationInfo : (ODataResourceSerializationInfo) null;
    }

    protected ODataResourceSerializationInfo GetLinkSerializationInfo(ODataItem item)
    {
      ODataDeltaSerializationInfo serializationInfo1 = (ODataDeltaSerializationInfo) null;
      ODataResourceSerializationInfo serializationInfo2 = (ODataResourceSerializationInfo) null;
      if (item is ODataDeltaLink odataDeltaLink)
        serializationInfo1 = odataDeltaLink.SerializationInfo;
      if (item is ODataDeltaDeletedLink deltaDeletedLink)
        serializationInfo1 = deltaDeletedLink.SerializationInfo;
      if (serializationInfo1 == null)
      {
        if (this.CurrentScope is ODataWriterCore.DeltaResourceSetScope currentScope)
        {
          ODataResourceSerializationInfo serializationInfo3 = ((ODataResourceSetBase) currentScope.Item).SerializationInfo;
          if (serializationInfo3 != null)
            serializationInfo2 = serializationInfo3;
        }
      }
      else
        serializationInfo2 = new ODataResourceSerializationInfo()
        {
          NavigationSourceName = serializationInfo1.NavigationSourceName
        };
      return serializationInfo2;
    }

    protected virtual ODataWriterCore.NestedResourceInfoScope CreateNestedResourceInfoScope(
      ODataWriterCore.WriterState writerState,
      ODataNestedResourceInfo navLink,
      IEdmNavigationSource navigationSource,
      IEdmType itemType,
      bool skipWriting,
      SelectedPropertiesNode selectedProperties,
      ODataUri odataUri)
    {
      return new ODataWriterCore.NestedResourceInfoScope(writerState, navLink, navigationSource, itemType, skipWriting, selectedProperties, odataUri);
    }

    protected virtual void PrepareResourceForWriteStart(
      ODataWriterCore.ResourceScope resourceScope,
      ODataResource resource,
      bool writingResponse,
      SelectedPropertiesNode selectedProperties)
    {
    }

    protected virtual void PrepareDeletedResourceForWriteStart(
      ODataWriterCore.DeletedResourceScope resourceScope,
      ODataDeletedResource deletedResource,
      bool writingResponse,
      SelectedPropertiesNode selectedProperties)
    {
    }

    protected IEdmStructuredType GetResourceType(ODataResourceBase resource) => TypeNameOracle.ResolveAndValidateTypeFromTypeName(this.outputContext.Model, this.CurrentScope.ResourceType, resource.TypeName, this.WriterValidator);

    protected IEdmStructuredType GetResourceSetType(ODataResourceSetBase resourceSet) => TypeNameOracle.ResolveAndValidateTypeFromTypeName(this.outputContext.Model, this.CurrentScope.ResourceType, EdmLibraryExtensions.GetCollectionItemTypeName(resourceSet.TypeName), this.WriterValidator);

    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "An instance field is used in a debug assert.")]
    protected void ValidateNoDeltaLinkForExpandedResourceSet(ODataResourceSet resourceSet)
    {
      if (resourceSet.DeltaLink != (Uri) null)
        throw new ODataException(Strings.ODataWriterCore_DeltaLinkNotSupportedOnExpandedResourceSet);
    }

    protected bool ShouldOmitNullValues() => !this.writingDelta && this.outputContext.ShouldOmitNullValues();

    private void VerifyCanWriteStartResourceSet(bool synchronousCall, ODataResourceSet resourceSet)
    {
      ExceptionUtils.CheckArgumentNotNull<ODataResourceSet>(resourceSet, nameof (resourceSet));
      this.VerifyNotDisposed();
      this.VerifyCallAllowed(synchronousCall);
      this.StartPayloadInStartState();
    }

    private void WriteStartResourceSetImplementation(ODataResourceSet resourceSet)
    {
      this.CheckForNestedResourceInfoWithContent(ODataPayloadKind.ResourceSet, (ODataItem) resourceSet);
      this.EnterScope(ODataWriterCore.WriterState.ResourceSet, (ODataItem) resourceSet);
      if (this.SkipWriting)
        return;
      this.InterceptException((Action) (() =>
      {
        if (resourceSet.Count.HasValue && !this.outputContext.WritingResponse)
          this.ThrowODataException(Strings.ODataWriterCore_QueryCountInRequest, (ODataItem) resourceSet);
        this.StartResourceSet(resourceSet);
      }));
    }

    private void VerifyCanWriteStartDeltaResourceSet(
      bool synchronousCall,
      ODataDeltaResourceSet deltaResourceSet)
    {
      ExceptionUtils.CheckArgumentNotNull<ODataDeltaResourceSet>(deltaResourceSet, nameof (deltaResourceSet));
      this.VerifyWritingDelta();
      this.VerifyNotDisposed();
      this.VerifyCallAllowed(synchronousCall);
      this.StartPayloadInStartState();
    }

    private void WriteStartDeltaResourceSetImplementation(ODataDeltaResourceSet deltaResourceSet)
    {
      this.CheckForNestedResourceInfoWithContent(ODataPayloadKind.ResourceSet, (ODataItem) deltaResourceSet);
      this.EnterScope(ODataWriterCore.WriterState.DeltaResourceSet, (ODataItem) deltaResourceSet);
      this.InterceptException((Action) (() =>
      {
        if (!this.outputContext.WritingResponse)
        {
          if (deltaResourceSet.NextPageLink != (Uri) null)
            this.ThrowODataException(Strings.ODataWriterCore_QueryNextLinkInRequest, (ODataItem) deltaResourceSet);
          if (deltaResourceSet.DeltaLink != (Uri) null)
            this.ThrowODataException(Strings.ODataWriterCore_QueryDeltaLinkInRequest, (ODataItem) deltaResourceSet);
        }
        this.StartDeltaResourceSet(deltaResourceSet);
      }));
    }

    private void VerifyCanWriteStartResource(bool synchronousCall, ODataResource resource)
    {
      this.VerifyNotDisposed();
      this.VerifyCallAllowed(synchronousCall);
    }

    private void VerifyCanWriteStartDeletedResource(
      bool synchronousCall,
      ODataDeletedResource resource)
    {
      ExceptionUtils.CheckArgumentNotNull<ODataDeletedResource>(resource, nameof (resource));
      this.VerifyWritingDelta();
      this.VerifyNotDisposed();
      this.VerifyCallAllowed(synchronousCall);
    }

    private void WriteStartResourceImplementation(ODataResource resource)
    {
      this.StartPayloadInStartState();
      this.CheckForNestedResourceInfoWithContent(ODataPayloadKind.Resource, (ODataItem) resource);
      this.EnterScope(ODataWriterCore.WriterState.Resource, (ODataItem) resource);
      if (this.SkipWriting)
        return;
      this.IncreaseResourceDepth();
      this.InterceptException((Action) (() =>
      {
        if (resource != null)
        {
          ODataWriterCore.ResourceScope currentScope = (ODataWriterCore.ResourceScope) this.CurrentScope;
          this.ValidateResourceForResourceSet((ODataResourceBase) resource, (ODataWriterCore.ResourceBaseScope) currentScope);
          this.PrepareResourceForWriteStart(currentScope, resource, this.outputContext.WritingResponse, currentScope.SelectedProperties);
        }
        this.StartResource(resource);
      }));
    }

    private void WriteStartDeletedResourceImplementation(ODataDeletedResource resource)
    {
      this.StartPayloadInStartState();
      this.CheckForNestedResourceInfoWithContent(ODataPayloadKind.Resource, (ODataItem) resource);
      this.EnterScope(ODataWriterCore.WriterState.DeletedResource, (ODataItem) resource);
      this.IncreaseResourceDepth();
      this.InterceptException((Action) (() =>
      {
        ODataWriterCore.DeletedResourceScope currentScope = this.CurrentScope as ODataWriterCore.DeletedResourceScope;
        this.ValidateResourceForResourceSet((ODataResourceBase) resource, (ODataWriterCore.ResourceBaseScope) currentScope);
        this.PrepareDeletedResourceForWriteStart(currentScope, resource, this.outputContext.WritingResponse, currentScope.SelectedProperties);
        this.StartDeletedResource(resource);
      }));
    }

    private void VerifyCanWriteProperty(bool synchronousCall, ODataPropertyInfo property)
    {
      ExceptionUtils.CheckArgumentNotNull<ODataPropertyInfo>(property, nameof (property));
      this.VerifyNotDisposed();
      this.VerifyCallAllowed(synchronousCall);
    }

    private void WriteStartPropertyImplementation(ODataPropertyInfo property)
    {
      this.EnterScope(ODataWriterCore.WriterState.Property, (ODataItem) property);
      if (this.SkipWriting)
        return;
      this.InterceptException((Action) (() =>
      {
        this.StartProperty(property);
        if (!(property is ODataProperty))
          return;
        (this.CurrentScope as ODataWriterCore.PropertyInfoScope).ValueWritten = true;
      }));
    }

    private void WriteDeltaLinkImplementation(ODataDeltaLinkBase deltaLink)
    {
      this.EnterScope(deltaLink is ODataDeltaLink ? ODataWriterCore.WriterState.DeltaLink : ODataWriterCore.WriterState.DeltaDeletedLink, (ODataItem) deltaLink);
      this.StartDeltaLink(deltaLink);
      this.WriteEnd();
    }

    private void VerifyCanWriteStartNestedResourceInfo(
      bool synchronousCall,
      ODataNestedResourceInfo nestedResourceInfo)
    {
      ExceptionUtils.CheckArgumentNotNull<ODataNestedResourceInfo>(nestedResourceInfo, nameof (nestedResourceInfo));
      this.VerifyNotDisposed();
      this.VerifyCallAllowed(synchronousCall);
    }

    private void WriteStartNestedResourceInfoImplementation(
      ODataNestedResourceInfo nestedResourceInfo)
    {
      this.EnterScope(ODataWriterCore.WriterState.NestedResourceInfo, (ODataItem) nestedResourceInfo);
      ODataResourceBase odataResourceBase = (ODataResourceBase) this.scopeStack.Parent.Item;
      if (odataResourceBase.MetadataBuilder == null)
        return;
      nestedResourceInfo.MetadataBuilder = odataResourceBase.MetadataBuilder;
    }

    private void VerifyCanWritePrimitive(bool synchronousCall, ODataPrimitiveValue primitiveValue)
    {
      this.VerifyNotDisposed();
      this.VerifyCallAllowed(synchronousCall);
    }

    private void WritePrimitiveValueImplementation(ODataPrimitiveValue primitiveValue) => this.InterceptException((Action) (() =>
    {
      this.EnterScope(ODataWriterCore.WriterState.Primitive, (ODataItem) primitiveValue);
      if (this.CurrentResourceSetValidator != null && primitiveValue != null)
        this.CurrentResourceSetValidator.ValidateResource(EdmLibraryExtensions.GetPrimitiveTypeReference(primitiveValue.Value.GetType()).Definition);
      this.WritePrimitiveValue(primitiveValue);
      this.WriteEnd();
    }));

    private void VerifyCanCreateWriteStream(bool synchronousCall)
    {
      this.VerifyNotDisposed();
      this.VerifyCallAllowed(synchronousCall);
    }

    private Stream CreateWriteStreamImplementation()
    {
      this.EnterScope(ODataWriterCore.WriterState.Stream, (ODataItem) null);
      return (Stream) new ODataNotificationStream(this.StartBinaryStream(), (IODataStreamListener) this);
    }

    private void VerifyCanCreateTextWriter(bool synchronousCall)
    {
      this.VerifyNotDisposed();
      this.VerifyCallAllowed(synchronousCall);
    }

    private TextWriter CreateTextWriterImplementation()
    {
      this.EnterScope(ODataWriterCore.WriterState.String, (ODataItem) null);
      return (TextWriter) new ODataNotificationWriter(this.StartTextWriter(), (IODataStreamListener) this);
    }

    private void VerifyCanWriteEnd(bool synchronousCall)
    {
      this.VerifyNotDisposed();
      this.VerifyCallAllowed(synchronousCall);
    }

    private void WriteEndImplementation() => this.InterceptException((Action) (() =>
    {
      ODataWriterCore.Scope currentScope = this.CurrentScope;
      switch (currentScope.State)
      {
        case ODataWriterCore.WriterState.Start:
        case ODataWriterCore.WriterState.Completed:
        case ODataWriterCore.WriterState.Error:
          throw new ODataException(Strings.ODataWriterCore_WriteEndCalledInInvalidState((object) currentScope.State.ToString()));
        case ODataWriterCore.WriterState.Resource:
          if (!this.SkipWriting)
          {
            this.EndResource((ODataResource) currentScope.Item);
            this.DecreaseResourceDepth();
            goto case ODataWriterCore.WriterState.DeltaLink;
          }
          else
            goto case ODataWriterCore.WriterState.DeltaLink;
        case ODataWriterCore.WriterState.ResourceSet:
          if (!this.SkipWriting)
          {
            ODataResourceSet resourceSet = (ODataResourceSet) currentScope.Item;
            WriterValidationUtils.ValidateResourceSetAtEnd(resourceSet, !this.outputContext.WritingResponse);
            this.EndResourceSet(resourceSet);
            goto case ODataWriterCore.WriterState.DeltaLink;
          }
          else
            goto case ODataWriterCore.WriterState.DeltaLink;
        case ODataWriterCore.WriterState.DeltaResourceSet:
          if (!this.SkipWriting)
          {
            ODataDeltaResourceSet deltaResourceSet = (ODataDeltaResourceSet) currentScope.Item;
            WriterValidationUtils.ValidateDeltaResourceSetAtEnd(deltaResourceSet, !this.outputContext.WritingResponse);
            this.EndDeltaResourceSet(deltaResourceSet);
            goto case ODataWriterCore.WriterState.DeltaLink;
          }
          else
            goto case ODataWriterCore.WriterState.DeltaLink;
        case ODataWriterCore.WriterState.DeletedResource:
          if (!this.SkipWriting)
          {
            this.EndDeletedResource((ODataDeletedResource) currentScope.Item);
            this.DecreaseResourceDepth();
            goto case ODataWriterCore.WriterState.DeltaLink;
          }
          else
            goto case ODataWriterCore.WriterState.DeltaLink;
        case ODataWriterCore.WriterState.DeltaLink:
        case ODataWriterCore.WriterState.DeltaDeletedLink:
        case ODataWriterCore.WriterState.Primitive:
          this.LeaveScope();
          break;
        case ODataWriterCore.WriterState.NestedResourceInfo:
          if (!this.outputContext.WritingResponse)
            throw new ODataException(Strings.ODataWriterCore_DeferredLinkInRequest);
          if (!this.SkipWriting)
          {
            ODataNestedResourceInfo nestedResourceInfo = (ODataNestedResourceInfo) currentScope.Item;
            this.DuplicatePropertyNameChecker.ValidatePropertyUniqueness(nestedResourceInfo);
            this.WriteDeferredNestedResourceInfo(nestedResourceInfo);
            this.MarkNestedResourceInfoAsProcessed(nestedResourceInfo);
            goto case ODataWriterCore.WriterState.DeltaLink;
          }
          else
            goto case ODataWriterCore.WriterState.DeltaLink;
        case ODataWriterCore.WriterState.NestedResourceInfoWithContent:
          if (!this.SkipWriting)
          {
            ODataNestedResourceInfo nestedResourceInfo = (ODataNestedResourceInfo) currentScope.Item;
            this.EndNestedResourceInfoWithContent(nestedResourceInfo);
            this.MarkNestedResourceInfoAsProcessed(nestedResourceInfo);
            goto case ODataWriterCore.WriterState.DeltaLink;
          }
          else
            goto case ODataWriterCore.WriterState.DeltaLink;
        case ODataWriterCore.WriterState.Property:
          this.EndProperty((ODataPropertyInfo) currentScope.Item);
          goto case ODataWriterCore.WriterState.DeltaLink;
        case ODataWriterCore.WriterState.Stream:
        case ODataWriterCore.WriterState.String:
          throw new ODataException(Strings.ODataWriterCore_StreamNotDisposed);
        default:
          throw new ODataException(Strings.General_InternalError((object) InternalErrorCodes.ODataWriterCore_WriteEnd_UnreachableCodePath));
      }
    }));

    private void MarkNestedResourceInfoAsProcessed(ODataNestedResourceInfo link) => ((ODataResourceBase) this.scopeStack.Parent.Item).MetadataBuilder.MarkNestedResourceInfoProcessed(link.Name);

    private void VerifyCanWriteEntityReferenceLink(
      ODataEntityReferenceLink entityReferenceLink,
      bool synchronousCall)
    {
      ExceptionUtils.CheckArgumentNotNull<ODataEntityReferenceLink>(entityReferenceLink, nameof (entityReferenceLink));
      this.VerifyNotDisposed();
      this.VerifyCallAllowed(synchronousCall);
    }

    private void VerifyCanWriteLink(bool synchronousCall, ODataDeltaLinkBase deltaLink)
    {
      this.VerifyWritingDelta();
      this.VerifyNotDisposed();
      this.VerifyCallAllowed(synchronousCall);
      ExceptionUtils.CheckArgumentNotNull<ODataDeltaLinkBase>(deltaLink, "delta link");
    }

    private void WriteEntityReferenceLinkImplementation(ODataEntityReferenceLink entityReferenceLink)
    {
      this.CheckForNestedResourceInfoWithContent(ODataPayloadKind.EntityReferenceLink, (ODataItem) null);
      if (this.SkipWriting)
        return;
      this.InterceptException((Action) (() =>
      {
        WriterValidationUtils.ValidateEntityReferenceLink(entityReferenceLink);
        if (!(this.CurrentScope.Item is ODataNestedResourceInfo parentNestedResourceInfo2))
          parentNestedResourceInfo2 = (ODataNestedResourceInfo) this.ParentNestedResourceInfoScope.Item;
        this.WriteEntityReferenceInNavigationLinkContent(parentNestedResourceInfo2, entityReferenceLink);
      }));
    }

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
          throw new ODataException(Strings.ODataWriterCore_SyncCallOnAsyncWriter);
      }
      else if (this.outputContext.Synchronous)
        throw new ODataException(Strings.ODataWriterCore_AsyncCallOnSyncWriter);
    }

    private void VerifyWritingDelta()
    {
      if (!this.writingDelta)
        throw new ODataException(Strings.ODataWriterCore_CannotWriteDeltaWithResourceSetWriter);
    }

    private void ThrowODataException(string errorMessage, ODataItem item)
    {
      this.EnterScope(ODataWriterCore.WriterState.Error, item);
      throw new ODataException(errorMessage);
    }

    private void StartPayloadInStartState()
    {
      if (this.State != ODataWriterCore.WriterState.Start)
        return;
      this.InterceptException(new Action(this.StartPayload));
    }

    private void CheckForNestedResourceInfoWithContent(
      ODataPayloadKind contentPayloadKind,
      ODataItem contentPayload)
    {
      ODataWriterCore.Scope currentScope = this.CurrentScope;
      if (currentScope.State == ODataWriterCore.WriterState.NestedResourceInfo || currentScope.State == ODataWriterCore.WriterState.NestedResourceInfoWithContent)
      {
        ODataNestedResourceInfo currentNestedResourceInfo = (ODataNestedResourceInfo) currentScope.Item;
        this.InterceptException((Action) (() =>
        {
          if (this.ParentResourceType == null)
            return;
          if (this.ParentResourceType.FindProperty(currentNestedResourceInfo.Name) is IEdmStructuralProperty property2)
          {
            this.CurrentScope.ItemType = property2.Type.Definition.AsElementType();
            this.CurrentScope.NavigationSource = this.ParentResourceNavigationSource;
          }
          else
          {
            IEdmNavigationProperty navigationProperty = this.WriterValidator.ValidateNestedResourceInfo(currentNestedResourceInfo, this.ParentResourceType, new ODataPayloadKind?(contentPayloadKind));
            if (navigationProperty == null)
              return;
            this.CurrentScope.ResourceType = (IEdmStructuredType) navigationProperty.ToEntityType();
            IEdmNavigationSource navigationSource = this.ParentResourceNavigationSource;
            if (this.CurrentScope.NavigationSource != null)
              return;
            this.CurrentScope.NavigationSource = navigationSource == null ? (IEdmNavigationSource) null : navigationSource.FindNavigationTarget(navigationProperty, new Func<IEdmPathExpression, List<ODataPathSegment>, bool>(BindingPathHelper.MatchBindingPath), this.CurrentScope.ODataUri.Path.ToList<ODataPathSegment>(), out IEdmPathExpression _);
          }
        }));
        if (currentScope.State == ODataWriterCore.WriterState.NestedResourceInfoWithContent)
        {
          bool? isCollection = currentNestedResourceInfo.IsCollection;
          bool flag = true;
          if (isCollection.GetValueOrDefault() == flag & isCollection.HasValue)
            return;
          this.ThrowODataException(Strings.ODataWriterCore_MultipleItemsInNestedResourceInfoWithContent, (ODataItem) currentNestedResourceInfo);
        }
        else
        {
          this.PromoteNestedResourceInfoScope(contentPayload);
          if (this.SkipWriting)
            return;
          this.InterceptException((Action) (() =>
          {
            if (currentNestedResourceInfo.SerializationInfo != null && currentNestedResourceInfo.SerializationInfo.IsComplex || this.CurrentScope.ItemType != null && !this.CurrentScope.ItemType.IsEntityOrEntityCollectionType())
              return;
            this.DuplicatePropertyNameChecker.ValidatePropertyUniqueness(currentNestedResourceInfo);
            this.StartNestedResourceInfoWithContent(currentNestedResourceInfo);
          }));
        }
      }
      else
      {
        if (contentPayloadKind != ODataPayloadKind.EntityReferenceLink)
          return;
        ODataWriterCore.Scope resourceInfoScope = (ODataWriterCore.Scope) this.ParentNestedResourceInfoScope;
        if (resourceInfoScope.State == ODataWriterCore.WriterState.NestedResourceInfo || resourceInfoScope.State == ODataWriterCore.WriterState.NestedResourceInfoWithContent)
          return;
        this.ThrowODataException(Strings.ODataWriterCore_EntityReferenceLinkWithoutNavigationLink, (ODataItem) null);
      }
    }

    private void ValidateResourceForResourceSet(
      ODataResourceBase resource,
      ODataWriterCore.ResourceBaseScope resourceScope)
    {
      IEdmStructuredType resourceType = this.GetResourceType(resource);
      ODataWriterCore.NestedResourceInfoScope resourceInfoScope = this.ParentNestedResourceInfoScope;
      if (resourceInfoScope != null)
      {
        this.WriterValidator.ValidateResourceInNestedResourceInfo(resourceType, resourceInfoScope.ResourceType);
        resourceScope.ResourceTypeFromMetadata = resourceInfoScope.ResourceType;
        this.WriterValidator.ValidateDerivedTypeConstraint(resourceType, resourceScope.ResourceTypeFromMetadata, resourceInfoScope.DerivedTypeConstraints, "property", ((ODataNestedResourceInfo) resourceInfoScope.Item).Name);
      }
      else
      {
        resourceScope.ResourceTypeFromMetadata = this.ParentScope.ResourceType;
        if (this.CurrentResourceSetValidator != null)
        {
          if (this.ParentScope.State == ODataWriterCore.WriterState.DeltaResourceSet && this.currentResourceDepth <= 1 && resourceScope.NavigationSource != null)
          {
            if (!EdmLibraryExtensions.IsAssignableFrom(resourceScope.NavigationSource.EntityType(), resourceType))
              throw new ODataException(Strings.ResourceSetWithoutExpectedTypeValidator_IncompatibleTypes((object) resourceType.FullTypeName(), (object) resourceScope.NavigationSource.EntityType()));
            resourceScope.ResourceTypeFromMetadata = (IEdmStructuredType) resourceScope.NavigationSource.EntityType();
          }
          else
            this.CurrentResourceSetValidator.ValidateResource((IEdmType) resourceType);
        }
        if (this.ParentScope.NavigationSource != null)
          this.WriterValidator.ValidateDerivedTypeConstraint(resourceType, resourceScope.ResourceTypeFromMetadata, this.ParentScope.DerivedTypeConstraints, "navigation source", this.ParentScope.NavigationSource.Name);
      }
      resourceScope.ResourceType = resourceType;
      if (this.ParentScope.State != ODataWriterCore.WriterState.DeltaResourceSet)
        return;
      IEdmEntityType entityType = resourceType as IEdmEntityType;
      if (!(resource.Id == (Uri) null) || entityType == null)
        return;
      if (!(resource is ODataDeletedResource))
      {
        ODataVersion? version = this.outputContext.MessageWriterSettings.Version;
        ODataVersion odataVersion = ODataVersion.V4;
        if (!(version.GetValueOrDefault() > odataVersion & version.HasValue))
          return;
      }
      if (!ODataWriterCore.HasKeyProperties(entityType, resource.Properties))
        throw new ODataException(Strings.ODataWriterCore_DeltaResourceWithoutIdOrKeyProperties);
    }

    private static bool HasKeyProperties(
      IEdmEntityType entityType,
      IEnumerable<ODataProperty> properties)
    {
      return properties != null && entityType.Key().All<IEdmStructuralProperty>((Func<IEdmStructuralProperty, bool>) (keyProp => properties.Select<ODataProperty, string>((Func<ODataProperty, string>) (p => p.Name)).Contains<string>(keyProp.Name)));
    }

    private void InterceptException(Action action)
    {
      try
      {
        action();
      }
      catch
      {
        if (!ODataWriterCore.IsErrorState(this.State))
          this.EnterScope(ODataWriterCore.WriterState.Error, this.CurrentScope.Item);
        throw;
      }
    }

    private void IncreaseResourceDepth()
    {
      ++this.currentResourceDepth;
      if (this.currentResourceDepth <= this.outputContext.MessageWriterSettings.MessageQuotas.MaxNestingDepth)
        return;
      this.ThrowODataException(Strings.ValidationUtils_MaxDepthOfNestedEntriesExceeded((object) this.outputContext.MessageWriterSettings.MessageQuotas.MaxNestingDepth), (ODataItem) null);
    }

    private void DecreaseResourceDepth() => --this.currentResourceDepth;

    private void NotifyListener(ODataWriterCore.WriterState newState)
    {
      if (this.listener == null)
        return;
      if (ODataWriterCore.IsErrorState(newState))
      {
        this.listener.OnException();
      }
      else
      {
        if (newState != ODataWriterCore.WriterState.Completed)
          return;
        this.listener.OnCompleted();
      }
    }

    [SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily", Justification = "Debug only cast.")]
    private void EnterScope(ODataWriterCore.WriterState newState, ODataItem item)
    {
      this.InterceptException((Action) (() => this.ValidateTransition(newState)));
      bool skipWriting = this.SkipWriting;
      ODataWriterCore.Scope currentScope = this.CurrentScope;
      IEdmNavigationSource navigationSource1 = (IEdmNavigationSource) null;
      IEdmType itemType = (IEdmType) null;
      SelectedPropertiesNode selectedProperties = currentScope.SelectedProperties;
      ODataUri odataUri = currentScope.ODataUri.Clone();
      if (odataUri.Path == null)
        odataUri.Path = new ODataPath(new ODataPathSegment[0]);
      IEnumerable<string> derivedTypeConstraints = (IEnumerable<string>) null;
      ODataWriterCore.WriterState state = currentScope.State;
      if (newState == ODataWriterCore.WriterState.Resource || newState == ODataWriterCore.WriterState.ResourceSet || newState == ODataWriterCore.WriterState.Primitive || newState == ODataWriterCore.WriterState.DeltaResourceSet || newState == ODataWriterCore.WriterState.DeletedResource)
      {
        if (item is ODataResourceBase odataResourceBase)
        {
          IEdmModel model = this.outputContext.Model;
          if (model != null)
          {
            if (model.IsUserModel())
            {
              try
              {
                string typeName = odataResourceBase.TypeName;
                if (!string.IsNullOrEmpty(typeName))
                  itemType = TypeNameOracle.ResolveAndValidateTypeName(model, typeName, EdmTypeKind.None, new bool?(true), this.outputContext.WriterValidator);
                ODataResourceSerializationInfo serializationInfo = odataResourceBase.SerializationInfo;
                if (serializationInfo != null)
                {
                  if (serializationInfo.NavigationSourceName != null)
                  {
                    odataUri = new ODataUriParser(model, new Uri(serializationInfo.NavigationSourceName, UriKind.Relative), this.outputContext.Container).ParseUri();
                    navigationSource1 = odataUri.Path.NavigationSource();
                    itemType = itemType ?? (IEdmType) navigationSource1.EntityType();
                  }
                  if (typeName == null)
                  {
                    if (!string.IsNullOrEmpty(serializationInfo.ExpectedTypeName))
                      itemType = TypeNameOracle.ResolveAndValidateTypeName(model, serializationInfo.ExpectedTypeName, EdmTypeKind.None, new bool?(true), this.outputContext.WriterValidator);
                    else if (!string.IsNullOrEmpty(serializationInfo.NavigationSourceEntityTypeName))
                      itemType = TypeNameOracle.ResolveAndValidateTypeName(model, serializationInfo.NavigationSourceEntityTypeName, EdmTypeKind.Entity, new bool?(true), this.outputContext.WriterValidator);
                  }
                }
              }
              catch (ODataException ex)
              {
              }
            }
          }
        }
        derivedTypeConstraints = navigationSource1 != null ? this.outputContext.Model.GetDerivedTypeConstraints(navigationSource1) : currentScope.DerivedTypeConstraints;
        navigationSource1 = navigationSource1 ?? currentScope.NavigationSource;
        itemType = itemType ?? currentScope.ItemType;
        if (itemType == null && (state == ODataWriterCore.WriterState.Start || state == ODataWriterCore.WriterState.NestedResourceInfo || state == ODataWriterCore.WriterState.NestedResourceInfoWithContent) && (newState == ODataWriterCore.WriterState.ResourceSet || newState == ODataWriterCore.WriterState.DeltaResourceSet) && item is ODataResourceSetBase odataResourceSetBase && odataResourceSetBase.TypeName != null && this.outputContext.Model.IsUserModel() && TypeNameOracle.ResolveAndValidateTypeName(this.outputContext.Model, odataResourceSetBase.TypeName, EdmTypeKind.Collection, new bool?(false), this.outputContext.WriterValidator) is IEdmCollectionType edmCollectionType)
          itemType = edmCollectionType.ElementType.Definition;
      }
      if ((state == ODataWriterCore.WriterState.Resource || state == ODataWriterCore.WriterState.DeletedResource) && newState == ODataWriterCore.WriterState.NestedResourceInfo)
      {
        ODataNestedResourceInfo nestedResourceInfo = (ODataNestedResourceInfo) item;
        if (!skipWriting)
        {
          selectedProperties = currentScope.SelectedProperties.GetSelectedPropertiesForNavigationProperty(currentScope.ResourceType, nestedResourceInfo.Name);
          if (this.outputContext.WritingResponse || this.writingDelta)
          {
            ODataPath odataPath = odataUri.Path;
            IEdmStructuredType resourceType = currentScope.ResourceType;
            ODataWriterCore.ResourceBaseScope resourceBaseScope = currentScope as ODataWriterCore.ResourceBaseScope;
            TypeSegment newSegment = (TypeSegment) null;
            if (resourceBaseScope.ResourceTypeFromMetadata != resourceType)
              newSegment = new TypeSegment((IEdmType) resourceType, (IEdmNavigationSource) null);
            if (this.WriterValidator.ValidatePropertyDefined(nestedResourceInfo.Name, resourceType) is IEdmStructuralProperty structuralProperty)
            {
              ODataPath path = this.AppendEntitySetKeySegment(odataPath, false);
              itemType = structuralProperty.Type == null ? (IEdmType) null : structuralProperty.Type.Definition.AsElementType();
              navigationSource1 = (IEdmNavigationSource) null;
              if (newSegment != null)
                path.Add((ODataPathSegment) newSegment);
              odataPath = path.AppendPropertySegment(structuralProperty);
              derivedTypeConstraints = this.outputContext.Model.GetDerivedTypeConstraints((IEdmVocabularyAnnotatable) structuralProperty);
            }
            else
            {
              IEdmNavigationProperty navigationProperty = this.WriterValidator.ValidateNestedResourceInfo(nestedResourceInfo, resourceType, new ODataPayloadKind?());
              if (navigationProperty != null)
              {
                derivedTypeConstraints = this.outputContext.Model.GetDerivedTypeConstraints((IEdmVocabularyAnnotatable) navigationProperty);
                itemType = (IEdmType) navigationProperty.ToEntityType();
                bool? isCollection = nestedResourceInfo.IsCollection;
                if (!isCollection.HasValue)
                  nestedResourceInfo.IsCollection = new bool?(navigationProperty.Type.IsEntityCollectionType());
                isCollection = nestedResourceInfo.IsCollection;
                if (!isCollection.HasValue)
                  nestedResourceInfo.IsCollection = new bool?(navigationProperty.Type.IsEntityCollectionType());
                IEdmNavigationSource navigationSource2 = currentScope.NavigationSource;
                if (newSegment != null)
                  odataPath.Add((ODataPathSegment) newSegment);
                navigationSource1 = navigationSource2 == null ? (IEdmNavigationSource) null : navigationSource2.FindNavigationTarget(navigationProperty, new Func<IEdmPathExpression, List<ODataPathSegment>, bool>(BindingPathHelper.MatchBindingPath), odataPath.ToList<ODataPathSegment>(), out IEdmPathExpression _);
                SelectExpandClause selectAndExpand = odataUri.SelectAndExpand;
                TypeSegment typeSegment = (TypeSegment) null;
                if (selectAndExpand != null)
                {
                  SelectExpandClause subSelectExpand;
                  selectAndExpand.GetSubSelectExpandClause(nestedResourceInfo.Name, out subSelectExpand, out typeSegment);
                  odataUri.SelectAndExpand = subSelectExpand;
                }
                switch (navigationSource1.NavigationSourceKind())
                {
                  case EdmNavigationSourceKind.EntitySet:
                    odataPath = new ODataPath(new ODataPathSegment[1]
                    {
                      (ODataPathSegment) new EntitySetSegment(navigationSource1 as IEdmEntitySet)
                    });
                    break;
                  case EdmNavigationSourceKind.Singleton:
                    odataPath = new ODataPath(new ODataPathSegment[1]
                    {
                      (ODataPathSegment) new SingletonSegment(navigationSource1 as IEdmSingleton)
                    });
                    break;
                  case EdmNavigationSourceKind.ContainedEntitySet:
                    ODataPath path = odataPath.Count != 0 ? this.AppendEntitySetKeySegment(odataPath, true) : throw new ODataException(Strings.ODataWriterCore_PathInODataUriMustBeSetWhenWritingContainedElement);
                    if (path != null && typeSegment != null)
                      path.Add((ODataPathSegment) typeSegment);
                    IEdmContainedEntitySet containedEntitySet = (IEdmContainedEntitySet) navigationSource1;
                    odataPath = path.AppendNavigationPropertySegment(containedEntitySet.NavigationProperty, (IEdmNavigationSource) containedEntitySet);
                    break;
                  default:
                    odataPath = (ODataPath) null;
                    break;
                }
              }
            }
            odataUri.Path = odataPath;
          }
        }
      }
      else if ((state == ODataWriterCore.WriterState.ResourceSet || state == ODataWriterCore.WriterState.DeltaResourceSet) && (newState == ODataWriterCore.WriterState.Resource || newState == ODataWriterCore.WriterState.Primitive || newState == ODataWriterCore.WriterState.ResourceSet || newState == ODataWriterCore.WriterState.DeletedResource) && (state == ODataWriterCore.WriterState.ResourceSet || state == ODataWriterCore.WriterState.DeltaResourceSet))
        ++((ODataWriterCore.ResourceSetBaseScope) currentScope).ResourceCount;
      if (navigationSource1 == null)
        navigationSource1 = this.CurrentScope.NavigationSource ?? odataUri.Path.TargetNavigationSource();
      this.PushScope(newState, item, navigationSource1, itemType, skipWriting, selectedProperties, odataUri, derivedTypeConstraints);
      this.NotifyListener(newState);
    }

    private ODataPath AppendEntitySetKeySegment(ODataPath odataPath, bool throwIfFail)
    {
      ODataPath path = odataPath;
      try
      {
        if (EdmExtensionMethods.HasKey(this.CurrentScope.NavigationSource, this.CurrentScope.ResourceType))
        {
          IEdmEntityType resourceType = this.CurrentScope.ResourceType as IEdmEntityType;
          ODataResourceBase resource = this.CurrentScope.Item as ODataResourceBase;
          KeyValuePair<string, object>[] keyProperties = ODataResourceMetadataContext.GetKeyProperties(resource, this.GetResourceSerializationInfo(resource), resourceType);
          path = path.AppendKeySegment((IEnumerable<KeyValuePair<string, object>>) keyProperties, resourceType, this.CurrentScope.NavigationSource);
        }
      }
      catch (ODataException ex)
      {
        if (throwIfFail)
          throw;
      }
      return path;
    }

    private void LeaveScope()
    {
      this.scopeStack.Pop();
      if (this.scopeStack.Count != 1)
        return;
      ODataWriterCore.Scope scope = this.scopeStack.Pop();
      this.PushScope(ODataWriterCore.WriterState.Completed, (ODataItem) null, scope.NavigationSource, (IEdmType) scope.ResourceType, false, scope.SelectedProperties, scope.ODataUri, (IEnumerable<string>) null);
      this.InterceptException(new Action(this.EndPayload));
      this.NotifyListener(ODataWriterCore.WriterState.Completed);
    }

    [SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily", Justification = "Second cast only in debug.")]
    private void PromoteNestedResourceInfoScope(ODataItem content)
    {
      this.ValidateTransition(ODataWriterCore.WriterState.NestedResourceInfoWithContent);
      ODataWriterCore.NestedResourceInfoScope resourceInfoScope = ((ODataWriterCore.NestedResourceInfoScope) this.scopeStack.Pop()).Clone(ODataWriterCore.WriterState.NestedResourceInfoWithContent);
      this.scopeStack.Push((ODataWriterCore.Scope) resourceInfoScope);
      if (resourceInfoScope.ItemType != null || content == null || this.SkipWriting)
        return;
      switch (content)
      {
        case ODataPrimitiveValue _:
          break;
        case ODataPrimitiveValue odataPrimitiveValue:
          resourceInfoScope.ItemType = EdmLibraryExtensions.GetPrimitiveTypeReference(odataPrimitiveValue.GetType()).Definition;
          break;
        default:
          ODataResourceBase resource = content as ODataResourceBase;
          resourceInfoScope.ResourceType = resource != null ? this.GetResourceType(resource) : this.GetResourceSetType(content as ODataResourceSetBase);
          break;
      }
    }

    [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "All the transition checks are encapsulated in this method.")]
    private void ValidateTransition(ODataWriterCore.WriterState newState)
    {
      if (!ODataWriterCore.IsErrorState(this.State) && ODataWriterCore.IsErrorState(newState))
        return;
      switch (this.State)
      {
        case ODataWriterCore.WriterState.Start:
          if (newState != ODataWriterCore.WriterState.ResourceSet && newState != ODataWriterCore.WriterState.Resource && newState != ODataWriterCore.WriterState.DeltaResourceSet)
            throw new ODataException(Strings.ODataWriterCore_InvalidTransitionFromStart((object) this.State.ToString(), (object) newState.ToString()));
          if ((newState == ODataWriterCore.WriterState.ResourceSet || newState == ODataWriterCore.WriterState.DeltaResourceSet) && !this.writingResourceSet)
            throw new ODataException(Strings.ODataWriterCore_CannotWriteTopLevelResourceSetWithResourceWriter);
          if (newState != ODataWriterCore.WriterState.Resource || !this.writingResourceSet)
            break;
          throw new ODataException(Strings.ODataWriterCore_CannotWriteTopLevelResourceWithResourceSetWriter);
        case ODataWriterCore.WriterState.Resource:
        case ODataWriterCore.WriterState.DeletedResource:
          if (this.CurrentScope.Item == null)
            throw new ODataException(Strings.ODataWriterCore_InvalidTransitionFromNullResource((object) this.State.ToString(), (object) newState.ToString()));
          if (newState != ODataWriterCore.WriterState.NestedResourceInfo && newState != ODataWriterCore.WriterState.Property)
            throw new ODataException(Strings.ODataWriterCore_InvalidTransitionFromResource((object) this.State.ToString(), (object) newState.ToString()));
          if (newState == ODataWriterCore.WriterState.DeletedResource && this.ParentScope.State != ODataWriterCore.WriterState.DeltaResourceSet)
            throw new ODataException(Strings.ODataWriterCore_InvalidTransitionFromResourceSet((object) this.State.ToString(), (object) newState.ToString()));
          if (this.State != ODataWriterCore.WriterState.DeletedResource)
            break;
          ODataVersion? version1 = this.Version;
          ODataVersion odataVersion1 = ODataVersion.V401;
          if (!(version1.GetValueOrDefault() < odataVersion1 & version1.HasValue) || newState != ODataWriterCore.WriterState.NestedResourceInfo)
            break;
          throw new ODataException(Strings.ODataWriterCore_InvalidTransitionFrom40DeletedResource((object) this.State.ToString(), (object) newState.ToString()));
        case ODataWriterCore.WriterState.ResourceSet:
          if (newState == ODataWriterCore.WriterState.Resource || this.CurrentScope.ResourceType == null || this.CurrentScope.ResourceType.TypeKind == EdmTypeKind.Untyped && (newState == ODataWriterCore.WriterState.Primitive || newState == ODataWriterCore.WriterState.Stream || newState == ODataWriterCore.WriterState.String || newState == ODataWriterCore.WriterState.ResourceSet))
            break;
          throw new ODataException(Strings.ODataWriterCore_InvalidTransitionFromResourceSet((object) this.State.ToString(), (object) newState.ToString()));
        case ODataWriterCore.WriterState.DeltaResourceSet:
          if (newState == ODataWriterCore.WriterState.Resource || newState == ODataWriterCore.WriterState.DeletedResource || this.ScopeLevel < 3 && (newState == ODataWriterCore.WriterState.DeltaDeletedLink || newState == ODataWriterCore.WriterState.DeltaLink))
            break;
          throw new ODataException(Strings.ODataWriterCore_InvalidTransitionFromResourceSet((object) this.State.ToString(), (object) newState.ToString()));
        case ODataWriterCore.WriterState.NestedResourceInfo:
          if (newState == ODataWriterCore.WriterState.NestedResourceInfoWithContent)
            break;
          throw new ODataException(Strings.ODataWriterCore_InvalidStateTransition((object) this.State.ToString(), (object) newState.ToString()));
        case ODataWriterCore.WriterState.NestedResourceInfoWithContent:
          if (newState == ODataWriterCore.WriterState.ResourceSet || newState == ODataWriterCore.WriterState.Resource || newState == ODataWriterCore.WriterState.Primitive)
            break;
          ODataVersion? version2 = this.Version;
          ODataVersion odataVersion2 = ODataVersion.V401;
          if (!(version2.GetValueOrDefault() < odataVersion2 & version2.HasValue) && (newState == ODataWriterCore.WriterState.DeltaResourceSet || newState == ODataWriterCore.WriterState.DeletedResource))
            break;
          throw new ODataException(Strings.ODataWriterCore_InvalidTransitionFromExpandedLink((object) this.State.ToString(), (object) newState.ToString()));
        case ODataWriterCore.WriterState.Property:
          ODataWriterCore.PropertyInfoScope currentScope = this.CurrentScope as ODataWriterCore.PropertyInfoScope;
          if (currentScope.ValueWritten)
            throw new ODataException(Strings.ODataWriterCore_PropertyValueAlreadyWritten((object) (currentScope.Item as ODataPropertyInfo).Name));
          if (newState != ODataWriterCore.WriterState.Stream && newState != ODataWriterCore.WriterState.String && newState != ODataWriterCore.WriterState.Primitive)
            throw new ODataException(Strings.ODataWriterCore_InvalidStateTransition((object) this.State.ToString(), (object) newState.ToString()));
          currentScope.ValueWritten = true;
          break;
        case ODataWriterCore.WriterState.Stream:
        case ODataWriterCore.WriterState.String:
          throw new ODataException(Strings.ODataWriterCore_StreamNotDisposed);
        case ODataWriterCore.WriterState.Completed:
          throw new ODataException(Strings.ODataWriterCore_InvalidTransitionFromCompleted((object) this.State.ToString(), (object) newState.ToString()));
        case ODataWriterCore.WriterState.Error:
          if (newState == ODataWriterCore.WriterState.Error)
            break;
          throw new ODataException(Strings.ODataWriterCore_InvalidTransitionFromError((object) this.State.ToString(), (object) newState.ToString()));
        default:
          throw new ODataException(Strings.General_InternalError((object) InternalErrorCodes.ODataWriterCore_ValidateTransition_UnreachableCodePath));
      }
    }

    [SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily", Justification = "Debug.Assert check only.")]
    private void PushScope(
      ODataWriterCore.WriterState state,
      ODataItem item,
      IEdmNavigationSource navigationSource,
      IEdmType itemType,
      bool skipWriting,
      SelectedPropertiesNode selectedProperties,
      ODataUri odataUri,
      IEnumerable<string> derivedTypeConstraints)
    {
      IEdmStructuredType edmStructuredType = itemType as IEdmStructuredType;
      bool isUndeclared = false;
      if ((state == ODataWriterCore.WriterState.Resource || state == ODataWriterCore.WriterState.ResourceSet) && (this.CurrentScope.State == ODataWriterCore.WriterState.NestedResourceInfo || this.CurrentScope.State == ODataWriterCore.WriterState.NestedResourceInfoWithContent))
        isUndeclared = this.IsUndeclared(this.CurrentScope.Item as ODataNestedResourceInfo);
      ODataWriterCore.Scope scope;
      switch (state)
      {
        case ODataWriterCore.WriterState.Start:
        case ODataWriterCore.WriterState.Primitive:
        case ODataWriterCore.WriterState.Stream:
        case ODataWriterCore.WriterState.String:
        case ODataWriterCore.WriterState.Completed:
        case ODataWriterCore.WriterState.Error:
          scope = new ODataWriterCore.Scope(state, item, navigationSource, itemType, skipWriting, selectedProperties, odataUri);
          break;
        case ODataWriterCore.WriterState.Resource:
          scope = (ODataWriterCore.Scope) this.CreateResourceScope((ODataResource) item, navigationSource, edmStructuredType, skipWriting, selectedProperties, odataUri, isUndeclared);
          break;
        case ODataWriterCore.WriterState.ResourceSet:
          scope = (ODataWriterCore.Scope) this.CreateResourceSetScope((ODataResourceSet) item, navigationSource, itemType, skipWriting, selectedProperties, odataUri, isUndeclared);
          if (this.outputContext.Model.IsUserModel())
          {
            ((ODataWriterCore.ResourceSetBaseScope) scope).ResourceTypeValidator = new ResourceSetWithoutExpectedTypeValidator(itemType);
            break;
          }
          break;
        case ODataWriterCore.WriterState.DeltaResourceSet:
          scope = (ODataWriterCore.Scope) this.CreateDeltaResourceSetScope((ODataDeltaResourceSet) item, navigationSource, edmStructuredType, skipWriting, selectedProperties, odataUri, isUndeclared);
          if (this.outputContext.Model.IsUserModel())
          {
            ((ODataWriterCore.ResourceSetBaseScope) scope).ResourceTypeValidator = new ResourceSetWithoutExpectedTypeValidator((IEdmType) edmStructuredType);
            break;
          }
          break;
        case ODataWriterCore.WriterState.DeletedResource:
          scope = (ODataWriterCore.Scope) this.CreateDeletedResourceScope((ODataDeletedResource) item, navigationSource, (IEdmEntityType) itemType, skipWriting, selectedProperties, odataUri, isUndeclared);
          break;
        case ODataWriterCore.WriterState.DeltaLink:
        case ODataWriterCore.WriterState.DeltaDeletedLink:
          scope = (ODataWriterCore.Scope) this.CreateDeltaLinkScope((ODataDeltaLinkBase) item, navigationSource, (IEdmEntityType) itemType, selectedProperties, odataUri);
          break;
        case ODataWriterCore.WriterState.NestedResourceInfo:
        case ODataWriterCore.WriterState.NestedResourceInfoWithContent:
          scope = (ODataWriterCore.Scope) this.CreateNestedResourceInfoScope(state, (ODataNestedResourceInfo) item, navigationSource, itemType, skipWriting, selectedProperties, odataUri);
          break;
        case ODataWriterCore.WriterState.Property:
          scope = (ODataWriterCore.Scope) this.CreatePropertyInfoScope((ODataPropertyInfo) item, navigationSource, edmStructuredType, selectedProperties, odataUri);
          break;
        default:
          throw new ODataException(Strings.General_InternalError((object) InternalErrorCodes.ODataWriterCore_Scope_Create_UnreachableCodePath));
      }
      scope.DerivedTypeConstraints = derivedTypeConstraints;
      this.scopeStack.Push(scope);
    }

    private bool IsUndeclared(ODataNestedResourceInfo nestedResourceInfo)
    {
      if (nestedResourceInfo.SerializationInfo != null)
        return nestedResourceInfo.SerializationInfo.IsUndeclared;
      return this.ParentResourceType != null && this.ParentResourceType.FindProperty((this.CurrentScope.Item as ODataNestedResourceInfo).Name) == null;
    }

    internal enum WriterState
    {
      Start,
      Resource,
      ResourceSet,
      DeltaResourceSet,
      DeletedResource,
      DeltaLink,
      DeltaDeletedLink,
      NestedResourceInfo,
      NestedResourceInfoWithContent,
      Primitive,
      Property,
      Stream,
      String,
      Completed,
      Error,
    }

    internal sealed class ScopeStack
    {
      private readonly Stack<ODataWriterCore.Scope> scopes = new Stack<ODataWriterCore.Scope>();

      internal ScopeStack()
      {
      }

      internal int Count => this.scopes.Count;

      internal ODataWriterCore.Scope Parent
      {
        get
        {
          ODataWriterCore.Scope scope = this.scopes.Pop();
          ODataWriterCore.Scope parent = this.scopes.Peek();
          this.scopes.Push(scope);
          return parent;
        }
      }

      internal ODataWriterCore.Scope ParentOfParent
      {
        get
        {
          ODataWriterCore.Scope scope1 = this.scopes.Pop();
          ODataWriterCore.Scope scope2 = this.scopes.Pop();
          ODataWriterCore.Scope parentOfParent = this.scopes.Peek();
          this.scopes.Push(scope2);
          this.scopes.Push(scope1);
          return parentOfParent;
        }
      }

      internal ODataWriterCore.Scope ParentOrNull => this.Count != 0 ? this.Parent : (ODataWriterCore.Scope) null;

      internal Stack<ODataWriterCore.Scope> Scopes => this.scopes;

      internal void Push(ODataWriterCore.Scope scope) => this.scopes.Push(scope);

      internal ODataWriterCore.Scope Pop() => this.scopes.Pop();

      internal ODataWriterCore.Scope Peek() => this.scopes.Peek();
    }

    internal class Scope
    {
      private readonly ODataWriterCore.WriterState state;
      private readonly ODataItem item;
      private readonly bool skipWriting;
      private readonly SelectedPropertiesNode selectedProperties;
      private IEdmNavigationSource navigationSource;
      private IEdmStructuredType resourceType;
      private IEdmType itemType;
      private ODataUri odataUri;

      internal Scope(
        ODataWriterCore.WriterState state,
        ODataItem item,
        IEdmNavigationSource navigationSource,
        IEdmType itemType,
        bool skipWriting,
        SelectedPropertiesNode selectedProperties,
        ODataUri odataUri)
      {
        this.state = state;
        this.item = item;
        this.itemType = itemType;
        this.resourceType = itemType as IEdmStructuredType;
        this.navigationSource = navigationSource;
        this.skipWriting = skipWriting;
        this.selectedProperties = selectedProperties;
        this.odataUri = odataUri;
      }

      public IEdmStructuredType ResourceType
      {
        get => this.resourceType;
        set
        {
          this.resourceType = value;
          this.itemType = (IEdmType) value;
        }
      }

      public IEdmType ItemType
      {
        get => this.itemType;
        set
        {
          this.itemType = value;
          this.resourceType = value as IEdmStructuredType;
        }
      }

      internal ODataWriterCore.WriterState State => this.state;

      internal ODataItem Item => this.item;

      internal IEdmNavigationSource NavigationSource
      {
        get => this.navigationSource;
        set => this.navigationSource = value;
      }

      internal SelectedPropertiesNode SelectedProperties => this.selectedProperties;

      internal ODataUri ODataUri => this.odataUri;

      internal bool SkipWriting => this.skipWriting;

      internal IEnumerable<string> DerivedTypeConstraints { get; set; }
    }

    internal abstract class ResourceSetBaseScope : ODataWriterCore.Scope
    {
      private readonly ODataResourceSerializationInfo serializationInfo;
      private ResourceSetWithoutExpectedTypeValidator resourceTypeValidator;
      private int resourceCount;
      private InstanceAnnotationWriteTracker instanceAnnotationWriteTracker;
      private ODataResourceTypeContext typeContext;

      internal ResourceSetBaseScope(
        ODataWriterCore.WriterState writerState,
        ODataResourceSetBase resourceSet,
        IEdmNavigationSource navigationSource,
        IEdmType itemType,
        bool skipWriting,
        SelectedPropertiesNode selectedProperties,
        ODataUri odataUri)
        : base(writerState, (ODataItem) resourceSet, navigationSource, itemType, skipWriting, selectedProperties, odataUri)
      {
        this.serializationInfo = resourceSet.SerializationInfo;
      }

      internal int ResourceCount
      {
        get => this.resourceCount;
        set => this.resourceCount = value;
      }

      internal InstanceAnnotationWriteTracker InstanceAnnotationWriteTracker
      {
        get
        {
          if (this.instanceAnnotationWriteTracker == null)
            this.instanceAnnotationWriteTracker = new InstanceAnnotationWriteTracker();
          return this.instanceAnnotationWriteTracker;
        }
      }

      internal ResourceSetWithoutExpectedTypeValidator ResourceTypeValidator
      {
        get => this.resourceTypeValidator;
        set => this.resourceTypeValidator = value;
      }

      internal ODataResourceTypeContext GetOrCreateTypeContext(bool writingResponse)
      {
        if (this.typeContext == null)
        {
          bool throwIfMissingTypeInfo = writingResponse && (this.ResourceType == null || this.ResourceType.TypeKind == EdmTypeKind.Entity);
          this.typeContext = ODataResourceTypeContext.Create(this.serializationInfo, this.NavigationSource, EdmTypeWriterResolver.Instance.GetElementType(this.NavigationSource), this.ResourceType, throwIfMissingTypeInfo);
        }
        return this.typeContext;
      }
    }

    internal abstract class ResourceSetScope : ODataWriterCore.ResourceSetBaseScope
    {
      protected ResourceSetScope(
        ODataResourceSet item,
        IEdmNavigationSource navigationSource,
        IEdmType itemType,
        bool skipWriting,
        SelectedPropertiesNode selectedProperties,
        ODataUri odataUri)
        : base(ODataWriterCore.WriterState.ResourceSet, (ODataResourceSetBase) item, navigationSource, itemType, skipWriting, selectedProperties, odataUri)
      {
      }
    }

    internal abstract class DeltaResourceSetScope : ODataWriterCore.ResourceSetBaseScope
    {
      protected DeltaResourceSetScope(
        ODataDeltaResourceSet item,
        IEdmNavigationSource navigationSource,
        IEdmStructuredType resourceType,
        SelectedPropertiesNode selectedProperties,
        ODataUri odataUri)
        : base(ODataWriterCore.WriterState.DeltaResourceSet, (ODataResourceSetBase) item, navigationSource, (IEdmType) resourceType, false, selectedProperties, odataUri)
      {
      }

      public ODataContextUrlInfo ContextUriInfo { get; set; }
    }

    internal class ResourceBaseScope : ODataWriterCore.Scope
    {
      private readonly IDuplicatePropertyNameChecker duplicatePropertyNameChecker;
      private readonly ODataResourceSerializationInfo serializationInfo;
      private IEdmStructuredType resourceTypeFromMetadata;
      private ODataResourceTypeContext typeContext;
      private InstanceAnnotationWriteTracker instanceAnnotationWriteTracker;

      internal ResourceBaseScope(
        ODataWriterCore.WriterState state,
        ODataResourceBase resource,
        ODataResourceSerializationInfo serializationInfo,
        IEdmNavigationSource navigationSource,
        IEdmType itemType,
        bool skipWriting,
        ODataMessageWriterSettings writerSettings,
        SelectedPropertiesNode selectedProperties,
        ODataUri odataUri)
        : base(state, (ODataItem) resource, navigationSource, itemType, skipWriting, selectedProperties, odataUri)
      {
        if (resource != null)
          this.duplicatePropertyNameChecker = writerSettings.Validator.CreateDuplicatePropertyNameChecker();
        this.serializationInfo = serializationInfo;
      }

      public IEdmStructuredType ResourceTypeFromMetadata
      {
        get => this.resourceTypeFromMetadata;
        internal set => this.resourceTypeFromMetadata = value;
      }

      public ODataResourceSerializationInfo SerializationInfo => this.serializationInfo;

      internal IDuplicatePropertyNameChecker DuplicatePropertyNameChecker => this.duplicatePropertyNameChecker;

      internal InstanceAnnotationWriteTracker InstanceAnnotationWriteTracker
      {
        get
        {
          if (this.instanceAnnotationWriteTracker == null)
            this.instanceAnnotationWriteTracker = new InstanceAnnotationWriteTracker();
          return this.instanceAnnotationWriteTracker;
        }
      }

      public ODataResourceTypeContext GetOrCreateTypeContext(bool writingResponse)
      {
        if (this.typeContext == null)
        {
          IEdmStructuredType expectedResourceType = this.ResourceTypeFromMetadata ?? this.ResourceType;
          bool throwIfMissingTypeInfo = writingResponse && (expectedResourceType == null || expectedResourceType.TypeKind == EdmTypeKind.Entity);
          this.typeContext = ODataResourceTypeContext.Create(this.serializationInfo, this.NavigationSource, EdmTypeWriterResolver.Instance.GetElementType(this.NavigationSource), expectedResourceType, throwIfMissingTypeInfo);
        }
        return this.typeContext;
      }
    }

    internal class ResourceScope : ODataWriterCore.ResourceBaseScope
    {
      protected ResourceScope(
        ODataResource resource,
        ODataResourceSerializationInfo serializationInfo,
        IEdmNavigationSource navigationSource,
        IEdmStructuredType resourceType,
        bool skipWriting,
        ODataMessageWriterSettings writerSettings,
        SelectedPropertiesNode selectedProperties,
        ODataUri odataUri)
        : base(ODataWriterCore.WriterState.Resource, (ODataResourceBase) resource, serializationInfo, navigationSource, (IEdmType) resourceType, skipWriting, writerSettings, selectedProperties, odataUri)
      {
      }
    }

    internal class DeletedResourceScope : ODataWriterCore.ResourceBaseScope
    {
      protected DeletedResourceScope(
        ODataDeletedResource resource,
        ODataResourceSerializationInfo serializationInfo,
        IEdmNavigationSource navigationSource,
        IEdmEntityType entityType,
        ODataMessageWriterSettings writerSettings,
        SelectedPropertiesNode selectedProperties,
        ODataUri odataUri)
        : base(ODataWriterCore.WriterState.DeletedResource, (ODataResourceBase) resource, serializationInfo, navigationSource, (IEdmType) entityType, false, writerSettings, selectedProperties, odataUri)
      {
      }
    }

    internal abstract class DeltaLinkScope : ODataWriterCore.Scope
    {
      private readonly ODataResourceSerializationInfo serializationInfo;
      private readonly EdmEntityType fakeEntityType = new EdmEntityType("MyNS", "Fake");
      private ODataResourceTypeContext typeContext;

      protected DeltaLinkScope(
        ODataWriterCore.WriterState state,
        ODataItem link,
        ODataResourceSerializationInfo serializationInfo,
        IEdmNavigationSource navigationSource,
        IEdmEntityType entityType,
        SelectedPropertiesNode selectedProperties,
        ODataUri odataUri)
        : base(state, link, navigationSource, (IEdmType) entityType, false, selectedProperties, odataUri)
      {
        this.serializationInfo = serializationInfo;
      }

      public ODataResourceTypeContext GetOrCreateTypeContext(bool writingResponse = true)
      {
        if (this.typeContext == null)
          this.typeContext = ODataResourceTypeContext.Create(this.serializationInfo, this.NavigationSource, EdmTypeWriterResolver.Instance.GetElementType(this.NavigationSource), (IEdmStructuredType) this.fakeEntityType, writingResponse);
        return this.typeContext;
      }
    }

    internal class PropertyInfoScope : ODataWriterCore.Scope
    {
      internal PropertyInfoScope(
        ODataPropertyInfo property,
        IEdmNavigationSource navigationSource,
        IEdmStructuredType resourceType,
        SelectedPropertiesNode selectedProperties,
        ODataUri odataUri)
        : base(ODataWriterCore.WriterState.Property, (ODataItem) property, navigationSource, (IEdmType) resourceType, false, selectedProperties, odataUri)
      {
        this.ValueWritten = false;
      }

      public ODataPropertyInfo Property => (ODataPropertyInfo) (this.Item as ODataProperty);

      internal bool ValueWritten { get; set; }
    }

    internal class NestedResourceInfoScope : ODataWriterCore.Scope
    {
      internal NestedResourceInfoScope(
        ODataWriterCore.WriterState writerState,
        ODataNestedResourceInfo navLink,
        IEdmNavigationSource navigationSource,
        IEdmType itemType,
        bool skipWriting,
        SelectedPropertiesNode selectedProperties,
        ODataUri odataUri)
        : base(writerState, (ODataItem) navLink, navigationSource, itemType, skipWriting, selectedProperties, odataUri)
      {
      }

      internal virtual ODataWriterCore.NestedResourceInfoScope Clone(
        ODataWriterCore.WriterState newWriterState)
      {
        ODataWriterCore.NestedResourceInfoScope resourceInfoScope = new ODataWriterCore.NestedResourceInfoScope(newWriterState, (ODataNestedResourceInfo) this.Item, this.NavigationSource, this.ItemType, this.SkipWriting, this.SelectedProperties, this.ODataUri);
        resourceInfoScope.DerivedTypeConstraints = this.DerivedTypeConstraints;
        return resourceInfoScope;
      }
    }
  }
}
