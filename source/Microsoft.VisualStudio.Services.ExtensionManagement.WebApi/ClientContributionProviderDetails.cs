// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ExtensionManagement.WebApi.ClientContributionProviderDetails
// Assembly: Microsoft.VisualStudio.Services.ExtensionManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A4FCC2C3-B106-43A6-A409-E4BF8CFC545C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ExtensionManagement.WebApi.dll

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.ExtensionManagement.WebApi
{
  [DataContract]
  public class ClientContributionProviderDetails : DataProviderMetadata
  {
    public ClientContributionProviderDetails(string dataspaceId)
      : base(dataspaceId)
    {
    }

    [DataMember(Name = "name")]
    public string Name { get; set; }

    [DataMember(Name = "displayName")]
    public string DisplayName { get; set; }

    [DataMember(Name = "version")]
    public string Version { get; set; }

    [DataMember(Name = "properties")]
    public Dictionary<string, string> Properties { get; set; }
  }
}
