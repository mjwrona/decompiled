// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.BuildDefinitionSummary
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using System;

namespace Microsoft.TeamFoundation.Build2.Server
{
  public class BuildDefinitionSummary
  {
    public int Id;
    public string Name;
    public string Path;
    public DateTime CreatedDate;
    public Microsoft.TeamFoundation.Build2.Server.DefinitionQuality? DefinitionQuality;

    public bool HasQueueBuildPermission { get; set; }

    public BuildDefinitionSummary(BuildDefinition buildDef, bool hasQueueBuildPermission = false)
    {
      this.Id = buildDef.Id;
      this.Name = buildDef.Name;
      this.Path = buildDef.Path;
      this.CreatedDate = buildDef.CreatedDate;
      this.DefinitionQuality = buildDef.DefinitionQuality;
      this.HasQueueBuildPermission = hasQueueBuildPermission;
    }
  }
}
