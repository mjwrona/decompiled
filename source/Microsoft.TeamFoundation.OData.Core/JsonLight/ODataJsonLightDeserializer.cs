// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.JsonLight.ODataJsonLightDeserializer
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Edm;
using Microsoft.OData.Evaluation;
using Microsoft.OData.Json;
using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace Microsoft.OData.JsonLight
{
  internal abstract class ODataJsonLightDeserializer : ODataDeserializer
  {
    private readonly ODataJsonLightInputContext jsonLightInputContext;
    private IODataMetadataContext metadataContext;
    private ODataJsonLightContextUriParseResult contextUriParseResult;
    private ODataUri odataUri;
    private bool isODataUriRead;

    protected ODataJsonLightDeserializer(ODataJsonLightInputContext jsonLightInputContext)
      : base((ODataInputContext) jsonLightInputContext)
    {
      this.jsonLightInputContext = jsonLightInputContext;
    }

    internal ODataUri ODataUri
    {
      get
      {
        if (this.isODataUriRead)
          return this.odataUri;
        if (this.ContextUriParseResult == null || this.ContextUriParseResult.NavigationSource == null || !(this.ContextUriParseResult.NavigationSource is IEdmContainedEntitySet) || this.contextUriParseResult.Path == null)
        {
          this.isODataUriRead = true;
          return this.odataUri = (ODataUri) null;
        }
        this.odataUri = new ODataUri()
        {
          Path = this.ContextUriParseResult.Path
        };
        this.isODataUriRead = true;
        return this.odataUri;
      }
    }

    internal IODataMetadataContext MetadataContext => this.metadataContext ?? (this.metadataContext = (IODataMetadataContext) new ODataMetadataContext(this.ReadingResponse, (Func<IEdmStructuredType, bool>) null, this.JsonLightInputContext.EdmTypeResolver, this.Model, this.MetadataDocumentUri, this.ODataUri, this.JsonLightInputContext.MetadataLevel));

    internal BufferingJsonReader JsonReader => this.jsonLightInputContext.JsonReader;

    internal ODataJsonLightContextUriParseResult ContextUriParseResult => this.contextUriParseResult;

    internal ODataJsonLightInputContext JsonLightInputContext => this.jsonLightInputContext;

    protected Func<PropertyAndAnnotationCollector, string, object> ReadPropertyCustomAnnotationValue { get; set; }

    private Uri MetadataDocumentUri => this.ContextUriParseResult == null || !(this.ContextUriParseResult.MetadataDocumentUri != (Uri) null) ? (Uri) null : this.ContextUriParseResult.MetadataDocumentUri;

    internal static bool TryParsePropertyAnnotation(
      string propertyAnnotationName,
      out string propertyName,
      out string annotationName)
    {
      int length = propertyAnnotationName.IndexOf('@');
      if (length <= 0 || length == propertyAnnotationName.Length - 1)
      {
        propertyName = (string) null;
        annotationName = (string) null;
        return false;
      }
      propertyName = propertyAnnotationName.Substring(0, length);
      annotationName = propertyAnnotationName.Substring(length + 1);
      return true;
    }

    internal void ReadPayloadStart(
      ODataPayloadKind payloadKind,
      PropertyAndAnnotationCollector propertyAndAnnotationCollector,
      bool isReadingNestedPayload,
      bool allowEmptyPayload)
    {
      string contextUriFromPayload = this.ReadPayloadStartImplementation(payloadKind, propertyAndAnnotationCollector, isReadingNestedPayload, allowEmptyPayload);
      ODataJsonLightContextUriParseResult contextUriParseResult = (ODataJsonLightContextUriParseResult) null;
      if (!isReadingNestedPayload && payloadKind != ODataPayloadKind.Error && contextUriFromPayload != null)
        contextUriParseResult = ODataJsonLightContextUriParser.Parse(this.Model, contextUriFromPayload, payloadKind, this.MessageReaderSettings.ClientCustomTypeResolver, this.JsonLightInputContext.ReadingResponse || payloadKind == ODataPayloadKind.Delta, this.JsonLightInputContext.MessageReaderSettings.ThrowIfTypeConflictsWithMetadata);
      this.contextUriParseResult = contextUriParseResult;
    }

    internal Task ReadPayloadStartAsync(
      ODataPayloadKind payloadKind,
      PropertyAndAnnotationCollector propertyAndAnnotationCollector,
      bool isReadingNestedPayload,
      bool allowEmptyPayload)
    {
      return TaskUtils.GetTaskForSynchronousOperation((Action) (() =>
      {
        string contextUriFromPayload = this.ReadPayloadStartImplementation(payloadKind, propertyAndAnnotationCollector, isReadingNestedPayload, allowEmptyPayload);
        if (isReadingNestedPayload || payloadKind == ODataPayloadKind.Error || contextUriFromPayload == null)
          return;
        this.contextUriParseResult = ODataJsonLightContextUriParser.Parse(this.Model, contextUriFromPayload, payloadKind, this.MessageReaderSettings.ClientCustomTypeResolver, this.JsonLightInputContext.ReadingResponse);
      }));
    }

    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "isReadingNestedPayload", Justification = "The parameter is used in debug builds.")]
    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Needs to access this in debug only.")]
    internal void ReadPayloadEnd(bool isReadingNestedPayload)
    {
    }

    internal string ReadAndValidateAnnotationStringValue(string annotationName)
    {
      string propertyValue = this.JsonReader.ReadStringValue(annotationName);
      ODataJsonLightReaderUtils.ValidateAnnotationValue((object) propertyValue, annotationName);
      return propertyValue;
    }

    internal string ReadAnnotationStringValue(string annotationName) => this.JsonReader.ReadStringValue(annotationName);

    internal Uri ReadAnnotationStringValueAsUri(string annotationName)
    {
      string str = this.JsonReader.ReadStringValue(annotationName);
      if (str == null)
        return (Uri) null;
      return !this.ReadingResponse ? new Uri(str, UriKind.RelativeOrAbsolute) : this.ProcessUriFromPayload(str);
    }

    internal Uri ReadAndValidateAnnotationStringValueAsUri(string annotationName)
    {
      string str = this.ReadAndValidateAnnotationStringValue(annotationName);
      return !this.ReadingResponse ? new Uri(str, UriKind.RelativeOrAbsolute) : this.ProcessUriFromPayload(str);
    }

    internal long ReadAndValidateAnnotationAsLongForIeee754Compatible(string annotationName)
    {
      object propertyValue = this.JsonReader.ReadPrimitiveValue();
      ODataJsonLightReaderUtils.ValidateAnnotationValue(propertyValue, annotationName);
      if (propertyValue is string ^ this.JsonReader.IsIeee754Compatible)
        throw new ODataException(Microsoft.OData.Strings.ODataJsonReaderUtils_ConflictBetweenInputFormatAndParameter((object) "Edm.Int64"));
      return (long) ODataJsonLightReaderUtils.ConvertValue(propertyValue, EdmCoreModel.Instance.GetInt64(false), this.MessageReaderSettings, true, annotationName, this.JsonLightInputContext.PayloadValueConverter);
    }

    internal Uri ProcessUriFromPayload(string uriFromPayload)
    {
      Uri uri1 = new Uri(uriFromPayload, UriKind.RelativeOrAbsolute);
      Uri metadataDocumentUri = this.MetadataDocumentUri;
      Uri uri2 = this.JsonLightInputContext.ResolveUri(metadataDocumentUri, uri1);
      if (uri2 != (Uri) null)
        return uri2;
      if (!uri1.IsAbsoluteUri)
        uri1 = !(metadataDocumentUri == (Uri) null) ? UriUtils.UriToAbsoluteUri(metadataDocumentUri, uri1) : throw new ODataException(Microsoft.OData.Strings.ODataJsonLightDeserializer_RelativeUriUsedWithouODataMetadataAnnotation((object) uriFromPayload, (object) "odata.context"));
      return uri1;
    }

    internal void ProcessProperty(
      PropertyAndAnnotationCollector propertyAndAnnotationCollector,
      Func<string, object> readPropertyAnnotationValue,
      Action<ODataJsonLightDeserializer.PropertyParsingResult, string> handleProperty)
    {
      string parsedPropertyName;
      ODataJsonLightDeserializer.PropertyParsingResult property;
      for (property = this.ParseProperty(propertyAndAnnotationCollector, readPropertyAnnotationValue, out parsedPropertyName); property == ODataJsonLightDeserializer.PropertyParsingResult.CustomInstanceAnnotation && this.ShouldSkipCustomInstanceAnnotation(parsedPropertyName); property = this.ParseProperty(propertyAndAnnotationCollector, readPropertyAnnotationValue, out parsedPropertyName))
      {
        this.JsonReader.Read();
        this.JsonReader.SkipValue();
      }
      handleProperty(property, parsedPropertyName);
      if (property == ODataJsonLightDeserializer.PropertyParsingResult.EndOfObject || property == ODataJsonLightDeserializer.PropertyParsingResult.CustomInstanceAnnotation)
        return;
      propertyAndAnnotationCollector.MarkPropertyAsProcessed(parsedPropertyName);
    }

    [Conditional("DEBUG")]
    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Needs access to this in Debug only.")]
    internal void AssertJsonCondition(params JsonNodeType[] allowedNodeTypes)
    {
    }

    internal string ReadContextUriAnnotation(
      ODataPayloadKind payloadKind,
      PropertyAndAnnotationCollector propertyAndAnnotationCollector,
      bool failOnMissingContextUriAnnotation)
    {
      if (this.JsonReader.NodeType != JsonNodeType.Property)
      {
        if (!failOnMissingContextUriAnnotation || payloadKind == ODataPayloadKind.Unsupported)
          return (string) null;
        throw new ODataException(Microsoft.OData.Strings.ODataJsonLightDeserializer_ContextLinkNotFoundAsFirstProperty);
      }
      string propertyName = this.JsonReader.GetPropertyName();
      if (string.CompareOrdinal("@odata.context", propertyName) != 0 && !this.CompareSimplifiedODataAnnotation("@context", propertyName))
      {
        if (!failOnMissingContextUriAnnotation || payloadKind == ODataPayloadKind.Unsupported)
          return (string) null;
        throw new ODataException(Microsoft.OData.Strings.ODataJsonLightDeserializer_ContextLinkNotFoundAsFirstProperty);
      }
      propertyAndAnnotationCollector?.MarkPropertyAsProcessed(propertyName);
      int num = (int) this.JsonReader.ReadNext();
      return this.JsonReader.ReadStringValue();
    }

    protected bool CompareSimplifiedODataAnnotation(
      string simplifiedPropertyName,
      string propertyName)
    {
      return this.JsonLightInputContext.OptionalODataPrefix && string.CompareOrdinal(simplifiedPropertyName, propertyName) == 0;
    }

    protected string CompleteSimplifiedODataAnnotation(string annotationName)
    {
      if (this.JsonLightInputContext.OptionalODataPrefix && annotationName.IndexOf('.') == -1)
        annotationName = "odata." + annotationName;
      return annotationName;
    }

    private bool ShouldSkipCustomInstanceAnnotation(string annotationName) => (!(this is ODataJsonLightErrorDeserializer) || this.MessageReaderSettings.ShouldIncludeAnnotation != null) && this.MessageReaderSettings.ShouldSkipAnnotation(annotationName);

    private static bool IsInstanceAnnotation(string annotationName) => !string.IsNullOrEmpty(annotationName) && annotationName[0] == '@';

    private bool SkippedOverUnknownODataAnnotation(
      string annotationName,
      out object annotationValue)
    {
      if (ODataAnnotationNames.IsUnknownODataAnnotationName(annotationName))
      {
        annotationValue = this.ReadODataOrCustomInstanceAnnotationValue(annotationName);
        return true;
      }
      annotationValue = (object) null;
      return false;
    }

    private object ReadODataOrCustomInstanceAnnotationValue(string annotationName)
    {
      this.JsonReader.Read();
      object obj;
      if (this.JsonReader.NodeType != JsonNodeType.PrimitiveValue)
      {
        obj = (object) this.JsonReader.ReadAsUntypedOrNullValue();
      }
      else
      {
        obj = this.JsonReader.Value;
        this.JsonReader.SkipValue();
      }
      return obj;
    }

    private ODataJsonLightDeserializer.PropertyParsingResult ParseProperty(
      PropertyAndAnnotationCollector propertyAndAnnotationCollector,
      Func<string, object> readPropertyAnnotationValue,
      out string parsedPropertyName)
    {
      string p0 = (string) null;
      parsedPropertyName = (string) null;
      while (this.JsonReader.NodeType == JsonNodeType.Property)
      {
        string propertyName1 = this.JsonReader.GetPropertyName();
        string propertyName2;
        string annotationName;
        bool propertyAnnotation = ODataJsonLightDeserializer.TryParsePropertyAnnotation(propertyName1, out propertyName2, out annotationName);
        if (propertyAnnotation && string.CompareOrdinal(this.CompleteSimplifiedODataAnnotation(annotationName), "odata.delta") == 0)
        {
          this.JsonReader.Read();
          parsedPropertyName = propertyName2;
          return ODataJsonLightDeserializer.PropertyParsingResult.NestedDeltaResourceSet;
        }
        bool flag = false;
        if (!propertyAnnotation)
        {
          flag = ODataJsonLightDeserializer.IsInstanceAnnotation(propertyName1);
          propertyName2 = flag ? this.CompleteSimplifiedODataAnnotation(propertyName1.Substring(1)) : propertyName1;
        }
        if (parsedPropertyName != null && string.CompareOrdinal(parsedPropertyName, propertyName2) != 0)
        {
          if (ODataJsonLightReaderUtils.IsAnnotationProperty(parsedPropertyName))
            throw new ODataException(Microsoft.OData.Strings.ODataJsonLightDeserializer_AnnotationTargetingInstanceAnnotationWithoutValue((object) p0, (object) parsedPropertyName));
          return ODataJsonLightDeserializer.PropertyParsingResult.PropertyWithoutValue;
        }
        object annotationValue = (object) null;
        if (propertyAnnotation)
        {
          annotationName = this.CompleteSimplifiedODataAnnotation(annotationName);
          if (!ODataJsonLightReaderUtils.IsAnnotationProperty(propertyName2) && this.SkippedOverUnknownODataAnnotation(annotationName, out annotationValue))
          {
            propertyAndAnnotationCollector.AddODataPropertyAnnotation(propertyName2, annotationName, annotationValue);
          }
          else
          {
            parsedPropertyName = propertyName2;
            p0 = annotationName;
            this.ProcessPropertyAnnotation(propertyName2, annotationName, propertyAndAnnotationCollector, readPropertyAnnotationValue);
          }
        }
        else if (flag && this.SkippedOverUnknownODataAnnotation(propertyName2, out annotationValue))
        {
          propertyAndAnnotationCollector.AddODataScopeAnnotation(propertyName2, annotationValue);
        }
        else
        {
          parsedPropertyName = propertyName2;
          if (!flag && ODataJsonLightUtils.IsMetadataReferenceProperty(propertyName2))
            return ODataJsonLightDeserializer.PropertyParsingResult.MetadataReferenceProperty;
          if (!flag && !ODataJsonLightReaderUtils.IsAnnotationProperty(propertyName2))
            return ODataJsonLightDeserializer.PropertyParsingResult.PropertyWithValue;
          return flag && ODataJsonLightReaderUtils.IsODataAnnotationName(propertyName2) ? ODataJsonLightDeserializer.PropertyParsingResult.ODataInstanceAnnotation : ODataJsonLightDeserializer.PropertyParsingResult.CustomInstanceAnnotation;
        }
      }
      if (parsedPropertyName == null)
        return ODataJsonLightDeserializer.PropertyParsingResult.EndOfObject;
      if (ODataJsonLightReaderUtils.IsAnnotationProperty(parsedPropertyName))
        throw new ODataException(Microsoft.OData.Strings.ODataJsonLightDeserializer_AnnotationTargetingInstanceAnnotationWithoutValue((object) p0, (object) parsedPropertyName));
      return ODataJsonLightDeserializer.PropertyParsingResult.PropertyWithoutValue;
    }

    private void ProcessPropertyAnnotation(
      string annotatedPropertyName,
      string annotationName,
      PropertyAndAnnotationCollector propertyAndAnnotationCollector,
      Func<string, object> readPropertyAnnotationValue)
    {
      if (ODataJsonLightReaderUtils.IsAnnotationProperty(annotatedPropertyName) && string.CompareOrdinal(annotationName, "odata.type") != 0)
        throw new ODataException(Microsoft.OData.Strings.ODataJsonLightDeserializer_OnlyODataTypeAnnotationCanTargetInstanceAnnotation((object) annotationName, (object) annotatedPropertyName, (object) "odata.type"));
      this.ReadODataOrCustomInstanceAnnotationValue(annotatedPropertyName, annotationName, propertyAndAnnotationCollector, readPropertyAnnotationValue);
    }

    private void ReadODataOrCustomInstanceAnnotationValue(
      string annotatedPropertyName,
      string annotationName,
      PropertyAndAnnotationCollector propertyAndAnnotationCollector,
      Func<string, object> readPropertyAnnotationValue)
    {
      this.JsonReader.Read();
      if (ODataJsonLightReaderUtils.IsODataAnnotationName(annotationName))
        propertyAndAnnotationCollector.AddODataPropertyAnnotation(annotatedPropertyName, annotationName, readPropertyAnnotationValue(annotationName));
      else if (this.ShouldSkipCustomInstanceAnnotation(annotationName) || this is ODataJsonLightErrorDeserializer && this.MessageReaderSettings.ShouldIncludeAnnotation == null)
      {
        propertyAndAnnotationCollector.CheckIfPropertyOpenForAnnotations(annotatedPropertyName, annotationName);
        this.JsonReader.SkipValue();
      }
      else
        propertyAndAnnotationCollector.AddCustomPropertyAnnotation(annotatedPropertyName, annotationName, this.ReadPropertyCustomAnnotationValue(propertyAndAnnotationCollector, annotationName));
    }

    private string ReadPayloadStartImplementation(
      ODataPayloadKind payloadKind,
      PropertyAndAnnotationCollector propertyAndAnnotationCollector,
      bool isReadingNestedPayload,
      bool allowEmptyPayload)
    {
      if (!isReadingNestedPayload)
      {
        this.JsonReader.Read();
        if (allowEmptyPayload && this.JsonReader.NodeType == JsonNodeType.EndOfInput)
          return (string) null;
        this.JsonReader.ReadStartObject();
        if (payloadKind != ODataPayloadKind.Error)
        {
          bool readingResponse = this.jsonLightInputContext.ReadingResponse;
          return this.ReadContextUriAnnotation(payloadKind, propertyAndAnnotationCollector, readingResponse);
        }
      }
      return (string) null;
    }

    internal enum PropertyParsingResult
    {
      EndOfObject,
      PropertyWithValue,
      PropertyWithoutValue,
      ODataInstanceAnnotation,
      CustomInstanceAnnotation,
      MetadataReferenceProperty,
      NestedDeltaResourceSet,
    }
  }
}
