// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Internals.WorkItemFormExtensionsConstants
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E674B254-26A0-4D88-8FF3-86800090789C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Common.dll

using Microsoft.VisualStudio.Services.Common;
using System.ComponentModel;

namespace Microsoft.TeamFoundation.WorkItemTracking.Internals
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  [GenerateAllConstants(null)]
  public static class WorkItemFormExtensionsConstants
  {
    public const int ContributionDefaultHeight_Group = 150;
    public const int ContributionDefaultHeight_Control = 75;
    public const string ContributionTarget_Form = "ms.vss-work-web.work-item-form";
    public const string ContributionType_Page = "ms.vss-work-web.work-item-form-page";
    public const string ContributionType_Group = "ms.vss-work-web.work-item-form-group";
    public const string ContributionType_Control = "ms.vss-work-web.work-item-form-control";
    public const string ContributionType_Notifications = "ms.vss-work-web.work-item-notifications";
    public static string[] ValidFormContributionTypes = new string[3]
    {
      "ms.vss-work-web.work-item-form-page",
      "ms.vss-work-web.work-item-form-group",
      "ms.vss-work-web.work-item-form-control"
    };
  }
}
