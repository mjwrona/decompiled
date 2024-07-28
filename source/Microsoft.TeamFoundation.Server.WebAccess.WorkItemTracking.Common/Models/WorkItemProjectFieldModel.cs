// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.WorkItemProjectFieldModel
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models
{
  [DataContract]
  public class WorkItemProjectFieldModel : BaseSecuredObjectModel
  {
    public WorkItemProjectFieldModel(
      WorkItemProjectModel project,
      IEnumerable<FieldDefinition> fields,
      ISecuredObject securedObject)
      : base(securedObject)
    {
      this.Projects = (IEnumerable<WorkItemProjectModel>) new WorkItemProjectModel[1]
      {
        project
      };
      this.Fields = fields.Select<FieldDefinition, WorkItemFieldModel>((Func<FieldDefinition, WorkItemFieldModel>) (f => new WorkItemFieldModel(f, securedObject)));
    }

    [DataMember]
    public IEnumerable<WorkItemProjectModel> Projects { get; set; }

    [DataMember]
    public IEnumerable<WorkItemFieldModel> Fields { get; set; }
  }
}
