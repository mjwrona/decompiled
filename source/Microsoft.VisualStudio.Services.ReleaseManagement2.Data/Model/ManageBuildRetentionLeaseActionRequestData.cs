// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ManageBuildRetentionLeaseActionRequestData
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model
{
  [DataContract]
  public class ManageBuildRetentionLeaseActionRequestData : ActionRequestData
  {
    [DataMember]
    public int BuildId { get; private set; }

    [DataMember]
    public Guid ProjectId { get; private set; }

    [DataMember]
    public ManageBuildRetentionLeaseActionType ActionType { get; private set; }

    [DataMember]
    public Guid ReleaseProjectId { get; set; }

    [DataMember]
    public int BuildDefinitionId { get; private set; }

    [DataMember]
    public string LeaseOwnerId { get; private set; }

    [DataMember]
    public int ReleaseId { get; set; }

    public ManageBuildRetentionLeaseActionRequestData(
      Guid projectId,
      int buildId,
      ManageBuildRetentionLeaseActionType actionType,
      Guid releaseProjectId,
      int buildDefinitionId,
      string leaseOwnerId,
      int releaseId)
    {
      this.ActionType = actionType;
      this.ProjectId = projectId;
      this.BuildId = buildId;
      this.ReleaseProjectId = releaseProjectId;
      this.BuildDefinitionId = buildDefinitionId;
      this.LeaseOwnerId = leaseOwnerId;
      this.ReleaseId = releaseId;
    }
  }
}
