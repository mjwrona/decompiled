// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.Model.GitHubContributorActivityType
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

using Microsoft.VisualStudio.Services.Analytics.OData.Infrastructure;

namespace Microsoft.VisualStudio.Services.Analytics.Model
{
  public enum GitHubContributorActivityType : byte
  {
    [LocalizedDisplayName("ENUM_TYPE_CONTRIBUTOR_ACTIVITY_TYPE_CLOSED", false)] Closed,
    [LocalizedDisplayName("ENUM_TYPE_CONTRIBUTOR_ACTIVITY_TYPE_CREATED", false)] Created,
    [LocalizedDisplayName("ENUM_TYPE_CONTRIBUTOR_ACTIVITY_TYPE_COMMENTED", false)] Commented,
    [LocalizedDisplayName("ENUM_TYPE_CONTRIBUTOR_ACTIVITY_TYPE_MERGED", false)] Merged,
    [LocalizedDisplayName("ENUM_TYPE_CONTRIBUTOR_ACTIVITY_TYPE_REVIEW_REQUEST", false)] ReviewRequest,
    [LocalizedDisplayName("ENUM_TYPE_CONTRIBUTOR_ACTIVITY_TYPE_REVIEW_REQUEST_REMOVED", false)] ReviewRequestRemoved,
    [LocalizedDisplayName("ENUM_TYPE_CONTRIBUTOR_ACTIVITY_TYPE_REVIEWED", false)] Reviewed,
    [LocalizedDisplayName("ENUM_TYPE_CONTRIBUTOR_ACTIVITY_TYPE_REOPEN", false)] Reopen,
    [LocalizedDisplayName("ENUM_TYPE_CONTRIBUTOR_ACTIVITY_TYPE_INACTIVE", false)] Inactive,
  }
}
