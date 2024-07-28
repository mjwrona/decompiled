// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ExtensionManagement.WebApi.ClientContribution
// Assembly: Microsoft.VisualStudio.Services.ExtensionManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A4FCC2C3-B106-43A6-A409-E4BF8CFC545C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ExtensionManagement.WebApi.dll

using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.ExtensionManagement.WebApi
{
  [DataContract]
  public class ClientContribution : DataProviderMetadata
  {
    public ClientContribution(string dataspaceId)
      : base(dataspaceId)
    {
    }

    [DataMember(Name = "id", EmitDefaultValue = false)]
    public string Id { get; set; }

    [DataMember(Name = "description", EmitDefaultValue = false)]
    public string Description { get; set; }

    [DataMember(Name = "type", EmitDefaultValue = false)]
    public string Type { get; set; }

    [DataMember(Name = "targets", EmitDefaultValue = false)]
    public List<string> Targets { get; set; }

    [DataMember(Name = "properties", EmitDefaultValue = false)]
    public JObject Properties { get; set; }

    [DataMember(Name = "includes", EmitDefaultValue = false)]
    public List<string> Includes { get; set; }
  }
}
