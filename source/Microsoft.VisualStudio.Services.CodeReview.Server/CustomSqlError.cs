// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.CodeReview.Server.CustomSqlError
// Assembly: Microsoft.VisualStudio.Services.CodeReview.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2BCB2866-BDCB-4FDE-9EA3-48DFA660C131
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.CodeReview.Server.dll

namespace Microsoft.VisualStudio.Services.CodeReview.Server
{
  public static class CustomSqlError
  {
    public const int LowerBound = 1550000;
    public const int UpperBound = 1559999;
    public const int GenericWrapperCode = 50000;
    public const int TransactionRequired = 800000;
    public const int CodeReviewNotFound = 1550001;
    public const int CodeReviewArgumentNullException = 1550002;
    public const int CodeReviewNotActiveException = 1550003;
    public const int CodeReviewInvalidStatusChangeException = 1550004;
    public const int CodeReviewArgumentEmptyException = 1550005;
    public const int CodeReviewUnexpectedDiffFileId = 1550011;
    public const int IterationNotFound = 1550101;
    public const int IterationSaveFailed = 1550102;
    public const int IterationAlreadyExistsException = 1550103;
    public const int IterationMismatchedIdsException = 1550104;
    public const int IterationCannotBeUnpublishedException = 1550105;
    public const int UnpublishedIterationAlreadyExistsException = 1550106;
    public const int IterationCannotBePublishedException = 1550107;
    public const int ReviewerSaveFailedUponNullException = 1550201;
    public const int ReviewerCannotBeAssociatedWithUnpublishedIterationIdException = 1550202;
    public const int ReviewWithNoPublishedIterationException = 1550203;
    public const int ChangesAlreadyExistException = 1550301;
    public const int ChangesWithContentHashNotFoundException = 1550302;
    public const int StatusNotFound = 1550501;
    public const int TooManyStatusRecords = 1550502;
    public const int DatabaseQueryFailure = 1559901;
    public const int DatabaseSaveFailure = 1559902;
    public const int MAX_SQL_ERROR = 1559902;
  }
}
