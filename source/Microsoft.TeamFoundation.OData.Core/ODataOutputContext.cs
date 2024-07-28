// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.ODataOutputContext
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Edm;
using Microsoft.OData.Metadata;
using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace Microsoft.OData
{
  public abstract class ODataOutputContext : IDisposable
  {
    private readonly ODataFormat format;
    private readonly ODataMessageWriterSettings messageWriterSettings;
    private readonly bool writingResponse;
    private readonly bool synchronous;
    private readonly IEdmModel model;
    private readonly IODataPayloadUriConverter payloadUriConverter;
    private readonly IServiceProvider container;
    private readonly EdmTypeResolver edmTypeResolver;
    private readonly ODataPayloadValueConverter payloadValueConverter;
    private readonly IWriterValidator writerValidator;
    private readonly ODataSimplifiedOptions odataSimplifiedOptions;

    protected ODataOutputContext(
      ODataFormat format,
      ODataMessageInfo messageInfo,
      ODataMessageWriterSettings messageWriterSettings)
    {
      ExceptionUtils.CheckArgumentNotNull<ODataFormat>(format, nameof (format));
      ExceptionUtils.CheckArgumentNotNull<ODataMessageWriterSettings>(messageWriterSettings, nameof (messageWriterSettings));
      this.format = format;
      this.messageWriterSettings = messageWriterSettings;
      this.writingResponse = messageInfo.IsResponse;
      this.synchronous = !messageInfo.IsAsync;
      this.model = messageInfo.Model ?? (IEdmModel) EdmCoreModel.Instance;
      this.payloadUriConverter = messageInfo.PayloadUriConverter;
      this.container = messageInfo.Container;
      this.edmTypeResolver = (EdmTypeResolver) EdmTypeWriterResolver.Instance;
      this.payloadValueConverter = ODataPayloadValueConverter.GetPayloadValueConverter(this.container);
      this.writerValidator = messageWriterSettings.Validator;
      this.odataSimplifiedOptions = ODataSimplifiedOptions.GetODataSimplifiedOptions(this.container, messageWriterSettings.Version);
    }

    public ODataMessageWriterSettings MessageWriterSettings => this.messageWriterSettings;

    public bool WritingResponse => this.writingResponse;

    public bool Synchronous => this.synchronous;

    public IEdmModel Model => this.model;

    public IODataPayloadUriConverter PayloadUriConverter => this.payloadUriConverter;

    internal IServiceProvider Container => this.container;

    internal EdmTypeResolver EdmTypeResolver => this.edmTypeResolver;

    internal ODataPayloadValueConverter PayloadValueConverter => this.payloadValueConverter;

    internal IWriterValidator WriterValidator => this.writerValidator;

    internal ODataSimplifiedOptions ODataSimplifiedOptions => this.odataSimplifiedOptions;

    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize((object) this);
    }

    public virtual ODataWriter CreateODataResourceSetWriter(
      IEdmEntitySetBase entitySet,
      IEdmStructuredType resourceType)
    {
      throw this.CreatePayloadKindNotSupportedException(ODataPayloadKind.ResourceSet);
    }

    public virtual Task<ODataWriter> CreateODataResourceSetWriterAsync(
      IEdmEntitySetBase entitySet,
      IEdmStructuredType entityType)
    {
      throw this.CreatePayloadKindNotSupportedException(ODataPayloadKind.ResourceSet);
    }

    public virtual ODataWriter CreateODataDeltaResourceSetWriter(
      IEdmEntitySetBase entitySet,
      IEdmStructuredType resourceType)
    {
      throw this.CreatePayloadKindNotSupportedException(ODataPayloadKind.ResourceSet);
    }

    public virtual Task<ODataWriter> CreateODataDeltaResourceSetWriterAsync(
      IEdmEntitySetBase entitySet,
      IEdmStructuredType entityType)
    {
      throw this.CreatePayloadKindNotSupportedException(ODataPayloadKind.ResourceSet);
    }

    public virtual ODataWriter CreateODataResourceWriter(
      IEdmNavigationSource navigationSource,
      IEdmStructuredType resourceType)
    {
      throw this.CreatePayloadKindNotSupportedException(ODataPayloadKind.Resource);
    }

    public virtual Task<ODataWriter> CreateODataResourceWriterAsync(
      IEdmNavigationSource navigationSource,
      IEdmStructuredType resourceType)
    {
      throw this.CreatePayloadKindNotSupportedException(ODataPayloadKind.Resource);
    }

    public virtual ODataCollectionWriter CreateODataCollectionWriter(
      IEdmTypeReference itemTypeReference)
    {
      throw this.CreatePayloadKindNotSupportedException(ODataPayloadKind.Collection);
    }

    public virtual Task<ODataCollectionWriter> CreateODataCollectionWriterAsync(
      IEdmTypeReference itemTypeReference)
    {
      throw this.CreatePayloadKindNotSupportedException(ODataPayloadKind.Collection);
    }

    public virtual ODataWriter CreateODataUriParameterResourceWriter(
      IEdmNavigationSource navigationSource,
      IEdmStructuredType resourceType)
    {
      throw this.CreatePayloadKindNotSupportedException(ODataPayloadKind.Resource);
    }

    public virtual Task<ODataWriter> CreateODataUriParameterResourceWriterAsync(
      IEdmNavigationSource navigationSource,
      IEdmStructuredType resourceType)
    {
      throw this.CreatePayloadKindNotSupportedException(ODataPayloadKind.Resource);
    }

    public virtual ODataWriter CreateODataUriParameterResourceSetWriter(
      IEdmEntitySetBase entitySet,
      IEdmStructuredType resourceType)
    {
      throw this.CreatePayloadKindNotSupportedException(ODataPayloadKind.ResourceSet);
    }

    public virtual Task<ODataWriter> CreateODataUriParameterResourceSetWriterAsync(
      IEdmEntitySetBase entitySet,
      IEdmStructuredType resourceType)
    {
      throw this.CreatePayloadKindNotSupportedException(ODataPayloadKind.ResourceSet);
    }

    public virtual ODataParameterWriter CreateODataParameterWriter(IEdmOperation operation) => throw this.CreatePayloadKindNotSupportedException(ODataPayloadKind.Error);

    public virtual Task<ODataParameterWriter> CreateODataParameterWriterAsync(
      IEdmOperation operation)
    {
      throw this.CreatePayloadKindNotSupportedException(ODataPayloadKind.Error);
    }

    public virtual void WriteProperty(ODataProperty odataProperty) => throw this.CreatePayloadKindNotSupportedException(ODataPayloadKind.Property);

    public virtual Task WritePropertyAsync(ODataProperty odataProperty) => throw this.CreatePayloadKindNotSupportedException(ODataPayloadKind.Property);

    public virtual void WriteError(ODataError odataError, bool includeDebugInformation) => throw this.CreatePayloadKindNotSupportedException(ODataPayloadKind.Error);

    public virtual Task WriteErrorAsync(ODataError odataError, bool includeDebugInformation) => throw this.CreatePayloadKindNotSupportedException(ODataPayloadKind.Error);

    internal bool ShouldOmitNullValues() => this.WritingResponse && this.MessageWriterSettings.OmitNullValues;

    internal virtual void WriteInStreamError(ODataError error, bool includeDebugInformation) => throw this.CreatePayloadKindNotSupportedException(ODataPayloadKind.Error);

    internal virtual Task WriteInStreamErrorAsync(ODataError error, bool includeDebugInformation) => throw this.CreatePayloadKindNotSupportedException(ODataPayloadKind.Error);

    internal virtual ODataAsynchronousWriter CreateODataAsynchronousWriter() => throw this.CreatePayloadKindNotSupportedException(ODataPayloadKind.Asynchronous);

    internal virtual Task<ODataAsynchronousWriter> CreateODataAsynchronousWriterAsync() => throw this.CreatePayloadKindNotSupportedException(ODataPayloadKind.Asynchronous);

    internal virtual ODataDeltaWriter CreateODataDeltaWriter(
      IEdmEntitySetBase entitySet,
      IEdmEntityType entityType)
    {
      throw this.CreatePayloadKindNotSupportedException(ODataPayloadKind.ResourceSet);
    }

    internal virtual Task<ODataDeltaWriter> CreateODataDeltaWriterAsync(
      IEdmEntitySetBase entitySet,
      IEdmEntityType entityType)
    {
      throw this.CreatePayloadKindNotSupportedException(ODataPayloadKind.ResourceSet);
    }

    internal virtual ODataBatchWriter CreateODataBatchWriter() => throw this.CreatePayloadKindNotSupportedException(ODataPayloadKind.Batch);

    internal virtual Task<ODataBatchWriter> CreateODataBatchWriterAsync() => throw this.CreatePayloadKindNotSupportedException(ODataPayloadKind.Batch);

    internal virtual void WriteServiceDocument(ODataServiceDocument serviceDocument) => throw this.CreatePayloadKindNotSupportedException(ODataPayloadKind.ServiceDocument);

    internal virtual Task WriteServiceDocumentAsync(ODataServiceDocument serviceDocument) => throw this.CreatePayloadKindNotSupportedException(ODataPayloadKind.ServiceDocument);

    internal virtual void WriteEntityReferenceLinks(ODataEntityReferenceLinks links) => throw this.CreatePayloadKindNotSupportedException(ODataPayloadKind.EntityReferenceLinks);

    internal virtual Task WriteEntityReferenceLinksAsync(ODataEntityReferenceLinks links) => throw this.CreatePayloadKindNotSupportedException(ODataPayloadKind.EntityReferenceLinks);

    internal virtual void WriteEntityReferenceLink(ODataEntityReferenceLink link) => throw this.CreatePayloadKindNotSupportedException(ODataPayloadKind.EntityReferenceLink);

    internal virtual Task WriteEntityReferenceLinkAsync(ODataEntityReferenceLink link) => throw this.CreatePayloadKindNotSupportedException(ODataPayloadKind.EntityReferenceLink);

    internal virtual void WriteValue(object value) => throw this.CreatePayloadKindNotSupportedException(ODataPayloadKind.Value);

    internal virtual Task WriteValueAsync(object value) => throw this.CreatePayloadKindNotSupportedException(ODataPayloadKind.Value);

    internal virtual void WriteMetadataDocument() => throw this.CreatePayloadKindNotSupportedException(ODataPayloadKind.MetadataDocument);

    [Conditional("DEBUG")]
    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Needs to access this in debug only.")]
    internal void AssertSynchronous()
    {
    }

    [Conditional("DEBUG")]
    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Needs to access this in debug only.")]
    internal void AssertAsynchronous()
    {
    }

    protected virtual void Dispose(bool disposing)
    {
    }

    private ODataException CreatePayloadKindNotSupportedException(ODataPayloadKind payloadKind) => new ODataException(Strings.ODataOutputContext_UnsupportedPayloadKindForFormat((object) this.format.ToString(), (object) payloadKind.ToString()));
  }
}
