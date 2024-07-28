// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Expressions.XorNode
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

namespace Microsoft.TeamFoundation.DistributedTask.Expressions
{
  internal sealed class XorNode : FunctionNode
  {
    protected override sealed bool TraceFullyRealized => false;

    protected override sealed object EvaluateCore(EvaluationContext context) => (object) (this.Parameters[0].EvaluateBoolean(context) ^ this.Parameters[1].EvaluateBoolean(context));
  }
}
