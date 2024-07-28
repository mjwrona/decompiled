// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.Expressions.ExpressionConstants
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using Microsoft.TeamFoundation.DistributedTask.Expressions;
using System.ComponentModel;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines.Expressions
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public static class ExpressionConstants
  {
    public static readonly string Dependencies = "dependencies";
    public static readonly string StageDependencies = "stageDependencies";
    public static readonly string Variables = "variables";
    public static readonly string Repositories = "repositories";
    public static readonly string Containers = "containers";
    public static readonly INamedValueInfo PipelineNamedValue = (INamedValueInfo) new NamedValueInfo<PipelineContextNode>("pipeline");
    public static readonly INamedValueInfo ResourcesNamedValue = (INamedValueInfo) new NamedValueInfo<ResourceContextNode>("resources");
    public static readonly INamedValueInfo VariablesNamedValue = (INamedValueInfo) new NamedValueInfo<VariablesContextNode>("variables");
    public static readonly IFunctionInfo CounterFunction = (IFunctionInfo) new FunctionInfo<CounterNode>("counter", 0, 2);
  }
}
