// Decompiled with JetBrains decompiler
// Type: Microsoft.AzureAd.Icm.Types.PirControl
// Assembly: Microsoft.AzureAd.Icm.Types, Version=2.1.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 6A852DFE-F17D-49CB-9E7D-8AB8112703DB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.AzureAd.Icm.Types.dll

using System.Runtime.Serialization;

namespace Microsoft.AzureAd.Icm.Types
{
  [DataContract]
  public class PirControl
  {
    public PirControl(
      string name,
      string prefix,
      PirStatus requiredStatus,
      string requiredMessage)
    {
      this.Name = name;
      this.Prefix = prefix;
      this.RequiredStatus = requiredStatus;
      this.RequiredMessage = requiredMessage;
    }

    [DataMember(Name = "Name")]
    public string Name { get; set; }

    [DataMember(Name = "Prefix")]
    public string Prefix { get; set; }

    [DataMember(Name = "RequiredStatus")]
    public PirStatus RequiredStatus { get; set; }

    [DataMember(Name = "RequiredMessage")]
    public string RequiredMessage { get; set; }
  }
}
