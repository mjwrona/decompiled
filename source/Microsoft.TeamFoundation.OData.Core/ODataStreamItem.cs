// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.ODataStreamItem
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Edm;

namespace Microsoft.OData
{
  public sealed class ODataStreamItem : ODataItem
  {
    private EdmPrimitiveTypeKind typeKind;

    public ODataStreamItem(EdmPrimitiveTypeKind primitiveTypeKind)
      : this(primitiveTypeKind, (string) null)
    {
    }

    public ODataStreamItem(EdmPrimitiveTypeKind primitiveTypeKind, string contentType)
    {
      this.PrimitiveTypeKind = primitiveTypeKind;
      this.ContentType = contentType;
    }

    public EdmPrimitiveTypeKind PrimitiveTypeKind
    {
      get => this.typeKind;
      private set => this.typeKind = this.typeKind == EdmPrimitiveTypeKind.String || this.typeKind == EdmPrimitiveTypeKind.Binary || this.typeKind == EdmPrimitiveTypeKind.None ? value : throw new ODataException(Strings.StreamItemInvalidPrimitiveKind((object) value));
    }

    public string ContentType { get; private set; }
  }
}
