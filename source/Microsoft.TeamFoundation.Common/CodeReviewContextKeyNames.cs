// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.CodeReviewContextKeyNames
// Assembly: Microsoft.TeamFoundation.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E643B42-FE11-4FC2-A9D6-79417E26CF92
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Common.dll

using System.ComponentModel;

namespace Microsoft.TeamFoundation
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public static class CodeReviewContextKeyNames
  {
    public static readonly string Workspace = nameof (Workspace);
    public static readonly string WorkItem = nameof (WorkItem);
    public static readonly string WorkItemId = nameof (WorkItemId);
    public static readonly string Shelveset = nameof (Shelveset);
    public static readonly string ShelvesetName = nameof (ShelvesetName);
    public static readonly string ShelvesetExcludedCount = "ExcludedCount";
    public static readonly string ChangesetId = nameof (ChangesetId);
  }
}
