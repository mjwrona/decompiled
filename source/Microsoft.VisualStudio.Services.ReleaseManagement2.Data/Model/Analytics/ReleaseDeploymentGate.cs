// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Analytics.ReleaseDeploymentGate
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Analytics
{
  public class ReleaseDeploymentGate
  {
    public Guid ProjectGuid { get; set; }

    public int ReleaseId { get; set; }

    public int ReleaseEnvironmentStepId { get; set; }

    public int ReleaseEnvironmentId { get; set; }

    public int ReleaseDeploymentId { get; set; }

    public int GateType { get; set; }

    public int Status { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public Guid? RunPlanGuid { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public DateTime? StartedOn { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public DateTime? LastModifiedOn { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public DateTime? StabilizationCompletedOn { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public DateTime? DeploymentLastModifiedOn { get; set; }
  }
}
