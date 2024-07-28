// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestImpact.WebApi.Contracts.DefinitionRunInfo
// Assembly: Microsoft.TeamFoundation.TestImpact.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7794F4D7-E029-40EA-A47C-F590EB851438
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestImpact.WebApi.dll

using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.TestImpact.WebApi.Contracts
{
  [DataContract]
  public class DefinitionRunInfo
  {
    [DataMember(EmitDefaultValue = false, IsRequired = false)]
    public int DefinitionRunId { get; set; }

    [DataMember(EmitDefaultValue = false, IsRequired = false)]
    public DefinitionType DefinitionType { get; set; }

    [DataMember(EmitDefaultValue = false, IsRequired = false)]
    public int BaseLineDefinitionRunId { get; set; }
  }
}
