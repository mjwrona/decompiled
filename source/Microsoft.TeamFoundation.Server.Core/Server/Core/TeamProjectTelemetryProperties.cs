// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.TeamProjectTelemetryProperties
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

namespace Microsoft.TeamFoundation.Server.Core
{
  public static class TeamProjectTelemetryProperties
  {
    internal const string ProjectTelemetryPropertyBaseName = "TFS.Project.Activity";
    internal static readonly string Blocks = "TFS.Project.Activity.Blocks";
    internal static readonly string StartTime = "TFS.Project.Activity.StartTime";
    internal static readonly string Duration = "TFS.Project.Activity.Duration";
    internal static readonly string Outcome = "TFS.Project.Activity.Outcome";
    internal static readonly string Failures = "TFS.Project.Activity.Failures";
  }
}
