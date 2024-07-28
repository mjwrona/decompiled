// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.AuditLog.ChecksServiceAuditConstants
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

namespace Microsoft.TeamFoundation.Framework.Server.AuditLog
{
  public static class ChecksServiceAuditConstants
  {
    public const string ConfigurationCreated = "CheckConfiguration.Created";
    public const string ConfigurationUpdated = "CheckConfiguration.Updated";
    public const string ConfigurationDeleted = "CheckConfiguration.Deleted";
    public const string ConfigurationDisabled = "CheckConfiguration.Disabled";
    public const string ConfigurationEnabled = "CheckConfiguration.Enabled";
    public const string ConfigurationApprovalCheckOrderChanged = "CheckConfiguration.ApprovalCheckOrderChanged";
    public const string CheckId = "CheckId";
    public const string CheckType = "Type";
    public const string ResourceType = "ResourceType";
    public const string ResourceId = "ResourceId";
    public const string ResourceName = "ResourceName";
    public const string IsDisabled = "IsDisabled";
    public const string OriginalApprovalType = "OriginalApprovalType";
    public const string FinalApprovalType = "FinalApprovalType";
    public const string CheckSuiteCompleted = "CheckSuite.Completed";
    public const string Status = "Status";
    public const string RunName = "RunName";
    public const string PipelineName = "PipelineName";
    public const string StageName = "StageName";
    public const string CheckSuiteId = "CheckSuiteId";
    public const string CheckSuiteStatus = "CheckSuiteStatus";
    public const string CheckRuns = "CheckRuns";
    public const string CheckRunId = "Id";
    public const string Approvers = "Approvers";
    public const string ChecksAuditData = "ChecksAuditData";
    public const string ApproverReassigned = "ApproverReassigned";
    public const string ApprovalSkipped = "ApprovalSkipped";
    public const int AuditDataMaxLength = 4000;
  }
}
