// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.JsonLight.ODataJsonLightParameterDeserializer
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Edm;
using Microsoft.OData.Json;
using System;

namespace Microsoft.OData.JsonLight
{
  internal sealed class ODataJsonLightParameterDeserializer : 
    ODataJsonLightPropertyAndValueDeserializer
  {
    private static readonly Func<string, object> propertyAnnotationValueReader = (Func<string, object>) (annotationName =>
    {
      throw new ODataException(Microsoft.OData.Strings.ODataJsonLightParameterDeserializer_PropertyAnnotationForParameters);
    });
    private readonly ODataJsonLightParameterReader parameterReader;

    internal ODataJsonLightParameterDeserializer(
      ODataJsonLightParameterReader parameterReader,
      ODataJsonLightInputContext jsonLightInputContext)
      : base(jsonLightInputContext)
    {
      this.parameterReader = parameterReader;
    }

    internal bool ReadNextParameter(
      PropertyAndAnnotationCollector propertyAndAnnotationCollector)
    {
      bool parameterRead = false;
      if (this.JsonReader.NodeType == JsonNodeType.Property)
      {
        bool foundCustomInstanceAnnotation = false;
        this.ProcessProperty(propertyAndAnnotationCollector, ODataJsonLightParameterDeserializer.propertyAnnotationValueReader, (Action<ODataJsonLightDeserializer.PropertyParsingResult, string>) ((propertyParsingResult, parameterName) =>
        {
          if (this.JsonReader.NodeType == JsonNodeType.Property)
            this.JsonReader.Read();
          switch (propertyParsingResult)
          {
            case ODataJsonLightDeserializer.PropertyParsingResult.EndOfObject:
              break;
            case ODataJsonLightDeserializer.PropertyParsingResult.PropertyWithValue:
              IEdmTypeReference parameterTypeReference = this.parameterReader.GetParameterTypeReference(parameterName);
              object p1;
              ODataParameterReaderState state;
              switch (parameterTypeReference.TypeKind())
              {
                case EdmTypeKind.Primitive:
                  IEdmPrimitiveTypeReference primitiveTypeReference = parameterTypeReference.AsPrimitive();
                  p1 = ExtensionMethods.PrimitiveKind(primitiveTypeReference) != EdmPrimitiveTypeKind.Stream ? this.ReadNonEntityValue((string) null, (IEdmTypeReference) primitiveTypeReference, (PropertyAndAnnotationCollector) null, (CollectionWithoutExpectedTypeValidator) null, true, false, false, parameterName) : throw new ODataException(Microsoft.OData.Strings.ODataJsonLightParameterDeserializer_UnsupportedPrimitiveParameterType((object) parameterName, (object) ExtensionMethods.PrimitiveKind(primitiveTypeReference)));
                  state = ODataParameterReaderState.Value;
                  break;
                case EdmTypeKind.Entity:
                case EdmTypeKind.Complex:
                  p1 = (object) null;
                  state = ODataParameterReaderState.Resource;
                  break;
                case EdmTypeKind.Collection:
                  p1 = (object) null;
                  if (this.JsonReader.NodeType == JsonNodeType.PrimitiveValue)
                  {
                    p1 = this.JsonReader.ReadPrimitiveValue();
                    if (p1 != null)
                      throw new ODataException(Microsoft.OData.Strings.ODataJsonLightParameterDeserializer_NullCollectionExpected((object) JsonNodeType.PrimitiveValue, p1));
                    state = ODataParameterReaderState.Value;
                    break;
                  }
                  state = !((IEdmCollectionType) parameterTypeReference.Definition).ElementType.IsStructured() ? ODataParameterReaderState.Collection : ODataParameterReaderState.ResourceSet;
                  break;
                case EdmTypeKind.Enum:
                  p1 = this.ReadNonEntityValue((string) null, (IEdmTypeReference) parameterTypeReference.AsEnum(), (PropertyAndAnnotationCollector) null, (CollectionWithoutExpectedTypeValidator) null, true, false, false, parameterName);
                  state = ODataParameterReaderState.Value;
                  break;
                case EdmTypeKind.TypeDefinition:
                  p1 = this.ReadNonEntityValue((string) null, (IEdmTypeReference) parameterTypeReference.AsTypeDefinition(), (PropertyAndAnnotationCollector) null, (CollectionWithoutExpectedTypeValidator) null, true, false, false, parameterName);
                  state = ODataParameterReaderState.Value;
                  break;
                default:
                  throw new ODataException(Microsoft.OData.Strings.ODataJsonLightParameterDeserializer_UnsupportedParameterTypeKind((object) parameterName, (object) parameterTypeReference.TypeKind()));
              }
              parameterRead = true;
              this.parameterReader.EnterScope(state, parameterName, p1);
              break;
            case ODataJsonLightDeserializer.PropertyParsingResult.PropertyWithoutValue:
              throw new ODataException(Microsoft.OData.Strings.ODataJsonLightParameterDeserializer_PropertyAnnotationWithoutPropertyForParameters((object) parameterName));
            case ODataJsonLightDeserializer.PropertyParsingResult.ODataInstanceAnnotation:
              throw new ODataException(Microsoft.OData.Strings.ODataJsonLightPropertyAndValueDeserializer_UnexpectedAnnotationProperties((object) parameterName));
            case ODataJsonLightDeserializer.PropertyParsingResult.CustomInstanceAnnotation:
              this.JsonReader.SkipValue();
              foundCustomInstanceAnnotation = true;
              break;
            case ODataJsonLightDeserializer.PropertyParsingResult.MetadataReferenceProperty:
              throw new ODataException(Microsoft.OData.Strings.ODataJsonLightPropertyAndValueDeserializer_UnexpectedMetadataReferenceProperty((object) parameterName));
            default:
              throw new ODataException(Microsoft.OData.Strings.General_InternalError((object) InternalErrorCodes.ODataJsonLightParameterDeserializer_ReadNextParameter));
          }
        }));
        if (foundCustomInstanceAnnotation)
          return this.ReadNextParameter(propertyAndAnnotationCollector);
      }
      if (!parameterRead && this.JsonReader.NodeType == JsonNodeType.EndObject)
      {
        this.JsonReader.ReadEndObject();
        this.ReadPayloadEnd(false);
        if (this.parameterReader.State != ODataParameterReaderState.Start)
          this.parameterReader.PopScope(this.parameterReader.State);
        this.parameterReader.PopScope(ODataParameterReaderState.Start);
        this.parameterReader.EnterScope(ODataParameterReaderState.Completed, (string) null, (object) null);
      }
      return parameterRead;
    }
  }
}
