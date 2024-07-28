// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models.Process.WorkItemType
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 141BFF87-1CDC-4DC5-9A7C-F7D6EA031F34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.dll

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models.Process
{
  [DataContract]
  public class WorkItemType
  {
    [DataMember(Name = "Id")]
    public string Id { get; set; }

    [DataMember(Name = "Name")]
    public string Name { get; set; }

    [DataMember(Name = "Description")]
    public string Description { get; set; }

    [DataMember(Name = "Color")]
    public string Color { get; set; }

    [DataMember(Name = "ParentWorkItemTypeId")]
    public string ParentWorkItemTypeId { get; set; }

    [DataMember(Name = "LayoutGroups")]
    public List<LayoutGroup> LayoutGroups { get; internal set; }

    [DataMember(Name = "IsCustomType")]
    public bool IsCustomType { get; set; }

    [DataMember(Name = "IsDisabled")]
    public bool IsDisabled { get; set; }
  }
}
