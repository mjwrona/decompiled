// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.UriParser.SegmentArgumentParser
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Edm;
using Microsoft.OData.Metadata;
using System;
using System.Collections.Generic;

namespace Microsoft.OData.UriParser
{
  internal sealed class SegmentArgumentParser
  {
    private static readonly SegmentArgumentParser Empty = new SegmentArgumentParser();
    private readonly bool keysAsSegment;
    private readonly bool enableUriTemplateParsing;
    private Dictionary<string, string> namedValues;
    private List<string> positionalValues;

    private SegmentArgumentParser()
    {
    }

    private SegmentArgumentParser(
      Dictionary<string, string> namedValues,
      List<string> positionalValues,
      bool keysAsSegment,
      bool enableUriTemplateParsing)
    {
      this.namedValues = namedValues;
      this.positionalValues = positionalValues;
      this.keysAsSegment = keysAsSegment;
      this.enableUriTemplateParsing = enableUriTemplateParsing;
    }

    public bool AreValuesNamed => this.namedValues != null;

    public bool IsEmpty => this == SegmentArgumentParser.Empty;

    public IDictionary<string, string> NamedValues => (IDictionary<string, string>) this.namedValues;

    public IList<string> PositionalValues => (IList<string>) this.positionalValues;

    public bool KeyAsSegment => this.keysAsSegment;

    public int ValueCount
    {
      get
      {
        if (this == SegmentArgumentParser.Empty)
          return 0;
        return this.namedValues != null ? this.namedValues.Count : this.positionalValues.Count;
      }
    }

    public void AddNamedValue(string key, string value)
    {
      SegmentArgumentParser.CreateIfNull<Dictionary<string, string>>(ref this.namedValues);
      if (this.namedValues.ContainsKey(key))
        return;
      this.namedValues[key] = value;
    }

    public static bool TryParseKeysFromUri(
      string text,
      out SegmentArgumentParser instance,
      bool enableUriTemplateParsing)
    {
      return SegmentArgumentParser.TryParseFromUri(text, out instance, enableUriTemplateParsing);
    }

    public static SegmentArgumentParser FromSegment(
      string segmentText,
      bool enableUriTemplateParsing)
    {
      return new SegmentArgumentParser((Dictionary<string, string>) null, new List<string>()
      {
        segmentText
      }, true, (enableUriTemplateParsing ? 1 : 0) != 0);
    }

    public static bool TryParseNullableTokens(string text, out SegmentArgumentParser instance) => SegmentArgumentParser.TryParseFromUri(text, out instance, false);

    public bool TryConvertValues(
      IEdmEntityType targetEntityType,
      out IEnumerable<KeyValuePair<string, object>> keyPairs,
      ODataUriResolver resolver)
    {
      keyPairs = this.NamedValues == null ? resolver.ResolveKeys(targetEntityType, this.PositionalValues, new Func<IEdmTypeReference, string, object>(this.ConvertValueWrapper)) : resolver.ResolveKeys(targetEntityType, this.NamedValues, new Func<IEdmTypeReference, string, object>(this.ConvertValueWrapper));
      return true;
    }

    private object ConvertValueWrapper(IEdmTypeReference typeReference, string valueText)
    {
      object convertedValue;
      return !this.TryConvertValue(typeReference, valueText, out convertedValue) ? (object) null : convertedValue;
    }

    private bool TryConvertValue(
      IEdmTypeReference typeReference,
      string valueText,
      out object convertedValue)
    {
      UriTemplateExpression expression;
      if (this.enableUriTemplateParsing && UriTemplateParser.TryParseLiteral(valueText, typeReference, out expression))
      {
        convertedValue = (object) expression;
        return true;
      }
      if (typeReference.IsEnum())
      {
        QueryNode boundEnum = (QueryNode) null;
        if (EnumBinder.TryBindIdentifier(valueText, typeReference.AsEnum(), (IEdmModel) null, out boundEnum))
        {
          convertedValue = ((ConstantNode) boundEnum).Value;
          return true;
        }
        convertedValue = (object) null;
        return false;
      }
      IEdmPrimitiveTypeReference primitiveTypeReference = typeReference.AsPrimitive();
      Type primitiveClrType = EdmLibraryExtensions.GetPrimitiveClrType((IEdmPrimitiveType) primitiveTypeReference.Definition, primitiveTypeReference.IsNullable);
      return LiteralParser.ForKeys(this.keysAsSegment).TryParseLiteral(primitiveClrType, valueText, out convertedValue);
    }

    private static bool TryParseFromUri(
      string text,
      out SegmentArgumentParser instance,
      bool enableUriTemplateParsing)
    {
      Dictionary<string, string> namedValues = (Dictionary<string, string>) null;
      List<string> positionalValues = (List<string>) null;
      ExpressionLexer lexer = new ExpressionLexer("(" + text + ")", true, false);
      UriQueryExpressionParser parser = new UriQueryExpressionParser(800, lexer);
      FunctionParameterToken[] listOrEntityKeyList = new FunctionCallParser(lexer, parser).ParseArgumentListOrEntityKeyList();
      if (lexer.CurrentToken.Kind != ExpressionTokenKind.End)
      {
        instance = (SegmentArgumentParser) null;
        return false;
      }
      if (listOrEntityKeyList.Length == 0)
      {
        instance = SegmentArgumentParser.Empty;
        return true;
      }
      foreach (FunctionParameterToken functionParameterToken in listOrEntityKeyList)
      {
        string literalText = (string) null;
        if (functionParameterToken.ValueToken is LiteralToken valueToken2)
        {
          literalText = valueToken2.OriginalText;
          if (!enableUriTemplateParsing && UriTemplateParser.IsValidTemplateLiteral(literalText))
          {
            instance = (SegmentArgumentParser) null;
            return false;
          }
        }
        else if (functionParameterToken.ValueToken is DottedIdentifierToken valueToken1)
          literalText = valueToken1.Identifier;
        if (literalText != null)
        {
          if (functionParameterToken.ParameterName == null)
          {
            if (namedValues != null)
            {
              instance = (SegmentArgumentParser) null;
              return false;
            }
            SegmentArgumentParser.CreateIfNull<List<string>>(ref positionalValues);
            positionalValues.Add(literalText);
          }
          else
          {
            if (positionalValues != null)
            {
              instance = (SegmentArgumentParser) null;
              return false;
            }
            SegmentArgumentParser.CreateIfNull<Dictionary<string, string>>(ref namedValues);
            namedValues.Add(functionParameterToken.ParameterName, literalText);
          }
        }
        else
        {
          instance = (SegmentArgumentParser) null;
          return false;
        }
      }
      instance = new SegmentArgumentParser(namedValues, positionalValues, false, enableUriTemplateParsing);
      return true;
    }

    private static void CreateIfNull<T>(ref T value) where T : new()
    {
      if ((object) value != null)
        return;
      value = new T();
    }
  }
}
