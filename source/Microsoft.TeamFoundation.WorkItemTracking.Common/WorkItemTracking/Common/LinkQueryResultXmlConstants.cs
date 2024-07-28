// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Common.LinkQueryResultXmlConstants
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E674B254-26A0-4D88-8FF3-86800090789C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Common.dll

using System.ComponentModel;
using System.Runtime.InteropServices;

namespace Microsoft.TeamFoundation.WorkItemTracking.Common
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  [StructLayout(LayoutKind.Sequential, Size = 1)]
  public struct LinkQueryResultXmlConstants
  {
    public const string RootElement = "WorkItemLinkRelations";
    public const string ChildElement = "R";
    public const string SourceID = "S";
    public const string TargetID = "T";
    public const string LinkType = "L";
    public const string EnumValue = "E";
    public const string SourceRun = "S";
    public const string TargetRun = "T";
    public const string RunStart = "S";
    public const string RunEnd = "E";
    public const int MissingID = 0;
  }
}
