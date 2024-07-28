// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.BuildDefinitionChangingEvent
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Build2.Server
{
  public class BuildDefinitionChangingEvent
  {
    public BuildDefinitionChangingEvent()
    {
    }

    public BuildDefinitionChangingEvent(
      AuditAction changeType,
      BuildDefinition originalDefinition,
      BuildDefinition newDefinition)
    {
      this.OriginalDefinition = originalDefinition;
      this.NewDefinition = newDefinition;
      this.ChangeType = changeType;
    }

    public BuildDefinitionChangingEvent(
      AuditAction changeType,
      BuildDefinition originalDefinition,
      BuildDefinition newDefinition,
      List<BuildRepository> originalResourceRepositories,
      List<BuildRepository> newResourceRepositories)
    {
      this.OriginalDefinition = originalDefinition;
      this.NewDefinition = newDefinition;
      this.ChangeType = changeType;
      this.OriginalResourceRepositories = originalResourceRepositories;
      this.NewResourceRepositories = newResourceRepositories;
    }

    public BuildDefinition OriginalDefinition { get; set; }

    public BuildDefinition NewDefinition { get; set; }

    public AuditAction ChangeType { get; set; }

    public List<BuildRepository> OriginalResourceRepositories { get; set; }

    public List<BuildRepository> NewResourceRepositories { get; set; }
  }
}
