// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.JsonLight.ODataJsonLightReader
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Vocabularies;
using Microsoft.OData.Evaluation;
using Microsoft.OData.Json;
using Microsoft.OData.Metadata;
using Microsoft.OData.UriParser;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.OData.JsonLight
{
  internal sealed class ODataJsonLightReader : ODataReaderCoreAsync
  {
    private readonly ODataJsonLightInputContext jsonLightInputContext;
    private readonly ODataJsonLightResourceDeserializer jsonLightResourceDeserializer;
    private readonly ODataJsonLightReader.JsonLightTopLevelScope topLevelScope;
    private readonly bool readingParameter;

    internal ODataJsonLightReader(
      ODataJsonLightInputContext jsonLightInputContext,
      IEdmNavigationSource navigationSource,
      IEdmStructuredType expectedResourceType,
      bool readingResourceSet,
      bool readingParameter = false,
      bool readingDelta = false,
      IODataReaderWriterListener listener = null)
      : base((ODataInputContext) jsonLightInputContext, readingResourceSet, readingDelta, listener)
    {
      this.jsonLightInputContext = jsonLightInputContext;
      this.jsonLightResourceDeserializer = new ODataJsonLightResourceDeserializer(jsonLightInputContext);
      this.readingParameter = readingParameter;
      this.topLevelScope = new ODataJsonLightReader.JsonLightTopLevelScope(navigationSource, expectedResourceType, new ODataUri());
      this.EnterScope((ODataReaderCore.Scope) this.topLevelScope);
    }

    private IODataJsonLightReaderResourceState CurrentResourceState => (IODataJsonLightReaderResourceState) this.CurrentScope;

    private ODataJsonLightReader.JsonLightResourceSetScope CurrentJsonLightResourceSetScope => (ODataJsonLightReader.JsonLightResourceSetScope) this.CurrentScope;

    private ODataJsonLightReader.JsonLightNestedResourceInfoScope CurrentJsonLightNestedResourceInfoScope => (ODataJsonLightReader.JsonLightNestedResourceInfoScope) this.CurrentScope;

    private ODataNestedResourceInfo ParentNestedInfo
    {
      get
      {
        ODataReaderCore.Scope scope = this.SeekScope<ODataJsonLightReader.JsonLightNestedResourceInfoScope>(3);
        return scope == null ? (ODataNestedResourceInfo) null : (ODataNestedResourceInfo) scope.Item;
      }
    }

    protected override bool ReadAtStartImplementation()
    {
      PropertyAndAnnotationCollector annotationCollector = this.jsonLightInputContext.CreatePropertyAndAnnotationCollector();
      this.jsonLightResourceDeserializer.ReadPayloadStart(this.ReadingResourceSet ? (this.ReadingDelta ? ODataPayloadKind.Delta : ODataPayloadKind.ResourceSet) : ODataPayloadKind.Resource, annotationCollector, this.IsReadingNestedPayload || this.readingParameter, false);
      this.ResolveScopeInfoFromContextUrl();
      ODataReaderCore.Scope currentScope = this.CurrentScope;
      if (this.jsonLightInputContext.Model.IsUserModel())
      {
        IEnumerable<string> derivedTypeConstraints = this.jsonLightInputContext.Model.GetDerivedTypeConstraints(currentScope.NavigationSource);
        if (derivedTypeConstraints != null)
          currentScope.DerivedTypeValidator = new DerivedTypeValidator(currentScope.ResourceType, derivedTypeConstraints, "navigation source", currentScope.NavigationSource.Name);
      }
      return this.ReadAtStartImplementationSynchronously(annotationCollector);
    }

    protected override Task<bool> ReadAtStartImplementationAsync()
    {
      PropertyAndAnnotationCollector propertyAndAnnotationCollector = this.jsonLightInputContext.CreatePropertyAndAnnotationCollector();
      return this.jsonLightResourceDeserializer.ReadPayloadStartAsync(this.ReadingDelta ? ODataPayloadKind.Delta : (this.ReadingResourceSet ? ODataPayloadKind.ResourceSet : ODataPayloadKind.Resource), propertyAndAnnotationCollector, this.IsReadingNestedPayload, false).FollowOnSuccessWith((Action<Task>) (t => this.ResolveScopeInfoFromContextUrl())).FollowOnSuccessWith<bool>((Func<Task, bool>) (t => this.ReadAtStartImplementationSynchronously(propertyAndAnnotationCollector)));
    }

    protected override bool ReadAtResourceSetStartImplementation() => this.ReadAtResourceSetStartImplementationSynchronously();

    protected override Task<bool> ReadAtResourceSetStartImplementationAsync() => TaskUtils.GetTaskForSynchronousOperation<bool>(new Func<bool>(this.ReadAtResourceSetStartImplementationSynchronously));

    protected override bool ReadAtResourceSetEndImplementation() => this.ReadAtResourceSetEndImplementationSynchronously();

    protected override Task<bool> ReadAtResourceSetEndImplementationAsync() => TaskUtils.GetTaskForSynchronousOperation<bool>(new Func<bool>(this.ReadAtResourceSetEndImplementationSynchronously));

    protected override bool ReadAtResourceStartImplementation() => this.ReadAtResourceStartImplementationSynchronously();

    protected override Task<bool> ReadAtResourceStartImplementationAsync() => TaskUtils.GetTaskForSynchronousOperation<bool>(new Func<bool>(this.ReadAtResourceStartImplementationSynchronously));

    protected override bool ReadAtResourceEndImplementation() => this.ReadAtResourceEndImplementationSynchronously();

    protected override Task<bool> ReadAtResourceEndImplementationAsync() => TaskUtils.GetTaskForSynchronousOperation<bool>(new Func<bool>(this.ReadAtResourceEndImplementationSynchronously));

    protected override bool ReadAtPrimitiveImplementation() => this.ReadAtPrimitiveSynchronously();

    protected override Task<bool> ReadAtPrimitiveImplementationAsync() => TaskUtils.GetTaskForSynchronousOperation<bool>(new Func<bool>(this.ReadAtPrimitiveSynchronously));

    protected override bool ReadAtNestedPropertyInfoImplementation() => this.ReadAtNestedPropertyInfoSynchronously();

    protected override Task<bool> ReadAtNestedPropertyInfoImplementationAsync() => TaskUtils.GetTaskForSynchronousOperation<bool>(new Func<bool>(this.ReadAtNestedPropertyInfoSynchronously));

    protected override bool ReadAtStreamImplementation() => this.ReadAtStreamSynchronously();

    protected override Task<bool> ReadAtStreamImplementationAsync() => TaskUtils.GetTaskForSynchronousOperation<bool>(new Func<bool>(this.ReadAtStreamSynchronously));

    protected override Stream CreateReadStreamImplementation()
    {
      IJsonStreamReader jsonReader = (IJsonStreamReader) this.jsonLightInputContext.JsonReader;
      Stream streamImplementation;
      if (jsonReader != null)
      {
        streamImplementation = jsonReader.CreateReadStream();
      }
      else
      {
        this.jsonLightInputContext.JsonReader.Read();
        streamImplementation = (Stream) new MemoryStream(Convert.FromBase64String(this.jsonLightInputContext.JsonReader.ReadStringValue().Replace('_', '/').Replace('-', '+')));
      }
      return streamImplementation;
    }

    protected override TextReader CreateTextReaderImplementation()
    {
      IJsonStreamReader jsonReader = (IJsonStreamReader) this.jsonLightInputContext.JsonReader;
      TextReader readerImplementation;
      if (jsonReader != null)
      {
        readerImplementation = jsonReader.CreateTextReader();
      }
      else
      {
        this.jsonLightInputContext.JsonReader.Read();
        readerImplementation = (TextReader) new StringReader(this.jsonLightInputContext.JsonReader.ReadStringValue());
      }
      return readerImplementation;
    }

    protected override bool ReadAtNestedResourceInfoStartImplementation() => this.ReadAtNestedResourceInfoStartImplementationSynchronously();

    protected override Task<bool> ReadAtNestedResourceInfoStartImplementationAsync() => TaskUtils.GetTaskForSynchronousOperation<bool>(new Func<bool>(this.ReadAtNestedResourceInfoStartImplementationSynchronously));

    protected override bool ReadAtNestedResourceInfoEndImplementation() => this.ReadAtNestedResourceInfoEndImplementationSynchronously();

    protected override Task<bool> ReadAtNestedResourceInfoEndImplementationAsync() => TaskUtils.GetTaskForSynchronousOperation<bool>(new Func<bool>(this.ReadAtNestedResourceInfoEndImplementationSynchronously));

    protected override bool ReadAtEntityReferenceLink() => this.ReadAtEntityReferenceLinkSynchronously();

    protected override Task<bool> ReadAtEntityReferenceLinkAsync() => TaskUtils.GetTaskForSynchronousOperation<bool>(new Func<bool>(this.ReadAtEntityReferenceLinkSynchronously));

    protected override bool ReadAtDeltaResourceSetStartImplementation() => this.ReadAtResourceSetStartImplementationSynchronously();

    protected override Task<bool> ReadAtDeltaResourceSetStartImplementationAsync() => TaskUtils.GetTaskForSynchronousOperation<bool>(new Func<bool>(this.ReadAtResourceSetStartImplementationSynchronously));

    protected override bool ReadAtDeltaResourceSetEndImplementation() => this.ReadAtResourceSetEndImplementationSynchronously();

    protected override Task<bool> ReadAtDeltaResourceSetEndImplementationAsync() => TaskUtils.GetTaskForSynchronousOperation<bool>(new Func<bool>(this.ReadAtResourceSetEndImplementationSynchronously));

    protected override bool ReadAtDeletedResourceStartImplementation() => this.ReadAtDeletedResourceStartImplementationSynchronously();

    protected override Task<bool> ReadAtDeletedResourceStartImplementationAsync() => TaskUtils.GetTaskForSynchronousOperation<bool>(new Func<bool>(this.ReadAtDeletedResourceStartImplementationSynchronously));

    protected override bool ReadAtDeletedResourceEndImplementation() => this.ReadAtResourceEndImplementationSynchronously();

    protected override Task<bool> ReadAtDeletedResourceEndImplementationAsync() => TaskUtils.GetTaskForSynchronousOperation<bool>(new Func<bool>(this.ReadAtResourceEndImplementationSynchronously));

    protected override bool ReadAtDeltaLinkImplementation() => this.ReadAtDeltaLinkImplementationSynchronously();

    protected override Task<bool> ReadAtDeltaLinkImplementationAsync() => TaskUtils.GetTaskForSynchronousOperation<bool>(new Func<bool>(this.ReadAtDeltaLinkImplementationSynchronously));

    protected override bool ReadAtDeltaDeletedLinkImplementation() => this.ReadAtDeltaDeletedLinkImplementationSynchronously();

    protected override Task<bool> ReadAtDeltaDeletedLinkImplementationAsync() => TaskUtils.GetTaskForSynchronousOperation<bool>(new Func<bool>(this.ReadAtDeltaDeletedLinkImplementationSynchronously));

    private bool ReadAtStartImplementationSynchronously(
      PropertyAndAnnotationCollector propertyAndAnnotationCollector)
    {
      if (this.jsonLightInputContext.ReadingResponse && !this.IsReadingNestedPayload)
        ReaderValidationUtils.ValidateResourceSetOrResourceContextUri(this.jsonLightResourceDeserializer.ContextUriParseResult, this.CurrentScope, true);
      SelectedPropertiesNode selectedProperties = SelectedPropertiesNode.Create(this.jsonLightResourceDeserializer.ContextUriParseResult == null ? (string) null : this.jsonLightResourceDeserializer.ContextUriParseResult.SelectQueryOption);
      if (this.ReadingResourceSet)
      {
        this.topLevelScope.PropertyAndAnnotationCollector = propertyAndAnnotationCollector;
        bool readAllResourceSetProperties = this.jsonLightInputContext.JsonReader is ReorderingJsonReader;
        if (this.ReadingDelta)
        {
          ODataDeltaResourceSet deltaResourceSet = new ODataDeltaResourceSet();
          this.jsonLightResourceDeserializer.ReadTopLevelResourceSetAnnotations((ODataResourceSetBase) deltaResourceSet, propertyAndAnnotationCollector, true, readAllResourceSetProperties);
          this.ReadDeltaResourceSetStart(deltaResourceSet, selectedProperties);
        }
        else
        {
          ODataResourceSet resourceSet = new ODataResourceSet();
          if (!this.IsReadingNestedPayload)
          {
            if (!this.readingParameter)
              this.jsonLightResourceDeserializer.ReadTopLevelResourceSetAnnotations((ODataResourceSetBase) resourceSet, propertyAndAnnotationCollector, true, readAllResourceSetProperties);
            else
              this.jsonLightResourceDeserializer.JsonReader.Read();
          }
          this.ReadResourceSetStart(resourceSet, selectedProperties);
        }
        return true;
      }
      this.ReadResourceSetItemStart(propertyAndAnnotationCollector, selectedProperties);
      return true;
    }

    private bool ReadAtResourceSetStartImplementationSynchronously()
    {
      this.ReadNextResourceSetItem();
      return true;
    }

    private bool ReadAtResourceSetEndImplementationSynchronously()
    {
      bool isTopLevel = this.IsTopLevel;
      bool expandedLinkContent = this.IsExpandedLinkContent;
      this.PopScope(this.State == ODataReaderState.ResourceSetEnd ? ODataReaderState.ResourceSetEnd : ODataReaderState.DeltaResourceSetEnd);
      if (((this.IsReadingNestedPayload ? 1 : (this.readingParameter ? 1 : 0)) & (isTopLevel ? 1 : 0)) != 0)
      {
        this.ReplaceScope(ODataReaderState.Completed);
        return false;
      }
      if (isTopLevel)
      {
        this.jsonLightResourceDeserializer.JsonReader.Read();
        this.jsonLightResourceDeserializer.ReadPayloadEnd(this.IsReadingNestedPayload);
        this.ReplaceScope(ODataReaderState.Completed);
        return false;
      }
      if (expandedLinkContent)
      {
        this.ReadExpandedNestedResourceInfoEnd(true);
        return true;
      }
      this.ReadNextResourceSetItem();
      return true;
    }

    private bool ReadAtResourceStartImplementationSynchronously()
    {
      if (this.Item is ODataResourceBase odataResourceBase && !this.IsReadingNestedPayload)
      {
        this.CurrentResourceState.ResourceTypeFromMetadata = this.ParentScope.ResourceType as IEdmStructuredType;
        ODataResourceMetadataBuilder builderForReader = this.jsonLightResourceDeserializer.MetadataContext.GetResourceMetadataBuilderForReader(this.CurrentResourceState, this.jsonLightInputContext.ODataSimplifiedOptions.EnableReadingKeyAsSegment, this.ReadingDelta);
        if (builderForReader != odataResourceBase.MetadataBuilder)
        {
          ODataNestedResourceInfo parentNestedInfo = this.ParentNestedInfo;
          if (builderForReader is ODataConventionalResourceMetadataBuilder resourceMetadataBuilder1)
          {
            if (parentNestedInfo != null)
            {
              resourceMetadataBuilder1.NameAsProperty = parentNestedInfo.Name;
              ODataConventionalResourceMetadataBuilder resourceMetadataBuilder = resourceMetadataBuilder1;
              bool? isCollection = parentNestedInfo.IsCollection;
              bool flag = true;
              int num = isCollection.GetValueOrDefault() == flag & isCollection.HasValue ? 1 : 0;
              resourceMetadataBuilder.IsFromCollection = num != 0;
              resourceMetadataBuilder1.ODataUri = this.ResolveODataUriFromContextUrl(parentNestedInfo) ?? this.CurrentScope.ODataUri;
            }
            resourceMetadataBuilder1.StartResource();
          }
          odataResourceBase.MetadataBuilder = builderForReader;
          if (parentNestedInfo != null && parentNestedInfo.MetadataBuilder != null)
            odataResourceBase.MetadataBuilder.ParentMetadataBuilder = parentNestedInfo.MetadataBuilder;
        }
      }
      if (odataResourceBase == null)
        this.EndEntry();
      else if (this.CurrentResourceState.FirstNestedInfo != null)
        this.ReadNestedInfo(this.CurrentResourceState.FirstNestedInfo);
      else
        this.EndEntry();
      return true;
    }

    private ODataUri ResolveODataUriFromContextUrl(ODataNestedResourceInfo nestedInfo)
    {
      if (nestedInfo == null || !(nestedInfo.ContextUrl != (Uri) null))
        return (ODataUri) null;
      ODataPayloadKind payloadKind = nestedInfo.IsCollection.GetValueOrDefault() ? ODataPayloadKind.ResourceSet : ODataPayloadKind.Resource;
      ODataPath path = ODataJsonLightContextUriParser.Parse(this.jsonLightResourceDeserializer.Model, UriUtils.UriToString(nestedInfo.ContextUrl), payloadKind, this.jsonLightResourceDeserializer.MessageReaderSettings.ClientCustomTypeResolver, this.jsonLightResourceDeserializer.JsonLightInputContext.ReadingResponse).Path;
      return new ODataUri() { Path = path };
    }

    private bool ReadAtResourceEndImplementationSynchronously()
    {
      bool isTopLevel = this.IsTopLevel;
      bool expandedLinkContent = this.IsExpandedLinkContent;
      this.PopScope(this.State == ODataReaderState.ResourceEnd ? ODataReaderState.ResourceEnd : ODataReaderState.DeletedResourceEnd);
      this.jsonLightResourceDeserializer.JsonReader.Read();
      bool flag = true;
      if (isTopLevel)
      {
        this.jsonLightResourceDeserializer.ReadPayloadEnd(this.IsReadingNestedPayload);
        this.ReplaceScope(ODataReaderState.Completed);
        flag = false;
      }
      else if (expandedLinkContent)
        this.ReadExpandedNestedResourceInfoEnd(false);
      else
        this.ReadNextResourceSetItem();
      return flag;
    }

    private bool ReadAtPrimitiveSynchronously()
    {
      this.PopScope(ODataReaderState.Primitive);
      this.jsonLightResourceDeserializer.JsonReader.Read();
      this.ReadNextResourceSetItem();
      return true;
    }

    private bool ReadAtDeletedResourceStartImplementationSynchronously()
    {
      if (!((ODataJsonLightReader.JsonLightDeletedResourceScope) this.CurrentScope).Is40DeletedResource)
        return this.ReadAtResourceStartImplementationSynchronously();
      this.EndEntry();
      return true;
    }

    private bool ReadAtDeltaLinkImplementationSynchronously() => this.EndDeltaLink(ODataReaderState.DeltaLink);

    private bool ReadAtDeltaDeletedLinkImplementationSynchronously() => this.EndDeltaLink(ODataReaderState.DeltaDeletedLink);

    private bool EndDeltaLink(ODataReaderState readerState)
    {
      this.PopScope(readerState);
      this.jsonLightResourceDeserializer.JsonReader.Read();
      this.ReadNextResourceSetItem();
      return true;
    }

    private bool ReadAtNestedResourceInfoStartImplementationSynchronously()
    {
      ODataNestedResourceInfo nestedResourceInfo1 = this.CurrentNestedResourceInfo;
      IODataJsonLightReaderResourceState parentScope1 = (IODataJsonLightReaderResourceState) this.ParentScope;
      if (this.jsonLightInputContext.ReadingResponse)
      {
        if (parentScope1.ProcessingMissingProjectedNestedResourceInfos)
          this.ReplaceScope(ODataReaderState.NestedResourceInfoEnd);
        else if (!this.jsonLightResourceDeserializer.JsonReader.IsOnValueNode())
        {
          ReaderUtils.CheckForDuplicateNestedResourceInfoNameAndSetAssociationLink(parentScope1.PropertyAndAnnotationCollector, nestedResourceInfo1);
          parentScope1.NavigationPropertiesRead.Add(nestedResourceInfo1.Name);
          this.ReplaceScope(ODataReaderState.NestedResourceInfoEnd);
        }
        else if (!nestedResourceInfo1.IsCollection.Value)
        {
          ReaderUtils.CheckForDuplicateNestedResourceInfoNameAndSetAssociationLink(parentScope1.PropertyAndAnnotationCollector, nestedResourceInfo1);
          this.ReadExpandedNestedResourceInfoStart(nestedResourceInfo1);
        }
        else
        {
          ReaderUtils.CheckForDuplicateNestedResourceInfoNameAndSetAssociationLink(parentScope1.PropertyAndAnnotationCollector, nestedResourceInfo1);
          ODataJsonLightReaderNestedResourceInfo nestedResourceInfo2 = this.CurrentJsonLightNestedResourceInfoScope.ReaderNestedResourceInfo;
          ODataJsonLightReader.JsonLightResourceBaseScope parentScope2 = (ODataJsonLightReader.JsonLightResourceBaseScope) this.ParentScope;
          SelectedPropertiesNode selectedProperties = parentScope2.SelectedProperties;
          if (nestedResourceInfo2.NestedResourceSet is ODataResourceSet nestedResourceSet)
            this.ReadResourceSetStart(nestedResourceSet, selectedProperties.GetSelectedPropertiesForNavigationProperty(parentScope2.ResourceType, nestedResourceInfo1.Name));
          else
            this.ReadDeltaResourceSetStart(nestedResourceInfo2.NestedResourceSet as ODataDeltaResourceSet, selectedProperties.GetSelectedPropertiesForNavigationProperty(parentScope2.ResourceType, nestedResourceInfo1.Name));
        }
      }
      else
      {
        ReaderUtils.CheckForDuplicateNestedResourceInfoNameAndSetAssociationLink(parentScope1.PropertyAndAnnotationCollector, nestedResourceInfo1);
        this.ReadNextNestedResourceInfoContentItemInRequest();
      }
      return true;
    }

    private bool ReadAtNestedResourceInfoEndImplementationSynchronously()
    {
      this.PopScope(ODataReaderState.NestedResourceInfoEnd);
      return this.ReadNextNestedInfo();
    }

    private bool ReadAtNestedPropertyInfoSynchronously()
    {
      ODataPropertyInfo odataPropertyInfo = this.CurrentScope.Item as ODataPropertyInfo;
      if (odataPropertyInfo is ODataStreamPropertyInfo streamPropertyInfo && !string.IsNullOrEmpty(streamPropertyInfo.ContentType))
        this.StartNestedStreamInfo(new ODataJsonLightReaderStreamInfo(streamPropertyInfo.PrimitiveTypeKind, streamPropertyInfo.ContentType));
      else
        this.StartNestedStreamInfo(new ODataJsonLightReaderStreamInfo(odataPropertyInfo.PrimitiveTypeKind));
      return true;
    }

    private bool ReadAtStreamSynchronously()
    {
      this.PopScope(ODataReaderState.Stream);
      if (this.State == ODataReaderState.ResourceSetStart || this.State == ODataReaderState.DeltaResourceSetStart)
      {
        this.ReadNextResourceSetItem();
        return true;
      }
      if (this.State == ODataReaderState.NestedProperty)
        this.PopScope(ODataReaderState.NestedProperty);
      return this.ReadNextNestedInfo();
    }

    private bool ReadNextNestedInfo()
    {
      IODataJsonLightReaderResourceState currentResourceState = this.CurrentResourceState;
      ODataJsonLightReaderNestedInfo nestedInfo = !this.jsonLightInputContext.ReadingResponse || !currentResourceState.ProcessingMissingProjectedNestedResourceInfos ? this.jsonLightResourceDeserializer.ReadResourceContent(currentResourceState) : (ODataJsonLightReaderNestedInfo) currentResourceState.Resource.MetadataBuilder.GetNextUnprocessedNavigationLink();
      if (nestedInfo == null)
        this.EndEntry();
      else
        this.ReadNestedInfo(nestedInfo);
      return true;
    }

    private void ReadNestedInfo(ODataJsonLightReaderNestedInfo nestedInfo)
    {
      switch (nestedInfo)
      {
        case ODataJsonLightReaderNestedResourceInfo readerNestedResourceInfo:
          this.StartNestedResourceInfo(readerNestedResourceInfo);
          break;
        case ODataJsonLightReaderNestedPropertyInfo readerNestedPropertyInfo:
          this.StartNestedPropertyInfo(readerNestedPropertyInfo);
          break;
      }
    }

    private bool ReadAtEntityReferenceLinkSynchronously()
    {
      this.PopScope(ODataReaderState.EntityReferenceLink);
      this.ReadNextNestedResourceInfoContentItemInRequest();
      return true;
    }

    private void ReadResourceSetStart(
      ODataResourceSet resourceSet,
      SelectedPropertiesNode selectedProperties)
    {
      this.jsonLightResourceDeserializer.ReadResourceSetContentStart();
      IJsonReader jsonReader = (IJsonReader) this.jsonLightResourceDeserializer.JsonReader;
      if (jsonReader.NodeType != JsonNodeType.EndArray && jsonReader.NodeType != JsonNodeType.StartObject && jsonReader.NodeType != JsonNodeType.PrimitiveValue && jsonReader.NodeType != JsonNodeType.StartArray)
        throw new ODataException(Microsoft.OData.Strings.ODataJsonLightResourceDeserializer_InvalidNodeTypeForItemsInResourceSet((object) jsonReader.NodeType));
      this.EnterScope((ODataReaderCore.Scope) new ODataJsonLightReader.JsonLightResourceSetScope((ODataResourceSetBase) resourceSet, this.CurrentNavigationSource, this.CurrentScope.ResourceTypeReference, selectedProperties, this.CurrentScope.ODataUri, false));
    }

    private void ReadResourceSetEnd()
    {
      this.jsonLightResourceDeserializer.ReadResourceSetContentEnd();
      ODataJsonLightReaderNestedResourceInfo expandedNestedResourceInfo = (ODataJsonLightReaderNestedResourceInfo) null;
      ODataJsonLightReader.JsonLightNestedResourceInfoScope contentParentScope = (ODataJsonLightReader.JsonLightNestedResourceInfoScope) this.ExpandedLinkContentParentScope;
      if (contentParentScope != null)
        expandedNestedResourceInfo = contentParentScope.ReaderNestedResourceInfo;
      if (!this.IsReadingNestedPayload && (this.IsExpandedLinkContent || this.IsTopLevel))
        this.jsonLightResourceDeserializer.ReadNextLinkAnnotationAtResourceSetEnd(this.Item as ODataResourceSetBase, expandedNestedResourceInfo, this.topLevelScope.PropertyAndAnnotationCollector);
      this.ReplaceScope(this.State == ODataReaderState.ResourceSetStart ? ODataReaderState.ResourceSetEnd : ODataReaderState.DeltaResourceSetEnd);
    }

    private void ReadExpandedNestedResourceInfoStart(ODataNestedResourceInfo nestedResourceInfo)
    {
      if (this.jsonLightResourceDeserializer.JsonReader.NodeType == JsonNodeType.PrimitiveValue)
      {
        IEdmStructuralProperty structuralProperty = this.CurrentJsonLightNestedResourceInfoScope.ReaderNestedResourceInfo.StructuralProperty;
        if (structuralProperty != null && !structuralProperty.Type.IsNullable && (this.jsonLightResourceDeserializer.ReadingResponse ? 0 : (int) this.jsonLightResourceDeserializer.Model.NullValueReadBehaviorKind((IEdmProperty) structuralProperty)) == 0)
          throw new ODataException(Microsoft.OData.Strings.ReaderValidationUtils_NullNamedValueForNonNullableType((object) nestedResourceInfo.Name, (object) structuralProperty.Type.FullName()));
        this.EnterScope((ODataReaderCore.Scope) new ODataJsonLightReader.JsonLightResourceScope(ODataReaderState.ResourceStart, (ODataResourceBase) null, this.CurrentNavigationSource, this.CurrentResourceTypeReference, (PropertyAndAnnotationCollector) null, (SelectedPropertiesNode) null, this.CurrentScope.ODataUri));
      }
      else
      {
        ODataJsonLightReader.JsonLightResourceBaseScope parentScope = (ODataJsonLightReader.JsonLightResourceBaseScope) this.ParentScope;
        this.ReadResourceSetItemStart((PropertyAndAnnotationCollector) null, parentScope.SelectedProperties.GetSelectedPropertiesForNavigationProperty(parentScope.ResourceType, nestedResourceInfo.Name));
      }
    }

    private void ReadExpandedNestedResourceInfoEnd(bool isCollection)
    {
      this.CurrentNestedResourceInfo.IsCollection = new bool?(isCollection);
      ((IODataJsonLightReaderResourceState) this.ParentScope).NavigationPropertiesRead.Add(this.CurrentNestedResourceInfo.Name);
      this.ReplaceScope(ODataReaderState.NestedResourceInfoEnd);
    }

    private void ReadResourceSetItemStart(
      PropertyAndAnnotationCollector propertyAndAnnotationCollector,
      SelectedPropertiesNode selectedProperties)
    {
      IEdmNavigationSource navigationSource = this.CurrentNavigationSource;
      IEdmTypeReference edmTypeReference = this.CurrentResourceTypeReference;
      if (this.jsonLightResourceDeserializer.JsonReader.NodeType == JsonNodeType.PrimitiveValue)
      {
        object obj = this.jsonLightResourceDeserializer.JsonReader.Value;
        if (obj != null)
        {
          if (this.CurrentResourceType.TypeKind != EdmTypeKind.Untyped)
            throw new ODataException(Microsoft.OData.Strings.ODataJsonLightReader_UnexpectedPrimitiveValueForODataResource);
          this.EnterScope((ODataReaderCore.Scope) new ODataJsonLightReader.JsonLightPrimitiveScope((ODataValue) new ODataPrimitiveValue(obj), this.CurrentNavigationSource, this.CurrentResourceTypeReference, this.CurrentScope.ODataUri));
        }
        else
        {
          if (edmTypeReference.IsComplex() || edmTypeReference.IsUntyped())
            this.jsonLightResourceDeserializer.MessageReaderSettings.Validator.ValidateNullValue(this.CurrentResourceTypeReference, true, "", new bool?());
          this.EnterScope((ODataReaderCore.Scope) new ODataJsonLightReader.JsonLightResourceScope(ODataReaderState.ResourceStart, (ODataResourceBase) null, this.CurrentNavigationSource, this.CurrentResourceTypeReference, (PropertyAndAnnotationCollector) null, (SelectedPropertiesNode) null, this.CurrentScope.ODataUri));
        }
      }
      else
      {
        if (this.jsonLightResourceDeserializer.JsonReader.NodeType == JsonNodeType.StartObject)
          this.jsonLightResourceDeserializer.JsonReader.Read();
        ODataDeltaKind odataDeltaKind = ODataDeltaKind.Resource;
        if (this.ReadingResourceSet || this.IsExpandedLinkContent || this.ReadingDelta && !this.IsTopLevel)
        {
          string uriFromPayload = this.jsonLightResourceDeserializer.ReadContextUriAnnotation(ODataPayloadKind.Resource, propertyAndAnnotationCollector, false);
          if (uriFromPayload != null)
          {
            ODataJsonLightContextUriParseResult contextUriParseResult = ODataJsonLightContextUriParser.Parse(this.jsonLightResourceDeserializer.Model, UriUtils.UriToString(this.jsonLightResourceDeserializer.ProcessUriFromPayload(uriFromPayload)), this.ReadingDelta ? ODataPayloadKind.Delta : ODataPayloadKind.Resource, this.jsonLightResourceDeserializer.MessageReaderSettings.ClientCustomTypeResolver, this.jsonLightInputContext.ReadingResponse || this.ReadingDelta);
            if (contextUriParseResult != null)
            {
              odataDeltaKind = contextUriParseResult.DeltaKind;
              if (this.ReadingDelta && this.IsTopLevel && (odataDeltaKind == ODataDeltaKind.Resource || odataDeltaKind == ODataDeltaKind.DeletedEntry))
              {
                if (contextUriParseResult.EdmType is IEdmStructuredType edmType)
                {
                  edmTypeReference = edmType.ToTypeReference(true);
                  navigationSource = contextUriParseResult.NavigationSource;
                }
              }
              else
                ReaderValidationUtils.ValidateResourceSetOrResourceContextUri(contextUriParseResult, this.CurrentScope, false);
            }
          }
        }
        ODataDeletedResource deletedResource = (ODataDeletedResource) null;
        if (this.ReadingDelta && (odataDeltaKind == ODataDeltaKind.Resource || odataDeltaKind == ODataDeltaKind.DeletedEntry))
        {
          deletedResource = this.jsonLightResourceDeserializer.IsDeletedResource();
          if (deletedResource != null)
            odataDeltaKind = ODataDeltaKind.DeletedEntry;
        }
        switch (odataDeltaKind)
        {
          case ODataDeltaKind.None:
          case ODataDeltaKind.Resource:
            this.StartResource(navigationSource, edmTypeReference, propertyAndAnnotationCollector, selectedProperties);
            this.StartReadingResource();
            break;
          case ODataDeltaKind.ResourceSet:
            this.ReadAtResourceSetStartImplementation();
            break;
          case ODataDeltaKind.DeletedEntry:
            if (deletedResource == null)
            {
              this.StartDeletedResource(this.jsonLightResourceDeserializer.ReadDeletedEntry(), navigationSource, edmTypeReference, propertyAndAnnotationCollector, selectedProperties, true);
              break;
            }
            this.StartDeletedResource(deletedResource, navigationSource, edmTypeReference, propertyAndAnnotationCollector, selectedProperties);
            this.StartReadingResource();
            break;
          case ODataDeltaKind.Link:
            this.StartDeltaLink(ODataReaderState.DeltaLink);
            break;
          case ODataDeltaKind.DeletedLink:
            this.StartDeltaLink(ODataReaderState.DeltaDeletedLink);
            break;
        }
      }
    }

    private void ReadDeltaResourceSetStart(
      ODataDeltaResourceSet deltaResourceSet,
      SelectedPropertiesNode selectedProperties)
    {
      this.jsonLightResourceDeserializer.ReadResourceSetContentStart();
      IJsonReader jsonReader = (IJsonReader) this.jsonLightResourceDeserializer.JsonReader;
      if (jsonReader.NodeType != JsonNodeType.EndArray && jsonReader.NodeType != JsonNodeType.StartObject)
        throw new ODataException(Microsoft.OData.Strings.ODataJsonLightResourceDeserializer_InvalidNodeTypeForItemsInResourceSet((object) jsonReader.NodeType));
      this.EnterScope((ODataReaderCore.Scope) new ODataJsonLightReader.JsonLightResourceSetScope((ODataResourceSetBase) deltaResourceSet, this.CurrentNavigationSource, (IEdmTypeReference) (this.CurrentResourceTypeReference as IEdmEntityTypeReference), selectedProperties, this.CurrentScope.ODataUri, true));
    }

    private void StartReadingResource()
    {
      ODataResourceBase odataResourceBase = this.Item as ODataResourceBase;
      this.jsonLightResourceDeserializer.ReadResourceTypeName(this.CurrentResourceState);
      this.ApplyResourceTypeNameFromPayload(odataResourceBase.TypeName);
      if (this.CurrentDerivedTypeValidator != null)
        this.CurrentDerivedTypeValidator.ValidateResourceType(this.CurrentResourceType);
      if (this.CurrentResourceSetValidator != null && (!this.ReadingDelta || this.CurrentResourceDepth != 0))
        this.CurrentResourceSetValidator.ValidateResource(this.CurrentResourceType);
      this.CurrentResourceState.FirstNestedInfo = this.jsonLightResourceDeserializer.ReadResourceContent(this.CurrentResourceState);
    }

    private void ReadNextResourceSetItem()
    {
      IEdmType resourceType = this.CurrentScope.ResourceType;
      switch (this.jsonLightResourceDeserializer.JsonReader.NodeType)
      {
        case JsonNodeType.StartObject:
          this.ReadResourceSetItemStart((PropertyAndAnnotationCollector) null, this.CurrentJsonLightResourceSetScope.SelectedProperties);
          break;
        case JsonNodeType.StartArray:
          this.ReadResourceSetStart(new ODataResourceSet(), SelectedPropertiesNode.EntireSubtree);
          break;
        case JsonNodeType.EndArray:
          this.ReadResourceSetEnd();
          break;
        case JsonNodeType.PrimitiveValue:
          if (this.TryReadPrimitiveAsStream(resourceType))
            break;
          object obj = this.jsonLightResourceDeserializer.JsonReader.Value;
          if (obj != null)
          {
            this.EnterScope((ODataReaderCore.Scope) new ODataJsonLightReader.JsonLightPrimitiveScope((ODataValue) new ODataPrimitiveValue(obj), this.CurrentNavigationSource, this.CurrentResourceTypeReference, this.CurrentScope.ODataUri));
            break;
          }
          if (resourceType.TypeKind == EdmTypeKind.Primitive || resourceType.TypeKind == EdmTypeKind.Enum)
          {
            this.EnterScope((ODataReaderCore.Scope) new ODataJsonLightReader.JsonLightPrimitiveScope((ODataValue) new ODataNullValue(), this.CurrentNavigationSource, this.CurrentResourceTypeReference, this.CurrentScope.ODataUri));
            break;
          }
          this.ReadResourceSetItemStart((PropertyAndAnnotationCollector) null, this.CurrentJsonLightResourceSetScope.SelectedProperties);
          break;
        default:
          throw new ODataException(Microsoft.OData.Strings.ODataJsonReader_CannotReadResourcesOfResourceSet((object) this.jsonLightResourceDeserializer.JsonReader.NodeType));
      }
    }

    private bool TryReadPrimitiveAsStream(IEdmType resourceType)
    {
      Func<IEdmPrimitiveType, bool, string, IEdmProperty, bool> readAsStreamFunc = this.jsonLightInputContext.MessageReaderSettings.ReadAsStreamFunc;
      if ((resourceType == null || !resourceType.IsStream()) && (resourceType == null || readAsStreamFunc == null || !resourceType.IsBinary() && !resourceType.IsString() || !readAsStreamFunc(resourceType as IEdmPrimitiveType, false, (string) null, (IEdmProperty) null)))
        return false;
      if (resourceType == null || resourceType.IsUntyped())
        this.StartNestedStreamInfo(new ODataJsonLightReaderStreamInfo(EdmPrimitiveTypeKind.None));
      else if (resourceType.IsString())
      {
        this.StartNestedStreamInfo(new ODataJsonLightReaderStreamInfo(EdmPrimitiveTypeKind.String));
      }
      else
      {
        if (!resourceType.IsStream() && !resourceType.IsBinary())
          return false;
        this.StartNestedStreamInfo(new ODataJsonLightReaderStreamInfo(EdmPrimitiveTypeKind.Binary));
      }
      return true;
    }

    private void ReadNextNestedResourceInfoContentItemInRequest()
    {
      ODataJsonLightReaderNestedResourceInfo nestedResourceInfo = this.CurrentJsonLightNestedResourceInfoScope.ReaderNestedResourceInfo;
      if (nestedResourceInfo.HasEntityReferenceLink)
        this.EnterScope(new ODataReaderCore.Scope(ODataReaderState.EntityReferenceLink, (ODataItem) nestedResourceInfo.ReportEntityReferenceLink(), this.CurrentScope.ODataUri));
      else if (nestedResourceInfo.HasValue)
      {
        bool? isCollection = nestedResourceInfo.NestedResourceInfo.IsCollection;
        bool flag = true;
        if (isCollection.GetValueOrDefault() == flag & isCollection.HasValue)
        {
          SelectedPropertiesNode entireSubtree = SelectedPropertiesNode.EntireSubtree;
          if (nestedResourceInfo.NestedResourceSet is ODataDeltaResourceSet nestedResourceSet)
          {
            this.ReadDeltaResourceSetStart(nestedResourceSet, entireSubtree);
          }
          else
          {
            if (!(nestedResourceInfo.NestedResourceSet is ODataResourceSet resourceSet))
              resourceSet = new ODataResourceSet();
            this.ReadResourceSetStart(resourceSet, entireSubtree);
          }
        }
        else
          this.ReadExpandedNestedResourceInfoStart(nestedResourceInfo.NestedResourceInfo);
      }
      else
        this.ReplaceScope(ODataReaderState.NestedResourceInfoEnd);
    }

    private void StartResource(
      IEdmNavigationSource source,
      IEdmTypeReference resourceType,
      PropertyAndAnnotationCollector propertyAndAnnotationCollector,
      SelectedPropertiesNode selectedProperties)
    {
      this.EnterScope((ODataReaderCore.Scope) new ODataJsonLightReader.JsonLightResourceScope(ODataReaderState.ResourceStart, (ODataResourceBase) ReaderUtils.CreateNewResource(), source, resourceType, propertyAndAnnotationCollector ?? this.jsonLightInputContext.CreatePropertyAndAnnotationCollector(), selectedProperties, this.CurrentScope.ODataUri));
    }

    private void StartDeletedResource(
      ODataDeletedResource deletedResource,
      IEdmNavigationSource source,
      IEdmTypeReference resourceType,
      PropertyAndAnnotationCollector propertyAndAnnotationCollector,
      SelectedPropertiesNode selectedProperties,
      bool is40DeletedResource = false)
    {
      this.EnterScope((ODataReaderCore.Scope) new ODataJsonLightReader.JsonLightDeletedResourceScope(ODataReaderState.DeletedResourceStart, deletedResource, source, resourceType, propertyAndAnnotationCollector ?? this.jsonLightInputContext.CreatePropertyAndAnnotationCollector(), selectedProperties, this.CurrentScope.ODataUri, is40DeletedResource));
    }

    private void StartDeltaLink(ODataReaderState state)
    {
      ODataDeltaLinkBase link = state != ODataReaderState.DeltaLink ? (ODataDeltaLinkBase) new ODataDeltaDeletedLink((Uri) null, (Uri) null, (string) null) : (ODataDeltaLinkBase) new ODataDeltaLink((Uri) null, (Uri) null, (string) null);
      this.EnterScope((ODataReaderCore.Scope) new ODataJsonLightReader.JsonLightDeltaLinkScope(state, link, this.CurrentNavigationSource, this.CurrentResourceType as IEdmEntityType, this.CurrentScope.ODataUri));
      this.jsonLightResourceDeserializer.ReadDeltaLinkSource(link);
      this.jsonLightResourceDeserializer.ReadDeltaLinkRelationship(link);
      this.jsonLightResourceDeserializer.ReadDeltaLinkTarget(link);
    }

    private void StartNestedResourceInfo(
      ODataJsonLightReaderNestedResourceInfo readerNestedResourceInfo)
    {
      ODataNestedResourceInfo nestedResourceInfo = readerNestedResourceInfo.NestedResourceInfo;
      IEdmProperty nestedProperty = readerNestedResourceInfo.NestedProperty;
      IEdmTypeReference expectedTypeReference = readerNestedResourceInfo.NestedResourceTypeReference;
      if (expectedTypeReference == null && nestedProperty != null)
      {
        IEdmTypeReference type = nestedProperty.Type;
        expectedTypeReference = type.IsCollection() ? (IEdmTypeReference) type.AsCollection().ElementType().AsStructured() : (IEdmTypeReference) type.AsStructured();
      }
      if (this.jsonLightInputContext.ReadingResponse && !this.IsReadingNestedPayload && (expectedTypeReference == null || expectedTypeReference.Definition.IsStructuredOrStructuredCollectionType()))
      {
        this.CurrentResourceState.ResourceTypeFromMetadata = this.ParentScope.ResourceType as IEdmStructuredType;
        ODataResourceMetadataBuilder builderForReader = this.jsonLightResourceDeserializer.MetadataContext.GetResourceMetadataBuilderForReader(this.CurrentResourceState, this.jsonLightInputContext.ODataSimplifiedOptions.EnableReadingKeyAsSegment, this.ReadingDelta);
        nestedResourceInfo.MetadataBuilder = builderForReader;
      }
      IEdmNavigationProperty navigationProperty = readerNestedResourceInfo.NavigationProperty;
      ODataJsonLightReader.JsonLightResourceBaseScope currentScope = this.CurrentScope as ODataJsonLightReader.JsonLightResourceBaseScope;
      ODataUri odataUri = this.CurrentScope.ODataUri.Clone();
      ODataPath odataPath1 = odataUri.Path ?? new ODataPath(new ODataPathSegment[0]);
      if (currentScope != null && currentScope.ResourceTypeFromMetadata != currentScope.ResourceType)
        odataPath1.Add((ODataPathSegment) new TypeSegment((IEdmType) currentScope.ResourceType, (IEdmNavigationSource) null));
      IEdmNavigationSource navigationSource = navigationProperty != null ? (this.CurrentNavigationSource == null ? (IEdmNavigationSource) null : this.CurrentNavigationSource.FindNavigationTarget(navigationProperty, new Func<IEdmPathExpression, List<ODataPathSegment>, bool>(BindingPathHelper.MatchBindingPath), odataPath1.ToList<ODataPathSegment>(), out IEdmPathExpression _)) : this.CurrentNavigationSource;
      if (navigationProperty != null)
      {
        ODataPath odataPath2;
        switch (navigationSource)
        {
          case IEdmContainedEntitySet _:
            if (this.TryAppendEntitySetKeySegment(ref odataPath1))
            {
              odataPath1 = odataPath1.AppendNavigationPropertySegment(navigationProperty, navigationSource);
              goto label_16;
            }
            else
              goto label_16;
          case null:
          case IEdmUnknownEntitySet _:
            odataPath1 = new ODataPath(new ODataPathSegment[0]);
            goto label_16;
          case IEdmEntitySet entitySet:
            odataPath2 = new ODataPath(new ODataPathSegment[1]
            {
              (ODataPathSegment) new EntitySetSegment(entitySet)
            });
            break;
          default:
            odataPath2 = new ODataPath(new ODataPathSegment[1]
            {
              (ODataPathSegment) new SingletonSegment(navigationSource as IEdmSingleton)
            });
            break;
        }
        odataPath1 = odataPath2;
      }
      else if (nestedProperty != null)
        odataPath1 = odataPath1.AppendPropertySegment(nestedProperty as IEdmStructuralProperty);
label_16:
      odataUri.Path = odataPath1;
      ODataJsonLightReader.JsonLightNestedResourceInfoScope resourceInfoScope = new ODataJsonLightReader.JsonLightNestedResourceInfoScope(readerNestedResourceInfo, navigationSource, expectedTypeReference, odataUri);
      IEnumerable<string> derivedTypeConstraints = this.jsonLightInputContext.Model.GetDerivedTypeConstraints((IEdmVocabularyAnnotatable) nestedProperty);
      if (derivedTypeConstraints != null)
        resourceInfoScope.DerivedTypeValidator = new DerivedTypeValidator((IEdmType) nestedProperty.Type.ToStructuredType(), derivedTypeConstraints, "nested resource", nestedProperty.Name);
      this.EnterScope((ODataReaderCore.Scope) resourceInfoScope);
    }

    private void StartNestedPropertyInfo(
      ODataJsonLightReaderNestedPropertyInfo readerNestedPropertyInfo)
    {
      this.EnterScope((ODataReaderCore.Scope) new ODataJsonLightReader.JsonLightNestedPropertyInfoScope(readerNestedPropertyInfo, this.CurrentNavigationSource, this.CurrentScope.ODataUri));
    }

    private void StartNestedStreamInfo(ODataJsonLightReaderStreamInfo readerStreamInfo) => this.EnterScope((ODataReaderCore.Scope) new ODataJsonLightReader.JsonLightStreamScope(readerStreamInfo, this.CurrentNavigationSource, this.CurrentScope.ODataUri));

    private bool TryAppendEntitySetKeySegment(ref ODataPath odataPath)
    {
      try
      {
        if (EdmExtensionMethods.HasKey(this.CurrentScope.NavigationSource, this.CurrentScope.ResourceType as IEdmStructuredType))
        {
          IEdmEntityType resourceType = this.CurrentScope.ResourceType as IEdmEntityType;
          KeyValuePair<string, object>[] keyProperties = ODataResourceMetadataContext.GetKeyProperties(this.CurrentScope.Item as ODataResourceBase, (ODataResourceSerializationInfo) null, resourceType);
          odataPath = odataPath.AppendKeySegment((IEnumerable<KeyValuePair<string, object>>) keyProperties, resourceType, this.CurrentScope.NavigationSource);
        }
      }
      catch (ODataException ex)
      {
        odataPath = (ODataPath) null;
        return false;
      }
      return true;
    }

    private void ReplaceScope(ODataReaderState state) => this.ReplaceScope(new ODataReaderCore.Scope(state, this.Item, this.CurrentNavigationSource, this.CurrentResourceTypeReference, this.CurrentScope.ODataUri));

    private void EndEntry()
    {
      IODataJsonLightReaderResourceState currentResourceState = this.CurrentResourceState;
      if (this.Item is ODataResourceBase odataResourceBase && !this.IsReadingNestedPayload)
      {
        foreach (string navigationPropertyName in this.CurrentResourceState.NavigationPropertiesRead)
          odataResourceBase.MetadataBuilder.MarkNestedResourceInfoProcessed(navigationPropertyName);
        if (odataResourceBase.MetadataBuilder is ODataConventionalEntityMetadataBuilder metadataBuilder)
          metadataBuilder.EndResource();
      }
      this.jsonLightResourceDeserializer.ValidateMediaEntity(currentResourceState);
      if (this.jsonLightInputContext.ReadingResponse && !this.ReadingDelta && odataResourceBase != null)
      {
        ODataJsonLightReaderNestedResourceInfo unprocessedNavigationLink = odataResourceBase.MetadataBuilder.GetNextUnprocessedNavigationLink();
        if (unprocessedNavigationLink != null)
        {
          this.CurrentResourceState.ProcessingMissingProjectedNestedResourceInfos = true;
          this.StartNestedResourceInfo(unprocessedNavigationLink);
          return;
        }
      }
      if (this.State == ODataReaderState.ResourceStart)
        this.EndEntry((ODataReaderCore.Scope) new ODataJsonLightReader.JsonLightResourceScope(ODataReaderState.ResourceEnd, (ODataResourceBase) this.Item, this.CurrentNavigationSource, this.CurrentResourceTypeReference, this.CurrentResourceState.PropertyAndAnnotationCollector, this.CurrentResourceState.SelectedProperties, this.CurrentScope.ODataUri));
      else
        this.EndEntry((ODataReaderCore.Scope) new ODataJsonLightReader.JsonLightDeletedResourceScope(ODataReaderState.DeletedResourceEnd, (ODataDeletedResource) this.Item, this.CurrentNavigationSource, this.CurrentResourceTypeReference, this.CurrentResourceState.PropertyAndAnnotationCollector, this.CurrentResourceState.SelectedProperties, this.CurrentScope.ODataUri));
    }

    private void ResolveScopeInfoFromContextUrl()
    {
      if (this.jsonLightResourceDeserializer.ContextUriParseResult == null)
        return;
      this.CurrentScope.ODataUri.Path = this.jsonLightResourceDeserializer.ContextUriParseResult.Path;
      if (this.CurrentScope.NavigationSource == null)
        this.CurrentScope.NavigationSource = this.jsonLightResourceDeserializer.ContextUriParseResult.NavigationSource;
      if (this.CurrentScope.ResourceType != null)
        return;
      IEdmType type1 = this.jsonLightResourceDeserializer.ContextUriParseResult.EdmType;
      if (type1 == null)
        return;
      if (type1.TypeKind == EdmTypeKind.Collection)
      {
        type1 = ((IEdmCollectionType) type1).ElementType.Definition;
        if (!(type1 is IEdmStructuredType))
        {
          type1 = (IEdmType) new EdmUntypedStructuredType();
          this.jsonLightResourceDeserializer.ContextUriParseResult.EdmType = (IEdmType) new EdmCollectionType(type1.ToTypeReference());
        }
      }
      if (!(type1 is IEdmStructuredType type2))
      {
        type2 = (IEdmStructuredType) new EdmUntypedStructuredType();
        this.jsonLightResourceDeserializer.ContextUriParseResult.EdmType = (IEdmType) type2;
      }
      this.CurrentScope.ResourceTypeReference = (IEdmTypeReference) type2.ToTypeReference(true).AsStructured();
    }

    private sealed class JsonLightTopLevelScope : ODataReaderCore.Scope
    {
      internal JsonLightTopLevelScope(
        IEdmNavigationSource navigationSource,
        IEdmStructuredType expectedResourceType,
        ODataUri odataUri)
        : base(ODataReaderState.Start, (ODataItem) null, navigationSource, expectedResourceType.ToTypeReference(true), odataUri)
      {
      }

      public PropertyAndAnnotationCollector PropertyAndAnnotationCollector { get; set; }
    }

    private sealed class JsonLightPrimitiveScope : ODataReaderCore.Scope
    {
      internal JsonLightPrimitiveScope(
        ODataValue primitiveValue,
        IEdmNavigationSource navigationSource,
        IEdmTypeReference expectedTypeReference,
        ODataUri odataUri)
        : base(ODataReaderState.Primitive, (ODataItem) primitiveValue, navigationSource, expectedTypeReference, odataUri)
      {
      }
    }

    private abstract class JsonLightResourceBaseScope : 
      ODataReaderCore.Scope,
      IODataJsonLightReaderResourceState
    {
      private List<string> navigationPropertiesRead;

      protected JsonLightResourceBaseScope(
        ODataReaderState readerState,
        ODataResourceBase resource,
        IEdmNavigationSource navigationSource,
        IEdmTypeReference expectedResourceTypeReference,
        PropertyAndAnnotationCollector propertyAndAnnotationCollector,
        SelectedPropertiesNode selectedProperties,
        ODataUri odataUri)
        : base(readerState, (ODataItem) resource, navigationSource, expectedResourceTypeReference, odataUri)
      {
        this.PropertyAndAnnotationCollector = propertyAndAnnotationCollector;
        this.SelectedProperties = selectedProperties;
      }

      public ODataResourceMetadataBuilder MetadataBuilder { get; set; }

      public bool AnyPropertyFound { get; set; }

      public ODataJsonLightReaderNestedInfo FirstNestedInfo { get; set; }

      public PropertyAndAnnotationCollector PropertyAndAnnotationCollector { get; private set; }

      public SelectedPropertiesNode SelectedProperties { get; private set; }

      public List<string> NavigationPropertiesRead => this.navigationPropertiesRead ?? (this.navigationPropertiesRead = new List<string>());

      public bool ProcessingMissingProjectedNestedResourceInfos { get; set; }

      public IEdmStructuredType ResourceTypeFromMetadata { get; set; }

      public IEdmStructuredType ResourceType => base.ResourceType as IEdmStructuredType;

      ODataResourceBase IODataJsonLightReaderResourceState.Resource => (ODataResourceBase) this.Item;

      IEdmStructuredType IODataJsonLightReaderResourceState.ResourceType => this.ResourceType;

      IEdmNavigationSource IODataJsonLightReaderResourceState.NavigationSource => this.NavigationSource;
    }

    private sealed class JsonLightResourceScope : ODataJsonLightReader.JsonLightResourceBaseScope
    {
      internal JsonLightResourceScope(
        ODataReaderState readerState,
        ODataResourceBase resource,
        IEdmNavigationSource navigationSource,
        IEdmTypeReference expectedResourceTypeReference,
        PropertyAndAnnotationCollector propertyAndAnnotationCollector,
        SelectedPropertiesNode selectedProperties,
        ODataUri odataUri)
        : base(readerState, resource, navigationSource, expectedResourceTypeReference, propertyAndAnnotationCollector, selectedProperties, odataUri)
      {
      }
    }

    private sealed class JsonLightDeletedResourceScope : 
      ODataJsonLightReader.JsonLightResourceBaseScope
    {
      internal JsonLightDeletedResourceScope(
        ODataReaderState readerState,
        ODataDeletedResource resource,
        IEdmNavigationSource navigationSource,
        IEdmTypeReference expectedResourceType,
        PropertyAndAnnotationCollector propertyAndAnnotationCollector,
        SelectedPropertiesNode selectedProperties,
        ODataUri odataUri,
        bool is40DeletedResource = false)
        : base(readerState, (ODataResourceBase) resource, navigationSource, expectedResourceType, propertyAndAnnotationCollector, selectedProperties, odataUri)
      {
        this.Is40DeletedResource = is40DeletedResource;
      }

      internal bool Is40DeletedResource { get; private set; }
    }

    private sealed class JsonLightResourceSetScope : ODataReaderCore.Scope
    {
      internal JsonLightResourceSetScope(
        ODataResourceSetBase resourceSet,
        IEdmNavigationSource navigationSource,
        IEdmTypeReference expectedResourceTypeReference,
        SelectedPropertiesNode selectedProperties,
        ODataUri odataUri,
        bool isDelta)
        : base(isDelta ? ODataReaderState.DeltaResourceSetStart : ODataReaderState.ResourceSetStart, (ODataItem) resourceSet, navigationSource, expectedResourceTypeReference, odataUri)
      {
        this.SelectedProperties = selectedProperties;
      }

      public SelectedPropertiesNode SelectedProperties { get; private set; }
    }

    private sealed class JsonLightNestedResourceInfoScope : ODataReaderCore.Scope
    {
      internal JsonLightNestedResourceInfoScope(
        ODataJsonLightReaderNestedResourceInfo nestedResourceInfo,
        IEdmNavigationSource navigationSource,
        IEdmTypeReference expectedTypeReference,
        ODataUri odataUri)
        : base(ODataReaderState.NestedResourceInfoStart, (ODataItem) nestedResourceInfo.NestedResourceInfo, navigationSource, expectedTypeReference, odataUri)
      {
        this.ReaderNestedResourceInfo = nestedResourceInfo;
      }

      public ODataJsonLightReaderNestedResourceInfo ReaderNestedResourceInfo { get; private set; }
    }

    private sealed class JsonLightNestedPropertyInfoScope : ODataReaderCore.Scope
    {
      internal JsonLightNestedPropertyInfoScope(
        ODataJsonLightReaderNestedPropertyInfo nestedPropertyInfo,
        IEdmNavigationSource navigationSource,
        ODataUri odataUri)
        : base(ODataReaderState.NestedProperty, (ODataItem) nestedPropertyInfo.NestedPropertyInfo, navigationSource, (IEdmTypeReference) EdmCoreModel.Instance.GetPrimitive(EdmPrimitiveTypeKind.Stream, true), odataUri)
      {
      }
    }

    private sealed class JsonLightStreamScope : ODataReaderCore.StreamScope
    {
      internal JsonLightStreamScope(
        ODataJsonLightReaderStreamInfo streamInfo,
        IEdmNavigationSource navigationSource,
        ODataUri odataUri)
        : base(ODataReaderState.Stream, (ODataItem) new ODataStreamItem(streamInfo.PrimitiveTypeKind, streamInfo.ContentType), navigationSource, (IEdmTypeReference) EdmCoreModel.Instance.GetPrimitive(EdmPrimitiveTypeKind.Stream, true), odataUri)
      {
      }
    }

    private sealed class JsonLightDeltaLinkScope : ODataReaderCore.Scope
    {
      public JsonLightDeltaLinkScope(
        ODataReaderState state,
        ODataDeltaLinkBase link,
        IEdmNavigationSource navigationSource,
        IEdmEntityType expectedEntityType,
        ODataUri odataUri)
        : base(state, (ODataItem) link, navigationSource, expectedEntityType.ToTypeReference(true), odataUri)
      {
      }
    }
  }
}
