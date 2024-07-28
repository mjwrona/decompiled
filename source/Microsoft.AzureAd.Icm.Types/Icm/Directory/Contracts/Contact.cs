// Decompiled with JetBrains decompiler
// Type: Microsoft.Icm.Directory.Contracts.Contact
// Assembly: Microsoft.AzureAd.Icm.Types, Version=2.1.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 6A852DFE-F17D-49CB-9E7D-8AB8112703DB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.AzureAd.Icm.Types.dll

using System.Collections.Generic;

namespace Microsoft.Icm.Directory.Contracts
{
  public class Contact : ModifiableDocument
  {
    private ICollection<PhoneNumber> phoneNumbers;

    public string Alias { get; set; }

    public string EmailAddress { get; set; }

    public string Upn { get; set; }

    public string FirstName { get; set; }

    public string MiddleName { get; set; }

    public string LastName { get; set; }

    public string Status { get; set; }

    public string TimeZoneId { get; set; }

    public string Discipline { get; set; }

    public ICollection<PhoneNumber> PhoneNumbers
    {
      get => this.phoneNumbers ?? (this.phoneNumbers = (ICollection<PhoneNumber>) new List<PhoneNumber>());
      set => this.phoneNumbers = value;
    }

    public string FullName { get; set; }
  }
}
