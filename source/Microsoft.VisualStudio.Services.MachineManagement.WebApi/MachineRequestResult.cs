// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.MachineManagement.WebApi.MachineRequestResult
// Assembly: Microsoft.VisualStudio.Services.MachineManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0CB85E58-B74B-46EE-B86D-9E028F77476B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.MachineManagement.WebApi.dll

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.MachineManagement.WebApi
{
  [DataContract]
  public sealed class MachineRequestResult
  {
    private IDictionary<string, string> m_outputs;

    internal MachineRequestResult()
    {
    }

    public MachineRequestResult(MachineRequestOutcome outcome) => this.Outcome = outcome;

    [DataMember(IsRequired = true)]
    public MachineRequestOutcome Outcome { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public IDictionary<string, string> Outputs
    {
      get
      {
        if (this.m_outputs == null)
          this.m_outputs = (IDictionary<string, string>) new Dictionary<string, string>();
        return this.m_outputs;
      }
      set => this.m_outputs = value;
    }
  }
}
