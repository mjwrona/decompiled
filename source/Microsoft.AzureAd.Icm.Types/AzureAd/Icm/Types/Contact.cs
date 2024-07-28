// Decompiled with JetBrains decompiler
// Type: Microsoft.AzureAd.Icm.Types.Contact
// Assembly: Microsoft.AzureAd.Icm.Types, Version=2.1.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 6A852DFE-F17D-49CB-9E7D-8AB8112703DB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.AzureAd.Icm.Types.dll

using System.Runtime.Serialization;

namespace Microsoft.AzureAd.Icm.Types
{
  [DataContract]
  public class Contact
  {
    public string EmailAddress { get; set; }

    [DataMember]
    public string FirstName { get; set; }

    [DataMember]
    public string LastName { get; set; }

    [DataMember]
    public string Alias { get; set; }

    [DataMember]
    public string WorkPhone { get; set; }

    [DataMember]
    public string HomePhone { get; set; }

    [DataMember]
    public string CellPhone { get; set; }

    [DataMember]
    public RotationSlot Rotation { get; set; }

    public override string ToString() => this.Alias;
  }
}
