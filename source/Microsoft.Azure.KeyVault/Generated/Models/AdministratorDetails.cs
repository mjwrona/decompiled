// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.KeyVault.Models.AdministratorDetails
// Assembly: Microsoft.Azure.KeyVault, Version=3.0.5.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 594DACFC-3846-4701-8E31-E06E75D35FE9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.KeyVault.dll

using Newtonsoft.Json;

namespace Microsoft.Azure.KeyVault.Models
{
  public class AdministratorDetails
  {
    public AdministratorDetails()
    {
    }

    public AdministratorDetails(
      string firstName = null,
      string lastName = null,
      string emailAddress = null,
      string phone = null)
    {
      this.FirstName = firstName;
      this.LastName = lastName;
      this.EmailAddress = emailAddress;
      this.Phone = phone;
    }

    [JsonProperty(PropertyName = "first_name")]
    public string FirstName { get; set; }

    [JsonProperty(PropertyName = "last_name")]
    public string LastName { get; set; }

    [JsonProperty(PropertyName = "email")]
    public string EmailAddress { get; set; }

    [JsonProperty(PropertyName = "phone")]
    public string Phone { get; set; }
  }
}
