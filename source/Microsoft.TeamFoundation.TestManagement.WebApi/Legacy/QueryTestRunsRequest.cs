// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.QueryTestRunsRequest
// Assembly: Microsoft.TeamFoundation.TestManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 10F0A812-3ECA-42B4-865D-429941F99EBE
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.TestManagement.WebApi.dll

using System;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.TestManagement.WebApi.Legacy
{
  [DataContract]
  public class QueryTestRunsRequest
  {
    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public int TestRunId { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public Guid Owner { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public string BuildUri { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public string TeamProjectName { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    [DefaultValue(-1)]
    public int PlanId { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    [DefaultValue(0)]
    public int Skip { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    [DefaultValue(2147483647)]
    public int Top { get; set; }
  }
}
