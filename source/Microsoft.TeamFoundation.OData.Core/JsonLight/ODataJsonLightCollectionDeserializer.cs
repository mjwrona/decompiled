// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.JsonLight.ODataJsonLightCollectionDeserializer
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Edm;
using Microsoft.OData.Json;
using Microsoft.OData.Metadata;
using System;

namespace Microsoft.OData.JsonLight
{
  internal sealed class ODataJsonLightCollectionDeserializer : 
    ODataJsonLightPropertyAndValueDeserializer
  {
    private readonly PropertyAndAnnotationCollector propertyAndAnnotationCollector;

    internal ODataJsonLightCollectionDeserializer(ODataJsonLightInputContext jsonLightInputContext)
      : base(jsonLightInputContext)
    {
      this.propertyAndAnnotationCollector = this.CreatePropertyAndAnnotationCollector();
    }

    internal ODataCollectionStart ReadCollectionStart(
      PropertyAndAnnotationCollector collectionStartPropertyAndAnnotationCollector,
      bool isReadingNestedPayload,
      IEdmTypeReference expectedItemTypeReference,
      out IEdmTypeReference actualItemTypeReference)
    {
      actualItemTypeReference = expectedItemTypeReference;
      ODataCollectionStart collectionStart = (ODataCollectionStart) null;
      if (isReadingNestedPayload)
      {
        collectionStart = new ODataCollectionStart()
        {
          Name = (string) null
        };
      }
      else
      {
        while (this.JsonReader.NodeType == JsonNodeType.Property)
        {
          IEdmTypeReference actualItemTypeRef = expectedItemTypeReference;
          this.ProcessProperty(collectionStartPropertyAndAnnotationCollector, new Func<string, object>(((ODataJsonLightPropertyAndValueDeserializer) this).ReadTypePropertyAnnotationValue), (Action<ODataJsonLightDeserializer.PropertyParsingResult, string>) ((propertyParsingResult, propertyName) =>
          {
            if (this.JsonReader.NodeType == JsonNodeType.Property)
              this.JsonReader.Read();
            switch (propertyParsingResult)
            {
              case ODataJsonLightDeserializer.PropertyParsingResult.EndOfObject:
                break;
              case ODataJsonLightDeserializer.PropertyParsingResult.PropertyWithValue:
                string str = string.CompareOrdinal("value", propertyName) == 0 ? ODataJsonLightPropertyAndValueDeserializer.ValidateDataPropertyTypeNameAnnotation(collectionStartPropertyAndAnnotationCollector, propertyName) : throw new ODataException(Microsoft.OData.Strings.ODataJsonLightPropertyAndValueDeserializer_InvalidTopLevelPropertyName((object) propertyName, (object) "value"));
                if (str != null)
                {
                  string collectionItemTypeName = EdmLibraryExtensions.GetCollectionItemTypeName(str);
                  if (collectionItemTypeName == null)
                    throw new ODataException(Microsoft.OData.Strings.ODataJsonLightCollectionDeserializer_InvalidCollectionTypeName((object) str));
                  Func<EdmTypeKind> typeKindFromPayloadFunc = (Func<EdmTypeKind>) (() =>
                  {
                    throw new ODataException(Microsoft.OData.Strings.General_InternalError((object) InternalErrorCodes.ODataJsonLightCollectionDeserializer_ReadCollectionStart_TypeKindFromPayloadFunc));
                  });
                  actualItemTypeRef = this.ReaderValidator.ResolvePayloadTypeNameAndComputeTargetType(EdmTypeKind.None, new bool?(), (IEdmType) null, expectedItemTypeReference, collectionItemTypeName, this.Model, typeKindFromPayloadFunc, out EdmTypeKind _, out ODataTypeAnnotation _);
                }
                collectionStart = new ODataCollectionStart()
                {
                  Name = (string) null
                };
                break;
              case ODataJsonLightDeserializer.PropertyParsingResult.PropertyWithoutValue:
                throw new ODataException(Microsoft.OData.Strings.ODataJsonLightPropertyAndValueDeserializer_TopLevelPropertyAnnotationWithoutProperty((object) propertyName));
              case ODataJsonLightDeserializer.PropertyParsingResult.ODataInstanceAnnotation:
                if (!ODataJsonLightCollectionDeserializer.IsValidODataAnnotationOfCollection(propertyName))
                  throw new ODataException(Microsoft.OData.Strings.ODataJsonLightPropertyAndValueDeserializer_UnexpectedAnnotationProperties((object) propertyName));
                this.JsonReader.SkipValue();
                break;
              case ODataJsonLightDeserializer.PropertyParsingResult.CustomInstanceAnnotation:
                this.JsonReader.SkipValue();
                break;
              case ODataJsonLightDeserializer.PropertyParsingResult.MetadataReferenceProperty:
                throw new ODataException(Microsoft.OData.Strings.ODataJsonLightPropertyAndValueDeserializer_UnexpectedMetadataReferenceProperty((object) propertyName));
              default:
                throw new ODataException(Microsoft.OData.Strings.General_InternalError((object) InternalErrorCodes.ODataJsonLightCollectionDeserializer_ReadCollectionStart));
            }
          }));
          actualItemTypeReference = actualItemTypeRef;
        }
        if (collectionStart == null)
          throw new ODataException(Microsoft.OData.Strings.ODataJsonLightCollectionDeserializer_ExpectedCollectionPropertyNotFound((object) "value"));
      }
      if (this.JsonReader.NodeType != JsonNodeType.StartArray)
        throw new ODataException(Microsoft.OData.Strings.ODataJsonLightCollectionDeserializer_CannotReadCollectionContentStart((object) this.JsonReader.NodeType));
      return collectionStart;
    }

    internal object ReadCollectionItem(
      IEdmTypeReference expectedItemTypeReference,
      CollectionWithoutExpectedTypeValidator collectionValidator)
    {
      return this.ReadNonEntityValue((string) null, expectedItemTypeReference, this.propertyAndAnnotationCollector, collectionValidator, true, false, false, (string) null);
    }

    internal void ReadCollectionEnd(bool isReadingNestedPayload)
    {
      this.JsonReader.ReadEndArray();
      if (isReadingNestedPayload)
        return;
      PropertyAndAnnotationCollector annotationCollector = this.CreatePropertyAndAnnotationCollector();
      while (this.JsonReader.NodeType == JsonNodeType.Property)
        this.ProcessProperty(annotationCollector, new Func<string, object>(((ODataJsonLightPropertyAndValueDeserializer) this).ReadTypePropertyAnnotationValue), (Action<ODataJsonLightDeserializer.PropertyParsingResult, string>) ((propertyParsingResult, propertyName) =>
        {
          if (this.JsonReader.NodeType == JsonNodeType.Property)
            this.JsonReader.Read();
          switch (propertyParsingResult)
          {
            case ODataJsonLightDeserializer.PropertyParsingResult.EndOfObject:
              break;
            case ODataJsonLightDeserializer.PropertyParsingResult.PropertyWithValue:
            case ODataJsonLightDeserializer.PropertyParsingResult.PropertyWithoutValue:
            case ODataJsonLightDeserializer.PropertyParsingResult.MetadataReferenceProperty:
              throw new ODataException(Microsoft.OData.Strings.ODataJsonLightCollectionDeserializer_CannotReadCollectionEnd((object) propertyName));
            case ODataJsonLightDeserializer.PropertyParsingResult.ODataInstanceAnnotation:
              if (!ODataJsonLightCollectionDeserializer.IsValidODataAnnotationOfCollection(propertyName))
                throw new ODataException(Microsoft.OData.Strings.ODataJsonLightCollectionDeserializer_CannotReadCollectionEnd((object) propertyName));
              this.JsonReader.SkipValue();
              break;
            case ODataJsonLightDeserializer.PropertyParsingResult.CustomInstanceAnnotation:
              this.JsonReader.SkipValue();
              break;
            default:
              throw new ODataException(Microsoft.OData.Strings.General_InternalError((object) InternalErrorCodes.ODataJsonLightCollectionDeserializer_ReadCollectionEnd));
          }
        }));
      this.JsonReader.ReadEndObject();
    }

    private static bool IsValidODataAnnotationOfCollection(string propertyName) => string.CompareOrdinal("odata.count", propertyName) == 0 || string.CompareOrdinal("odata.nextLink", propertyName) == 0;
  }
}
