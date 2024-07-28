// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.WebApi.ExtensionShare
// Assembly: Microsoft.VisualStudio.Services.Gallery.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EE9D0AAA-B110-4AD6-813B-50FA04AC401A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Gallery.WebApi.dll

using Newtonsoft.Json;

namespace Microsoft.VisualStudio.Services.Gallery.WebApi
{
  public class ExtensionShare
  {
    public const string SharedAccount = "account";
    public const string SharedOrganization = "organization";

    public string Id { get; set; }

    public string Type { get; set; }

    public string Name { get; set; }

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public bool? IsOrg { get; set; }

    public ExtensionShare ShallowCopy() => (ExtensionShare) this.MemberwiseClone();
  }
}
