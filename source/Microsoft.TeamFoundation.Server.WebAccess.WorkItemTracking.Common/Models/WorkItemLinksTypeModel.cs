// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.WorkItemLinksTypeModel
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models
{
  [DataContract]
  public class WorkItemLinksTypeModel : BaseSecuredObjectModel
  {
    public WorkItemLinksTypeModel(ISecuredObject securedObject)
      : base(securedObject)
    {
    }

    [DataMember]
    public string ReferenceName { get; set; }

    [DataMember]
    public string Topology { get; set; }

    [DataMember]
    public bool CanDelete { get; set; }

    [DataMember]
    public bool CanEdit { get; set; }

    [DataMember]
    public bool IsActive { get; set; }

    [DataMember]
    public bool IsDirectional { get; set; }

    [DataMember]
    public bool IsNonCircular { get; set; }

    [DataMember]
    public bool IsOneToMany { get; set; }

    [DataMember]
    public WorkItemLinkTypeEndModel ForwardEnd { get; set; }

    [DataMember]
    public WorkItemLinkTypeEndModel ReverseEnd { get; set; }

    [DataMember]
    public bool IsRemote { get; set; }
  }
}
