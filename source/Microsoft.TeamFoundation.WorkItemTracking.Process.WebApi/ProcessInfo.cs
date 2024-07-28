// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models.ProcessInfo
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 141BFF87-1CDC-4DC5-9A7C-F7D6EA031F34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.dll

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models
{
  [DataContract]
  public class ProcessInfo
  {
    [DataMember]
    public Guid TypeId { get; set; }

    [DataMember]
    public string Name { get; set; }

    [DataMember]
    public string ReferenceName { get; set; }

    [DataMember]
    public string Description { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public IEnumerable<ProjectReference> Projects { get; set; }

    [DataMember]
    public Guid ParentProcessTypeId { get; set; }

    [DataMember]
    public bool IsEnabled { get; set; }

    [DataMember]
    public bool IsDefault { get; set; }

    [DataMember]
    public CustomizationType CustomizationType { get; set; }
  }
}
