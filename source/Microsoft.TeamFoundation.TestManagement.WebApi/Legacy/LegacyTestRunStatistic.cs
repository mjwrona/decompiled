// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.LegacyTestRunStatistic
// Assembly: Microsoft.TeamFoundation.TestManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 10F0A812-3ECA-42B4-865D-429941F99EBE
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.TestManagement.WebApi.dll

using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.TestManagement.WebApi.Legacy
{
  [DataContract]
  public sealed class LegacyTestRunStatistic
  {
    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public int TestRunId { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public byte State { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public byte Outcome { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public TestResolutionState ResolutionState { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public int Count { get; set; }
  }
}
