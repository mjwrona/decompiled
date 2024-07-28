// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Checks.WebApi.CheckConfigurationRef
// Assembly: Microsoft.Azure.Pipelines.Checks.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 381241F9-9196-42AF-BB4C-5187E3EFE32E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.Checks.WebApi.dll

using System.Runtime.Serialization;

namespace Microsoft.Azure.Pipelines.Checks.WebApi
{
  [DataContract]
  public class CheckConfigurationRef
  {
    private CheckType m_checkType;

    [DataMember(EmitDefaultValue = false)]
    public int Id { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public int Version { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public CheckType Type
    {
      get => this.m_checkType ?? (this.m_checkType = new CheckType());
      set => this.m_checkType = value;
    }

    [DataMember(EmitDefaultValue = false)]
    public string Url { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public Resource Resource { get; set; }
  }
}
