// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.ODataUtils
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Edm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.OData
{
  public static class ODataUtils
  {
    private const string Version4NumberString = "4.0";
    private const string Version401NumberString = "4.01";

    public static ODataFormat SetHeadersForPayload(
      ODataMessageWriter messageWriter,
      ODataPayloadKind payloadKind)
    {
      ExceptionUtils.CheckArgumentNotNull<ODataMessageWriter>(messageWriter, nameof (messageWriter));
      return payloadKind != ODataPayloadKind.Unsupported ? messageWriter.SetHeaders(payloadKind) : throw new ArgumentException(Strings.ODataMessageWriter_CannotSetHeadersWithInvalidPayloadKind((object) payloadKind), nameof (payloadKind));
    }

    public static ODataFormat GetReadFormat(ODataMessageReader messageReader)
    {
      ExceptionUtils.CheckArgumentNotNull<ODataMessageReader>(messageReader, nameof (messageReader));
      return messageReader.GetFormat();
    }

    public static ODataNullValueBehaviorKind NullValueReadBehaviorKind(
      this IEdmModel model,
      IEdmProperty property)
    {
      ExceptionUtils.CheckArgumentNotNull<IEdmModel>(model, nameof (model));
      ExceptionUtils.CheckArgumentNotNull<IEdmProperty>(property, nameof (property));
      ODataEdmPropertyAnnotation annotationValue = model.GetAnnotationValue<ODataEdmPropertyAnnotation>((IEdmElement) property);
      return annotationValue != null ? annotationValue.NullValueReadBehaviorKind : ODataNullValueBehaviorKind.Default;
    }

    public static void SetNullValueReaderBehavior(
      this IEdmModel model,
      IEdmProperty property,
      ODataNullValueBehaviorKind nullValueReadBehaviorKind)
    {
      ExceptionUtils.CheckArgumentNotNull<IEdmModel>(model, nameof (model));
      ExceptionUtils.CheckArgumentNotNull<IEdmProperty>(property, nameof (property));
      ODataEdmPropertyAnnotation annotationValue = model.GetAnnotationValue<ODataEdmPropertyAnnotation>((IEdmElement) property);
      if (annotationValue == null)
      {
        if (nullValueReadBehaviorKind == ODataNullValueBehaviorKind.Default)
          return;
        ODataEdmPropertyAnnotation propertyAnnotation = new ODataEdmPropertyAnnotation()
        {
          NullValueReadBehaviorKind = nullValueReadBehaviorKind
        };
        model.SetAnnotationValue<ODataEdmPropertyAnnotation>((IEdmElement) property, propertyAnnotation);
      }
      else
        annotationValue.NullValueReadBehaviorKind = nullValueReadBehaviorKind;
    }

    public static string ODataVersionToString(ODataVersion version)
    {
      if (version == ODataVersion.V4)
        return "4.0";
      if (version == ODataVersion.V401)
        return "4.01";
      throw new ODataException(Strings.ODataUtils_UnsupportedVersionNumber);
    }

    public static ODataVersion StringToODataVersion(string version)
    {
      string str = version;
      ExceptionUtils.CheckArgumentStringNotNullOrEmpty(version, nameof (version));
      int length = str.IndexOf(';');
      if (length >= 0)
        str = str.Substring(0, length);
      switch (str.Trim())
      {
        case "4.0":
          return ODataVersion.V4;
        case "4.01":
          return ODataVersion.V401;
        default:
          throw new ODataException(Strings.ODataUtils_UnsupportedVersionHeader((object) version));
      }
    }

    public static Func<string, bool> CreateAnnotationFilter(string annotationFilter) => new Func<string, bool>(AnnotationFilter.Create(annotationFilter).Matches);

    public static ODataServiceDocument GenerateServiceDocument(this IEdmModel model)
    {
      ExceptionUtils.CheckArgumentNotNull<IEdmModel>(model, nameof (model));
      if (model.EntityContainer == null)
        throw new ODataException(Strings.ODataUtils_ModelDoesNotHaveContainer);
      return new ODataServiceDocument()
      {
        EntitySets = (IEnumerable<ODataEntitySetInfo>) model.EntityContainer.EntitySets().Select<IEdmEntitySet, ODataEntitySetInfo>((Func<IEdmEntitySet, ODataEntitySetInfo>) (entitySet =>
        {
          return new ODataEntitySetInfo()
          {
            Name = entitySet.Name,
            Title = entitySet.Name,
            Url = new Uri(entitySet.Name, UriKind.RelativeOrAbsolute)
          };
        })).ToList<ODataEntitySetInfo>(),
        Singletons = (IEnumerable<ODataSingletonInfo>) model.EntityContainer.Singletons().Select<IEdmSingleton, ODataSingletonInfo>((Func<IEdmSingleton, ODataSingletonInfo>) (singleton =>
        {
          return new ODataSingletonInfo()
          {
            Name = singleton.Name,
            Title = singleton.Name,
            Url = new Uri(singleton.Name, UriKind.RelativeOrAbsolute)
          };
        })).ToList<ODataSingletonInfo>(),
        FunctionImports = (IEnumerable<ODataFunctionImportInfo>) model.EntityContainer.OperationImports().OfType<IEdmFunctionImport>().Where<IEdmFunctionImport>((Func<IEdmFunctionImport, bool>) (functionImport => functionImport.IncludeInServiceDocument && !functionImport.Function.Parameters.Any<IEdmOperationParameter>())).Select<IEdmFunctionImport, ODataFunctionImportInfo>((Func<IEdmFunctionImport, ODataFunctionImportInfo>) (functionImport =>
        {
          return new ODataFunctionImportInfo()
          {
            Name = functionImport.Name,
            Title = functionImport.Name,
            Url = new Uri(functionImport.Name, UriKind.RelativeOrAbsolute)
          };
        })).ToList<ODataFunctionImportInfo>()
      };
    }

    public static string AppendDefaultHeaderValue(string headerName, string headerValue) => ODataUtils.AppendDefaultHeaderValue(headerName, headerValue, ODataVersion.V4);

    public static string AppendDefaultHeaderValue(
      string headerName,
      string headerValue,
      ODataVersion version)
    {
      if (string.CompareOrdinal(headerName, "Content-Type") != 0)
        return headerValue;
      if (headerValue == null)
        return (string) null;
      IList<KeyValuePair<ODataMediaType, string>> source = HttpUtils.MediaTypesFromString(headerValue);
      ODataMediaType key = source.Single<KeyValuePair<ODataMediaType, string>>().Key;
      Encoding encodingFromCharsetName = HttpUtils.GetEncodingFromCharsetName(source.Single<KeyValuePair<ODataMediaType, string>>().Value);
      if (string.CompareOrdinal(key.FullTypeName, "application/json") != 0)
        return headerValue;
      List<KeyValuePair<string, string>> parameters = new List<KeyValuePair<string, string>>();
      ODataMediaType odataMediaType = new ODataMediaType(key.Type, key.SubType, (IEnumerable<KeyValuePair<string, string>>) parameters);
      bool flag1 = false;
      bool flag2 = false;
      bool flag3 = false;
      if (key.Parameters != null)
      {
        foreach (KeyValuePair<string, string> parameter in key.Parameters)
        {
          parameters.Add(parameter);
          if (HttpUtils.IsMetadataParameter(parameter.Key))
            flag1 = true;
          if (HttpUtils.IsStreamingParameter(parameter.Key))
            flag2 = true;
          if (string.Compare(parameter.Key, "IEEE754Compatible", StringComparison.OrdinalIgnoreCase) == 0)
            flag3 = true;
        }
      }
      if (!flag1)
        parameters.Add(new KeyValuePair<string, string>(version < ODataVersion.V401 ? "odata.metadata" : "metadata", "minimal"));
      if (!flag2)
        parameters.Add(new KeyValuePair<string, string>(version < ODataVersion.V401 ? "odata.streaming" : "streaming", "true"));
      if (!flag3)
        parameters.Add(new KeyValuePair<string, string>("IEEE754Compatible", "false"));
      return odataMediaType.ToText(encodingFromCharsetName);
    }
  }
}
