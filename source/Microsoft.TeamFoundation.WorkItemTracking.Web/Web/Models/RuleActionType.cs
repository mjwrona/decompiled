// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Web.Models.RuleActionType
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CCDEBCB9-DB6B-41A2-8797-78C2455AE321
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Web.dll

namespace Microsoft.TeamFoundation.WorkItemTracking.Web.Models
{
  public static class RuleActionType
  {
    public const string MakeRequired = "$makeRequired";
    public const string MakeReadOnly = "$makeReadOnly";
    public const string SetDefaultValue = "$setDefaultValue";
    public const string SetDefaultFromClock = "$setDefaultFromClock";
    public const string SetDefaultFromCurrentUser = "$setDefaultFromCurrentUser";
    public const string SetDefaultFromField = "$setDefaultFromField";
    public const string CopyValue = "$copyValue";
    public const string CopyFromClock = "$copyFromClock";
    public const string CopyFromCurrentUser = "$copyFromCurrentUser";
    public const string CopyFromField = "$copyFromField";
    public const string SetValueToEmpty = "$setValueToEmpty";
    public const string CopyFromServerClock = "$copyFromServerClock";
    public const string CopyFromServerCurrentUser = "$copyFromServerCurrentUser";
    public const string HideTargetField = "$hideTargetField";
    public const string DisallowValue = "$disallowValue";
  }
}
