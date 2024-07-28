// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.JsonLight.ODataJsonLightErrorDeserializer
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Edm;
using Microsoft.OData.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.OData.JsonLight
{
  internal sealed class ODataJsonLightErrorDeserializer : ODataJsonLightDeserializer
  {
    internal ODataJsonLightErrorDeserializer(ODataJsonLightInputContext jsonLightInputContext)
      : base(jsonLightInputContext)
    {
    }

    internal ODataError ReadTopLevelError()
    {
      this.JsonReader.DisableInStreamErrorDetection = true;
      PropertyAndAnnotationCollector annotationCollector = this.CreatePropertyAndAnnotationCollector();
      try
      {
        this.ReadPayloadStart(ODataPayloadKind.Error, annotationCollector, false, false);
        return this.ReadTopLevelErrorImplementation();
      }
      finally
      {
        this.JsonReader.DisableInStreamErrorDetection = false;
      }
    }

    internal Task<ODataError> ReadTopLevelErrorAsync()
    {
      this.JsonReader.DisableInStreamErrorDetection = true;
      return this.ReadPayloadStartAsync(ODataPayloadKind.Error, this.CreatePropertyAndAnnotationCollector(), false, false).FollowOnSuccessWith<ODataError>((Func<Task, ODataError>) (t => this.ReadTopLevelErrorImplementation())).FollowAlwaysWith<ODataError>((Action<Task<ODataError>>) (t => this.JsonReader.DisableInStreamErrorDetection = false));
    }

    private ODataError ReadTopLevelErrorImplementation()
    {
      ODataError error = (ODataError) null;
      while (this.JsonReader.NodeType == JsonNodeType.Property)
      {
        string str = this.JsonReader.ReadPropertyName();
        if (string.CompareOrdinal("error", str) != 0)
          throw new ODataException(Microsoft.OData.Strings.ODataJsonErrorDeserializer_TopLevelErrorWithInvalidProperty((object) str));
        error = error == null ? new ODataError() : throw new ODataException(Microsoft.OData.Strings.ODataJsonReaderUtils_MultipleErrorPropertiesWithSameName((object) "error"));
        this.ReadODataErrorObject(error);
      }
      this.JsonReader.ReadEndObject();
      this.ReadPayloadEnd(false);
      return error;
    }

    private void ReadJsonObjectInErrorPayload(
      Action<string, PropertyAndAnnotationCollector> readPropertyWithValue)
    {
      PropertyAndAnnotationCollector propertyAndAnnotationCollector = this.CreatePropertyAndAnnotationCollector();
      this.JsonReader.ReadStartObject();
      while (this.JsonReader.NodeType == JsonNodeType.Property)
        this.ProcessProperty(propertyAndAnnotationCollector, new Func<string, object>(this.ReadErrorPropertyAnnotationValue), (Action<ODataJsonLightDeserializer.PropertyParsingResult, string>) ((propertyParsingResult, propertyName) =>
        {
          if (this.JsonReader.NodeType == JsonNodeType.Property)
            this.JsonReader.Read();
          switch (propertyParsingResult)
          {
            case ODataJsonLightDeserializer.PropertyParsingResult.PropertyWithValue:
              readPropertyWithValue(propertyName, propertyAndAnnotationCollector);
              break;
            case ODataJsonLightDeserializer.PropertyParsingResult.PropertyWithoutValue:
              throw new ODataException(Microsoft.OData.Strings.ODataJsonLightErrorDeserializer_PropertyAnnotationWithoutPropertyForError((object) propertyName));
            case ODataJsonLightDeserializer.PropertyParsingResult.ODataInstanceAnnotation:
              throw new ODataException(Microsoft.OData.Strings.ODataJsonLightErrorDeserializer_InstanceAnnotationNotAllowedInErrorPayload((object) propertyName));
            case ODataJsonLightDeserializer.PropertyParsingResult.CustomInstanceAnnotation:
              readPropertyWithValue(propertyName, propertyAndAnnotationCollector);
              break;
            case ODataJsonLightDeserializer.PropertyParsingResult.MetadataReferenceProperty:
              throw new ODataException(Microsoft.OData.Strings.ODataJsonLightPropertyAndValueDeserializer_UnexpectedMetadataReferenceProperty((object) propertyName));
          }
        }));
      this.JsonReader.ReadEndObject();
    }

    private object ReadErrorPropertyAnnotationValue(string propertyAnnotationName)
    {
      if (string.CompareOrdinal(propertyAnnotationName, "odata.type") != 0)
        throw new ODataException(Microsoft.OData.Strings.ODataJsonLightErrorDeserializer_PropertyAnnotationNotAllowedInErrorPayload((object) propertyAnnotationName));
      return (object) (ReaderUtils.AddEdmPrefixOfTypeName(ReaderUtils.RemovePrefixOfTypeName(this.JsonReader.ReadStringValue())) ?? throw new ODataException(Microsoft.OData.Strings.ODataJsonLightPropertyAndValueDeserializer_InvalidTypeName((object) propertyAnnotationName)));
    }

    private void ReadODataErrorObject(ODataError error) => this.ReadJsonObjectInErrorPayload((Action<string, PropertyAndAnnotationCollector>) ((propertyName, duplicationPropertyNameChecker) => this.ReadPropertyValueInODataErrorObject(error, propertyName, duplicationPropertyNameChecker)));

    private ODataInnerError ReadInnerError(int recursionDepth)
    {
      ValidationUtils.IncreaseAndValidateRecursionDepth(ref recursionDepth, this.MessageReaderSettings.MessageQuotas.MaxNestingDepth);
      ODataInnerError innerError = new ODataInnerError();
      this.ReadJsonObjectInErrorPayload((Action<string, PropertyAndAnnotationCollector>) ((propertyName, propertyAndAnnotationCollector) => this.ReadPropertyValueInInnerError(recursionDepth, innerError, propertyName)));
      return innerError;
    }

    private void ReadPropertyValueInInnerError(
      int recursionDepth,
      ODataInnerError innerError,
      string propertyName)
    {
      switch (propertyName)
      {
        case "message":
          innerError.Message = this.JsonReader.ReadStringValue("message");
          break;
        case "type":
          innerError.TypeName = this.JsonReader.ReadStringValue("type");
          break;
        case "stacktrace":
          innerError.StackTrace = this.JsonReader.ReadStringValue("stacktrace");
          break;
        case "internalexception":
          innerError.InnerError = this.ReadInnerError(recursionDepth);
          break;
        default:
          if (!innerError.Properties.ContainsKey(propertyName))
          {
            innerError.Properties.Add(propertyName, this.JsonReader.ReadODataValue());
            break;
          }
          innerError.Properties[propertyName] = this.JsonReader.ReadODataValue();
          break;
      }
    }

    private void ReadPropertyValueInODataErrorObject(
      ODataError error,
      string propertyName,
      PropertyAndAnnotationCollector duplicationPropertyNameChecker)
    {
      switch (propertyName)
      {
        case "code":
          error.ErrorCode = this.JsonReader.ReadStringValue("code");
          break;
        case "message":
          error.Message = this.JsonReader.ReadStringValue("message");
          break;
        case "target":
          error.Target = this.JsonReader.ReadStringValue("target");
          break;
        case "details":
          error.Details = this.ReadDetails();
          break;
        case "innererror":
          error.InnerError = this.ReadInnerError(0);
          break;
        default:
          if (!ODataJsonLightReaderUtils.IsAnnotationProperty(propertyName))
            throw new ODataException(Microsoft.OData.Strings.ODataJsonLightErrorDeserializer_TopLevelErrorValueWithInvalidProperty((object) propertyName));
          ODataJsonLightPropertyAndValueDeserializer valueDeserializer = new ODataJsonLightPropertyAndValueDeserializer(this.JsonLightInputContext);
          object payloadTypeName = (object) null;
          duplicationPropertyNameChecker.GetODataPropertyAnnotations(propertyName).TryGetValue("odata.type", out payloadTypeName);
          object objectToConvert = valueDeserializer.ReadNonEntityValue(payloadTypeName as string, (IEdmTypeReference) null, (PropertyAndAnnotationCollector) null, (CollectionWithoutExpectedTypeValidator) null, false, false, false, propertyName);
          error.GetInstanceAnnotations().Add(new ODataInstanceAnnotation(propertyName, objectToConvert.ToODataValue()));
          break;
      }
    }

    private ICollection<ODataErrorDetail> ReadDetails()
    {
      List<ODataErrorDetail> odataErrorDetailList = new List<ODataErrorDetail>();
      this.JsonReader.ReadStartArray();
      while (this.JsonReader.NodeType == JsonNodeType.StartObject)
      {
        ODataErrorDetail odataErrorDetail = this.ReadDetail();
        odataErrorDetailList.Add(odataErrorDetail);
      }
      this.JsonReader.ReadEndArray();
      return (ICollection<ODataErrorDetail>) odataErrorDetailList;
    }

    private ODataErrorDetail ReadDetail()
    {
      ODataErrorDetail detail = new ODataErrorDetail();
      this.ReadJsonObjectInErrorPayload((Action<string, PropertyAndAnnotationCollector>) ((propertyName, duplicationPropertyNameChecker) => this.ReadPropertyValueInODataErrorDetailObject(detail, propertyName)));
      return detail;
    }

    private void ReadPropertyValueInODataErrorDetailObject(
      ODataErrorDetail detail,
      string propertyName)
    {
      switch (propertyName)
      {
        case "code":
          detail.ErrorCode = this.JsonReader.ReadStringValue("code");
          break;
        case "message":
          detail.Message = this.JsonReader.ReadStringValue("message");
          break;
        case "target":
          detail.Target = this.JsonReader.ReadStringValue("target");
          break;
        default:
          this.JsonReader.SkipValue();
          break;
      }
    }
  }
}
