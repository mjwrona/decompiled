// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.ExternalProviders.GitHub.WebApi.Models.Event.Account
// Assembly: Microsoft.VisualStudio.ExternalProviders.GitHub.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4FE25D33-B783-4B98-BAFC-7E522D8D8D08
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.ExternalProviders.GitHub.WebApi.dll

using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.ExternalProviders.GitHub.WebApi.Models.Event
{
  [DataContract]
  public class Account
  {
    [DataMember]
    public string Login { get; set; }

    [DataMember]
    public int Id { get; set; }

    [DataMember(Name = "node_id")]
    public string NodeId { get; set; }

    [DataMember(Name = "avatar_url")]
    public string AvatarUrl { get; set; }

    [DataMember(Name = "gravatar_id")]
    public string GravatarId { get; set; }

    [DataMember]
    public string Url { get; set; }

    [DataMember(Name = "html_url")]
    public string HtmlUrl { get; set; }

    [DataMember(Name = "followers_url")]
    public string FollowersUrl { get; set; }

    [DataMember(Name = "following_url")]
    public string FollowingUrl { get; set; }

    [DataMember(Name = "gists_url")]
    public string GistsUrl { get; set; }

    [DataMember(Name = "starred_url")]
    public string StarredUrl { get; set; }

    [DataMember(Name = "subscriptions_url")]
    public string SubscriptionsUrl { get; set; }

    [DataMember(Name = "organizations_url")]
    public string OrganizationsUrl { get; set; }

    [DataMember(Name = "repos_url")]
    public string ReposUrl { get; set; }

    [DataMember(Name = "events_url")]
    public string EventsUrl { get; set; }

    [DataMember(Name = "received_events_url")]
    public string ReceivedEventsUrl { get; set; }

    [DataMember]
    public AccountType Type { get; set; }

    [DataMember(Name = "site_admin")]
    public bool SiteAdmin { get; set; }
  }
}
