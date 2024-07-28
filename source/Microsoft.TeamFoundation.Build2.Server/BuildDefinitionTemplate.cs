// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.BuildDefinitionTemplate
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Build2.Server
{
  public class BuildDefinitionTemplate
  {
    private Dictionary<string, string> m_icons;
    private const string CustomCategoryName = "Custom";

    public BuildDefinitionTemplate() => this.Category = "Custom";

    public string Id { get; set; }

    public string Name { get; set; }

    public bool CanDelete { get; set; }

    public string Category { get; set; }

    public string DefaultHostedQueue { get; set; }

    public Guid IconTaskId { get; set; }

    public string Description { get; set; }

    public BuildDefinition Template { get; set; }

    public IDictionary<string, string> Icons
    {
      get
      {
        if (this.m_icons == null)
          this.m_icons = new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.Ordinal);
        return (IDictionary<string, string>) this.m_icons;
      }
      set
      {
        if (value == null)
          return;
        this.m_icons = new Dictionary<string, string>(value, (IEqualityComparer<string>) StringComparer.Ordinal);
      }
    }
  }
}
