// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.WebApi.VariableMultipliersServerExecutionOptions
// Assembly: Microsoft.TeamFoundation.Build2.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0683407D-7C61-4505-8CA6-86AD7E3B6BCA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Build2.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Build.WebApi
{
  [DataContract]
  public class VariableMultipliersServerExecutionOptions : 
    ServerTargetExecutionOptions,
    IVariableMultiplierExecutionOptions
  {
    [DataMember(Name = "Multipliers", EmitDefaultValue = false)]
    private List<string> m_serializedMultipliers;
    private List<string> m_multipliers;

    public VariableMultipliersServerExecutionOptions()
      : this((ISecuredObject) null)
    {
    }

    internal VariableMultipliersServerExecutionOptions(ISecuredObject securedObject)
      : base(1, securedObject)
    {
      this.MaxConcurrency = 1;
      this.ContinueOnError = false;
    }

    [DataMember(EmitDefaultValue = true)]
    [DefaultValue(1)]
    public int MaxConcurrency { get; set; }

    [DataMember(EmitDefaultValue = true)]
    public bool ContinueOnError { get; set; }

    public List<string> Multipliers
    {
      get
      {
        if (this.m_multipliers == null)
          this.m_multipliers = new List<string>();
        return this.m_multipliers;
      }
      set => this.m_multipliers = new List<string>((IEnumerable<string>) value);
    }

    [System.Runtime.Serialization.OnDeserialized]
    private void OnDeserialized(StreamingContext context) => SerializationHelper.Copy<string>(ref this.m_serializedMultipliers, ref this.m_multipliers, true);

    [System.Runtime.Serialization.OnSerializing]
    private void OnSerializing(StreamingContext context) => SerializationHelper.Copy<string>(ref this.m_multipliers, ref this.m_serializedMultipliers);
  }
}
