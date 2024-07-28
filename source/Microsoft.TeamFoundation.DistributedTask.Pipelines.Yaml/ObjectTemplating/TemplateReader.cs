// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.ObjectTemplating.TemplateReader
// Assembly: Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2DC134C9-663D-46C7-A414-3ADCC50BB112
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.dll

using Microsoft.TeamFoundation.DistributedTask.Expressions;
using Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.ObjectTemplating.Schemas;
using Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.ObjectTemplating.Tokens;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using YamlDotNet.Core;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.ObjectTemplating
{
  internal sealed class TemplateReader
  {
    private const string c_openExpression = "${{";
    private const string c_closeExpression = "}}";
    private readonly TemplateContext m_context;
    private readonly int? m_fileId;
    private readonly TemplateMemory m_memory;
    private readonly IObjectReader m_objectReader;
    private readonly TemplateSchema m_schema;
    private readonly bool m_includeFileContentInErrors;

    private TemplateReader(
      TemplateContext context,
      TemplateSchema schema,
      IObjectReader objectReader,
      int? fileId,
      bool includeFileContentInErrors)
    {
      this.m_context = context;
      this.m_schema = schema;
      this.m_memory = context.Memory;
      this.m_objectReader = objectReader;
      this.m_fileId = fileId;
      this.m_includeFileContentInErrors = includeFileContentInErrors;
    }

    internal static TemplateToken Read(
      TemplateContext context,
      string type,
      IObjectReader objectReader,
      int? fileId,
      bool includeFileContentInErrors,
      out int bytes)
    {
      return TemplateReader.Read(context, type, objectReader, fileId, context.Schema, includeFileContentInErrors, out bytes);
    }

    internal static TemplateToken Read(
      TemplateContext context,
      string type,
      IObjectReader objectReader,
      int? fileId,
      TemplateSchema schema,
      bool includeFileContentInErrors,
      out int bytes)
    {
      TemplateToken templateToken = (TemplateToken) null;
      TemplateReader templateReader = new TemplateReader(context, schema, objectReader, fileId, includeFileContentInErrors);
      int currentBytes = context.Memory.CurrentBytes;
      try
      {
        objectReader.ValidateStart();
        templateToken = templateReader.ReadValue(type, false);
        objectReader.ValidateEnd();
      }
      catch (SemanticErrorException ex)
      {
        string[] source = ex.Message.Split(':');
        string message = source.Length != 0 ? ((IEnumerable<string>) source).Last<string>() : ex.Message;
        int? line = ex.Start?.Line;
        int? column = ex.Start?.Column;
        context.Error(fileId, line, column, message);
      }
      catch (Exception ex)
      {
        context.Error(fileId, new int?(), new int?(), ex);
      }
      finally
      {
        bytes = context.Memory.CurrentBytes - currentBytes;
      }
      return templateToken;
    }

    private TemplateToken ReadValue(string type, bool allowExpressions, bool isSequenceItem = false)
    {
      this.m_memory.IncrementEvents();
      Definition definition = this.m_schema.GetDefinition(type);
      if (!allowExpressions)
        allowExpressions = definition.AllowExpressionsInSubTree;
      int? line;
      int? column;
      string scalar1;
      Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.ObjectTemplating.Tokens.ScalarStyle? style;
      if (this.m_objectReader.AllowScalar(out line, out column, out scalar1, out style))
      {
        ScalarToken scalar2 = this.ParseScalar(line, column, scalar1, style);
        this.m_memory.AddBytes((TemplateToken) scalar2);
        this.Validate(scalar2, definition.Schemas, allowExpressions);
        return (TemplateToken) scalar2;
      }
      if (this.m_objectReader.AllowSequenceStart(out line, out column))
      {
        this.m_memory.IncrementDepth();
        SequenceToken sequence = new SequenceToken(this.m_fileId, line, column);
        this.m_memory.AddBytes(sequence);
        SequenceSchema sequenceSchema = definition.Schemas.OfType<SequenceSchema>().FirstOrDefault<SequenceSchema>();
        if (sequenceSchema != null)
        {
          while (!this.m_objectReader.AllowSequenceEnd())
          {
            TemplateToken templateToken = this.ReadValue(sequenceSchema.ItemType, allowExpressions, true);
            sequence.Add(templateToken);
          }
        }
        else
        {
          this.m_context.Error((TemplateToken) sequence, YamlStrings.UnexpectedSequenceStart());
          while (!this.m_objectReader.AllowSequenceEnd())
            this.SkipValue();
        }
        this.m_memory.DecrementDepth();
        return (TemplateToken) sequence;
      }
      if (!this.m_objectReader.AllowMappingStart(out line, out column))
        throw new InvalidOperationException(YamlStrings.ExpectedScalarSequenceOrMapping());
      this.m_memory.IncrementDepth();
      MappingToken mapping = new MappingToken(this.m_fileId, line, column);
      this.m_memory.AddBytes(mapping);
      List<MappingSchema> mappingSchemas = new List<MappingSchema>(definition.Schemas.OfType<MappingSchema>());
      if (mappingSchemas.Count > 0)
      {
        if (!string.IsNullOrEmpty(mappingSchemas[0].FirstKey))
          this.HandleMappingWithSignificantFirstKey(type, definition, mappingSchemas, mapping, allowExpressions);
        else if (this.m_schema.HasProperties(mappingSchemas[0]))
          this.HandleMappingWithWellKnownProperties(type, definition, mappingSchemas, mapping, allowExpressions);
        else
          this.HandleMappingWithAllLooseProperties(type, definition, mappingSchemas[0], mapping, allowExpressions);
      }
      else if (allowExpressions & isSequenceItem && this.m_objectReader.AllowScalar(out line, out column, out scalar1, out style))
      {
        ScalarToken scalar3 = this.ParseScalar(line, column, scalar1, style);
        switch (scalar3.Type)
        {
          case 5:
          case 6:
          case 7:
          case 8:
            mapping.Add(scalar3, this.ReadValue("any", allowExpressions));
            while (this.m_objectReader.AllowScalar(out line, out column, out scalar1, out style))
            {
              this.m_context.Error((TemplateToken) scalar3, this.GetUnexpectedValueMessage(scalar1));
              this.SkipValue();
            }
            this.ExpectMappingEnd();
            break;
          default:
            this.m_context.Error((TemplateToken) mapping, YamlStrings.UnexpectedMappingStart());
            this.SkipValue();
            while (!this.m_objectReader.AllowMappingEnd())
            {
              this.SkipValue();
              this.SkipValue();
            }
            break;
        }
      }
      else
      {
        this.m_context.Error((TemplateToken) mapping, YamlStrings.UnexpectedMappingStart());
        while (!this.m_objectReader.AllowMappingEnd())
        {
          this.SkipValue();
          this.SkipValue();
        }
      }
      this.m_memory.DecrementDepth();
      return (TemplateToken) mapping;
    }

    private void HandleMappingWithSignificantFirstKey(
      string type,
      Definition definition,
      List<MappingSchema> mappingSchemas,
      MappingToken mapping,
      bool allowExpressions)
    {
      int? line;
      int? column;
      string scalar1;
      Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.ObjectTemplating.Tokens.ScalarStyle? style;
      if (!this.m_objectReader.AllowScalar(out line, out column, out scalar1, out style))
      {
        this.m_context.Error((TemplateToken) mapping, YamlStrings.ExpectedAtLeastOnePair());
        this.ExpectMappingEnd();
      }
      else
      {
        ScalarToken scalar2 = this.ParseScalar(line, column, scalar1, style);
        this.m_memory.AddBytes((TemplateToken) scalar2);
        if (!(scalar2 is LiteralToken key1))
        {
          if (!allowExpressions)
          {
            this.m_context.Error((TemplateToken) scalar2, YamlStrings.ExpressionNotAllowed());
            this.SkipValue();
          }
          else
            mapping.Add(scalar2, this.ReadValue("any", allowExpressions));
          while (!this.m_objectReader.AllowMappingEnd())
          {
            ScalarToken key = this.ExpectScalar();
            this.m_memory.AddBytes((TemplateToken) key);
            mapping.Add(key, this.ReadValue("any", allowExpressions));
          }
        }
        else
        {
          string firstValueType;
          string looseKeyType;
          string looseValueType;
          if (!this.m_schema.TryMatchFirstKey(mappingSchemas, key1.Value, out firstValueType, out looseKeyType, out looseValueType))
          {
            this.m_context.Error((TemplateToken) key1, this.GetUnexpectedValueMessage(key1.Value));
            this.SkipValue();
            while (!this.m_objectReader.AllowMappingEnd())
            {
              this.SkipValue();
              this.SkipValue();
            }
          }
          else
          {
            mapping.Add((ScalarToken) key1, this.ReadValue(firstValueType, allowExpressions));
            HashSet<string> stringSet = new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
            stringSet.Add(key1.Value);
            while (this.m_objectReader.AllowScalar(out line, out column, out scalar1, out style))
            {
              ScalarToken scalar3 = this.ParseScalar(line, column, scalar1, style);
              this.m_memory.AddBytes((TemplateToken) scalar3);
              if (!(scalar3 is LiteralToken literalToken))
              {
                if (!allowExpressions)
                {
                  this.m_context.Error((TemplateToken) scalar3, YamlStrings.ExpressionNotAllowed());
                  this.SkipValue();
                }
                else
                  mapping.Add(scalar3, this.ReadValue("any", allowExpressions));
              }
              else if (!stringSet.Add(literalToken.Value))
              {
                this.m_context.Error((TemplateToken) literalToken, YamlStrings.ValueAlreadyDefined((object) literalToken.Value));
                this.SkipValue();
              }
              else
              {
                string valueType;
                if (this.m_schema.TryMatchKey(mappingSchemas, literalToken.Value, out valueType))
                  mapping.Add((ScalarToken) literalToken, this.ReadValue(valueType, allowExpressions));
                else if (looseKeyType != null)
                {
                  this.Validate((ScalarToken) literalToken, this.m_schema.GetDefinition(looseKeyType).Schemas, allowExpressions);
                  mapping.Add((ScalarToken) literalToken, this.ReadValue(looseValueType, allowExpressions));
                }
                else
                {
                  this.m_context.Error((TemplateToken) literalToken, this.GetUnexpectedValueMessage(literalToken.Value));
                  this.SkipValue();
                }
              }
            }
            this.ExpectMappingEnd();
          }
        }
      }
    }

    private void HandleMappingWithWellKnownProperties(
      string type,
      Definition definition,
      List<MappingSchema> mappingSchemas,
      MappingToken mapping,
      bool allowExpressions)
    {
      string type1 = (string) null;
      string type2 = (string) null;
      if (!string.IsNullOrEmpty(mappingSchemas[0].LooseKeyType))
      {
        type1 = mappingSchemas[0].LooseKeyType;
        type2 = mappingSchemas[0].LooseValueType;
      }
      HashSet<string> stringSet = new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      int? line;
      int? column;
      string scalar1;
      Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.ObjectTemplating.Tokens.ScalarStyle? style;
      while (this.m_objectReader.AllowScalar(out line, out column, out scalar1, out style))
      {
        ScalarToken scalar2 = this.ParseScalar(line, column, scalar1, style);
        this.m_memory.AddBytes((TemplateToken) scalar2);
        if (!(scalar2 is LiteralToken literalToken))
        {
          if (!allowExpressions)
          {
            this.m_context.Error((TemplateToken) scalar2, YamlStrings.ExpressionNotAllowed());
            this.SkipValue();
          }
          else
            mapping.Add(scalar2, this.ReadValue("any", allowExpressions));
        }
        else if (!stringSet.Add(literalToken.Value))
        {
          this.m_context.Error((TemplateToken) literalToken, YamlStrings.ValueAlreadyDefined((object) literalToken.Value));
          this.SkipValue();
        }
        else
        {
          string valueType;
          if (this.m_schema.TryMatchKey(mappingSchemas, literalToken.Value, out valueType))
            mapping.Add((ScalarToken) literalToken, this.ReadValue(valueType, allowExpressions));
          else if (type1 != null)
          {
            this.Validate((ScalarToken) literalToken, this.m_schema.GetDefinition(type1).Schemas, allowExpressions);
            mapping.Add((ScalarToken) literalToken, this.ReadValue(type2, allowExpressions));
          }
          else
          {
            this.m_context.Error((TemplateToken) literalToken, this.GetUnexpectedValueMessage(literalToken.Value));
            this.SkipValue();
          }
        }
      }
      this.ExpectMappingEnd();
    }

    private void HandleMappingWithAllLooseProperties(
      string type,
      Definition definition,
      MappingSchema mappingSchema,
      MappingToken mapping,
      bool allowExpressions)
    {
      string looseKeyType = mappingSchema.LooseKeyType;
      string looseValueType = mappingSchema.LooseValueType;
      HashSet<string> stringSet = new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      int? line;
      int? column;
      string scalar1;
      Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.ObjectTemplating.Tokens.ScalarStyle? style;
      while (this.m_objectReader.AllowScalar(out line, out column, out scalar1, out style))
      {
        ScalarToken scalar2 = this.ParseScalar(line, column, scalar1, style);
        this.m_memory.AddBytes((TemplateToken) scalar2);
        if (!(scalar2 is LiteralToken literalToken))
        {
          if (!allowExpressions)
          {
            this.m_context.Error((TemplateToken) scalar2, YamlStrings.ExpressionNotAllowed());
            this.SkipValue();
          }
          else
            mapping.Add(scalar2, this.ReadValue("any", allowExpressions));
        }
        else if (!stringSet.Add(literalToken.Value))
        {
          this.m_context.Error((TemplateToken) literalToken, YamlStrings.ValueAlreadyDefined((object) literalToken.Value));
          this.SkipValue();
        }
        else
        {
          this.Validate((ScalarToken) literalToken, this.m_schema.GetDefinition(looseKeyType).Schemas, allowExpressions);
          mapping.Add((ScalarToken) literalToken, this.ReadValue(looseValueType, allowExpressions));
        }
      }
      this.ExpectMappingEnd();
    }

    private ScalarToken ExpectScalar()
    {
      int? line;
      int? column;
      string scalar;
      Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.ObjectTemplating.Tokens.ScalarStyle? style;
      if (this.m_objectReader.AllowScalar(out line, out column, out scalar, out style))
        return this.ParseScalar(line, column, scalar, style);
      throw new Exception(YamlStrings.ExpectedScalar());
    }

    private void ExpectMappingEnd()
    {
      if (!this.m_objectReader.AllowMappingEnd())
        throw new Exception("Expected mapping end");
    }

    private void SkipValue()
    {
      this.m_memory.IncrementEvents();
      int? nullable1;
      int? nullable2;
      if (this.m_objectReader.AllowScalar(out nullable1, out nullable2, out string _, out Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.ObjectTemplating.Tokens.ScalarStyle? _))
        return;
      if (this.m_objectReader.AllowSequenceStart(out nullable2, out nullable1))
      {
        this.m_memory.IncrementDepth();
        while (!this.m_objectReader.AllowSequenceEnd())
          this.SkipValue();
        this.m_memory.DecrementDepth();
      }
      else
      {
        if (!this.m_objectReader.AllowMappingStart(out nullable1, out nullable2))
          throw new InvalidOperationException(YamlStrings.ExpectedScalarSequenceOrMapping());
        this.m_memory.IncrementDepth();
        while (!this.m_objectReader.AllowMappingEnd())
        {
          this.SkipValue();
          this.SkipValue();
        }
        this.m_memory.DecrementDepth();
      }
    }

    private void Validate(ScalarToken scalar, List<Schema> schemas, bool allowExpressions)
    {
      switch (scalar.Type)
      {
        case 0:
          LiteralToken literal = scalar as LiteralToken;
          if (schemas.OfType<ScalarSchema>().Any<ScalarSchema>((Func<ScalarSchema, bool>) (x => x.IsMatch(literal))))
            break;
          this.m_context.Error((TemplateToken) literal, this.GetUnexpectedValueMessage(literal.Value));
          break;
        case 3:
          if (allowExpressions)
            break;
          this.m_context.Error((TemplateToken) scalar, YamlStrings.ExpressionNotAllowed());
          break;
        default:
          this.m_context.Error((TemplateToken) scalar, this.GetUnexpectedValueMessage(scalar.ToString()));
          break;
      }
    }

    private ScalarToken ParseScalar(int? line, int? column, string raw, Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.ObjectTemplating.Tokens.ScalarStyle? style)
    {
      int num1;
      if (string.IsNullOrEmpty(raw) || (num1 = raw.IndexOf("${{")) < 0)
        return (ScalarToken) new LiteralToken(this.m_fileId, line, column, raw, style);
      List<ScalarToken> segments = new List<ScalarToken>();
      int num2 = 0;
      while (num2 < raw.Length)
      {
        if (num2 == num1)
        {
          int num3 = num2;
          int num4 = -1;
          bool flag = false;
          for (num2 += "${{".Length; num2 < raw.Length; ++num2)
          {
            if (raw[num2] == '\'')
              flag = !flag;
            else if (!flag && raw[num2] == '}' && raw[num2 - 1] == '}')
            {
              num4 = num2;
              ++num2;
              break;
            }
          }
          if (num4 < num3)
          {
            this.m_context.Error(this.m_fileId, line, column, YamlStrings.ExpressionNotClosed());
            return (ScalarToken) new LiteralToken(this.m_fileId, line, column, raw);
          }
          string str = raw.Substring(num3 + "${{".Length, num4 - num3 + 1 - "${{".Length - "}}".Length);
          Exception ex;
          ExpressionToken expression = this.ParseExpression(line, column, str, out ex);
          if (ex != null)
          {
            this.m_context.Error(this.m_fileId, line, column, ex);
            return (ScalarToken) new LiteralToken(this.m_fileId, line, column, raw);
          }
          if (!string.IsNullOrEmpty(expression.Directive) && (num3 != 0 || num2 < raw.Length))
          {
            this.m_context.Error(this.m_fileId, line, column, YamlStrings.DirectiveNotAllowedInline((object) expression.Directive));
            return (ScalarToken) new LiteralToken(this.m_fileId, line, column, raw);
          }
          segments.Add((ScalarToken) expression);
          num1 = raw.IndexOf("${{", num2);
        }
        else if (num2 < num1)
        {
          this.AddLiteral(segments, line, column, raw.Substring(num2, num1 - num2));
          num2 = num1;
        }
        else
        {
          this.AddLiteral(segments, line, column, raw.Substring(num2));
          break;
        }
      }
      string str1;
      if (segments.Count == 1 && segments[0] is BasicExpressionToken basicExpressionToken1 && TemplateReader.IsExpressionString(basicExpressionToken1.Expression, out str1))
        return (ScalarToken) new LiteralToken(this.m_fileId, line, column, str1);
      if (segments.Count == 1)
        return segments[0];
      StringBuilder stringBuilder1 = new StringBuilder();
      StringBuilder stringBuilder2 = new StringBuilder();
      int num5 = 0;
      foreach (ScalarToken scalarToken in segments)
      {
        if (scalarToken is LiteralToken literalToken)
        {
          string str2 = ExpressionUtil.StringEscape(literalToken.Value).Replace("{", "{{").Replace("}", "}}");
          stringBuilder1.Append(str2);
        }
        else
        {
          stringBuilder1.Append("{" + num5.ToString((IFormatProvider) CultureInfo.InvariantCulture) + "}");
          ++num5;
          BasicExpressionToken basicExpressionToken2 = scalarToken as BasicExpressionToken;
          stringBuilder2.Append(", ");
          stringBuilder2.Append(basicExpressionToken2.Expression);
        }
      }
      return (ScalarToken) new BasicExpressionToken(this.m_fileId, line, column, string.Format("format('{0}'{1})", (object) stringBuilder1, (object) stringBuilder2));
    }

    private ExpressionToken ParseExpression(
      int? line,
      int? column,
      string value,
      out Exception ex)
    {
      string str = value.Trim();
      if (string.IsNullOrEmpty(str))
      {
        ex = (Exception) new ArgumentException(YamlStrings.ExpectedExpression());
        return (ExpressionToken) null;
      }
      List<string> parameters;
      if (TemplateReader.MatchesDirective(str, "if", 1, out parameters, out ex))
        return !ExpressionToken.IsValidExpression(parameters[0], out ex) ? (ExpressionToken) null : (ExpressionToken) new IfExpressionToken(this.m_fileId, line, column, parameters[0]);
      if (TemplateReader.MatchesDirective(str, "elseif", 1, out parameters, out ex))
        return !ExpressionToken.IsValidExpression(parameters[0], out ex) ? (ExpressionToken) null : (ExpressionToken) new ElseIfExpressionToken(this.m_fileId, line, column, parameters[0]);
      if (ex == null && TemplateReader.MatchesDirective(str, "else", 0, out parameters, out ex))
        return (ExpressionToken) new ElseExpressionToken(this.m_fileId, line, column);
      if (ex == null && this.m_context.EnableEachExpressions && TemplateReader.MatchesDirective(str, "each", 3, out parameters, out ex))
      {
        if (!TemplateReader.IsValidIdentifier(parameters[0], out ex))
          return (ExpressionToken) null;
        if (!string.Equals(parameters[1], "in", StringComparison.Ordinal))
        {
          ex = (Exception) new ArgumentException(YamlStrings.InvalidEachParameter1((object) parameters[1]));
          return (ExpressionToken) null;
        }
        return !ExpressionToken.IsValidExpression(parameters[2], out ex) ? (ExpressionToken) null : (ExpressionToken) new EachExpressionToken(this.m_fileId, line, column, parameters[0], parameters[2]);
      }
      if (ex == null && TemplateReader.MatchesDirective(str, "insert", 0, out parameters, out ex))
        return (ExpressionToken) new InsertExpressionToken(this.m_fileId, line, column);
      if (ex != null)
        return (ExpressionToken) null;
      return !ExpressionToken.IsValidExpression(str, out ex) ? (ExpressionToken) null : (ExpressionToken) new BasicExpressionToken(this.m_fileId, line, column, str);
    }

    private void AddLiteral(List<ScalarToken> segments, int? line, int? column, string value)
    {
      if (segments.Count > 0 && segments[segments.Count - 1] is LiteralToken segment)
        segments[segments.Count - 1] = (ScalarToken) new LiteralToken(this.m_fileId, line, column, segment.Value + value);
      else
        segments.Add((ScalarToken) new LiteralToken(this.m_fileId, line, column, value));
    }

    private string GetUnexpectedValueMessage(string content) => !this.m_includeFileContentInErrors ? YamlStrings.UnexpectedValueWithoutContent() : YamlStrings.UnexpectedValue((object) content);

    private static bool MatchesDirective(
      string trimmed,
      string directive,
      int expectedParameters,
      out List<string> parameters,
      out Exception ex)
    {
      if (trimmed.StartsWith(directive, StringComparison.Ordinal) && (trimmed.Length == directive.Length || char.IsWhiteSpace(trimmed[directive.Length])))
      {
        parameters = new List<string>();
        int startIndex = directive.Length;
        bool flag = false;
        int num = 0;
        for (int index = startIndex; index < trimmed.Length; ++index)
        {
          char c = trimmed[index];
          if (char.IsWhiteSpace(c) && !flag && num == 0)
          {
            if (startIndex < index)
              parameters.Add(trimmed.Substring(startIndex, index - startIndex));
            startIndex = index + 1;
          }
          else if (c == '\'')
            flag = !flag;
          else if (c == '(' && !flag)
            ++num;
          else if (c == ')' && !flag)
            --num;
        }
        if (startIndex < trimmed.Length)
          parameters.Add(trimmed.Substring(startIndex));
        if (expectedParameters != parameters.Count)
        {
          ex = (Exception) new ArgumentException(YamlStrings.ExpectedNParametersFollowingDirective((object) expectedParameters, (object) directive, (object) parameters.Count));
          parameters = (List<string>) null;
          return false;
        }
        ex = (Exception) null;
        return true;
      }
      ex = (Exception) null;
      parameters = (List<string>) null;
      return false;
    }

    private static bool IsExpressionString(string trimmed, out string str)
    {
      StringBuilder stringBuilder = new StringBuilder();
      bool flag = false;
      for (int index = 0; index < trimmed.Length; ++index)
      {
        char ch = trimmed[index];
        if (ch == '\'')
        {
          flag = !flag;
          if (flag && index != 0)
            stringBuilder.Append(ch);
        }
        else
        {
          if (!flag)
          {
            str = (string) null;
            return false;
          }
          stringBuilder.Append(ch);
        }
      }
      str = stringBuilder.ToString();
      return true;
    }

    private static bool IsValidIdentifier(string str, out Exception ex)
    {
      if (string.IsNullOrEmpty(str))
      {
        ex = (Exception) new ArgumentException(YamlStrings.InvalidIdentifier((object) str));
        return false;
      }
      char ch1 = str[0];
      if (ch1 >= 'a' && ch1 <= 'z' || ch1 >= 'A' && ch1 <= 'Z' || ch1 == '_')
      {
        for (int index = 1; index < str.Length; ++index)
        {
          char ch2 = str[index];
          if ((ch2 < 'a' || ch2 > 'z') && (ch2 < 'A' || ch2 > 'Z') && (ch2 < '0' || ch2 > '9') && ch2 != '_')
          {
            ex = (Exception) new ArgumentException(YamlStrings.InvalidIdentifier((object) str));
            return false;
          }
        }
        if (string.Equals(str, "true", StringComparison.OrdinalIgnoreCase) || string.Equals(str, "false", StringComparison.OrdinalIgnoreCase) || string.Equals(str, "_", StringComparison.OrdinalIgnoreCase))
        {
          ex = (Exception) new ArgumentException(YamlStrings.InvalidIdentifierReserved((object) str));
          return false;
        }
        ex = (Exception) null;
        return true;
      }
      ex = (Exception) new ArgumentException(YamlStrings.InvalidIdentifier((object) str));
      return false;
    }
  }
}
