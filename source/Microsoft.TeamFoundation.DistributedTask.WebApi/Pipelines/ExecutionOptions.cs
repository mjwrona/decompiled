// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.ExecutionOptions
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines
{
  [DataContract]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class ExecutionOptions
  {
    [DataMember(Name = "SystemTokenCustomClaims", EmitDefaultValue = false)]
    private IDictionary<string, string> m_systemTokenCustomClaims;

    [DataMember(EmitDefaultValue = false)]
    public bool RestrictSecrets { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string SystemTokenScope { get; set; }

    public IDictionary<string, string> SystemTokenCustomClaims
    {
      get
      {
        if (this.m_systemTokenCustomClaims == null)
          this.m_systemTokenCustomClaims = (IDictionary<string, string>) new Dictionary<string, string>();
        return this.m_systemTokenCustomClaims;
      }
    }

    [DataMember(EmitDefaultValue = false)]
    public int? MaxJobExpansion { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public int? MaxParallelism { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public int? MaxHostedJobTimeout { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public bool EnableResourceExpressions { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public bool EnforceLegalNodeNames { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public bool DisallowWideCharactersInNodeNames { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public bool AllowHyphenNames { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public bool RunJobsWithDemandsOnSingleHostedPool { get; set; }
  }
}
