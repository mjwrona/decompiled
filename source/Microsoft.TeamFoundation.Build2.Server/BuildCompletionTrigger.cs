// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.BuildCompletionTrigger
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Build2.Server
{
  public class BuildCompletionTrigger
  {
    public Guid ProjectId { get; set; }

    public int DefinitionId { get; set; }

    public string Path { get; set; }

    public bool RequiresSuccessfulBuild { get; set; }

    public List<string> BranchFilters { get; set; }

    public BuildCompletionTrigger Clone() => new BuildCompletionTrigger()
    {
      ProjectId = this.ProjectId,
      DefinitionId = this.DefinitionId,
      Path = this.Path,
      RequiresSuccessfulBuild = this.RequiresSuccessfulBuild,
      BranchFilters = this.BranchFilters.ConvertAll<string>((Converter<string, string>) (filter => filter))
    };
  }
}
