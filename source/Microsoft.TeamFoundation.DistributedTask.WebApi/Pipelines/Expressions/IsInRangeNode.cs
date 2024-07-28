// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.Expressions.IsInRangeNode
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using Microsoft.TeamFoundation.DistributedTask.Expressions;
using System;
using System.ComponentModel;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines.Expressions
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class IsInRangeNode : FunctionNode
  {
    public static int minParameters = 3;
    public static int maxParameters = 3;

    protected override sealed bool TraceFullyRealized => false;

    protected override sealed object EvaluateCore(EvaluationContext context)
    {
      Decimal number1 = this.Parameters[0].EvaluateNumber(context);
      Decimal number2 = this.Parameters[1].EvaluateNumber(context);
      Decimal number3 = this.Parameters[2].EvaluateNumber(context);
      return (object) (bool) (!(number1 >= number2) ? 0 : (number1 <= number3 ? 1 : 0));
    }
  }
}
