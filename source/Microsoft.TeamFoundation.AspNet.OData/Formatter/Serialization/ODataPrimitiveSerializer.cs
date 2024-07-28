// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Formatter.Serialization.ODataPrimitiveSerializer
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.AspNet.OData.Common;
using Microsoft.OData;
using Microsoft.OData.Edm;
using System;
using System.Data.Linq;
using System.Xml.Linq;

namespace Microsoft.AspNet.OData.Formatter.Serialization
{
  public class ODataPrimitiveSerializer : ODataEdmTypeSerializer
  {
    private static readonly ODataNullValue NullValue = new ODataNullValue();

    public ODataPrimitiveSerializer()
      : base(ODataPayloadKind.Property)
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

    public override ODataValue CreateODataValue(
      object graph,
      IEdmTypeReference expectedType,
      ODataSerializerContext writeContext)
    {
      if (!expectedType.IsPrimitive())
        throw Microsoft.AspNet.OData.Common.Error.InvalidOperation(SRResources.CannotWriteType, (object) typeof (ODataPrimitiveSerializer), (object) expectedType.FullName());
      return (ODataValue) this.CreateODataPrimitiveValue(graph, expectedType.AsPrimitive(), writeContext) ?? (ODataValue) ODataPrimitiveSerializer.NullValue;
    }

    public virtual ODataPrimitiveValue CreateODataPrimitiveValue(
      object graph,
      IEdmPrimitiveTypeReference primitiveType,
      ODataSerializerContext writeContext)
    {
      return ODataPrimitiveSerializer.CreatePrimitive(graph, primitiveType, writeContext);
    }

    internal static void AddTypeNameAnnotationAsNeeded(
      ODataPrimitiveValue primitive,
      IEdmPrimitiveTypeReference primitiveType,
      ODataMetadataLevel metadataLevel)
    {
      object obj = primitive.Value;
      string typeName = (string) null;
      int metadataLevel1 = (int) metadataLevel;
      if (!ODataPrimitiveSerializer.ShouldSuppressTypeNameSerialization(obj, (ODataMetadataLevel) metadataLevel1))
        typeName = primitiveType.FullName();
      primitive.TypeAnnotation = new ODataTypeAnnotation(typeName);
    }

    internal static ODataPrimitiveValue CreatePrimitive(
      object value,
      IEdmPrimitiveTypeReference primitiveType,
      ODataSerializerContext writeContext)
    {
      if (value == null)
        return (ODataPrimitiveValue) null;
      ODataPrimitiveValue primitive = new ODataPrimitiveValue(ODataPrimitiveSerializer.ConvertPrimitiveValue(value, primitiveType));
      if (writeContext != null)
        ODataPrimitiveSerializer.AddTypeNameAnnotationAsNeeded(primitive, primitiveType, writeContext.MetadataLevel);
      return primitive;
    }

    internal static object ConvertPrimitiveValue(
      object value,
      IEdmPrimitiveTypeReference primitiveType)
    {
      if (value == null)
        return (object) null;
      Type type = value.GetType();
      if (primitiveType != null && primitiveType.IsDate() && TypeHelper.IsDateTime(type))
        return (object) (Date) (DateTime) value;
      return primitiveType != null && primitiveType.IsTimeOfDay() && TypeHelper.IsTimeSpan(type) ? (object) (TimeOfDay) (TimeSpan) value : ODataPrimitiveSerializer.ConvertUnsupportedPrimitives(value);
    }

    internal static object ConvertUnsupportedPrimitives(object value)
    {
      if (value != null)
      {
        Type type = value.GetType();
        switch (Type.GetTypeCode(type))
        {
          case TypeCode.Char:
            return (object) new string((char) value, 1);
          case TypeCode.UInt16:
            return (object) (int) (ushort) value;
          case TypeCode.UInt32:
            return (object) (long) (uint) value;
          case TypeCode.UInt64:
            return (object) checked ((long) (ulong) value);
          case TypeCode.DateTime:
            DateTime dateTime = (DateTime) value;
            TimeZoneInfo timeZone = TimeZoneInfoHelper.TimeZone;
            TimeSpan utcOffset1 = timeZone.GetUtcOffset(dateTime);
            if (utcOffset1 >= TimeSpan.Zero)
            {
              if (dateTime <= DateTime.MinValue + utcOffset1)
                return (object) DateTimeOffset.MinValue;
            }
            else if (dateTime >= DateTime.MaxValue + utcOffset1)
              return (object) DateTimeOffset.MaxValue;
            if (dateTime.Kind == DateTimeKind.Local)
            {
              TimeSpan utcOffset2 = TimeZoneInfo.Local.GetUtcOffset(dateTime);
              if (utcOffset2 < TimeSpan.Zero)
              {
                if (dateTime >= DateTime.MaxValue + utcOffset2)
                  return (object) DateTimeOffset.MaxValue;
              }
              else if (dateTime <= DateTime.MinValue + utcOffset2)
                return (object) DateTimeOffset.MinValue;
              return (object) TimeZoneInfo.ConvertTime(new DateTimeOffset(dateTime), timeZone);
            }
            return dateTime.Kind == DateTimeKind.Utc ? (object) TimeZoneInfo.ConvertTime(new DateTimeOffset(dateTime), timeZone) : (object) new DateTimeOffset(dateTime, timeZone.GetUtcOffset(dateTime));
          default:
            if (type == typeof (char[]))
              return (object) new string(value as char[]);
            if (type == typeof (XElement))
              return (object) ((XElement) value).ToString();
            if (type == typeof (Binary))
              return (object) ((Binary) value).ToArray();
            break;
        }
      }
      return value;
    }

    internal static bool CanTypeBeInferredInJson(object value)
    {
      switch (Type.GetTypeCode(value.GetType()))
      {
        case TypeCode.Boolean:
        case TypeCode.Int32:
        case TypeCode.String:
          return true;
        case TypeCode.Double:
          double d = (double) value;
          return !double.IsNaN(d) && !double.IsInfinity(d);
        default:
          return false;
      }
    }

    internal static bool ShouldSuppressTypeNameSerialization(
      object value,
      ODataMetadataLevel metadataLevel)
    {
      return metadataLevel != ODataMetadataLevel.FullMetadata || ODataPrimitiveSerializer.CanTypeBeInferredInJson(value);
    }
  }
}
