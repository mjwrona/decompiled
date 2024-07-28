// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Analytics.ReleaseDeployment
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Analytics
{
  public class ReleaseDeployment
  {
    public Guid ProjectGuid { get; set; }

    public int ReleaseId { get; set; }

    public int ReleaseEnvironmentId { get; set; }

    public int ReleaseDeploymentId { get; set; }

    public int ReleaseDefinitionId { get; set; }

    public int EnvironmentDefinitionId { get; set; }

    public int Attempt { get; set; }

    public int Reason { get; set; }

    public int DeploymentStatus { get; set; }

    public int OperationStatus { get; set; }

    public Guid RequestedByGuid { get; set; }

    public Guid RequestedForGuid { get; set; }

    public Guid LastModifiedByGuid { get; set; }

    public DateTime QueuedOn { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public DateTime? StartedOn { get; set; }

    public DateTime LastModifiedOn { get; set; }
  }
}
