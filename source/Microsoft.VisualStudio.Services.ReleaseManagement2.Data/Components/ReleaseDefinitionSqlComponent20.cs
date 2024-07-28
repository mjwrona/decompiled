// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components.ReleaseDefinitionSqlComponent20
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataAccess;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components
{
  [SuppressMessage("Microsoft.Maintainability", "CA1501:AvoidExcessiveInheritance", Justification = "Versioning mechanism")]
  public class ReleaseDefinitionSqlComponent20 : ReleaseDefinitionSqlComponent19
  {
    public override void BindDefinitionEnvironmentTable(IList<DefinitionEnvironment> environments) => this.BindDefinitionEnvironmentTable8("definitionEnvironments", (IEnumerable<DefinitionEnvironment>) environments);

    protected override void BindReleaseDefinitionArtifactSourceTable(
      IEnumerable<ArtifactSource> linkedArtifactSources)
    {
      this.BindReleaseDefinitionArtifactSourceTable4("definitionArtifactSources", linkedArtifactSources);
    }

    protected override void BindReleaseTriggerTable(ReleaseDefinition releaseDefinition)
    {
      if (releaseDefinition == null)
        throw new ArgumentNullException(nameof (releaseDefinition));
      this.BindReleaseTriggerTable6("triggers", (IEnumerable<ReleaseTriggerBase>) releaseDefinition.Triggers);
    }

    public override void BindDeployPhaseTable(IEnumerable<DeployPhase> environmentDeployPhases) => this.BindDefinitionEnvironmentDeployPhaseTable3("definitionEnvironmentDeployPhases", environmentDeployPhases);

    protected override void BindReleaseTriggerTable(IEnumerable<ReleaseTriggerBase> triggers) => this.BindReleaseTriggerTable6(nameof (triggers), triggers);

    protected override void BindDefinitionEnvironmentSteps(
      IEnumerable<DefinitionEnvironmentStep> steps)
    {
      this.BindDefinitionEnvironmentStepTable2("definitionEnvironmentSteps", steps);
    }
  }
}
