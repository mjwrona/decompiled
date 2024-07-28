// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ApprovalOptions
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi;
using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model
{
  public class ApprovalOptions
  {
    private const int AllApproversRequired = 0;
    public static readonly int ApprovalDefaultTimeoutInMinutes = 43200;
    public static readonly int ApprovalMinTimeoutInMinutes = 0;
    public static readonly int ApprovalMaxTimeoutInMinutes = 525600;

    public int? ReqApproverCount { get; set; }

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    [DefaultValue(true)]
    public bool ReleaseCreatorCanBeApprover { get; set; }

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    [DefaultValue(false)]
    public bool AutoTriggeredAndPreviousEnvironmentApprovedCanBeSkipped { get; set; }

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    [DefaultValue(false)]
    public bool EnforceIdentityRevalidation { get; set; }

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    public int TimeoutInMinutes { get; set; }

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    [DefaultValue(ApprovalExecutionOrder.BeforeGates)]
    public ApprovalExecutionOrder ExecutionOrder { get; set; }

    public ApprovalOptions()
    {
      this.ReleaseCreatorCanBeApprover = true;
      this.ReqApproverCount = new int?(0);
      this.AutoTriggeredAndPreviousEnvironmentApprovedCanBeSkipped = false;
      this.EnforceIdentityRevalidation = false;
      this.TimeoutInMinutes = ApprovalOptions.ApprovalMinTimeoutInMinutes;
      this.ExecutionOrder = ApprovalExecutionOrder.BeforeGates;
    }

    public ApprovalOptions DeepClone() => (ApprovalOptions) this.MemberwiseClone();

    [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "method name")]
    public int GetApprovalTimeout()
    {
      if (this.TimeoutInMinutes <= ApprovalOptions.ApprovalMinTimeoutInMinutes)
        return ApprovalOptions.ApprovalDefaultTimeoutInMinutes;
      return this.TimeoutInMinutes > ApprovalOptions.ApprovalMaxTimeoutInMinutes ? ApprovalOptions.ApprovalMaxTimeoutInMinutes : this.TimeoutInMinutes;
    }

    public bool IsApproverAllowed(
      Guid actualApproverId,
      Guid releaseCreatedBy,
      Guid deploymentRequestedFor)
    {
      if (this.ReleaseCreatorCanBeApprover)
        return true;
      return !actualApproverId.Equals(releaseCreatedBy) && !actualApproverId.Equals(deploymentRequestedFor);
    }

    public bool HasMetRequiredNumberOfApproverCriteria(
      int currentlyApprovedCount,
      int totalNumberOfApprovers)
    {
      return currentlyApprovedCount >= this.RequiredNumberOfApprovals(totalNumberOfApprovers);
    }

    public bool AreAllApprovalsRequired(int totalNumberOfApprovers) => totalNumberOfApprovers == this.RequiredNumberOfApprovals(totalNumberOfApprovers);

    private int RequiredNumberOfApprovals(int totalNumberOfApprovers)
    {
      int num = this.ReqApproverCount.GetValueOrDefault();
      if (num == 0 || num > totalNumberOfApprovers)
        num = totalNumberOfApprovers;
      return num;
    }
  }
}
