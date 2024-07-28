// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.ObjectTemplating.TransformFunction
// Assembly: Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2DC134C9-663D-46C7-A414-3ADCC50BB112
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.dll

using Microsoft.TeamFoundation.DistributedTask.Expressions;
using Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.ObjectTemplating.Tokens;
using System;
using System.Globalization;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.ObjectTemplating
{
  internal sealed class TransformFunction : FunctionNode
  {
    internal static IFunctionInfo CreateFunctionInfo() => (IFunctionInfo) new FunctionInfo<TransformFunction>("transform", 2, int.MaxValue);

    protected override object EvaluateCore(EvaluationContext context)
    {
      string key1 = this.Parameters[0].EvaluateString(context);
      object[] objArray = new object[this.Parameters.Count - 1];
      for (int index = 1; index < this.Parameters.Count; ++index)
      {
        EvaluationResult evaluationResult = this.Parameters[index].Evaluate(context);
        objArray[index - 1] = evaluationResult.Raw == null ? evaluationResult.Value : evaluationResult.Raw;
      }
      TemplateContext state = context.State as TemplateContext;
      TemplateToken template;
      if (!state.Schema.Transforms.TryGetValue(key1, out template) || template == null)
        throw new ArgumentException("Transform '" + key1 + "' not found");
      TemplateContext context1 = state.NewScope(true, true);
      for (int index = 0; index < objArray.Length; ++index)
      {
        string key2 = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "arg{0}", (object) index);
        context1.ExpressionValues[key2] = objArray[index];
      }
      context1.ExpressionFunctions.Add(TransformFunction.CreateFunctionInfo());
      return (object) TemplateEvaluator.Evaluate(context1, "any", template, 0, template.FileId, true);
    }
  }
}
