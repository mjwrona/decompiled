// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.ObjectTemplating.Tokens.BasicExpressionToken
// Assembly: Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2DC134C9-663D-46C7-A414-3ADCC50BB112
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.dll

using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.ObjectTemplating.Tokens
{
  [DataContract]
  internal sealed class BasicExpressionToken : ExpressionToken
  {
    [DataMember(Name = "expr", EmitDefaultValue = false)]
    private string m_expression;

    internal BasicExpressionToken(int? fileId, int? line, int? column, string expression)
      : base(3, fileId, line, column, (string) null)
    {
      this.m_expression = expression;
    }

    public override string ToString() => "${{ " + this.m_expression + " }}";

    internal string Expression
    {
      get
      {
        if (this.m_expression == null)
          this.m_expression = string.Empty;
        return this.m_expression;
      }
    }

    internal LiteralToken EvaluateLiteralToken(TemplateContext context, out int bytes) => this.EvaluateLiteralToken(context, this.Expression, out bytes);

    internal MappingToken EvaluateMappingToken(TemplateContext context, out int bytes) => this.EvaluateMappingToken(context, this.Expression, out bytes);

    internal SequenceToken EvaluateSequenceToken(TemplateContext context, out int bytes) => this.EvaluateSequenceToken(context, this.Expression, out bytes);

    internal TemplateToken EvaluateTemplateToken(TemplateContext context, out int bytes) => this.EvaluateTemplateToken(context, this.Expression, true, out bytes);

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
