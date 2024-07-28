// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3FA6C797-B300-46B2-A8C9-CFED891348F5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.VisualStudio.Services.WebApi.Patch;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models
{
  [DataContract]
  [SkipValidateTypeForRemovePatchOperation]
  public class WorkItem : WorkItemTrackingResource
  {
    [DataMember(EmitDefaultValue = false)]
    public int? Id { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public int? Rev { get; set; }

    [DataMember(EmitDefaultValue = false)]
    [JsonConverter(typeof (WorkItemFieldDictionaryConverter))]
    public IDictionary<string, object> Fields { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public IList<WorkItemRelation> Relations { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public WorkItemCommentVersionRef CommentVersionRef { get; set; }

    public WorkItem() => this.Fields = (IDictionary<string, object>) new Dictionary<string, object>();

    public WorkItem(ISecuredObject securedObject)
      : base(securedObject)
    {
      this.Fields = (IDictionary<string, object>) new Dictionary<string, object>();
    }
  }
}
