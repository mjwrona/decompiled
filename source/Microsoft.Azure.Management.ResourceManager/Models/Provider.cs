// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Management.ResourceManager.Models.Provider
// Assembly: Microsoft.Azure.Management.ResourceManager, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: ABBAD935-2366-4053-A43B-1C3AE5FDB3D3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Management.ResourceManager.dll

using Newtonsoft.Json;
using System.Collections.Generic;

namespace Microsoft.Azure.Management.ResourceManager.Models
{
  public class Provider
  {
    public Provider()
    {
    }

    public Provider(
      string id = null,
      string namespaceProperty = null,
      string registrationState = null,
      string registrationPolicy = null,
      IList<ProviderResourceType> resourceTypes = null)
    {
      this.Id = id;
      this.NamespaceProperty = namespaceProperty;
      this.RegistrationState = registrationState;
      this.RegistrationPolicy = registrationPolicy;
      this.ResourceTypes = resourceTypes;
    }

    [JsonProperty(PropertyName = "id")]
    public string Id { get; private set; }

    [JsonProperty(PropertyName = "namespace")]
    public string NamespaceProperty { get; set; }

    [JsonProperty(PropertyName = "registrationState")]
    public string RegistrationState { get; private set; }

    [JsonProperty(PropertyName = "registrationPolicy")]
    public string RegistrationPolicy { get; private set; }

    [JsonProperty(PropertyName = "resourceTypes")]
    public IList<ProviderResourceType> ResourceTypes { get; private set; }
  }
}
