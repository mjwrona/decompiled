// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Boards.WebApi.Common.WorkItemType
// Assembly: Microsoft.Azure.Boards.WebApi.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FC99C479-6852-4E74-BCA4-2660760F9D83
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Boards.WebApi.Common.dll

using Microsoft.TeamFoundation.WorkItemTracking.Server.FormLayout;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.Azure.Boards.WebApi.Common
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

    [DataMember(Name = "Layout")]
    public Layout Layout { get; set; }

    [DataMember(Name = "IsCustomType")]
    public bool IsCustomType { get; set; }

    [DataMember(Name = "IsDisabled")]
    public bool IsDisabled { get; set; }
  }
}
