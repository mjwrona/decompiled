// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.BuildOption
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Build2.Server
{
  public class BuildOption
  {
    private Dictionary<string, string> m_inputs;

    [DataMember(IsRequired = true, Order = 1, Name = "Definition")]
    public BuildOptionDefinition Definition { get; set; }

    [DataMember(EmitDefaultValue = true)]
    public bool Enabled { get; set; }

    public IDictionary<string, string> Inputs
    {
      get
      {
        if (this.m_inputs == null)
          this.m_inputs = new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        return (IDictionary<string, string>) this.m_inputs;
      }
      set
      {
        if (value == null)
          return;
        this.m_inputs = new Dictionary<string, string>(value, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      }
    }

    public BuildOption Clone() => new BuildOption()
    {
      Definition = this.Definition,
      Enabled = this.Enabled,
      Inputs = this.Inputs
    };
  }
}
