// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.RetainBuildActionRequestData
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model
{
  [DataContract]
  public class RetainBuildActionRequestData : ActionRequestData
  {
    [DataMember]
    public int BuildId { get; private set; }

    [DataMember]
    public Guid ProjectId { get; private set; }

    [DataMember]
    public RetainBuildActionType ActionType { get; private set; }

    public RetainBuildActionRequestData(
      Guid projectId,
      int buildId,
      RetainBuildActionType actionType)
    {
      this.ProjectId = projectId;
      this.BuildId = buildId;
      this.ActionType = actionType;
    }
  }
}
