// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.IPipelineContext
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using Microsoft.TeamFoundation.DistributedTask.Expressions;
using Microsoft.TeamFoundation.DistributedTask.Logging;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.ComponentModel;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public interface IPipelineContext
  {
    ICounterStore CounterStore { get; }

    int EnvironmentVersion { get; }

    EvaluationOptions ExpressionOptions { get; }

    IPipelineIdGenerator IdGenerator { get; }

    IPackageStore PackageStore { get; }

    PipelineResources ReferencedResources { get; }

    IResourceStore ResourceStore { get; }

    IReadOnlyList<IStepProvider> StepProviders { get; }

    ISecretMasker SecretMasker { get; }

    ITaskStore TaskStore { get; }

    IPipelineTraceWriter Trace { get; }

    ISet<string> SystemVariableNames { get; }

    IDictionary<string, VariableValue> Variables { get; }

    IDictionary<string, bool> FeatureFlags { get; }

    string ExpandVariables(string value, bool maskSecrets = false);

    ExpressionResult<T> Evaluate<T>(string expression);

    ExpressionResult<JObject> Evaluate(JObject value);
  }
}
