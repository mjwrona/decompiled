// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.EtwServicingContextListenerConfiguration
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class EtwServicingContextListenerConfiguration
  {
    public string ReleaseDefinitionId { get; set; }

    public string ReleaseId { get; set; }

    public string AttemptNumber { get; set; }

    public string ServiceName { get; set; }

    public string BranchName { get; set; }

    public string BuildNumber { get; set; }

    public string DeploymentName { get; set; }

    public string JobId { get; set; }
  }
}
