// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Alerts.DeliveryPreferenceModel
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Alerts, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0FF2CB39-6514-430A-A4E9-A45535A580D6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Alerts.dll

using Microsoft.VisualStudio.Services.Notifications.Common;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Server.WebAccess.Alerts
{
  [DataContract]
  internal class DeliveryPreferenceModel
  {
    public DeliveryPreferenceModel()
    {
    }

    public DeliveryPreferenceModel(DeliveryPreference deliveryPreference)
    {
      this.Address = deliveryPreference.Address;
      this.Schedule = deliveryPreference.Schedule;
      this.Type = deliveryPreference.Type;
    }

    [DataMember(Name = "address", EmitDefaultValue = false)]
    public string Address { get; set; }

    [DataMember(Name = "schedule")]
    public DeliverySchedule Schedule { get; set; }

    [DataMember(Name = "type")]
    public DeliveryType Type { get; set; }

    public DeliveryPreference GetPreference() => new DeliveryPreference()
    {
      Address = this.Address == null ? (string) null : this.Address.Trim(),
      Schedule = this.Schedule,
      Type = this.Type
    };
  }
}
