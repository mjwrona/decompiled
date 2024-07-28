// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.WebApi.TestRunExtended2
// Assembly: Microsoft.TeamFoundation.TestManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 10F0A812-3ECA-42B4-865D-429941F99EBE
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.TestManagement.WebApi.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.TestManagement.WebApi
{
  [DataContract]
  public sealed class TestRunExtended2
  {
    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public Guid ProjectId { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public int TestRunId { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public byte? Substate { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public string SourceFilter { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public string TestCaseFilter { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public string TestEnvironmentUrl { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public string AutEnvironmentUrl { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public string CsmContent { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public string CsmParameters { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public string SubscriptionName { get; set; }
  }
}
