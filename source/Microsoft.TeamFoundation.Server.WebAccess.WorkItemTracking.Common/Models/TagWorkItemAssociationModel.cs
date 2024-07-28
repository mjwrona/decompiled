// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.TagWorkItemAssociationModel
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using Microsoft.Azure.Devops.Tags.Server.Models;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models
{
  [DataContract]
  public class TagWorkItemAssociationModel
  {
    public TagWorkItemAssociationModel(Microsoft.Azure.Devops.Tags.Server.TagDefinition td, IEnumerable<int> workItemIds)
    {
      this.TagDefinition = new TagDefinitionModel(td);
      this.WorkItemIds = workItemIds;
    }

    [DataMember(Name = "tagDefinition", EmitDefaultValue = false)]
    public TagDefinitionModel TagDefinition { get; set; }

    [DataMember(Name = "workitemIds", EmitDefaultValue = false)]
    public IEnumerable<int> WorkItemIds { get; set; }

    public JsObject ToJson()
    {
      JsObject json = new JsObject();
      json["tagDefinition"] = (object) this.TagDefinition.ToJson();
      json["workitemIds"] = (object) this.WorkItemIds;
      return json;
    }
  }
}
