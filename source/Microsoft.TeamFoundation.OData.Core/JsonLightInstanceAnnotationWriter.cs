// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.JsonLightInstanceAnnotationWriter
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Edm;
using Microsoft.OData.Json;
using Microsoft.OData.JsonLight;
using Microsoft.OData.Metadata;
using System;
using System.Collections.Generic;

namespace Microsoft.OData
{
  internal sealed class JsonLightInstanceAnnotationWriter
  {
    private readonly ODataJsonLightValueSerializer valueSerializer;
    private readonly JsonLightTypeNameOracle typeNameOracle;
    private readonly IJsonWriter jsonWriter;
    private readonly JsonLightODataAnnotationWriter odataAnnotationWriter;
    private readonly IWriterValidator writerValidator;

    internal JsonLightInstanceAnnotationWriter(
      ODataJsonLightValueSerializer valueSerializer,
      JsonLightTypeNameOracle typeNameOracle)
    {
      this.valueSerializer = valueSerializer;
      this.typeNameOracle = typeNameOracle;
      this.jsonWriter = this.valueSerializer.JsonWriter;
      this.odataAnnotationWriter = new JsonLightODataAnnotationWriter(this.jsonWriter, valueSerializer.JsonLightOutputContext.OmitODataPrefix, this.valueSerializer.MessageWriterSettings.Version);
      this.writerValidator = this.valueSerializer.MessageWriterSettings.Validator;
    }

    internal void WriteInstanceAnnotations(
      IEnumerable<ODataInstanceAnnotation> instanceAnnotations,
      InstanceAnnotationWriteTracker tracker,
      bool ignoreFilter = false,
      string propertyName = null)
    {
      HashSet<string> stringSet = new HashSet<string>((IEqualityComparer<string>) StringComparer.Ordinal);
      foreach (ODataInstanceAnnotation instanceAnnotation in instanceAnnotations)
      {
        if (!stringSet.Add(instanceAnnotation.Name))
          throw new ODataException(Strings.JsonLightInstanceAnnotationWriter_DuplicateAnnotationNameInCollection((object) instanceAnnotation.Name));
        if (!tracker.IsAnnotationWritten(instanceAnnotation.Name) && (!ODataAnnotationNames.IsODataAnnotationName(instanceAnnotation.Name) || ODataAnnotationNames.IsUnknownODataAnnotationName(instanceAnnotation.Name)))
        {
          this.WriteInstanceAnnotation(instanceAnnotation, ignoreFilter, propertyName);
          tracker.MarkAnnotationWritten(instanceAnnotation.Name);
        }
      }
    }

    internal void WriteInstanceAnnotations(
      IEnumerable<ODataInstanceAnnotation> instanceAnnotations,
      string propertyName = null,
      bool isUndeclaredProperty = false)
    {
      if (isUndeclaredProperty)
      {
        foreach (ODataInstanceAnnotation instanceAnnotation in instanceAnnotations)
          this.WriteInstanceAnnotation(instanceAnnotation, true, propertyName);
      }
      else
        this.WriteInstanceAnnotations(instanceAnnotations, new InstanceAnnotationWriteTracker(), propertyName: propertyName);
    }

    internal void WriteInstanceAnnotationsForError(
      IEnumerable<ODataInstanceAnnotation> instanceAnnotations)
    {
      this.WriteInstanceAnnotations(instanceAnnotations, new InstanceAnnotationWriteTracker(), true);
    }

    internal void WriteInstanceAnnotation(
      ODataInstanceAnnotation instanceAnnotation,
      bool ignoreFilter = false,
      string propertyName = null)
    {
      string name = instanceAnnotation.Name;
      ODataValue odataValue = instanceAnnotation.Value;
      if (!ignoreFilter && this.valueSerializer.MessageWriterSettings.ShouldSkipAnnotation(name))
        return;
      IEdmTypeReference edmTypeReference1 = MetadataUtils.LookupTypeOfTerm(name, this.valueSerializer.Model);
      if (odataValue is ODataNullValue)
      {
        if (edmTypeReference1 != null && !edmTypeReference1.IsNullable)
          throw new ODataException(Strings.JsonLightInstanceAnnotationWriter_NullValueNotAllowedForInstanceAnnotation((object) instanceAnnotation.Name, (object) edmTypeReference1.FullName()));
        this.WriteInstanceAnnotationName(propertyName, name);
        this.valueSerializer.WriteNullValue();
      }
      else
      {
        bool flag = edmTypeReference1 == null;
        if (odataValue is ODataResourceValue resourceValue)
        {
          this.WriteInstanceAnnotationName(propertyName, name);
          this.valueSerializer.WriteResourceValue(resourceValue, edmTypeReference1, flag, false, this.valueSerializer.CreateDuplicatePropertyNameChecker());
        }
        else if (odataValue is ODataCollectionValue collectionValue)
        {
          IEdmTypeReference edmTypeReference2 = TypeNameOracle.ResolveAndValidateTypeForCollectionValue(this.valueSerializer.Model, edmTypeReference1, collectionValue, flag, this.writerValidator);
          string typeNameForWriting = this.typeNameOracle.GetValueTypeNameForWriting((ODataValue) collectionValue, edmTypeReference1, edmTypeReference2, flag);
          if (typeNameForWriting != null)
            this.odataAnnotationWriter.WriteODataTypePropertyAnnotation(name, typeNameForWriting);
          this.WriteInstanceAnnotationName(propertyName, name);
          this.valueSerializer.WriteCollectionValue(collectionValue, edmTypeReference1, edmTypeReference2, false, false, flag, false);
        }
        else if (odataValue is ODataUntypedValue odataUntypedValue)
        {
          this.WriteInstanceAnnotationName(propertyName, name);
          this.valueSerializer.WriteUntypedValue(odataUntypedValue);
        }
        else if (odataValue is ODataEnumValue odataEnumValue)
        {
          this.WriteInstanceAnnotationName(propertyName, name);
          this.valueSerializer.WriteEnumValue(odataEnumValue, edmTypeReference1);
        }
        else
        {
          ODataPrimitiveValue primitiveValue = odataValue as ODataPrimitiveValue;
          IEdmTypeReference edmTypeReference3 = TypeNameOracle.ResolveAndValidateTypeForPrimitiveValue(primitiveValue);
          string typeNameForWriting = this.typeNameOracle.GetValueTypeNameForWriting((ODataValue) primitiveValue, edmTypeReference1, edmTypeReference3, flag);
          if (typeNameForWriting != null)
            this.odataAnnotationWriter.WriteODataTypePropertyAnnotation(name, typeNameForWriting);
          this.WriteInstanceAnnotationName(propertyName, name);
          this.valueSerializer.WritePrimitiveValue(primitiveValue.Value, edmTypeReference3, edmTypeReference1);
        }
      }
    }

    private void WriteInstanceAnnotationName(string propertyName, string annotationName)
    {
      if (propertyName != null)
        this.jsonWriter.WritePropertyAnnotationName(propertyName, annotationName);
      else
        this.jsonWriter.WriteInstanceAnnotationName(annotationName);
    }
  }
}
