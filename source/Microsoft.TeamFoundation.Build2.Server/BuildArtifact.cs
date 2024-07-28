// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.BuildArtifact
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using System;

namespace Microsoft.TeamFoundation.Build2.Server
{
  public class BuildArtifact
  {
    public int Id { get; set; }

    public string Name { get; set; }

    public string Source { get; set; }

    public DateTime SourceCreatedDate { get; set; }

    public int BuildId { get; set; }

    public BuildArtifactResource Resource { get; set; }
  }
}
