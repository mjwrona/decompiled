// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.ODataMessageReader
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
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Microsoft.OData
{
  [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Main resource point for reader functionality")]
  public sealed class ODataMessageReader : IDisposable
  {
    private readonly ODataMessage message;
    private readonly bool readingResponse;
    private readonly ODataMessageReaderSettings settings;
    private readonly IEdmModel model;
    private readonly IODataPayloadUriConverter payloadUriConverter;
    private readonly IServiceProvider container;
    private readonly EdmTypeResolver edmTypeResolver;
    private readonly ODataMediaTypeResolver mediaTypeResolver;
    private bool readMethodCalled;
    private bool isDisposed;
    private ODataInputContext inputContext;
    private ODataPayloadKind readerPayloadKind = ODataPayloadKind.Unsupported;
    private ODataFormat format;
    private ODataMediaType contentType;
    private Encoding encoding;
    private ODataMessageInfo messageInfo;

    public ODataMessageReader(IODataRequestMessage requestMessage)
      : this(requestMessage, (ODataMessageReaderSettings) null)
    {
    }

    public ODataMessageReader(
      IODataRequestMessage requestMessage,
      ODataMessageReaderSettings settings)
      : this(requestMessage, settings, (IEdmModel) null)
    {
    }

    public ODataMessageReader(
      IODataRequestMessage requestMessage,
      ODataMessageReaderSettings settings,
      IEdmModel model)
    {
      ExceptionUtils.CheckArgumentNotNull<IODataRequestMessage>(requestMessage, nameof (requestMessage));
      this.container = ODataMessageReader.GetContainer<IODataRequestMessage>(requestMessage);
      this.settings = ODataMessageReaderSettings.CreateReaderSettings(this.container, settings);
      ReaderValidationUtils.ValidateMessageReaderSettings(this.settings, false);
      this.readingResponse = false;
      this.message = (ODataMessage) new ODataRequestMessage(requestMessage, false, this.settings.EnableMessageStreamDisposal, this.settings.MessageQuotas.MaxReceivedMessageSize);
      this.payloadUriConverter = requestMessage as IODataPayloadUriConverter;
      this.mediaTypeResolver = ODataMediaTypeResolver.GetMediaTypeResolver(this.container);
      ODataVersion odataVersion = ODataUtilsInternal.GetODataVersion(this.message, this.settings.MaxProtocolVersion);
      if (odataVersion > this.settings.MaxProtocolVersion)
        throw new ODataException(Strings.ODataUtils_MaxProtocolVersionExceeded((object) ODataUtils.ODataVersionToString(odataVersion), (object) ODataUtils.ODataVersionToString(this.settings.MaxProtocolVersion)));
      this.model = model ?? ODataMessageReader.GetModel(this.container);
      this.edmTypeResolver = (EdmTypeResolver) new EdmTypeReaderResolver(this.model, this.settings.ClientCustomTypeResolver);
    }

    public ODataMessageReader(IODataResponseMessage responseMessage)
      : this(responseMessage, (ODataMessageReaderSettings) null)
    {
    }

    public ODataMessageReader(
      IODataResponseMessage responseMessage,
      ODataMessageReaderSettings settings)
      : this(responseMessage, settings, (IEdmModel) null)
    {
    }

    public ODataMessageReader(
      IODataResponseMessage responseMessage,
      ODataMessageReaderSettings settings,
      IEdmModel model)
    {
      ExceptionUtils.CheckArgumentNotNull<IODataResponseMessage>(responseMessage, nameof (responseMessage));
      this.container = ODataMessageReader.GetContainer<IODataResponseMessage>(responseMessage);
      this.settings = ODataMessageReaderSettings.CreateReaderSettings(this.container, settings);
      ReaderValidationUtils.ValidateMessageReaderSettings(this.settings, true);
      this.readingResponse = true;
      this.message = (ODataMessage) new ODataResponseMessage(responseMessage, false, this.settings.EnableMessageStreamDisposal, this.settings.MessageQuotas.MaxReceivedMessageSize);
      this.payloadUriConverter = responseMessage as IODataPayloadUriConverter;
      this.mediaTypeResolver = ODataMediaTypeResolver.GetMediaTypeResolver(this.container);
      ODataVersion odataVersion = ODataUtilsInternal.GetODataVersion(this.message, this.settings.MaxProtocolVersion);
      if (odataVersion > this.settings.MaxProtocolVersion)
        throw new ODataException(Strings.ODataUtils_MaxProtocolVersionExceeded((object) ODataUtils.ODataVersionToString(odataVersion), (object) ODataUtils.ODataVersionToString(this.settings.MaxProtocolVersion)));
      this.model = model ?? ODataMessageReader.GetModel(this.container);
      this.edmTypeResolver = (EdmTypeResolver) new EdmTypeReaderResolver(this.model, this.settings.ClientCustomTypeResolver);
      string annotationFilter = responseMessage.PreferenceAppliedHeader().AnnotationFilter;
      if (this.settings.ShouldIncludeAnnotation != null || string.IsNullOrEmpty(annotationFilter))
        return;
      this.settings.ShouldIncludeAnnotation = ODataUtils.CreateAnnotationFilter(annotationFilter);
    }

    internal ODataMessageReaderSettings Settings => this.settings;

    public IEnumerable<ODataPayloadKindDetectionResult> DetectPayloadKind()
    {
      IEnumerable<ODataPayloadKindDetectionResult> payloadKindResults;
      if (this.TryGetSinglePayloadKindResultFromContentType(out payloadKindResults))
        return payloadKindResults;
      List<ODataPayloadKindDetectionResult> kindDetectionResultList = new List<ODataPayloadKindDetectionResult>();
      try
      {
        foreach (IGrouping<ODataFormat, ODataPayloadKindDetectionResult> grouping in payloadKindResults.GroupBy<ODataPayloadKindDetectionResult, ODataFormat>((Func<ODataPayloadKindDetectionResult, ODataFormat>) (kvp => kvp.Format)))
        {
          ODataMessageInfo messageInfo = this.GetOrCreateMessageInfo(this.message.GetStream(), false);
          IEnumerable<ODataPayloadKind> odataPayloadKinds = grouping.Key.DetectPayloadKind(messageInfo, this.settings);
          if (odataPayloadKinds != null)
          {
            foreach (ODataPayloadKind odataPayloadKind in odataPayloadKinds)
            {
              ODataPayloadKind kind = odataPayloadKind;
              if (payloadKindResults.Any<ODataPayloadKindDetectionResult>((Func<ODataPayloadKindDetectionResult, bool>) (pk => pk.PayloadKind == kind)))
                kindDetectionResultList.Add(new ODataPayloadKindDetectionResult(kind, grouping.Key));
            }
          }
        }
      }
      finally
      {
        this.message.UseBufferingReadStream = new bool?(false);
        this.message.BufferingReadStream.StopBuffering();
      }
      kindDetectionResultList.Sort(new Comparison<ODataPayloadKindDetectionResult>(this.ComparePayloadKindDetectionResult));
      return (IEnumerable<ODataPayloadKindDetectionResult>) kindDetectionResultList;
    }

    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Need to a return a task of an enumerable.")]
    public Task<IEnumerable<ODataPayloadKindDetectionResult>> DetectPayloadKindAsync()
    {
      IEnumerable<ODataPayloadKindDetectionResult> payloadKindResults;
      if (this.TryGetSinglePayloadKindResultFromContentType(out payloadKindResults))
        return TaskUtils.GetCompletedTask<IEnumerable<ODataPayloadKindDetectionResult>>(payloadKindResults);
      List<ODataPayloadKindDetectionResult> detectedPayloadKinds = new List<ODataPayloadKindDetectionResult>();
      return Task.Factory.Iterate(this.GetPayloadKindDetectionTasks(payloadKindResults, detectedPayloadKinds)).FollowAlwaysWith((Action<Task>) (t =>
      {
        this.message.UseBufferingReadStream = new bool?(false);
        this.message.BufferingReadStream.StopBuffering();
      })).FollowOnSuccessWith<IEnumerable<ODataPayloadKindDetectionResult>>((Func<Task, IEnumerable<ODataPayloadKindDetectionResult>>) (t =>
      {
        detectedPayloadKinds.Sort(new Comparison<ODataPayloadKindDetectionResult>(this.ComparePayloadKindDetectionResult));
        return (IEnumerable<ODataPayloadKindDetectionResult>) detectedPayloadKinds;
      }));
    }

    public ODataAsynchronousReader CreateODataAsynchronousReader()
    {
      this.VerifyCanCreateODataAsynchronousReader();
      return this.ReadFromInput<ODataAsynchronousReader>((Func<ODataInputContext, ODataAsynchronousReader>) (context => context.CreateAsynchronousReader()), ODataPayloadKind.Asynchronous);
    }

    public Task<ODataAsynchronousReader> CreateODataAsynchronousReaderAsync()
    {
      this.VerifyCanCreateODataAsynchronousReader();
      return this.ReadFromInputAsync<ODataAsynchronousReader>((Func<ODataInputContext, Task<ODataAsynchronousReader>>) (context => context.CreateAsynchronousReaderAsync()), ODataPayloadKind.Asynchronous);
    }

    public ODataReader CreateODataResourceSetReader() => this.CreateODataResourceSetReader((IEdmEntitySetBase) null, (IEdmStructuredType) null);

    public ODataReader CreateODataResourceSetReader(IEdmStructuredType expectedResourceType) => this.CreateODataResourceSetReader((IEdmEntitySetBase) null, expectedResourceType);

    public ODataReader CreateODataResourceSetReader(
      IEdmEntitySetBase entitySet,
      IEdmStructuredType expectedResourceType)
    {
      this.VerifyCanCreateODataResourceSetReader(entitySet, expectedResourceType);
      expectedResourceType = expectedResourceType ?? (IEdmStructuredType) this.edmTypeResolver.GetElementType((IEdmNavigationSource) entitySet);
      return this.ReadFromInput<ODataReader>((Func<ODataInputContext, ODataReader>) (context => context.CreateResourceSetReader(entitySet, expectedResourceType)), new ODataPayloadKind[1]);
    }

    public Task<ODataReader> CreateODataResourceSetReaderAsync() => this.CreateODataResourceSetReaderAsync((IEdmEntitySetBase) null, (IEdmStructuredType) null);

    public Task<ODataReader> CreateODataResourceSetReaderAsync(
      IEdmStructuredType expectedResourceType)
    {
      return this.CreateODataResourceSetReaderAsync((IEdmEntitySetBase) null, expectedResourceType);
    }

    public Task<ODataReader> CreateODataResourceSetReaderAsync(
      IEdmEntitySetBase entitySet,
      IEdmStructuredType expectedResourceType)
    {
      this.VerifyCanCreateODataResourceSetReader(entitySet, expectedResourceType);
      expectedResourceType = expectedResourceType ?? (IEdmStructuredType) this.edmTypeResolver.GetElementType((IEdmNavigationSource) entitySet);
      return this.ReadFromInputAsync<ODataReader>((Func<ODataInputContext, Task<ODataReader>>) (context => context.CreateResourceSetReaderAsync(entitySet, expectedResourceType)), new ODataPayloadKind[1]);
    }

    public ODataReader CreateODataDeltaResourceSetReader() => this.CreateODataDeltaResourceSetReader((IEdmEntitySetBase) null, (IEdmStructuredType) null);

    public ODataReader CreateODataDeltaResourceSetReader(IEdmStructuredType expectedResourceType) => this.CreateODataDeltaResourceSetReader((IEdmEntitySetBase) null, expectedResourceType);

    public ODataReader CreateODataDeltaResourceSetReader(
      IEdmEntitySetBase entitySet,
      IEdmStructuredType expectedResourceType)
    {
      this.VerifyCanCreateODataResourceSetReader(entitySet, expectedResourceType);
      expectedResourceType = expectedResourceType ?? (IEdmStructuredType) this.edmTypeResolver.GetElementType((IEdmNavigationSource) entitySet);
      return this.ReadFromInput<ODataReader>((Func<ODataInputContext, ODataReader>) (context => context.CreateDeltaResourceSetReader(entitySet, expectedResourceType)), new ODataPayloadKind[1]);
    }

    public Task<ODataReader> CreateODataDeltaResourceSetReaderAsync() => this.CreateODataDeltaResourceSetReaderAsync((IEdmEntitySetBase) null, (IEdmStructuredType) null);

    public Task<ODataReader> CreateODataDeltaResourceSetReaderAsync(
      IEdmStructuredType expectedResourceType)
    {
      return this.CreateODataDeltaResourceSetReaderAsync((IEdmEntitySetBase) null, expectedResourceType);
    }

    public Task<ODataReader> CreateODataDeltaResourceSetReaderAsync(
      IEdmEntitySetBase entitySet,
      IEdmStructuredType expectedResourceType)
    {
      this.VerifyCanCreateODataResourceSetReader(entitySet, expectedResourceType);
      expectedResourceType = expectedResourceType ?? (IEdmStructuredType) this.edmTypeResolver.GetElementType((IEdmNavigationSource) entitySet);
      return this.ReadFromInputAsync<ODataReader>((Func<ODataInputContext, Task<ODataReader>>) (context => context.CreateDeltaResourceSetReaderAsync(entitySet, expectedResourceType)), ODataPayloadKind.Delta);
    }

    [Obsolete("Use CreateODataDeltaResourceSetReader.", false)]
    public ODataDeltaReader CreateODataDeltaReader(
      IEdmEntitySetBase entitySet,
      IEdmEntityType expectedBaseEntityType)
    {
      this.VerifyCanCreateODataDeltaReader(entitySet, expectedBaseEntityType);
      expectedBaseEntityType = expectedBaseEntityType ?? this.edmTypeResolver.GetElementType((IEdmNavigationSource) entitySet);
      return this.ReadFromInput<ODataDeltaReader>((Func<ODataInputContext, ODataDeltaReader>) (context => context.CreateDeltaReader(entitySet, expectedBaseEntityType)), new ODataPayloadKind[1]);
    }

    [Obsolete("Use CreateODataDeltaResourceSetReader.", false)]
    public Task<ODataDeltaReader> CreateODataDeltaReaderAsync(
      IEdmEntitySetBase entitySet,
      IEdmEntityType expectedBaseEntityType)
    {
      this.VerifyCanCreateODataResourceSetReader(entitySet, (IEdmStructuredType) expectedBaseEntityType);
      expectedBaseEntityType = expectedBaseEntityType ?? this.edmTypeResolver.GetElementType((IEdmNavigationSource) entitySet);
      return this.ReadFromInputAsync<ODataDeltaReader>((Func<ODataInputContext, Task<ODataDeltaReader>>) (context => context.CreateDeltaReaderAsync(entitySet, expectedBaseEntityType)), new ODataPayloadKind[1]);
    }

    public ODataReader CreateODataResourceReader() => this.CreateODataResourceReader((IEdmNavigationSource) null, (IEdmStructuredType) null);

    public ODataReader CreateODataResourceReader(IEdmStructuredType resourceType) => this.CreateODataResourceReader((IEdmNavigationSource) null, resourceType);

    public ODataReader CreateODataResourceReader(
      IEdmNavigationSource navigationSource,
      IEdmStructuredType resourceType)
    {
      this.VerifyCanCreateODataResourceReader(navigationSource, resourceType);
      resourceType = resourceType ?? (IEdmStructuredType) this.edmTypeResolver.GetElementType(navigationSource);
      return this.ReadFromInput<ODataReader>((Func<ODataInputContext, ODataReader>) (context => context.CreateResourceReader(navigationSource, resourceType)), ODataPayloadKind.Resource);
    }

    public Task<ODataReader> CreateODataResourceReaderAsync() => this.CreateODataResourceReaderAsync((IEdmNavigationSource) null, (IEdmStructuredType) null);

    public Task<ODataReader> CreateODataResourceReaderAsync(IEdmStructuredType resourceType) => this.CreateODataResourceReaderAsync((IEdmNavigationSource) null, resourceType);

    public Task<ODataReader> CreateODataResourceReaderAsync(
      IEdmNavigationSource navigationSource,
      IEdmStructuredType resourceType)
    {
      this.VerifyCanCreateODataResourceReader(navigationSource, resourceType);
      resourceType = resourceType ?? (IEdmStructuredType) this.edmTypeResolver.GetElementType(navigationSource);
      return this.ReadFromInputAsync<ODataReader>((Func<ODataInputContext, Task<ODataReader>>) (context => context.CreateResourceReaderAsync(navigationSource, resourceType)), ODataPayloadKind.Resource);
    }

    public ODataCollectionReader CreateODataCollectionReader() => this.CreateODataCollectionReader((IEdmTypeReference) null);

    public ODataCollectionReader CreateODataCollectionReader(
      IEdmTypeReference expectedItemTypeReference)
    {
      this.VerifyCanCreateODataCollectionReader(expectedItemTypeReference);
      return this.ReadFromInput<ODataCollectionReader>((Func<ODataInputContext, ODataCollectionReader>) (context => context.CreateCollectionReader(expectedItemTypeReference)), ODataPayloadKind.Collection);
    }

    public Task<ODataCollectionReader> CreateODataCollectionReaderAsync() => this.CreateODataCollectionReaderAsync((IEdmTypeReference) null);

    public Task<ODataCollectionReader> CreateODataCollectionReaderAsync(
      IEdmTypeReference expectedItemTypeReference)
    {
      this.VerifyCanCreateODataCollectionReader(expectedItemTypeReference);
      return this.ReadFromInputAsync<ODataCollectionReader>((Func<ODataInputContext, Task<ODataCollectionReader>>) (context => context.CreateCollectionReaderAsync(expectedItemTypeReference)), ODataPayloadKind.Collection);
    }

    public ODataBatchReader CreateODataBatchReader()
    {
      this.VerifyCanCreateODataBatchReader();
      return this.ReadFromInput<ODataBatchReader>((Func<ODataInputContext, ODataBatchReader>) (context => context.CreateBatchReader()), ODataPayloadKind.Batch);
    }

    public Task<ODataBatchReader> CreateODataBatchReaderAsync()
    {
      this.VerifyCanCreateODataBatchReader();
      return this.ReadFromInputAsync<ODataBatchReader>((Func<ODataInputContext, Task<ODataBatchReader>>) (context => context.CreateBatchReaderAsync()), ODataPayloadKind.Batch);
    }

    public ODataReader CreateODataUriParameterResourceReader(
      IEdmNavigationSource navigationSource,
      IEdmStructuredType expectedResourceType)
    {
      this.VerifyCanCreateODataResourceReader(navigationSource, expectedResourceType);
      expectedResourceType = expectedResourceType ?? (IEdmStructuredType) this.edmTypeResolver.GetElementType(navigationSource);
      return this.ReadFromInput<ODataReader>((Func<ODataInputContext, ODataReader>) (context => context.CreateUriParameterResourceReader(navigationSource, expectedResourceType)), ODataPayloadKind.Resource);
    }

    public Task<ODataReader> CreateODataUriParameterResourceReaderAsync(
      IEdmNavigationSource navigationSource,
      IEdmStructuredType expectedResourceType)
    {
      this.VerifyCanCreateODataResourceReader(navigationSource, expectedResourceType);
      expectedResourceType = expectedResourceType ?? (IEdmStructuredType) this.edmTypeResolver.GetElementType(navigationSource);
      return this.ReadFromInputAsync<ODataReader>((Func<ODataInputContext, Task<ODataReader>>) (context => context.CreateUriParameterResourceReaderAsync(navigationSource, expectedResourceType)), ODataPayloadKind.Resource);
    }

    public ODataReader CreateODataUriParameterResourceSetReader(
      IEdmEntitySetBase entitySet,
      IEdmStructuredType expectedResourceType)
    {
      this.VerifyCanCreateODataResourceSetReader(entitySet, expectedResourceType);
      expectedResourceType = expectedResourceType ?? (IEdmStructuredType) this.edmTypeResolver.GetElementType((IEdmNavigationSource) entitySet);
      return this.ReadFromInput<ODataReader>((Func<ODataInputContext, ODataReader>) (context => context.CreateUriParameterResourceSetReader(entitySet, expectedResourceType)), new ODataPayloadKind[1]);
    }

    public Task<ODataReader> CreateODataUriParameterResourceSetReaderAsync(
      IEdmEntitySetBase entitySet,
      IEdmStructuredType expectedResourceType)
    {
      this.VerifyCanCreateODataResourceSetReader(entitySet, expectedResourceType);
      expectedResourceType = expectedResourceType ?? (IEdmStructuredType) this.edmTypeResolver.GetElementType((IEdmNavigationSource) entitySet);
      return this.ReadFromInputAsync<ODataReader>((Func<ODataInputContext, Task<ODataReader>>) (context => context.CreateUriParameterResourceSetReaderAsync(entitySet, expectedResourceType)), new ODataPayloadKind[1]);
    }

    public ODataParameterReader CreateODataParameterReader(IEdmOperation operation)
    {
      this.VerifyCanCreateODataParameterReader(operation);
      return this.ReadFromInput<ODataParameterReader>((Func<ODataInputContext, ODataParameterReader>) (context => context.CreateParameterReader(operation)), ODataPayloadKind.Parameter);
    }

    public Task<ODataParameterReader> CreateODataParameterReaderAsync(IEdmOperation operation)
    {
      this.VerifyCanCreateODataParameterReader(operation);
      return this.ReadFromInputAsync<ODataParameterReader>((Func<ODataInputContext, Task<ODataParameterReader>>) (context => context.CreateParameterReaderAsync(operation)), ODataPayloadKind.Parameter);
    }

    public ODataServiceDocument ReadServiceDocument()
    {
      this.VerifyCanReadServiceDocument();
      return this.ReadFromInput<ODataServiceDocument>((Func<ODataInputContext, ODataServiceDocument>) (context => context.ReadServiceDocument()), ODataPayloadKind.ServiceDocument);
    }

    public Task<ODataServiceDocument> ReadServiceDocumentAsync()
    {
      this.VerifyCanReadServiceDocument();
      return this.ReadFromInputAsync<ODataServiceDocument>((Func<ODataInputContext, Task<ODataServiceDocument>>) (context => context.ReadServiceDocumentAsync()), ODataPayloadKind.ServiceDocument);
    }

    public ODataProperty ReadProperty() => this.ReadProperty((IEdmTypeReference) null);

    public ODataProperty ReadProperty(IEdmTypeReference expectedPropertyTypeReference)
    {
      this.VerifyCanReadProperty(expectedPropertyTypeReference);
      return this.ReadFromInput<ODataProperty>((Func<ODataInputContext, ODataProperty>) (context => context.ReadProperty((IEdmStructuralProperty) null, expectedPropertyTypeReference)), ODataPayloadKind.Property);
    }

    public ODataProperty ReadProperty(IEdmStructuralProperty property)
    {
      this.VerifyCanReadProperty(property);
      return this.ReadFromInput<ODataProperty>((Func<ODataInputContext, ODataProperty>) (context => context.ReadProperty(property, property.Type)), ODataPayloadKind.Property);
    }

    public Task<ODataProperty> ReadPropertyAsync() => this.ReadPropertyAsync((IEdmTypeReference) null);

    public Task<ODataProperty> ReadPropertyAsync(IEdmTypeReference expectedPropertyTypeReference)
    {
      this.VerifyCanReadProperty(expectedPropertyTypeReference);
      return this.ReadFromInputAsync<ODataProperty>((Func<ODataInputContext, Task<ODataProperty>>) (context => context.ReadPropertyAsync((IEdmStructuralProperty) null, expectedPropertyTypeReference)), ODataPayloadKind.Property);
    }

    public Task<ODataProperty> ReadPropertyAsync(IEdmStructuralProperty property)
    {
      this.VerifyCanReadProperty(property);
      return this.ReadFromInputAsync<ODataProperty>((Func<ODataInputContext, Task<ODataProperty>>) (context => context.ReadPropertyAsync(property, property.Type)), ODataPayloadKind.Property);
    }

    public ODataError ReadError()
    {
      this.VerifyCanReadError();
      return this.ReadFromInput<ODataError>((Func<ODataInputContext, ODataError>) (context => context.ReadError()), ODataPayloadKind.Error);
    }

    public Task<ODataError> ReadErrorAsync()
    {
      this.VerifyCanReadError();
      return this.ReadFromInputAsync<ODataError>((Func<ODataInputContext, Task<ODataError>>) (context => context.ReadErrorAsync()), ODataPayloadKind.Error);
    }

    public ODataEntityReferenceLinks ReadEntityReferenceLinks()
    {
      this.VerifyCanReadEntityReferenceLinks();
      return this.ReadFromInput<ODataEntityReferenceLinks>((Func<ODataInputContext, ODataEntityReferenceLinks>) (context => context.ReadEntityReferenceLinks()), ODataPayloadKind.EntityReferenceLinks);
    }

    public Task<ODataEntityReferenceLinks> ReadEntityReferenceLinksAsync()
    {
      this.VerifyCanReadEntityReferenceLinks();
      return this.ReadFromInputAsync<ODataEntityReferenceLinks>((Func<ODataInputContext, Task<ODataEntityReferenceLinks>>) (context => context.ReadEntityReferenceLinksAsync()), ODataPayloadKind.EntityReferenceLinks);
    }

    public ODataEntityReferenceLink ReadEntityReferenceLink()
    {
      this.VerifyCanReadEntityReferenceLink();
      return this.ReadFromInput<ODataEntityReferenceLink>((Func<ODataInputContext, ODataEntityReferenceLink>) (context => context.ReadEntityReferenceLink()), ODataPayloadKind.EntityReferenceLink);
    }

    public Task<ODataEntityReferenceLink> ReadEntityReferenceLinkAsync()
    {
      this.VerifyCanReadEntityReferenceLink();
      return this.ReadFromInputAsync<ODataEntityReferenceLink>((Func<ODataInputContext, Task<ODataEntityReferenceLink>>) (context => context.ReadEntityReferenceLinkAsync()), ODataPayloadKind.EntityReferenceLink);
    }

    public object ReadValue(IEdmTypeReference expectedTypeReference) => this.ReadFromInput<object>((Func<ODataInputContext, object>) (context => context.ReadValue(expectedTypeReference.AsPrimitiveOrNull())), this.VerifyCanReadValue(expectedTypeReference));

    public Task<object> ReadValueAsync(IEdmTypeReference expectedTypeReference) => this.ReadFromInputAsync<object>((Func<ODataInputContext, Task<object>>) (context => context.ReadValueAsync((IEdmPrimitiveTypeReference) expectedTypeReference)), this.VerifyCanReadValue(expectedTypeReference));

    public IEdmModel ReadMetadataDocument()
    {
      this.VerifyCanReadMetadataDocument();
      return this.ReadFromInput<IEdmModel>((Func<ODataInputContext, IEdmModel>) (context => context.ReadMetadataDocument((Func<Uri, XmlReader>) null)), ODataPayloadKind.MetadataDocument);
    }

    public IEdmModel ReadMetadataDocument(Func<Uri, XmlReader> getReferencedModelReaderFunc)
    {
      this.VerifyCanReadMetadataDocument();
      return this.ReadFromInput<IEdmModel>((Func<ODataInputContext, IEdmModel>) (context => context.ReadMetadataDocument(getReferencedModelReaderFunc)), ODataPayloadKind.MetadataDocument);
    }

    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize((object) this);
    }

    internal ODataFormat GetFormat() => this.format != null ? this.format : throw new ODataException(Strings.ODataMessageReader_GetFormatCalledBeforeReadingStarted);

    private static IServiceProvider GetContainer<T>(T message) where T : class => message is IContainerProvider containerProvider ? containerProvider.Container : (IServiceProvider) null;

    private static IEdmModel GetModel(IServiceProvider container) => container != null ? container.GetRequiredService<IEdmModel>() : (IEdmModel) EdmCoreModel.Instance;

    private ODataMessageInfo GetOrCreateMessageInfo(Stream messageStream, bool isAsync)
    {
      if (this.messageInfo == null)
      {
        this.messageInfo = this.container != null ? this.container.GetRequiredService<ODataMessageInfo>() : new ODataMessageInfo();
        this.messageInfo.Encoding = this.encoding;
        this.messageInfo.IsResponse = this.readingResponse;
        this.messageInfo.IsAsync = isAsync;
        this.messageInfo.MediaType = this.contentType;
        this.messageInfo.Model = this.model;
        this.messageInfo.PayloadUriConverter = this.payloadUriConverter;
        this.messageInfo.Container = this.container;
        this.messageInfo.MessageStream = messageStream;
        this.messageInfo.PayloadKind = this.readerPayloadKind;
      }
      return this.messageInfo;
    }

    private void ProcessContentType(params ODataPayloadKind[] payloadKinds) => this.format = MediaTypeUtils.GetFormatFromContentType(this.GetContentTypeHeader(payloadKinds), payloadKinds, this.mediaTypeResolver, out this.contentType, out this.encoding, out this.readerPayloadKind);

    private string GetContentTypeHeader(params ODataPayloadKind[] payloadKinds)
    {
      string header = this.message.GetHeader("Content-Type");
      string contentTypeHeader = header == null ? (string) null : header.Trim();
      if (string.IsNullOrEmpty(contentTypeHeader))
      {
        if (this.GetContentLengthHeader() != 0)
          throw new ODataContentTypeException(Strings.ODataMessageReader_NoneOrEmptyContentTypeHeader);
        contentTypeHeader = !((IEnumerable<ODataPayloadKind>) payloadKinds).Contains<ODataPayloadKind>(ODataPayloadKind.Value) ? (!((IEnumerable<ODataPayloadKind>) payloadKinds).Contains<ODataPayloadKind>(ODataPayloadKind.BinaryValue) ? "application/json" : "application/octet-stream") : "text/plain";
      }
      return contentTypeHeader;
    }

    private int GetContentLengthHeader()
    {
      int result = 0;
      int.TryParse(this.message.GetHeader("Content-Length"), out result);
      return result;
    }

    private void VerifyCanCreateODataResourceSetReader(
      IEdmEntitySetBase entitySet,
      IEdmStructuredType expectedBaseResourceType)
    {
      this.VerifyReaderNotDisposedAndNotUsed();
      if (this.model.IsUserModel())
        return;
      if (entitySet != null)
        throw new ArgumentException(Strings.ODataMessageReader_EntitySetSpecifiedWithoutMetadata((object) nameof (entitySet)), nameof (entitySet));
      if (expectedBaseResourceType != null)
        throw new ArgumentException(Strings.ODataMessageReader_ExpectedTypeSpecifiedWithoutMetadata((object) "expectedBaseEntityType"), "expectedBaseEntityType");
    }

    private void VerifyCanCreateODataDeltaReader(
      IEdmEntitySetBase entitySet,
      IEdmEntityType expectedBaseEntityType)
    {
      this.VerifyReaderNotDisposedAndNotUsed();
      if (!this.readingResponse)
        throw new ODataException(Strings.ODataMessageReader_DeltaInRequest);
      if (this.model.IsUserModel())
        return;
      if (entitySet != null)
        throw new ArgumentException(Strings.ODataMessageReader_EntitySetSpecifiedWithoutMetadata((object) nameof (entitySet)), nameof (entitySet));
      if (expectedBaseEntityType != null)
        throw new ArgumentException(Strings.ODataMessageReader_ExpectedTypeSpecifiedWithoutMetadata((object) nameof (expectedBaseEntityType)), nameof (expectedBaseEntityType));
    }

    private void VerifyCanCreateODataResourceReader(
      IEdmNavigationSource navigationSource,
      IEdmStructuredType resourceType)
    {
      this.VerifyReaderNotDisposedAndNotUsed();
      if (this.model.IsUserModel())
        return;
      if (navigationSource != null)
        throw new ArgumentException(Strings.ODataMessageReader_EntitySetSpecifiedWithoutMetadata((object) nameof (navigationSource)), nameof (navigationSource));
      if (resourceType != null)
        throw new ArgumentException(Strings.ODataMessageReader_ExpectedTypeSpecifiedWithoutMetadata((object) nameof (resourceType)), nameof (resourceType));
    }

    private void VerifyCanCreateODataCollectionReader(IEdmTypeReference expectedItemTypeReference)
    {
      this.VerifyReaderNotDisposedAndNotUsed();
      if (expectedItemTypeReference == null)
        return;
      if (!this.model.IsUserModel())
        throw new ArgumentException(Strings.ODataMessageReader_ExpectedTypeSpecifiedWithoutMetadata((object) nameof (expectedItemTypeReference)), nameof (expectedItemTypeReference));
      if (!expectedItemTypeReference.IsODataPrimitiveTypeKind() && expectedItemTypeReference.TypeKind() != EdmTypeKind.Complex && expectedItemTypeReference.TypeKind() != EdmTypeKind.Enum)
        throw new ArgumentException(Strings.ODataMessageReader_ExpectedCollectionTypeWrongKind((object) expectedItemTypeReference.TypeKind().ToString()), nameof (expectedItemTypeReference));
    }

    private void VerifyCanCreateODataAsynchronousReader() => this.VerifyReaderNotDisposedAndNotUsed();

    private void VerifyCanCreateODataBatchReader() => this.VerifyReaderNotDisposedAndNotUsed();

    private void VerifyCanCreateODataParameterReader(IEdmOperation operation)
    {
      this.VerifyReaderNotDisposedAndNotUsed();
      if (this.readingResponse)
        throw new ODataException(Strings.ODataMessageReader_ParameterPayloadInResponse);
      if (operation != null && !this.model.IsUserModel())
        throw new ArgumentException(Strings.ODataMessageReader_OperationSpecifiedWithoutMetadata((object) nameof (operation)), nameof (operation));
    }

    private void VerifyCanReadServiceDocument()
    {
      this.VerifyReaderNotDisposedAndNotUsed();
      if (!this.readingResponse)
        throw new ODataException(Strings.ODataMessageReader_ServiceDocumentInRequest);
    }

    private void VerifyCanReadMetadataDocument()
    {
      this.VerifyReaderNotDisposedAndNotUsed();
      if (!this.readingResponse)
        throw new ODataException(Strings.ODataMessageReader_MetadataDocumentInRequest);
    }

    private void VerifyCanReadProperty(IEdmStructuralProperty property)
    {
      if (property == null)
        return;
      this.VerifyCanReadProperty(property.Type);
    }

    private void VerifyCanReadProperty(IEdmTypeReference expectedPropertyTypeReference)
    {
      this.VerifyReaderNotDisposedAndNotUsed();
      if (expectedPropertyTypeReference == null)
        return;
      if (!this.model.IsUserModel())
        throw new ArgumentException(Strings.ODataMessageReader_ExpectedTypeSpecifiedWithoutMetadata((object) nameof (expectedPropertyTypeReference)), nameof (expectedPropertyTypeReference));
      if (expectedPropertyTypeReference.Definition is IEdmCollectionType definition && definition.ElementType.IsODataEntityTypeKind())
        throw new ArgumentException(Strings.ODataMessageReader_ExpectedPropertyTypeEntityCollectionKind, nameof (expectedPropertyTypeReference));
      if (expectedPropertyTypeReference.IsODataEntityTypeKind())
        throw new ArgumentException(Strings.ODataMessageReader_ExpectedPropertyTypeEntityKind, nameof (expectedPropertyTypeReference));
      if (expectedPropertyTypeReference.IsStream())
        throw new ArgumentException(Strings.ODataMessageReader_ExpectedPropertyTypeStream, nameof (expectedPropertyTypeReference));
    }

    private void VerifyCanReadError()
    {
      this.VerifyReaderNotDisposedAndNotUsed();
      if (!this.readingResponse)
        throw new ODataException(Strings.ODataMessageReader_ErrorPayloadInRequest);
    }

    private void VerifyCanReadEntityReferenceLinks() => this.VerifyReaderNotDisposedAndNotUsed();

    private void VerifyCanReadEntityReferenceLink() => this.VerifyReaderNotDisposedAndNotUsed();

    private ODataPayloadKind[] VerifyCanReadValue(IEdmTypeReference expectedTypeReference)
    {
      this.VerifyReaderNotDisposedAndNotUsed();
      if (expectedTypeReference != null)
      {
        if (!expectedTypeReference.IsODataPrimitiveTypeKind() && !expectedTypeReference.IsODataTypeDefinitionTypeKind())
          throw new ArgumentException(Strings.ODataMessageReader_ExpectedValueTypeWrongKind((object) expectedTypeReference.TypeKind().ToString()), nameof (expectedTypeReference));
        return expectedTypeReference.IsBinary() ? new ODataPayloadKind[1]
        {
          ODataPayloadKind.BinaryValue
        } : new ODataPayloadKind[1]
        {
          ODataPayloadKind.Value
        };
      }
      return new ODataPayloadKind[2]
      {
        ODataPayloadKind.Value,
        ODataPayloadKind.BinaryValue
      };
    }

    private void VerifyReaderNotDisposedAndNotUsed()
    {
      this.VerifyNotDisposed();
      if (this.readMethodCalled)
        throw new ODataException(Strings.ODataMessageReader_ReaderAlreadyUsed);
      if (this.message.BufferingReadStream != null && this.message.BufferingReadStream.IsBuffering)
        throw new ODataException(Strings.ODataMessageReader_PayloadKindDetectionRunning);
      this.readMethodCalled = true;
    }

    private void VerifyNotDisposed()
    {
      if (this.isDisposed)
        throw new ObjectDisposedException(this.GetType().FullName);
    }

    private void Dispose(bool disposing)
    {
      this.isDisposed = true;
      if (!disposing)
        return;
      try
      {
        if (this.inputContext != null)
          this.inputContext.Dispose();
      }
      finally
      {
        this.inputContext = (ODataInputContext) null;
      }
      if (!this.settings.EnableMessageStreamDisposal || this.message.BufferingReadStream == null)
        return;
      this.message.BufferingReadStream.Dispose();
    }

    private T ReadFromInput<T>(
      Func<ODataInputContext, T> readFunc,
      params ODataPayloadKind[] payloadKinds)
      where T : class
    {
      this.ProcessContentType(payloadKinds);
      this.inputContext = this.format.CreateInputContext(this.GetOrCreateMessageInfo(this.message.GetStream(), false), this.settings);
      return readFunc(this.inputContext);
    }

    private bool TryGetSinglePayloadKindResultFromContentType(
      out IEnumerable<ODataPayloadKindDetectionResult> payloadKindResults)
    {
      bool? bufferingReadStream = this.message.UseBufferingReadStream;
      bool flag = true;
      if (bufferingReadStream.GetValueOrDefault() == flag & bufferingReadStream.HasValue)
        throw new ODataException(Strings.ODataMessageReader_DetectPayloadKindMultipleTimes);
      IList<ODataPayloadKindDetectionResult> kindsForContentType = MediaTypeUtils.GetPayloadKindsForContentType(this.GetContentTypeHeader(), this.mediaTypeResolver, out this.contentType, out this.encoding);
      payloadKindResults = kindsForContentType.Where<ODataPayloadKindDetectionResult>((Func<ODataPayloadKindDetectionResult, bool>) (r => ODataUtilsInternal.IsPayloadKindSupported(r.PayloadKind, !this.readingResponse)));
      if (payloadKindResults.Count<ODataPayloadKindDetectionResult>() <= 1)
        return true;
      this.message.UseBufferingReadStream = new bool?(true);
      return false;
    }

    private int ComparePayloadKindDetectionResult(
      ODataPayloadKindDetectionResult first,
      ODataPayloadKindDetectionResult second)
    {
      if (first.PayloadKind == second.PayloadKind)
        return 0;
      return first.PayloadKind >= second.PayloadKind ? 1 : -1;
    }

    private IEnumerable<Task> GetPayloadKindDetectionTasks(
      IEnumerable<ODataPayloadKindDetectionResult> payloadKindsFromContentType,
      List<ODataPayloadKindDetectionResult> detectionResults)
    {
      foreach (IGrouping<ODataFormat, ODataPayloadKindDetectionResult> grouping in payloadKindsFromContentType.GroupBy<ODataPayloadKindDetectionResult, ODataFormat>((Func<ODataPayloadKindDetectionResult, ODataFormat>) (kvp => kvp.Format)))
      {
        IGrouping<ODataFormat, ODataPayloadKindDetectionResult> payloadKindGroup = grouping;
        yield return this.message.GetStreamAsync().FollowOnSuccessWithTask<Stream, IEnumerable<ODataPayloadKind>>((Func<Task<Stream>, Task<IEnumerable<ODataPayloadKind>>>) (streamTask => payloadKindGroup.Key.DetectPayloadKindAsync(this.GetOrCreateMessageInfo(streamTask.Result, true), this.settings))).FollowOnSuccessWith<IEnumerable<ODataPayloadKind>>((Action<Task<IEnumerable<ODataPayloadKind>>>) (t =>
        {
          IEnumerable<ODataPayloadKind> result = t.Result;
          if (result == null)
            return;
          foreach (ODataPayloadKind odataPayloadKind in result)
          {
            ODataPayloadKind kind = odataPayloadKind;
            if (payloadKindsFromContentType.Any<ODataPayloadKindDetectionResult>((Func<ODataPayloadKindDetectionResult, bool>) (pk => pk.PayloadKind == kind)))
              detectionResults.Add(new ODataPayloadKindDetectionResult(kind, payloadKindGroup.Key));
          }
        }));
      }
    }

    private Task<T> ReadFromInputAsync<T>(
      Func<ODataInputContext, Task<T>> readFunc,
      params ODataPayloadKind[] payloadKinds)
      where T : class
    {
      this.ProcessContentType(payloadKinds);
      return this.message.GetStreamAsync().FollowOnSuccessWithTask<Stream, ODataInputContext>((Func<Task<Stream>, Task<ODataInputContext>>) (streamTask => this.format.CreateInputContextAsync(this.GetOrCreateMessageInfo(streamTask.Result, true), this.settings))).FollowOnSuccessWithTask<ODataInputContext, T>((Func<Task<ODataInputContext>, Task<T>>) (createInputContextTask =>
      {
        this.inputContext = createInputContextTask.Result;
        return readFunc(this.inputContext);
      }));
    }
  }
}
