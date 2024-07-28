// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.BuildOptionDefinition
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Build2.Server
{
  public class BuildOptionDefinition
  {
    private List<BuildOptionInputDefinition> m_inputs;
    private List<BuildOptionGroupDefinition> m_groups;

    public Guid Id { get; set; }

    public int Ordinal { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }

    public List<BuildOptionInputDefinition> Inputs
    {
      get
      {
        if (this.m_inputs == null)
          this.m_inputs = new List<BuildOptionInputDefinition>();
        return this.m_inputs;
      }
      set
      {
        if (value == null)
          return;
        this.m_inputs = new List<BuildOptionInputDefinition>((IEnumerable<BuildOptionInputDefinition>) value);
      }
    }

    public IList<BuildOptionGroupDefinition> Groups
    {
      get
      {
        if (this.m_groups == null)
          this.m_groups = new List<BuildOptionGroupDefinition>();
        return (IList<BuildOptionGroupDefinition>) this.m_groups;
      }
      set
      {
        if (value == null)
          return;
        this.m_groups = new List<BuildOptionGroupDefinition>((IEnumerable<BuildOptionGroupDefinition>) value);
      }
    }
  }
}
