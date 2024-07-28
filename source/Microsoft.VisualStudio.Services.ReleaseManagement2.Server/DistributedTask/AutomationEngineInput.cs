// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.DistributedTask.AutomationEngineInput
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server.DistributedTask
{
  public class AutomationEngineInput
  {
    private IDictionary<string, string> data;

    public AutomationEngineInput()
    {
      this.Variables = new Dictionary<string, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      this.ExternalVariableTasks = (IList<TaskInstance>) new List<TaskInstance>();
      this.Artifacts = (IList<ArtifactSource>) new List<ArtifactSource>();
      this.data = (IDictionary<string, string>) new Dictionary<string, string>();
    }

    public int ReleaseId { get; set; }

    public int EnvironmentId { get; set; }

    public int ReleaseStepId { get; set; }

    public int ReleaseDeployPhaseId { get; set; }

    public int DeploymentId { get; set; }

    public int StepType { get; set; }

    public DeployPhaseSnapshot DeployPhaseData { get; set; }

    public Dictionary<string, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue> Variables { get; private set; }

    public IList<TaskInstance> ExternalVariableTasks { get; private set; }

    public int AttemptNumber { get; set; }

    public Guid RequestedForId { get; set; }

    public Guid ReleaseDeploymentRequestedForId { get; set; }

    public IList<ArtifactSource> Artifacts { get; }

    public IDictionary<string, string> Data => this.data;

    public IOrchestrationProcess Process { get; set; }

    public void DataMerger(IDictionary<string, string> dictionaryData) => this.data = DictionaryMerger.MergeDictionaries<string, string>((IEnumerable<IDictionary<string, string>>) new List<IDictionary<string, string>>()
    {
      dictionaryData,
      this.data
    });

    public void MergeArtifacts(IList<ArtifactSource> artifacts)
    {
      if (artifacts == null)
        throw new ArgumentNullException(nameof (artifacts));
      this.Artifacts.AddRange<ArtifactSource, IList<ArtifactSource>>((IEnumerable<ArtifactSource>) artifacts);
    }
  }
}
