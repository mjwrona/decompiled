// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Deployment
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.Conditions;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model
{
  [SuppressMessage("Microsoft.Naming", "CA1724:TypeNamesShouldNotMatchNamespaces", Justification = "Deployment is a concept in RM")]
  public class Deployment
  {
    private Guid requestedFor = Guid.Empty;

    public int Id { get; set; }

    public Guid ProjectId { get; set; }

    public int ReleaseDefinitionId { get; set; }

    public string ReleaseDefinitionName { get; set; }

    public string ReleaseDefinitionPath { get; set; }

    public int ReleaseId { get; set; }

    public string ReleaseName { get; set; }

    public int ReleaseEnvironmentId { get; set; }

    public string ReleaseEnvironmentName { get; set; }

    public int DefinitionEnvironmentId { get; set; }

    public int Attempt { get; set; }

    public DeploymentReason Reason { get; set; }

    public DeploymentStatus Status { get; set; }

    public DeploymentOperationStatus OperationStatus { get; set; }

    public Guid RequestedBy { get; set; }

    public Guid RequestedFor
    {
      get => !(this.requestedFor == Guid.Empty) ? this.requestedFor : this.RequestedBy;
      set => this.requestedFor = value;
    }

    public DateTime QueuedOn { get; set; }

    public DateTime StartedOn { get; set; }

    public DateTime LastModifiedOn { get; set; }

    public Guid LastModifiedBy { get; set; }

    public IList<ReleaseEnvironmentStep> Steps { get; private set; }

    public IList<ReleaseCondition> Conditions { get; set; }

    public IList<ArtifactSource> LinkedArtifacts { get; private set; }

    public IList<DeploymentGate> DeploymentGates { get; private set; }

    public DateTime ScheduledDeploymentTime { get; set; }

    public IList<Issue> DeploymentIssues { get; private set; }

    public Deployment()
    {
      this.Steps = (IList<ReleaseEnvironmentStep>) new List<ReleaseEnvironmentStep>();
      this.Conditions = (IList<ReleaseCondition>) new List<ReleaseCondition>();
      this.LinkedArtifacts = (IList<ArtifactSource>) new List<ArtifactSource>();
      this.DeploymentGates = (IList<DeploymentGate>) new List<DeploymentGate>();
      this.DeploymentIssues = (IList<Issue>) new List<Issue>();
    }

    public bool IsRedeployAttempt() => this.Attempt > 1;
  }
}
