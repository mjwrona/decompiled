// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItemField
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3FA6C797-B300-46B2-A8C9-CFED891348F5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models
{
  [DataContract]
  public class WorkItemField : WorkItemTrackingResource
  {
    public WorkItemField()
    {
    }

    public WorkItemField(ISecuredObject securedObject)
      : base(securedObject)
    {
    }

    [DataMember]
    public string Name { get; set; }

    [DataMember]
    public string ReferenceName { get; set; }

    [DataMember]
    public string Description { get; set; }

    [DataMember]
    public FieldType Type { get; set; }

    [DataMember]
    public FieldUsage Usage { get; set; }

    [DataMember]
    public bool ReadOnly { get; set; }

    [DataMember]
    public bool CanSortBy { get; set; }

    [DataMember]
    public bool IsQueryable { get; set; }

    [DataMember]
    public IEnumerable<WorkItemFieldOperation> SupportedOperations { get; set; }

    [DataMember]
    public bool IsIdentity { get; set; }

    [DataMember]
    public bool IsPicklist { get; set; }

    [DataMember]
    public bool IsPicklistSuggested { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public Guid? PicklistId { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public bool IsDeleted { get; set; }
  }
}
