// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.DevSecOps.WebApi.ContainerMetadata
// Assembly: Microsoft.VisualStudio.Services.DevSecOps.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 78BC9F0A-6262-4C96-81AF-14E523464B20
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.DevSecOps.WebApi.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.DevSecOps.WebApi
{
  [DataContract]
  public class ContainerMetadata
  {
    public ContainerMetadata()
    {
    }

    public ContainerMetadata(
      ContainerType containerType,
      RulesetType rulesetType,
      int rulesetVersion,
      bool isInternal,
      Guid? hostId = null,
      string hostName = null,
      Guid? projectId = null,
      string projectName = null,
      Guid? componentId = null,
      string componentName = null,
      string containerId = null,
      DateTime? containerTime = null,
      string branchName = null,
      int countOfContainersNotScanned = 0,
      int countOfTotalContainers = 0)
    {
      this.ContainerType = containerType;
      this.RulesetType = rulesetType;
      this.RulesetVersion = rulesetVersion;
      this.IsInternal = isInternal;
      this.HostId = hostId;
      this.HostName = hostName;
      this.ProjectId = projectId;
      this.ProjectName = projectName;
      this.ComponentId = componentId;
      this.ComponentName = componentName;
      this.BranchName = branchName;
      this.ContainerId = containerId;
      this.ContainerTime = containerTime;
      this.CountOfContainersNotScanned = countOfContainersNotScanned;
      this.CountOfTotalContainers = countOfTotalContainers;
    }

    [DataMember]
    public ContainerType ContainerType { get; set; }

    [DataMember]
    public RulesetType RulesetType { get; set; }

    [DataMember]
    public int RulesetVersion { get; set; }

    [DataMember]
    public bool IsInternal { get; set; }

    [DataMember]
    public Guid? HostId { get; set; }

    [DataMember]
    public string HostName { get; set; }

    [DataMember]
    public Guid? ProjectId { get; set; }

    [DataMember]
    public string ProjectName { get; set; }

    [DataMember]
    public Guid? ComponentId { get; set; }

    [DataMember]
    public string ComponentName { get; set; }

    [DataMember]
    public string BranchName { get; set; }

    [DataMember]
    public string ContainerId { get; set; }

    [DataMember]
    public DateTime? ContainerTime { get; set; }

    [DataMember]
    public int CountOfContainersNotScanned { get; set; }

    [DataMember]
    public int CountOfTotalContainers { get; set; }

    public bool IsInternalRulesetType() => this.RulesetType == RulesetType.StayCleanInternal || this.RulesetType == RulesetType.GetCleanInternal;
  }
}
