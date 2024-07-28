// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.DeploymentServiceHostOptions
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class DeploymentServiceHostOptions
  {
    public readonly HostProcessType ProcessType;
    public readonly bool FailOnInvalidConfiguration;

    public DeploymentServiceHostOptions(
      HostProcessType processType = HostProcessType.Generic,
      bool failOnInvalidConfiguration = true)
    {
      this.ProcessType = processType;
      this.FailOnInvalidConfiguration = failOnInvalidConfiguration;
    }
  }
}
