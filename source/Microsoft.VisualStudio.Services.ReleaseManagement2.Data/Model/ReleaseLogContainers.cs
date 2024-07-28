// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseLogContainers
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model
{
  public class ReleaseLogContainers
  {
    public ReleaseLogContainers()
    {
      this.DeployPhases = (IList<ReleaseDeployPhaseRef>) new List<ReleaseDeployPhaseRef>();
      this.Gates = (IList<DeploymentGateRef>) new List<DeploymentGateRef>();
      this.DeploySteps = (IList<ReleaseEnvironmentStep>) new List<ReleaseEnvironmentStep>();
    }

    [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "explicitly set by caller")]
    public IList<ReleaseDeployPhaseRef> DeployPhases { get; set; }

    [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "explicitly set by caller")]
    public IList<DeploymentGateRef> Gates { get; set; }

    public IList<ReleaseEnvironmentStep> DeploySteps { get; }
  }
}
