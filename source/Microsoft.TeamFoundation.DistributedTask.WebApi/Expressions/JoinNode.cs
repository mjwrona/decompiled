// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Expressions.JoinNode
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using System.Text;

namespace Microsoft.TeamFoundation.DistributedTask.Expressions
{
  internal sealed class JoinNode : FunctionNode
  {
    protected override sealed bool TraceFullyRealized => true;

    protected override sealed object EvaluateCore(EvaluationContext context)
    {
      EvaluationResult evaluationResult = this.Parameters[1].Evaluate(context);
      object collection;
      if (evaluationResult.TryGetCollectionInterface(out collection) && collection is IReadOnlyArray readOnlyArray)
      {
        if (readOnlyArray.Count <= 0)
          return (object) string.Empty;
        StringBuilder stringBuilder = new StringBuilder();
        MemoryCounter memoryCounter = new MemoryCounter((ExpressionNode) this, new int?(context.Options.MaxMemory));
        object obj1 = readOnlyArray[0];
        ResultMemory conversionResultMemory;
        string result1;
        if (EvaluationResult.CreateIntermediateResult(context, obj1, out conversionResultMemory).TryConvertToString(context, out result1))
        {
          memoryCounter.Add(result1);
          stringBuilder.Append(result1);
        }
        if (readOnlyArray.Count > 1)
        {
          string str = this.Parameters[0].EvaluateString(context);
          for (int index = 1; index < readOnlyArray.Count; ++index)
          {
            memoryCounter.Add(str);
            stringBuilder.Append(str);
            object obj2 = readOnlyArray[index];
            string result2;
            if (EvaluationResult.CreateIntermediateResult(context, obj2, out conversionResultMemory).TryConvertToString(context, out result2))
            {
              memoryCounter.Add(result2);
              stringBuilder.Append(result2);
            }
          }
        }
        return (object) stringBuilder.ToString();
      }
      string result;
      return evaluationResult.TryConvertToString(context, out result) ? (object) result : (object) string.Empty;
    }
  }
}
