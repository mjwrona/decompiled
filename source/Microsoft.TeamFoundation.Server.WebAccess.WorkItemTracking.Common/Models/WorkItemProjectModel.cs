// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.WorkItemProjectModel
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models
{
  [DataContract]
  public class WorkItemProjectModel : BaseSecuredObjectModel
  {
    public WorkItemProjectModel(
      string projectName,
      int projectId,
      Guid projectGuid,
      IEnumerable<string> workItemTypeNames,
      IEnumerable<int> fieldIds,
      ISecuredObject securedObject)
      : base(securedObject)
    {
      this.Name = projectName;
      this.Id = projectId;
      this.Guid = projectGuid;
      this.WorkItemTypes = workItemTypeNames;
      this.FieldIds = fieldIds;
    }

    [DataMember]
    public string Name { get; set; }

    [DataMember]
    public int Id { get; set; }

    [DataMember]
    public Guid Guid { get; set; }

    [DataMember]
    public IEnumerable<string> WorkItemTypes { get; set; }

    [DataMember]
    public IEnumerable<int> FieldIds { get; set; }

    [DataMember]
    public ProcessDescriptorModel Process { get; set; }
  }
}
