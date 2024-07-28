// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.ObjectTemplating.Tokens.ElseIfExpressionToken
// Assembly: Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2DC134C9-663D-46C7-A414-3ADCC50BB112
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.dll

using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.ObjectTemplating.Tokens
{
  [DataContract]
  internal sealed class ElseIfExpressionToken : ExpressionToken
  {
    [DataMember(Name = "elseif", EmitDefaultValue = false)]
    private string m_expression;

    internal ElseIfExpressionToken(int? fileId, int? line, int? column, string expression)
      : base(6, fileId, line, column, "elseif")
    {
      this.m_expression = expression;
    }

    public override string ToString() => "${{ elseif " + this.m_expression + " }}";

    internal string Expression
    {
      get
      {
        if (this.m_expression == null)
          this.m_expression = string.Empty;
        return this.m_expression;
      }
    }

    internal bool Evaluate(TemplateContext context) => this.EvaluateBoolean(context, this.Expression);

    [System.Runtime.Serialization.OnSerializing]
    private void OnSerializing(StreamingContext context)
    {
      string expression = this.m_expression;
      if ((expression != null ? (expression.Length == 0 ? 1 : 0) : 0) == 0)
        return;
      this.m_expression = (string) null;
    }
  }
}
