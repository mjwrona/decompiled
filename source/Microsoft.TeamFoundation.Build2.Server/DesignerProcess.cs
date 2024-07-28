// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.DesignerProcess
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Build2.Server
{
  [DataContract]
  public class DesignerProcess : BuildProcess
  {
    [DataMember(Name = "Phases", EmitDefaultValue = false)]
    private List<Phase> m_phases;

    public DesignerProcess()
      : base(1)
    {
    }

    public List<Phase> Phases
    {
      get
      {
        if (this.m_phases == null)
          this.m_phases = new List<Phase>();
        return this.m_phases;
      }
    }

    [DataMember(EmitDefaultValue = false)]
    public DesignerProcessTarget Target { get; set; }
  }
}
