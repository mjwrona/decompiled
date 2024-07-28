// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.ProcessVersionExtension
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

namespace Microsoft.TeamFoundation.Build2.Server
{
  public static class ProcessVersionExtension
  {
    public const int Current = 3;

    public static bool SupportsPhaseDependencies(this BuildProcess buildProcess) => buildProcess != null && buildProcess.Version >= 1;

    public static bool SupportsYamlRepositoryEndpointAuthorization(this BuildProcess process) => process != null && process.Version >= 2;

    public static bool SupportsAuthorizedResources(this BuildProcess process) => process != null && process.Version >= 3;
  }
}
