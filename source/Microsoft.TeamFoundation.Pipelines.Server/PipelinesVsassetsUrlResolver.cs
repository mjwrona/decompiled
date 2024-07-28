// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Pipelines.Server.PipelinesVsassetsUrlResolver
// Assembly: Microsoft.TeamFoundation.Pipelines.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07451E6B-67F8-4956-AC64-CC041BD809B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Pipelines.Server.dll

using Microsoft.TeamFoundation.Framework.Server;

namespace Microsoft.TeamFoundation.Pipelines.Server
{
  [ExtensionPriority(50)]
  [ExtensionStrategy("Hosted")]
  internal class PipelinesVsassetsUrlResolver : PipelinesUrlResolver
  {
    protected override string Layer => nameof (PipelinesVsassetsUrlResolver);

    public override string Name => "PipelinesVsassets";

    protected override string AccessMappingName => PipelineConstants.VsassetsAccessMapping;
  }
}
