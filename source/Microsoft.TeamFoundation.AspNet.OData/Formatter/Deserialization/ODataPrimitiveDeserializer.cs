// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Formatter.Deserialization.ODataPrimitiveDeserializer
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.AspNet.OData.Common;
using Microsoft.OData;
using Microsoft.OData.Edm;
using System;

namespace Microsoft.AspNet.OData.Formatter.Deserialization
{
  public class ODataPrimitiveDeserializer : ODataEdmTypeDeserializer
  {
    public ODataPrimitiveDeserializer()
      : base(ODataPayloadKind.Property)
    {
    }

    public override object Read(
      ODataMessageReader messageReader,
      Type type,
      ODataDeserializerContext readContext)
    {
      if (messageReader == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (messageReader));
      IEdmTypeReference edmType = readContext.GetEdmType(type);
      return this.ReadInline((object) messageReader.ReadProperty(edmType), edmType, readContext);
    }

    public override sealed object ReadInline(
      object item,
      IEdmTypeReference edmType,
      ODataDeserializerContext readContext)
    {
      if (item == null)
        return (object) null;
      return item is ODataProperty primitiveProperty ? this.ReadPrimitive(primitiveProperty, readContext) : throw Microsoft.AspNet.OData.Common.Error.Argument(nameof (item), SRResources.ArgumentMustBeOfType, (object) typeof (ODataProperty).Name);
    }

    public virtual object ReadPrimitive(
      ODataProperty primitiveProperty,
      ODataDeserializerContext readContext)
    {
      if (primitiveProperty == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (primitiveProperty));
      if (readContext == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (readContext));
      return readContext.ResourceType != (Type) null && primitiveProperty.Value != null ? EdmPrimitiveHelpers.ConvertPrimitiveValue(primitiveProperty.Value, readContext.ResourceType) : primitiveProperty.Value;
    }
  }
}
