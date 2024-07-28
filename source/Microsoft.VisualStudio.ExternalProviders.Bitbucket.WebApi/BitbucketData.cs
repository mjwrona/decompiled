// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.ExternalProviders.Bitbucket.WebApi.BitbucketData
// Assembly: Microsoft.VisualStudio.ExternalProviders.Bitbucket.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 223C9BE7-A3E9-431B-86B7-A81B8A6447FF
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.ExternalProviders.Bitbucket.WebApi.dll

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.ExternalProviders.Bitbucket.WebApi
{
  public static class BitbucketData
  {
    public static class V2
    {
      public class ErrorResponse
      {
        [JsonProperty("error")]
        public BitbucketData.V2.ErrorMessage Error { get; set; }
      }

      public class ErrorMessage
      {
        [JsonProperty("message")]
        public string Message { get; set; }
      }

      public class PagedResponse<T>
      {
        [JsonProperty("next")]
        public string Next { get; set; }

        [JsonProperty("previous")]
        public string Previous { get; set; }

        [JsonProperty("pagelen")]
        public int PageLength { get; set; }

        [JsonProperty("size")]
        public int Size { get; set; }

        [JsonProperty("page")]
        public int Page { get; set; }

        [JsonProperty("values")]
        public T[] Values { get; set; }
      }

      public class Repository
      {
        [JsonProperty("uuid")]
        public Guid Id { get; set; }

        [JsonProperty("scm")]
        public string Scm { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("links")]
        public BitbucketData.V2.RepositoryLinks Links { get; set; }

        [JsonProperty("fork_policy")]
        public string ForkPolicy { get; set; }

        [JsonProperty("language")]
        public string Language { get; set; }

        [JsonProperty("mainbranch")]
        public BitbucketData.V2.Branch MainBranch { get; set; }

        [JsonProperty("full_name")]
        public string FullName { get; set; }

        [JsonProperty("owner")]
        public BitbucketData.V2.User Owner { get; set; }

        [JsonProperty("updated_on")]
        public DateTime UpdatedOn { get; set; }

        [JsonProperty("size")]
        public long Size { get; set; }

        [JsonProperty("slug")]
        public string Slug { get; set; }

        [JsonProperty("is_private")]
        public bool IsPrivate { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("parent")]
        public BitbucketData.V2.Repository Parent { get; set; }
      }

      public class RepositoryLinks
      {
        [JsonProperty("watchers")]
        public BitbucketData.V2.Link Watchers { get; set; }

        [JsonProperty("branches")]
        public BitbucketData.V2.Link Branches { get; set; }

        [JsonProperty("tags")]
        public BitbucketData.V2.Link Tags { get; set; }

        [JsonProperty("commits")]
        public BitbucketData.V2.Link Commits { get; set; }

        [JsonProperty("clone")]
        public BitbucketData.V2.Link[] Clone { get; set; }

        [JsonProperty("self")]
        public BitbucketData.V2.Link Self { get; set; }

        [JsonProperty("html")]
        public BitbucketData.V2.Link Html { get; set; }

        [JsonProperty("avatar")]
        public BitbucketData.V2.Link Avatar { get; set; }

        [JsonProperty("hooks")]
        public BitbucketData.V2.Link Hooks { get; set; }

        [JsonProperty("forkes")]
        public BitbucketData.V2.Link Forkes { get; set; }

        [JsonProperty("downloads")]
        public BitbucketData.V2.Link Downloads { get; set; }

        [JsonProperty("pullrequests")]
        public BitbucketData.V2.Link PullRequests { get; set; }
      }

      public class User
      {
        [JsonProperty("uuid")]
        public Guid Id { get; set; }

        [JsonProperty("account_id")]
        public string AccountId { get; set; }

        [JsonProperty("nickname")]
        public string Nickname { get; set; }

        [JsonProperty("display_name")]
        public string DisplayName { get; set; }

        [JsonProperty("created_on")]
        public string CreatedOn { get; set; }

        [JsonProperty("links")]
        public Dictionary<string, BitbucketData.V2.Link> Links { get; set; }
      }

      public class AuthenticatedUser : BitbucketData.V2.User
      {
        [JsonProperty("username")]
        public string Username { get; set; }
      }

      public class Link
      {
        [JsonProperty("href")]
        public string Href { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
      }

      public class Branch
      {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("links")]
        public Dictionary<string, BitbucketData.V2.Link> Links { get; set; }

        [JsonProperty("target")]
        public BitbucketData.V2.Commit Target { get; set; }
      }

      public class Commit
      {
        [JsonProperty("hash")]
        public string Hash { get; set; }

        [JsonProperty("links")]
        public Dictionary<string, BitbucketData.V2.Link> Links { get; set; }

        [JsonProperty("repository")]
        public BitbucketData.V2.Repository Repository { get; set; }

        [JsonProperty("author")]
        public BitbucketData.V2.Author Author { get; set; }

        [JsonProperty("parents")]
        public BitbucketData.V2.Commit[] Parents { get; set; }

        [JsonProperty("date")]
        public DateTime Date { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }
      }

      public class FileDiff
      {
        [JsonProperty("old")]
        public BitbucketData.V2.ContentData Old { get; set; }

        [JsonProperty("new")]
        public BitbucketData.V2.ContentData New { get; set; }
      }

      public class DiffStat
      {
        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("lines_removed")]
        public int LinesRemoved { get; set; }

        [JsonProperty("lines_added")]
        public int LinesAdded { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("old")]
        public BitbucketData.V2.ContentData Old { get; set; }

        [JsonProperty("new")]
        public BitbucketData.V2.ContentData New { get; set; }
      }

      public class Author
      {
        [JsonProperty("raw")]
        public string Raw { get; set; }

        [JsonProperty("user")]
        public BitbucketData.V2.User User { get; set; }
      }

      public class PushEvent
      {
        [JsonProperty("push")]
        public BitbucketData.V2.Push Push { get; set; }

        [JsonProperty("repository")]
        public BitbucketData.V2.Repository Repository { get; set; }

        [JsonProperty("actor")]
        public BitbucketData.V2.User Actor { get; set; }
      }

      public class Push
      {
        [JsonProperty("changes")]
        public BitbucketData.V2.Change[] Changes { get; set; }
      }

      public class Change
      {
        [JsonProperty("forced")]
        public bool Forced { get; set; }

        [JsonProperty("created")]
        public bool Created { get; set; }

        [JsonProperty("truncated")]
        public bool Truncated { get; set; }

        [JsonProperty("closed")]
        public bool Closed { get; set; }

        [JsonProperty("old")]
        public BitbucketData.V2.Branch Old { get; set; }

        [JsonProperty("new")]
        public BitbucketData.V2.Branch New { get; set; }

        [JsonProperty("commits")]
        public BitbucketData.V2.Commit[] Commits { get; set; }

        [JsonProperty("links")]
        public Dictionary<string, BitbucketData.V2.Link> Links { get; set; }
      }

      public class Hook
      {
        [JsonProperty("uuid")]
        public string Id { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("links")]
        public Dictionary<string, BitbucketData.V2.Link> Links { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("created_at")]
        public DateTime CreatedAt { get; set; }

        [JsonProperty("skip_cert_verification")]
        public bool SkipCertVerification { get; set; }

        [JsonProperty("active")]
        public bool Active { get; set; }

        [JsonProperty("subject")]
        public BitbucketData.V2.Repository Subject { get; set; }

        [JsonProperty("events")]
        public string[] Events { get; set; }
      }

      public class Build
      {
        [JsonProperty("key")]
        public string Key { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("repository")]
        public BitbucketData.V2.Repository Repository { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("state")]
        [JsonConverter(typeof (StringEnumConverter))]
        public BitbucketData.V2.BuildState State { get; set; }

        [JsonProperty("created_on")]
        public DateTime CreatedOn { get; set; }

        [JsonProperty("updated_on")]
        public DateTime UpdatedOn { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }
      }

      public enum BuildState
      {
        [EnumMember(Value = "SUCCESSFUL")] Successful,
        [EnumMember(Value = "FAILED")] Failed,
        [EnumMember(Value = "INPROGRESS")] InProgress,
        [EnumMember(Value = "STOPPED")] Stopped,
      }

      public class PullRequestEvent
      {
        [JsonProperty("pullrequest")]
        public BitbucketData.V2.PullRequest PullRequest { get; set; }

        [JsonProperty("repository")]
        public BitbucketData.V2.Repository Repository { get; set; }

        [JsonProperty("actor")]
        public BitbucketData.V2.User Actor { get; set; }
      }

      public class PullRequest
      {
        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("links")]
        public Dictionary<string, BitbucketData.V2.Link> Links { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("destination")]
        public BitbucketData.V2.Target Destination { get; set; }

        [JsonProperty("closed_by")]
        public BitbucketData.V2.User ClosedBy { get; set; }

        [JsonProperty("source")]
        public BitbucketData.V2.Target Source { get; set; }

        [JsonProperty("created_on")]
        public DateTime CreatedOn { get; set; }

        [JsonProperty("state")]
        public string State { get; set; }

        [JsonProperty("author")]
        public BitbucketData.V2.User Author { get; set; }

        [JsonProperty("reason")]
        public string Reason { get; set; }

        [JsonProperty("updated_on")]
        public DateTime UpdatedOn { get; set; }

        [JsonProperty("merge_commit")]
        public BitbucketData.V2.Commit MergeCommit { get; set; }

        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("task_count")]
        public int TaskCount { get; set; }
      }

      public class Tag
      {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("links")]
        public Dictionary<string, BitbucketData.V2.Link> Links { get; set; }

        [JsonProperty("tagger")]
        public BitbucketData.V2.Author Tagger { get; set; }

        [JsonProperty("target")]
        public BitbucketData.V2.Commit Target { get; set; }
      }

      public class Target
      {
        [JsonProperty("commit")]
        public BitbucketData.V2.Commit Commit { get; set; }

        [JsonProperty("branch")]
        public BitbucketData.V2.Branch Branch { get; set; }

        [JsonProperty("repository")]
        public BitbucketData.V2.Repository Repository { get; set; }
      }

      public class ContentData
      {
        [JsonProperty("path")]
        public string Path { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("links")]
        public Dictionary<string, BitbucketData.V2.Link> Links { get; set; }

        [JsonProperty("commit")]
        public BitbucketData.V2.Commit Commit { get; set; }
      }
    }

    public class Authorization
    {
      [JsonProperty("access_token")]
      public string AccessToken { get; set; }

      [JsonProperty("scopes")]
      public string Scopes { get; set; }

      [JsonProperty("token_type")]
      public string TokenType { get; set; }

      [JsonProperty("expires_in")]
      public int ExpiresIn { get; set; }

      [JsonProperty("refresh_token")]
      public string RefreshToken { get; set; }
    }

    public class AuthorizationError
    {
      [JsonProperty("error", Required = Required.Always)]
      public string Error { get; set; }

      [JsonProperty("error_description")]
      public string Description { get; set; }

      [JsonProperty("error_uri")]
      public Uri Uri { get; set; }
    }

    public class Authentication
    {
      public Authentication(string username, string password)
      {
        this.Username = username;
        this.Password = password;
      }

      public Authentication(string accessToken) => this.AccessToken = accessToken;

      public string AccessToken { get; }

      public string Username { get; }

      public string Password { get; }
    }
  }
}
