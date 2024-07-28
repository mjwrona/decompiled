// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.Expressions.ContainerNode
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using System.Collections.Generic;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines.Expressions
{
  internal class ContainerNode
  {
    public IDictionary<string, string> Environment { get; }

    public bool MapDockerSocket { get; }

    public string Image { get; }

    public string Options { get; }

    public string Volumes { get; }

    public string Ports { get; }

    public ContainerNode(ContainerResource resource)
    {
      this.Environment = resource.Environment;
      this.MapDockerSocket = resource.MapDockerSocket;
      this.Image = resource.Image;
      this.Options = resource.Options;
      if (resource.Volumes != null)
        this.Volumes = string.Join(",", (IEnumerable<string>) resource.Volumes);
      if (resource.Ports == null)
        return;
      this.Ports = string.Join(",", (IEnumerable<string>) resource.Ports);
    }
  }
}
