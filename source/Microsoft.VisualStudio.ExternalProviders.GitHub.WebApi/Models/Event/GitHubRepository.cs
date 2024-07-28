// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.ExternalProviders.GitHub.WebApi.Models.Event.GitHubRepository
// Assembly: Microsoft.VisualStudio.ExternalProviders.GitHub.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4FE25D33-B783-4B98-BAFC-7E522D8D8D08
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.ExternalProviders.GitHub.WebApi.dll

using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.ExternalProviders.GitHub.WebApi.Models.Event
{
  [DataContract]
  public class GitHubRepository
  {
    [DataMember]
    public int Id { get; set; }

    [DataMember(Name = "node_id")]
    public string NodeId { get; set; }

    [DataMember]
    public string Name { get; set; }

    [DataMember(Name = "full_name")]
    public string FullName { get; set; }

    [DataMember(Name = "html_url")]
    public string WebUrl { get; set; }

    [DataMember]
    public bool Private { get; set; }
  }
}
