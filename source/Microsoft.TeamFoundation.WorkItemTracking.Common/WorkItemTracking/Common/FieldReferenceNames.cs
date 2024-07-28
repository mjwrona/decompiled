// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Common.FieldReferenceNames
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E674B254-26A0-4D88-8FF3-86800090789C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Common.dll

using System.Runtime.InteropServices;

namespace Microsoft.TeamFoundation.WorkItemTracking.Common
{
  [StructLayout(LayoutKind.Sequential, Size = 1)]
  public struct FieldReferenceNames
  {
    public const string FoundInBuild = "Microsoft.VSTS.Build.FoundIn";
    public const string IntegrationBuild = "Microsoft.VSTS.Build.IntegrationBuild";
    public const string ReproSteps = "Microsoft.VSTS.TCM.ReproSteps";
    public const string StartDate = "Microsoft.VSTS.Scheduling.StartDate";
    public const string TargetDate = "Microsoft.VSTS.Scheduling.TargetDate";
  }
}
