// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Test.WebApi.TestAgent
// Assembly: Microsoft.TeamFoundation.Test.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 17829F78-DAC0-47C1-AC4C-95D401C54448
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Test.WebApi.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Test.WebApi
{
  [DataContract]
  public sealed class TestAgent
  {
    [DataMember(IsRequired = true, EmitDefaultValue = false)]
    public int Id { get; set; }

    [DataMember(IsRequired = true, EmitDefaultValue = false)]
    public string Name { get; set; }

    [DataMember(IsRequired = true, EmitDefaultValue = false)]
    public ShallowReference DtlEnvironment { get; set; }

    [DataMember(IsRequired = true, EmitDefaultValue = false)]
    public ShallowReference DtlMachine { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string[] Capabilities { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public int TestRunId { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public DateTime LastHeartBeat { get; set; }
  }
}
