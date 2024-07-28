// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.Yaml.YamlLoadResult
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using Microsoft.TeamFoundation.DistributedTask.Pipelines;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server.Yaml
{
  internal class YamlLoadResult
  {
    private List<string> errors;

    public PipelineTemplate PipelineTemplate { get; set; }

    public PipelineResources PipelineResources { get; set; }

    public IList<string> Errors
    {
      get
      {
        if (this.errors == null)
          this.errors = new List<string>();
        return (IList<string>) this.errors;
      }
    }
  }
}
