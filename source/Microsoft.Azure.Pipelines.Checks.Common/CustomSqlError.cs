// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Checks.Common.CustomSqlError
// Assembly: Microsoft.Azure.Pipelines.Checks.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8C585FB3-01FB-4B82-B4E2-03BD94D0A581
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.Checks.Common.dll

namespace Microsoft.Azure.Pipelines.Checks.Common
{
  public static class CustomSqlError
  {
    public const int GenericDatabaseUserMessage = 50000;
    public const int GenericDatabaseUpdateFailure = 1803800;
    public const int PolicyEvaluationRecordAlreadyExists = 1803801;
    public const int PolicyAssignmentIdNotFound = 1803802;
    public const int PolicyEvaluationRequestIdNotFound = 1803803;
    public const int PolicyEvaluationBatchRequestIdNotFound = 1803804;
    public const int PolicyEvaluationBatchRequestIdAlreadyExists = 1803805;
    public const int TransactionRequired = 1803901;
    public const int ApprovalExists = 1804000;
    public const int ApprovalIdNotProvided = 1804001;
  }
}
