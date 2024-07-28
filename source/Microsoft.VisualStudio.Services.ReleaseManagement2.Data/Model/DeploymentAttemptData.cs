// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.DeploymentAttemptData
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model
{
  [SuppressMessage("Microsoft.Naming", "CA1724:TypeNamesShouldNotMatchNamespaces", Justification = "Deployment is a concept in RM")]
  public class DeploymentAttemptData
  {
    public int DeploymentId { get; set; }

    public Guid ProjectId { get; set; }

    public int ReleaseDefinitionId { get; set; }

    public int ReleaseId { get; set; }

    public int ReleaseEnvironmentId { get; set; }

    public int DefinitionEnvironmentId { get; set; }

    public int Attempt { get; set; }

    public DeploymentReason Reason { get; set; }

    public DeploymentStatus Status { get; set; }

    public DeploymentOperationStatus OperationStatus { get; set; }
  }
}
