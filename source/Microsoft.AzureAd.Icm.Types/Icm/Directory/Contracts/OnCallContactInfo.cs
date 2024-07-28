// Decompiled with JetBrains decompiler
// Type: Microsoft.Icm.Directory.Contracts.OnCallContactInfo
// Assembly: Microsoft.AzureAd.Icm.Types, Version=2.1.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 6A852DFE-F17D-49CB-9E7D-8AB8112703DB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.AzureAd.Icm.Types.dll

namespace Microsoft.Icm.Directory.Contracts
{
  public class OnCallContactInfo
  {
    public long ContactId { get; set; }

    public string Alias { get; set; }

    public string FirstName { get; set; }

    public string LastName { get; set; }

    public ContactMechanism ContactMechanism { get; set; }
  }
}
