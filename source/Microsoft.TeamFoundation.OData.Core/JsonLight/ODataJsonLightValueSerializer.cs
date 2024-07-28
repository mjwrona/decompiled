// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.JsonLight.ODataJsonLightValueSerializer
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Edm;
using Microsoft.OData.Evaluation;
using Microsoft.OData.Json;
using Microsoft.OData.Metadata;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace Microsoft.OData.JsonLight
{
  internal class ODataJsonLightValueSerializer : ODataJsonLightSerializer
  {
    private int recursionDepth;
    private ODataJsonLightPropertySerializer propertySerializer;

    internal ODataJsonLightValueSerializer(
      ODataJsonLightPropertySerializer propertySerializer,
      bool initContextUriBuilder = false)
      : base(propertySerializer.JsonLightOutputContext, initContextUriBuilder)
    {
      this.propertySerializer = propertySerializer;
    }

    internal ODataJsonLightValueSerializer(
      ODataJsonLightOutputContext outputContext,
      bool initContextUriBuilder = false)
      : base(outputContext, initContextUriBuilder)
    {
    }

    private ODataJsonLightPropertySerializer PropertySerializer
    {
      get
      {
        if (this.propertySerializer == null)
          this.propertySerializer = new ODataJsonLightPropertySerializer(this.JsonLightOutputContext);
        return this.propertySerializer;
      }
    }

    public virtual void WriteNullValue() => this.JsonWriter.WriteValue((string) null);

    public virtual void WriteEnumValue(
      ODataEnumValue value,
      IEdmTypeReference expectedTypeReference)
    {
      if (value.Value == null)
        this.WriteNullValue();
      else
        this.JsonWriter.WritePrimitiveValue((object) value.Value);
    }

    [SuppressMessage("Microsoft.Naming", "CA2204:LiteralsShouldBeSpelledCorrectly", Justification = "Names are correct. String can't be localized after string freeze.")]
    public virtual void WriteResourceValue(
      ODataResourceValue resourceValue,
      IEdmTypeReference metadataTypeReference,
      bool isOpenPropertyType,
      bool omitNullValues,
      IDuplicatePropertyNameChecker duplicatePropertyNamesChecker)
    {
      this.IncreaseRecursionDepth();
      this.JsonWriter.StartObjectScope();
      string typeName = resourceValue.TypeName;
      if (metadataTypeReference == null && !this.WritingResponse && typeName == null && this.Model.IsUserModel())
        throw new ODataException(Microsoft.OData.Strings.ODataJsonLightPropertyAndValueSerializer_NoExpectedTypeOrTypeNameSpecifiedForResourceValueRequest);
      IEdmStructuredTypeReference structuredTypeReference = (IEdmStructuredTypeReference) TypeNameOracle.ResolveAndValidateTypeForResourceValue(this.Model, metadataTypeReference, resourceValue, isOpenPropertyType, this.WriterValidator);
      string typeNameForWriting = this.JsonLightOutputContext.TypeNameOracle.GetValueTypeNameForWriting((ODataValue) resourceValue, metadataTypeReference, (IEdmTypeReference) structuredTypeReference, isOpenPropertyType);
      if (typeNameForWriting != null)
        this.ODataAnnotationWriter.WriteODataTypeInstanceAnnotation(typeNameForWriting);
      this.InstanceAnnotationWriter.WriteInstanceAnnotations((IEnumerable<ODataInstanceAnnotation>) resourceValue.InstanceAnnotations);
      this.PropertySerializer.WriteProperties(structuredTypeReference == null ? (IEdmStructuredType) null : structuredTypeReference.StructuredDefinition(), resourceValue.Properties, true, omitNullValues, duplicatePropertyNamesChecker, (ODataResourceMetadataBuilder) null);
      this.JsonWriter.EndObjectScope();
      this.DecreaseRecursionDepth();
    }

    [SuppressMessage("Microsoft.Naming", "CA2204:LiteralsShouldBeSpelledCorrectly", Justification = "Names are correct. String can't be localized after string freeze.")]
    public virtual void WriteCollectionValue(
      ODataCollectionValue collectionValue,
      IEdmTypeReference metadataTypeReference,
      IEdmTypeReference valueTypeReference,
      bool isTopLevelProperty,
      bool isInUri,
      bool isOpenPropertyType,
      bool omitNullValues)
    {
      this.IncreaseRecursionDepth();
      string typeName = collectionValue.TypeName;
      if (isTopLevelProperty)
      {
        if (typeName == null)
          throw new ODataException(Microsoft.OData.Strings.ODataJsonLightValueSerializer_MissingTypeNameOnCollection);
      }
      else if (metadataTypeReference == null && !this.WritingResponse && typeName == null && this.Model.IsUserModel())
        throw new ODataException(Microsoft.OData.Strings.ODataJsonLightPropertyAndValueSerializer_NoExpectedTypeOrTypeNameSpecifiedForCollectionValueInRequest);
      if (valueTypeReference == null)
        valueTypeReference = TypeNameOracle.ResolveAndValidateTypeForCollectionValue(this.Model, metadataTypeReference, collectionValue, isOpenPropertyType, this.WriterValidator);
      bool flag = false;
      if (isInUri)
      {
        string typeNameForWriting = this.JsonLightOutputContext.TypeNameOracle.GetValueTypeNameForWriting((ODataValue) collectionValue, metadataTypeReference, valueTypeReference, isOpenPropertyType);
        if (!string.IsNullOrEmpty(typeNameForWriting))
        {
          flag = true;
          this.JsonWriter.StartObjectScope();
          this.ODataAnnotationWriter.WriteODataTypeInstanceAnnotation(typeNameForWriting);
          this.JsonWriter.WriteValuePropertyName();
        }
      }
      this.JsonWriter.StartArrayScope();
      IEnumerable items = (IEnumerable) collectionValue.Items;
      if (items != null)
      {
        IEdmTypeReference edmTypeReference = valueTypeReference == null ? (IEdmTypeReference) null : ((IEdmCollectionTypeReference) valueTypeReference).ElementType();
        IDuplicatePropertyNameChecker duplicatePropertyNamesChecker = (IDuplicatePropertyNameChecker) null;
        foreach (object obj in items)
        {
          ValidationUtils.ValidateCollectionItem(obj, edmTypeReference.IsNullable());
          switch (obj)
          {
            case ODataResourceValue resourceValue:
              if (duplicatePropertyNamesChecker == null)
                duplicatePropertyNamesChecker = this.CreateDuplicatePropertyNameChecker();
              this.WriteResourceValue(resourceValue, edmTypeReference, false, omitNullValues, duplicatePropertyNamesChecker);
              duplicatePropertyNamesChecker.Reset();
              continue;
            case ODataEnumValue odataEnumValue:
              this.WriteEnumValue(odataEnumValue, edmTypeReference);
              continue;
            case ODataUntypedValue odataUntypedValue:
              this.WriteUntypedValue(odataUntypedValue);
              continue;
            case null:
              this.WriteNullValue();
              continue;
            default:
              this.WritePrimitiveValue(obj, edmTypeReference);
              continue;
          }
        }
      }
      this.JsonWriter.EndArrayScope();
      if (flag)
        this.JsonWriter.EndObjectScope();
      this.DecreaseRecursionDepth();
    }

    public virtual void WritePrimitiveValue(object value, IEdmTypeReference expectedTypeReference) => this.WritePrimitiveValue(value, (IEdmTypeReference) null, expectedTypeReference);

    public virtual void WritePrimitiveValue(
      object value,
      IEdmTypeReference actualTypeReference,
      IEdmTypeReference expectedTypeReference)
    {
      if (actualTypeReference == null)
      {
        value = this.Model.ConvertToUnderlyingTypeIfUIntValue(value, expectedTypeReference);
        actualTypeReference = (IEdmTypeReference) EdmLibraryExtensions.GetPrimitiveTypeReference(value.GetType());
      }
      ODataPayloadValueConverter payloadValueConverter = this.JsonLightOutputContext.PayloadValueConverter;
      if (expectedTypeReference != null && payloadValueConverter.GetType() == typeof (ODataPayloadValueConverter))
        this.WriterValidator.ValidateIsExpectedPrimitiveType(value, (IEdmPrimitiveTypeReference) actualTypeReference, expectedTypeReference);
      value = payloadValueConverter.ConvertToPayloadValue(value, expectedTypeReference);
      if (actualTypeReference != null && actualTypeReference.IsSpatial())
        PrimitiveConverter.Instance.WriteJsonLight(value, this.JsonWriter);
      else
        this.JsonWriter.WritePrimitiveValue(value);
    }

    public virtual void WriteUntypedValue(ODataUntypedValue value)
    {
      if (string.IsNullOrEmpty(value.RawValue))
        throw new ODataException(Microsoft.OData.Strings.ODataJsonLightValueSerializer_MissingRawValueOnUntyped);
      this.JsonWriter.WriteRawValue(value.RawValue);
    }

    public virtual void WriteStreamValue(ODataBinaryStreamValue streamValue)
    {
      if (!(this.JsonWriter is IJsonStreamWriter jsonWriter))
      {
        this.JsonWriter.WritePrimitiveValue((object) new StreamReader(streamValue.Stream).ReadToEnd());
      }
      else
      {
        Stream destination = jsonWriter.StartStreamValueScope();
        streamValue.Stream.CopyTo(destination);
        destination.Flush();
        destination.Dispose();
        jsonWriter.EndStreamValueScope();
      }
    }

    [Conditional("DEBUG")]
    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "The this is needed in DEBUG build.")]
    internal void AssertRecursionDepthIsZero()
    {
    }

    private void IncreaseRecursionDepth() => ValidationUtils.IncreaseAndValidateRecursionDepth(ref this.recursionDepth, this.MessageWriterSettings.MessageQuotas.MaxNestingDepth);

    private void DecreaseRecursionDepth() => --this.recursionDepth;
  }
}
