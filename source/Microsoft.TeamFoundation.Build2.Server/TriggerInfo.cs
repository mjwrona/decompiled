// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.TriggerInfo
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Build2.Server
{
  public class TriggerInfo
  {
    private Lazy<List<BuildDefinitionBranch>> m_branches;

    public TriggerInfo(BuildDefinition definition, FilteredBuildTrigger trigger)
    {
      this.Definition = definition;
      this.Trigger = trigger;
      this.m_branches = new Lazy<List<BuildDefinitionBranch>>();
    }

    public List<BuildDefinitionBranch> Branches => this.m_branches.Value;

    public BuildDefinition Definition { get; }

    public FilteredBuildTrigger Trigger { get; }
  }
}
