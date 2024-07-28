// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Json.ODataJsonWriterUtils
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.OData.Json
{
  internal static class ODataJsonWriterUtils
  {
    internal static void WriteError(
      IJsonWriter jsonWriter,
      Action<IEnumerable<ODataInstanceAnnotation>> writeInstanceAnnotationsDelegate,
      ODataError error,
      bool includeDebugInformation,
      int maxInnerErrorDepth,
      bool writingJsonLight)
    {
      string code;
      string message;
      ErrorUtils.GetErrorDetails(error, out code, out message);
      ODataInnerError innerError = includeDebugInformation ? error.InnerError : (ODataInnerError) null;
      ODataJsonWriterUtils.WriteError(jsonWriter, code, message, error.Target, (IEnumerable<ODataErrorDetail>) error.Details, innerError, (IEnumerable<ODataInstanceAnnotation>) error.GetInstanceAnnotations(), writeInstanceAnnotationsDelegate, maxInnerErrorDepth, writingJsonLight);
    }

    internal static void StartJsonPaddingIfRequired(
      IJsonWriter jsonWriter,
      ODataMessageWriterSettings settings)
    {
      if (!settings.HasJsonPaddingFunction())
        return;
      jsonWriter.WritePaddingFunctionName(settings.JsonPCallback);
      jsonWriter.StartPaddingFunctionScope();
    }

    internal static void EndJsonPaddingIfRequired(
      IJsonWriter jsonWriter,
      ODataMessageWriterSettings settings)
    {
      if (!settings.HasJsonPaddingFunction())
        return;
      jsonWriter.EndPaddingFunctionScope();
    }

    private static void WriteError(
      IJsonWriter jsonWriter,
      string code,
      string message,
      string target,
      IEnumerable<ODataErrorDetail> details,
      ODataInnerError innerError,
      IEnumerable<ODataInstanceAnnotation> instanceAnnotations,
      Action<IEnumerable<ODataInstanceAnnotation>> writeInstanceAnnotationsDelegate,
      int maxInnerErrorDepth,
      bool writingJsonLight)
    {
      jsonWriter.StartObjectScope();
      if (writingJsonLight)
        jsonWriter.WriteName("error");
      else
        jsonWriter.WriteName("error");
      jsonWriter.StartObjectScope();
      jsonWriter.WriteName(nameof (code));
      jsonWriter.WriteValue(code);
      jsonWriter.WriteName(nameof (message));
      jsonWriter.WriteValue(message);
      if (target != null)
      {
        jsonWriter.WriteName(nameof (target));
        jsonWriter.WriteValue(target);
      }
      if (details != null)
        ODataJsonWriterUtils.WriteErrorDetails(jsonWriter, details, nameof (details));
      if (innerError != null)
        ODataJsonWriterUtils.WriteInnerError(jsonWriter, innerError, "innererror", 0, maxInnerErrorDepth);
      if (writingJsonLight)
        writeInstanceAnnotationsDelegate(instanceAnnotations);
      jsonWriter.EndObjectScope();
      jsonWriter.EndObjectScope();
    }

    private static void WriteErrorDetails(
      IJsonWriter jsonWriter,
      IEnumerable<ODataErrorDetail> details,
      string odataErrorDetailsName)
    {
      jsonWriter.WriteName(odataErrorDetailsName);
      jsonWriter.StartArrayScope();
      foreach (ODataErrorDetail odataErrorDetail in details.Where<ODataErrorDetail>((Func<ODataErrorDetail, bool>) (d => d != null)))
      {
        jsonWriter.StartObjectScope();
        jsonWriter.WriteName("code");
        jsonWriter.WriteValue(odataErrorDetail.ErrorCode ?? string.Empty);
        if (odataErrorDetail.Target != null)
        {
          jsonWriter.WriteName("target");
          jsonWriter.WriteValue(odataErrorDetail.Target);
        }
        jsonWriter.WriteName("message");
        jsonWriter.WriteValue(odataErrorDetail.Message ?? string.Empty);
        jsonWriter.EndObjectScope();
      }
      jsonWriter.EndArrayScope();
    }

    private static void WriteInnerError(
      IJsonWriter jsonWriter,
      ODataInnerError innerError,
      string innerErrorPropertyName,
      int recursionDepth,
      int maxInnerErrorDepth)
    {
      ValidationUtils.IncreaseAndValidateRecursionDepth(ref recursionDepth, maxInnerErrorDepth);
      jsonWriter.WriteName(innerErrorPropertyName);
      jsonWriter.StartObjectScope();
      if (innerError.Properties != null)
      {
        foreach (KeyValuePair<string, ODataValue> property in (IEnumerable<KeyValuePair<string, ODataValue>>) innerError.Properties)
        {
          jsonWriter.WriteName(property.Key);
          if (property.Value is ODataNullValue && (property.Key == "message" || property.Key == "stacktrace" || property.Key == "type"))
            jsonWriter.WriteODataValue((ODataValue) new ODataPrimitiveValue((object) string.Empty));
          else
            jsonWriter.WriteODataValue(property.Value);
        }
      }
      if (innerError.InnerError != null)
        ODataJsonWriterUtils.WriteInnerError(jsonWriter, innerError.InnerError, "internalexception", recursionDepth, maxInnerErrorDepth);
      jsonWriter.EndObjectScope();
    }

    internal static void ODataValueToString(StringBuilder sb, ODataValue value)
    {
      if (value == null || value is ODataNullValue)
        sb.Append("null");
      if (value is ODataCollectionValue odataCollectionValue)
        ODataJsonWriterUtils.ODataCollectionValueToString(sb, odataCollectionValue);
      if (value is ODataResourceValue odataResourceValue)
        ODataJsonWriterUtils.ODataResourceValueToString(sb, odataResourceValue);
      if (!(value is ODataPrimitiveValue odataValue))
        return;
      if (odataValue.FromODataValue() is string)
        sb.Append("\"" + JsonValueUtils.GetEscapedJsonString(value.FromODataValue()?.ToString()) + "\"");
      else
        sb.Append(JsonValueUtils.GetEscapedJsonString(value.FromODataValue()?.ToString()));
    }

    private static void ODataCollectionValueToString(StringBuilder sb, ODataCollectionValue value)
    {
      bool flag = true;
      sb.Append("[");
      foreach (object obj in value.Items)
      {
        if (flag)
          flag = false;
        else
          sb.Append(",");
        if (!(obj is ODataValue odataValue))
          throw new ODataException(Strings.ODataJsonWriter_UnsupportedValueInCollection);
        ODataJsonWriterUtils.ODataValueToString(sb, odataValue);
      }
      sb.Append("]");
    }

    private static void ODataResourceValueToString(StringBuilder sb, ODataResourceValue value)
    {
      bool flag = true;
      sb.Append("{");
      foreach (ODataProperty property in value.Properties)
      {
        if (flag)
          flag = false;
        else
          sb.Append(",");
        sb.Append("\"").Append(property.Name).Append("\"").Append(":");
        ODataJsonWriterUtils.ODataValueToString(sb, property.ODataValue);
      }
      sb.Append("}");
    }
  }
}
