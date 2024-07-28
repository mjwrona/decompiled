// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.DeploymentData
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using System;

namespace Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts
{
  public class DeploymentData
  {
    public int Id { get; set; }

    public int AttemptNumber { get; set; }

    public DeploymentState Status { get; set; }

    public DateTime CompletionTime { get; set; }

    public Microsoft.VisualStudio.Services.Identity.Identity Identity { get; set; }

    public Pipeline Pipeline { get; set; }

    public Run Run { get; set; }

    public Environment Environment { get; set; }
  }
}
