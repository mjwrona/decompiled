// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.JsonLight.ODataJsonLightParameterReader
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Edm;
using Microsoft.OData.Json;
using System;
using System.Threading.Tasks;

namespace Microsoft.OData.JsonLight
{
  internal sealed class ODataJsonLightParameterReader : ODataParameterReaderCoreAsync
  {
    private readonly ODataJsonLightInputContext jsonLightInputContext;
    private readonly ODataJsonLightParameterDeserializer jsonLightParameterDeserializer;
    private PropertyAndAnnotationCollector propertyAndAnnotationCollector;

    internal ODataJsonLightParameterReader(
      ODataJsonLightInputContext jsonLightInputContext,
      IEdmOperation operation)
      : base((ODataInputContext) jsonLightInputContext, operation)
    {
      this.jsonLightInputContext = jsonLightInputContext;
      this.jsonLightParameterDeserializer = new ODataJsonLightParameterDeserializer(this, jsonLightInputContext);
    }

    protected override bool ReadAtStartImplementation()
    {
      this.propertyAndAnnotationCollector = this.jsonLightInputContext.CreatePropertyAndAnnotationCollector();
      this.jsonLightParameterDeserializer.ReadPayloadStart(ODataPayloadKind.Parameter, this.propertyAndAnnotationCollector, false, true);
      return this.ReadAtStartImplementationSynchronously();
    }

    protected override Task<bool> ReadAtStartImplementationAsync()
    {
      this.propertyAndAnnotationCollector = this.jsonLightInputContext.CreatePropertyAndAnnotationCollector();
      return this.jsonLightParameterDeserializer.ReadPayloadStartAsync(ODataPayloadKind.Parameter, this.propertyAndAnnotationCollector, false, true).FollowOnSuccessWith<bool>((Func<Task, bool>) (t => this.ReadAtStartImplementationSynchronously()));
    }

    protected override bool ReadNextParameterImplementation() => this.ReadNextParameterImplementationSynchronously();

    protected override Task<bool> ReadNextParameterImplementationAsync() => TaskUtils.GetTaskForSynchronousOperation<bool>(new Func<bool>(this.ReadNextParameterImplementationSynchronously));

    protected override ODataReader CreateResourceReader(IEdmStructuredType expectedResourceType) => this.CreateResourceReaderSynchronously(expectedResourceType);

    protected override Task<ODataReader> CreateResourceReaderAsync(
      IEdmStructuredType expectedResourceType)
    {
      return TaskUtils.GetTaskForSynchronousOperation<ODataReader>((Func<ODataReader>) (() => this.CreateResourceReaderSynchronously(expectedResourceType)));
    }

    protected override ODataReader CreateResourceSetReader(IEdmStructuredType expectedResourceType) => this.CreateResourceSetReaderSynchronously(expectedResourceType);

    protected override Task<ODataReader> CreateResourceSetReaderAsync(
      IEdmStructuredType expectedResourceType)
    {
      return TaskUtils.GetTaskForSynchronousOperation<ODataReader>((Func<ODataReader>) (() => this.CreateResourceSetReaderSynchronously(expectedResourceType)));
    }

    protected override ODataCollectionReader CreateCollectionReader(
      IEdmTypeReference expectedItemTypeReference)
    {
      return this.CreateCollectionReaderSynchronously(expectedItemTypeReference);
    }

    protected override Task<ODataCollectionReader> CreateCollectionReaderAsync(
      IEdmTypeReference expectedItemTypeReference)
    {
      return TaskUtils.GetTaskForSynchronousOperation<ODataCollectionReader>((Func<ODataCollectionReader>) (() => this.CreateCollectionReaderSynchronously(expectedItemTypeReference)));
    }

    private bool ReadAtStartImplementationSynchronously()
    {
      if (this.jsonLightInputContext.JsonReader.NodeType != JsonNodeType.EndOfInput)
        return this.jsonLightParameterDeserializer.ReadNextParameter(this.propertyAndAnnotationCollector);
      this.PopScope(ODataParameterReaderState.Start);
      this.EnterScope(ODataParameterReaderState.Completed, (string) null, (object) null);
      return false;
    }

    private bool ReadNextParameterImplementationSynchronously()
    {
      this.PopScope(this.State);
      return this.jsonLightParameterDeserializer.ReadNextParameter(this.propertyAndAnnotationCollector);
    }

    private ODataReader CreateResourceReaderSynchronously(IEdmStructuredType expectedResourceType) => (ODataReader) new ODataJsonLightReader(this.jsonLightInputContext, (IEdmNavigationSource) null, expectedResourceType, false, true, listener: (IODataReaderWriterListener) this);

    private ODataReader CreateResourceSetReaderSynchronously(IEdmStructuredType expectedResourceType) => (ODataReader) new ODataJsonLightReader(this.jsonLightInputContext, (IEdmNavigationSource) null, expectedResourceType, true, true, listener: (IODataReaderWriterListener) this);

    private ODataCollectionReader CreateCollectionReaderSynchronously(
      IEdmTypeReference expectedItemTypeReference)
    {
      return (ODataCollectionReader) new ODataJsonLightCollectionReader(this.jsonLightInputContext, expectedItemTypeReference, (IODataReaderWriterListener) this);
    }
  }
}
