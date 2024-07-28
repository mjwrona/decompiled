// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.KpiDefinition
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class KpiDefinition
  {
    private List<KpiStateDefinition> m_states;

    public KpiDefinition() => this.m_states = new List<KpiStateDefinition>();

    public KpiDefinition(int id)
      : this()
    {
      this.Id = id;
    }

    public int Id { get; private set; }

    public string Area { get; set; }

    public string Name { get; set; }

    public string Scope { get; set; }

    public string DisplayName { get; set; }

    public string Description { get; set; }

    public bool HigherIsBetter { get; set; }

    public List<KpiStateDefinition> States => this.m_states;
  }
}
