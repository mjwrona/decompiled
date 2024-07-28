// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Common.RepositoryConstants
// Assembly: Microsoft.VisualStudio.Services.Search.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8E09DCBA-148E-4EB7-BB73-B53B030BE93E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Common.dll

using System;

namespace Microsoft.VisualStudio.Services.Search.Common
{
  public static class RepositoryConstants
  {
    public const int DefaultLastProcessedPushId = -1;
    public const int IndexingInSyncPushId = -2;
    public const int InvalidPushId = -3;
    public const string DefaultChangesetId = "-1";
    public static readonly string DefaultGitRepositoryBranchName = "DummyBranchNameForEmptyRepositories\\" + new Guid("01234567890123456789012345678911").ToString("P") + "/\\Ignore.lock";
    public static readonly string DefaultLastIndexCommitId = new string('0', 40);
    public static readonly int SecurityHashLength = 32;
    public static readonly DateTime DefaultLastIndexChangeUtcTime = new DateTime(2000, 1, 1, 1, 1, 1, DateTimeKind.Utc);
    public static readonly DateTime DefaultLastProcessedTime = new DateTime(2000, 1, 1, 1, 1, 1, DateTimeKind.Utc);
    public static readonly string BranchCreationOrDeletionCommitId = new string('0', 40);
  }
}
