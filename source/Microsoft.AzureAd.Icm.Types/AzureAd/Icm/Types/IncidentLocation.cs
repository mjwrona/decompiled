// Decompiled with JetBrains decompiler
// Type: Microsoft.AzureAd.Icm.Types.IncidentLocation
// Assembly: Microsoft.AzureAd.Icm.Types, Version=2.1.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 6A852DFE-F17D-49CB-9E7D-8AB8112703DB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.AzureAd.Icm.Types.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.AzureAd.Icm.Types
{
  [DataContract]
  public class IncidentLocation : IEquatable<IncidentLocation>
  {
    [DataMember]
    public string Environment { get; set; }

    [DataMember]
    public string DataCenter { get; set; }

    [DataMember]
    public string DeviceGroup { get; set; }

    [DataMember]
    public string DeviceName { get; set; }

    [DataMember]
    public string ServiceInstanceId { get; set; }

    public static void ThrowIfNotValid(IncidentLocation location)
    {
      TypeUtility.ValidateStringParameter(location.Environment, "location.Environment", 1, 32, true);
      TypeUtility.ValidateStringParameter(location.DataCenter, "location.DataCenter", 1, 32, true);
      TypeUtility.ValidateStringParameter(location.DeviceGroup, "location.DeviceGroup", 1, 64, true);
      TypeUtility.ValidateStringParameter(location.DeviceName, "location.DeviceName", 1, 128, true);
      TypeUtility.ValidateStringParameter(location.ServiceInstanceId, "location.ServiceInstanceId", 1, 64, true);
    }

    public bool Equals(IncidentLocation other) => other != null && string.Equals(this.DataCenter, other.DataCenter, StringComparison.OrdinalIgnoreCase) && string.Equals(this.DeviceGroup, other.DeviceGroup, StringComparison.OrdinalIgnoreCase) && string.Equals(this.DeviceName, other.DeviceName, StringComparison.OrdinalIgnoreCase) && string.Equals(this.Environment, other.Environment, StringComparison.OrdinalIgnoreCase) && string.Equals(this.ServiceInstanceId, other.ServiceInstanceId, StringComparison.OrdinalIgnoreCase);
  }
}
