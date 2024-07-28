// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ManualIntervention
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations;
using System;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model
{
  public class ManualIntervention
  {
    public int Id { get; set; }

    public Guid ProjectId { get; set; }

    public int ReleaseId { get; set; }

    public string ReleaseName { get; set; }

    public int ReleaseEnvironmentId { get; set; }

    public string ReleaseEnvironmentName { get; set; }

    public int ReleaseDefinitionId { get; set; }

    public string ReleaseDefinitionName { get; set; }

    public string ReleaseDefinitionPath { get; set; }

    public int ReleaseDeployPhaseId { get; set; }

    public ManualInterventionStatus Status { get; set; }

    public Guid Approver { get; set; }

    public string Comments { get; set; }

    public DateTime CreatedOn { get; set; }

    public DateTime ModifiedOn { get; set; }

    public TaskActivityData TaskActivityData { get; set; }

    public string Instructions { get; set; }

    public Guid TimeoutJobId { get; set; }
  }
}
