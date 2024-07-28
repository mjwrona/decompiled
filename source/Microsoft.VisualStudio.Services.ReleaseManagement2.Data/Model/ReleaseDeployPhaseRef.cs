// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseDeployPhaseRef
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations;
using System;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model
{
  public class ReleaseDeployPhaseRef
  {
    public string EnvironmentName { get; set; }

    public int EnvironmentRank { get; set; }

    public int Attempt { get; set; }

    public DeployPhaseTypes PhaseType { get; set; }

    public string PhaseName { get; set; }

    public int PhaseId { get; set; }

    public int PhaseRank { get; set; }

    public Guid PlanId { get; set; }
  }
}
