// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.PhaseCondition
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using Microsoft.TeamFoundation.DistributedTask.Expressions;
using Microsoft.TeamFoundation.DistributedTask.Pipelines.Expressions;
using Microsoft.TeamFoundation.DistributedTask.Pipelines.Runtime;
using System.Collections.Generic;
using System.ComponentModel;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class PhaseCondition : GraphCondition<PhaseInstance>
  {
    public PhaseCondition(string condition)
      : base(condition)
    {
    }

    protected override IEnumerable<INamedValueInfo> GetSupportedNamedValues()
    {
      foreach (INamedValueInfo supportedNamedValue in base.GetSupportedNamedValues())
        yield return supportedNamedValue;
      yield return (INamedValueInfo) new NamedValueInfo<StageDependenciesContextNode>(Microsoft.TeamFoundation.DistributedTask.Pipelines.Expressions.ExpressionConstants.StageDependencies);
    }

    public ConditionResult Evaluate(PhaseExecutionContext context)
    {
      GraphCondition<PhaseInstance>.ConditionTraceWriter trace = new GraphCondition<PhaseInstance>.ConditionTraceWriter();
      bool flag = this.m_parsedCondition.Evaluate<bool>((ITraceWriter) trace, context.SecretMasker, (object) context, context.ExpressionOptions);
      return new ConditionResult()
      {
        Value = flag,
        Trace = trace.Trace
      };
    }
  }
}
