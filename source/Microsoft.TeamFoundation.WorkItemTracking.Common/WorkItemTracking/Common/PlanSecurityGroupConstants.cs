// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Common.PlanSecurityGroupConstants
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E674B254-26A0-4D88-8FF3-86800090789C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Common.dll

using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace Microsoft.TeamFoundation.WorkItemTracking.Common
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  [StructLayout(LayoutKind.Sequential, Size = 1)]
  public struct PlanSecurityGroupConstants
  {
    public static readonly Guid SecurityNamespaceId = new Guid("BED337F8-E5F3-4FB9-80DA-81E17D06E7A8");
    public static readonly string SecurityNamespaceName = "Plan";
    public static readonly char PathSeparator = '/';
    public static readonly string RootToken = "Plan/";
  }
}
