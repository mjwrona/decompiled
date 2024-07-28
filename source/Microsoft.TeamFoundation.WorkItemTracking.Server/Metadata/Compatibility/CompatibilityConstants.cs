// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.Compatibility.CompatibilityConstants
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.DataModels;
using System;
using System.Globalization;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.Compatibility
{
  internal static class CompatibilityConstants
  {
    internal const RuleFlags DefaultRuleFlags = RuleFlags.Editable | RuleFlags.FlowdownTree | RuleFlags.Default;
    internal const RuleFlags AllowedValueRuleFlags = RuleFlags.ThenAllNodes | RuleFlags.Editable | RuleFlags.DenyWrite | RuleFlags.Unless | RuleFlags.FlowdownTree;
    internal const RuleFlags2 AllowExistingValueFlags = RuleFlags2.ThenImplicitEmpty | RuleFlags2.ThenImplicitUnchanged;
    internal const string PropID = "PropID";
    internal const string RuleFlags1 = "RuleFlags1";
    internal const string TreeType = "TreeType";
    internal const string WorkItemTypeUsageID = "WorkItemTypeUsageID";
    internal const int TFSEveryone = -1;
    internal const int AuthenticatedUsers = -2;
    internal const int TFSNamespaceAdministrators = -30;
    internal const int EmptyConstant = -10000;
    internal const int SameAsOldValueConstant = -10001;
    internal const int CurentUserCopyDefault = -10002;
    internal const int IncrementFieldValue = -10003;
    internal const int ValueInAnotherField = -10012;
    internal const int ServerDateTime = -10013;
    internal const int BecameNonEmpty = -10014;
    internal const int RemainedNonEmpty = -10015;
    internal const int InDeletedArea = -10016;
    internal const int InAdminOnlyArea = -10017;
    internal const int CurentUserServer = -10026;
    internal const int DefaultFromClock = -10028;
    internal const int GreaterThanPreviousValue = -10030;
    internal const int RandomGuid = -10031;

    internal static string WorkItemTypeAllowedValueListHead(int projectId) => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "__Valid work item types for ({0})", (object) projectId);
  }
}
