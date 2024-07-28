// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Common.WarehouseDimensionFieldNames
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E674B254-26A0-4D88-8FF3-86800090789C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Common.dll

using System.Runtime.InteropServices;

namespace Microsoft.TeamFoundation.WorkItemTracking.Common
{
  [StructLayout(LayoutKind.Sequential, Size = 1)]
  internal struct WarehouseDimensionFieldNames
  {
    public const string Identifier = "Work Item";
    public const string AssignedTo = "Assigned To";
    public const string ChangedBy = "Changed By";
    public const string ChangedDate = "Date";
    public const string CreatedBy = "Created By";
    public const string Area = "Area";
    public const string Iteration = "Iteration";
    public const string TeamProject = "Team Project";
  }
}
