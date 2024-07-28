// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.SocialWorkItem.Models.WorkItemProjectModel
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.SocialWorkItem.Models
{
  [DataContract]
  public class WorkItemProjectModel : BaseSecuredObjectModel
  {
    public WorkItemProjectModel(ISecuredObject securedObject)
      : base(securedObject)
    {
    }

    [DataMember(Name = "name")]
    public string Name { get; set; }

    [DataMember(Name = "id")]
    public int Id { get; set; }

    [DataMember(Name = "guid")]
    public Guid Guid { get; set; }

    [DataMember(Name = "workItemTypes")]
    public IEnumerable<WorkItemTypeModel> WorkItemTypes { get; set; }
  }
}
