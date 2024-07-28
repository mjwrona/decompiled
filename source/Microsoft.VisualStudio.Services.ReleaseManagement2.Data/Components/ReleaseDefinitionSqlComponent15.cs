// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components.ReleaseDefinitionSqlComponent15
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataAccess;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components
{
  [SuppressMessage("Microsoft.Maintainability", "CA1501:AvoidExcessiveInheritance", Justification = "Necessary to handle onprem host servicing")]
  public class ReleaseDefinitionSqlComponent15 : ReleaseDefinitionSqlComponent14
  {
    protected override void BindDeployPhases(ReleaseDefinition releaseDefinition)
    {
      if (releaseDefinition == null)
        throw new ArgumentNullException(nameof (releaseDefinition));
      this.BindForeignKeyReferenceTable("environmentDeployPhasesLink", ReleaseDefinitionSqlComponent.GetParentChildReference<DefinitionEnvironment, DeployPhase>((IEnumerable<DefinitionEnvironment>) releaseDefinition.Environments, (Func<DefinitionEnvironment, IList<DeployPhase>>) (e => e.DeployPhases)));
      this.BindDeployPhaseTable(releaseDefinition.Environments.SelectMany<DefinitionEnvironment, DeployPhase>((Func<DefinitionEnvironment, IEnumerable<DeployPhase>>) (e => (IEnumerable<DeployPhase>) e.DeployPhases)));
    }

    public override void BindDeployPhaseTable(IEnumerable<DeployPhase> environmentDeployPhases) => this.BindDefinitionEnvironmentDeployPhaseTable2("definitionEnvironmentDeployPhases", environmentDeployPhases);
  }
}
