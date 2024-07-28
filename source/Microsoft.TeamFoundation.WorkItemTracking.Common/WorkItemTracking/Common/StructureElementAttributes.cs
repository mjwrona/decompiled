// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Common.StructureElementAttributes
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E674B254-26A0-4D88-8FF3-86800090789C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Common.dll

using System.ComponentModel;
using System.Runtime.InteropServices;

namespace Microsoft.TeamFoundation.WorkItemTracking.Common
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  [StructLayout(LayoutKind.Sequential, Size = 1)]
  public struct StructureElementAttributes
  {
    public const string Id = "Id";
    public const string Name = "Name";
    public const string ParentId = "ParentId";
    public const string StructureType = "StructureId";
    public const string IsStructureNode = "IsStructureNode";
    public const string ProjectId = "ProjectId";
    public const string Deleted = "Deleted";
    public const string ForwardingId = "ForwardingId";
  }
}
