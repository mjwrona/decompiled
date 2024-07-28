// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.PropertyFunctionNode
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.Expressions;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.DistributedTask.Server
{
  internal sealed class PropertyFunctionNode : FunctionNode
  {
    protected override sealed bool TraceFullyRealized => false;

    protected override sealed object EvaluateCore(EvaluationContext context)
    {
      IDictionary<string, string> state = context.State as IDictionary<string, string>;
      string key = this.Parameters[0]?.EvaluateString(context);
      return state != null && key != null && state.ContainsKey(key) ? (object) state[key] : (object) null;
    }

    public static string FunctionName => "property";
  }
}
