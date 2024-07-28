// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItemDeleteReference
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3FA6C797-B300-46B2-A8C9-CFED891348F5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.WebApi.dll

using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models
{
  [DataContract]
  public class WorkItemDeleteReference
  {
    [DataMember(EmitDefaultValue = false)]
    public int? Id { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string Type { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string Name { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string Project { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string DeletedDate { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string DeletedBy { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public int? Code { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string Message { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string Url { get; set; }
  }
}
