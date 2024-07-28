// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ExtensionManagement.WebApi.ClientContributionNode
// Assembly: Microsoft.VisualStudio.Services.ExtensionManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A4FCC2C3-B106-43A6-A409-E4BF8CFC545C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ExtensionManagement.WebApi.dll

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.ExtensionManagement.WebApi
{
  [DataContract]
  public class ClientContributionNode : DataProviderMetadata
  {
    public ClientContributionNode(string dataspaceId)
      : base(dataspaceId)
    {
      this.Parents = new List<string>();
      this.Children = new List<string>();
    }

    [DataMember(Name = "contribution")]
    public ClientContribution Contribution { get; set; }

    [DataMember(Name = "parents")]
    public List<string> Parents { get; private set; }

    [DataMember(Name = "children")]
    public List<string> Children { get; private set; }
  }
}
