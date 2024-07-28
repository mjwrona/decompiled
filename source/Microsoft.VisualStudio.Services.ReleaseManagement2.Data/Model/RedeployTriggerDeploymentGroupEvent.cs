// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.RedeployTriggerDeploymentGroupEvent
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model
{
  public class RedeployTriggerDeploymentGroupEvent
  {
    public RedeployTriggerDeploymentGroupEvent()
    {
      this.TagsInfo = new RedeployTriggerEventTagsInfo();
      this.TargetIds = (IList<int>) new List<int>();
    }

    public RedeployTriggerDeploymentGroupEvent(
      IList<int> targetIds,
      RedeployTriggerEventTagsInfo tagsInfo,
      int environmentId)
    {
      this.TargetIds = targetIds;
      this.TagsInfo = tagsInfo;
      this.EnvironmentId = environmentId;
    }

    public RedeployTriggerDeploymentGroupEvent(
      string eventType,
      int deploymentGroupId,
      IList<int> targetIds,
      RedeployTriggerEventTagsInfo tagsInfo,
      int environmentId,
      int id,
      DateTime modifiedOn,
      bool isReadyForProcessing)
    {
      this.EventType = eventType;
      this.DeploymentGroupId = deploymentGroupId;
      this.TargetIds = targetIds;
      this.TagsInfo = tagsInfo;
      this.EnvironmentId = environmentId;
      this.Id = (long) id;
      this.ModifiedOn = modifiedOn;
      this.IsReadyForProcessing = isReadyForProcessing;
    }

    public long Id { get; set; }

    public string EventType { get; set; }

    public int DeploymentGroupId { get; set; }

    [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "Setter needed for deserialization")]
    public IList<int> TargetIds { get; set; }

    [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "Setter needed for deserialization")]
    public RedeployTriggerEventTagsInfo TagsInfo { get; set; }

    public int EnvironmentId { get; set; }

    public DateTime ModifiedOn { get; set; }

    public bool IsReadyForProcessing { get; set; }

    public string MetaInfo { get; set; }

    public bool IsEqualsTo(RedeployTriggerDeploymentGroupEvent eventToCompare) => eventToCompare != null && this.DeploymentGroupId == eventToCompare.DeploymentGroupId && this.TagsInfo.Tags.OrderBy<string, string>((Func<string, string>) (t => t)).SequenceEqual<string>((IEnumerable<string>) eventToCompare.TagsInfo.Tags.OrderBy<string, string>((Func<string, string>) (t => t))) && this.TagsInfo.TagsAdded.OrderBy<string, string>((Func<string, string>) (t => t)).SequenceEqual<string>((IEnumerable<string>) eventToCompare.TagsInfo.TagsAdded.OrderBy<string, string>((Func<string, string>) (t => t))) && this.TagsInfo.TagsDeleted.OrderBy<string, string>((Func<string, string>) (t => t)).SequenceEqual<string>((IEnumerable<string>) eventToCompare.TagsInfo.TagsDeleted.OrderBy<string, string>((Func<string, string>) (t => t)));
  }
}
