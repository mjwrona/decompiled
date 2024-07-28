// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ExtensionManagement.WebApi.ContributionNodeQueryResult
// Assembly: Microsoft.VisualStudio.Services.ExtensionManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A4FCC2C3-B106-43A6-A409-E4BF8CFC545C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ExtensionManagement.WebApi.dll

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.ExtensionManagement.WebApi
{
  [DataContract]
  public class ContributionNodeQueryResult : DataProviderMetadata
  {
    public ContributionNodeQueryResult(string dataspaceId)
      : base(dataspaceId)
    {
      this.Nodes = (IDictionary<string, ClientContributionNode>) new Dictionary<string, ClientContributionNode>();
      this.ProviderDetails = (IDictionary<string, ClientContributionProviderDetails>) new Dictionary<string, ClientContributionProviderDetails>();
    }

    [DataMember(Name = "nodes")]
    public IDictionary<string, ClientContributionNode> Nodes { get; private set; }

    [DataMember(Name = "providerDetails")]
    public IDictionary<string, ClientContributionProviderDetails> ProviderDetails { get; private set; }
  }
}
