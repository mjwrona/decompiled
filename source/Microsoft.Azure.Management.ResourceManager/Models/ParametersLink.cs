// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Management.ResourceManager.Models.ParametersLink
// Assembly: Microsoft.Azure.Management.ResourceManager, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: ABBAD935-2366-4053-A43B-1C3AE5FDB3D3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Management.ResourceManager.dll

using Microsoft.Rest;
using Newtonsoft.Json;

namespace Microsoft.Azure.Management.ResourceManager.Models
{
  public class ParametersLink
  {
    public ParametersLink()
    {
    }

    public ParametersLink(string uri, string contentVersion = null)
    {
      this.Uri = uri;
      this.ContentVersion = contentVersion;
    }

    [JsonProperty(PropertyName = "uri")]
    public string Uri { get; set; }

    [JsonProperty(PropertyName = "contentVersion")]
    public string ContentVersion { get; set; }

    public virtual void Validate()
    {
      if (this.Uri == null)
        throw new ValidationException(ValidationRules.CannotBeNull, "Uri");
    }
  }
}
