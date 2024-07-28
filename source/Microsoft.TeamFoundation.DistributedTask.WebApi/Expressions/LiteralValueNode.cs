// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Expressions.LiteralValueNode
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using Microsoft.TeamFoundation.DistributedTask.Logging;
using System.ComponentModel;

namespace Microsoft.TeamFoundation.DistributedTask.Expressions
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class LiteralValueNode : ExpressionNode
  {
    public LiteralValueNode(object val)
    {
      ValueKind kind;
      this.Value = ExpressionUtil.ConvertToCanonicalValue((EvaluationOptions) null, val, out kind, out object _, out ResultMemory _);
      this.Kind = kind;
      this.Name = kind.ToString();
    }

    public ValueKind Kind { get; }

    public object Value { get; }

    protected override sealed bool TraceFullyRealized => false;

    internal override sealed string ConvertToExpression() => ExpressionUtil.FormatValue((ISecretMasker) null, this.Value, this.Kind);

    internal override sealed string ConvertToRealizedExpression(EvaluationContext context) => ExpressionUtil.FormatValue((ISecretMasker) null, this.Value, this.Kind);

    protected override sealed object EvaluateCore(EvaluationContext context) => this.Value;
  }
}
