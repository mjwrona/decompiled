// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Common.WarehouseDimensionNames
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E674B254-26A0-4D88-8FF3-86800090789C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Common.dll

using System.Runtime.InteropServices;

namespace Microsoft.TeamFoundation.WorkItemTracking.Common
{
  [StructLayout(LayoutKind.Sequential, Size = 1)]
  internal struct WarehouseDimensionNames
  {
    public const string Person = "Person";
    public const string Project = "Team Project";
    public const string ProjectTime = "Date";
    public const string Area = "Area";
    public const string Iteration = "Iteration";
    public const string Build = "Build";
    public const string WorkItem = "Work Item";
  }
}
