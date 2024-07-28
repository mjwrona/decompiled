// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.WorkItemModel
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using Microsoft.Azure.Devops.Tags.Server.Models;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models
{
  [DataContract]
  public class WorkItemModel : BaseSecuredObjectModel
  {
    protected WorkItemModel(ISecuredObject securedObject)
      : base(securedObject)
    {
    }

    public int Id { get; protected set; }

    [DataMember]
    public int Revision { get; protected set; }

    [DataMember]
    public DateTime LoadTime { get; protected set; }

    public IReadOnlyDictionary<int, object> LatestData { get; protected set; }

    [DataMember]
    public IEnumerable<TagDefinitionModel> Tags { get; protected set; }

    [DataMember]
    public IEnumerable<SecuredDictionary<string, object>> Files { get; protected set; }

    [DataMember]
    public IEnumerable<SecuredDictionary<string, object>> Relations { get; protected set; }

    [DataMember]
    public IEnumerable<SecuredDictionary<string, object>> RelationRevisions { get; protected set; }

    [DataMember]
    public Dictionary<int, WitIdentityRef> ReferencedPersons { get; set; }

    [DataMember]
    public IEnumerable<Guid> CurrentExtensions { get; protected set; }

    public IReadOnlyCollection<IReadOnlyDictionary<int, object>> RevisionData { get; protected set; }

    [DataMember]
    public ReferencedNodes ReferencedNodes { get; protected set; }

    [DataMember]
    public bool IsReadOnly { get; protected set; }

    [DataMember]
    public Guid ProjectId { get; protected set; }

    [DataMember]
    public Dictionary<string, object> Fields { get; set; }

    [DataMember]
    public Dictionary<int, WorkItemCommentVersionModel> CommentVersions { get; set; }

    [DataMember]
    public IEnumerable<Dictionary<string, object>> Revisions { get; set; }

    [DataMember]
    public bool isCommentingAvailable { get; protected set; }
  }
}
