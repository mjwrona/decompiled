// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.WebApi.DesignerProcess
// Assembly: Microsoft.TeamFoundation.Build2.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0683407D-7C61-4505-8CA6-86AD7E3B6BCA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Build2.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Build.WebApi
{
  [DataContract]
  public class DesignerProcess : BuildProcess
  {
    [DataMember(Name = "Phases", EmitDefaultValue = false)]
    private List<Phase> m_phases;

    public DesignerProcess()
      : this((ISecuredObject) null)
    {
    }

    internal DesignerProcess(ISecuredObject securedObject)
      : base(1, securedObject)
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
