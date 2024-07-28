// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.WebPreview.Model.WorkItemUpdate
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CCDEBCB9-DB6B-41A2-8797-78C2455AE321
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Web.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.WorkItemTracking.WebPreview.Model
{
  [DataContract]
  public class WorkItemUpdate
  {
    [DataMember]
    public int Id { get; set; }

    [DataMember]
    public int Rev { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public IEnumerable<FieldValue> Fields { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public IEnumerable<WorkItemLinkUpdate> Links { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public IEnumerable<WorkItemResourceLinkUpdate> ResourceLinks { get; set; }

    public Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.WorkItemUpdate ToWorkItemUpdate(
      IVssRequestContext tfsRequestContext)
    {
      return new Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.WorkItemUpdate()
      {
        Id = this.Id,
        Rev = this.Rev,
        Fields = this.Fields == null || !this.Fields.Any<FieldValue>() ? (IEnumerable<KeyValuePair<string, object>>) (KeyValuePair<string, object>[]) null : (IEnumerable<KeyValuePair<string, object>>) this.Fields.Select<FieldValue, KeyValuePair<string, object>>((Func<FieldValue, KeyValuePair<string, object>>) (field => new KeyValuePair<string, object>(field.Field.RefName, field.Value))).ToArray<KeyValuePair<string, object>>(),
        LinkUpdates = this.Links == null || !this.Links.Any<WorkItemLinkUpdate>() ? (IEnumerable<Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.WorkItemLinkUpdate>) null : this.Links.Select<WorkItemLinkUpdate, Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.WorkItemLinkUpdate>((Func<WorkItemLinkUpdate, Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.WorkItemLinkUpdate>) (link => link.ToWorkItemLinkUpdate(tfsRequestContext))),
        ResourceLinkUpdates = this.ResourceLinks == null || !this.ResourceLinks.Any<WorkItemResourceLinkUpdate>() ? (IEnumerable<Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.WorkItemResourceLinkUpdate>) null : this.ResourceLinks.Select<WorkItemResourceLinkUpdate, Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.WorkItemResourceLinkUpdate>((Func<WorkItemResourceLinkUpdate, Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.WorkItemResourceLinkUpdate>) (link => link.ToWorkItemResourceLinkUpdate()))
      };
    }
  }
}
