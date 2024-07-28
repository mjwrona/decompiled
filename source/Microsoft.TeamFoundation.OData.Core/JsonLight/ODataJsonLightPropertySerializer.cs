// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.JsonLight.ODataJsonLightPropertySerializer
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Edm;
using Microsoft.OData.Evaluation;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Microsoft.OData.JsonLight
{
  internal class ODataJsonLightPropertySerializer : ODataJsonLightSerializer
  {
    private readonly ODataJsonLightValueSerializer jsonLightValueSerializer;
    private PropertySerializationInfo currentPropertyInfo;

    internal ODataJsonLightPropertySerializer(
      ODataJsonLightOutputContext jsonLightOutputContext,
      bool initContextUriBuilder = false)
      : base(jsonLightOutputContext, initContextUriBuilder)
    {
      this.jsonLightValueSerializer = new ODataJsonLightValueSerializer(this, initContextUriBuilder);
    }

    internal ODataJsonLightValueSerializer JsonLightValueSerializer => this.jsonLightValueSerializer;

    internal void WriteTopLevelProperty(ODataProperty property) => this.WriteTopLevelPayload((Action) (() =>
    {
      this.JsonWriter.StartObjectScope();
      ODataPayloadKind payloadKind = this.JsonLightOutputContext.MessageWriterSettings.IsIndividualProperty ? ODataPayloadKind.IndividualProperty : ODataPayloadKind.Property;
      if (!(this.JsonLightOutputContext.MetadataLevel is JsonNoMetadataLevel))
      {
        ODataContextUrlInfo contextInfo = ODataContextUrlInfo.Create(property.ODataValue, (ODataVersion) ((int) this.MessageWriterSettings.Version ?? 0), this.JsonLightOutputContext.MessageWriterSettings.ODataUri, this.Model);
        this.WriteContextUriProperty(payloadKind, (Func<ODataContextUrlInfo>) (() => contextInfo));
      }
      this.WriteProperty(property, (IEdmStructuredType) null, true, false, this.CreateDuplicatePropertyNameChecker(), (ODataResourceMetadataBuilder) null);
      this.JsonWriter.EndObjectScope();
    }));

    internal void WriteProperties(
      IEdmStructuredType owningType,
      IEnumerable<ODataProperty> properties,
      bool isComplexValue,
      bool omitNullValues,
      IDuplicatePropertyNameChecker duplicatePropertyNameChecker,
      ODataResourceMetadataBuilder metadataBuilder)
    {
      if (properties == null)
        return;
      foreach (ODataProperty property in properties)
        this.WriteProperty(property, owningType, false, omitNullValues && property.InstanceAnnotations.Count == 0, duplicatePropertyNameChecker, metadataBuilder);
    }

    [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Splitting the code would make the logic harder to understand; class coupling is only slightly above threshold.")]
    internal void WriteProperty(
      ODataProperty property,
      IEdmStructuredType owningType,
      bool isTopLevel,
      bool omitNullValues,
      IDuplicatePropertyNameChecker duplicatePropertyNameChecker,
      ODataResourceMetadataBuilder metadataBuilder)
    {
      this.WritePropertyInfo((ODataPropertyInfo) property, owningType, isTopLevel, duplicatePropertyNameChecker, metadataBuilder);
      ODataValue odataValue = property.ODataValue;
      bool flag = odataValue == null || odataValue is ODataNullValue;
      if (flag & omitNullValues && !this.currentPropertyInfo.IsTopLevel)
        return;
      if (odataValue is ODataUntypedValue untypedValue)
        this.WriteUntypedValue(untypedValue);
      else if (odataValue is ODataStreamReferenceValue streamInfo && !(this.JsonLightOutputContext.MetadataLevel is JsonNoMetadataLevel))
        this.WriteStreamValue((IODataStreamReferenceInfo) streamInfo, property.Name, metadataBuilder);
      else if (flag)
      {
        this.WriteNullProperty((ODataPropertyInfo) property);
      }
      else
      {
        bool isOpenPropertyType = this.IsOpenProperty((ODataPropertyInfo) property);
        switch (odataValue)
        {
          case ODataPrimitiveValue primitiveValue:
            this.WritePrimitiveProperty(primitiveValue, isOpenPropertyType);
            break;
          case ODataEnumValue enumValue:
            this.WriteEnumProperty(enumValue, isOpenPropertyType);
            break;
          case ODataResourceValue resourceValue:
            if (isTopLevel)
              throw new ODataException(Microsoft.OData.Strings.ODataMessageWriter_NotAllowedWriteTopLevelPropertyWithResourceValue((object) property.Name));
            this.WriteResourceProperty(property, resourceValue, isOpenPropertyType, omitNullValues);
            break;
          case ODataCollectionValue collectionValue:
            if (isTopLevel && collectionValue.Items != null && collectionValue.Items.Any<object>((Func<object, bool>) (i => i is ODataResourceValue)))
              throw new ODataException(Microsoft.OData.Strings.ODataMessageWriter_NotAllowedWriteTopLevelPropertyWithResourceValue((object) property.Name));
            this.WriteCollectionProperty(collectionValue, isOpenPropertyType, omitNullValues);
            break;
          case ODataBinaryStreamValue streamValue:
            this.WriteStreamProperty(streamValue, isOpenPropertyType);
            break;
        }
      }
    }

    internal void WritePropertyInfo(
      ODataPropertyInfo propertyInfo,
      IEdmStructuredType owningType,
      bool isTopLevel,
      IDuplicatePropertyNameChecker duplicatePropertyNameChecker,
      ODataResourceMetadataBuilder metadataBuilder)
    {
      WriterValidationUtils.ValidatePropertyNotNull(propertyInfo);
      string name = propertyInfo.Name;
      if (this.JsonLightOutputContext.MessageWriterSettings.Validations != ValidationKinds.None)
        WriterValidationUtils.ValidatePropertyName(name);
      if (!this.JsonLightOutputContext.PropertyCacheHandler.InResourceSetScope())
        this.currentPropertyInfo = new PropertySerializationInfo(this.JsonLightOutputContext.Model, name, owningType)
        {
          IsTopLevel = isTopLevel
        };
      else
        this.currentPropertyInfo = this.JsonLightOutputContext.PropertyCacheHandler.GetProperty(this.JsonLightOutputContext.Model, name, owningType);
      WriterValidationUtils.ValidatePropertyDefined(this.currentPropertyInfo, this.MessageWriterSettings.ThrowOnUndeclaredPropertyForNonOpenType);
      duplicatePropertyNameChecker.ValidatePropertyUniqueness(propertyInfo);
      if (this.currentPropertyInfo.MetadataType.IsUndeclaredProperty)
        this.WriteODataTypeAnnotation(propertyInfo, isTopLevel);
      this.WriteInstanceAnnotation(propertyInfo, isTopLevel, this.currentPropertyInfo.MetadataType.IsUndeclaredProperty);
      if (!(propertyInfo is ODataStreamPropertyInfo streamInfo) || this.JsonLightOutputContext.MetadataLevel is JsonNoMetadataLevel)
        return;
      this.WriteStreamValue((IODataStreamReferenceInfo) streamInfo, propertyInfo.Name, metadataBuilder);
    }

    private bool IsOpenProperty(ODataPropertyInfo property) => property.SerializationInfo == null ? !this.WritingResponse && this.currentPropertyInfo.MetadataType.OwningType == null || this.currentPropertyInfo.MetadataType.IsOpenProperty : property.SerializationInfo.PropertyKind == ODataPropertyKind.Open;

    private void WriteUntypedValue(ODataUntypedValue untypedValue)
    {
      this.JsonWriter.WriteName(this.currentPropertyInfo.WireName);
      this.jsonLightValueSerializer.WriteUntypedValue(untypedValue);
    }

    private void WriteStreamValue(
      IODataStreamReferenceInfo streamInfo,
      string propertyName,
      ODataResourceMetadataBuilder metadataBuilder)
    {
      WriterValidationUtils.ValidateStreamPropertyInfo(streamInfo, this.currentPropertyInfo.MetadataType.EdmProperty, propertyName, this.WritingResponse);
      this.WriteStreamInfo(propertyName, streamInfo);
      metadataBuilder?.MarkStreamPropertyProcessed(propertyName);
    }

    private void WriteInstanceAnnotation(
      ODataPropertyInfo property,
      bool isTopLevel,
      bool isUndeclaredProperty)
    {
      if (property.InstanceAnnotations.Count == 0)
        return;
      if (isTopLevel)
        this.InstanceAnnotationWriter.WriteInstanceAnnotations((IEnumerable<ODataInstanceAnnotation>) property.InstanceAnnotations);
      else
        this.InstanceAnnotationWriter.WriteInstanceAnnotations((IEnumerable<ODataInstanceAnnotation>) property.InstanceAnnotations, property.Name, isUndeclaredProperty);
    }

    private void WriteODataTypeAnnotation(ODataPropertyInfo property, bool isTopLevel)
    {
      if (property.TypeAnnotation == null || property.TypeAnnotation.TypeName == null)
        return;
      string typeName = property.TypeAnnotation.TypeName;
      if (EdmCoreModel.Instance.FindType(typeName) is IEdmPrimitiveType type && (type.PrimitiveKind == EdmPrimitiveTypeKind.String || type.PrimitiveKind == EdmPrimitiveTypeKind.Decimal || type.PrimitiveKind == EdmPrimitiveTypeKind.Boolean))
        return;
      if (isTopLevel)
        this.ODataAnnotationWriter.WriteODataTypeInstanceAnnotation(typeName);
      else
        this.ODataAnnotationWriter.WriteODataTypePropertyAnnotation(property.Name, typeName);
    }

    private void WriteStreamInfo(string propertyName, IODataStreamReferenceInfo streamInfo)
    {
      Uri editLink = streamInfo.EditLink;
      if (editLink != (Uri) null)
      {
        this.ODataAnnotationWriter.WritePropertyAnnotationName(propertyName, "odata.mediaEditLink");
        this.JsonWriter.WriteValue(this.UriToString(editLink));
      }
      Uri readLink = streamInfo.ReadLink;
      if (readLink != (Uri) null)
      {
        this.ODataAnnotationWriter.WritePropertyAnnotationName(propertyName, "odata.mediaReadLink");
        this.JsonWriter.WriteValue(this.UriToString(readLink));
      }
      string contentType = streamInfo.ContentType;
      if (contentType != null)
      {
        this.ODataAnnotationWriter.WritePropertyAnnotationName(propertyName, "odata.mediaContentType");
        this.JsonWriter.WriteValue(contentType);
      }
      string etag = streamInfo.ETag;
      if (etag == null)
        return;
      this.ODataAnnotationWriter.WritePropertyAnnotationName(propertyName, "odata.mediaEtag");
      this.JsonWriter.WriteValue(etag);
    }

    private void WriteNullProperty(ODataPropertyInfo property)
    {
      this.WriterValidator.ValidateNullPropertyValue(this.currentPropertyInfo.MetadataType.TypeReference, property.Name, this.currentPropertyInfo.IsTopLevel, this.Model);
      if (this.currentPropertyInfo.IsTopLevel)
      {
        if (this.JsonLightOutputContext.MessageWriterSettings.LibraryCompatibility < ODataLibraryCompatibility.Version7)
        {
          ODataVersion? version = this.JsonLightOutputContext.MessageWriterSettings.Version;
          ODataVersion odataVersion = ODataVersion.V401;
          if (version.GetValueOrDefault() < odataVersion & version.HasValue)
          {
            this.ODataAnnotationWriter.WriteInstanceAnnotationName("odata.null");
            this.JsonWriter.WriteValue(true);
            return;
          }
        }
        throw new ODataException(Microsoft.OData.Strings.ODataMessageWriter_CannotWriteTopLevelNull);
      }
      if (this.MessageWriterSettings.OmitNullValues)
        return;
      this.JsonWriter.WriteName(property.Name);
      this.JsonLightValueSerializer.WriteNullValue();
    }

    private void WriteResourceProperty(
      ODataProperty property,
      ODataResourceValue resourceValue,
      bool isOpenPropertyType,
      bool omitNullValues)
    {
      this.JsonWriter.WriteName(property.Name);
      this.JsonLightValueSerializer.WriteResourceValue(resourceValue, this.currentPropertyInfo.MetadataType.TypeReference, isOpenPropertyType, omitNullValues, this.CreateDuplicatePropertyNameChecker());
    }

    private void WriteEnumProperty(ODataEnumValue enumValue, bool isOpenPropertyType)
    {
      this.ResolveEnumValueTypeName(enumValue, isOpenPropertyType);
      this.WritePropertyTypeName();
      this.JsonWriter.WriteName(this.currentPropertyInfo.WireName);
      this.JsonLightValueSerializer.WriteEnumValue(enumValue, this.currentPropertyInfo.MetadataType.TypeReference);
    }

    private void ResolveEnumValueTypeName(ODataEnumValue enumValue, bool isOpenPropertyType)
    {
      if (this.currentPropertyInfo.ValueType == null || this.currentPropertyInfo.ValueType.TypeName != enumValue.TypeName)
      {
        IEdmTypeReference edmTypeReference = TypeNameOracle.ResolveAndValidateTypeForEnumValue(this.Model, enumValue, isOpenPropertyType);
        bool flag = string.Equals(this.JsonLightOutputContext.Model.GetType().Name, "ClientEdmModel", StringComparison.OrdinalIgnoreCase);
        string typeNameForWriting = this.JsonLightOutputContext.TypeNameOracle.GetValueTypeNameForWriting((ODataValue) enumValue, this.currentPropertyInfo.MetadataType.TypeReference, edmTypeReference, flag | isOpenPropertyType);
        this.currentPropertyInfo.ValueType = new PropertyValueTypeInfo(enumValue.TypeName, edmTypeReference);
        this.currentPropertyInfo.TypeNameToWrite = typeNameForWriting;
      }
      else
      {
        string propertyName;
        if (!TypeNameOracle.TryGetTypeNameFromAnnotation((ODataValue) enumValue, out propertyName))
          return;
        this.currentPropertyInfo.TypeNameToWrite = propertyName;
      }
    }

    private void WriteCollectionProperty(
      ODataCollectionValue collectionValue,
      bool isOpenPropertyType,
      bool omitNullValues)
    {
      this.ResolveCollectionValueTypeName(collectionValue, isOpenPropertyType);
      this.WritePropertyTypeName();
      this.JsonWriter.WriteName(this.currentPropertyInfo.WireName);
      this.JsonLightValueSerializer.WriteCollectionValue(collectionValue, this.currentPropertyInfo.MetadataType.TypeReference, this.currentPropertyInfo.ValueType.TypeReference, this.currentPropertyInfo.IsTopLevel, false, isOpenPropertyType, omitNullValues);
    }

    private void ResolveCollectionValueTypeName(
      ODataCollectionValue collectionValue,
      bool isOpenPropertyType)
    {
      if (this.currentPropertyInfo.ValueType == null || this.currentPropertyInfo.ValueType.TypeName != collectionValue.TypeName)
      {
        IEdmTypeReference typeReference = TypeNameOracle.ResolveAndValidateTypeForCollectionValue(this.Model, this.currentPropertyInfo.MetadataType.TypeReference, collectionValue, isOpenPropertyType, this.WriterValidator);
        this.currentPropertyInfo.ValueType = new PropertyValueTypeInfo(collectionValue.TypeName, typeReference);
        this.currentPropertyInfo.TypeNameToWrite = this.JsonLightOutputContext.TypeNameOracle.GetValueTypeNameForWriting((ODataValue) collectionValue, this.currentPropertyInfo, isOpenPropertyType);
      }
      else
      {
        string propertyName;
        if (!TypeNameOracle.TryGetTypeNameFromAnnotation((ODataValue) collectionValue, out propertyName))
          return;
        this.currentPropertyInfo.TypeNameToWrite = propertyName;
      }
    }

    private void WriteStreamProperty(ODataBinaryStreamValue streamValue, bool isOpenPropertyType)
    {
      this.JsonWriter.WriteName(this.currentPropertyInfo.WireName);
      this.JsonLightValueSerializer.WriteStreamValue(streamValue);
    }

    private void WritePrimitiveProperty(ODataPrimitiveValue primitiveValue, bool isOpenPropertyType)
    {
      this.ResolvePrimitiveValueTypeName(primitiveValue, isOpenPropertyType);
      WriterValidationUtils.ValidatePropertyDerivedTypeConstraint(this.currentPropertyInfo);
      this.WritePropertyTypeName();
      this.JsonWriter.WriteName(this.currentPropertyInfo.WireName);
      this.JsonLightValueSerializer.WritePrimitiveValue(primitiveValue.Value, this.currentPropertyInfo.ValueType.TypeReference, this.currentPropertyInfo.MetadataType.TypeReference);
    }

    private void ResolvePrimitiveValueTypeName(
      ODataPrimitiveValue primitiveValue,
      bool isOpenPropertyType)
    {
      string name = primitiveValue.Value.GetType().Name;
      if (this.currentPropertyInfo.ValueType == null || this.currentPropertyInfo.ValueType.TypeName != name)
      {
        IEdmTypeReference typeReference = TypeNameOracle.ResolveAndValidateTypeForPrimitiveValue(primitiveValue);
        this.currentPropertyInfo.ValueType = new PropertyValueTypeInfo(name, typeReference);
        this.currentPropertyInfo.TypeNameToWrite = this.JsonLightOutputContext.TypeNameOracle.GetValueTypeNameForWriting((ODataValue) primitiveValue, this.currentPropertyInfo, isOpenPropertyType);
      }
      else
      {
        string propertyName;
        if (!TypeNameOracle.TryGetTypeNameFromAnnotation((ODataValue) primitiveValue, out propertyName))
          return;
        this.currentPropertyInfo.TypeNameToWrite = propertyName;
      }
    }

    private void WritePropertyTypeName()
    {
      string typeNameToWrite = this.currentPropertyInfo.TypeNameToWrite;
      if (typeNameToWrite == null)
        return;
      if (this.currentPropertyInfo.IsTopLevel)
        this.ODataAnnotationWriter.WriteODataTypeInstanceAnnotation(typeNameToWrite);
      else
        this.ODataAnnotationWriter.WriteODataTypePropertyAnnotation(this.currentPropertyInfo.PropertyName, typeNameToWrite);
    }
  }
}
