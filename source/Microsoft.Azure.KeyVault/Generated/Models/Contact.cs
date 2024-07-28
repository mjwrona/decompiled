// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.KeyVault.Models.Contact
// Assembly: Microsoft.Azure.KeyVault, Version=3.0.5.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 594DACFC-3846-4701-8E31-E06E75D35FE9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.KeyVault.dll

using Newtonsoft.Json;

namespace Microsoft.Azure.KeyVault.Models
{
  public class Contact
  {
    public Contact()
    {
    }

    public Contact(string emailAddress = null, string name = null, string phone = null)
    {
      this.EmailAddress = emailAddress;
      this.Name = name;
      this.Phone = phone;
    }

    [JsonProperty(PropertyName = "email")]
    public string EmailAddress { get; set; }

    [JsonProperty(PropertyName = "name")]
    public string Name { get; set; }

    [JsonProperty(PropertyName = "phone")]
    public string Phone { get; set; }
  }
}
