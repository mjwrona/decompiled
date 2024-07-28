// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Web.Models.AdminBehaviorField
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CCDEBCB9-DB6B-41A2-8797-78C2455AE321
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Web.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.WorkItemTracking.Web.Models
{
  [DataContract]
  public class AdminBehaviorField
  {
    [DataMember]
    public string BehaviorFieldId { get; set; }

    [DataMember]
    public string Id { get; set; }

    [DataMember]
    public string Name { get; set; }

    public static IReadOnlyCollection<AdminBehaviorField> Create(
      IVssRequestContext requestContext,
      Behavior behavior)
    {
      return (IReadOnlyCollection<AdminBehaviorField>) behavior.GetLegacyCombinedFields(requestContext).Select<KeyValuePair<string, ProcessFieldDefinition>, AdminBehaviorField>((Func<KeyValuePair<string, ProcessFieldDefinition>, AdminBehaviorField>) (kv => new AdminBehaviorField()
      {
        BehaviorFieldId = kv.Key,
        Id = kv.Value.ReferenceName,
        Name = kv.Value.Name
      })).ToList<AdminBehaviorField>();
    }
  }
}
