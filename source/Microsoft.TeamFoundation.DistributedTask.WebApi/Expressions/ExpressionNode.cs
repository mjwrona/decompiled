// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Expressions.ExpressionNode
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using Microsoft.TeamFoundation.DistributedTask.Logging;
using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace Microsoft.TeamFoundation.DistributedTask.Expressions
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public abstract class ExpressionNode : IExpressionNode
  {
    private static readonly ValueKind[] s_simpleKinds = new ValueKind[6]
    {
      ValueKind.Boolean,
      ValueKind.DateTime,
      ValueKind.Null,
      ValueKind.Number,
      ValueKind.String,
      ValueKind.Version
    };
    private string m_name;

    internal ContainerNode Container { get; set; }

    internal int Level { get; private set; }

    protected internal string Name
    {
      get => string.IsNullOrEmpty(this.m_name) ? this.GetType().Name : this.m_name;
      set => this.m_name = value;
    }

    protected abstract bool TraceFullyRealized { get; }

    internal abstract string ConvertToExpression();

    internal abstract string ConvertToRealizedExpression(EvaluationContext context);

    protected virtual object EvaluateCore(EvaluationContext context) => throw new InvalidOperationException("Method EvaluateCore not implemented");

    protected virtual object EvaluateCore(EvaluationContext context, out ResultMemory resultMemory)
    {
      resultMemory = (ResultMemory) null;
      return this.EvaluateCore(context);
    }

    public T Evaluate<T>(
      ITraceWriter trace,
      ISecretMasker secretMasker,
      object state,
      EvaluationOptions options = null)
    {
      if (this.Container != null)
        throw new NotSupportedException("Expected IExpressionNode.Evaluate to be called on root node only.");
      ISecretMasker secretMasker1 = secretMasker;
      try
      {
        secretMasker = secretMasker?.Clone() ?? (ISecretMasker) new SecretMasker();
        trace = (ITraceWriter) new EvaluationTraceWriter(trace, secretMasker);
        EvaluationContext context = new EvaluationContext(trace, secretMasker, state, options, this);
        trace.Info("Evaluating: " + this.ConvertToExpression());
        if (typeof (T).Equals(typeof (string)))
        {
          string result = this.EvaluateString(context);
          this.TraceTreeResult(context, (object) result, ValueKind.String);
          return (T) result;
        }
        if (typeof (T).Equals(typeof (bool)))
        {
          bool boolean = this.EvaluateBoolean(context);
          this.TraceTreeResult(context, (object) boolean, ValueKind.Boolean);
          return (T) (ValueType) boolean;
        }
        if (typeof (T).Equals(typeof (Version)))
        {
          Version version = this.EvaluateVersion(context);
          this.TraceTreeResult(context, (object) version, ValueKind.Version);
          return (T) version;
        }
        if (typeof (T).Equals(typeof (DateTimeOffset)))
        {
          DateTimeOffset dateTime = this.EvaluateDateTime(context);
          this.TraceTreeResult(context, (object) dateTime, ValueKind.DateTime);
          return (T) (ValueType) dateTime;
        }
        if (typeof (T).Equals(typeof (DateTime)))
        {
          DateTimeOffset dateTime = this.EvaluateDateTime(context);
          this.TraceTreeResult(context, (object) dateTime, ValueKind.DateTime);
          return (T) (ValueType) dateTime.UtcDateTime;
        }
        TypeInfo typeInfo = typeof (T).GetTypeInfo();
        if (typeInfo.IsPrimitive)
        {
          if (typeof (T).Equals(typeof (Decimal)))
          {
            Decimal number = this.EvaluateNumber(context);
            this.TraceTreeResult(context, (object) number, ValueKind.Number);
            return (T) (ValueType) number;
          }
          if (typeof (T).Equals(typeof (byte)) || typeof (T).Equals(typeof (sbyte)) || typeof (T).Equals(typeof (short)) || typeof (T).Equals(typeof (ushort)) || typeof (T).Equals(typeof (int)) || typeof (T).Equals(typeof (uint)) || typeof (T).Equals(typeof (long)) || typeof (T).Equals(typeof (ulong)) || typeof (T).Equals(typeof (float)) || typeof (T).Equals(typeof (double)))
          {
            Decimal number = this.EvaluateNumber(context);
            trace.Verbose("Converting expression result to type " + typeof (T).Name);
            try
            {
              T obj = (T) Convert.ChangeType((object) number, typeof (T));
              this.TraceTreeResult(context, (object) Convert.ToDecimal((object) obj), ValueKind.Number);
              return obj;
            }
            catch (Exception ex)
            {
              context.Trace.Verbose("Failed to convert the result number into the type " + typeof (T).Name + ". " + ex.Message);
              throw new TypeCastException(secretMasker, (object) number, ValueKind.Number, typeof (T), ex.Message);
            }
          }
        }
        EvaluationResult evaluationResult = this.Evaluate(context);
        this.TraceTreeResult(context, evaluationResult.Value, evaluationResult.Kind);
        if (typeof (T).Equals(typeof (JToken)))
        {
          if (evaluationResult.Value == null)
            return default (T);
          return evaluationResult.Value is JToken ? (T) evaluationResult.Value : (T) JToken.FromObject(evaluationResult.Value, JsonUtility.CreateJsonSerializer());
        }
        if (evaluationResult.Kind == ValueKind.Object || evaluationResult.Kind == ValueKind.Array)
        {
          Type type = evaluationResult.Value.GetType();
          context.Trace.Verbose("Result type: " + type.Name);
          if (typeInfo.IsAssignableFrom(type.GetTypeInfo()))
            return (T) evaluationResult.Value;
          context.Trace.Verbose("Unable to assign result to the type " + typeof (T).Name);
          throw new TypeCastException(type, typeof (T));
        }
        if (evaluationResult.Kind == ValueKind.Null)
          return default (T);
        if (evaluationResult.Kind == ValueKind.String)
        {
          string toDeserialize = evaluationResult.Value as string;
          if (string.IsNullOrEmpty(toDeserialize))
            return default (T);
          try
          {
            return JsonUtility.FromString<T>(toDeserialize);
          }
          catch (Exception ex) when (ex is JsonReaderException || ex is JsonSerializationException)
          {
            context.Trace.Verbose("Failed to json-deserialize the result string into the type " + typeof (T).Name + ". " + ex.Message);
            throw new TypeCastException(context.SecretMasker, (object) toDeserialize, ValueKind.String, typeof (T), ex.Message);
          }
        }
        else
        {
          context.Trace.Verbose(string.Format("Unable to convert from kind {0} to the type {1}", (object) evaluationResult.Kind, (object) typeof (T).Name));
          throw new TypeCastException(context.SecretMasker, evaluationResult.Value, evaluationResult.Kind, typeof (T));
        }
      }
      finally
      {
        if (secretMasker != null && secretMasker != secretMasker1)
        {
          if (secretMasker is IDisposable disposable)
            disposable.Dispose();
          secretMasker = (ISecretMasker) null;
        }
      }
    }

    public object Evaluate(
      ITraceWriter trace,
      ISecretMasker secretMasker,
      object state,
      EvaluationOptions options = null)
    {
      return this.Evaluate(trace, secretMasker, state, options, out ValueKind _, out object _);
    }

    public bool EvaluateBoolean(ITraceWriter trace, ISecretMasker secretMasker, object state) => this.Evaluate<bool>(trace, secretMasker, state, (EvaluationOptions) null);

    [EditorBrowsable(EditorBrowsableState.Never)]
    public EvaluationResult EvaluateResult(
      ITraceWriter trace,
      ISecretMasker secretMasker,
      object state,
      EvaluationOptions options)
    {
      ValueKind kind;
      object raw;
      return new EvaluationResult((EvaluationContext) null, 0, this.Evaluate(trace, secretMasker, state, options, out kind, out raw), kind, raw, true);
    }

    public EvaluationResult Evaluate(EvaluationContext context)
    {
      this.Level = this.Container == null ? 0 : this.Container.Level + 1;
      ExpressionNode.TraceVerbose(context, this.Level, "Evaluating " + this.Name + ":");
      ResultMemory resultMemory;
      object core = this.EvaluateCore(context, out resultMemory);
      if (resultMemory == null)
        resultMemory = new ResultMemory();
      ValueKind kind;
      object raw;
      ResultMemory conversionResultMemory;
      object canonicalValue = ExpressionUtil.ConvertToCanonicalValue(context.Options, core, out kind, out raw, out conversionResultMemory);
      bool trimDepth = resultMemory.IsTotal || raw == null && ((IEnumerable<ValueKind>) ExpressionNode.s_simpleKinds).Contains<ValueKind>(kind);
      int? bytes1 = resultMemory.Bytes;
      int bytes2 = bytes1 ?? EvaluationMemory.CalculateBytes(raw ?? canonicalValue);
      context.Memory.AddAmount(this.Level, bytes2, trimDepth);
      if (raw != null)
      {
        if (conversionResultMemory == null)
          conversionResultMemory = new ResultMemory();
        bytes1 = conversionResultMemory.Bytes;
        int bytes3 = bytes1 ?? EvaluationMemory.CalculateBytes(canonicalValue);
        context.Memory.AddAmount(this.Level, bytes3);
      }
      EvaluationResult result = new EvaluationResult(context, this.Level, canonicalValue, kind, raw);
      if (this.TraceFullyRealized)
        context.SetTraceResult(this, result);
      return result;
    }

    public bool EvaluateBoolean(EvaluationContext context) => this.Evaluate(context).ConvertToBoolean(context);

    public DateTimeOffset EvaluateDateTime(EvaluationContext context) => this.Evaluate(context).ConvertToDateTime(context);

    public Decimal EvaluateNumber(EvaluationContext context) => this.Evaluate(context).ConvertToNumber(context);

    public string EvaluateString(EvaluationContext context) => this.Evaluate(context).ConvertToString(context);

    public Version EvaluateVersion(EvaluationContext context) => this.Evaluate(context).ConvertToVersion(context);

    public virtual IEnumerable<T> GetParameters<T>() where T : IExpressionNode => (IEnumerable<T>) Array.Empty<T>();

    protected MemoryCounter CreateMemoryCounter(EvaluationContext context) => new MemoryCounter(this, new int?(context.Options.MaxMemory));

    private object Evaluate(
      ITraceWriter trace,
      ISecretMasker secretMasker,
      object state,
      EvaluationOptions options,
      out ValueKind kind,
      out object raw)
    {
      if (this.Container != null)
        throw new NotSupportedException("Expected IExpressionNode.Evaluate to be called on root node only.");
      ISecretMasker secretMasker1 = secretMasker;
      try
      {
        secretMasker = secretMasker?.Clone() ?? (ISecretMasker) new SecretMasker();
        trace = (ITraceWriter) new EvaluationTraceWriter(trace, secretMasker);
        EvaluationContext context = new EvaluationContext(trace, secretMasker, state, options, this);
        trace.Info("Evaluating: " + this.ConvertToExpression());
        EvaluationResult evaluationResult = this.Evaluate(context);
        this.TraceTreeResult(context, evaluationResult.Value, evaluationResult.Kind);
        kind = evaluationResult.Kind;
        raw = evaluationResult.Raw;
        return evaluationResult.Value;
      }
      finally
      {
        if (secretMasker != null && secretMasker != secretMasker1)
        {
          if (secretMasker is IDisposable disposable)
            disposable.Dispose();
          secretMasker = (ISecretMasker) null;
        }
      }
    }

    private void TraceTreeResult(EvaluationContext context, object result, ValueKind kind)
    {
      string realizedExpression = this.ConvertToRealizedExpression(context);
      string b = ExpressionUtil.FormatValue(context.SecretMasker, result, kind);
      if (!string.Equals(realizedExpression, b, StringComparison.Ordinal) && (kind != ValueKind.Number || !string.Equals(realizedExpression, "'" + b + "'", StringComparison.Ordinal)))
        context.Trace.Info("Expanded: " + realizedExpression);
      context.Trace.Info("Result: " + b);
    }

    private static void TraceVerbose(EvaluationContext context, int level, string message) => context.Trace.Verbose(string.Empty.PadLeft(level * 2, '.') + (message ?? string.Empty));
  }
}
