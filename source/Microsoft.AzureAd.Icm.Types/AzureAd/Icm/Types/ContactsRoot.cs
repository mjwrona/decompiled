// Decompiled with JetBrains decompiler
// Type: Microsoft.AzureAd.Icm.Types.ContactsRoot
// Assembly: Microsoft.AzureAd.Icm.Types, Version=2.1.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 6A852DFE-F17D-49CB-9E7D-8AB8112703DB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.AzureAd.Icm.Types.dll

using System;
using System.Collections.ObjectModel;
using System.Xml.Serialization;

namespace Microsoft.AzureAd.Icm.Types
{
  [XmlRoot("Contacts")]
  [Serializable]
  public class ContactsRoot
  {
    [XmlAttribute("Tenant")]
    public string Tenant { get; set; }

    [XmlAttribute("ClaimProviderId")]
    public string ClaimProvider { get; set; }

    [XmlElement("Contact")]
    public Collection<ContactInformation> Contacts { get; set; }
  }
}
