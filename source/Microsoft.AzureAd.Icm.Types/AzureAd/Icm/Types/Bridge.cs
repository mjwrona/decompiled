// Decompiled with JetBrains decompiler
// Type: Microsoft.AzureAd.Icm.Types.Bridge
// Assembly: Microsoft.AzureAd.Icm.Types, Version=2.1.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 6A852DFE-F17D-49CB-9E7D-8AB8112703DB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.AzureAd.Icm.Types.dll

using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Microsoft.AzureAd.Icm.Types
{
  [DataContract]
  public class Bridge
  {
    [DataMember(Name = "BridgeURI")]
    public string BridgeURI { get; set; }

    [Key]
    [DataMember(Name = "Id")]
    public long BridgeNumber { get; set; }

    [DataMember(Name = "BridgeConfId")]
    public string BridgeConfId { get; set; }

    [DataMember(Name = "ExpirationDate")]
    public DateTime ExpirationDate { get; set; }

    [DataMember(Name = "PhoneNumber")]
    public string PhoneNumber { get; set; }
  }
}
