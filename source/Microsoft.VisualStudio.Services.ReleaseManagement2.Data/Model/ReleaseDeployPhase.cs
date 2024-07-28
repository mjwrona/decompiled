// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseDeployPhase
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations;
using System;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model
{
  public class ReleaseDeployPhase
  {
    public int ReleaseId { get; set; }

    public Guid ProjectId { get; set; }

    public int ReleaseEnvironmentId { get; set; }

    public int Id { get; set; }

    public int Rank { get; set; }

    public DeployPhaseTypes PhaseType { get; set; }

    public int Attempt { get; set; }

    public DeployPhaseStatus Status { get; set; }

    public Guid? RunPlanId { get; set; }

    public string Logs { get; set; }

    public DateTime? DeploymentStartTime { get; set; }

    public int DeploymentId { get; set; }

    public DateTime? DeploymentLastModifiedOn { get; set; }

    public virtual ReleaseDeployPhase DeepClone()
    {
      ReleaseDeployPhase clonedObject = (ReleaseDeployPhase) this.MemberwiseClone();
      this.UpdateClonedObjectReferences(clonedObject);
      return clonedObject;
    }

    protected virtual void UpdateClonedObjectReferences(ReleaseDeployPhase clonedObject)
    {
      if (clonedObject == null || this.Logs == null)
        return;
      clonedObject.Logs = string.Copy(this.Logs);
    }
  }
}
