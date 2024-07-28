// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.ObjectTemplating.TemplateUtil
// Assembly: Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2DC134C9-663D-46C7-A414-3ADCC50BB112
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.dll

using Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.ObjectTemplating.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.ObjectTemplating
{
  internal static class TemplateUtil
  {
    internal static LiteralToken AssertLiteral(TemplateToken value, string objectDescription) => value is LiteralToken literalToken ? literalToken : throw new ArgumentException("Unexpected type '" + value?.GetType().Name + "' encountered while reading '" + objectDescription + "'. The type 'LiteralToken' was expected.");

    internal static MappingToken AssertMapping(TemplateToken value, string objectDescription) => value is MappingToken mappingToken ? mappingToken : throw new ArgumentException("Unexpected type '" + value?.GetType().Name + "' encountered while reading '" + objectDescription + "'. The type 'MappingToken' was expected.");

    internal static void AssertNotEmpty(MappingToken mapping, string objectDescription)
    {
      if (mapping.Count == 0)
        throw new ArgumentException("Unexpected empty mapping when reading '" + objectDescription + "'");
    }

    internal static SequenceToken AssertSequence(TemplateToken value, string objectDescription) => value is SequenceToken sequenceToken ? sequenceToken : throw new ArgumentException("Unexpected type '" + value?.GetType().Name + "' encountered while reading '" + objectDescription + "'. The type 'SequenceToken' was expected.");

    internal static void AssertUnexpectedValue(LiteralToken literal, string objectDescription) => throw new ArgumentException("Error while reading '" + objectDescription + "'. Unexpected value '" + literal.Value + "'");

    internal static JToken ConvertToJToken(TemplateToken value, bool parseLiterals = false)
    {
      JToken jtoken1;
      switch (value)
      {
        case LiteralToken token:
          if (parseLiterals)
          {
            if (token.Style.HasValue)
            {
              ScalarStyle? style = token.Style;
              ScalarStyle scalarStyle = ScalarStyle.Plain;
              if (!(style.GetValueOrDefault() == scalarStyle & style.HasValue))
                goto label_6;
            }
            jtoken1 = TemplateUtil.ParseLiteral(token);
            if (jtoken1.Type != JTokenType.Boolean && jtoken1.Type != JTokenType.Float && jtoken1.Type != JTokenType.Integer && jtoken1.Type != JTokenType.Null && jtoken1.Type != JTokenType.String)
            {
              jtoken1 = (JToken) token.Value;
              break;
            }
            break;
          }
label_6:
          jtoken1 = (JToken) (token.Value ?? string.Empty);
          break;
        case SequenceToken sequenceToken:
          JArray jarray = new JArray();
          foreach (TemplateToken templateToken in sequenceToken)
            jarray.Add(TemplateUtil.ConvertToJToken(templateToken, parseLiterals));
          jtoken1 = (JToken) jarray;
          break;
        case MappingToken mappingToken:
          JObject jobject = new JObject();
          foreach (KeyValuePair<ScalarToken, TemplateToken> keyValuePair in mappingToken)
          {
            string propertyName = (keyValuePair.Key as LiteralToken).Value ?? string.Empty;
            JToken jtoken2 = TemplateUtil.ConvertToJToken(keyValuePair.Value, parseLiterals);
            if (jtoken2 != null)
              jobject[propertyName] = jtoken2;
          }
          jtoken1 = (JToken) jobject;
          break;
        default:
          throw new InvalidOperationException("Internal error reading the template. Expected a scalar, a sequence, or a mapping");
      }
      return jtoken1;
    }

    internal static bool TryConvertToJToken(
      TemplateToken token,
      out JToken result,
      bool parseLiterals = false)
    {
      result = (JToken) null;
      try
      {
        result = TemplateUtil.ConvertToJToken(token, parseLiterals);
        return true;
      }
      catch (InvalidOperationException ex)
      {
      }
      return false;
    }

    internal static bool TryConvertToBooleanJToken(TemplateToken token, out JToken result)
    {
      result = (JToken) null;
      bool result1;
      if (!(token is LiteralToken literalToken) || string.IsNullOrEmpty(literalToken.Value) || !bool.TryParse(literalToken.Value.ToLower(), out result1))
        return false;
      result = (JToken) new JValue(result1);
      return true;
    }

    internal static bool TryConvertToNumberJToken(TemplateToken token, out JToken result)
    {
      result = (JToken) null;
      if (token is LiteralToken literalToken && !string.IsNullOrEmpty(literalToken.Value))
      {
        int result1;
        if (int.TryParse(literalToken.Value, out result1))
        {
          result = (JToken) new JValue((long) result1);
          return true;
        }
        Decimal result2;
        if (Decimal.TryParse(literalToken.Value, out result2))
        {
          result = (JToken) new JValue(result2);
          return true;
        }
      }
      return false;
    }

    internal static bool TryConvertToStringJToken(TemplateToken token, out JToken result)
    {
      result = (JToken) null;
      if (!(token is LiteralToken literalToken) || literalToken.Value == null)
        return false;
      result = (JToken) literalToken.Value;
      return true;
    }

    private static JToken ParseLiteral(LiteralToken token)
    {
      try
      {
        return JToken.Parse(token.Value);
      }
      catch (JsonReaderException ex)
      {
        return (JToken) token.Value;
      }
    }
  }
}
