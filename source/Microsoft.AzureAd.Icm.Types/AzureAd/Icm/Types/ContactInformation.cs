// Decompiled with JetBrains decompiler
// Type: Microsoft.AzureAd.Icm.Types.ContactInformation
// Assembly: Microsoft.AzureAd.Icm.Types, Version=2.1.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 6A852DFE-F17D-49CB-9E7D-8AB8112703DB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.AzureAd.Icm.Types.dll

using System;

namespace Microsoft.AzureAd.Icm.Types
{
  [Serializable]
  public class ContactInformation
  {
    public string FirstName { get; set; }

    public string LastName { get; set; }

    public string Alias { get; set; }

    public string WorkPhone { get; set; }

    public bool IsWorkPhoneInUSCanada { get; set; }

    public string HomePhone { get; set; }

    public bool IsHomePhoneInUSCanada { get; set; }

    public string CellPhone { get; set; }

    public bool IsCellPhoneInUSCanada { get; set; }

    public bool EnableSmsForCellPhone { get; set; }

    public string EmailAddress { get; set; }

    public string Discipline { get; set; }

    public string TimeZoneId { get; set; }

    public string CompanyName { get; set; }
  }
}
