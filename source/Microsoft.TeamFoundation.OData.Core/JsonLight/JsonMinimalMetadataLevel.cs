// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.JsonLight.JsonMinimalMetadataLevel
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Edm;
using Microsoft.OData.Evaluation;

namespace Microsoft.OData.JsonLight
{
  internal sealed class JsonMinimalMetadataLevel : JsonLightMetadataLevel
  {
    internal override JsonLightTypeNameOracle GetTypeNameOracle() => (JsonLightTypeNameOracle) new JsonMinimalMetadataTypeNameOracle();

    internal override ODataResourceMetadataBuilder CreateResourceMetadataBuilder(
      ODataResourceBase resource,
      IODataResourceTypeContext typeContext,
      ODataResourceSerializationInfo serializationInfo,
      IEdmStructuredType actualResourceType,
      SelectedPropertiesNode selectedProperties,
      bool isResponse,
      bool keyAsSegment,
      ODataUri odataUri,
      ODataMessageWriterSettings settings)
    {
      return (ODataResourceMetadataBuilder) null;
    }

    internal override void InjectMetadataBuilder(
      ODataResourceBase resource,
      ODataResourceMetadataBuilder builder)
    {
    }
  }
}
