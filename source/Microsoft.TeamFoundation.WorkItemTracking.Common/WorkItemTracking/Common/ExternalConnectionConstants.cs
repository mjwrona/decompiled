// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Common.ExternalConnectionConstants
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E674B254-26A0-4D88-8FF3-86800090789C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Common.dll

using System;
using System.ComponentModel;

namespace Microsoft.TeamFoundation.WorkItemTracking.Common
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public static class ExternalConnectionConstants
  {
    public const int ExternalTinyStringMaxLength = 100;
    public const int ExternalShortStringMaxLength = 400;
    public const int ExternalLongStringMaxLength = 4000;
    public static readonly Guid GitHubProviderId = new Guid("FE17D68F-209F-4052-B8D8-C07C3CD4FBE2");
    public const string GitHubProviderKey = "github.com";
    public const string GitHubProviderType = "GH";
    public const string GitHubEnterpriseProviderType = "GHE";
    public const string GitHubEnterpriseHostHeaderKey = "X-GitHub-Enterprise-Host";
    public const string GitHubEnterpriseVersionHeaderKey = "X-GitHub-Enterprise-Version";
    public const int DefaultDataCollectionTimeFrameInDays = 28;
  }
}
