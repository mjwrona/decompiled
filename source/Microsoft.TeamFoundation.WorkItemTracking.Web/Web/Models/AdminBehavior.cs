// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Web.Models.AdminBehavior
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CCDEBCB9-DB6B-41A2-8797-78C2455AE321
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Web.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.WorkItemTracking.Web.Models
{
  [DataContract]
  public class AdminBehavior
  {
    [DataMember]
    public string Id { get; set; }

    [DataMember]
    public string Description { get; set; }

    [DataMember]
    public bool Abstract { get; set; }

    [DataMember]
    public bool Overriden { get; set; }

    [DataMember]
    public bool Custom { get; set; }

    [DataMember]
    public string Name { get; set; }

    [DataMember]
    public string Color { get; set; }

    [DataMember]
    public string Icon { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public IEnumerable<AdminBehaviorField> Fields { get; set; }

    [DataMember]
    public string Inherits { get; set; }

    [DataMember]
    public int Rank { get; set; }

    public static AdminBehavior Create(IVssRequestContext requestContext, Behavior behavior) => new AdminBehavior()
    {
      Name = behavior.Name,
      Description = behavior.Description,
      Id = behavior.ReferenceName,
      Abstract = behavior.IsAbstract,
      Overriden = behavior.Overridden,
      Custom = behavior.Custom,
      Color = behavior.Color,
      Icon = behavior.Icon,
      Fields = (IEnumerable<AdminBehaviorField>) AdminBehaviorField.Create(requestContext, behavior),
      Rank = behavior.Rank,
      Inherits = behavior.ParentBehaviorReferenceName
    };
  }
}
