// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.SocialWorkItem.Models.WorkItemTypeModel
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.Common;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.WebApi;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.SocialWorkItem.Models
{
  [DataContract]
  public class WorkItemTypeModel : BaseSecuredObjectModel
  {
    public WorkItemTypeModel(ISecuredObject securedObject)
      : base(securedObject)
    {
    }

    [DataMember(Name = "name")]
    public string Name { get; set; }

    [DataMember(Name = "referenceName")]
    public string ReferenceName { get; set; }

    [DataMember(Name = "description")]
    public string Description { get; set; }

    [DataMember(Name = "color")]
    public string Color { get; set; }

    [DataMember(Name = "icon")]
    public string Icon { get; set; }

    [DataMember(Name = "isDisabled")]
    public bool IsDisabled { get; set; }

    [DataMember(Name = "fields")]
    public IEnumerable<WorkItemTypeFieldModel> Fields { get; set; }

    [DataMember(Name = "stateColors")]
    public IEnumerable<WorkItemStateColor> StateColors { get; set; }

    [DataMember(Name = "layout")]
    public WorkItemLayout Layout { get; set; }
  }
}
