// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.BuildRequestValidationContext
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;

namespace Microsoft.TeamFoundation.Build2.Server
{
  public class BuildRequestValidationContext
  {
    public BuildDefinition Definition { get; set; }

    public BuildData Build { get; set; }

    public IOrchestrationEnvironment Environment { get; set; }

    public IOrchestrationProcess Process { get; set; }

    public AgentPoolQueue Queue { get; set; }

    public IBuildSourceProvider SourceProvider { get; set; }
  }
}
