// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.KeyVault.Models.Contacts
// Assembly: Microsoft.Azure.KeyVault, Version=3.0.5.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 594DACFC-3846-4701-8E31-E06E75D35FE9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.KeyVault.dll

using Newtonsoft.Json;
using System.Collections.Generic;

namespace Microsoft.Azure.KeyVault.Models
{
  public class Contacts
  {
    public Contacts()
    {
    }

    public Contacts(string id = null, IList<Contact> contactList = null)
    {
      this.Id = id;
      this.ContactList = contactList;
    }

    [JsonProperty(PropertyName = "id")]
    public string Id { get; private set; }

    [JsonProperty(PropertyName = "contacts")]
    public IList<Contact> ContactList { get; set; }
  }
}
