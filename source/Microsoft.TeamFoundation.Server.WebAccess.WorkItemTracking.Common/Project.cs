// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Project
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using Microsoft.TeamFoundation.Core.WebApi;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common
{
  public class Project : IComparable<Project>
  {
    public string Name { get; set; }

    public int Id { get; set; }

    public Guid Guid { get; set; }

    public Guid ProcessTemplateId { get; set; }

    public ProjectVisibility Visibility { get; set; }

    internal static Project Create(TreeNode node) => new Project()
    {
      Name = node.Name,
      Id = node.Id,
      Guid = node.Guid
    };

    public JsObject ToJson(
      IEnumerable<string> workItemTypeNames,
      IEnumerable<int> fieldIds,
      object extras)
    {
      JsObject json = new JsObject();
      json["id"] = (object) this.Id;
      json["name"] = (object) this.Name;
      json["guid"] = (object) this.Guid;
      json["visibility"] = (object) this.Visibility;
      if (workItemTypeNames != null)
        json["workItemTypes"] = (object) workItemTypeNames;
      if (fieldIds != null)
        json[nameof (fieldIds)] = (object) fieldIds;
      if (extras != null)
        json[nameof (extras)] = extras;
      return json;
    }

    public int CompareTo(Project other) => TFStringComparer.TeamProjectName.Compare(this.Name, other.Name);
  }
}
