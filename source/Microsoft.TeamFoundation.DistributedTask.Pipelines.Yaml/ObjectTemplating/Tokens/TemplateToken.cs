// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.ObjectTemplating.Tokens.TemplateToken
// Assembly: Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2DC134C9-663D-46C7-A414-3ADCC50BB112
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.dll

using Microsoft.TeamFoundation.DistributedTask.Expressions;
using Microsoft.TeamFoundation.DistributedTask.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.ObjectTemplating.Tokens
{
  [DataContract]
  [KnownType(typeof (LiteralToken))]
  [KnownType(typeof (ExpressionToken))]
  [KnownType(typeof (SequenceToken))]
  [KnownType(typeof (MappingToken))]
  [JsonConverter(typeof (TemplateTokenJsonConverter))]
  internal abstract class TemplateToken
  {
    protected TemplateToken(int type, int? fileId, int? line, int? column)
    {
      this.Type = type;
      this.FileId = fileId;
      this.Line = line;
      this.Column = column;
    }

    internal int? FileId { get; set; }

    [DataMember(Name = "line", EmitDefaultValue = false)]
    internal int? Line { get; }

    [DataMember(Name = "col", EmitDefaultValue = false)]
    internal int? Column { get; }

    [DataMember(Name = "type", EmitDefaultValue = false)]
    internal int Type { get; }

    internal virtual IReadOnlyTemplateToken ToReadOnly() => throw new NotSupportedException("Type " + this.GetType().Name + " cannot be converted to read only");

    protected bool EvaluateBoolean(TemplateContext context, string expression) => new ExpressionParser().CreateTree(expression, (Microsoft.TeamFoundation.DistributedTask.Expressions.ITraceWriter) null, (IEnumerable<INamedValueInfo>) context.GetExpressionNamedValues(), (IEnumerable<IFunctionInfo>) context.ExpressionFunctions).Evaluate<bool>(context.TraceWriter.ToExpressionTraceWriter(), (ISecretMasker) null, (object) context, this.EvaluationOptions(context));

    protected LiteralToken EvaluateLiteralToken(
      TemplateContext context,
      string expression,
      out int bytes)
    {
      int currentBytes = context.Memory.CurrentBytes;
      try
      {
        EvaluationResult result = new ExpressionParser().CreateTree(expression, (Microsoft.TeamFoundation.DistributedTask.Expressions.ITraceWriter) null, (IEnumerable<INamedValueInfo>) context.GetExpressionNamedValues(), (IEnumerable<IFunctionInfo>) context.ExpressionFunctions).EvaluateResult(context.TraceWriter.ToExpressionTraceWriter(), (ISecretMasker) null, (object) context, this.EvaluationOptions(context));
        LiteralToken literal;
        if (this.TryConvertToLiteralToken(context, result, out literal))
          return literal;
        context.Error(this, YamlStrings.ExpectedScalar());
        return this.CreateLiteralToken(context, expression);
      }
      finally
      {
        bytes = context.Memory.CurrentBytes - currentBytes;
      }
    }

    protected SequenceToken EvaluateSequenceToken(
      TemplateContext context,
      string expression,
      out int bytes)
    {
      int currentBytes = context.Memory.CurrentBytes;
      try
      {
        EvaluationResult result = new ExpressionParser().CreateTree(expression, (Microsoft.TeamFoundation.DistributedTask.Expressions.ITraceWriter) null, (IEnumerable<INamedValueInfo>) context.GetExpressionNamedValues(), (IEnumerable<IFunctionInfo>) context.ExpressionFunctions).EvaluateResult(context.TraceWriter.ToExpressionTraceWriter(), (ISecretMasker) null, (object) context, this.EvaluationOptions(context));
        if (this.ConvertToTemplateToken(context, result) is SequenceToken templateToken)
          return templateToken;
        context.Error(this, YamlStrings.ExpectedSequence());
        return this.CreateSequenceToken(context);
      }
      finally
      {
        bytes = context.Memory.CurrentBytes - currentBytes;
      }
    }

    protected MappingToken EvaluateMappingToken(
      TemplateContext context,
      string expression,
      out int bytes)
    {
      int currentBytes = context.Memory.CurrentBytes;
      try
      {
        EvaluationResult result = new ExpressionParser().CreateTree(expression, (Microsoft.TeamFoundation.DistributedTask.Expressions.ITraceWriter) null, (IEnumerable<INamedValueInfo>) context.GetExpressionNamedValues(), (IEnumerable<IFunctionInfo>) context.ExpressionFunctions).EvaluateResult(context.TraceWriter.ToExpressionTraceWriter(), (ISecretMasker) null, (object) context, this.EvaluationOptions(context));
        if (this.ConvertToTemplateToken(context, result) is MappingToken templateToken)
          return templateToken;
        context.Error(this, YamlStrings.ExpectedMapping());
        return this.CreateMappingToken(context);
      }
      finally
      {
        bytes = context.Memory.CurrentBytes - currentBytes;
      }
    }

    protected TemplateToken EvaluateTemplateToken(
      TemplateContext context,
      string expression,
      bool coerceNull,
      out int bytes)
    {
      int currentBytes = context.Memory.CurrentBytes;
      try
      {
        EvaluationResult result = new ExpressionParser().CreateTree(expression, (Microsoft.TeamFoundation.DistributedTask.Expressions.ITraceWriter) null, (IEnumerable<INamedValueInfo>) context.GetExpressionNamedValues(), (IEnumerable<IFunctionInfo>) context.ExpressionFunctions).EvaluateResult(context.TraceWriter.ToExpressionTraceWriter(), (ISecretMasker) null, (object) context, this.EvaluationOptions(context));
        return !coerceNull && result.Value == null ? (TemplateToken) null : this.ConvertToTemplateToken(context, result);
      }
      finally
      {
        bytes = context.Memory.CurrentBytes - currentBytes;
      }
    }

    private Microsoft.TeamFoundation.DistributedTask.Expressions.EvaluationOptions EvaluationOptions(
      TemplateContext context)
    {
      Microsoft.TeamFoundation.DistributedTask.Expressions.EvaluationOptions evaluationOptions = new Microsoft.TeamFoundation.DistributedTask.Expressions.EvaluationOptions()
      {
        Converters = context.ExpressionConverters,
        MaxMemory = context.Memory.MaxBytes,
        UseCollectionInterfaces = true
      };
      if (!context.AllowUndeclaredParameters && context.TemplateLoader.ParseOptions.EnableParameterReferenceErrors)
        evaluationOptions.StrictlyIndexedObjects = (ICollection<string>) new string[1]
        {
          "parameters"
        };
      return evaluationOptions;
    }

    private TemplateToken ConvertToTemplateToken(TemplateContext context, EvaluationResult result)
    {
      LiteralToken literal;
      if (this.TryConvertToLiteralToken(context, result, out literal))
        return (TemplateToken) literal;
      if (result.Raw != null)
      {
        if (result.Raw is SequenceToken raw1)
        {
          context.Memory.AddBytes((TemplateToken) raw1, true);
          return (TemplateToken) raw1;
        }
        if (result.Raw is MappingToken raw2)
        {
          context.Memory.AddBytes((TemplateToken) raw2, true);
          return (TemplateToken) raw2;
        }
      }
      object collection;
      ResultMemory conversionResultMemory;
      if (result.TryGetCollectionInterface(out collection))
      {
        switch (collection)
        {
          case IReadOnlyObject readOnlyObject:
            MappingToken mappingToken = this.CreateMappingToken(context);
            foreach (KeyValuePair<string, object> keyValuePair in (IEnumerable<KeyValuePair<string, object>>) readOnlyObject)
            {
              LiteralToken literalToken = this.CreateLiteralToken(context, keyValuePair.Key);
              EvaluationResult intermediateResult = EvaluationResult.CreateIntermediateResult((EvaluationContext) null, keyValuePair.Value, out conversionResultMemory);
              TemplateToken templateToken = this.ConvertToTemplateToken(context, intermediateResult);
              mappingToken.Add((ScalarToken) literalToken, templateToken);
            }
            return (TemplateToken) mappingToken;
          case IReadOnlyArray readOnlyArray:
            SequenceToken sequenceToken = this.CreateSequenceToken(context);
            foreach (object obj in (IEnumerable<object>) readOnlyArray)
            {
              EvaluationResult intermediateResult = EvaluationResult.CreateIntermediateResult((EvaluationContext) null, obj, out conversionResultMemory);
              TemplateToken templateToken = this.ConvertToTemplateToken(context, intermediateResult);
              sequenceToken.Add(templateToken);
            }
            return (TemplateToken) sequenceToken;
        }
      }
      throw new ArgumentException(YamlStrings.UnableToConvertToTemplateToken((object) result.Value?.GetType().FullName));
    }

    private bool TryConvertToLiteralToken(
      TemplateContext context,
      EvaluationResult result,
      out LiteralToken literal)
    {
      if (result.Raw != null)
      {
        if (result.Raw is LiteralToken raw)
        {
          context.Memory.AddBytes(raw);
          literal = raw;
          return true;
        }
        literal = (LiteralToken) null;
        return false;
      }
      string result1;
      if (result.TryConvertToString((EvaluationContext) null, out result1))
      {
        literal = this.CreateLiteralToken(context, result1);
        return true;
      }
      literal = (LiteralToken) null;
      return false;
    }

    private LiteralToken CreateLiteralToken(TemplateContext context, string value)
    {
      ScalarStyle? style = new ScalarStyle?();
      if (value.StartsWith("0"))
        style = new ScalarStyle?(ScalarStyle.SingleQuoted);
      LiteralToken literal = new LiteralToken(this.FileId, this.Line, this.Column, value, style);
      context.Memory.AddBytes(literal);
      return literal;
    }

    private SequenceToken CreateSequenceToken(TemplateContext context)
    {
      SequenceToken sequence = new SequenceToken(this.FileId, this.Line, this.Column);
      context.Memory.AddBytes(sequence);
      return sequence;
    }

    private MappingToken CreateMappingToken(TemplateContext context)
    {
      MappingToken mapping = new MappingToken(this.FileId, this.Line, this.Column);
      context.Memory.AddBytes(mapping);
      return mapping;
    }
  }
}
