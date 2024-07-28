// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.ExpressionValue`1
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines
{
  [DataContract]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class ExpressionValue<T> : ExpressionValue, IEquatable<ExpressionValue<T>>
  {
    [DataMember(Name = "LiteralValue", EmitDefaultValue = false)]
    private readonly T m_literalValue;
    [DataMember(Name = "VariableValue", EmitDefaultValue = false)]
    private readonly string m_expression;

    public ExpressionValue(T literalValue) => this.m_literalValue = literalValue;

    internal ExpressionValue(string expression, bool isExpression) => this.m_expression = ExpressionValue.IsExpression(expression) ? ExpressionValue.TrimExpression(expression) : throw new ArgumentException(PipelineStrings.ExpressionInvalid((object) expression));

    [JsonConstructor]
    private ExpressionValue()
    {
    }

    internal T Literal => this.m_literalValue;

    internal string Expression => this.m_expression;

    internal bool IsLiteral => string.IsNullOrEmpty(this.m_expression);

    public ExpressionResult<T> GetValue(IPipelineContext context = null)
    {
      if (this.IsLiteral)
        return new ExpressionResult<T>(this.m_literalValue, false);
      return context?.Evaluate<T>(this.m_expression);
    }

    public override string ToString()
    {
      if (!string.IsNullOrEmpty(this.m_expression))
        return "$[ " + this.m_expression + " ]";
      T literalValue = this.m_literalValue;
      ref T local = ref literalValue;
      return (object) local == null ? (string) null : local.ToString();
    }

    public static implicit operator ExpressionValue<T>(T value) => new ExpressionValue<T>(value);

    public bool Equals(ExpressionValue<T> rhs)
    {
      if ((object) rhs == null)
        return false;
      if ((object) this == (object) rhs)
        return true;
      return this.IsLiteral ? EqualityComparer<T>.Default.Equals(this.Literal, rhs.Literal) : this.Expression == rhs.Expression;
    }

    public override bool Equals(object obj) => this.Equals(obj as ExpressionValue<T>);

    public static bool operator ==(ExpressionValue<T> lhs, ExpressionValue<T> rhs) => (object) lhs == null ? (object) rhs == null : lhs.Equals(rhs);

    public static bool operator !=(ExpressionValue<T> lhs, ExpressionValue<T> rhs) => !(lhs == rhs);

    public override int GetHashCode()
    {
      if (this.IsLiteral)
      {
        if ((object) this.Literal != null)
          return this.Literal.GetHashCode();
      }
      else if (this.Expression != null)
        return this.Expression.GetHashCode();
      return 0;
    }
  }
}
