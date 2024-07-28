// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Models.WorkItemTypeModel
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 74AD14A4-225D-46D2-B154-945941A2D167
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Models
{
  [DataContract]
  public class WorkItemTypeModel
  {
    public WorkItemTypeModel(IVssRequestContext requestContext, IWorkItemType workItemType)
    {
      this.Name = workItemType.Name;
      this.ReferenceName = workItemType.ReferenceName;
      this.Color = workItemType.Color;
    }

    [DataMember]
    public string Name { get; set; }

    [DataMember]
    public string ReferenceName { get; set; }

    [DataMember]
    public IEnumerable<WorkItemFieldModel> Fields { get; set; }

    [DataMember]
    public string Color { get; set; }

    [DataMember]
    public Rules Rules { get; set; }
  }
}
