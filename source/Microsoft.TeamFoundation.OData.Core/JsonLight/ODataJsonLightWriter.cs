// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.JsonLight.ODataJsonLightWriter
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Edm;
using Microsoft.OData.Evaluation;
using Microsoft.OData.Json;
using Microsoft.OData.Metadata;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.OData.JsonLight
{
  internal sealed class ODataJsonLightWriter : ODataWriterCore
  {
    private readonly ODataJsonLightOutputContext jsonLightOutputContext;
    private readonly ODataJsonLightResourceSerializer jsonLightResourceSerializer;
    private readonly ODataJsonLightValueSerializer jsonLightValueSerializer;
    private readonly ODataJsonLightPropertySerializer jsonLightPropertySerializer;
    private readonly bool writingParameter;
    private readonly IJsonWriter jsonWriter;
    private readonly IJsonStreamWriter jsonStreamWriter;
    private readonly JsonLightODataAnnotationWriter odataAnnotationWriter;

    internal ODataJsonLightWriter(
      ODataJsonLightOutputContext jsonLightOutputContext,
      IEdmNavigationSource navigationSource,
      IEdmStructuredType resourceType,
      bool writingResourceSet,
      bool writingParameter = false,
      bool writingDelta = false,
      IODataReaderWriterListener listener = null)
      : base((ODataOutputContext) jsonLightOutputContext, navigationSource, resourceType, writingResourceSet, writingDelta, listener)
    {
      this.jsonLightOutputContext = jsonLightOutputContext;
      this.jsonLightResourceSerializer = new ODataJsonLightResourceSerializer(this.jsonLightOutputContext);
      this.jsonLightValueSerializer = new ODataJsonLightValueSerializer(this.jsonLightOutputContext);
      this.jsonLightPropertySerializer = new ODataJsonLightPropertySerializer(this.jsonLightOutputContext);
      this.writingParameter = writingParameter;
      this.jsonWriter = this.jsonLightOutputContext.JsonWriter;
      this.jsonStreamWriter = this.jsonWriter as IJsonStreamWriter;
      this.odataAnnotationWriter = new JsonLightODataAnnotationWriter(this.jsonWriter, this.jsonLightOutputContext.OmitODataPrefix, this.jsonLightOutputContext.MessageWriterSettings.Version);
    }

    private ODataJsonLightWriter.JsonLightResourceScope CurrentResourceScope => this.CurrentScope as ODataJsonLightWriter.JsonLightResourceScope;

    private ODataJsonLightWriter.JsonLightDeletedResourceScope CurrentDeletedResourceScope => this.CurrentScope as ODataJsonLightWriter.JsonLightDeletedResourceScope;

    private ODataJsonLightWriter.JsonLightDeltaLinkScope CurrentDeltaLinkScope => this.CurrentScope as ODataJsonLightWriter.JsonLightDeltaLinkScope;

    private ODataJsonLightWriter.JsonLightResourceSetScope CurrentResourceSetScope => this.CurrentScope as ODataJsonLightWriter.JsonLightResourceSetScope;

    private ODataJsonLightWriter.JsonLightDeltaResourceSetScope CurrentDeltaResourceSetScope => this.CurrentScope as ODataJsonLightWriter.JsonLightDeltaResourceSetScope;

    protected override void VerifyNotDisposed() => this.jsonLightOutputContext.VerifyNotDisposed();

    protected override void FlushSynchronously() => this.jsonLightOutputContext.Flush();

    protected override Task FlushAsynchronously() => this.jsonLightOutputContext.FlushAsync();

    protected override void StartPayload() => this.jsonLightResourceSerializer.WritePayloadStart();

    protected override void EndPayload() => this.jsonLightResourceSerializer.WritePayloadEnd();

    protected override void PrepareResourceForWriteStart(
      ODataWriterCore.ResourceScope resourceScope,
      ODataResource resource,
      bool writingResponse,
      SelectedPropertiesNode selectedProperties)
    {
      ODataResourceTypeContext typeContext = resourceScope.GetOrCreateTypeContext(writingResponse);
      if (this.jsonLightOutputContext.MetadataLevel is JsonNoMetadataLevel)
      {
        this.InnerPrepareResourceForWriteStart((ODataResourceBase) resource, typeContext, selectedProperties);
      }
      else
      {
        if (!this.jsonLightOutputContext.Model.IsUserModel() && resourceScope.SerializationInfo == null)
          return;
        this.InnerPrepareResourceForWriteStart((ODataResourceBase) resource, typeContext, selectedProperties);
      }
    }

    protected override void PrepareDeletedResourceForWriteStart(
      ODataWriterCore.DeletedResourceScope resourceScope,
      ODataDeletedResource deletedResource,
      bool writingResponse,
      SelectedPropertiesNode selectedProperties)
    {
      ODataVersion? version = this.jsonLightOutputContext.MessageWriterSettings.Version;
      ODataVersion odataVersion = ODataVersion.V4;
      if (!(version.GetValueOrDefault() > odataVersion & version.HasValue))
        return;
      ODataResourceTypeContext typeContext = resourceScope.GetOrCreateTypeContext(writingResponse);
      if (this.jsonLightOutputContext.MetadataLevel is JsonNoMetadataLevel)
      {
        this.InnerPrepareResourceForWriteStart((ODataResourceBase) deletedResource, typeContext, selectedProperties);
      }
      else
      {
        if (!this.jsonLightOutputContext.Model.IsUserModel() && resourceScope.SerializationInfo == null)
          return;
        this.InnerPrepareResourceForWriteStart((ODataResourceBase) deletedResource, typeContext, selectedProperties);
      }
    }

    protected override void StartProperty(ODataPropertyInfo property)
    {
      ODataWriterCore.ResourceBaseScope parentScope = this.ParentScope as ODataWriterCore.ResourceBaseScope;
      ODataResource odataResource = parentScope.Item as ODataResource;
      if (property is ODataProperty property1)
        this.jsonLightPropertySerializer.WriteProperty(property1, parentScope.ResourceType, false, this.ShouldOmitNullValues(), this.DuplicatePropertyNameChecker, odataResource.MetadataBuilder);
      else
        this.jsonLightPropertySerializer.WritePropertyInfo(property, parentScope.ResourceType, false, this.DuplicatePropertyNameChecker, odataResource.MetadataBuilder);
    }

    protected override void EndProperty(ODataPropertyInfo property)
    {
    }

    protected override void StartResource(ODataResource resource)
    {
      ODataNestedResourceInfo nestedResourceInfo = this.ParentNestedResourceInfo;
      if (nestedResourceInfo != null)
      {
        if (resource == null)
        {
          if (nestedResourceInfo.TypeAnnotation != null && nestedResourceInfo.TypeAnnotation.TypeName != null)
            this.jsonLightResourceSerializer.ODataAnnotationWriter.WriteODataTypePropertyAnnotation(nestedResourceInfo.Name, nestedResourceInfo.TypeAnnotation.TypeName);
          this.jsonLightResourceSerializer.InstanceAnnotationWriter.WriteInstanceAnnotations((IEnumerable<ODataInstanceAnnotation>) nestedResourceInfo.GetInstanceAnnotations(), nestedResourceInfo.Name);
          if (!this.ShouldOmitNullValues() || nestedResourceInfo.GetInstanceAnnotations().Count > 0)
          {
            this.jsonWriter.WriteName(nestedResourceInfo.Name);
            this.jsonWriter.WriteValue((string) null);
            return;
          }
        }
        else
          this.jsonWriter.WriteName(nestedResourceInfo.Name);
      }
      if (resource == null)
      {
        if (this.ShouldOmitNullValues())
          return;
        this.jsonWriter.WriteValue((string) null);
      }
      else
      {
        this.jsonWriter.StartObjectScope();
        ODataJsonLightWriter.JsonLightResourceScope currentResourceScope = this.CurrentResourceScope;
        if (this.IsTopLevel && !(this.jsonLightOutputContext.MetadataLevel is JsonNoMetadataLevel))
        {
          ODataContextUrlInfo odataContextUrlInfo = this.jsonLightResourceSerializer.WriteResourceContextUri(currentResourceScope.GetOrCreateTypeContext(this.jsonLightOutputContext.WritingResponse));
          if (odataContextUrlInfo != null)
          {
            ODataJsonLightWriter.JsonLightResourceScope lightResourceScope = currentResourceScope;
            bool? isUndeclared = odataContextUrlInfo.IsUndeclared;
            int num;
            if (isUndeclared.HasValue)
            {
              isUndeclared = odataContextUrlInfo.IsUndeclared;
              num = isUndeclared.Value ? 1 : 0;
            }
            else
              num = 0;
            lightResourceScope.IsUndeclared = num != 0;
          }
        }
        else if (this.ParentScope.State == ODataWriterCore.WriterState.DeltaResourceSet && this.ScopeLevel == 3)
        {
          ODataWriterCore.DeltaResourceSetScope parentScope = this.ParentScope as ODataWriterCore.DeltaResourceSetScope;
          string name = parentScope.NavigationSource == null ? (string) null : parentScope.NavigationSource.Name;
          string str = resource.SerializationInfo != null ? resource.SerializationInfo.NavigationSourceName : (currentResourceScope.NavigationSource == null ? (string) null : currentResourceScope.NavigationSource.Name);
          if (string.IsNullOrEmpty(str) || str != name)
            this.jsonLightResourceSerializer.WriteDeltaContextUri(this.CurrentResourceScope.GetOrCreateTypeContext(true), ODataDeltaKind.Resource, parentScope.ContextUriInfo);
        }
        this.jsonLightResourceSerializer.WriteResourceStartMetadataProperties((IODataJsonLightWriterResourceState) currentResourceScope);
        this.jsonLightResourceSerializer.WriteResourceMetadataProperties((IODataJsonLightWriterResourceState) currentResourceScope);
        this.jsonLightOutputContext.PropertyCacheHandler.SetCurrentResourceScopeLevel(this.ScopeLevel);
        this.jsonLightResourceSerializer.InstanceAnnotationWriter.WriteInstanceAnnotations((IEnumerable<ODataInstanceAnnotation>) resource.InstanceAnnotations, currentResourceScope.InstanceAnnotationWriteTracker);
        if (resource.NonComputedProperties == null)
          return;
        this.jsonLightResourceSerializer.WriteProperties(this.ResourceType, resource.NonComputedProperties, false, this.ShouldOmitNullValues(), this.DuplicatePropertyNameChecker, resource.MetadataBuilder);
      }
    }

    protected override void EndResource(ODataResource resource)
    {
      if (resource == null)
        return;
      ODataJsonLightWriter.JsonLightResourceScope currentResourceScope = this.CurrentResourceScope;
      this.jsonLightResourceSerializer.WriteResourceMetadataProperties((IODataJsonLightWriterResourceState) currentResourceScope);
      this.jsonLightResourceSerializer.InstanceAnnotationWriter.WriteInstanceAnnotations((IEnumerable<ODataInstanceAnnotation>) resource.InstanceAnnotations, currentResourceScope.InstanceAnnotationWriteTracker);
      this.jsonLightResourceSerializer.WriteResourceEndMetadataProperties((IODataJsonLightWriterResourceState) currentResourceScope, currentResourceScope.DuplicatePropertyNameChecker);
      this.jsonWriter.EndObjectScope();
    }

    protected override void EndDeletedResource(ODataDeletedResource deletedResource)
    {
      if (deletedResource == null)
        return;
      this.jsonWriter.EndObjectScope();
    }

    protected override void StartResourceSet(ODataResourceSet resourceSet)
    {
      if (this.ParentNestedResourceInfo == null && (this.writingParameter || this.ParentScope.State == ODataWriterCore.WriterState.ResourceSet))
        this.jsonWriter.StartArrayScope();
      else if (this.ParentNestedResourceInfo == null)
      {
        this.jsonWriter.StartObjectScope();
        this.jsonLightResourceSerializer.WriteResourceSetContextUri(this.CurrentResourceSetScope.GetOrCreateTypeContext(this.jsonLightOutputContext.WritingResponse));
        if (this.jsonLightOutputContext.WritingResponse)
        {
          IEnumerable<ODataAction> actions = resourceSet.Actions;
          if (actions != null && actions.Any<ODataAction>())
            this.jsonLightResourceSerializer.WriteOperations(actions.Cast<ODataOperation>(), true);
          IEnumerable<ODataFunction> functions = resourceSet.Functions;
          if (functions != null && functions.Any<ODataFunction>())
            this.jsonLightResourceSerializer.WriteOperations(functions.Cast<ODataOperation>(), false);
          this.WriteResourceSetCount(resourceSet.Count, (string) null);
          this.WriteResourceSetNextLink(resourceSet.NextPageLink, (string) null);
          this.WriteResourceSetDeltaLink(resourceSet.DeltaLink);
        }
        this.jsonLightResourceSerializer.InstanceAnnotationWriter.WriteInstanceAnnotations((IEnumerable<ODataInstanceAnnotation>) resourceSet.InstanceAnnotations, this.CurrentResourceSetScope.InstanceAnnotationWriteTracker);
        this.jsonWriter.WriteValuePropertyName();
        this.jsonWriter.StartArrayScope();
      }
      else
      {
        this.ValidateNoDeltaLinkForExpandedResourceSet(resourceSet);
        this.ValidateNoCustomInstanceAnnotationsForExpandedResourceSet(resourceSet);
        string name = this.ParentNestedResourceInfo.Name;
        bool isUndeclared = (this.CurrentScope as ODataJsonLightWriter.JsonLightResourceSetScope).IsUndeclared;
        string resourceTypeName = this.CurrentResourceSetScope.GetOrCreateTypeContext(this.jsonLightOutputContext.WritingResponse).ExpectedResourceTypeName;
        if (this.jsonLightOutputContext.WritingResponse)
        {
          this.WriteResourceSetCount(resourceSet.Count, name);
          this.WriteResourceSetNextLink(resourceSet.NextPageLink, name);
          this.jsonLightResourceSerializer.WriteResourceSetStartMetadataProperties(resourceSet, name, resourceTypeName, isUndeclared);
          this.jsonWriter.WriteName(name);
          this.jsonWriter.StartArrayScope();
        }
        else
        {
          ODataJsonLightWriter.JsonLightNestedResourceInfoScope resourceInfoScope = (ODataJsonLightWriter.JsonLightNestedResourceInfoScope) this.ParentNestedResourceInfoScope;
          if (!resourceInfoScope.ResourceSetWritten)
          {
            if (resourceInfoScope.EntityReferenceLinkWritten)
              this.jsonWriter.EndArrayScope();
            this.jsonLightResourceSerializer.WriteResourceSetStartMetadataProperties(resourceSet, name, resourceTypeName, isUndeclared);
            this.jsonWriter.WriteName(name);
            this.jsonWriter.StartArrayScope();
            resourceInfoScope.ResourceSetWritten = true;
          }
        }
      }
      this.jsonLightOutputContext.PropertyCacheHandler.EnterResourceSetScope(this.CurrentResourceSetScope.ResourceType, this.ScopeLevel);
    }

    protected override void EndResourceSet(ODataResourceSet resourceSet)
    {
      if (this.ParentNestedResourceInfo == null && (this.writingParameter || this.ParentScope.State == ODataWriterCore.WriterState.ResourceSet))
        this.jsonWriter.EndArrayScope();
      else if (this.ParentNestedResourceInfo == null)
      {
        this.jsonWriter.EndArrayScope();
        this.jsonLightResourceSerializer.InstanceAnnotationWriter.WriteInstanceAnnotations((IEnumerable<ODataInstanceAnnotation>) resourceSet.InstanceAnnotations, this.CurrentResourceSetScope.InstanceAnnotationWriteTracker);
        if (this.jsonLightOutputContext.WritingResponse)
        {
          this.WriteResourceSetNextLink(resourceSet.NextPageLink, (string) null);
          this.WriteResourceSetDeltaLink(resourceSet.DeltaLink);
        }
        this.jsonWriter.EndObjectScope();
      }
      else
      {
        string name = this.ParentNestedResourceInfo.Name;
        this.ValidateNoDeltaLinkForExpandedResourceSet(resourceSet);
        this.ValidateNoCustomInstanceAnnotationsForExpandedResourceSet(resourceSet);
        if (this.jsonLightOutputContext.WritingResponse)
        {
          this.jsonWriter.EndArrayScope();
          this.WriteResourceSetNextLink(resourceSet.NextPageLink, name);
        }
      }
      this.jsonLightOutputContext.PropertyCacheHandler.LeaveResourceSetScope();
    }

    protected override void StartDeltaResourceSet(ODataDeltaResourceSet deltaResourceSet)
    {
      if (this.ParentNestedResourceInfo == null)
      {
        this.jsonWriter.StartObjectScope();
        this.CurrentDeltaResourceSetScope.ContextUriInfo = this.jsonLightResourceSerializer.WriteDeltaContextUri(this.CurrentDeltaResourceSetScope.GetOrCreateTypeContext(this.jsonLightOutputContext.WritingResponse), ODataDeltaKind.ResourceSet);
        this.WriteResourceSetCount(deltaResourceSet.Count, (string) null);
        this.WriteResourceSetNextLink(deltaResourceSet.NextPageLink, (string) null);
        this.WriteResourceSetDeltaLink(deltaResourceSet.DeltaLink);
        this.jsonLightResourceSerializer.InstanceAnnotationWriter.WriteInstanceAnnotations((IEnumerable<ODataInstanceAnnotation>) deltaResourceSet.InstanceAnnotations, this.CurrentDeltaResourceSetScope.InstanceAnnotationWriteTracker);
        this.jsonWriter.WriteValuePropertyName();
        this.jsonWriter.StartArrayScope();
      }
      else
      {
        string name = this.ParentNestedResourceInfo.Name;
        this.WriteResourceSetCount(deltaResourceSet.Count, name);
        this.WriteResourceSetNextLink(deltaResourceSet.NextPageLink, name);
        this.jsonWriter.WritePropertyAnnotationName(name, "delta");
        this.jsonWriter.StartArrayScope();
      }
    }

    protected override void EndDeltaResourceSet(ODataDeltaResourceSet deltaResourceSet)
    {
      if (this.ParentNestedResourceInfo == null)
      {
        this.jsonWriter.EndArrayScope();
        this.jsonLightResourceSerializer.InstanceAnnotationWriter.WriteInstanceAnnotations((IEnumerable<ODataInstanceAnnotation>) deltaResourceSet.InstanceAnnotations, this.CurrentDeltaResourceSetScope.InstanceAnnotationWriteTracker);
        this.WriteResourceSetNextLink(deltaResourceSet.NextPageLink, (string) null);
        this.WriteResourceSetDeltaLink(deltaResourceSet.DeltaLink);
        this.jsonWriter.EndObjectScope();
      }
      else
        this.jsonWriter.EndArrayScope();
    }

    protected override void StartDeletedResource(ODataDeletedResource resource)
    {
      ODataWriterCore.DeletedResourceScope deletedResourceScope = (ODataWriterCore.DeletedResourceScope) this.CurrentDeletedResourceScope;
      ODataNestedResourceInfo nestedResourceInfo = this.ParentNestedResourceInfo;
      if (nestedResourceInfo != null)
      {
        ODataVersion? nullable = this.Version.HasValue ? this.Version : throw new ODataException(Microsoft.OData.Strings.ODataWriterCore_NestedContentNotAllowedIn40DeletedEntry);
        ODataVersion odataVersion = ODataVersion.V401;
        if (!(nullable.GetValueOrDefault() < odataVersion & nullable.HasValue))
        {
          this.jsonWriter.WriteName(nestedResourceInfo.Name);
          this.jsonWriter.StartObjectScope();
          this.WriteDeletedEntryContents(resource);
        }
      }
      else
      {
        ODataWriterCore.DeltaResourceSetScope parentScope = this.ParentScope as ODataWriterCore.DeltaResourceSetScope;
        this.jsonWriter.StartObjectScope();
        ODataVersion? version = this.Version;
        if (version.HasValue)
        {
          version = this.Version;
          ODataVersion odataVersion = ODataVersion.V401;
          if (!(version.GetValueOrDefault() < odataVersion & version.HasValue))
          {
            string name = parentScope.NavigationSource == null ? (string) null : parentScope.NavigationSource.Name;
            string str = resource.SerializationInfo != null ? resource.SerializationInfo.NavigationSourceName : (deletedResourceScope.NavigationSource == null ? (string) null : deletedResourceScope.NavigationSource.Name);
            if (string.IsNullOrEmpty(str) || str != name)
              this.jsonLightResourceSerializer.WriteDeltaContextUri(this.CurrentDeletedResourceScope.GetOrCreateTypeContext(this.jsonLightOutputContext.WritingResponse), ODataDeltaKind.DeletedEntry, parentScope.ContextUriInfo);
            this.WriteDeletedEntryContents(resource);
            return;
          }
        }
        this.jsonLightResourceSerializer.WriteDeltaContextUri(this.CurrentDeletedResourceScope.GetOrCreateTypeContext(this.jsonLightOutputContext.WritingResponse), ODataDeltaKind.DeletedEntry, parentScope.ContextUriInfo);
        this.WriteV4DeletedEntryContents(resource);
      }
    }

    protected override void StartDeltaLink(ODataDeltaLinkBase link)
    {
      this.jsonWriter.StartObjectScope();
      if (link is ODataDeltaLink)
        this.WriteDeltaLinkContextUri(ODataDeltaKind.Link);
      else
        this.WriteDeltaLinkContextUri(ODataDeltaKind.DeletedLink);
      this.WriteDeltaLinkSource(link);
      this.WriteDeltaLinkRelationship(link);
      this.WriteDeltaLinkTarget(link);
      this.jsonWriter.EndObjectScope();
    }

    protected override void WritePrimitiveValue(ODataPrimitiveValue primitiveValue)
    {
      if (this.ParentScope != null && this.ParentScope.Item is ODataPropertyInfo odataPropertyInfo)
        this.jsonWriter.WriteName(odataPropertyInfo.Name);
      if (primitiveValue == null)
        this.jsonLightValueSerializer.WriteNullValue();
      else
        this.jsonLightValueSerializer.WritePrimitiveValue(primitiveValue.Value, (IEdmTypeReference) null);
    }

    protected override Stream StartBinaryStream()
    {
      if (this.ParentScope != null && this.ParentScope.Item is ODataPropertyInfo odataPropertyInfo)
      {
        this.jsonWriter.WriteName(odataPropertyInfo.Name);
        this.jsonWriter.Flush();
      }
      Stream stream;
      if (this.jsonStreamWriter == null)
      {
        this.jsonLightOutputContext.BinaryValueStream = new MemoryStream();
        stream = (Stream) this.jsonLightOutputContext.BinaryValueStream;
      }
      else
        stream = this.jsonStreamWriter.StartStreamValueScope();
      return stream;
    }

    protected override sealed void EndBinaryStream()
    {
      if (this.jsonStreamWriter == null)
      {
        this.jsonWriter.WriteValue(this.jsonLightOutputContext.BinaryValueStream.ToArray());
        this.jsonLightOutputContext.BinaryValueStream.Flush();
        this.jsonLightOutputContext.BinaryValueStream.Dispose();
        this.jsonLightOutputContext.BinaryValueStream = (MemoryStream) null;
      }
      else
        this.jsonStreamWriter.EndStreamValueScope();
    }

    protected override TextWriter StartTextWriter()
    {
      odataPropertyInfo = (ODataPropertyInfo) null;
      if (this.ParentScope != null && this.ParentScope.Item is ODataPropertyInfo odataPropertyInfo)
      {
        this.jsonWriter.WriteName(odataPropertyInfo.Name);
        this.jsonWriter.Flush();
      }
      TextWriter textWriter;
      if (this.jsonStreamWriter == null)
      {
        this.jsonLightOutputContext.StringWriter = new StringWriter((IFormatProvider) CultureInfo.InvariantCulture);
        textWriter = (TextWriter) this.jsonLightOutputContext.StringWriter;
      }
      else
      {
        string contentType = "text/plain";
        if (odataPropertyInfo is ODataStreamPropertyInfo streamPropertyInfo && streamPropertyInfo.ContentType != null)
          contentType = streamPropertyInfo.ContentType;
        textWriter = this.jsonStreamWriter.StartTextWriterValueScope(contentType);
      }
      return textWriter;
    }

    protected override sealed void EndTextWriter()
    {
      if (this.jsonStreamWriter == null)
      {
        this.jsonLightOutputContext.StringWriter.Flush();
        this.jsonWriter.WriteValue(this.jsonLightOutputContext.StringWriter.GetStringBuilder().ToString());
        this.jsonLightOutputContext.StringWriter.Dispose();
        this.jsonLightOutputContext.StringWriter = (StringWriter) null;
      }
      else
        this.jsonStreamWriter.EndTextWriterValueScope();
    }

    protected override void WriteDeferredNestedResourceInfo(
      ODataNestedResourceInfo nestedResourceInfo)
    {
      this.jsonLightResourceSerializer.WriteNavigationLinkMetadata(nestedResourceInfo, this.DuplicatePropertyNameChecker);
    }

    protected override void StartNestedResourceInfoWithContent(
      ODataNestedResourceInfo nestedResourceInfo)
    {
      if (this.jsonLightOutputContext.WritingResponse)
      {
        if (this.CurrentScope.NavigationSource is IEdmContainedEntitySet navigationSource1 && this.jsonLightOutputContext.MessageWriterSettings.LibraryCompatibility < ODataLibraryCompatibility.Version7)
        {
          ODataVersion? version1 = this.jsonLightOutputContext.MessageWriterSettings.Version;
          ODataVersion odataVersion = ODataVersion.V401;
          if (version1.GetValueOrDefault() < odataVersion & version1.HasValue)
          {
            IEdmNavigationSource navigationSource = this.CurrentScope.NavigationSource;
            string expectedEntityTypeName = this.CurrentScope.ResourceType.FullTypeName();
            int num = navigationSource1.NavigationProperty.Type.TypeKind() != EdmTypeKind.Collection ? 1 : 0;
            ODataUri odataUri = this.CurrentScope.ODataUri;
            version1 = this.jsonLightOutputContext.MessageWriterSettings.Version;
            int version2 = (int) version1 ?? 0;
            ODataContextUrlInfo contextUrlInfo = ODataContextUrlInfo.Create(navigationSource, expectedEntityTypeName, num != 0, odataUri, (ODataVersion) version2);
            this.jsonLightResourceSerializer.WriteNestedResourceInfoContextUrl(nestedResourceInfo, contextUrlInfo);
          }
        }
        this.jsonLightResourceSerializer.WriteNavigationLinkMetadata(nestedResourceInfo, this.DuplicatePropertyNameChecker);
      }
      else
        this.WriterValidator.ValidateNestedResourceInfoHasCardinality(nestedResourceInfo);
    }

    protected override void EndNestedResourceInfoWithContent(
      ODataNestedResourceInfo nestedResourceInfo)
    {
      ODataJsonLightWriter.JsonLightNestedResourceInfoScope currentScope = (ODataJsonLightWriter.JsonLightNestedResourceInfoScope) this.CurrentScope;
      if (this.jsonLightOutputContext.WritingResponse)
        return;
      if (currentScope.EntityReferenceLinkWritten && !currentScope.ResourceSetWritten && nestedResourceInfo.IsCollection.Value)
        this.jsonWriter.EndArrayScope();
      if (!currentScope.ResourceSetWritten)
        return;
      this.jsonWriter.EndArrayScope();
    }

    protected override void WriteEntityReferenceInNavigationLinkContent(
      ODataNestedResourceInfo parentNestedResourceInfo,
      ODataEntityReferenceLink entityReferenceLink)
    {
      if (!(this.CurrentScope is ODataJsonLightWriter.JsonLightNestedResourceInfoScope resourceInfoScope))
        resourceInfoScope = this.ParentNestedResourceInfoScope as ODataJsonLightWriter.JsonLightNestedResourceInfoScope;
      if (resourceInfoScope.ResourceSetWritten)
        throw new ODataException(Microsoft.OData.Strings.ODataJsonLightWriter_EntityReferenceLinkAfterResourceSetInRequest);
      ODataVersion? version;
      if (!resourceInfoScope.EntityReferenceLinkWritten)
      {
        if (!this.jsonLightOutputContext.WritingResponse)
        {
          version = this.Version;
          if (version.HasValue)
          {
            version = this.Version;
            ODataVersion odataVersion = ODataVersion.V401;
            if (!(version.GetValueOrDefault() < odataVersion & version.HasValue))
            {
              this.jsonWriter.WriteName(parentNestedResourceInfo.Name);
              goto label_10;
            }
          }
          this.odataAnnotationWriter.WritePropertyAnnotationName(parentNestedResourceInfo.Name, "odata.bind");
label_10:
          if (parentNestedResourceInfo.IsCollection.Value)
            this.jsonWriter.StartArrayScope();
        }
        else if (!parentNestedResourceInfo.IsCollection.Value)
          this.jsonWriter.WriteName(parentNestedResourceInfo.Name);
        resourceInfoScope.EntityReferenceLinkWritten = true;
      }
      if (!this.jsonLightOutputContext.WritingResponse)
      {
        version = this.Version;
        if (version.HasValue)
        {
          version = this.Version;
          ODataVersion odataVersion = ODataVersion.V401;
          if (!(version.GetValueOrDefault() < odataVersion & version.HasValue))
            goto label_19;
        }
        this.jsonWriter.WriteValue(this.jsonLightResourceSerializer.UriToString(entityReferenceLink.Url));
        return;
      }
label_19:
      this.WriteEntityReferenceLinkImplementation(entityReferenceLink);
    }

    protected override ODataWriterCore.ResourceSetScope CreateResourceSetScope(
      ODataResourceSet resourceSet,
      IEdmNavigationSource navigationSource,
      IEdmType itemType,
      bool skipWriting,
      SelectedPropertiesNode selectedProperties,
      ODataUri odataUri,
      bool isUndeclared)
    {
      return (ODataWriterCore.ResourceSetScope) new ODataJsonLightWriter.JsonLightResourceSetScope(resourceSet, navigationSource, itemType, skipWriting, selectedProperties, odataUri, isUndeclared);
    }

    protected override ODataWriterCore.DeltaResourceSetScope CreateDeltaResourceSetScope(
      ODataDeltaResourceSet deltaResourceSet,
      IEdmNavigationSource navigationSource,
      IEdmStructuredType resourceType,
      bool skipWriting,
      SelectedPropertiesNode selectedProperties,
      ODataUri odataUri,
      bool isUndeclared)
    {
      return (ODataWriterCore.DeltaResourceSetScope) new ODataJsonLightWriter.JsonLightDeltaResourceSetScope(deltaResourceSet, navigationSource, resourceType, selectedProperties, odataUri);
    }

    protected override ODataWriterCore.DeletedResourceScope CreateDeletedResourceScope(
      ODataDeletedResource deltaResource,
      IEdmNavigationSource navigationSource,
      IEdmEntityType resourceType,
      bool skipWriting,
      SelectedPropertiesNode selectedProperties,
      ODataUri odataUri,
      bool isUndeclared)
    {
      return (ODataWriterCore.DeletedResourceScope) new ODataJsonLightWriter.JsonLightDeletedResourceScope(deltaResource, this.GetResourceSerializationInfo((ODataResourceBase) deltaResource), navigationSource, resourceType, skipWriting, this.jsonLightOutputContext.MessageWriterSettings, selectedProperties, odataUri, isUndeclared);
    }

    protected override ODataWriterCore.PropertyInfoScope CreatePropertyInfoScope(
      ODataPropertyInfo property,
      IEdmNavigationSource navigationSource,
      IEdmStructuredType resourceType,
      SelectedPropertiesNode selectedProperties,
      ODataUri odataUri)
    {
      return (ODataWriterCore.PropertyInfoScope) new ODataJsonLightWriter.JsonLightPropertyScope(property, navigationSource, resourceType, selectedProperties, odataUri);
    }

    protected override ODataWriterCore.DeltaLinkScope CreateDeltaLinkScope(
      ODataDeltaLinkBase link,
      IEdmNavigationSource navigationSource,
      IEdmEntityType entityType,
      SelectedPropertiesNode selectedProperties,
      ODataUri odataUri)
    {
      return (ODataWriterCore.DeltaLinkScope) new ODataJsonLightWriter.JsonLightDeltaLinkScope(link is ODataDeltaLink ? ODataWriterCore.WriterState.DeltaLink : ODataWriterCore.WriterState.DeltaDeletedLink, (ODataItem) link, this.GetLinkSerializationInfo((ODataItem) link), navigationSource, entityType, selectedProperties, odataUri);
    }

    protected override ODataWriterCore.ResourceScope CreateResourceScope(
      ODataResource resource,
      IEdmNavigationSource navigationSource,
      IEdmStructuredType resourceType,
      bool skipWriting,
      SelectedPropertiesNode selectedProperties,
      ODataUri odataUri,
      bool isUndeclared)
    {
      return (ODataWriterCore.ResourceScope) new ODataJsonLightWriter.JsonLightResourceScope(resource, this.GetResourceSerializationInfo((ODataResourceBase) resource), navigationSource, resourceType, skipWriting, this.jsonLightOutputContext.MessageWriterSettings, selectedProperties, odataUri, isUndeclared);
    }

    protected override ODataWriterCore.NestedResourceInfoScope CreateNestedResourceInfoScope(
      ODataWriterCore.WriterState writerState,
      ODataNestedResourceInfo navLink,
      IEdmNavigationSource navigationSource,
      IEdmType itemType,
      bool skipWriting,
      SelectedPropertiesNode selectedProperties,
      ODataUri odataUri)
    {
      return (ODataWriterCore.NestedResourceInfoScope) new ODataJsonLightWriter.JsonLightNestedResourceInfoScope(writerState, navLink, navigationSource, itemType, skipWriting, selectedProperties, odataUri);
    }

    private void WriteEntityReferenceLinkImplementation(ODataEntityReferenceLink entityReferenceLink)
    {
      WriterValidationUtils.ValidateEntityReferenceLink(entityReferenceLink);
      this.jsonWriter.StartObjectScope();
      this.odataAnnotationWriter.WriteInstanceAnnotationName("odata.id");
      Uri uri = this.jsonLightOutputContext.MessageWriterSettings.MetadataDocumentUri.MakeRelativeUri(entityReferenceLink.Url);
      this.jsonWriter.WriteValue(uri == (Uri) null ? (string) null : this.jsonLightResourceSerializer.UriToString(uri));
      this.jsonLightResourceSerializer.InstanceAnnotationWriter.WriteInstanceAnnotations((IEnumerable<ODataInstanceAnnotation>) entityReferenceLink.InstanceAnnotations);
      this.jsonWriter.EndObjectScope();
    }

    private void InnerPrepareResourceForWriteStart(
      ODataResourceBase resource,
      ODataResourceTypeContext typeContext,
      SelectedPropertiesNode selectedProperties)
    {
      ODataResourceSerializationInfo serializationInfo;
      IEdmStructuredType resourceType;
      ODataUri odataUri;
      if (resource is ODataResource)
      {
        ODataWriterCore.ResourceScope currentScope = (ODataWriterCore.ResourceScope) this.CurrentScope;
        serializationInfo = currentScope.SerializationInfo;
        resourceType = currentScope.ResourceType;
        odataUri = currentScope.ODataUri;
      }
      else
      {
        ODataWriterCore.DeletedResourceScope currentScope = (ODataWriterCore.DeletedResourceScope) this.CurrentScope;
        serializationInfo = currentScope.SerializationInfo;
        resourceType = currentScope.ResourceType;
        odataUri = currentScope.ODataUri;
      }
      ODataResourceMetadataBuilder resourceMetadataBuilder1 = this.jsonLightOutputContext.MetadataLevel.CreateResourceMetadataBuilder(resource, (IODataResourceTypeContext) typeContext, serializationInfo, resourceType, selectedProperties, this.jsonLightOutputContext.WritingResponse, this.jsonLightOutputContext.ODataSimplifiedOptions.EnableWritingKeyAsSegment, odataUri, this.jsonLightOutputContext.MessageWriterSettings);
      if (resourceMetadataBuilder1 == null)
        return;
      resourceMetadataBuilder1.NameAsProperty = this.BelongingNestedResourceInfo != null ? this.BelongingNestedResourceInfo.Name : (string) null;
      ODataResourceMetadataBuilder resourceMetadataBuilder2 = resourceMetadataBuilder1;
      int num;
      if (this.BelongingNestedResourceInfo != null)
      {
        bool? isCollection = this.BelongingNestedResourceInfo.IsCollection;
        bool flag = true;
        num = isCollection.GetValueOrDefault() == flag & isCollection.HasValue ? 1 : 0;
      }
      else
        num = 0;
      resourceMetadataBuilder2.IsFromCollection = num != 0;
      if (resourceMetadataBuilder1 is ODataConventionalResourceMetadataBuilder)
        resourceMetadataBuilder1.ParentMetadataBuilder = this.FindParentResourceMetadataBuilder();
      this.jsonLightOutputContext.MetadataLevel.InjectMetadataBuilder(resource, resourceMetadataBuilder1);
    }

    private ODataResourceMetadataBuilder FindParentResourceMetadataBuilder()
    {
      ODataWriterCore.ResourceScope parentResourceScope = this.GetParentResourceScope();
      return parentResourceScope != null && parentResourceScope.Item is ODataResourceBase odataResourceBase ? odataResourceBase.MetadataBuilder : (ODataResourceMetadataBuilder) null;
    }

    private void WriteResourceSetCount(long? count, string propertyName)
    {
      if (!count.HasValue)
        return;
      if (propertyName == null)
        this.odataAnnotationWriter.WriteInstanceAnnotationName("odata.count");
      else
        this.odataAnnotationWriter.WritePropertyAnnotationName(propertyName, "odata.count");
      this.jsonWriter.WriteValue(count.Value);
    }

    private void WriteResourceSetNextLink(Uri nextPageLink, string propertyName)
    {
      bool flag = this.State == ODataWriterCore.WriterState.ResourceSet ? this.CurrentResourceSetScope.NextPageLinkWritten : this.CurrentDeltaResourceSetScope.NextPageLinkWritten;
      if (!(nextPageLink != (Uri) null) || flag)
        return;
      if (propertyName == null)
        this.odataAnnotationWriter.WriteInstanceAnnotationName("odata.nextLink");
      else
        this.odataAnnotationWriter.WritePropertyAnnotationName(propertyName, "odata.nextLink");
      this.jsonWriter.WriteValue(this.jsonLightResourceSerializer.UriToString(nextPageLink));
      if (this.State == ODataWriterCore.WriterState.ResourceSet)
        this.CurrentResourceSetScope.NextPageLinkWritten = true;
      else
        this.CurrentDeltaResourceSetScope.NextPageLinkWritten = true;
    }

    private void WriteResourceSetDeltaLink(Uri deltaLink)
    {
      if (deltaLink == (Uri) null || (this.State == ODataWriterCore.WriterState.ResourceSet ? this.CurrentResourceSetScope.DeltaLinkWritten : this.CurrentDeltaResourceSetScope.DeltaLinkWritten))
        return;
      this.odataAnnotationWriter.WriteInstanceAnnotationName("odata.deltaLink");
      this.jsonWriter.WriteValue(this.jsonLightResourceSerializer.UriToString(deltaLink));
      if (this.State == ODataWriterCore.WriterState.ResourceSet)
        this.CurrentResourceSetScope.DeltaLinkWritten = true;
      else
        this.CurrentDeltaResourceSetScope.DeltaLinkWritten = true;
    }

    private void WriteV4DeletedEntryContents(ODataDeletedResource resource)
    {
      this.WriteDeletedResourceId(resource);
      this.WriteDeltaResourceReason(resource);
    }

    private void WriteDeletedEntryContents(ODataDeletedResource resource)
    {
      this.odataAnnotationWriter.WriteInstanceAnnotationName("odata.removed");
      this.jsonWriter.StartObjectScope();
      this.WriteDeltaResourceReason(resource);
      this.jsonWriter.EndObjectScope();
      ODataJsonLightWriter.JsonLightDeletedResourceScope deletedResourceScope = this.CurrentDeletedResourceScope;
      this.jsonLightResourceSerializer.WriteResourceStartMetadataProperties((IODataJsonLightWriterResourceState) deletedResourceScope);
      this.jsonLightResourceSerializer.WriteResourceMetadataProperties((IODataJsonLightWriterResourceState) deletedResourceScope);
      this.jsonLightOutputContext.PropertyCacheHandler.SetCurrentResourceScopeLevel(this.ScopeLevel);
      this.jsonLightResourceSerializer.InstanceAnnotationWriter.WriteInstanceAnnotations((IEnumerable<ODataInstanceAnnotation>) resource.InstanceAnnotations, deletedResourceScope.InstanceAnnotationWriteTracker);
      this.WriteDeltaResourceProperties((ODataResourceBase) resource);
    }

    private void WriteDeletedResourceId(ODataDeletedResource resource)
    {
      if (this.Version.HasValue)
      {
        ODataVersion? version = this.Version;
        ODataVersion odataVersion = ODataVersion.V401;
        if (!(version.GetValueOrDefault() < odataVersion & version.HasValue))
        {
          Uri id;
          if (!resource.MetadataBuilder.TryGetIdForSerialization(out id))
            return;
          this.jsonWriter.WriteInstanceAnnotationName("id");
          this.jsonWriter.WriteValue(id.OriginalString);
          return;
        }
      }
      this.jsonWriter.WriteName("id");
      this.jsonWriter.WriteValue(resource.Id.OriginalString);
    }

    private void WriteDeltaResourceProperties(ODataResourceBase resource)
    {
      if (resource.NonComputedProperties == null)
        return;
      this.jsonLightResourceSerializer.WriteProperties(this.ResourceType, resource.NonComputedProperties, false, false, this.DuplicatePropertyNameChecker, resource.MetadataBuilder);
    }

    private void WriteDeltaResourceReason(ODataDeletedResource resource)
    {
      if (!resource.Reason.HasValue)
        return;
      this.jsonWriter.WriteName("reason");
      switch (resource.Reason.Value)
      {
        case DeltaDeletedEntryReason.Deleted:
          this.jsonWriter.WriteValue("deleted");
          break;
        case DeltaDeletedEntryReason.Changed:
          this.jsonWriter.WriteValue("changed");
          break;
      }
    }

    private void WriteDeltaLinkContextUri(ODataDeltaKind kind) => this.jsonLightResourceSerializer.WriteDeltaContextUri(this.CurrentDeltaLinkScope.GetOrCreateTypeContext(), kind);

    private void WriteDeltaLinkSource(ODataDeltaLinkBase link)
    {
      this.jsonWriter.WriteName("source");
      this.jsonWriter.WriteValue(UriUtils.UriToString(link.Source));
    }

    private void WriteDeltaLinkRelationship(ODataDeltaLinkBase link)
    {
      this.jsonWriter.WriteName("relationship");
      this.jsonWriter.WriteValue(link.Relationship);
    }

    private void WriteDeltaLinkTarget(ODataDeltaLinkBase link)
    {
      this.jsonWriter.WriteName("target");
      this.jsonWriter.WriteValue(UriUtils.UriToString(link.Target));
    }

    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "An instance field is used in a debug assert.")]
    private void ValidateNoCustomInstanceAnnotationsForExpandedResourceSet(
      ODataResourceSet resourceSet)
    {
      if (resourceSet.InstanceAnnotations.Count > 0)
        throw new ODataException(Microsoft.OData.Strings.ODataJsonLightWriter_InstanceAnnotationNotSupportedOnExpandedResourceSet);
    }

    private sealed class JsonLightResourceSetScope : ODataWriterCore.ResourceSetScope
    {
      private bool nextLinkWritten;
      private bool deltaLinkWritten;
      private bool isUndeclared;

      internal JsonLightResourceSetScope(
        ODataResourceSet resourceSet,
        IEdmNavigationSource navigationSource,
        IEdmType itemType,
        bool skipWriting,
        SelectedPropertiesNode selectedProperties,
        ODataUri odataUri,
        bool isUndeclared)
        : base(resourceSet, navigationSource, itemType, skipWriting, selectedProperties, odataUri)
      {
        this.isUndeclared = isUndeclared;
      }

      internal bool NextPageLinkWritten
      {
        get => this.nextLinkWritten;
        set => this.nextLinkWritten = value;
      }

      internal bool DeltaLinkWritten
      {
        get => this.deltaLinkWritten;
        set => this.deltaLinkWritten = value;
      }

      internal bool IsUndeclared => this.isUndeclared;
    }

    private sealed class JsonLightDeletedResourceScope : 
      ODataWriterCore.DeletedResourceScope,
      IODataJsonLightWriterResourceState
    {
      private int alreadyWrittenMetadataProperties;
      private bool isUndeclared;

      internal JsonLightDeletedResourceScope(
        ODataDeletedResource resource,
        ODataResourceSerializationInfo serializationInfo,
        IEdmNavigationSource navigationSource,
        IEdmEntityType resourceType,
        bool skipWriting,
        ODataMessageWriterSettings writerSettings,
        SelectedPropertiesNode selectedProperties,
        ODataUri odataUri,
        bool isUndeclared)
        : base(resource, serializationInfo, navigationSource, resourceType, writerSettings, selectedProperties, odataUri)
      {
        this.isUndeclared = isUndeclared;
      }

      ODataResourceBase IODataJsonLightWriterResourceState.Resource => (ODataResourceBase) this.Item;

      public bool IsUndeclared => this.isUndeclared;

      public bool EditLinkWritten
      {
        get => this.IsMetadataPropertyWritten(ODataJsonLightWriter.JsonLightDeletedResourceScope.JsonLightEntryMetadataProperty.EditLink);
        set => this.SetWrittenMetadataProperty(ODataJsonLightWriter.JsonLightDeletedResourceScope.JsonLightEntryMetadataProperty.EditLink);
      }

      public bool ReadLinkWritten
      {
        get => this.IsMetadataPropertyWritten(ODataJsonLightWriter.JsonLightDeletedResourceScope.JsonLightEntryMetadataProperty.ReadLink);
        set => this.SetWrittenMetadataProperty(ODataJsonLightWriter.JsonLightDeletedResourceScope.JsonLightEntryMetadataProperty.ReadLink);
      }

      public bool MediaEditLinkWritten
      {
        get => this.IsMetadataPropertyWritten(ODataJsonLightWriter.JsonLightDeletedResourceScope.JsonLightEntryMetadataProperty.MediaEditLink);
        set => this.SetWrittenMetadataProperty(ODataJsonLightWriter.JsonLightDeletedResourceScope.JsonLightEntryMetadataProperty.MediaEditLink);
      }

      public bool MediaReadLinkWritten
      {
        get => this.IsMetadataPropertyWritten(ODataJsonLightWriter.JsonLightDeletedResourceScope.JsonLightEntryMetadataProperty.MediaReadLink);
        set => this.SetWrittenMetadataProperty(ODataJsonLightWriter.JsonLightDeletedResourceScope.JsonLightEntryMetadataProperty.MediaReadLink);
      }

      public bool MediaContentTypeWritten
      {
        get => this.IsMetadataPropertyWritten(ODataJsonLightWriter.JsonLightDeletedResourceScope.JsonLightEntryMetadataProperty.MediaContentType);
        set => this.SetWrittenMetadataProperty(ODataJsonLightWriter.JsonLightDeletedResourceScope.JsonLightEntryMetadataProperty.MediaContentType);
      }

      public bool MediaETagWritten
      {
        get => this.IsMetadataPropertyWritten(ODataJsonLightWriter.JsonLightDeletedResourceScope.JsonLightEntryMetadataProperty.MediaETag);
        set => this.SetWrittenMetadataProperty(ODataJsonLightWriter.JsonLightDeletedResourceScope.JsonLightEntryMetadataProperty.MediaETag);
      }

      private void SetWrittenMetadataProperty(
        ODataJsonLightWriter.JsonLightDeletedResourceScope.JsonLightEntryMetadataProperty jsonLightMetadataProperty)
      {
        this.alreadyWrittenMetadataProperties = (int) ((ODataJsonLightWriter.JsonLightDeletedResourceScope.JsonLightEntryMetadataProperty) this.alreadyWrittenMetadataProperties | jsonLightMetadataProperty);
      }

      private bool IsMetadataPropertyWritten(
        ODataJsonLightWriter.JsonLightDeletedResourceScope.JsonLightEntryMetadataProperty jsonLightMetadataProperty)
      {
        return ((ODataJsonLightWriter.JsonLightDeletedResourceScope.JsonLightEntryMetadataProperty) this.alreadyWrittenMetadataProperties & jsonLightMetadataProperty) == jsonLightMetadataProperty;
      }

      [Flags]
      private enum JsonLightEntryMetadataProperty
      {
        EditLink = 1,
        ReadLink = 2,
        MediaEditLink = 4,
        MediaReadLink = 8,
        MediaContentType = 16, // 0x00000010
        MediaETag = 32, // 0x00000020
      }
    }

    private sealed class JsonLightPropertyScope : ODataWriterCore.PropertyInfoScope
    {
      internal JsonLightPropertyScope(
        ODataPropertyInfo property,
        IEdmNavigationSource navigationSource,
        IEdmStructuredType resourceType,
        SelectedPropertiesNode selectedProperties,
        ODataUri odataUri)
        : base(property, navigationSource, resourceType, selectedProperties, odataUri)
      {
      }
    }

    private sealed class JsonLightDeltaLinkScope : ODataWriterCore.DeltaLinkScope
    {
      public JsonLightDeltaLinkScope(
        ODataWriterCore.WriterState state,
        ODataItem link,
        ODataResourceSerializationInfo serializationInfo,
        IEdmNavigationSource navigationSource,
        IEdmEntityType entityType,
        SelectedPropertiesNode selectedProperties,
        ODataUri odataUri)
        : base(state, link, serializationInfo, navigationSource, entityType, selectedProperties, odataUri)
      {
      }
    }

    private sealed class JsonLightDeltaResourceSetScope : ODataWriterCore.DeltaResourceSetScope
    {
      private bool nextLinkWritten;
      private bool deltaLinkWritten;

      public JsonLightDeltaResourceSetScope(
        ODataDeltaResourceSet resourceSet,
        IEdmNavigationSource navigationSource,
        IEdmStructuredType resourceType,
        SelectedPropertiesNode selectedProperties,
        ODataUri odataUri)
        : base(resourceSet, navigationSource, resourceType, selectedProperties, odataUri)
      {
      }

      internal bool NextPageLinkWritten
      {
        get => this.nextLinkWritten;
        set => this.nextLinkWritten = value;
      }

      internal bool DeltaLinkWritten
      {
        get => this.deltaLinkWritten;
        set => this.deltaLinkWritten = value;
      }
    }

    private sealed class JsonLightResourceScope : 
      ODataWriterCore.ResourceScope,
      IODataJsonLightWriterResourceState
    {
      private int alreadyWrittenMetadataProperties;
      private bool isUndeclared;

      internal JsonLightResourceScope(
        ODataResource resource,
        ODataResourceSerializationInfo serializationInfo,
        IEdmNavigationSource navigationSource,
        IEdmStructuredType resourceType,
        bool skipWriting,
        ODataMessageWriterSettings writerSettings,
        SelectedPropertiesNode selectedProperties,
        ODataUri odataUri,
        bool isUndeclared)
        : base(resource, serializationInfo, navigationSource, resourceType, skipWriting, writerSettings, selectedProperties, odataUri)
      {
        this.isUndeclared = isUndeclared;
      }

      ODataResourceBase IODataJsonLightWriterResourceState.Resource => (ODataResourceBase) this.Item;

      public bool IsUndeclared
      {
        get => this.isUndeclared;
        set => this.isUndeclared = value;
      }

      public bool EditLinkWritten
      {
        get => this.IsMetadataPropertyWritten(ODataJsonLightWriter.JsonLightResourceScope.JsonLightEntryMetadataProperty.EditLink);
        set => this.SetWrittenMetadataProperty(ODataJsonLightWriter.JsonLightResourceScope.JsonLightEntryMetadataProperty.EditLink);
      }

      public bool ReadLinkWritten
      {
        get => this.IsMetadataPropertyWritten(ODataJsonLightWriter.JsonLightResourceScope.JsonLightEntryMetadataProperty.ReadLink);
        set => this.SetWrittenMetadataProperty(ODataJsonLightWriter.JsonLightResourceScope.JsonLightEntryMetadataProperty.ReadLink);
      }

      public bool MediaEditLinkWritten
      {
        get => this.IsMetadataPropertyWritten(ODataJsonLightWriter.JsonLightResourceScope.JsonLightEntryMetadataProperty.MediaEditLink);
        set => this.SetWrittenMetadataProperty(ODataJsonLightWriter.JsonLightResourceScope.JsonLightEntryMetadataProperty.MediaEditLink);
      }

      public bool MediaReadLinkWritten
      {
        get => this.IsMetadataPropertyWritten(ODataJsonLightWriter.JsonLightResourceScope.JsonLightEntryMetadataProperty.MediaReadLink);
        set => this.SetWrittenMetadataProperty(ODataJsonLightWriter.JsonLightResourceScope.JsonLightEntryMetadataProperty.MediaReadLink);
      }

      public bool MediaContentTypeWritten
      {
        get => this.IsMetadataPropertyWritten(ODataJsonLightWriter.JsonLightResourceScope.JsonLightEntryMetadataProperty.MediaContentType);
        set => this.SetWrittenMetadataProperty(ODataJsonLightWriter.JsonLightResourceScope.JsonLightEntryMetadataProperty.MediaContentType);
      }

      public bool MediaETagWritten
      {
        get => this.IsMetadataPropertyWritten(ODataJsonLightWriter.JsonLightResourceScope.JsonLightEntryMetadataProperty.MediaETag);
        set => this.SetWrittenMetadataProperty(ODataJsonLightWriter.JsonLightResourceScope.JsonLightEntryMetadataProperty.MediaETag);
      }

      private void SetWrittenMetadataProperty(
        ODataJsonLightWriter.JsonLightResourceScope.JsonLightEntryMetadataProperty jsonLightMetadataProperty)
      {
        this.alreadyWrittenMetadataProperties = (int) ((ODataJsonLightWriter.JsonLightResourceScope.JsonLightEntryMetadataProperty) this.alreadyWrittenMetadataProperties | jsonLightMetadataProperty);
      }

      private bool IsMetadataPropertyWritten(
        ODataJsonLightWriter.JsonLightResourceScope.JsonLightEntryMetadataProperty jsonLightMetadataProperty)
      {
        return ((ODataJsonLightWriter.JsonLightResourceScope.JsonLightEntryMetadataProperty) this.alreadyWrittenMetadataProperties & jsonLightMetadataProperty) == jsonLightMetadataProperty;
      }

      [Flags]
      private enum JsonLightEntryMetadataProperty
      {
        EditLink = 1,
        ReadLink = 2,
        MediaEditLink = 4,
        MediaReadLink = 8,
        MediaContentType = 16, // 0x00000010
        MediaETag = 32, // 0x00000020
      }
    }

    private sealed class JsonLightNestedResourceInfoScope : ODataWriterCore.NestedResourceInfoScope
    {
      private bool entityReferenceLinkWritten;
      private bool resourceSetWritten;

      internal JsonLightNestedResourceInfoScope(
        ODataWriterCore.WriterState writerState,
        ODataNestedResourceInfo navLink,
        IEdmNavigationSource navigationSource,
        IEdmType itemType,
        bool skipWriting,
        SelectedPropertiesNode selectedProperties,
        ODataUri odataUri)
        : base(writerState, navLink, navigationSource, itemType, skipWriting, selectedProperties, odataUri)
      {
      }

      internal bool EntityReferenceLinkWritten
      {
        get => this.entityReferenceLinkWritten;
        set => this.entityReferenceLinkWritten = value;
      }

      internal bool ResourceSetWritten
      {
        get => this.resourceSetWritten;
        set => this.resourceSetWritten = value;
      }

      internal override ODataWriterCore.NestedResourceInfoScope Clone(
        ODataWriterCore.WriterState newWriterState)
      {
        ODataJsonLightWriter.JsonLightNestedResourceInfoScope resourceInfoScope = new ODataJsonLightWriter.JsonLightNestedResourceInfoScope(newWriterState, (ODataNestedResourceInfo) this.Item, this.NavigationSource, this.ItemType, this.SkipWriting, this.SelectedProperties, this.ODataUri);
        resourceInfoScope.EntityReferenceLinkWritten = this.entityReferenceLinkWritten;
        resourceInfoScope.ResourceSetWritten = this.resourceSetWritten;
        resourceInfoScope.DerivedTypeConstraints = this.DerivedTypeConstraints;
        return (ODataWriterCore.NestedResourceInfoScope) resourceInfoScope;
      }
    }
  }
}
