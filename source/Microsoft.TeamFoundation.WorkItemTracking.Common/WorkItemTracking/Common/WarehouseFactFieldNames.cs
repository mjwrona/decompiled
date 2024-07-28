// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Common.WarehouseFactFieldNames
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E674B254-26A0-4D88-8FF3-86800090789C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Common.dll

using System.Runtime.InteropServices;

namespace Microsoft.TeamFoundation.WorkItemTracking.Common
{
  [StructLayout(LayoutKind.Sequential, Size = 1)]
  internal struct WarehouseFactFieldNames
  {
    public const string BuiltinProjectSuffix = "Team Project";
    public const string RevisionExpiredDate = "Revision Expired Date";
    public const string PreviousState = "Previous State";
    public const string RecordCount = "Record Count";
    public const string TransitionCount = "State Change Count";
    public const string RevisionCount = "Revision Count";
    public const string WorkItemFactTrackingId = "Logical Tracking ID";
  }
}
