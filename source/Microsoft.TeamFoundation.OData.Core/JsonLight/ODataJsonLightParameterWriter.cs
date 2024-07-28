// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.JsonLight.ODataJsonLightParameterWriter
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Edm;
using System.Threading.Tasks;

namespace Microsoft.OData.JsonLight
{
  internal sealed class ODataJsonLightParameterWriter : ODataParameterWriterCore
  {
    private readonly ODataJsonLightOutputContext jsonLightOutputContext;
    private readonly ODataJsonLightValueSerializer jsonLightValueSerializer;

    internal ODataJsonLightParameterWriter(
      ODataJsonLightOutputContext jsonLightOutputContext,
      IEdmOperation operation)
      : base((ODataOutputContext) jsonLightOutputContext, operation)
    {
      this.jsonLightOutputContext = jsonLightOutputContext;
      this.jsonLightValueSerializer = new ODataJsonLightValueSerializer(this.jsonLightOutputContext);
    }

    protected override void VerifyNotDisposed() => this.jsonLightOutputContext.VerifyNotDisposed();

    protected override void FlushSynchronously() => this.jsonLightOutputContext.Flush();

    protected override Task FlushAsynchronously() => this.jsonLightOutputContext.FlushAsync();

    protected override void StartPayload()
    {
      this.jsonLightValueSerializer.WritePayloadStart();
      this.jsonLightOutputContext.JsonWriter.StartObjectScope();
    }

    protected override void EndPayload()
    {
      this.jsonLightOutputContext.JsonWriter.EndObjectScope();
      this.jsonLightValueSerializer.WritePayloadEnd();
    }

    protected override void WriteValueParameter(
      string parameterName,
      object parameterValue,
      IEdmTypeReference expectedTypeReference)
    {
      this.jsonLightOutputContext.JsonWriter.WriteName(parameterName);
      if (parameterValue == null)
        this.jsonLightOutputContext.JsonWriter.WriteValue((string) null);
      else if (parameterValue is ODataEnumValue odataEnumValue)
        this.jsonLightValueSerializer.WriteEnumValue(odataEnumValue, expectedTypeReference);
      else
        this.jsonLightValueSerializer.WritePrimitiveValue(parameterValue, expectedTypeReference);
    }

    protected override ODataCollectionWriter CreateFormatCollectionWriter(
      string parameterName,
      IEdmTypeReference expectedItemType)
    {
      this.jsonLightOutputContext.JsonWriter.WriteName(parameterName);
      return (ODataCollectionWriter) new ODataJsonLightCollectionWriter(this.jsonLightOutputContext, expectedItemType, (IODataReaderWriterListener) this);
    }

    protected override ODataWriter CreateFormatResourceWriter(
      string parameterName,
      IEdmTypeReference expectedItemType)
    {
      this.jsonLightOutputContext.JsonWriter.WriteName(parameterName);
      return (ODataWriter) new ODataJsonLightWriter(this.jsonLightOutputContext, (IEdmNavigationSource) null, (IEdmStructuredType) null, false, true, listener: (IODataReaderWriterListener) this);
    }

    protected override ODataWriter CreateFormatResourceSetWriter(
      string parameterName,
      IEdmTypeReference expectedItemType)
    {
      this.jsonLightOutputContext.JsonWriter.WriteName(parameterName);
      return (ODataWriter) new ODataJsonLightWriter(this.jsonLightOutputContext, (IEdmNavigationSource) null, (IEdmStructuredType) null, true, true, listener: (IODataReaderWriterListener) this);
    }
  }
}
