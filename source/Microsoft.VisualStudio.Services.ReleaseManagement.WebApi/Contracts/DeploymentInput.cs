// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.DeploymentInput
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AE7F604E-30D7-44A7-BE7B-AB7FB5A67B31
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts
{
  [DataContract]
  public abstract class DeploymentInput : BaseDeploymentInput
  {
    private ArtifactsDownloadInput artifactsDownloadInput;

    [DataMember]
    public bool SkipArtifactsDownload { get; set; }

    [DataMember]
    public ArtifactsDownloadInput ArtifactsDownloadInput
    {
      get
      {
        if (this.artifactsDownloadInput == null)
          this.artifactsDownloadInput = new ArtifactsDownloadInput();
        return this.artifactsDownloadInput;
      }
      set => this.artifactsDownloadInput = value;
    }

    [DataMember]
    public int QueueId { get; set; }

    [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "Setter needed for deserialization")]
    [DataMember]
    public IList<Demand> Demands { get; set; }

    [DataMember]
    public bool EnableAccessToken { get; set; }

    internal override void SetSecuredObject(string token, int requiredPermissions)
    {
      base.SetSecuredObject(token, requiredPermissions);
      IList<Demand> demands = this.Demands;
      if (demands != null)
        demands.ForEach<Demand>((Action<Demand>) (i => i?.SetSecuredObject(token, requiredPermissions)));
      this.ArtifactsDownloadInput?.SetSecuredObject(token, requiredPermissions);
    }

    protected DeploymentInput() => this.Demands = (IList<Demand>) new List<Demand>();

    protected DeploymentInput(
      bool skipArtifactsDownload,
      bool enableAccessToken,
      int queueId,
      int timeOutInMinutes,
      IList<Demand> demands)
      : base(timeOutInMinutes, 1)
    {
      this.Demands = demands == null ? (IList<Demand>) new List<Demand>() : (IList<Demand>) new List<Demand>((IEnumerable<Demand>) demands);
      this.SkipArtifactsDownload = skipArtifactsDownload;
      this.EnableAccessToken = enableAccessToken;
      this.QueueId = queueId;
    }

    protected DeploymentInput(
      bool skipArtifactsDownload,
      bool enableAccessToken,
      int queueId,
      int timeoutInMinutes,
      IList<Demand> demands,
      int jobCancelTimeoutInMinutes,
      string condition,
      ArtifactsDownloadInput artifactsDownloadInput)
    {
      this.Demands = demands == null ? (IList<Demand>) new List<Demand>() : (IList<Demand>) new List<Demand>((IEnumerable<Demand>) demands);
      this.ArtifactsDownloadInput = artifactsDownloadInput;
      this.SkipArtifactsDownload = skipArtifactsDownload;
      this.EnableAccessToken = enableAccessToken;
      this.QueueId = queueId;
      this.TimeoutInMinutes = timeoutInMinutes;
      this.JobCancelTimeoutInMinutes = jobCancelTimeoutInMinutes;
      this.Condition = condition;
    }

    public override bool Equals(BaseDeploymentInput other)
    {
      DeploymentInput otherValue = other as DeploymentInput;
      return otherValue != null && base.Equals(other) && this.SkipArtifactsDownload == otherValue.SkipArtifactsDownload && (this.Condition ?? string.Empty).Equals(otherValue.Condition ?? string.Empty) && this.ArtifactsDownloadInput.Equals(otherValue.ArtifactsDownloadInput) && this.QueueId == otherValue.QueueId && this.EnableAccessToken == otherValue.EnableAccessToken && this.JobCancelTimeoutInMinutes == otherValue.JobCancelTimeoutInMinutes && (this.Demands != null || otherValue.Demands == null) && (this.Demands == null || otherValue.Demands != null) && (this.Demands == null || otherValue.Demands == null || this.Demands.Count == otherValue.Demands.Count && !this.Demands.Any<Demand>((Func<Demand, bool>) (d1 => !otherValue.Demands.Any<Demand>((Func<Demand, bool>) (d2 => d2.Equals((object) d1))))));
    }
  }
}
