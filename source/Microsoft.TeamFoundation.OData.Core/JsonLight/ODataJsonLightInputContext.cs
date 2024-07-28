// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.JsonLight.ODataJsonLightInputContext
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Edm;
using Microsoft.OData.Json;
using Microsoft.OData.Metadata;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.OData.JsonLight
{
  internal sealed class ODataJsonLightInputContext : ODataInputContext
  {
    private readonly JsonLightMetadataLevel metadataLevel;
    private TextReader textReader;
    private BufferingJsonReader jsonReader;
    private Stream stream;

    public ODataJsonLightInputContext(
      ODataMessageInfo messageInfo,
      ODataMessageReaderSettings messageReaderSettings)
      : this(ODataJsonLightInputContext.CreateTextReader(messageInfo.MessageStream, messageInfo.Encoding), messageInfo, messageReaderSettings)
    {
      this.stream = messageInfo.MessageStream;
    }

    internal ODataJsonLightInputContext(
      TextReader textReader,
      ODataMessageInfo messageInfo,
      ODataMessageReaderSettings messageReaderSettings)
      : base(ODataFormat.Json, messageInfo, messageReaderSettings)
    {
      try
      {
        this.textReader = textReader;
        IJsonReader jsonReader1 = ODataJsonLightInputContext.CreateJsonReader(this.Container, this.textReader, messageInfo.MediaType.HasIeee754CompatibleSetToTrue());
        if (messageReaderSettings.ArrayPool != null && jsonReader1 is Microsoft.OData.Json.JsonReader jsonReader2 && jsonReader2.ArrayPool == null)
          jsonReader2.ArrayPool = messageReaderSettings.ArrayPool;
        this.jsonReader = !messageInfo.MediaType.HasStreamingSetToTrue() ? (BufferingJsonReader) new ReorderingJsonReader(jsonReader1, messageReaderSettings.MessageQuotas.MaxNestingDepth) : new BufferingJsonReader(jsonReader1, "error", messageReaderSettings.MessageQuotas.MaxNestingDepth);
      }
      catch (Exception ex)
      {
        if (ExceptionUtils.IsCatchableExceptionType(ex) && this.textReader != null)
          this.textReader.Dispose();
        throw;
      }
      this.metadataLevel = JsonLightMetadataLevel.Create(messageInfo.MediaType, (Uri) null, this.Model, this.ReadingResponse);
    }

    public JsonLightMetadataLevel MetadataLevel => this.metadataLevel;

    public BufferingJsonReader JsonReader => this.jsonReader;

    internal Stream Stream => this.stream;

    internal bool OptionalODataPrefix
    {
      get
      {
        ODataVersion? version = this.MessageReaderSettings.Version;
        ODataVersion odataVersion = ODataVersion.V4;
        return !(version.GetValueOrDefault() == odataVersion & version.HasValue) || this.ODataSimplifiedOptions.EnableReadingODataAnnotationWithoutPrefix;
      }
    }

    public override ODataReader CreateResourceSetReader(
      IEdmEntitySetBase entitySet,
      IEdmStructuredType expectedResourceType)
    {
      this.VerifyCanCreateODataReader((IEdmNavigationSource) entitySet, expectedResourceType);
      return this.CreateResourceSetReaderImplementation(entitySet, expectedResourceType, false, false);
    }

    public override Task<ODataReader> CreateResourceSetReaderAsync(
      IEdmEntitySetBase entitySet,
      IEdmStructuredType expectedResourceType)
    {
      this.VerifyCanCreateODataReader((IEdmNavigationSource) entitySet, expectedResourceType);
      return TaskUtils.GetTaskForSynchronousOperation<ODataReader>((Func<ODataReader>) (() => this.CreateResourceSetReaderImplementation(entitySet, expectedResourceType, false, false)));
    }

    public override ODataReader CreateDeltaResourceSetReader(
      IEdmEntitySetBase entitySet,
      IEdmStructuredType expectedResourceType)
    {
      this.VerifyCanCreateODataReader((IEdmNavigationSource) entitySet, expectedResourceType);
      return this.CreateResourceSetReaderImplementation(entitySet, expectedResourceType, false, true);
    }

    public override Task<ODataReader> CreateDeltaResourceSetReaderAsync(
      IEdmEntitySetBase entitySet,
      IEdmStructuredType expectedResourceType)
    {
      this.VerifyCanCreateODataReader((IEdmNavigationSource) entitySet, expectedResourceType);
      return TaskUtils.GetTaskForSynchronousOperation<ODataReader>((Func<ODataReader>) (() => this.CreateResourceSetReaderImplementation(entitySet, expectedResourceType, false, true)));
    }

    public override ODataReader CreateResourceReader(
      IEdmNavigationSource navigationSource,
      IEdmStructuredType expectedResourceType)
    {
      this.VerifyCanCreateODataReader(navigationSource, expectedResourceType);
      return this.CreateResourceReaderImplementation(navigationSource, expectedResourceType);
    }

    public override Task<ODataReader> CreateResourceReaderAsync(
      IEdmNavigationSource navigationSource,
      IEdmStructuredType expectedResourceType)
    {
      this.VerifyCanCreateODataReader(navigationSource, expectedResourceType);
      return TaskUtils.GetTaskForSynchronousOperation<ODataReader>((Func<ODataReader>) (() => this.CreateResourceReaderImplementation(navigationSource, expectedResourceType)));
    }

    public override ODataCollectionReader CreateCollectionReader(
      IEdmTypeReference expectedItemTypeReference)
    {
      this.VerifyCanCreateCollectionReader(expectedItemTypeReference);
      return this.CreateCollectionReaderImplementation(expectedItemTypeReference);
    }

    public override Task<ODataCollectionReader> CreateCollectionReaderAsync(
      IEdmTypeReference expectedItemTypeReference)
    {
      this.VerifyCanCreateCollectionReader(expectedItemTypeReference);
      return TaskUtils.GetTaskForSynchronousOperation<ODataCollectionReader>((Func<ODataCollectionReader>) (() => this.CreateCollectionReaderImplementation(expectedItemTypeReference)));
    }

    public override ODataProperty ReadProperty(
      IEdmStructuralProperty property,
      IEdmTypeReference expectedPropertyTypeReference)
    {
      this.VerifyCanReadProperty();
      return new ODataJsonLightPropertyAndValueDeserializer(this).ReadTopLevelProperty(expectedPropertyTypeReference);
    }

    public override Task<ODataProperty> ReadPropertyAsync(
      IEdmStructuralProperty property,
      IEdmTypeReference expectedPropertyTypeReference)
    {
      this.VerifyCanReadProperty();
      return new ODataJsonLightPropertyAndValueDeserializer(this).ReadTopLevelPropertyAsync(expectedPropertyTypeReference);
    }

    public override ODataError ReadError() => new ODataJsonLightErrorDeserializer(this).ReadTopLevelError();

    public override Task<ODataError> ReadErrorAsync() => new ODataJsonLightErrorDeserializer(this).ReadTopLevelErrorAsync();

    public override ODataReader CreateUriParameterResourceSetReader(
      IEdmEntitySetBase entitySet,
      IEdmStructuredType expectedResourceType)
    {
      this.VerifyCanCreateODataReader((IEdmNavigationSource) entitySet, expectedResourceType);
      return this.CreateResourceSetReaderImplementation(entitySet, expectedResourceType, true, false);
    }

    public override Task<ODataReader> CreateUriParameterResourceSetReaderAsync(
      IEdmEntitySetBase entitySet,
      IEdmStructuredType expectedResourceType)
    {
      this.VerifyCanCreateODataReader((IEdmNavigationSource) entitySet, expectedResourceType);
      return TaskUtils.GetTaskForSynchronousOperation<ODataReader>((Func<ODataReader>) (() => this.CreateResourceSetReaderImplementation(entitySet, expectedResourceType, true, false)));
    }

    public override ODataReader CreateUriParameterResourceReader(
      IEdmNavigationSource navigationSource,
      IEdmStructuredType expectedResourceType)
    {
      return this.CreateResourceReader(navigationSource, expectedResourceType);
    }

    public override Task<ODataReader> CreateUriParameterResourceReaderAsync(
      IEdmNavigationSource navigationSource,
      IEdmStructuredType expectedResourceType)
    {
      return this.CreateResourceReaderAsync(navigationSource, expectedResourceType);
    }

    public override ODataParameterReader CreateParameterReader(IEdmOperation operation)
    {
      this.VerifyCanCreateParameterReader(operation);
      return this.CreateParameterReaderImplementation(operation);
    }

    public override Task<ODataParameterReader> CreateParameterReaderAsync(IEdmOperation operation)
    {
      this.VerifyCanCreateParameterReader(operation);
      return TaskUtils.GetTaskForSynchronousOperation<ODataParameterReader>((Func<ODataParameterReader>) (() => this.CreateParameterReaderImplementation(operation)));
    }

    public IEnumerable<ODataPayloadKind> DetectPayloadKind(
      ODataPayloadKindDetectionInfo detectionInfo)
    {
      this.VerifyCanDetectPayloadKind();
      return new ODataJsonLightPayloadKindDetectionDeserializer(this).DetectPayloadKind(detectionInfo);
    }

    public Task<IEnumerable<ODataPayloadKind>> DetectPayloadKindAsync(
      ODataPayloadKindDetectionInfo detectionInfo)
    {
      this.VerifyCanDetectPayloadKind();
      return new ODataJsonLightPayloadKindDetectionDeserializer(this).DetectPayloadKindAsync(detectionInfo);
    }

    internal override ODataDeltaReader CreateDeltaReader(
      IEdmEntitySetBase entitySet,
      IEdmEntityType expectedBaseEntityType)
    {
      this.VerifyCanCreateODataReader((IEdmNavigationSource) entitySet, (IEdmStructuredType) expectedBaseEntityType);
      return this.CreateDeltaReaderImplementation(entitySet, expectedBaseEntityType);
    }

    internal override Task<ODataDeltaReader> CreateDeltaReaderAsync(
      IEdmEntitySetBase entitySet,
      IEdmEntityType expectedBaseEntityType)
    {
      this.VerifyCanCreateODataReader((IEdmNavigationSource) entitySet, (IEdmStructuredType) expectedBaseEntityType);
      return TaskUtils.GetTaskForSynchronousOperation<ODataDeltaReader>((Func<ODataDeltaReader>) (() => this.CreateDeltaReaderImplementation(entitySet, expectedBaseEntityType)));
    }

    internal override ODataBatchReader CreateBatchReader() => this.CreateBatchReaderImplementation(true);

    internal override Task<ODataBatchReader> CreateBatchReaderAsync() => TaskUtils.GetTaskForSynchronousOperation<ODataBatchReader>((Func<ODataBatchReader>) (() => this.CreateBatchReaderImplementation(false)));

    internal override ODataServiceDocument ReadServiceDocument() => new ODataJsonLightServiceDocumentDeserializer(this).ReadServiceDocument();

    internal override Task<ODataServiceDocument> ReadServiceDocumentAsync() => new ODataJsonLightServiceDocumentDeserializer(this).ReadServiceDocumentAsync();

    internal override ODataEntityReferenceLinks ReadEntityReferenceLinks() => new ODataJsonLightEntityReferenceLinkDeserializer(this).ReadEntityReferenceLinks();

    internal override Task<ODataEntityReferenceLinks> ReadEntityReferenceLinksAsync() => new ODataJsonLightEntityReferenceLinkDeserializer(this).ReadEntityReferenceLinksAsync();

    internal override ODataEntityReferenceLink ReadEntityReferenceLink()
    {
      this.VerifyCanReadEntityReferenceLink();
      return new ODataJsonLightEntityReferenceLinkDeserializer(this).ReadEntityReferenceLink();
    }

    internal override Task<ODataEntityReferenceLink> ReadEntityReferenceLinkAsync()
    {
      this.VerifyCanReadEntityReferenceLink();
      return new ODataJsonLightEntityReferenceLinkDeserializer(this).ReadEntityReferenceLinkAsync();
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing)
      {
        try
        {
          this.stream = (Stream) null;
          if (this.textReader != null)
            this.textReader.Dispose();
        }
        finally
        {
          this.textReader = (TextReader) null;
          this.jsonReader = (BufferingJsonReader) null;
        }
      }
      base.Dispose(disposing);
    }

    private static TextReader CreateTextReader(Stream messageStream, Encoding encoding)
    {
      try
      {
        return (TextReader) new StreamReader(messageStream, encoding);
      }
      catch (Exception ex)
      {
        if (ExceptionUtils.IsCatchableExceptionType(ex) && messageStream != null)
          messageStream.Dispose();
        throw;
      }
    }

    private static IJsonReader CreateJsonReader(
      IServiceProvider container,
      TextReader textReader,
      bool isIeee754Compatible)
    {
      return container == null ? (IJsonReader) new Microsoft.OData.Json.JsonReader(textReader, isIeee754Compatible) : container.GetRequiredService<IJsonReaderFactory>().CreateJsonReader(textReader, isIeee754Compatible);
    }

    private void VerifyCanCreateParameterReader(IEdmOperation operation)
    {
      this.VerifyUserModel();
      if (operation == null)
        throw new ArgumentNullException(nameof (operation), Microsoft.OData.Strings.ODataJsonLightInputContext_OperationCannotBeNullForCreateParameterReader((object) nameof (operation)));
    }

    private void VerifyCanCreateODataReader(
      IEdmNavigationSource navigationSource,
      IEdmStructuredType structuredType)
    {
      if (!this.ReadingResponse)
      {
        this.VerifyUserModel();
        if (navigationSource == null && structuredType != null && structuredType.IsODataEntityTypeKind())
          throw new ODataException(Microsoft.OData.Strings.ODataJsonLightInputContext_NoEntitySetForRequest);
      }
      IEdmEntityType elementType = this.EdmTypeResolver.GetElementType(navigationSource);
      if (navigationSource != null && structuredType != null && !structuredType.IsOrInheritsFrom((IEdmType) elementType))
        throw new ODataException(Microsoft.OData.Strings.ODataJsonLightInputContext_EntityTypeMustBeCompatibleWithEntitySetBaseType((object) structuredType.FullTypeName(), (object) elementType.FullName(), (object) navigationSource.FullNavigationSourceName()));
    }

    private void VerifyCanCreateCollectionReader(IEdmTypeReference expectedItemTypeReference)
    {
      if (this.ReadingResponse)
        return;
      this.VerifyUserModel();
      if (expectedItemTypeReference == null)
        throw new ODataException(Microsoft.OData.Strings.ODataJsonLightInputContext_ItemTypeRequiredForCollectionReaderInRequests);
    }

    private void VerifyCanReadEntityReferenceLink()
    {
      if (this.ReadingResponse)
        return;
      this.VerifyUserModel();
    }

    private void VerifyCanReadProperty()
    {
      if (this.ReadingResponse)
        return;
      this.VerifyUserModel();
    }

    private void VerifyCanDetectPayloadKind()
    {
      if (!this.ReadingResponse)
        throw new ODataException(Microsoft.OData.Strings.ODataJsonLightInputContext_PayloadKindDetectionForRequest);
    }

    private void VerifyUserModel()
    {
      if (!this.Model.IsUserModel())
        throw new ODataException(Microsoft.OData.Strings.ODataJsonLightInputContext_ModelRequiredForReading);
    }

    private ODataReader CreateResourceSetReaderImplementation(
      IEdmEntitySetBase entitySet,
      IEdmStructuredType expectedResourceType,
      bool readingParameter,
      bool readingDelta)
    {
      return (ODataReader) new ODataJsonLightReader(this, (IEdmNavigationSource) entitySet, expectedResourceType, true, readingParameter, readingDelta);
    }

    private ODataDeltaReader CreateDeltaReaderImplementation(
      IEdmEntitySetBase entitySet,
      IEdmEntityType expectedBaseEntityType)
    {
      return (ODataDeltaReader) new ODataJsonLightDeltaReader(this, (IEdmNavigationSource) entitySet, expectedBaseEntityType);
    }

    private ODataReader CreateResourceReaderImplementation(
      IEdmNavigationSource navigationSource,
      IEdmStructuredType expectedBaseResourceType)
    {
      return (ODataReader) new ODataJsonLightReader(this, navigationSource, expectedBaseResourceType, false, readingDelta: !this.ReadingResponse);
    }

    private ODataCollectionReader CreateCollectionReaderImplementation(
      IEdmTypeReference expectedItemTypeReference)
    {
      return (ODataCollectionReader) new ODataJsonLightCollectionReader(this, expectedItemTypeReference, (IODataReaderWriterListener) null);
    }

    private ODataParameterReader CreateParameterReaderImplementation(IEdmOperation operation) => (ODataParameterReader) new ODataJsonLightParameterReader(this, operation);

    private ODataBatchReader CreateBatchReaderImplementation(bool synchronous) => (ODataBatchReader) new ODataJsonLightBatchReader(this, synchronous);
  }
}
