// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.ODataInputContext
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Edm;
using Microsoft.OData.Metadata;
using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using System.Xml;

namespace Microsoft.OData
{
  public abstract class ODataInputContext : IDisposable
  {
    private readonly ODataFormat format;
    private readonly ODataMessageReaderSettings messageReaderSettings;
    private readonly bool readingResponse;
    private readonly bool synchronous;
    private readonly IODataPayloadUriConverter payloadUriConverter;
    private readonly IServiceProvider container;
    private readonly IEdmModel model;
    private readonly EdmTypeResolver edmTypeResolver;
    private readonly ODataPayloadValueConverter payloadValueConverter;
    private readonly ODataSimplifiedOptions odataSimplifiedOptions;
    private bool disposed;

    protected ODataInputContext(
      ODataFormat format,
      ODataMessageInfo messageInfo,
      ODataMessageReaderSettings messageReaderSettings)
    {
      ExceptionUtils.CheckArgumentNotNull<ODataFormat>(format, nameof (format));
      ExceptionUtils.CheckArgumentNotNull<ODataMessageInfo>(messageInfo, nameof (messageInfo));
      ExceptionUtils.CheckArgumentNotNull<ODataMessageReaderSettings>(messageReaderSettings, nameof (messageReaderSettings));
      this.format = format;
      this.messageReaderSettings = messageReaderSettings;
      this.readingResponse = messageInfo.IsResponse;
      this.synchronous = !messageInfo.IsAsync;
      this.model = messageInfo.Model ?? (IEdmModel) EdmCoreModel.Instance;
      this.payloadUriConverter = messageInfo.PayloadUriConverter;
      this.container = messageInfo.Container;
      this.edmTypeResolver = (EdmTypeResolver) new EdmTypeReaderResolver(this.Model, this.MessageReaderSettings.ClientCustomTypeResolver);
      this.payloadValueConverter = ODataPayloadValueConverter.GetPayloadValueConverter(this.container);
      this.odataSimplifiedOptions = ODataSimplifiedOptions.GetODataSimplifiedOptions(this.container, messageReaderSettings.Version);
    }

    public ODataMessageReaderSettings MessageReaderSettings => this.messageReaderSettings;

    public bool ReadingResponse => this.readingResponse;

    public bool Synchronous => this.synchronous;

    public IEdmModel Model => this.model;

    public IODataPayloadUriConverter PayloadUriConverter => this.payloadUriConverter;

    internal IServiceProvider Container => this.container;

    internal EdmTypeResolver EdmTypeResolver => this.edmTypeResolver;

    internal ODataPayloadValueConverter PayloadValueConverter => this.payloadValueConverter;

    internal ODataSimplifiedOptions ODataSimplifiedOptions => this.odataSimplifiedOptions;

    public void Dispose()
    {
      if (this.disposed)
        return;
      this.Dispose(true);
      GC.SuppressFinalize((object) this);
      this.disposed = true;
    }

    public virtual ODataReader CreateResourceSetReader(
      IEdmEntitySetBase entitySet,
      IEdmStructuredType expectedResourceType)
    {
      throw this.CreatePayloadKindNotSupportedException(ODataPayloadKind.ResourceSet);
    }

    public virtual Task<ODataReader> CreateResourceSetReaderAsync(
      IEdmEntitySetBase entitySet,
      IEdmStructuredType expectedResourceType)
    {
      throw this.CreatePayloadKindNotSupportedException(ODataPayloadKind.ResourceSet);
    }

    public virtual ODataReader CreateDeltaResourceSetReader(
      IEdmEntitySetBase entitySet,
      IEdmStructuredType expectedResourceType)
    {
      throw this.CreatePayloadKindNotSupportedException(ODataPayloadKind.ResourceSet);
    }

    public virtual Task<ODataReader> CreateDeltaResourceSetReaderAsync(
      IEdmEntitySetBase entitySet,
      IEdmStructuredType expectedResourceType)
    {
      throw this.CreatePayloadKindNotSupportedException(ODataPayloadKind.ResourceSet);
    }

    public virtual ODataReader CreateResourceReader(
      IEdmNavigationSource navigationSource,
      IEdmStructuredType expectedResourceType)
    {
      throw this.CreatePayloadKindNotSupportedException(ODataPayloadKind.Resource);
    }

    public virtual Task<ODataReader> CreateResourceReaderAsync(
      IEdmNavigationSource navigationSource,
      IEdmStructuredType expectedResourceType)
    {
      throw this.CreatePayloadKindNotSupportedException(ODataPayloadKind.Resource);
    }

    public virtual ODataCollectionReader CreateCollectionReader(
      IEdmTypeReference expectedItemTypeReference)
    {
      throw this.CreatePayloadKindNotSupportedException(ODataPayloadKind.Collection);
    }

    public virtual Task<ODataCollectionReader> CreateCollectionReaderAsync(
      IEdmTypeReference expectedItemTypeReference)
    {
      throw this.CreatePayloadKindNotSupportedException(ODataPayloadKind.Collection);
    }

    public virtual ODataProperty ReadProperty(
      IEdmStructuralProperty edmStructuralProperty,
      IEdmTypeReference expectedPropertyTypeReference)
    {
      throw this.CreatePayloadKindNotSupportedException(ODataPayloadKind.Property);
    }

    public virtual Task<ODataProperty> ReadPropertyAsync(
      IEdmStructuralProperty edmStructuralProperty,
      IEdmTypeReference expectedPropertyTypeReference)
    {
      throw this.CreatePayloadKindNotSupportedException(ODataPayloadKind.Property);
    }

    public virtual ODataError ReadError() => throw this.CreatePayloadKindNotSupportedException(ODataPayloadKind.Error);

    public virtual Task<ODataError> ReadErrorAsync() => throw this.CreatePayloadKindNotSupportedException(ODataPayloadKind.Error);

    public virtual ODataReader CreateUriParameterResourceReader(
      IEdmNavigationSource navigationSource,
      IEdmStructuredType expectedResourceType)
    {
      throw this.CreatePayloadKindNotSupportedException(ODataPayloadKind.Parameter);
    }

    public virtual Task<ODataReader> CreateUriParameterResourceReaderAsync(
      IEdmNavigationSource navigationSource,
      IEdmStructuredType expectedResourceType)
    {
      throw this.CreatePayloadKindNotSupportedException(ODataPayloadKind.Parameter);
    }

    public virtual ODataReader CreateUriParameterResourceSetReader(
      IEdmEntitySetBase entitySet,
      IEdmStructuredType expectedResourceType)
    {
      throw this.CreatePayloadKindNotSupportedException(ODataPayloadKind.ResourceSet);
    }

    public virtual Task<ODataReader> CreateUriParameterResourceSetReaderAsync(
      IEdmEntitySetBase entitySet,
      IEdmStructuredType expectedResourceType)
    {
      throw this.CreatePayloadKindNotSupportedException(ODataPayloadKind.ResourceSet);
    }

    public virtual ODataParameterReader CreateParameterReader(IEdmOperation operation) => throw this.CreatePayloadKindNotSupportedException(ODataPayloadKind.Parameter);

    public virtual Task<ODataParameterReader> CreateParameterReaderAsync(IEdmOperation operation) => throw this.CreatePayloadKindNotSupportedException(ODataPayloadKind.Parameter);

    internal virtual ODataAsynchronousReader CreateAsynchronousReader() => throw this.CreatePayloadKindNotSupportedException(ODataPayloadKind.Asynchronous);

    internal virtual Task<ODataAsynchronousReader> CreateAsynchronousReaderAsync() => throw this.CreatePayloadKindNotSupportedException(ODataPayloadKind.Asynchronous);

    internal virtual ODataDeltaReader CreateDeltaReader(
      IEdmEntitySetBase entitySet,
      IEdmEntityType expectedBaseEntityType)
    {
      throw this.CreatePayloadKindNotSupportedException(ODataPayloadKind.ResourceSet);
    }

    internal virtual Task<ODataDeltaReader> CreateDeltaReaderAsync(
      IEdmEntitySetBase entitySet,
      IEdmEntityType expectedBaseEntityType)
    {
      throw this.CreatePayloadKindNotSupportedException(ODataPayloadKind.ResourceSet);
    }

    internal virtual ODataBatchReader CreateBatchReader() => throw this.CreatePayloadKindNotSupportedException(ODataPayloadKind.Batch);

    internal virtual Task<ODataBatchReader> CreateBatchReaderAsync() => throw this.CreatePayloadKindNotSupportedException(ODataPayloadKind.Batch);

    internal virtual ODataServiceDocument ReadServiceDocument() => throw this.CreatePayloadKindNotSupportedException(ODataPayloadKind.ServiceDocument);

    internal virtual Task<ODataServiceDocument> ReadServiceDocumentAsync() => throw this.CreatePayloadKindNotSupportedException(ODataPayloadKind.ServiceDocument);

    internal virtual IEdmModel ReadMetadataDocument(
      Func<Uri, XmlReader> getReferencedModelReaderFunc)
    {
      throw this.CreatePayloadKindNotSupportedException(ODataPayloadKind.MetadataDocument);
    }

    internal virtual ODataEntityReferenceLinks ReadEntityReferenceLinks() => throw this.CreatePayloadKindNotSupportedException(ODataPayloadKind.EntityReferenceLinks);

    internal virtual Task<ODataEntityReferenceLinks> ReadEntityReferenceLinksAsync() => throw this.CreatePayloadKindNotSupportedException(ODataPayloadKind.EntityReferenceLinks);

    internal virtual ODataEntityReferenceLink ReadEntityReferenceLink() => throw this.CreatePayloadKindNotSupportedException(ODataPayloadKind.EntityReferenceLink);

    internal virtual Task<ODataEntityReferenceLink> ReadEntityReferenceLinkAsync() => throw this.CreatePayloadKindNotSupportedException(ODataPayloadKind.EntityReferenceLink);

    internal virtual object ReadValue(
      IEdmPrimitiveTypeReference expectedPrimitiveTypeReference)
    {
      throw this.CreatePayloadKindNotSupportedException(ODataPayloadKind.Value);
    }

    internal virtual Task<object> ReadValueAsync(
      IEdmPrimitiveTypeReference expectedPrimitiveTypeReference)
    {
      throw this.CreatePayloadKindNotSupportedException(ODataPayloadKind.Value);
    }

    internal void VerifyNotDisposed()
    {
      if (this.disposed)
        throw new ObjectDisposedException(this.GetType().FullName);
    }

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

    internal PropertyAndAnnotationCollector CreatePropertyAndAnnotationCollector() => this.messageReaderSettings.Validator.CreatePropertyAndAnnotationCollector();

    internal Uri ResolveUri(Uri baseUri, Uri payloadUri) => this.PayloadUriConverter != null ? this.PayloadUriConverter.ConvertPayloadUri(baseUri, payloadUri) : (Uri) null;

    protected virtual void Dispose(bool disposing)
    {
    }

    private ODataException CreatePayloadKindNotSupportedException(ODataPayloadKind payloadKind) => new ODataException(Strings.ODataInputContext_UnsupportedPayloadKindForFormat((object) this.format.ToString(), (object) payloadKind.ToString()));
  }
}
