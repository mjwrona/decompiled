// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.WebApi.CounterVariable
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.WebApi
{
  [DataContract]
  public class CounterVariable
  {
    [DataMember(Name = "prefix", EmitDefaultValue = false)]
    private readonly string m_prefix;
    [DataMember(Name = "seed", EmitDefaultValue = false)]
    private readonly int m_seed;
    [DataMember(Name = "value", EmitDefaultValue = false)]
    private readonly int m_value;

    public CounterVariable(string prefix, int seed, int value)
    {
      this.m_prefix = prefix;
      this.m_seed = seed;
      this.m_value = value;
    }

    public string Prefix => this.m_prefix;

    public int Seed => this.m_seed;

    public int Value => this.m_value;
  }
}
