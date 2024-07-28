// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.ExternalProviders.GitHub.WebApi.Models.Event.GitHubAppInstallation
// Assembly: Microsoft.VisualStudio.ExternalProviders.GitHub.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4FE25D33-B783-4B98-BAFC-7E522D8D8D08
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.ExternalProviders.GitHub.WebApi.dll

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.ExternalProviders.GitHub.WebApi.Models.Event
{
  [DataContract]
  public class GitHubAppInstallation
  {
    [DataMember]
    public int Id { get; set; }

    [DataMember]
    public Account Account { get; set; }

    [DataMember(Name = "repository_selection")]
    public RepositorySelection RepositorySelection { get; set; }

    [DataMember(Name = "access_tokens_url")]
    public string AccessTokensUrl { get; set; }

    [DataMember(Name = "repositories_url")]
    public string RepositoriesUrl { get; set; }

    [DataMember(Name = "html_url")]
    public string HtmlUrl { get; set; }

    [DataMember(Name = "app_id")]
    public int AppId { get; set; }

    [DataMember(Name = "target_id")]
    public int TargetId { get; set; }

    [DataMember(Name = "target_type")]
    public string TargetType { get; set; }

    [DataMember]
    public Dictionary<string, string> Permissions { get; set; }

    [DataMember]
    public List<string> Events { get; set; }

    [DataMember(Name = "created_at")]
    public string CreatedAt { get; set; }

    [DataMember(Name = "updated_at")]
    public string UpdatedAt { get; set; }

    [DataMember(Name = "single_file_name")]
    public string SingleFileName { get; set; }
  }
}
