// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Common.FieldsConstants
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Common
{
  public static class FieldsConstants
  {
    public const string ActivatedByRefName = "Microsoft.VSTS.Common.ActivatedBy";
    public const string ActivatedDateRefName = "Microsoft.VSTS.Common.ActivatedDate";
    public const string ClosedByRefName = "Microsoft.VSTS.Common.ClosedBy";
    public const string ClosedDateRefName = "Microsoft.VSTS.Common.ClosedDate";
    public const string ResolvedByRefName = "Microsoft.VSTS.Common.ResolvedBy";
    public const string ResolvedDateRefName = "Microsoft.VSTS.Common.ResolvedDate";
    public const string StateChangeDateRefName = "Microsoft.VSTS.Common.StateChangeDate";
    public const string AreaIdRefName = "System.AreaId";

    public static string AllowedValuesListHead(string fieldReferenceName) => string.Format("__Allowed values for field {0}", (object) fieldReferenceName);

    public static string SuggestedValuesListHead(string fieldReferenceName) => string.Format("__Suggested values for field {0}", (object) fieldReferenceName);
  }
}
