// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.FormLayout.LayoutNode
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.FormLayout
{
  [DataContract]
  public abstract class LayoutNode : IRankable
  {
    [DataMember(Name = "id", EmitDefaultValue = false)]
    public virtual string Id { get; set; }

    [DataMember(Name = "inherited", EmitDefaultValue = false)]
    public virtual bool FromInheritedLayout { get; internal set; }

    [DataMember(Name = "overridden", EmitDefaultValue = false)]
    public virtual bool? Overridden { get; internal set; }

    [DataMember(Name = "rank", EmitDefaultValue = false)]
    public virtual int? Rank { get; set; }

    [DataMember(Name = "isContribution")]
    public virtual bool IsContribution => this.Contribution != null;

    [DataMember(Name = "visible", EmitDefaultValue = false)]
    public virtual bool? Visible { get; set; }

    [DataMember(Name = "contribution", EmitDefaultValue = false)]
    public virtual WitContribution Contribution { get; set; }

    public override string ToString() => !string.IsNullOrEmpty(this.Id) ? this.GetType().Name + ":" + this.Id : base.ToString();

    public bool IsInheritPlaceholderNode() => string.Equals(this.Id, "$inherited", StringComparison.OrdinalIgnoreCase);

    public bool IsEquivalentTo(IRankable that) => that is LayoutNode && string.Equals(this.Id, (that as LayoutNode).Id, StringComparison.OrdinalIgnoreCase);
  }
}
