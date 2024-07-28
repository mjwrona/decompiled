// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.IPipelineContextExtensions
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public static class IPipelineContextExtensions
  {
    public static IList<PipelineValidationError> Validate(
      this IPipelineContext context,
      IList<Step> steps,
      PhaseTarget target,
      BuildOptions options)
    {
      return new PipelineBuilder(context).Validate(steps, target, options);
    }

    internal static ExpressionResult<T> Evaluate<T>(
      this IPipelineContext context,
      string name,
      ExpressionValue<T> expression,
      T defaultValue,
      bool traceDefault = true)
    {
      ExpressionResult<T> expressionResult = (ExpressionResult<T>) null;
      if (expression != (ExpressionValue<T>) null)
      {
        if (expression.IsLiteral)
        {
          context.Trace?.Info(name + ": " + IPipelineContextExtensions.GetTraceValue<T>(expression.Literal));
          expressionResult = new ExpressionResult<T>(expression.Literal);
        }
        else
        {
          context.Trace?.EnterProperty(name);
          expressionResult = expression.GetValue(context);
          context.Trace?.LeaveProperty(name);
        }
      }
      else if (traceDefault && context.Trace != null)
        context.Trace.Info(string.Format("{0}: {1}", (object) name, (object) defaultValue));
      return expressionResult ?? new ExpressionResult<T>(defaultValue);
    }

    private static string GetTraceValue<T>(T value) => value.GetType().IsValueType ? value.ToString() : Environment.NewLine + JsonUtility.ToString((object) value, true);
  }
}
