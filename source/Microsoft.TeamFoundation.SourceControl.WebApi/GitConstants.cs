// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebApi.GitConstants
// Assembly: Microsoft.TeamFoundation.SourceControl.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 663B2C57-EC74-4E67-8BD7-7AC09B503305
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.SourceControl.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.ComponentModel;

namespace Microsoft.TeamFoundation.SourceControl.WebApi
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  [GenerateAllConstants(null)]
  public static class GitConstants
  {
    public static readonly Guid GitSecurityNamespaceId = new Guid("{2E9EB7ED-3C0A-47D4-87C1-0FFDD275FD87}");
    public const string SettingsServiceCompareBranchKey = "Branches.Compare";
    public static readonly int MaxGitRefNameLength = 400;
    public static readonly int SourceControlCapabilityFlag = 2;
    public static readonly int MaxRepositoryNameLength = 256;
    public const string RefsPrefix = "refs/";
    public const string RefsHeadsMaster = "refs/heads/master";
    public const string RefsHeadsPrefix = "refs/heads/";
    public const string RefsNotesPrefix = "refs/notes/";
    public const string RefsPullPrefix = "refs/pull/";
    public const string RefsRestMergePrefix = "refs/azure-repos/merges/";
    public const string RefsRemotesPrefix = "refs/remotes/";
    public const string RefsTagsPrefix = "refs/tags/";
    public const string RefsBreadcrumbsPrefix = "refs/internal/bc/";
    public const string RefsPullForkSourceSuffix = "source";
    public const string RefsPullMergeSuffix = "merge";
    public const string RefsPullSourceFixupSuffix = "sourceFixup";
    public const string RefsPullTargetFixupSuffix = "targetFixup";
    public const string RefsPullMergedBlobSuffix = "mergedBlob/";
    public const string RefDereferencedSuffix = "^{}";
    public const string GitModulesFileName = ".gitmodules";
    public const string SecurableRoot = "repoV2/";
    public const char SecurityTokenDelimiter = '/';
  }
}
