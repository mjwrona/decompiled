// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.WebApi.FilterPointQuery
// Assembly: Microsoft.TeamFoundation.TestManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 10F0A812-3ECA-42B4-865D-429941F99EBE
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.TestManagement.WebApi.dll

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.TestManagement.WebApi
{
  [DataContract]
  public class FilterPointQuery
  {
    [DataMember(IsRequired = true, EmitDefaultValue = false)]
    public int PlanId { get; set; }

    [DataMember(IsRequired = true, EmitDefaultValue = false)]
    public List<byte> PointOutcome { get; set; }

    [DataMember(IsRequired = true, EmitDefaultValue = false)]
    public List<byte> ResultState { get; set; }

    [DataMember(IsRequired = true, EmitDefaultValue = false)]
    public List<int> PointIds { get; set; }
  }
}
