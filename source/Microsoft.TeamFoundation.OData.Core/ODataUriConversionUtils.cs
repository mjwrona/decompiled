// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.ODataUriConversionUtils
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Edm;
using Microsoft.OData.Evaluation;
using Microsoft.OData.JsonLight;
using Microsoft.OData.Metadata;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Text;

namespace Microsoft.OData
{
  internal static class ODataUriConversionUtils
  {
    internal static string ConvertToUriPrimitiveLiteral(object value, ODataVersion version)
    {
      ExceptionUtils.CheckArgumentNotNull<object>(value, nameof (value));
      return LiteralFormatter.ForConstantsWithoutEncoding.Format(value);
    }

    internal static string ConvertToUriEnumLiteral(ODataEnumValue value, ODataVersion version)
    {
      ExceptionUtils.CheckArgumentNotNull<ODataEnumValue>(value, nameof (value));
      ExceptionUtils.CheckArgumentNotNull<string>(value.TypeName, "value.TypeName");
      ExceptionUtils.CheckArgumentNotNull<string>(value.Value, "value.Value");
      return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}'{1}'", new object[2]
      {
        (object) value.TypeName,
        (object) value.Value
      });
    }

    internal static object ConvertFromResourceValue(
      string value,
      IEdmModel model,
      IEdmTypeReference typeReference)
    {
      return ODataUriConversionUtils.ConvertFromResourceOrCollectionValue(value, model, typeReference);
    }

    internal static object ConvertFromCollectionValue(
      string value,
      IEdmModel model,
      IEdmTypeReference typeReference)
    {
      return ODataUriConversionUtils.ConvertFromResourceOrCollectionValue(value, model, typeReference);
    }

    internal static object VerifyAndCoerceUriPrimitiveLiteral(
      object primitiveValue,
      string literalValue,
      IEdmModel model,
      IEdmTypeReference expectedTypeReference)
    {
      ExceptionUtils.CheckArgumentNotNull<object>(primitiveValue, nameof (primitiveValue));
      ExceptionUtils.CheckArgumentNotNull<string>(literalValue, nameof (literalValue));
      ExceptionUtils.CheckArgumentNotNull<IEdmModel>(model, nameof (model));
      ExceptionUtils.CheckArgumentNotNull<IEdmTypeReference>(expectedTypeReference, nameof (expectedTypeReference));
      if (primitiveValue is ODataNullValue odataNullValue)
      {
        if (!expectedTypeReference.IsNullable)
          throw new ODataException(Strings.ODataUriUtils_ConvertFromUriLiteralNullOnNonNullableType((object) expectedTypeReference.FullName()));
        return (object) odataNullValue;
      }
      IEdmPrimitiveTypeReference primitiveTypeReference = expectedTypeReference.AsPrimitiveOrNull();
      object obj1 = primitiveTypeReference != null ? ODataUriConversionUtils.CoerceNumericType(primitiveValue, primitiveTypeReference.PrimitiveDefinition()) : throw new ODataException(Strings.ODataUriUtils_ConvertFromUriLiteralTypeVerificationFailure((object) expectedTypeReference.FullName(), (object) literalValue));
      if (obj1 != null)
        return obj1;
      object obj2 = ODataUriConversionUtils.CoerceTemporalType(primitiveValue, primitiveTypeReference.PrimitiveDefinition());
      if (obj2 != null)
        return obj2;
      Type type = primitiveValue.GetType();
      if (TypeUtils.GetNonNullableType(EdmLibraryExtensions.GetPrimitiveClrType(primitiveTypeReference)).IsAssignableFrom(type))
        return primitiveValue;
      throw new ODataException(Strings.ODataUriUtils_ConvertFromUriLiteralTypeVerificationFailure((object) primitiveTypeReference.FullName(), (object) literalValue));
    }

    internal static string ConvertToUriEntityLiteral(ODataResourceBase resource, IEdmModel model)
    {
      ExceptionUtils.CheckArgumentNotNull<ODataResourceBase>(resource, nameof (resource));
      ExceptionUtils.CheckArgumentNotNull<IEdmModel>(model, nameof (model));
      return ODataUriConversionUtils.ConvertToJsonLightLiteral(model, (Action<ODataOutputContext>) (context =>
      {
        ODataWriter parameterResourceWriter = context.CreateODataUriParameterResourceWriter((IEdmNavigationSource) null, (IEdmStructuredType) null);
        ODataUriConversionUtils.WriteStartResource(parameterResourceWriter, resource);
        parameterResourceWriter.WriteEnd();
      }));
    }

    internal static string ConvertToUriEntitiesLiteral(
      IEnumerable<ODataResourceBase> entries,
      IEdmModel model)
    {
      ExceptionUtils.CheckArgumentNotNull<IEnumerable<ODataResourceBase>>(entries, nameof (entries));
      ExceptionUtils.CheckArgumentNotNull<IEdmModel>(model, nameof (model));
      return ODataUriConversionUtils.ConvertToJsonLightLiteral(model, (Action<ODataOutputContext>) (context =>
      {
        ODataWriter resourceSetWriter = context.CreateODataUriParameterResourceSetWriter((IEdmEntitySetBase) null, (IEdmStructuredType) null);
        resourceSetWriter.WriteStart(new ODataResourceSet());
        foreach (ODataResourceBase entry in entries)
        {
          ODataUriConversionUtils.WriteStartResource(resourceSetWriter, entry);
          resourceSetWriter.WriteEnd();
        }
        resourceSetWriter.WriteEnd();
      }));
    }

    internal static string ConvertToUriEntityReferenceLiteral(
      ODataEntityReferenceLink link,
      IEdmModel model)
    {
      ExceptionUtils.CheckArgumentNotNull<ODataEntityReferenceLink>(link, nameof (link));
      ExceptionUtils.CheckArgumentNotNull<IEdmModel>(model, nameof (model));
      return ODataUriConversionUtils.ConvertToJsonLightLiteral(model, (Action<ODataOutputContext>) (context => context.WriteEntityReferenceLink(link)));
    }

    internal static string ConvertToUriEntityReferencesLiteral(
      ODataEntityReferenceLinks links,
      IEdmModel model)
    {
      ExceptionUtils.CheckArgumentNotNull<ODataEntityReferenceLinks>(links, nameof (links));
      ExceptionUtils.CheckArgumentNotNull<IEdmModel>(model, nameof (model));
      return ODataUriConversionUtils.ConvertToJsonLightLiteral(model, (Action<ODataOutputContext>) (context => context.WriteEntityReferenceLinks(links)));
    }

    internal static string ConvertToResourceLiteral(
      ODataResourceValue resourceValue,
      IEdmModel model,
      ODataVersion version)
    {
      ExceptionUtils.CheckArgumentNotNull<ODataResourceValue>(resourceValue, nameof (resourceValue));
      ExceptionUtils.CheckArgumentNotNull<IEdmModel>(model, nameof (model));
      StringBuilder sb = new StringBuilder();
      using (TextWriter textWriter = (TextWriter) new StringWriter(sb, (IFormatProvider) CultureInfo.InvariantCulture))
      {
        ODataMessageWriterSettings messageWriterSettings = new ODataMessageWriterSettings()
        {
          Version = new ODataVersion?(version),
          Validations = ~ValidationKinds.ThrowOnUndeclaredPropertyForNonOpenType,
          ShouldIncludeAnnotation = ODataUtils.CreateAnnotationFilter("*")
        };
        ODataUriConversionUtils.WriteJsonLightLiteral(model, messageWriterSettings, textWriter, (Action<ODataJsonLightValueSerializer>) (serializer => serializer.WriteResourceValue(resourceValue, (IEdmTypeReference) null, true, false, serializer.CreateDuplicatePropertyNameChecker())));
      }
      return sb.ToString();
    }

    internal static string ConvertToUriCollectionLiteral(
      ODataCollectionValue collectionValue,
      IEdmModel model,
      ODataVersion version)
    {
      ExceptionUtils.CheckArgumentNotNull<ODataCollectionValue>(collectionValue, nameof (collectionValue));
      ExceptionUtils.CheckArgumentNotNull<IEdmModel>(model, nameof (model));
      StringBuilder sb = new StringBuilder();
      using (TextWriter textWriter = (TextWriter) new StringWriter(sb, (IFormatProvider) CultureInfo.InvariantCulture))
      {
        ODataMessageWriterSettings messageWriterSettings = new ODataMessageWriterSettings()
        {
          Version = new ODataVersion?(version),
          Validations = ~ValidationKinds.ThrowOnUndeclaredPropertyForNonOpenType,
          ShouldIncludeAnnotation = ODataUtils.CreateAnnotationFilter("*")
        };
        ODataUriConversionUtils.WriteJsonLightLiteral(model, messageWriterSettings, textWriter, (Action<ODataJsonLightValueSerializer>) (serializer => serializer.WriteCollectionValue(collectionValue, (IEdmTypeReference) null, (IEdmTypeReference) null, false, true, false, false)));
      }
      return sb.ToString();
    }

    [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Centralized method for coercing numeric types in easiest to understand.")]
    internal static object CoerceNumericType(object primitiveValue, IEdmPrimitiveType targetEdmType)
    {
      ExceptionUtils.CheckArgumentNotNull<object>(primitiveValue, nameof (primitiveValue));
      ExceptionUtils.CheckArgumentNotNull<IEdmPrimitiveType>(targetEdmType, nameof (targetEdmType));
      EdmPrimitiveTypeKind primitiveKind = targetEdmType.PrimitiveKind;
      if (primitiveValue is sbyte)
      {
        switch (primitiveKind)
        {
          case EdmPrimitiveTypeKind.Decimal:
            return (object) Convert.ToDecimal((sbyte) primitiveValue);
          case EdmPrimitiveTypeKind.Double:
            return (object) Convert.ToDouble((sbyte) primitiveValue);
          case EdmPrimitiveTypeKind.Int16:
            return (object) Convert.ToInt16((sbyte) primitiveValue);
          case EdmPrimitiveTypeKind.Int32:
            return (object) Convert.ToInt32((sbyte) primitiveValue);
          case EdmPrimitiveTypeKind.Int64:
            return (object) Convert.ToInt64((sbyte) primitiveValue);
          case EdmPrimitiveTypeKind.SByte:
            return primitiveValue;
          case EdmPrimitiveTypeKind.Single:
            return (object) Convert.ToSingle((sbyte) primitiveValue);
        }
      }
      if (primitiveValue is byte)
      {
        switch (primitiveKind)
        {
          case EdmPrimitiveTypeKind.Byte:
            return primitiveValue;
          case EdmPrimitiveTypeKind.Decimal:
            return (object) Convert.ToDecimal((byte) primitiveValue);
          case EdmPrimitiveTypeKind.Double:
            return (object) Convert.ToDouble((byte) primitiveValue);
          case EdmPrimitiveTypeKind.Int16:
            return (object) Convert.ToInt16((byte) primitiveValue);
          case EdmPrimitiveTypeKind.Int32:
            return (object) Convert.ToInt32((byte) primitiveValue);
          case EdmPrimitiveTypeKind.Int64:
            return (object) Convert.ToInt64((byte) primitiveValue);
          case EdmPrimitiveTypeKind.Single:
            return (object) Convert.ToSingle((byte) primitiveValue);
        }
      }
      if (primitiveValue is short)
      {
        switch (primitiveKind)
        {
          case EdmPrimitiveTypeKind.Decimal:
            return (object) Convert.ToDecimal((short) primitiveValue);
          case EdmPrimitiveTypeKind.Double:
            return (object) Convert.ToDouble((short) primitiveValue);
          case EdmPrimitiveTypeKind.Int16:
            return primitiveValue;
          case EdmPrimitiveTypeKind.Int32:
            return (object) Convert.ToInt32((short) primitiveValue);
          case EdmPrimitiveTypeKind.Int64:
            return (object) Convert.ToInt64((short) primitiveValue);
          case EdmPrimitiveTypeKind.Single:
            return (object) Convert.ToSingle((short) primitiveValue);
        }
      }
      if (primitiveValue is int)
      {
        switch (primitiveKind)
        {
          case EdmPrimitiveTypeKind.Decimal:
            return (object) Convert.ToDecimal((int) primitiveValue);
          case EdmPrimitiveTypeKind.Double:
            return (object) Convert.ToDouble((int) primitiveValue);
          case EdmPrimitiveTypeKind.Int32:
            return primitiveValue;
          case EdmPrimitiveTypeKind.Int64:
            return (object) Convert.ToInt64((int) primitiveValue);
          case EdmPrimitiveTypeKind.Single:
            return (object) Convert.ToSingle((int) primitiveValue);
        }
      }
      if (primitiveValue is long)
      {
        switch (primitiveKind)
        {
          case EdmPrimitiveTypeKind.Decimal:
            return (object) Convert.ToDecimal((long) primitiveValue);
          case EdmPrimitiveTypeKind.Double:
            return (object) Convert.ToDouble((long) primitiveValue);
          case EdmPrimitiveTypeKind.Int64:
            return primitiveValue;
          case EdmPrimitiveTypeKind.Single:
            return (object) Convert.ToSingle((long) primitiveValue);
        }
      }
      if (primitiveValue is float)
      {
        switch (primitiveKind)
        {
          case EdmPrimitiveTypeKind.Decimal:
            return (object) Convert.ToDecimal((float) primitiveValue);
          case EdmPrimitiveTypeKind.Double:
            return (object) double.Parse(((float) primitiveValue).ToString("R", (IFormatProvider) CultureInfo.InvariantCulture), (IFormatProvider) CultureInfo.InvariantCulture);
          case EdmPrimitiveTypeKind.Single:
            return primitiveValue;
        }
      }
      if (primitiveValue is double)
      {
        switch (primitiveKind)
        {
          case EdmPrimitiveTypeKind.Decimal:
            Decimal result;
            return Decimal.TryParse(((double) primitiveValue).ToString("R", (IFormatProvider) CultureInfo.InvariantCulture), out result) ? (object) result : (object) Convert.ToDecimal((double) primitiveValue);
          case EdmPrimitiveTypeKind.Double:
            return primitiveValue;
        }
      }
      return primitiveValue is Decimal && primitiveKind == EdmPrimitiveTypeKind.Decimal ? primitiveValue : (object) null;
    }

    [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Centralized method for coercing temporal types in easiest to understand.")]
    internal static object CoerceTemporalType(
      object primitiveValue,
      IEdmPrimitiveType targetEdmType)
    {
      ExceptionUtils.CheckArgumentNotNull<object>(primitiveValue, nameof (primitiveValue));
      ExceptionUtils.CheckArgumentNotNull<IEdmPrimitiveType>(targetEdmType, nameof (targetEdmType));
      switch (targetEdmType.PrimitiveKind)
      {
        case EdmPrimitiveTypeKind.DateTimeOffset:
          if (primitiveValue is Date date)
            return (object) new DateTimeOffset(date.Year, date.Month, date.Day, 0, 0, 0, new TimeSpan(0L));
          break;
        case EdmPrimitiveTypeKind.Date:
          if (primitiveValue is string text)
            return (object) PlatformHelper.ConvertStringToDate(text);
          break;
      }
      return (object) null;
    }

    private static void WriteStartResource(ODataWriter writer, ODataResourceBase resource)
    {
      if (resource is ODataDeletedResource deletedResource)
        writer.WriteStart(deletedResource);
      else
        writer.WriteStart(resource as ODataResource);
    }

    private static void WriteJsonLightLiteral(
      IEdmModel model,
      ODataMessageWriterSettings messageWriterSettings,
      TextWriter textWriter,
      Action<ODataJsonLightValueSerializer> writeValue)
    {
      ODataMessageInfo messageInfo = new ODataMessageInfo()
      {
        Model = model,
        IsAsync = false,
        IsResponse = false
      };
      using (ODataJsonLightOutputContext outputContext = new ODataJsonLightOutputContext(textWriter, messageInfo, messageWriterSettings))
      {
        ODataJsonLightValueSerializer lightValueSerializer = new ODataJsonLightValueSerializer(outputContext);
        writeValue(lightValueSerializer);
      }
    }

    private static string ConvertToJsonLightLiteral(
      IEdmModel model,
      Action<ODataOutputContext> writeAction)
    {
      using (MemoryStream memoryStream = new MemoryStream())
      {
        ODataMessageWriterSettings messageWriterSettings = new ODataMessageWriterSettings()
        {
          Version = new ODataVersion?(ODataVersion.V4),
          Validations = ~ValidationKinds.ThrowOnUndeclaredPropertyForNonOpenType
        };
        ODataMediaType odataMediaType = new ODataMediaType("application", "json");
        using (ODataJsonLightOutputContext lightOutputContext = new ODataJsonLightOutputContext(new ODataMessageInfo()
        {
          MessageStream = (Stream) memoryStream,
          Encoding = Encoding.UTF8,
          IsAsync = false,
          IsResponse = false,
          MediaType = odataMediaType,
          Model = model
        }, messageWriterSettings))
        {
          writeAction((ODataOutputContext) lightOutputContext);
          memoryStream.Position = 0L;
          return new StreamReader((Stream) memoryStream).ReadToEnd();
        }
      }
    }

    private static object ConvertFromResourceOrCollectionValue(
      string value,
      IEdmModel model,
      IEdmTypeReference typeReference)
    {
      ODataMessageReaderSettings messageReaderSettings = new ODataMessageReaderSettings();
      messageReaderSettings.Validations &= ~ValidationKinds.ThrowOnUndeclaredPropertyForNonOpenType;
      using (StringReader stringReader = new StringReader(value))
      {
        ODataMessageInfo messageInfo = new ODataMessageInfo()
        {
          MediaType = new ODataMediaType("application", "json"),
          Model = model,
          IsResponse = false,
          IsAsync = false,
          MessageStream = (Stream) null
        };
        using (ODataJsonLightInputContext jsonLightInputContext = new ODataJsonLightInputContext((TextReader) stringReader, messageInfo, messageReaderSettings))
        {
          ODataJsonLightPropertyAndValueDeserializer valueDeserializer = new ODataJsonLightPropertyAndValueDeserializer(jsonLightInputContext);
          valueDeserializer.JsonReader.Read();
          object obj = valueDeserializer.ReadNonEntityValue((string) null, typeReference, (PropertyAndAnnotationCollector) null, (CollectionWithoutExpectedTypeValidator) null, true, false, false, (string) null);
          valueDeserializer.ReadPayloadEnd(false);
          return obj;
        }
      }
    }
  }
}
