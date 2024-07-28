// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Expressions.ContainsValueNode
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using System.Collections.Generic;

namespace Microsoft.TeamFoundation.DistributedTask.Expressions
{
  internal sealed class ContainsValueNode : FunctionNode
  {
    protected override sealed bool TraceFullyRealized => false;

    protected override sealed object EvaluateCore(EvaluationContext context)
    {
      object collection;
      if (this.Parameters[0].Evaluate(context).TryGetCollectionInterface(out collection))
      {
        EvaluationResult evaluationResult = this.Parameters[1].Evaluate(context);
        ResultMemory conversionResultMemory;
        switch (collection)
        {
          case IReadOnlyArray readOnlyArray:
            using (IEnumerator<object> enumerator = readOnlyArray.GetEnumerator())
            {
              while (enumerator.MoveNext())
              {
                object current = enumerator.Current;
                EvaluationResult intermediateResult = EvaluationResult.CreateIntermediateResult(context, current, out conversionResultMemory);
                if (evaluationResult.Equals(context, intermediateResult))
                  return (object) true;
              }
              break;
            }
          case IReadOnlyObject readOnlyObject:
            using (IEnumerator<object> enumerator = readOnlyObject.Values.GetEnumerator())
            {
              while (enumerator.MoveNext())
              {
                object current = enumerator.Current;
                EvaluationResult intermediateResult = EvaluationResult.CreateIntermediateResult(context, current, out conversionResultMemory);
                if (evaluationResult.Equals(context, intermediateResult))
                  return (object) true;
              }
              break;
            }
        }
      }
      return (object) false;
    }
  }
}
