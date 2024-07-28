// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.JsonLight.ODataJsonLightReaderStreamInfo
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Edm;

namespace Microsoft.OData.JsonLight
{
  internal sealed class ODataJsonLightReaderStreamInfo
  {
    private EdmPrimitiveTypeKind primitiveTypeKind;

    internal ODataJsonLightReaderStreamInfo(EdmPrimitiveTypeKind primitiveTypeKind) => this.PrimitiveTypeKind = primitiveTypeKind;

    internal ODataJsonLightReaderStreamInfo(
      EdmPrimitiveTypeKind primitiveTypeKind,
      string contentType)
    {
      this.PrimitiveTypeKind = primitiveTypeKind;
      this.ContentType = contentType;
      if (!contentType.Contains("application/json"))
        return;
      this.PrimitiveTypeKind = EdmPrimitiveTypeKind.String;
    }

    internal EdmPrimitiveTypeKind PrimitiveTypeKind
    {
      get => this.primitiveTypeKind;
      set => this.primitiveTypeKind = value == EdmPrimitiveTypeKind.Stream ? EdmPrimitiveTypeKind.Binary : value;
    }

    internal string ContentType { get; private set; }
  }
}
