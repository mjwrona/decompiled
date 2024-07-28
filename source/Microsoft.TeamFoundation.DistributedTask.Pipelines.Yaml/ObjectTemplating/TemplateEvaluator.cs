// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.ObjectTemplating.TemplateEvaluator
// Assembly: Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2DC134C9-663D-46C7-A414-3ADCC50BB112
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.dll

using Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.ObjectTemplating.Events;
using Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.ObjectTemplating.Schemas;
using Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.ObjectTemplating.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.ObjectTemplating
{
  internal class TemplateEvaluator
  {
    private readonly TemplateContext m_context;
    private readonly TemplateEvents m_events;
    private readonly TemplateUnraveler m_reader;
    private readonly TemplateSchema m_schema;

    private TemplateEvaluator(TemplateContext context, TemplateToken template, int removeBytes)
    {
      this.m_context = context;
      this.m_events = context.Events;
      this.m_schema = context.Schema;
      this.m_reader = new TemplateUnraveler(context, template, removeBytes);
    }

    internal static TemplateToken Evaluate(
      TemplateContext context,
      string type,
      TemplateToken template,
      int removeBytes,
      int? fileId,
      bool omitHeader = false)
    {
      if (!omitHeader)
      {
        if (fileId.HasValue)
          context.TraceWriter.Info("{0}", (object) ("Begin evaluating template '" + context.GetFileName(fileId.Value) + "'"));
        else
          context.TraceWriter.Info("{0}", (object) "Begin evaluating template");
      }
      TemplateEvaluator templateEvaluator = new TemplateEvaluator(context, template, removeBytes);
      TemplateToken templateToken;
      try
      {
        templateToken = templateEvaluator.Evaluate(type).Value;
        if (templateToken != null)
          templateEvaluator.m_reader.ReadEnd();
      }
      catch (Exception ex)
      {
        context.Error(fileId, new int?(), new int?(), ex);
        templateToken = (TemplateToken) null;
      }
      if (!omitHeader)
      {
        if (fileId.HasValue)
          context.TraceWriter.Info("{0}", (object) ("Finished evaluating template '" + context.GetFileName(fileId.Value) + "'"));
        else
          context.TraceWriter.Info("{0}", (object) "Finished evaluating template");
      }
      return templateToken;
    }

    private TemplateEvaluator.ValueResult Evaluate(string type) => this.Evaluate(type, out bool _);

    private TemplateEvaluator.ValueResult Evaluate(string type, out bool movable, bool moveNext = true)
    {
      Definition definition = this.m_schema.GetDefinition(type);
      movable = false;
      LiteralToken literal;
      if (this.m_reader.AllowLiteral(out literal, moveNext))
      {
        movable = true;
        return this.ValidateAndTransform(type, literal, definition.Schemas, false);
      }
      SequenceToken sequence1;
      if (this.m_reader.AllowSequenceStart(out sequence1))
      {
        foreach (Schema schema in definition.Schemas)
        {
          if (schema is SequenceSchema sequenceSchema)
          {
            this.m_events.RaiseOnSequenceStart(this.m_context, type, sequence1);
            while (!this.m_reader.AllowSequenceEnd())
            {
              TemplateEvaluator.ValueResult valueResult = this.Evaluate(sequenceSchema.ItemType);
              if (valueResult.FromLoad && valueResult.Value is SequenceToken sequence2)
              {
                foreach (TemplateToken templateToken in sequence2)
                {
                  this.m_events.RaiseOnSequenceItem(this.m_context, type, sequence1, templateToken);
                  sequence1.Add(templateToken);
                }
                this.m_context.Memory.SubtractBytes(sequence2, false);
              }
              else
              {
                this.m_events.RaiseOnSequenceItem(this.m_context, type, sequence1, valueResult.Value);
                sequence1.Add(valueResult.Value);
              }
            }
            this.m_events.RaiseOnSequenceEnd(this.m_context, type, sequence1);
            return this.Transform(type, schema, (TemplateToken) sequence1);
          }
        }
        this.m_context.Error((TemplateToken) sequence1, YamlStrings.UnexpectedSequenceStart());
        while (!this.m_reader.AllowSequenceEnd())
          this.m_reader.SkipSequenceItem();
        return new TemplateEvaluator.ValueResult();
      }
      MappingToken mapping;
      if (!this.m_reader.AllowMappingStart(out mapping))
        throw new ArgumentException(YamlStrings.ExpectedScalarSequenceOrMapping());
      List<MappingSchema> mappingSchemas = new List<MappingSchema>(definition.Schemas.OfType<MappingSchema>());
      if (mappingSchemas.Count > 0)
      {
        this.m_events.RaiseOnMappingStart(this.m_context, type, mapping);
        if (!string.IsNullOrEmpty(mappingSchemas[0].FirstKey))
          return this.HandleMappingWithSignificantFirstKey(type, definition, mappingSchemas, mapping);
        return this.m_schema.HasProperties(mappingSchemas[0]) ? this.HandleMappingWithWellKnownProperties(type, definition, mappingSchemas, mapping) : this.HandleMappingWithAllLooseProperties(type, definition, mappingSchemas[0], mapping);
      }
      this.m_context.Error((TemplateToken) mapping, YamlStrings.UnexpectedMappingStart());
      return new TemplateEvaluator.ValueResult();
    }

    private TemplateEvaluator.ValueResult HandleMappingWithSignificantFirstKey(
      string type,
      Definition definition,
      List<MappingSchema> mappingSchemas,
      MappingToken mapping)
    {
      LiteralToken literal1;
      if (!this.m_reader.AllowLiteral(out literal1))
      {
        this.m_context.Error((TemplateToken) mapping, YamlStrings.ExpectedAtLeastOnePair());
        this.m_reader.ReadMappingEnd();
        this.m_events.RaiseOnMappingEnd(this.m_context, type, mapping);
        return new TemplateEvaluator.ValueResult();
      }
      string firstValueType;
      string looseKeyType;
      string looseValueType;
      if (!this.m_schema.TryMatchFirstKey(mappingSchemas, literal1.Value, out firstValueType, out looseKeyType, out looseValueType))
      {
        this.m_context.Error((TemplateToken) literal1, YamlStrings.UnexpectedValue((object) literal1.Value));
        this.m_reader.SkipMappingValue();
        while (this.m_reader.AllowLiteral(out LiteralToken _))
          this.m_reader.SkipMappingValue();
        this.m_reader.ReadMappingEnd();
        this.m_events.RaiseOnMappingEnd(this.m_context, type, mapping);
        return new TemplateEvaluator.ValueResult();
      }
      this.m_events.RaiseOnMappingKey(this.m_context, type, mapping, literal1);
      TemplateToken templateToken1 = this.Evaluate(firstValueType).Value;
      this.m_events.RaiseOnMappingValue(this.m_context, type, mapping, literal1, templateToken1);
      mapping.Add((ScalarToken) literal1, templateToken1);
      HashSet<string> stringSet = new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      stringSet.Add(literal1.Value);
      LiteralToken literal2;
      while (this.m_reader.AllowLiteral(out literal2))
      {
        if (!stringSet.Add(literal2.Value))
        {
          this.m_context.Error((TemplateToken) literal2, YamlStrings.ValueAlreadyDefined((object) literal2.Value));
          this.m_reader.SkipMappingValue();
        }
        else
        {
          string valueType;
          if (this.m_schema.TryMatchKey(mappingSchemas, literal2.Value, out valueType))
          {
            this.m_events.RaiseOnMappingKey(this.m_context, type, mapping, literal2);
            TemplateToken templateToken2 = this.Evaluate(valueType).Value;
            this.m_events.RaiseOnMappingValue(this.m_context, type, mapping, literal2, templateToken2);
            mapping.Add((ScalarToken) literal2, templateToken2);
          }
          else if (looseKeyType != null)
          {
            LiteralToken key = this.ValidateAndTransform(looseKeyType, literal2, this.m_schema.GetDefinition(looseKeyType).Schemas, true).Value as LiteralToken;
            this.m_events.RaiseOnMappingKey(this.m_context, type, mapping, key);
            TemplateToken templateToken3 = this.Evaluate(looseValueType).Value;
            this.m_events.RaiseOnMappingValue(this.m_context, type, mapping, key, templateToken3);
            mapping.Add((ScalarToken) key, templateToken3);
          }
          else
          {
            this.m_context.Error((TemplateToken) literal2, YamlStrings.UnexpectedValue((object) literal2.Value));
            this.m_reader.SkipMappingValue();
          }
        }
      }
      this.m_reader.ReadMappingEnd();
      this.m_events.RaiseOnMappingEnd(this.m_context, type, mapping);
      return this.Transform(type, (Schema) mappingSchemas.FirstOrDefault<MappingSchema>(), (TemplateToken) mapping);
    }

    private TemplateEvaluator.ValueResult HandleMappingWithWellKnownProperties(
      string type,
      Definition definition,
      List<MappingSchema> mappingSchemas,
      MappingToken mapping)
    {
      string type1 = (string) null;
      string type2 = (string) null;
      if (!string.IsNullOrEmpty(mappingSchemas[0].LooseKeyType))
      {
        type1 = mappingSchemas[0].LooseKeyType;
        type2 = mappingSchemas[0].LooseValueType;
      }
      HashSet<string> stringSet = new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      LiteralToken literal;
      while (this.m_reader.AllowLiteral(out literal))
      {
        if (!stringSet.Add(literal.Value))
        {
          this.m_context.Error((TemplateToken) literal, YamlStrings.ValueAlreadyDefined((object) literal.Value));
          this.m_reader.SkipMappingValue();
        }
        else
        {
          string valueType;
          if (this.m_schema.TryMatchKey(mappingSchemas, literal.Value, out valueType))
          {
            this.m_events.RaiseOnMappingKey(this.m_context, type, mapping, literal);
            TemplateToken templateToken = this.Evaluate(valueType).Value;
            this.m_events.RaiseOnMappingValue(this.m_context, type, mapping, literal, templateToken);
            mapping.Add((ScalarToken) literal, templateToken);
          }
          else if (type1 != null)
          {
            LiteralToken key = this.ValidateAndTransform(type1, literal, this.m_schema.GetDefinition(type1).Schemas, true).Value as LiteralToken;
            this.m_events.RaiseOnMappingKey(this.m_context, type, mapping, key);
            TemplateToken templateToken = this.Evaluate(type2).Value;
            this.m_events.RaiseOnMappingValue(this.m_context, type, mapping, key, templateToken);
            mapping.Add((ScalarToken) key, templateToken);
          }
          else
          {
            this.m_context.Error((TemplateToken) literal, YamlStrings.UnexpectedValue((object) literal.Value));
            this.m_reader.SkipMappingValue();
          }
        }
      }
      this.m_reader.ReadMappingEnd();
      this.m_events.RaiseOnMappingEnd(this.m_context, type, mapping);
      return this.Transform(type, (Schema) mappingSchemas.FirstOrDefault<MappingSchema>(), (TemplateToken) mapping);
    }

    private TemplateEvaluator.ValueResult HandleMappingWithAllLooseProperties(
      string type,
      Definition definition,
      MappingSchema mappingSchema,
      MappingToken mapping)
    {
      string looseKeyType = mappingSchema.LooseKeyType;
      string looseValueType = mappingSchema.LooseValueType;
      HashSet<string> stringSet = new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      LiteralToken literal;
      while (this.m_reader.AllowLiteral(out literal))
      {
        if (!stringSet.Add(literal.Value))
        {
          this.m_context.Error((TemplateToken) literal, YamlStrings.ValueAlreadyDefined((object) literal.Value));
          this.m_reader.SkipMappingValue();
        }
        else
        {
          LiteralToken key = this.ValidateAndTransform(looseKeyType, literal, this.m_schema.GetDefinition(looseKeyType).Schemas, true).Value as LiteralToken;
          this.m_events.RaiseOnMappingKey(this.m_context, type, mapping, key);
          bool movable;
          TemplateToken templateToken = !this.m_context.EvaluateAfterAddingToVariablesMap || !(type == "variables") ? this.Evaluate(looseValueType, out movable).Value : this.Evaluate(looseValueType, out movable, false).Value;
          this.m_events.RaiseOnMappingValue(this.m_context, type, mapping, key, templateToken);
          mapping.Add((ScalarToken) key, templateToken);
          if (movable && this.m_context.EvaluateAfterAddingToVariablesMap && type == "variables")
            this.m_reader.MoveNext();
        }
      }
      this.m_reader.ReadMappingEnd();
      this.m_events.RaiseOnMappingEnd(this.m_context, type, mapping);
      return this.Transform(type, (Schema) mappingSchema, (TemplateToken) mapping);
    }

    private void MergeExtendsResources(
      string type,
      TemplateToken value,
      TemplateToken transformResult,
      TemplateContext context)
    {
      if (!type.Equals("extendsTemplate") && !type.Equals("pipeline") || !(value is MappingToken source1))
        return;
      int count = source1.Count;
      if (count < 2)
        return;
      KeyValuePair<ScalarToken, TemplateToken> keyValuePair1 = source1.Where<KeyValuePair<ScalarToken, TemplateToken>>((Func<KeyValuePair<ScalarToken, TemplateToken>, bool>) (x => x.Key is LiteralToken key1 && key1.Value.Equals("resources"))).FirstOrDefault<KeyValuePair<ScalarToken, TemplateToken>>();
      if (keyValuePair1.Key == null || !(source1[count - 1].Key is LiteralToken key2) || !key2.Value.Equals("extends") || !(transformResult is MappingToken source2))
        return;
      KeyValuePair<ScalarToken, TemplateToken> keyValuePair2 = source2.Where<KeyValuePair<ScalarToken, TemplateToken>>((Func<KeyValuePair<ScalarToken, TemplateToken>, bool>) (x => x.Key is LiteralToken key3 && key3.Value.Equals("resources"))).FirstOrDefault<KeyValuePair<ScalarToken, TemplateToken>>();
      if (keyValuePair2.Key == null)
      {
        source2.Insert(keyValuePair1);
        context.Memory.AddBytes((TemplateToken) keyValuePair1.Key, true);
        context.Memory.AddBytes(keyValuePair1.Value, true);
      }
      else
      {
        MappingToken source3 = keyValuePair2.Value as MappingToken;
        Dictionary<string, KeyValuePair<ScalarToken, TemplateToken>> dictionary = source3.ToDictionary<KeyValuePair<ScalarToken, TemplateToken>, string>((Func<KeyValuePair<ScalarToken, TemplateToken>, string>) (x => x.Key.ToString()));
        foreach (KeyValuePair<ScalarToken, TemplateToken> keyValuePair3 in keyValuePair1.Value as MappingToken)
        {
          ScalarToken key4 = keyValuePair3.Key;
          KeyValuePair<ScalarToken, TemplateToken> keyValuePair4;
          if (key4 != null && dictionary.TryGetValue(key4.ToString(), out keyValuePair4) && keyValuePair4.Value is SequenceToken sequenceToken1 && keyValuePair3.Value is SequenceToken sequenceToken2)
          {
            foreach (TemplateToken templateToken in sequenceToken2)
            {
              sequenceToken1.Add(templateToken);
              context.Memory.AddBytes(templateToken, true);
            }
          }
          else
          {
            source3.Add(keyValuePair3);
            context.Memory.AddBytes((TemplateToken) keyValuePair3.Key, true);
            context.Memory.AddBytes(keyValuePair3.Value, true);
          }
        }
      }
    }

    private void ExtractTemplateInfo(
      TemplateToken value,
      out LiteralToken referencePath,
      out MappingToken parametersMapping)
    {
      referencePath = (LiteralToken) null;
      parametersMapping = (MappingToken) null;
      foreach (KeyValuePair<ScalarToken, TemplateToken> keyValuePair in TemplateUtil.AssertMapping(value, "template reference"))
      {
        LiteralToken literalToken = TemplateUtil.AssertLiteral((TemplateToken) keyValuePair.Key, "template reference key");
        switch (literalToken.Value)
        {
          case "template":
            referencePath = TemplateUtil.AssertLiteral(keyValuePair.Value, "template reference path");
            continue;
          case "parameters":
            parametersMapping = TemplateUtil.AssertMapping(keyValuePair.Value, "template reference parameters");
            continue;
          default:
            throw new NotSupportedException("Unexpected template property '" + literalToken.Value + "'");
        }
      }
    }

    private void SetTemplateVariables(TemplateContext context)
    {
      IDictionary<string, string> publicSystemVariables = ParserUtil.GetPublicSystemVariables(context);
      UserVariables userVariables = new UserVariables();
      context.ExpressionValues["variables"] = (object) new CompositeVariables(publicSystemVariables, (IDictionary<string, string>) userVariables);
      ParserUtil.SetUserVariables(context, userVariables);
    }

    private TemplateToken RemoveFromMappingToken(TemplateToken token, string key)
    {
      TemplateToken templateToken = (TemplateToken) null;
      MappingToken mappingToken = TemplateUtil.AssertMapping(token, "mapping");
      for (int index = mappingToken.Count - 1; index >= 0; --index)
      {
        KeyValuePair<ScalarToken, TemplateToken> keyValuePair = mappingToken[index];
        if (keyValuePair.Key is LiteralToken key1 && string.Equals(key1.Value, key, StringComparison.OrdinalIgnoreCase))
        {
          templateToken = keyValuePair.Value;
          mappingToken.RemoveAt(index);
          break;
        }
      }
      return templateToken;
    }

    private void SetTemplateParameters(
      string type,
      TemplateContext context,
      TemplateToken parameterDefinitionsToken,
      MappingToken parameterValuesMapping)
    {
      context.AllowUndeclaredParameters = !string.Equals(type, "extends") && !(parameterDefinitionsToken is SequenceToken);
      TemplateParametersBuilder parametersBuilder = new TemplateParametersBuilder(context);
      if (parameterDefinitionsToken is MappingToken parametersMapping)
        parametersBuilder.AddParameters(parametersMapping);
      else if (parameterDefinitionsToken is SequenceToken parametersSequence)
        parametersBuilder.AddParameters(parametersSequence);
      if (parameterValuesMapping != null)
      {
        foreach (KeyValuePair<ScalarToken, TemplateToken> keyValuePair in parameterValuesMapping)
        {
          string name = TemplateUtil.AssertLiteral((TemplateToken) keyValuePair.Key, "template reference parameters key").Value;
          TemplateToken templateToken = keyValuePair.Value;
          TemplateParameter parameter;
          if (parametersBuilder.TryGetParameter(name, out parameter))
            templateToken = this.TransformParameterValue(context, parameter, templateToken);
          parametersBuilder.AddValue(name, templateToken);
        }
      }
      context.ExpressionValues["parameters"] = (object) parametersBuilder.Build();
    }

    private TemplateToken TransformParameterValue(
      TemplateContext context,
      TemplateParameter parameterDefinition,
      TemplateToken parameterValue)
    {
      if (parameterValue is SequenceToken sequenceToken1 && (parameterDefinition.Type == TemplateParameterType.StepList || parameterDefinition.Type == TemplateParameterType.JobList || parameterDefinition.Type == TemplateParameterType.StageList || parameterDefinition.Type == TemplateParameterType.DeploymentList))
      {
        SequenceToken sequenceToken = new SequenceToken(sequenceToken1.FileId, sequenceToken1.Line, sequenceToken1.Column);
        for (int index = 0; index < sequenceToken1.Count; ++index)
        {
          if (sequenceToken1[index] is MappingToken mappingToken && mappingToken.Count > 0)
          {
            if (mappingToken[0].Key.ToString().Equals("template"))
            {
              ScalarSchema scalarSchema = new ScalarSchema();
              scalarSchema.Load = "parametersTemplateReference";
              foreach (TemplateToken templateToken in this.Load("paramExtends", (Schema) scalarSchema, (TemplateToken) mappingToken).Value as SequenceToken)
                sequenceToken.Add(templateToken);
              parameterValue = (TemplateToken) sequenceToken;
            }
            else
              sequenceToken.Add(sequenceToken1[index]);
          }
        }
      }
      return parameterValue;
    }

    private TemplateEvaluator.ValueResult Transform(
      string type,
      Schema schema,
      TemplateToken value)
    {
      if (schema == null || schema.Transform == null && string.IsNullOrEmpty(schema.Load) || value == null || this.m_context.Errors.Count > 0)
        return new TemplateEvaluator.ValueResult()
        {
          Value = value
        };
      if (schema.Transform == null)
        return this.Load(type, schema, value);
      this.m_context.TraceWriter.Info("Begin transform: " + type);
      this.m_context.EvaluateAfterAddingToVariablesMap = true;
      TemplateContext context = this.m_context.NewScope(true, true);
      context.ExpressionValues[nameof (value)] = (object) value;
      context.ExpressionFunctions.Add(TransformFunction.CreateFunctionInfo());
      TemplateToken transformResult = TemplateEvaluator.Evaluate(context, "any", schema.Transform, 0, schema.Transform.FileId, true);
      context.Memory.SubtractBytes(value, true);
      this.MergeExtendsResources(type, value, transformResult, context);
      this.m_context.TraceWriter.Info("End transform: " + type);
      return new TemplateEvaluator.ValueResult()
      {
        Value = transformResult
      };
    }

    private TemplateEvaluator.ValueResult Load(string type, Schema schema, TemplateToken value)
    {
      this.m_context.TraceWriter.Info("Begin load: " + schema.Load);
      LiteralToken referencePath;
      MappingToken parametersMapping;
      this.ExtractTemplateInfo(value, out referencePath, out parametersMapping);
      TemplateContext context = this.m_context.NewScope();
      context.ExpressionFunctions.Clear();
      context.ExpressionValues.Clear();
      this.SetTemplateVariables(context);
      LoadTemplateResult loadTemplateResult = this.m_context.TemplateLoader.Load(context, schema.Load, referencePath, this.m_context.State);
      if (this.m_context.Errors.Count > 0)
        return new TemplateEvaluator.ValueResult()
        {
          Value = value
        };
      TemplateToken parameterDefinitionsToken = this.RemoveFromMappingToken(loadTemplateResult.Value, "parameters");
      this.SetTemplateParameters(type, context, parameterDefinitionsToken, parametersMapping);
      if (this.m_context.Errors.Count > 0)
        return new TemplateEvaluator.ValueResult()
        {
          Value = value
        };
      TemplateToken templateToken = TemplateEvaluator.Evaluate(context, loadTemplateResult.Type, loadTemplateResult.Value, loadTemplateResult.ValueBytes, loadTemplateResult.FileId);
      context.Memory.SubtractBytes(value, true);
      this.m_context.TraceWriter.Info("End load: " + schema.Load);
      return new TemplateEvaluator.ValueResult()
      {
        FromLoad = true,
        Value = templateToken
      };
    }

    private TemplateEvaluator.ValueResult ValidateAndTransform(
      string type,
      LiteralToken literal,
      List<Schema> schemas,
      bool requireLiteral)
    {
      ScalarSchema scalarSchema = schemas.OfType<ScalarSchema>().FirstOrDefault<ScalarSchema>((Func<ScalarSchema, bool>) (x => x.IsMatch(literal)));
      if (scalarSchema == null)
      {
        this.m_context.Error((TemplateToken) literal, YamlStrings.UnexpectedValue((object) literal.Value));
        return new TemplateEvaluator.ValueResult();
      }
      TemplateEvaluator.ValueResult valueResult = this.Transform(type, (Schema) scalarSchema, (TemplateToken) literal);
      if (valueResult.Value == null)
        return new TemplateEvaluator.ValueResult();
      if (!requireLiteral || valueResult.Value.Type == 0)
        return valueResult;
      string message;
      switch (valueResult.Value.Type)
      {
        case 1:
          message = YamlStrings.TransformResultSequenceExpectedScalar();
          break;
        case 2:
          message = YamlStrings.TransformResultMappingExpectedScalar();
          break;
        default:
          throw new NotSupportedException(string.Format("Unexpected transformed template type '{0}'", (object) valueResult.Value.Type));
      }
      this.m_context.Error((TemplateToken) literal, message);
      return new TemplateEvaluator.ValueResult();
    }

    private void ValidateEnd() => this.m_reader.ReadEnd();

    private struct ValueResult
    {
      public TemplateToken Value;
      public bool FromLoad;
    }
  }
}
