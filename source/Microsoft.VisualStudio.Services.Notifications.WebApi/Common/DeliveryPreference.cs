// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.Common.DeliveryPreference
// Assembly: Microsoft.VisualStudio.Services.Notifications.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FF217E0A-7730-437B-BE9F-877363CB7392
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.WebApi.dll

namespace Microsoft.VisualStudio.Services.Notifications.Common
{
  public class DeliveryPreference
  {
    public DeliveryType Type { get; set; }

    public DeliverySchedule Schedule { get; set; }

    public string Address { get; set; }

    public override bool Equals(object that) => that is DeliveryPreference deliveryPreference && this.Type == deliveryPreference.Type && this.Schedule == deliveryPreference.Schedule && this.Address == deliveryPreference.Address;

    public override int GetHashCode() => this.Address != null ? this.Address.GetHashCode() : 0;
  }
}
