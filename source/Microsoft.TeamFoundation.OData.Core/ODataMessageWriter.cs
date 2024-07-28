// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.ODataMessageWriter
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Edm;
using Microsoft.OData.Metadata;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.OData
{
  public sealed class ODataMessageWriter : IDisposable
  {
    private readonly ODataMessage message;
    private readonly bool writingResponse;
    private readonly ODataMessageWriterSettings settings;
    private readonly IEdmModel model;
    private readonly IODataPayloadUriConverter payloadUriConverter;
    private readonly IServiceProvider container;
    private readonly ODataMediaTypeResolver mediaTypeResolver;
    private bool writeMethodCalled;
    private bool isDisposed;
    private ODataOutputContext outputContext;
    private ODataPayloadKind writerPayloadKind = ODataPayloadKind.Unsupported;
    private ODataFormat format;
    private Encoding encoding;
    private bool writeErrorCalled;
    private ODataMediaType mediaType;
    private ODataMessageInfo messageInfo;

    public ODataMessageWriter(IODataRequestMessage requestMessage)
      : this(requestMessage, (ODataMessageWriterSettings) null)
    {
    }

    public ODataMessageWriter(
      IODataRequestMessage requestMessage,
      ODataMessageWriterSettings settings)
      : this(requestMessage, settings, (IEdmModel) null)
    {
    }

    public ODataMessageWriter(
      IODataRequestMessage requestMessage,
      ODataMessageWriterSettings settings,
      IEdmModel model)
    {
      ExceptionUtils.CheckArgumentNotNull<IODataRequestMessage>(requestMessage, nameof (requestMessage));
      this.container = ODataMessageWriter.GetContainer<IODataRequestMessage>(requestMessage);
      this.settings = ODataMessageWriterSettings.CreateWriterSettings(this.container, settings);
      this.writingResponse = false;
      this.payloadUriConverter = requestMessage as IODataPayloadUriConverter;
      this.mediaTypeResolver = ODataMediaTypeResolver.GetMediaTypeResolver(this.container);
      this.model = model ?? ODataMessageWriter.GetModel(this.container);
      WriterValidationUtils.ValidateMessageWriterSettings(this.settings, this.writingResponse);
      this.message = (ODataMessage) new ODataRequestMessage(requestMessage, true, this.settings.EnableMessageStreamDisposal, -1L);
      this.settings.ShouldIncludeAnnotation = new Func<string, bool>(AnnotationFilter.CreateIncludeAllFilter().Matches);
    }

    public ODataMessageWriter(IODataResponseMessage responseMessage)
      : this(responseMessage, (ODataMessageWriterSettings) null)
    {
    }

    public ODataMessageWriter(
      IODataResponseMessage responseMessage,
      ODataMessageWriterSettings settings)
      : this(responseMessage, settings, (IEdmModel) null)
    {
    }

    public ODataMessageWriter(
      IODataResponseMessage responseMessage,
      ODataMessageWriterSettings settings,
      IEdmModel model)
    {
      ExceptionUtils.CheckArgumentNotNull<IODataResponseMessage>(responseMessage, nameof (responseMessage));
      this.container = ODataMessageWriter.GetContainer<IODataResponseMessage>(responseMessage);
      this.settings = ODataMessageWriterSettings.CreateWriterSettings(this.container, settings);
      this.writingResponse = true;
      this.payloadUriConverter = responseMessage as IODataPayloadUriConverter;
      this.mediaTypeResolver = ODataMediaTypeResolver.GetMediaTypeResolver(this.container);
      this.model = model ?? ODataMessageWriter.GetModel(this.container);
      WriterValidationUtils.ValidateMessageWriterSettings(this.settings, this.writingResponse);
      this.message = (ODataMessage) new ODataResponseMessage(responseMessage, true, this.settings.EnableMessageStreamDisposal, -1L);
      if (string.Equals(responseMessage.PreferenceAppliedHeader().OmitValues, "nulls", StringComparison.OrdinalIgnoreCase))
        this.settings.OmitNullValues = true;
      string annotationFilter = responseMessage.PreferenceAppliedHeader().AnnotationFilter;
      if (string.IsNullOrEmpty(annotationFilter))
        return;
      this.settings.ShouldIncludeAnnotation = ODataUtils.CreateAnnotationFilter(annotationFilter);
    }

    internal ODataMessageWriterSettings Settings => this.settings;

    public ODataAsynchronousWriter CreateODataAsynchronousWriter()
    {
      this.VerifyCanCreateODataAsyncWriter();
      return this.WriteToOutput<ODataAsynchronousWriter>(ODataPayloadKind.Asynchronous, (Func<ODataOutputContext, ODataAsynchronousWriter>) (context => context.CreateODataAsynchronousWriter()));
    }

    public ODataWriter CreateODataResourceSetWriter() => this.CreateODataResourceSetWriter((IEdmEntitySetBase) null, (IEdmStructuredType) null);

    public ODataWriter CreateODataResourceSetWriter(IEdmEntitySetBase entitySet) => this.CreateODataResourceSetWriter(entitySet, (IEdmStructuredType) null);

    public ODataWriter CreateODataResourceSetWriter(
      IEdmEntitySetBase entitySet,
      IEdmStructuredType resourceType)
    {
      this.VerifyCanCreateODataResourceSetWriter();
      return this.WriteToOutput<ODataWriter>(ODataPayloadKind.ResourceSet, (Func<ODataOutputContext, ODataWriter>) (context => context.CreateODataResourceSetWriter(entitySet, resourceType)));
    }

    public Task<ODataAsynchronousWriter> CreateODataAsynchronousWriterAsync()
    {
      this.VerifyCanCreateODataAsyncWriter();
      return this.WriteToOutputAsync<ODataAsynchronousWriter>(ODataPayloadKind.Asynchronous, (Func<ODataOutputContext, Task<ODataAsynchronousWriter>>) (context => context.CreateODataAsynchronousWriterAsync()));
    }

    public Task<ODataWriter> CreateODataResourceSetWriterAsync() => this.CreateODataResourceSetWriterAsync((IEdmEntitySetBase) null, (IEdmEntityType) null);

    public Task<ODataWriter> CreateODataResourceSetWriterAsync(IEdmEntitySetBase entitySet) => this.CreateODataResourceSetWriterAsync(entitySet, (IEdmEntityType) null);

    public Task<ODataWriter> CreateODataResourceSetWriterAsync(
      IEdmEntitySetBase entitySet,
      IEdmEntityType entityType)
    {
      this.VerifyCanCreateODataResourceSetWriter();
      return this.WriteToOutputAsync<ODataWriter>(ODataPayloadKind.ResourceSet, (Func<ODataOutputContext, Task<ODataWriter>>) (context => context.CreateODataResourceSetWriterAsync(entitySet, (IEdmStructuredType) entityType)));
    }

    public ODataWriter CreateODataDeltaResourceSetWriter() => this.CreateODataDeltaResourceSetWriter((IEdmEntitySetBase) null, (IEdmStructuredType) null);

    public ODataWriter CreateODataDeltaResourceSetWriter(IEdmEntitySetBase entitySet) => this.CreateODataDeltaResourceSetWriter(entitySet, (IEdmStructuredType) null);

    public ODataWriter CreateODataDeltaResourceSetWriter(
      IEdmEntitySetBase entitySet,
      IEdmStructuredType resourceType)
    {
      this.VerifyCanCreateODataResourceSetWriter();
      return this.WriteToOutput<ODataWriter>(ODataPayloadKind.ResourceSet, (Func<ODataOutputContext, ODataWriter>) (context => context.CreateODataDeltaResourceSetWriter(entitySet, resourceType)));
    }

    public Task<ODataWriter> CreateODataDeltaResourceSetWriterAsync() => this.CreateODataDeltaResourceSetWriterAsync((IEdmEntitySetBase) null, (IEdmEntityType) null);

    public Task<ODataWriter> CreateODataDeltaResourceSetWriterAsync(IEdmEntitySetBase entitySet) => this.CreateODataDeltaResourceSetWriterAsync(entitySet, (IEdmEntityType) null);

    public Task<ODataWriter> CreateODataDeltaResourceSetWriterAsync(
      IEdmEntitySetBase entitySet,
      IEdmEntityType entityType)
    {
      this.VerifyCanCreateODataResourceSetWriter();
      return this.WriteToOutputAsync<ODataWriter>(ODataPayloadKind.ResourceSet, (Func<ODataOutputContext, Task<ODataWriter>>) (context => context.CreateODataDeltaResourceSetWriterAsync(entitySet, (IEdmStructuredType) entityType)));
    }

    [Obsolete("Use CreateODataDeltaResourceSetWriter.", false)]
    public ODataDeltaWriter CreateODataDeltaWriter(
      IEdmEntitySetBase entitySet,
      IEdmEntityType entityType)
    {
      this.VerifyCanCreateODataDeltaWriter();
      return this.WriteToOutput<ODataDeltaWriter>(ODataPayloadKind.ResourceSet, (Func<ODataOutputContext, ODataDeltaWriter>) (context => context.CreateODataDeltaWriter(entitySet, entityType)));
    }

    [Obsolete("Use CreateODataDeltaResourceSetWriterAsync.", false)]
    public Task<ODataDeltaWriter> CreateODataDeltaWriterAsync(
      IEdmEntitySetBase entitySet,
      IEdmEntityType entityType)
    {
      this.VerifyCanCreateODataResourceSetWriter();
      return this.WriteToOutputAsync<ODataDeltaWriter>(ODataPayloadKind.ResourceSet, (Func<ODataOutputContext, Task<ODataDeltaWriter>>) (context => context.CreateODataDeltaWriterAsync(entitySet, entityType)));
    }

    public ODataWriter CreateODataResourceWriter() => this.CreateODataResourceWriter((IEdmNavigationSource) null, (IEdmStructuredType) null);

    public ODataWriter CreateODataResourceWriter(IEdmNavigationSource navigationSource) => this.CreateODataResourceWriter(navigationSource, (IEdmStructuredType) null);

    public ODataWriter CreateODataResourceWriter(
      IEdmNavigationSource navigationSource,
      IEdmStructuredType resourceType)
    {
      this.VerifyCanCreateODataResourceWriter();
      return this.WriteToOutput<ODataWriter>(ODataPayloadKind.Resource, (Func<ODataOutputContext, ODataWriter>) (context => context.CreateODataResourceWriter(navigationSource, resourceType)));
    }

    public Task<ODataWriter> CreateODataResourceWriterAsync() => this.CreateODataResourceWriterAsync((IEdmNavigationSource) null, (IEdmStructuredType) null);

    public Task<ODataWriter> CreateODataResourceWriterAsync(IEdmNavigationSource navigationSource) => this.CreateODataResourceWriterAsync(navigationSource, (IEdmStructuredType) null);

    public Task<ODataWriter> CreateODataResourceWriterAsync(
      IEdmNavigationSource navigationSource,
      IEdmStructuredType resourceType)
    {
      this.VerifyCanCreateODataResourceWriter();
      return this.WriteToOutputAsync<ODataWriter>(ODataPayloadKind.Resource, (Func<ODataOutputContext, Task<ODataWriter>>) (context => context.CreateODataResourceWriterAsync(navigationSource, resourceType)));
    }

    public ODataWriter CreateODataUriParameterResourceWriter(
      IEdmNavigationSource navigationSource,
      IEdmStructuredType resourceType)
    {
      this.VerifyCanCreateODataResourceWriter();
      return this.WriteToOutput<ODataWriter>(ODataPayloadKind.Resource, (Func<ODataOutputContext, ODataWriter>) (context => context.CreateODataUriParameterResourceWriter(navigationSource, resourceType)));
    }

    public Task<ODataWriter> CreateODataUriParameterResourceWriterAsync(
      IEdmNavigationSource navigationSource,
      IEdmStructuredType resourceType)
    {
      this.VerifyCanCreateODataResourceWriter();
      return this.WriteToOutputAsync<ODataWriter>(ODataPayloadKind.Resource, (Func<ODataOutputContext, Task<ODataWriter>>) (context => context.CreateODataUriParameterResourceWriterAsync(navigationSource, resourceType)));
    }

    public ODataWriter CreateODataUriParameterResourceSetWriter(
      IEdmEntitySetBase entitySetBase,
      IEdmStructuredType resourceType)
    {
      this.VerifyCanCreateODataResourceSetWriter();
      return this.WriteToOutput<ODataWriter>(ODataPayloadKind.ResourceSet, (Func<ODataOutputContext, ODataWriter>) (context => context.CreateODataUriParameterResourceSetWriter(entitySetBase, resourceType)));
    }

    public Task<ODataWriter> CreateODataUriParameterResourceSetWriterAsync(
      IEdmEntitySetBase entitySetBase,
      IEdmStructuredType resourceType)
    {
      this.VerifyCanCreateODataResourceSetWriter();
      return this.WriteToOutputAsync<ODataWriter>(ODataPayloadKind.ResourceSet, (Func<ODataOutputContext, Task<ODataWriter>>) (context => context.CreateODataUriParameterResourceSetWriterAsync(entitySetBase, resourceType)));
    }

    public ODataCollectionWriter CreateODataCollectionWriter() => this.CreateODataCollectionWriter((IEdmTypeReference) null);

    public ODataCollectionWriter CreateODataCollectionWriter(IEdmTypeReference itemTypeReference)
    {
      this.VerifyCanCreateODataCollectionWriter(itemTypeReference);
      return this.WriteToOutput<ODataCollectionWriter>(ODataPayloadKind.Collection, (Func<ODataOutputContext, ODataCollectionWriter>) (context => context.CreateODataCollectionWriter(itemTypeReference)));
    }

    public Task<ODataCollectionWriter> CreateODataCollectionWriterAsync() => this.CreateODataCollectionWriterAsync((IEdmTypeReference) null);

    public Task<ODataCollectionWriter> CreateODataCollectionWriterAsync(
      IEdmTypeReference itemTypeReference)
    {
      this.VerifyCanCreateODataCollectionWriter(itemTypeReference);
      return this.WriteToOutputAsync<ODataCollectionWriter>(ODataPayloadKind.Collection, (Func<ODataOutputContext, Task<ODataCollectionWriter>>) (context => context.CreateODataCollectionWriterAsync(itemTypeReference)));
    }

    public ODataBatchWriter CreateODataBatchWriter()
    {
      this.VerifyCanCreateODataBatchWriter();
      return this.WriteToOutput<ODataBatchWriter>(ODataPayloadKind.Batch, (Func<ODataOutputContext, ODataBatchWriter>) (context => context.CreateODataBatchWriter()));
    }

    public Task<ODataBatchWriter> CreateODataBatchWriterAsync()
    {
      this.VerifyCanCreateODataBatchWriter();
      return this.WriteToOutputAsync<ODataBatchWriter>(ODataPayloadKind.Batch, (Func<ODataOutputContext, Task<ODataBatchWriter>>) (context => context.CreateODataBatchWriterAsync()));
    }

    public ODataParameterWriter CreateODataParameterWriter(IEdmOperation operation)
    {
      this.VerifyCanCreateODataParameterWriter(operation);
      return this.WriteToOutput<ODataParameterWriter>(ODataPayloadKind.Parameter, (Func<ODataOutputContext, ODataParameterWriter>) (context => context.CreateODataParameterWriter(operation)));
    }

    public Task<ODataParameterWriter> CreateODataParameterWriterAsync(IEdmOperation operation)
    {
      this.VerifyCanCreateODataParameterWriter(operation);
      return this.WriteToOutputAsync<ODataParameterWriter>(ODataPayloadKind.Parameter, (Func<ODataOutputContext, Task<ODataParameterWriter>>) (context => context.CreateODataParameterWriterAsync(operation)));
    }

    public void WriteServiceDocument(ODataServiceDocument serviceDocument)
    {
      this.VerifyCanWriteServiceDocument(serviceDocument);
      this.WriteToOutput(ODataPayloadKind.ServiceDocument, (Action<ODataOutputContext>) (context => context.WriteServiceDocument(serviceDocument)));
    }

    public Task WriteServiceDocumentAsync(ODataServiceDocument serviceDocument)
    {
      this.VerifyCanWriteServiceDocument(serviceDocument);
      return this.WriteToOutputAsync(ODataPayloadKind.ServiceDocument, (Func<ODataOutputContext, Task>) (context => context.WriteServiceDocumentAsync(serviceDocument)));
    }

    public void WriteProperty(ODataProperty property)
    {
      this.VerifyCanWriteProperty(property);
      this.WriteToOutput(ODataPayloadKind.Property, (Action<ODataOutputContext>) (context => context.WriteProperty(property)));
    }

    public Task WritePropertyAsync(ODataProperty property)
    {
      this.VerifyCanWriteProperty(property);
      return this.WriteToOutputAsync(ODataPayloadKind.Property, (Func<ODataOutputContext, Task>) (context => context.WritePropertyAsync(property)));
    }

    public void WriteError(ODataError error, bool includeDebugInformation)
    {
      if (this.outputContext == null)
      {
        this.VerifyCanWriteTopLevelError(error);
        this.WriteToOutput(ODataPayloadKind.Error, (Action<ODataOutputContext>) (context => context.WriteError(error, includeDebugInformation)));
      }
      else
      {
        this.VerifyCanWriteInStreamError(error);
        this.outputContext.WriteInStreamError(error, includeDebugInformation);
      }
    }

    public Task WriteErrorAsync(ODataError error, bool includeDebugInformation)
    {
      if (this.outputContext == null)
      {
        this.VerifyCanWriteTopLevelError(error);
        return this.WriteToOutputAsync(ODataPayloadKind.Error, (Func<ODataOutputContext, Task>) (context => context.WriteErrorAsync(error, includeDebugInformation)));
      }
      this.VerifyCanWriteInStreamError(error);
      return this.outputContext.WriteInStreamErrorAsync(error, includeDebugInformation);
    }

    public void WriteEntityReferenceLinks(ODataEntityReferenceLinks links)
    {
      this.VerifyCanWriteEntityReferenceLinks(links);
      this.WriteToOutput(ODataPayloadKind.EntityReferenceLinks, (Action<ODataOutputContext>) (context => context.WriteEntityReferenceLinks(links)));
    }

    public Task WriteEntityReferenceLinksAsync(ODataEntityReferenceLinks links)
    {
      this.VerifyCanWriteEntityReferenceLinks(links);
      return this.WriteToOutputAsync(ODataPayloadKind.EntityReferenceLinks, (Func<ODataOutputContext, Task>) (context => context.WriteEntityReferenceLinksAsync(links)));
    }

    public void WriteEntityReferenceLink(ODataEntityReferenceLink link)
    {
      this.VerifyCanWriteEntityReferenceLink(link);
      this.WriteToOutput(ODataPayloadKind.EntityReferenceLink, (Action<ODataOutputContext>) (context => context.WriteEntityReferenceLink(link)));
    }

    public Task WriteEntityReferenceLinkAsync(ODataEntityReferenceLink link)
    {
      this.VerifyCanWriteEntityReferenceLink(link);
      return this.WriteToOutputAsync(ODataPayloadKind.EntityReferenceLink, (Func<ODataOutputContext, Task>) (context => context.WriteEntityReferenceLinkAsync(link)));
    }

    public void WriteValue(object value) => this.WriteToOutput(this.VerifyCanWriteValue(value), (Action<ODataOutputContext>) (context => context.WriteValue(value)));

    public Task WriteValueAsync(object value) => this.WriteToOutputAsync(this.VerifyCanWriteValue(value), (Func<ODataOutputContext, Task>) (context => context.WriteValueAsync(value)));

    public void WriteMetadataDocument()
    {
      this.VerifyCanWriteMetadataDocument();
      this.WriteToOutput(ODataPayloadKind.MetadataDocument, (Action<ODataOutputContext>) (context => context.WriteMetadataDocument()));
    }

    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize((object) this);
    }

    internal ODataFormat SetHeaders(ODataPayloadKind payloadKind)
    {
      this.writerPayloadKind = payloadKind;
      this.EnsureODataVersion();
      this.EnsureODataFormatAndContentType();
      return this.format;
    }

    private static IServiceProvider GetContainer<T>(T message) where T : class => message is IContainerProvider containerProvider ? containerProvider.Container : (IServiceProvider) null;

    private static IEdmModel GetModel(IServiceProvider container) => container != null ? container.GetRequiredService<IEdmModel>() : (IEdmModel) EdmCoreModel.Instance;

    private void SetOrVerifyHeaders(ODataPayloadKind payloadKind)
    {
      this.VerifyPayloadKind(payloadKind);
      if (this.writerPayloadKind != ODataPayloadKind.Unsupported)
        return;
      this.SetHeaders(payloadKind);
    }

    private void EnsureODataVersion()
    {
      if (!this.settings.Version.HasValue)
      {
        this.settings.Version = new ODataVersion?(ODataUtilsInternal.GetODataVersion(this.message, ODataVersion.V4));
        if (!string.IsNullOrEmpty(this.message.GetHeader("OData-Version")))
          return;
        ODataUtilsInternal.SetODataVersion(this.message, this.settings);
      }
      else
        ODataUtilsInternal.SetODataVersion(this.message, this.settings);
    }

    private void EnsureODataFormatAndContentType()
    {
      string str1 = (string) null;
      if (!this.settings.UseFormat.HasValue)
      {
        string header = this.message.GetHeader("Content-Type");
        str1 = header == null ? (string) null : header.Trim();
      }
      if (!string.IsNullOrEmpty(str1))
      {
        this.format = MediaTypeUtils.GetFormatFromContentType(str1, new ODataPayloadKind[1]
        {
          this.writerPayloadKind
        }, this.mediaTypeResolver, out this.mediaType, out this.encoding, out ODataPayloadKind _);
        if (!this.settings.HasJsonPaddingFunction())
          return;
        this.message.SetHeader("Content-Type", MediaTypeUtils.AlterContentTypeForJsonPadding(str1));
      }
      else
      {
        this.format = MediaTypeUtils.GetContentTypeFromSettings(this.settings, this.writerPayloadKind, this.mediaTypeResolver, out this.mediaType, out this.encoding);
        IEnumerable<KeyValuePair<string, string>> mediaTypeParameters;
        string str2 = this.format.GetContentType(this.mediaType, this.encoding, this.writingResponse, out mediaTypeParameters);
        if (this.mediaType.Parameters != mediaTypeParameters)
          this.mediaType = new ODataMediaType(this.mediaType.Type, this.mediaType.SubType, mediaTypeParameters);
        if (this.settings.HasJsonPaddingFunction())
          str2 = MediaTypeUtils.AlterContentTypeForJsonPadding(str2);
        this.message.SetHeader("Content-Type", str2);
      }
    }

    private void VerifyCanCreateODataAsyncWriter()
    {
      if (!this.writingResponse)
        throw new ODataException(Strings.ODataMessageWriter_AsyncInRequest);
      this.VerifyWriterNotDisposedAndNotUsed();
    }

    private void VerifyCanCreateODataResourceSetWriter() => this.VerifyWriterNotDisposedAndNotUsed();

    private void VerifyCanCreateODataDeltaWriter()
    {
      if (!this.writingResponse)
        throw new ODataException(Strings.ODataMessageWriter_DeltaInRequest);
      this.VerifyWriterNotDisposedAndNotUsed();
    }

    private void VerifyCanCreateODataResourceWriter() => this.VerifyWriterNotDisposedAndNotUsed();

    [SuppressMessage("Microsoft.Naming", "CA2204:LiteralsShouldBeSpelledCorrectly", Justification = "Names are correct. String can't be localized after string freeze.")]
    private void VerifyCanCreateODataCollectionWriter(IEdmTypeReference itemTypeReference)
    {
      if (itemTypeReference != null && !itemTypeReference.IsPrimitive() && !itemTypeReference.IsComplex() && !itemTypeReference.IsEnum() && !itemTypeReference.IsTypeDefinition())
        throw new ODataException(Strings.ODataMessageWriter_NonCollectionType((object) itemTypeReference.FullName()));
      this.VerifyWriterNotDisposedAndNotUsed();
    }

    private void VerifyCanCreateODataBatchWriter() => this.VerifyWriterNotDisposedAndNotUsed();

    private void VerifyCanCreateODataParameterWriter(IEdmOperation operation)
    {
      if (this.writingResponse)
        throw new ODataException(Strings.ODataParameterWriter_CannotCreateParameterWriterOnResponseMessage);
      if (operation != null && !this.model.IsUserModel())
        throw new ODataException(Strings.ODataMessageWriter_CannotSpecifyOperationWithoutModel);
      this.VerifyWriterNotDisposedAndNotUsed();
    }

    private void VerifyCanWriteServiceDocument(ODataServiceDocument serviceDocument)
    {
      ExceptionUtils.CheckArgumentNotNull<ODataServiceDocument>(serviceDocument, nameof (serviceDocument));
      if (!this.writingResponse)
        throw new ODataException(Strings.ODataMessageWriter_ServiceDocumentInRequest);
      this.VerifyWriterNotDisposedAndNotUsed();
    }

    private void VerifyCanWriteProperty(ODataProperty property)
    {
      ExceptionUtils.CheckArgumentNotNull<ODataProperty>(property, nameof (property));
      if (property.Value is ODataStreamReferenceValue)
        throw new ODataException(Strings.ODataMessageWriter_CannotWriteStreamPropertyAsTopLevelProperty((object) property.Name));
      this.VerifyWriterNotDisposedAndNotUsed();
    }

    private void VerifyCanWriteTopLevelError(ODataError error)
    {
      ExceptionUtils.CheckArgumentNotNull<ODataError>(error, nameof (error));
      if (!this.writingResponse)
        throw new ODataException(Strings.ODataMessageWriter_ErrorPayloadInRequest);
      this.VerifyWriterNotDisposedAndNotUsed();
      this.writeErrorCalled = true;
    }

    private void VerifyCanWriteInStreamError(ODataError error)
    {
      ExceptionUtils.CheckArgumentNotNull<ODataError>(error, nameof (error));
      this.VerifyNotDisposed();
      if (!this.writingResponse)
        throw new ODataException(Strings.ODataMessageWriter_ErrorPayloadInRequest);
      this.writeErrorCalled = !this.writeErrorCalled ? true : throw new ODataException(Strings.ODataMessageWriter_WriteErrorAlreadyCalled);
      this.writeMethodCalled = true;
    }

    private void VerifyCanWriteEntityReferenceLinks(ODataEntityReferenceLinks links)
    {
      ExceptionUtils.CheckArgumentNotNull<ODataEntityReferenceLinks>(links, "ref");
      if (!this.writingResponse)
        throw new ODataException(Strings.ODataMessageWriter_EntityReferenceLinksInRequestNotAllowed);
      this.VerifyWriterNotDisposedAndNotUsed();
    }

    private void VerifyCanWriteEntityReferenceLink(ODataEntityReferenceLink link)
    {
      ExceptionUtils.CheckArgumentNotNull<ODataEntityReferenceLink>(link, nameof (link));
      this.VerifyWriterNotDisposedAndNotUsed();
    }

    private ODataPayloadKind VerifyCanWriteValue(object value)
    {
      if (value == null)
        throw new ODataException(Strings.ODataMessageWriter_CannotWriteNullInRawFormat);
      this.VerifyWriterNotDisposedAndNotUsed();
      return !(value is byte[]) ? ODataPayloadKind.Value : ODataPayloadKind.BinaryValue;
    }

    private void VerifyCanWriteMetadataDocument()
    {
      if (!this.writingResponse)
        throw new ODataException(Strings.ODataMessageWriter_MetadataDocumentInRequest);
      if (!this.model.IsUserModel())
        throw new ODataException(Strings.ODataMessageWriter_CannotWriteMetadataWithoutModel);
      this.VerifyWriterNotDisposedAndNotUsed();
    }

    private void VerifyWriterNotDisposedAndNotUsed()
    {
      this.VerifyNotDisposed();
      this.writeMethodCalled = !this.writeMethodCalled ? true : throw new ODataException(Strings.ODataMessageWriter_WriterAlreadyUsed);
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
        if (this.outputContext == null)
          return;
        this.outputContext.Dispose();
      }
      finally
      {
        this.outputContext = (ODataOutputContext) null;
      }
    }

    private void VerifyPayloadKind(ODataPayloadKind payloadKindToWrite)
    {
      if (this.writerPayloadKind != ODataPayloadKind.Unsupported && this.writerPayloadKind != payloadKindToWrite)
        throw new ODataException(Strings.ODataMessageWriter_IncompatiblePayloadKinds((object) this.writerPayloadKind, (object) payloadKindToWrite));
    }

    private ODataMessageInfo GetOrCreateMessageInfo(Stream messageStream, bool isAsync)
    {
      if (this.messageInfo == null)
      {
        this.messageInfo = this.container != null ? this.container.GetRequiredService<ODataMessageInfo>() : new ODataMessageInfo();
        this.messageInfo.Encoding = this.encoding;
        this.messageInfo.IsResponse = this.writingResponse;
        this.messageInfo.IsAsync = isAsync;
        this.messageInfo.MediaType = this.mediaType;
        this.messageInfo.Model = this.model;
        this.messageInfo.PayloadUriConverter = this.payloadUriConverter;
        this.messageInfo.Container = this.container;
        this.messageInfo.MessageStream = messageStream;
      }
      return this.messageInfo;
    }

    private void WriteToOutput(ODataPayloadKind payloadKind, Action<ODataOutputContext> writeAction)
    {
      this.SetOrVerifyHeaders(payloadKind);
      this.outputContext = this.format.CreateOutputContext(this.GetOrCreateMessageInfo(this.message.GetStream(), false), this.settings);
      writeAction(this.outputContext);
    }

    private TResult WriteToOutput<TResult>(
      ODataPayloadKind payloadKind,
      Func<ODataOutputContext, TResult> writeFunc)
    {
      this.SetOrVerifyHeaders(payloadKind);
      this.outputContext = this.format.CreateOutputContext(this.GetOrCreateMessageInfo(this.message.GetStream(), false), this.settings);
      return writeFunc(this.outputContext);
    }

    private Task WriteToOutputAsync(
      ODataPayloadKind payloadKind,
      Func<ODataOutputContext, Task> writeAsyncAction)
    {
      this.SetOrVerifyHeaders(payloadKind);
      return this.message.GetStreamAsync().FollowOnSuccessWithTask<Stream, ODataOutputContext>((Func<Task<Stream>, Task<ODataOutputContext>>) (streamTask => this.format.CreateOutputContextAsync(this.GetOrCreateMessageInfo(streamTask.Result, true), this.settings))).FollowOnSuccessWithTask<ODataOutputContext>((Func<Task<ODataOutputContext>, Task>) (createOutputContextTask =>
      {
        this.outputContext = createOutputContextTask.Result;
        return writeAsyncAction(this.outputContext);
      }));
    }

    private Task<TResult> WriteToOutputAsync<TResult>(
      ODataPayloadKind payloadKind,
      Func<ODataOutputContext, Task<TResult>> writeFunc)
    {
      this.SetOrVerifyHeaders(payloadKind);
      return this.message.GetStreamAsync().FollowOnSuccessWithTask<Stream, ODataOutputContext>((Func<Task<Stream>, Task<ODataOutputContext>>) (streamTask => this.format.CreateOutputContextAsync(this.GetOrCreateMessageInfo(streamTask.Result, true), this.settings))).FollowOnSuccessWithTask<ODataOutputContext, TResult>((Func<Task<ODataOutputContext>, Task<TResult>>) (createOutputContextTask =>
      {
        this.outputContext = createOutputContextTask.Result;
        return writeFunc(this.outputContext);
      }));
    }
  }
}
