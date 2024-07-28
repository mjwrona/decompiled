// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Common.ReportingFormulas
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E674B254-26A0-4D88-8FF3-86800090789C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Common.dll

using System.ComponentModel;
using System.Runtime.InteropServices;

namespace Microsoft.TeamFoundation.WorkItemTracking.Common
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  [StructLayout(LayoutKind.Sequential, Size = 1)]
  public struct ReportingFormulas
  {
    public const int None = 0;
    public const int Sum = 1;
    public const int Count = 2;
    public const int DistinctCount = 3;
    public const int Average = 4;
    public const int Minimum = 5;
    public const int Maximum = 6;
  }
}
