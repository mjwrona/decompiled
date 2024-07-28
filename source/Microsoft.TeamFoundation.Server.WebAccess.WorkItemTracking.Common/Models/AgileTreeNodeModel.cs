// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.AgileTreeNodeModel
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Internals;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models
{
  [DataContract]
  public class AgileTreeNodeModel
  {
    public AgileTreeNodeModel(IVssRequestContext requestContext, Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.TreeNode node)
    {
      this.StructureType = node.Type == TreeStructureType.Area ? "ProjectModelHierarchy" : "ProjectLifecycle";
      this.Name = node.GetName(requestContext);
      this.Path = node.GetCssPath(requestContext);
      this.FriendlyPath = node.GetPath(requestContext);
      this.StartDate = node.StartDate;
      this.FinishDate = node.FinishDate;
      this.Id = node.CssNodeId;
      this.NodeId = node.Id;
      this.ParentId = node.Parent.CssNodeId;
      List<string> values = new List<string>();
      values.Add(this.Name);
      DateTime? nullable = node.StartDate;
      if (nullable.HasValue)
      {
        nullable = node.StartDate;
        if (nullable.Value != DateTime.MinValue)
        {
          List<string> stringList1 = values;
          nullable = node.StartDate;
          string str1 = nullable.Value.ToString("d", (IFormatProvider) DateTimeFormatInfo.CurrentInfo);
          stringList1.Add(str1);
          nullable = node.FinishDate;
          if (nullable.HasValue)
          {
            nullable = node.FinishDate;
            if (nullable.Value != DateTime.MinValue)
            {
              List<string> stringList2 = values;
              nullable = node.FinishDate;
              string str2 = nullable.Value.ToString("d", (IFormatProvider) DateTimeFormatInfo.CurrentInfo);
              stringList2.Add(str2);
              goto label_6;
            }
          }
          values.Add("?");
        }
      }
label_6:
      this.DisplayText = string.Join(" - ", (IEnumerable<string>) values);
    }

    [DataMember(Name = "structureType", EmitDefaultValue = false)]
    public string StructureType { get; set; }

    [DataMember(Name = "name", EmitDefaultValue = false)]
    public string Name { get; set; }

    [DataMember(Name = "path", EmitDefaultValue = false)]
    public string Path { get; set; }

    [DataMember(Name = "friendlyPath", EmitDefaultValue = false)]
    public string FriendlyPath { get; set; }

    [DataMember(Name = "startDate", EmitDefaultValue = false)]
    public DateTime? StartDate { get; set; }

    [DataMember(Name = "finishDate", EmitDefaultValue = false)]
    public DateTime? FinishDate { get; set; }

    [DataMember(Name = "id", EmitDefaultValue = false)]
    public Guid Id { get; set; }

    [DataMember(Name = "parentId", EmitDefaultValue = false)]
    public Guid ParentId { get; set; }

    [DataMember(Name = "displayText", EmitDefaultValue = false)]
    public string DisplayText { get; set; }

    [DataMember(Name = "nodeId", EmitDefaultValue = false)]
    public int NodeId { get; set; }
  }
}
