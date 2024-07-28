// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.DeploymentGate
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model
{
  public class DeploymentGate
  {
    public Guid ProjectId { get; set; }

    public int ReleaseId { get; set; }

    public int ReleaseEnvironmentId { get; set; }

    public int DeploymentId { get; set; }

    public EnvironmentStepType GateType { get; set; }

    public GateStatus Status { get; set; }

    public Guid? RunPlanId { get; set; }

    public int ReleaseEnvironmentStepId { get; set; }

    public DateTime? StartedOn { get; set; }

    public DateTime? LastModifiedOn { get; set; }

    public DateTime? StabilizationCompletedOn { get; set; }

    public DateTime? SucceedingSince { get; set; }

    [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "Need it for deserialization")]
    public IList<IgnoredGate> IgnoredGates { get; set; }

    public DateTime? DeploymentLastModifiedOn { get; set; }
  }
}
