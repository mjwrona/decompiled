// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.JsonLight.ODataJsonLightCollectionReader
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Edm;
using Microsoft.OData.Json;
using System;
using System.Threading.Tasks;

namespace Microsoft.OData.JsonLight
{
  internal sealed class ODataJsonLightCollectionReader : ODataCollectionReaderCoreAsync
  {
    private readonly ODataJsonLightInputContext jsonLightInputContext;
    private readonly ODataJsonLightCollectionDeserializer jsonLightCollectionDeserializer;

    internal ODataJsonLightCollectionReader(
      ODataJsonLightInputContext jsonLightInputContext,
      IEdmTypeReference expectedItemTypeReference,
      IODataReaderWriterListener listener)
      : base((ODataInputContext) jsonLightInputContext, expectedItemTypeReference, listener)
    {
      this.jsonLightInputContext = jsonLightInputContext;
      this.jsonLightCollectionDeserializer = new ODataJsonLightCollectionDeserializer(jsonLightInputContext);
    }

    protected override bool ReadAtStartImplementation()
    {
      PropertyAndAnnotationCollector annotationCollector = this.jsonLightInputContext.CreatePropertyAndAnnotationCollector();
      this.jsonLightCollectionDeserializer.ReadPayloadStart(ODataPayloadKind.Collection, annotationCollector, this.IsReadingNestedPayload, false);
      return this.ReadAtStartImplementationSynchronously(annotationCollector);
    }

    protected override Task<bool> ReadAtStartImplementationAsync()
    {
      PropertyAndAnnotationCollector propertyAndAnnotationCollector = this.jsonLightInputContext.CreatePropertyAndAnnotationCollector();
      return this.jsonLightCollectionDeserializer.ReadPayloadStartAsync(ODataPayloadKind.Collection, propertyAndAnnotationCollector, this.IsReadingNestedPayload, false).FollowOnSuccessWith<bool>((Func<Task, bool>) (t => this.ReadAtStartImplementationSynchronously(propertyAndAnnotationCollector)));
    }

    protected override bool ReadAtCollectionStartImplementation() => this.ReadAtCollectionStartImplementationSynchronously();

    protected override Task<bool> ReadAtCollectionStartImplementationAsync() => TaskUtils.GetTaskForSynchronousOperation<bool>(new Func<bool>(this.ReadAtCollectionStartImplementationSynchronously));

    protected override bool ReadAtValueImplementation() => this.ReadAtValueImplementationSynchronously();

    protected override Task<bool> ReadAtValueImplementationAsync() => TaskUtils.GetTaskForSynchronousOperation<bool>(new Func<bool>(this.ReadAtValueImplementationSynchronously));

    protected override bool ReadAtCollectionEndImplementation() => this.ReadAtCollectionEndImplementationSynchronously();

    protected override Task<bool> ReadAtCollectionEndImplementationAsync() => TaskUtils.GetTaskForSynchronousOperation<bool>(new Func<bool>(this.ReadAtCollectionEndImplementationSynchronously));

    private bool ReadAtStartImplementationSynchronously(
      PropertyAndAnnotationCollector propertyAndAnnotationCollector)
    {
      this.ExpectedItemTypeReference = ReaderValidationUtils.ValidateCollectionContextUriAndGetPayloadItemTypeReference(this.jsonLightCollectionDeserializer.ContextUriParseResult, this.ExpectedItemTypeReference);
      IEdmTypeReference actualItemTypeReference;
      ODataCollectionStart odataCollectionStart = this.jsonLightCollectionDeserializer.ReadCollectionStart(propertyAndAnnotationCollector, this.IsReadingNestedPayload, this.ExpectedItemTypeReference, out actualItemTypeReference);
      if (actualItemTypeReference != null)
        this.ExpectedItemTypeReference = actualItemTypeReference;
      this.jsonLightCollectionDeserializer.JsonReader.ReadStartArray();
      this.EnterScope(ODataCollectionReaderState.CollectionStart, (object) odataCollectionStart);
      return true;
    }

    private bool ReadAtCollectionStartImplementationSynchronously()
    {
      if (this.jsonLightCollectionDeserializer.JsonReader.NodeType == JsonNodeType.EndArray)
        this.ReplaceScope(ODataCollectionReaderState.CollectionEnd, this.Item);
      else
        this.EnterScope(ODataCollectionReaderState.Value, this.jsonLightCollectionDeserializer.ReadCollectionItem(this.ExpectedItemTypeReference, this.CollectionValidator));
      return true;
    }

    private bool ReadAtValueImplementationSynchronously()
    {
      if (this.jsonLightCollectionDeserializer.JsonReader.NodeType == JsonNodeType.EndArray)
      {
        this.PopScope(ODataCollectionReaderState.Value);
        this.ReplaceScope(ODataCollectionReaderState.CollectionEnd, this.Item);
      }
      else
        this.ReplaceScope(ODataCollectionReaderState.Value, this.jsonLightCollectionDeserializer.ReadCollectionItem(this.ExpectedItemTypeReference, this.CollectionValidator));
      return true;
    }

    private bool ReadAtCollectionEndImplementationSynchronously()
    {
      this.PopScope(ODataCollectionReaderState.CollectionEnd);
      this.jsonLightCollectionDeserializer.ReadCollectionEnd(this.IsReadingNestedPayload);
      this.jsonLightCollectionDeserializer.ReadPayloadEnd(this.IsReadingNestedPayload);
      this.ReplaceScope(ODataCollectionReaderState.Completed, (object) null);
      return false;
    }
  }
}
