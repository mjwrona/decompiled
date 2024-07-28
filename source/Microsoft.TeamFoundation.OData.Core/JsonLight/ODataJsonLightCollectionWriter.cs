// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.JsonLight.ODataJsonLightCollectionWriter
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Edm;
using System.Threading.Tasks;

namespace Microsoft.OData.JsonLight
{
  internal sealed class ODataJsonLightCollectionWriter : ODataCollectionWriterCore
  {
    private readonly ODataJsonLightOutputContext jsonLightOutputContext;
    private readonly ODataJsonLightCollectionSerializer jsonLightCollectionSerializer;

    internal ODataJsonLightCollectionWriter(
      ODataJsonLightOutputContext jsonLightOutputContext,
      IEdmTypeReference itemTypeReference)
      : base((ODataOutputContext) jsonLightOutputContext, itemTypeReference)
    {
      this.jsonLightOutputContext = jsonLightOutputContext;
      this.jsonLightCollectionSerializer = new ODataJsonLightCollectionSerializer(this.jsonLightOutputContext, true);
    }

    internal ODataJsonLightCollectionWriter(
      ODataJsonLightOutputContext jsonLightOutputContext,
      IEdmTypeReference expectedItemType,
      IODataReaderWriterListener listener)
      : base((ODataOutputContext) jsonLightOutputContext, expectedItemType, listener)
    {
      this.jsonLightOutputContext = jsonLightOutputContext;
      this.jsonLightCollectionSerializer = new ODataJsonLightCollectionSerializer(this.jsonLightOutputContext, false);
    }

    protected override void VerifyNotDisposed() => this.jsonLightOutputContext.VerifyNotDisposed();

    protected override void FlushSynchronously() => this.jsonLightOutputContext.Flush();

    protected override Task FlushAsynchronously() => this.jsonLightOutputContext.FlushAsync();

    protected override void StartPayload() => this.jsonLightCollectionSerializer.WritePayloadStart();

    protected override void EndPayload() => this.jsonLightCollectionSerializer.WritePayloadEnd();

    protected override void StartCollection(ODataCollectionStart collectionStart) => this.jsonLightCollectionSerializer.WriteCollectionStart(collectionStart, this.ItemTypeReference);

    protected override void EndCollection() => this.jsonLightCollectionSerializer.WriteCollectionEnd();

    protected override void WriteCollectionItem(object item, IEdmTypeReference expectedItemType)
    {
      if (item == null)
      {
        this.jsonLightOutputContext.WriterValidator.ValidateNullCollectionItem(expectedItemType);
        this.jsonLightOutputContext.JsonWriter.WriteValue((string) null);
      }
      else
      {
        ODataResourceValue resourceValue = item as ODataResourceValue;
        if (resourceValue != null)
        {
          this.jsonLightCollectionSerializer.WriteResourceValue(resourceValue, expectedItemType, false, this.jsonLightOutputContext.ShouldOmitNullValues(), this.DuplicatePropertyNameChecker);
          this.DuplicatePropertyNameChecker.Reset();
        }
        else if (item is ODataEnumValue odataEnumValue)
        {
          if (odataEnumValue.Value == null)
            this.jsonLightCollectionSerializer.WriteNullValue();
          else
            this.jsonLightCollectionSerializer.WritePrimitiveValue((object) odataEnumValue.Value, (IEdmTypeReference) EdmCoreModel.Instance.GetString(true));
        }
        else
          this.jsonLightCollectionSerializer.WritePrimitiveValue(item, expectedItemType);
      }
    }
  }
}
