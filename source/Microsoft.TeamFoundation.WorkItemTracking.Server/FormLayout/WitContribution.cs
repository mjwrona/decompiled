// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.FormLayout.WitContribution
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.FormLayout
{
  [DataContract]
  public class WitContribution
  {
    public WitContribution(
      string contributionId,
      bool showOnDeletedWorkItem = false,
      IDictionary<string, object> inputs = null,
      int height = 0,
      string layoutInstanceId = null)
    {
      this.ContributionId = contributionId;
      this.ShowOnDeletedWorkItem = showOnDeletedWorkItem;
      this.Inputs = inputs;
      this.Height = height;
      this.LayoutInstanceId = layoutInstanceId;
    }

    [DataMember(Name = "contributionId", EmitDefaultValue = false)]
    public string ContributionId { get; set; }

    [DataMember(Name = "height", EmitDefaultValue = false)]
    public int Height { get; set; }

    [DataMember(Name = "showOnDeletedWorkItem", EmitDefaultValue = false)]
    public bool ShowOnDeletedWorkItem { get; set; }

    [DataMember(Name = "layoutInstanceId", EmitDefaultValue = false)]
    public string LayoutInstanceId { get; set; }

    [DataMember(Name = "inputs")]
    public IDictionary<string, object> Inputs { get; set; }

    public WitContribution Clone() => new WitContribution(this.ContributionId, this.ShowOnDeletedWorkItem, this.Inputs, this.Height, this.LayoutInstanceId);
  }
}
