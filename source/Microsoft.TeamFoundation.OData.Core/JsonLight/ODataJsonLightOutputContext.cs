// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.JsonLight.ODataJsonLightOutputContext
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Edm;
using Microsoft.OData.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading.Tasks;

namespace Microsoft.OData.JsonLight
{
  internal sealed class ODataJsonLightOutputContext : ODataOutputContext
  {
    internal MemoryStream BinaryValueStream;
    internal StringWriter StringWriter;
    private readonly JsonLightMetadataLevel metadataLevel;
    private IODataOutputInStreamErrorListener outputInStreamErrorListener;
    private Stream messageOutputStream;
    private AsyncBufferedStream asynchronousOutputStream;
    private TextWriter textWriter;
    private IJsonWriter jsonWriter;
    private JsonLightTypeNameOracle typeNameOracle;
    private PropertyCacheHandler propertyCacheHandler;

    public ODataJsonLightOutputContext(
      ODataMessageInfo messageInfo,
      ODataMessageWriterSettings messageWriterSettings)
      : base(ODataFormat.Json, messageInfo, messageWriterSettings)
    {
      try
      {
        this.messageOutputStream = messageInfo.MessageStream;
        Stream stream;
        if (this.Synchronous)
        {
          stream = this.messageOutputStream;
        }
        else
        {
          this.asynchronousOutputStream = new AsyncBufferedStream(this.messageOutputStream);
          stream = (Stream) this.asynchronousOutputStream;
        }
        this.textWriter = (TextWriter) new StreamWriter(stream, messageInfo.Encoding);
        this.jsonWriter = ODataJsonLightOutputContext.CreateJsonWriter(this.Container, this.textWriter, messageInfo.MediaType.HasIeee754CompatibleSetToTrue(), messageWriterSettings);
      }
      catch (Exception ex)
      {
        if (ExceptionUtils.IsCatchableExceptionType(ex))
          this.messageOutputStream.Dispose();
        throw;
      }
      Uri metadataDocumentUri = messageWriterSettings.MetadataDocumentUri;
      this.metadataLevel = JsonLightMetadataLevel.Create(messageInfo.MediaType, metadataDocumentUri, this.Model, this.WritingResponse);
      this.propertyCacheHandler = new PropertyCacheHandler();
    }

    internal ODataJsonLightOutputContext(
      TextWriter textWriter,
      ODataMessageInfo messageInfo,
      ODataMessageWriterSettings messageWriterSettings)
      : base(ODataFormat.Json, messageInfo, messageWriterSettings)
    {
      this.textWriter = textWriter;
      this.jsonWriter = ODataJsonLightOutputContext.CreateJsonWriter(messageInfo.Container, textWriter, true, messageWriterSettings);
      this.metadataLevel = (JsonLightMetadataLevel) new JsonMinimalMetadataLevel();
      this.propertyCacheHandler = new PropertyCacheHandler();
    }

    public IJsonWriter JsonWriter => this.jsonWriter;

    public JsonLightTypeNameOracle TypeNameOracle
    {
      get
      {
        if (this.typeNameOracle == null)
          this.typeNameOracle = this.MetadataLevel.GetTypeNameOracle();
        return this.typeNameOracle;
      }
    }

    public JsonLightMetadataLevel MetadataLevel => this.metadataLevel;

    public PropertyCacheHandler PropertyCacheHandler => this.propertyCacheHandler;

    internal bool OmitODataPrefix => this.ODataSimplifiedOptions.GetOmitODataPrefix((ODataVersion) ((int) this.MessageWriterSettings.Version ?? 0));

    public override ODataWriter CreateODataResourceSetWriter(
      IEdmEntitySetBase entitySet,
      IEdmStructuredType resourceType)
    {
      return this.CreateODataResourceSetWriterImplementation(entitySet, resourceType, false, false);
    }

    public override Task<ODataWriter> CreateODataResourceSetWriterAsync(
      IEdmEntitySetBase entitySet,
      IEdmStructuredType resourceType)
    {
      return TaskUtils.GetTaskForSynchronousOperation<ODataWriter>((Func<ODataWriter>) (() => this.CreateODataResourceSetWriterImplementation(entitySet, resourceType, false, false)));
    }

    public override ODataWriter CreateODataDeltaResourceSetWriter(
      IEdmEntitySetBase entitySet,
      IEdmStructuredType resourceType)
    {
      return this.CreateODataResourceSetWriterImplementation(entitySet, resourceType, false, true);
    }

    public override Task<ODataWriter> CreateODataDeltaResourceSetWriterAsync(
      IEdmEntitySetBase entitySet,
      IEdmStructuredType resourceType)
    {
      return TaskUtils.GetTaskForSynchronousOperation<ODataWriter>((Func<ODataWriter>) (() => this.CreateODataResourceSetWriterImplementation(entitySet, resourceType, false, true)));
    }

    public override ODataWriter CreateODataResourceWriter(
      IEdmNavigationSource navigationSource,
      IEdmStructuredType resourceType)
    {
      return this.CreateODataResourceWriterImplementation(navigationSource, resourceType);
    }

    public override Task<ODataWriter> CreateODataResourceWriterAsync(
      IEdmNavigationSource navigationSource,
      IEdmStructuredType resourceType)
    {
      return TaskUtils.GetTaskForSynchronousOperation<ODataWriter>((Func<ODataWriter>) (() => this.CreateODataResourceWriterImplementation(navigationSource, resourceType)));
    }

    public override ODataCollectionWriter CreateODataCollectionWriter(
      IEdmTypeReference itemTypeReference)
    {
      return this.CreateODataCollectionWriterImplementation(itemTypeReference);
    }

    public override Task<ODataCollectionWriter> CreateODataCollectionWriterAsync(
      IEdmTypeReference itemTypeReference)
    {
      return TaskUtils.GetTaskForSynchronousOperation<ODataCollectionWriter>((Func<ODataCollectionWriter>) (() => this.CreateODataCollectionWriterImplementation(itemTypeReference)));
    }

    public override ODataWriter CreateODataUriParameterResourceWriter(
      IEdmNavigationSource navigationSource,
      IEdmStructuredType resourceType)
    {
      return this.CreateODataResourceWriter(navigationSource, resourceType);
    }

    public override Task<ODataWriter> CreateODataUriParameterResourceWriterAsync(
      IEdmNavigationSource navigationSource,
      IEdmStructuredType resourceType)
    {
      return this.CreateODataResourceWriterAsync(navigationSource, resourceType);
    }

    public override ODataWriter CreateODataUriParameterResourceSetWriter(
      IEdmEntitySetBase entitySet,
      IEdmStructuredType resourceType)
    {
      return this.CreateODataResourceSetWriterImplementation(entitySet, resourceType, true, false);
    }

    public override Task<ODataWriter> CreateODataUriParameterResourceSetWriterAsync(
      IEdmEntitySetBase entitySet,
      IEdmStructuredType resourceType)
    {
      return TaskUtils.GetTaskForSynchronousOperation<ODataWriter>((Func<ODataWriter>) (() => this.CreateODataResourceSetWriterImplementation(entitySet, resourceType, true, false)));
    }

    public override ODataParameterWriter CreateODataParameterWriter(IEdmOperation operation) => this.CreateODataParameterWriterImplementation(operation);

    public override Task<ODataParameterWriter> CreateODataParameterWriterAsync(
      IEdmOperation operation)
    {
      return TaskUtils.GetTaskForSynchronousOperation<ODataParameterWriter>((Func<ODataParameterWriter>) (() => this.CreateODataParameterWriterImplementation(operation)));
    }

    public override void WriteProperty(ODataProperty property)
    {
      this.WritePropertyImplementation(property);
      this.Flush();
    }

    public override Task WritePropertyAsync(ODataProperty property) => TaskUtils.GetTaskForSynchronousOperationReturningTask((Func<Task>) (() =>
    {
      this.WritePropertyImplementation(property);
      return this.FlushAsync();
    }));

    public override void WriteError(ODataError error, bool includeDebugInformation)
    {
      this.WriteErrorImplementation(error, includeDebugInformation);
      this.Flush();
    }

    public override Task WriteErrorAsync(ODataError error, bool includeDebugInformation) => TaskUtils.GetTaskForSynchronousOperationReturningTask((Func<Task>) (() =>
    {
      this.WriteErrorImplementation(error, includeDebugInformation);
      return this.FlushAsync();
    }));

    public void VerifyNotDisposed()
    {
      if (this.messageOutputStream == null)
        throw new ObjectDisposedException(this.GetType().FullName);
    }

    public void Flush() => this.jsonWriter.Flush();

    public Task FlushAsync() => TaskUtils.GetTaskForSynchronousOperationReturningTask((Func<Task>) (() =>
    {
      this.jsonWriter.Flush();
      return this.asynchronousOutputStream.FlushAsync();
    })).FollowOnSuccessWithTask((Func<Task, Task>) (asyncBufferedStreamFlushTask => this.messageOutputStream.FlushAsync()));

    internal void FlushBuffers()
    {
      if (this.asynchronousOutputStream == null)
        return;
      this.asynchronousOutputStream.FlushSync();
    }

    internal Task FlushBuffersAsync() => this.asynchronousOutputStream != null ? this.asynchronousOutputStream.FlushAsync() : TaskUtils.CompletedTask;

    internal Stream GetOutputStream() => !this.Synchronous ? (Stream) this.asynchronousOutputStream : this.messageOutputStream;

    internal override ODataBatchWriter CreateODataBatchWriter() => this.CreateODataBatchWriterImplementation();

    internal override Task<ODataBatchWriter> CreateODataBatchWriterAsync() => TaskUtils.GetTaskForSynchronousOperation<ODataBatchWriter>((Func<ODataBatchWriter>) (() => this.CreateODataBatchWriterImplementation()));

    internal override void WriteInStreamError(ODataError error, bool includeDebugInformation)
    {
      this.WriteInStreamErrorImplementation(error, includeDebugInformation);
      this.Flush();
    }

    internal override Task WriteInStreamErrorAsync(ODataError error, bool includeDebugInformation) => TaskUtils.GetTaskForSynchronousOperationReturningTask((Func<Task>) (() =>
    {
      this.WriteInStreamErrorImplementation(error, includeDebugInformation);
      return this.FlushAsync();
    }));

    internal override ODataDeltaWriter CreateODataDeltaWriter(
      IEdmEntitySetBase entitySet,
      IEdmEntityType entityType)
    {
      return this.CreateODataDeltaWriterImplementation(entitySet, entityType);
    }

    internal override Task<ODataDeltaWriter> CreateODataDeltaWriterAsync(
      IEdmEntitySetBase entitySet,
      IEdmEntityType entityType)
    {
      return TaskUtils.GetTaskForSynchronousOperation<ODataDeltaWriter>((Func<ODataDeltaWriter>) (() => this.CreateODataDeltaWriterImplementation(entitySet, entityType)));
    }

    internal override void WriteServiceDocument(ODataServiceDocument serviceDocument)
    {
      this.WriteServiceDocumentImplementation(serviceDocument);
      this.Flush();
    }

    internal override Task WriteServiceDocumentAsync(ODataServiceDocument serviceDocument) => TaskUtils.GetTaskForSynchronousOperationReturningTask((Func<Task>) (() =>
    {
      this.WriteServiceDocumentImplementation(serviceDocument);
      return this.FlushAsync();
    }));

    internal override void WriteEntityReferenceLinks(ODataEntityReferenceLinks links)
    {
      this.WriteEntityReferenceLinksImplementation(links);
      this.Flush();
    }

    internal override Task WriteEntityReferenceLinksAsync(ODataEntityReferenceLinks links) => TaskUtils.GetTaskForSynchronousOperationReturningTask((Func<Task>) (() =>
    {
      this.WriteEntityReferenceLinksImplementation(links);
      return this.FlushAsync();
    }));

    internal override void WriteEntityReferenceLink(ODataEntityReferenceLink link)
    {
      this.WriteEntityReferenceLinkImplementation(link);
      this.Flush();
    }

    internal override Task WriteEntityReferenceLinkAsync(ODataEntityReferenceLink link) => TaskUtils.GetTaskForSynchronousOperationReturningTask((Func<Task>) (() =>
    {
      this.WriteEntityReferenceLinkImplementation(link);
      return this.FlushAsync();
    }));

    [SuppressMessage("Microsoft.Usage", "CA2213:DisposableFieldsShouldBeDisposed", MessageId = "textWriter", Justification = "We don't dispose the jsonWriter or textWriter, instead we dispose the underlying stream directly.")]
    protected override void Dispose(bool disposing)
    {
      try
      {
        if (this.messageOutputStream != null)
        {
          this.jsonWriter.Flush();
          if (this.jsonWriter is Microsoft.OData.Json.JsonWriter jsonWriter)
            jsonWriter.Dispose();
          if (this.asynchronousOutputStream != null)
          {
            this.asynchronousOutputStream.FlushSync();
            this.asynchronousOutputStream.Dispose();
          }
          this.messageOutputStream.Dispose();
        }
        if (this.BinaryValueStream != null)
        {
          this.BinaryValueStream.Flush();
          this.BinaryValueStream.Dispose();
        }
        if (this.StringWriter != null)
        {
          this.StringWriter.Flush();
          this.StringWriter.Dispose();
        }
      }
      finally
      {
        this.messageOutputStream = (Stream) null;
        this.asynchronousOutputStream = (AsyncBufferedStream) null;
        this.BinaryValueStream = (MemoryStream) null;
        this.textWriter = (TextWriter) null;
        this.jsonWriter = (IJsonWriter) null;
        this.StringWriter = (StringWriter) null;
      }
      base.Dispose(disposing);
    }

    private static IJsonWriter CreateJsonWriter(
      IServiceProvider container,
      TextWriter textWriter,
      bool isIeee754Compatible,
      ODataMessageWriterSettings writerSettings)
    {
      IJsonWriter jsonWriter1 = container != null ? container.GetRequiredService<IJsonWriterFactory>().CreateJsonWriter(textWriter, isIeee754Compatible) : (IJsonWriter) new Microsoft.OData.Json.JsonWriter(textWriter, isIeee754Compatible);
      if (jsonWriter1 is Microsoft.OData.Json.JsonWriter jsonWriter2 && writerSettings.ArrayPool != null)
        jsonWriter2.ArrayPool = writerSettings.ArrayPool;
      return jsonWriter1;
    }

    private ODataWriter CreateODataResourceSetWriterImplementation(
      IEdmEntitySetBase entitySet,
      IEdmStructuredType resourceType,
      bool writingParameter,
      bool writingDelta)
    {
      ODataJsonLightWriter writerImplementation = new ODataJsonLightWriter(this, (IEdmNavigationSource) entitySet, resourceType, true, writingParameter, writingDelta);
      this.outputInStreamErrorListener = (IODataOutputInStreamErrorListener) writerImplementation;
      return (ODataWriter) writerImplementation;
    }

    private ODataDeltaWriter CreateODataDeltaWriterImplementation(
      IEdmEntitySetBase entitySet,
      IEdmEntityType entityType)
    {
      ODataJsonLightDeltaWriter writerImplementation = new ODataJsonLightDeltaWriter(this, (IEdmNavigationSource) entitySet, entityType);
      this.outputInStreamErrorListener = (IODataOutputInStreamErrorListener) writerImplementation;
      return (ODataDeltaWriter) writerImplementation;
    }

    private ODataWriter CreateODataResourceWriterImplementation(
      IEdmNavigationSource navigationSource,
      IEdmStructuredType resourceType)
    {
      ODataJsonLightWriter writerImplementation = new ODataJsonLightWriter(this, navigationSource, resourceType, false);
      this.outputInStreamErrorListener = (IODataOutputInStreamErrorListener) writerImplementation;
      return (ODataWriter) writerImplementation;
    }

    private ODataCollectionWriter CreateODataCollectionWriterImplementation(
      IEdmTypeReference itemTypeReference)
    {
      ODataJsonLightCollectionWriter writerImplementation = new ODataJsonLightCollectionWriter(this, itemTypeReference);
      this.outputInStreamErrorListener = (IODataOutputInStreamErrorListener) writerImplementation;
      return (ODataCollectionWriter) writerImplementation;
    }

    private ODataParameterWriter CreateODataParameterWriterImplementation(IEdmOperation operation)
    {
      ODataJsonLightParameterWriter writerImplementation = new ODataJsonLightParameterWriter(this, operation);
      this.outputInStreamErrorListener = (IODataOutputInStreamErrorListener) writerImplementation;
      return (ODataParameterWriter) writerImplementation;
    }

    private ODataBatchWriter CreateODataBatchWriterImplementation()
    {
      ODataBatchWriter writerImplementation = (ODataBatchWriter) new ODataJsonLightBatchWriter(this);
      this.outputInStreamErrorListener = (IODataOutputInStreamErrorListener) writerImplementation;
      return writerImplementation;
    }

    private void WriteInStreamErrorImplementation(ODataError error, bool includeDebugInformation)
    {
      if (this.outputInStreamErrorListener != null)
        this.outputInStreamErrorListener.OnInStreamError();
      ODataJsonWriterUtils.WriteError(this.JsonWriter, new Action<IEnumerable<ODataInstanceAnnotation>>(new JsonLightInstanceAnnotationWriter(new ODataJsonLightValueSerializer(this), this.TypeNameOracle).WriteInstanceAnnotationsForError), error, includeDebugInformation, this.MessageWriterSettings.MessageQuotas.MaxNestingDepth, true);
    }

    private void WritePropertyImplementation(ODataProperty property) => new ODataJsonLightPropertySerializer(this, true).WriteTopLevelProperty(property);

    private void WriteServiceDocumentImplementation(ODataServiceDocument serviceDocument) => new ODataJsonLightServiceDocumentSerializer(this).WriteServiceDocument(serviceDocument);

    private void WriteErrorImplementation(ODataError error, bool includeDebugInformation) => new ODataJsonLightSerializer(this).WriteTopLevelError(error, includeDebugInformation);

    private void WriteEntityReferenceLinksImplementation(ODataEntityReferenceLinks links) => new ODataJsonLightEntityReferenceLinkSerializer(this).WriteEntityReferenceLinks(links);

    private void WriteEntityReferenceLinkImplementation(ODataEntityReferenceLink link) => new ODataJsonLightEntityReferenceLinkSerializer(this).WriteEntityReferenceLink(link);
  }
}
