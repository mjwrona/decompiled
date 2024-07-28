// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Formatter.Serialization.ODataEnumSerializer
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.AspNet.OData.Common;
using Microsoft.OData;
using Microsoft.OData.Edm;
using System;

namespace Microsoft.AspNet.OData.Formatter.Serialization
{
  public class ODataEnumSerializer : ODataEdmTypeSerializer
  {
    public ODataEnumSerializer(ODataSerializerProvider serializerProvider)
      : base(ODataPayloadKind.Property, serializerProvider)
    {
    }

    public override void WriteObject(
      object graph,
      Type type,
      ODataMessageWriter messageWriter,
      ODataSerializerContext writeContext)
    {
      if (messageWriter == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (messageWriter));
      if (writeContext == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (writeContext));
      if (writeContext.RootElementName == null)
        throw Microsoft.AspNet.OData.Common.Error.Argument(nameof (writeContext), SRResources.RootElementNameMissing, (object) typeof (ODataSerializerContext).Name);
      IEdmTypeReference edmType = writeContext.GetEdmType(graph, type);
      messageWriter.WriteProperty(this.CreateProperty(graph, edmType, writeContext.RootElementName, writeContext));
    }

    public override sealed ODataValue CreateODataValue(
      object graph,
      IEdmTypeReference expectedType,
      ODataSerializerContext writeContext)
    {
      if (!expectedType.IsEnum())
        throw Microsoft.AspNet.OData.Common.Error.InvalidOperation(SRResources.CannotWriteType, (object) typeof (ODataEnumSerializer).Name, (object) expectedType.FullName());
      return (ODataValue) this.CreateODataEnumValue(graph, expectedType.AsEnum(), writeContext) ?? (ODataValue) new ODataNullValue();
    }

    public virtual ODataEnumValue CreateODataEnumValue(
      object graph,
      IEdmEnumTypeReference enumType,
      ODataSerializerContext writeContext)
    {
      if (graph == null)
        return (ODataEnumValue) null;
      string str = (string) null;
      if (TypeHelper.IsEnum(graph.GetType()))
        str = graph.ToString();
      else if (graph.GetType() == typeof (EdmEnumObject))
        str = ((EdmEnumObject) graph).Value;
      ClrEnumMemberAnnotation memberAnnotation = writeContext.Model.GetClrEnumMemberAnnotation(enumType.EnumDefinition());
      if (memberAnnotation != null)
      {
        IEdmEnumMember edmEnumMember = memberAnnotation.GetEdmEnumMember((Enum) graph);
        if (edmEnumMember != null)
          str = edmEnumMember.Name;
      }
      ODataEnumValue enumValue = new ODataEnumValue(str, enumType.FullName());
      ODataMetadataLevel metadataLevel = writeContext != null ? writeContext.MetadataLevel : ODataMetadataLevel.MinimalMetadata;
      ODataEnumSerializer.AddTypeNameAnnotationAsNeeded(enumValue, enumType, metadataLevel);
      return enumValue;
    }

    internal static void AddTypeNameAnnotationAsNeeded(
      ODataEnumValue enumValue,
      IEdmEnumTypeReference enumType,
      ODataMetadataLevel metadataLevel)
    {
      if (!ODataEnumSerializer.ShouldAddTypeNameAnnotation(metadataLevel))
        return;
      string typeName = !ODataEnumSerializer.ShouldSuppressTypeNameSerialization(metadataLevel) ? enumType.FullName() : (string) null;
      enumValue.TypeAnnotation = new ODataTypeAnnotation(typeName);
    }

    private static bool ShouldAddTypeNameAnnotation(ODataMetadataLevel metadataLevel)
    {
      switch (metadataLevel)
      {
        case ODataMetadataLevel.MinimalMetadata:
          return false;
        default:
          return true;
      }
    }

    private static bool ShouldSuppressTypeNameSerialization(ODataMetadataLevel metadataLevel) => metadataLevel != ODataMetadataLevel.FullMetadata && metadataLevel == ODataMetadataLevel.NoMetadata;
  }
}
