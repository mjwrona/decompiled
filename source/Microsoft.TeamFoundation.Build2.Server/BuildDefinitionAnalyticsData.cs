// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.BuildDefinitionAnalyticsData
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using System;

namespace Microsoft.TeamFoundation.Build2.Server
{
  public class BuildDefinitionAnalyticsData
  {
    public Guid ProjectGuid { get; internal set; }

    public int DefinitionId { get; internal set; }

    public int DefinitionVersion { get; internal set; }

    public string DefinitionName { get; internal set; }

    public int? ProcessType { get; internal set; }

    public bool Deleted { get; internal set; }

    public DateTime? LastUpdated { get; set; }

    public byte DataSourceId { get; set; }
  }
}
