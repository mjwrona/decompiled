// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components.ReleaseDefinitionSqlComponent37
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataAccess;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataBinders;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components
{
  [SuppressMessage("Microsoft.Maintainability", "CA1501:AvoidExcessiveInheritance", Justification = "Versioning mechanism")]
  public class ReleaseDefinitionSqlComponent37 : ReleaseDefinitionSqlComponent36
  {
    protected override DefinitionEnvironmentDeployPhaseBinder GetDefinitionEnvironmentDeployPhaseBinder() => (DefinitionEnvironmentDeployPhaseBinder) new DefinitionEnvironmentDeployPhaseBinder2();

    public override void BindDeployPhaseTable(IEnumerable<DeployPhase> environmentDeployPhases) => this.BindDefinitionEnvironmentDeployPhaseTable4("definitionEnvironmentDeployPhases", environmentDeployPhases);
  }
}
