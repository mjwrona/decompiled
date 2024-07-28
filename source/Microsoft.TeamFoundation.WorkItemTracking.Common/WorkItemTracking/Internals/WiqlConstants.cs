// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Internals.WiqlConstants
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E674B254-26A0-4D88-8FF3-86800090789C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Common.dll

using System.ComponentModel;

namespace Microsoft.TeamFoundation.WorkItemTracking.Internals
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public abstract class WiqlConstants
  {
    public const string WorkItemTable = "WorkItems";
    public const string WorkItemLinkTable = "WorkItemLinks";
    public const string SourcePrefix = "Source";
    public const string TargetPrefix = "Target";
    public const string MustContain = "MustContain";
    public const string MayContain = "MayContain";
    public const string DoesNotContain = "DoesNotContain";
    public const string Recursive = "Recursive";
    public const string ReturnMatchingChild = "ReturnMatchingChildren";
  }
}
