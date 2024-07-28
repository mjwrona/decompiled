// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.ExternalProviders.GitHub.WebApi.GitHubData
// Assembly: Microsoft.VisualStudio.ExternalProviders.GitHub.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4FE25D33-B783-4B98-BAFC-7E522D8D8D08
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.ExternalProviders.GitHub.WebApi.dll

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.ExternalProviders.GitHub.WebApi
{
  public class GitHubData
  {
    public class Action
    {
      [DataContract]
      public class JustInTimeToken
      {
        [DataMember(EmitDefaultValue = false)]
        public string Token { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public DateTime Expires_at { get; set; }
      }
    }

    public class V3
    {
      [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
      public class ValueAttributesTypeAttribute : Attribute
      {
        public ValueAttributesTypeAttribute(string name) => this.Name = name;

        public string Name { get; set; }
      }

      public abstract class Attributes
      {
      }

      [DataContract]
      public class CommitData
      {
        [DataMember(EmitDefaultValue = false)]
        public string Sha { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string Url { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string Html_url { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public GitHubData.V3.User Author { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public GitHubData.V3.User Committer { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public GitHubData.V3.Trees Tree { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string Message { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public GitHubData.V3.Trees[] Parents { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public GitHubData.V3.Commit Commit { get; set; }
      }

      [DataContract]
      public class CommitsDiff
      {
        [DataMember(EmitDefaultValue = false)]
        public string Url { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string Html_Url { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string Permalink_Url { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string Diff_Url { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string Patch_Url { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public GitHubData.V3.CommitData Base_Commit { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public GitHubData.V3.CommitData Merge_Base_Commit { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string Status { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public int Ahead_By { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public int Behind_By { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public int Total_Commits { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public GitHubData.V3.CommitListItem[] Commits { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public GitHubData.V3.File[] Files { get; set; }
      }

      [DataContract]
      public class CommitListItem
      {
        [DataMember(EmitDefaultValue = false)]
        public string Sha { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string Node_id { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string Url { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string Html_url { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string Comments_url { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public GitHubData.V3.CommitListItemData Commit { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public GitHubData.V3.Owner Author { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public GitHubData.V3.Owner Committer { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public GitHubData.V3.Trees[] Parents { get; set; }
      }

      [DataContract]
      public class CommitListItemData
      {
        [DataMember(EmitDefaultValue = false)]
        public GitHubData.V3.User Author { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public GitHubData.V3.User Committer { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string Message { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public GitHubData.V3.Trees Tree { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string Url { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public int Comment_count { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public GitHubData.V3.TagVerification Verification { get; set; }
      }

      [DataContract]
      public class File
      {
        [DataMember(EmitDefaultValue = false)]
        public string Sha { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string Filename { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string Status { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public int Additions { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public int Deletions { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public int Changes { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string BlobUrl { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string RawUrl { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string ContentsUrl { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string Patch { get; set; }
      }

      [DataContract]
      public class Hook
      {
        [DataMember(EmitDefaultValue = false)]
        public string Url { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string Test_url { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string Id { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string Name { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public bool Active { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public IList<string> Events { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public GitHubData.V3.HookConfig Config { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public GitHubData.V3.HookLastResponse Last_response { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string Updated_at { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string Created_at { get; set; }
      }

      [DataContract]
      public class HookConfig
      {
        [DataMember(EmitDefaultValue = false)]
        public string Url { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string Content_type { get; set; }
      }

      [DataContract]
      public class HookLastResponse
      {
        [DataMember(EmitDefaultValue = false)]
        public string Code { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string Status { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string Message { get; set; }
      }

      [DataContract]
      public class Status
      {
        [DataMember]
        public string Created_at { get; set; }

        [DataMember]
        public string Updated_at { get; set; }

        [DataMember]
        public string State { get; set; }

        [DataMember]
        public string Target_url { get; set; }

        [DataMember]
        public string Description { get; set; }

        [DataMember]
        public long? Id { get; set; }

        [DataMember]
        public string Url { get; set; }

        [DataMember]
        public string Context { get; set; }

        [DataMember]
        public GitHubData.V3.Owner Creator { get; set; }
      }

      [DataContract]
      public class CheckRun
      {
        [DataMember]
        public long? Id { get; set; }

        [DataMember]
        public string Sha { get; set; }

        [DataMember]
        public string External_id { get; set; }

        [DataMember]
        public string Url { get; set; }

        [DataMember]
        public string Html_url { get; set; }

        [DataMember]
        public string Status { get; set; }

        [DataMember]
        public string Conclusion { get; set; }

        [DataMember]
        public string Started_at { get; set; }

        [DataMember]
        public string Completed_at { get; set; }

        [DataMember]
        public GitHubData.V3.Output Output { get; set; }

        [DataMember]
        public GitHubData.V3.Owner Creator { get; set; }

        [DataMember]
        public GitHubData.V3.Check Check { get; set; }

        [DataMember]
        public GitHubData.V3.App App { get; set; }
      }

      [DataContract]
      public class Check
      {
        [DataMember]
        public long? Id { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public long? Check_suite_id { get; set; }

        [DataMember]
        public string Created_at { get; set; }

        [DataMember]
        public string Updated_at { get; set; }
      }

      [DataContract]
      public class Output
      {
        [DataMember]
        public string Title { get; set; }

        [DataMember]
        public string Summary { get; set; }

        [DataMember]
        public string Text { get; set; }

        [DataMember]
        public long? Annotations_count { get; set; }

        [DataMember]
        public string Annotations_url { get; set; }
      }

      [DataContract]
      public class Annotation
      {
        [DataMember]
        public string Path { get; set; }

        [DataMember]
        public long? Start_line { get; set; }

        [DataMember]
        public long? End_line { get; set; }

        [DataMember]
        public long? Start_column { get; set; }

        [DataMember]
        public long? End_column { get; set; }

        [DataMember]
        public string Annotation_level { get; set; }

        [DataMember]
        public string Message { get; set; }

        [DataMember]
        public string Title { get; set; }

        [DataMember]
        public string Raw_details { get; set; }
      }

      [DataContract]
      public class TaskValues
      {
        [DataMember]
        public string Id { get; set; }

        [DataMember]
        public GitHubData.V3.Value[] Inputs { get; set; }

        [DataMember]
        public GitHubData.V3.Value[] Outputs { get; set; }
      }

      [DataContract]
      [JsonConverter(typeof (GitHubValueJsonConverter))]
      public class Value
      {
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Type { get; set; }

        [DataMember]
        public GitHubData.V3.Attributes Attributes { get; set; }
      }

      [DataContract]
      [GitHubData.V3.ValueAttributesType("Scalar")]
      public class ScalarAttributes : GitHubData.V3.Attributes
      {
        [DataMember]
        public string String { get; set; }
      }

      [DataContract]
      [GitHubData.V3.ValueAttributesType("Docker")]
      public class DockerAttributes : GitHubData.V3.Attributes
      {
        [DataMember]
        public string Tag { get; set; }

        [DataMember]
        public GitHubData.V3.Registry Registry { get; set; }
      }

      [DataContract]
      public class Registry
      {
        [DataMember]
        public string Server { get; set; }
      }

      [DataContract]
      public class App
      {
        [DataMember]
        public long? Id { get; set; }

        [DataMember]
        public GitHubData.V3.Owner Owner { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Description { get; set; }

        [DataMember]
        public string External_url { get; set; }

        [DataMember]
        public string Html_url { get; set; }

        [DataMember]
        public string Created_at { get; set; }

        [DataMember]
        public string Updated_at { get; set; }
      }

      [DataContract]
      public class Push
      {
        [DataMember(EmitDefaultValue = false)]
        public string Ref { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string After { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string Before { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public bool Created { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public bool Deleted { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public bool Forced { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string Compare { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public List<GitHubData.V3.Commit> Commits { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public GitHubData.V3.Commit Head_commit { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public GitHubData.V3.Repository Repository { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public GitHubData.V3.Identity Pusher { get; set; }
      }

      [DataContract]
      public class Commit
      {
        [DataMember(EmitDefaultValue = false)]
        public string Id { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public bool Distinct { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string Message { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string Timestamp { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string Url { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public GitHubData.V3.Identity Author { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public GitHubData.V3.Identity Committer { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public List<string> Added { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public List<string> Removed { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public List<string> Modified { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public GitHubData.V3.Trees Tree { get; set; }
      }

      [DataContract]
      public class Identity
      {
        [DataMember(EmitDefaultValue = false)]
        public string Name { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string Email { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string Date { get; set; }
      }

      [DataContract]
      public class SearchUsersResult
      {
        [DataMember(EmitDefaultValue = false)]
        public int TotalCount;
        [DataMember(EmitDefaultValue = false)]
        public bool IncompleteResults;
        [DataMember(EmitDefaultValue = false)]
        public GitHubData.V3.User[] Items;
      }

      [DataContract]
      public class RepositorySearch
      {
        [DataMember(EmitDefaultValue = false)]
        public int TotalCount;
        [DataMember(EmitDefaultValue = false)]
        public bool IncompleteResults;
        [DataMember(EmitDefaultValue = false)]
        public GitHubData.V3.Repository[] Items;
      }

      [DataContract]
      public class Repository
      {
        [DataMember(EmitDefaultValue = false)]
        public int? Id { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string Node_Id { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string Name { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string Full_name { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public GitHubData.V3.Owner Owner { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public bool Private { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string Html_url { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string Description { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public bool Fork { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string Url { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string Forks_url { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string Keys_url { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string Collaborators_url { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string Teams_url { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string Hooks_url { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string Issue_events_url { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string Events_url { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string Assignees_url { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string Branches_url { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string Tags_url { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string Blobs_url { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string Git_tags_url { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string Git_refs_url { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string Trees_url { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string Statuses_url { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string Languages_url { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string Stargazers_url { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string Contributors_url { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string Subscribers_url { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string Subscription_url { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string Commits_url { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string Git_commits_url { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string Comments_url { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string Issue_comment_url { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string Contents_url { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string Compare_url { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string Merges_url { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string Archive_url { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string Downloads_url { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string Issues_url { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string Pulls_url { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string Milestones_url { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string Notifications_url { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string Labels_url { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string Releases_url { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string Created_at { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string Updated_at { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string Pushed_at { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string Git_url { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string Ssh_url { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string Clone_url { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string Svn_url { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string Homepage { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public int? Size { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public int? Stargazers_count { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public int? Watchers_count { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string Language { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public bool Has_issues { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public bool Has_downloads { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public bool Has_wiki { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public bool Has_pages { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public int? Forks_count { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string Mirror_url { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public int? Open_issues_count { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public int? Forks { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public int? Open_issues { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public int? Watchers { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string Default_branch { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public int? Stargazers { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string Master_branch { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public GitHubData.V3.RepositoryPermissions Permissions { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public bool Archived { get; set; }
      }

      [DataContract]
      public class RepositoryPermissions
      {
        [DataMember(EmitDefaultValue = false)]
        public bool Admin { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public bool Push { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public bool Pull { get; set; }
      }

      [DataContract]
      public class RepositoryLanguage
      {
        [DataMember(EmitDefaultValue = false)]
        public string Name { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public long ByteCount { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public bool IsPrimaryLanguage { get; set; }
      }

      [DataContract]
      public class Owner
      {
        [DataMember(EmitDefaultValue = false)]
        public string Login { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string Id { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string Node_id { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string Email { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string Name { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string Avatar_url { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string Gravatar_url { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string Gravatar_Id { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string Url { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string Html_url { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string Followers_url { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string Following_url { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string Gists_url { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string Starred_url { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string Subscriptions_url { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string Organizations_url { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string Repos_url { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string Events_url { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string Received_events_url { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string Type { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public bool Site_admin { get; set; }
      }

      [DataContract]
      public class Org
      {
        [DataMember(EmitDefaultValue = false)]
        public string Login { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string Id { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string Node_id { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string url { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string Repos_url { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string Events_url { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string Members_url { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string Public_members_url { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string Avatar_url { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string Description { get; set; }
      }

      [DataContract]
      public class OrgMembership
      {
        [DataMember(EmitDefaultValue = false)]
        public string url { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string State { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string Role { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string Organization_url { get; set; }

        [DataMember]
        public GitHubData.V3.Org Organization { get; set; }

        [DataMember]
        public GitHubData.V3.Owner User { get; set; }
      }

      [DataContract]
      public class Blob
      {
        [DataMember(EmitDefaultValue = false)]
        public string Path { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public int Mode { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string Type { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string Sha { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public long Size { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string Url { get; set; }
      }

      [DataContract]
      public class BlobObject
      {
        [DataMember(EmitDefaultValue = false)]
        public string Content { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string Encoding { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string Url { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string Sha { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public int Size { get; set; }
      }

      [DataContract]
      public class Ref
      {
        [DataMember(EmitDefaultValue = false, Name = "ref")]
        public string Ref_name { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string Url { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public GitHubData.V3.Blob Object { get; set; }
      }

      [DataContract]
      public class Branch
      {
        [DataMember(EmitDefaultValue = false)]
        public string Name { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public GitHubData.V3.Commit[] Commits { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "protected")]
        public bool IsProtected { get; set; }
      }

      [DataContract]
      public class User
      {
        [DataMember(EmitDefaultValue = false)]
        public string Login { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string Id { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string Name { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string Email { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string Date { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string Avatar_url { get; set; }
      }

      [DataContract]
      public class UserPermission
      {
        [DataMember(EmitDefaultValue = false)]
        public string Permission { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public GitHubData.V3.User User { get; set; }
      }

      [DataContract]
      public class Trees
      {
        [DataMember(EmitDefaultValue = false)]
        public string Sha { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string Url { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public GitHubData.V3.Blob[] Tree { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public bool Truncated { get; set; }
      }

      [DataContract]
      public class ContentData
      {
        [DataMember(EmitDefaultValue = false)]
        public string Name { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string Content { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string Encoding { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string Path { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string Sha { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public int Size { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string Url { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string Html_url { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string Git_url { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string Download_url { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string Type { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public GitHubData.V3.Links Links { get; set; }
      }

      [DataContract]
      public class ContentResponseData
      {
        [DataMember(EmitDefaultValue = false)]
        public GitHubData.V3.ContentData Content { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public GitHubData.V3.CommitData Commit { get; set; }
      }

      [DataContract]
      public class Links
      {
        [DataMember(EmitDefaultValue = false)]
        public string Self { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string Git { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string Html { get; set; }
      }

      [DataContract]
      public class TagData
      {
        [DataMember(EmitDefaultValue = false)]
        public string Tag { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string Sha { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string Url { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string Message { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public GitHubData.V3.User Tagger { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public GitHubData.V3.TaggedObject Object { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public GitHubData.V3.TagVerification Verification { get; set; }
      }

      [DataContract]
      public class TaggedObject
      {
        [DataMember(EmitDefaultValue = false)]
        public string Type { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string Sha { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string Url { get; set; }
      }

      [DataContract]
      public class TagVerification
      {
        [DataMember(EmitDefaultValue = false)]
        public bool Verified { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string Reason { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string Signature { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string Payload { get; set; }
      }

      [DataContract]
      public class InstallationDetails
      {
        [DataMember(EmitDefaultValue = false)]
        public string Id { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public GitHubData.V3.Org Account { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string Target_type { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string Html_url { get; set; }
      }

      [DataContract]
      public class UserInstallationDetails
      {
        [DataMember]
        public int Total_count;
        [DataMember]
        public GitHubData.V3.InstallationDetails[] Installations;
      }

      [DataContract]
      public class MarketplaceListing
      {
        [DataMember(EmitDefaultValue = false)]
        public string Url { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string Type { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string Id { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string Login { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string Email { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string Organization_billing_email { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public GitHubData.V3.MarketplacePurchase Marketplace_purchase { get; set; }
      }

      [DataContract]
      public class MarketplacePurchase
      {
        [DataMember(EmitDefaultValue = false)]
        public string Billing_cycle { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public DateTime? Next_billing_date { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public int Unit_count { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public DateTime Updated_at { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public GitHubData.V3.MarketplacePlan Plan { get; set; }
      }

      [DataContract]
      public class MarketplacePlan
      {
        [DataMember(EmitDefaultValue = false)]
        public string Url { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string Accounts_url { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string Id { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string Name { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string Description { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public int Monthly_price_in_cents { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public int Yearly_price_in_cents { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string Price_model { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string Unit_name { get; set; }
      }

      [DataContract]
      public class Assignee
      {
        [DataMember]
        public long? Id { get; set; }

        [DataMember]
        public string Login { get; set; }

        [DataMember]
        public string Url { get; set; }

        [DataMember]
        public string Html_url { get; set; }

        [DataMember]
        public string Avatar_url { get; set; }

        [DataMember]
        public string Type { get; set; }
      }

      [DataContract]
      public class Issue
      {
        [DataMember]
        public long? Id { get; set; }

        [DataMember]
        public string Title { get; set; }

        [DataMember]
        public string Url { get; set; }

        [DataMember]
        public string Html_url { get; set; }

        [DataMember]
        public string State { get; set; }

        [DataMember]
        public string Body { get; set; }

        [DataMember]
        public int Number { get; set; }

        [DataMember]
        public GitHubData.V3.Assignee Assignee { get; set; }

        [DataMember]
        public string Closed_at { get; set; }

        [DataMember]
        public string Created_at { get; set; }

        [DataMember]
        public string Updated_at { get; set; }

        [DataMember]
        public GitHubData.V3.IssuePullRequest Pull_request { get; set; }
      }

      [DataContract]
      public class IssuePullRequest
      {
        [DataMember]
        public string Url { get; set; }
      }

      [DataContract]
      public class PullRequest
      {
        [DataMember]
        public long Id { get; set; }

        [DataMember]
        public string Title { get; set; }

        [DataMember]
        public string Url { get; set; }

        [DataMember]
        public string Html_url { get; set; }

        [DataMember]
        public string State { get; set; }

        [DataMember]
        public bool Draft { get; set; }

        [DataMember]
        public string Body { get; set; }

        [DataMember]
        public int Number { get; set; }

        [DataMember]
        public GitHubData.V3.Owner User { get; set; }

        [DataMember]
        public GitHubData.V3.Assignee Assignee { get; set; }

        [DataMember]
        public string Closed_at { get; set; }

        [DataMember]
        public string Created_at { get; set; }

        [DataMember]
        public string Updated_at { get; set; }

        [DataMember]
        public GitHubData.V3.PullRequestBranchRef Base { get; set; }

        [DataMember]
        public GitHubData.V3.PullRequestBranchRef Head { get; set; }
      }

      [DataContract]
      public class PullRequestBranchRef
      {
        [DataMember]
        public string Label { get; set; }

        [DataMember]
        public string Ref { get; set; }

        [DataMember]
        public string Sha { get; set; }

        [DataMember]
        public GitHubData.V3.Owner User { get; set; }

        [DataMember]
        public GitHubData.V3.Repository Repo { get; set; }
      }

      [DataContract]
      public class DeploymentStatus
      {
        [DataMember]
        public string Url { get; set; }

        [DataMember]
        public long? Id { get; set; }

        [DataMember]
        public string Node_Id { get; set; }

        [DataMember]
        public string State { get; set; }

        [DataMember]
        public GitHubData.V3.Owner Creator { get; set; }

        [DataMember]
        public string Description { get; set; }

        [DataMember]
        public string Created_at { get; set; }

        [DataMember]
        public string Updated_at { get; set; }

        [DataMember]
        public string Deployment_url { get; set; }

        [DataMember]
        public string Repository_url { get; set; }
      }

      [DataContract]
      public class Comment
      {
        [DataMember]
        public long? Id { get; set; }

        [DataMember]
        public string Url { get; set; }

        [DataMember]
        public string Html_url { get; set; }

        [DataMember]
        public string Body { get; set; }

        [DataMember]
        public string Created_at { get; set; }

        [DataMember]
        public string Updated_at { get; set; }

        [DataMember]
        public GitHubData.V3.Owner User { get; set; }
      }

      [DataContract]
      public class SecretPublicKey
      {
        [DataMember]
        public string Key_id { get; set; }

        [DataMember]
        public string Key { get; set; }
      }

      [DataContract]
      public class Secret
      {
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Created_at { get; set; }

        [DataMember]
        public string Updated_at { get; set; }
      }

      [DataContract]
      public class EncryptedSecret
      {
        [DataMember]
        public string Key_id { get; set; }

        [DataMember]
        public string Encrypted_value { get; set; }
      }
    }

    [DataContract]
    public class OauthAuthorization
    {
      [DataMember]
      public string Access_token { get; set; }

      [DataMember]
      public string Scope { get; set; }

      [DataMember]
      public string Token_type { get; set; }

      [DataMember]
      public string Error { get; set; }

      [DataMember]
      public string Error_description { get; set; }

      [DataMember]
      public string Error_uri { get; set; }
    }

    [DataContract]
    public class InstallationRepositories
    {
      [DataMember]
      public int Total_count;
      [DataMember]
      public string Repository_selection;
      [DataMember]
      public GitHubData.V3.Repository[] repositories;
    }

    [DataContract]
    public class InstallationAccessToken
    {
      [DataMember]
      public string Token { get; set; }

      [DataMember]
      public DateTime Expires_at { get; set; }
    }

    public class V4
    {
      [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = false)]
      internal class GraphQLObjectAttribute : Attribute
      {
      }

      [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
      internal class GraphQLArgumentAttribute : Attribute
      {
        public string Name { get; set; }

        public Type Type { get; set; }

        public bool IsRequired { get; set; }
      }

      [AttributeUsage(AttributeTargets.Property | AttributeTargets.Interface, AllowMultiple = false)]
      internal class GraphQLQueryGenerationIgnoreAttribute : Attribute
      {
      }

      [GitHubData.V4.GraphQLObject]
      public interface IActor
      {
        [DataMember(EmitDefaultValue = false, Name = "avatarUrl")]
        Uri AvatarUrl { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "login")]
        string Login { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "resourcePath")]
        Uri ResourcePath { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "url")]
        Uri Url { get; set; }
      }

      [GitHubData.V4.GraphQLObject]
      public interface IRepositoryOwner
      {
        [DataMember(EmitDefaultValue = false, Name = "avatarUrl")]
        Uri AvatarUrl { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "id")]
        string Id { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "login")]
        string Login { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "resourcePath")]
        Uri ResourcePath { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "url")]
        Uri Url { get; set; }
      }

      [GitHubData.V4.GraphQLObject]
      public interface IGitObject
      {
        [DataMember(Name = "id")]
        string Id { get; set; }

        [GitHubData.V4.GraphQLQueryGenerationIgnore]
        [DataMember(EmitDefaultValue = false, Name = "oid")]
        string Oid { get; set; }
      }

      [GitHubData.V4.GraphQLObject]
      public interface IStatusCheckRollupContext
      {
        string Id { get; set; }
      }

      [DataContract]
      [GitHubData.V4.GraphQLObject]
      public class Bot : GitHubData.V4.IActor
      {
        [DataMember(EmitDefaultValue = false, Name = "avatarUrl")]
        public Uri AvatarUrl { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "id")]
        public string Id { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "login")]
        public string Login { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "resourcePath")]
        public Uri ResourcePath { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "url")]
        public Uri Url { get; set; }

        [GitHubData.V4.GraphQLQueryGenerationIgnore]
        [DataMember(EmitDefaultValue = false, Name = "updatedAt")]
        public DateTime UpdatedAt { get; set; }
      }

      [DataContract]
      [GitHubData.V4.GraphQLObject]
      public class Organization : GitHubData.V4.IRepositoryOwner, GitHubData.V4.IActor
      {
        [DataMember(EmitDefaultValue = false, Name = "avatarUrl")]
        public Uri AvatarUrl { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "id")]
        public string Id { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "login")]
        public string Login { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "resourcePath")]
        public Uri ResourcePath { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "url")]
        public Uri Url { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "name")]
        public string Name { get; set; }

        [GitHubData.V4.GraphQLQueryGenerationIgnore]
        [DataMember(EmitDefaultValue = false, Name = "updatedAt")]
        public DateTime UpdatedAt { get; set; }
      }

      [DataContract]
      [GitHubData.V4.GraphQLObject]
      public class User : GitHubData.V4.IRepositoryOwner, GitHubData.V4.IActor
      {
        [DataMember(EmitDefaultValue = false, Name = "avatarUrl")]
        public Uri AvatarUrl { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "id")]
        public string Id { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "login")]
        public string Login { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "name")]
        public string Name { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "resourcePath")]
        public Uri ResourcePath { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "url")]
        public Uri Url { get; set; }

        [GitHubData.V4.GraphQLQueryGenerationIgnore]
        [DataMember(EmitDefaultValue = false, Name = "updatedAt")]
        public DateTime UpdatedAt { get; set; }
      }

      [DataContract]
      [GitHubData.V4.GraphQLObject]
      public class Blob : GitHubData.V4.IGitObject
      {
        [DataMember(Name = "byteSize")]
        public int? ByteSize { get; set; }

        [DataMember(Name = "id")]
        public string Id { get; set; }

        [DataMember(Name = "isBinary")]
        public bool? IsBinary { get; set; }

        [GitHubData.V4.GraphQLQueryGenerationIgnore]
        [DataMember(EmitDefaultValue = false, Name = "oid")]
        public string Oid { get; set; }
      }

      [DataContract]
      [GitHubData.V4.GraphQLObject]
      public class CheckRun : GitHubData.V4.IStatusCheckRollupContext
      {
        [DataMember(Name = "completedAt")]
        public DateTime? CompletedAt { get; set; }

        [DataMember(Name = "conclusion")]
        public string Conclusion { get; set; }

        [DataMember(Name = "detailsUrl")]
        public string DetailsUrl { get; set; }

        [DataMember(Name = "externalId")]
        public string ExternalId { get; set; }

        [DataMember(Name = "id")]
        public string Id { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "startedAt")]
        public DateTime? StartedAt { get; set; }

        [DataMember(Name = "status")]
        public string Status { get; set; }

        [DataMember(Name = "checkSuite")]
        public GitHubData.V4.CheckSuite CheckSuite { get; set; }
      }

      [DataContract]
      [GitHubData.V4.GraphQLObject]
      public class CheckSuite
      {
        [DataMember(Name = "createdAt")]
        public DateTime? CreatedAt { get; set; }

        [DataMember(Name = "id")]
        public string Id { get; set; }

        [DataMember(Name = "updatedAt")]
        public DateTime? UpdatedAt { get; set; }

        [DataMember(Name = "workflowRun")]
        public GitHubData.V4.WorkflowRun WorkflowRun { get; set; }
      }

      [DataContract]
      [GitHubData.V4.GraphQLObject]
      public class Commit : GitHubData.V4.IGitObject
      {
        [DataMember(EmitDefaultValue = false, Name = "abbreviatedOid")]
        public string AbbreviatedOid { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "additions")]
        public int Additions { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "author")]
        public GitHubData.V4.GitActor Author { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "authoredByCommitter")]
        public bool AuthoredByCommitter { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "authoredDate")]
        public DateTime AuthoredDate { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "changedFiles")]
        public int ChangedFiles { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "commitResourcePath")]
        public Uri CommitResourcePath { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "commitUrl")]
        public Uri CommitUrl { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "committedDate")]
        public DateTime CommittedDate { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "committedViaWeb")]
        public bool CommittedViaWeb { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "committer")]
        public GitHubData.V4.GitActor Committer { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "deletions")]
        public int Deletions { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "id")]
        public string Id { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "message")]
        public string Message { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "messageBody")]
        public string MessageBody { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "messageBodyHTML")]
        public string MessageBodyHTML { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "messageHeadline")]
        public string MessageHeadline { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "messageHeadlineHTML")]
        public string MessageHeadlineHTML { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "oid")]
        public string Oid { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "pushedDate")]
        public DateTime? PushedDate { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "resourcePath")]
        public Uri ResourcePath { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "status")]
        public GitHubData.V4.Status Status { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "statusCheckRollup")]
        public GitHubData.V4.StatusCheckRollup StatusCheckRollup { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "tarballUrl")]
        public Uri TarballUrl { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "tree")]
        public GitHubData.V4.Tree Tree { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "treeResourcePath")]
        public Uri TreeResourcePath { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "treeUrl")]
        public Uri TreeUrl { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "url")]
        public Uri Url { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "viewerCanSubscribe")]
        public bool ViewerCanSubscribe { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "viewerSubscription")]
        public GitHubData.V4.SubscriptionState ViewerSubscription { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "zipballUrl")]
        public Uri ZipballUrl { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "repository")]
        public GitHubData.V4.Repository Repository { get; set; }

        [GitHubData.V4.GraphQLQueryGenerationIgnore]
        [DataMember(EmitDefaultValue = false, Name = "history")]
        public GitHubData.V4.Connection<GitHubData.V4.Commit> History { get; set; }

        [GitHubData.V4.GraphQLQueryGenerationIgnore]
        [DataMember(EmitDefaultValue = false, Name = "parents")]
        public GitHubData.V4.Connection<GitHubData.V4.Commit> Parents { get; set; }
      }

      [DataContract]
      [GitHubData.V4.GraphQLObject]
      public class CommitComment
      {
        [DataMember(EmitDefaultValue = false, Name = "id")]
        public string Id { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "databaseId")]
        public string DatabaseId { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "updatedAt")]
        public DateTime UpdatedAt { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "body")]
        public string Body { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "url")]
        public Uri Url { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "repository")]
        public GitHubData.V4.Repository Repository { get; set; }
      }

      [DataContract]
      [GitHubData.V4.GraphQLObject]
      public class GitActor
      {
        [DataMember(EmitDefaultValue = false, Name = "avatarUrl")]
        public Uri AvatarUrl { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "date")]
        public DateTime? Date { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "email")]
        public string Email { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "name")]
        public string Name { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "user")]
        public GitHubData.V4.User User { get; set; }
      }

      [DataContract]
      [GitHubData.V4.GraphQLObject]
      public class Edge<T>
      {
        [JsonExtensionData]
        [IgnoreDataMember]
        public IDictionary<string, JToken> AdditionalData;

        [DataMember(EmitDefaultValue = false, Name = "node")]
        public T Node { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "cursor")]
        public string Cursor { get; set; }

        public Edge() => this.AdditionalData = (IDictionary<string, JToken>) new Dictionary<string, JToken>();
      }

      [DataContract]
      [GitHubData.V4.GraphQLObject]
      public class PageInfo
      {
        [DataMember(Name = "startCursor")]
        public string StartCursor { get; set; }

        [DataMember(Name = "endCursor")]
        public string EndCursor { get; set; }

        [DataMember(Name = "hasNextPage")]
        public bool HasNextPage { get; set; }

        [DataMember(Name = "hasPreviousPage")]
        public bool HasPreviousPage { get; set; }
      }

      [DataContract]
      [GitHubData.V4.GraphQLObject]
      public class Connection<T>
      {
        [DataMember(EmitDefaultValue = false, Name = "edges")]
        public GitHubData.V4.Edge<T>[] Edges { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "nodes")]
        public T[] Nodes { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "pageInfo")]
        public GitHubData.V4.PageInfo PageInfo { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "totalCount")]
        public int TotalCount { get; set; }

        public void AppendEdges(GitHubData.V4.Edge<T>[] additionalEdges)
        {
          if (additionalEdges.Length == 0)
            return;
          this.Edges = ((IEnumerable<GitHubData.V4.Edge<T>>) this.Edges).Concat<GitHubData.V4.Edge<T>>((IEnumerable<GitHubData.V4.Edge<T>>) additionalEdges).ToArray<GitHubData.V4.Edge<T>>();
        }

        public void AppendPage(GitHubData.V4.Connection<T> nextPage)
        {
          this.AppendEdges(nextPage.Edges);
          this.PageInfo = nextPage.PageInfo;
        }
      }

      [DataContract]
      [GitHubData.V4.GraphQLObject]
      public class Issue
      {
        [DataMember(EmitDefaultValue = false, Name = "number")]
        public string Number { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "id")]
        public string Id { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "title")]
        public string Title { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "body")]
        public string Body { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "state")]
        public GitHubData.V4.IssueState State { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "url")]
        public Uri Url { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "assignees")]
        public GitHubData.V4.Connection<GitHubData.V4.User> Assignees { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "author")]
        public GitHubData.V4.IActor Author { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "createdAt")]
        public DateTime CreatedAt { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "updatedAt")]
        public DateTime UpdatedAt { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "closedAt")]
        public DateTime? ClosedAt { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "repository")]
        public GitHubData.V4.Repository Repository { get; set; }
      }

      [DataContract]
      [GitHubData.V4.GraphQLObject]
      public class MergeCondition
      {
        [DataMember(Name = "displayName")]
        public string DisplayName { get; set; }

        [DataMember(Name = "description")]
        public string Description { get; set; }

        [DataMember(Name = "message")]
        public string Message { get; set; }

        [DataMember(Name = "result")]
        public string Result { get; set; }
      }

      [DataContract]
      [GitHubData.V4.GraphQLObject]
      public class MergeRequirements
      {
        [DataMember(Name = "state")]
        public string State { get; set; }

        [DataMember(Name = "commitAuthor")]
        public string CommitAuthor { get; set; }

        [GitHubData.V4.GraphQLQueryGenerationIgnore]
        [DataMember(EmitDefaultValue = false, Name = "conditions")]
        public GitHubData.V4.MergeCondition[] Conditions { get; set; }
      }

      [DataContract]
      [GitHubData.V4.GraphQLObject]
      public class PullRequestReview
      {
        [GitHubData.V4.GraphQLQueryGenerationIgnore]
        [DataMember(EmitDefaultValue = false, Name = "id")]
        public string Id { get; set; }
      }

      [DataContract]
      [GitHubData.V4.GraphQLObject]
      public class PullRequest
      {
        [DataMember(EmitDefaultValue = false, Name = "participants")]
        public GitHubData.V4.Connection<GitHubData.V4.User> Participants;

        [DataMember(EmitDefaultValue = false, Name = "number")]
        public string Number { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "id")]
        public string Id { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "databaseId")]
        public int DatabaseId { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "title")]
        public string Title { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "body")]
        public string Body { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "baseRefName")]
        public string BaseRefName { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "baseRefOid")]
        public string BaseRefOid { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "headRefName")]
        public string HeadRefName { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "headRef")]
        public GitHubData.V4.Ref HeadRef { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "headRefOid")]
        public string HeadRefOid { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "state")]
        public GitHubData.V4.PullRequestState State { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "url")]
        public Uri Url { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "createdAt")]
        public DateTime CreatedAt { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "updatedAt")]
        public DateTime UpdatedAt { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "mergedAt")]
        public DateTime? MergedAt { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "closedAt")]
        public DateTime? ClosedAt { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "isDraft")]
        public bool Draft { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "author")]
        public GitHubData.V4.IActor Author { get; set; }

        [GitHubData.V4.GraphQLQueryGenerationIgnore]
        [DataMember(EmitDefaultValue = false, Name = "mergedBy")]
        public GitHubData.V4.IActor MergedBy { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "repository")]
        public GitHubData.V4.Repository Repository { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "headRepository")]
        public GitHubData.V4.Repository HeadRepository { get; set; }

        [GitHubData.V4.GraphQLQueryGenerationIgnore]
        [DataMember(EmitDefaultValue = false, Name = "baseRepository")]
        public GitHubData.V4.Repository BaseRepository { get; set; }

        [GitHubData.V4.GraphQLQueryGenerationIgnore]
        [DataMember(EmitDefaultValue = false, Name = "baseRef")]
        public GitHubData.V4.Ref BaseRef { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "commits")]
        public GitHubData.V4.Connection<GitHubData.V4.PullRequestCommit> Commits { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "reviewDecision")]
        public GitHubData.V4.ReviewDecision? ReviewDecision { get; set; }

        [GitHubData.V4.GraphQLQueryGenerationIgnore]
        [DataMember(EmitDefaultValue = false, Name = "reviews")]
        public GitHubData.V4.Connection<GitHubData.V4.PullRequestReview> Reviews { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "mergeable")]
        public GitHubData.V4.MergeableState MergeState { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "mergeCommit")]
        public GitHubData.V4.Commit MergeCommit { get; set; }

        [GitHubData.V4.GraphQLQueryGenerationIgnore]
        [DataMember(EmitDefaultValue = false, Name = "mergeRequirements")]
        public GitHubData.V4.MergeRequirements MergeRequirements { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "potentialMergeCommit")]
        public GitHubData.V4.Commit PotentialMergeCommit { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "assignees")]
        public GitHubData.V4.Connection<GitHubData.V4.User> Assignees { get; set; }

        [GitHubData.V4.GraphQLQueryGenerationIgnore]
        [DataMember(EmitDefaultValue = false, Name = "isCrossRepository")]
        public bool IsCrossRepository { get; set; }

        [GitHubData.V4.GraphQLQueryGenerationIgnore]
        [DataMember(EmitDefaultValue = false, Name = "merged")]
        public bool Merged { get; set; }

        [GitHubData.V4.GraphQLQueryGenerationIgnore]
        [DataMember(EmitDefaultValue = false, Name = "additions")]
        public int? Additions { get; set; }

        [GitHubData.V4.GraphQLQueryGenerationIgnore]
        [DataMember(EmitDefaultValue = false, Name = "deletions")]
        public int? Deletions { get; set; }

        [GitHubData.V4.GraphQLQueryGenerationIgnore]
        [DataMember(EmitDefaultValue = false, Name = "changedFiles")]
        public int? ChangedFiles { get; set; }

        [GitHubData.V4.GraphQLQueryGenerationIgnore]
        [DataMember(EmitDefaultValue = false, Name = "closed")]
        public bool? IsClosed { get; set; }

        [GitHubData.V4.GraphQLQueryGenerationIgnore]
        [DataMember(EmitDefaultValue = false, Name = "locked")]
        public bool? IsLocked { get; set; }
      }

      [DataContract]
      [GitHubData.V4.GraphQLObject]
      public class PullRequestCommit
      {
        [DataMember(EmitDefaultValue = false, Name = "id")]
        public string Id { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "commit")]
        public GitHubData.V4.Commit Commit { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "url")]
        public Uri Url { get; set; }
      }

      [DataContract]
      [GitHubData.V4.GraphQLObject]
      public class PullRequestMergeConflictStateCondition : GitHubData.V4.MergeCondition
      {
      }

      [DataContract]
      [GitHubData.V4.GraphQLObject]
      public class PullRequestRepoStateCondition : GitHubData.V4.MergeCondition
      {
      }

      [DataContract]
      [GitHubData.V4.GraphQLObject]
      public class PullRequestRulesCondition : GitHubData.V4.MergeCondition
      {
        [GitHubData.V4.GraphQLQueryGenerationIgnore]
        [DataMember(EmitDefaultValue = false, Name = "ruleRollups")]
        public GitHubData.V4.RuleRollup[] RuleRollup { get; set; }
      }

      [DataContract]
      [GitHubData.V4.GraphQLObject]
      public class PullRequestStateCondition : GitHubData.V4.MergeCondition
      {
      }

      [DataContract]
      [GitHubData.V4.GraphQLObject]
      public class PullRequestUserStateCondition : GitHubData.V4.MergeCondition
      {
      }

      [DataContract]
      [GitHubData.V4.GraphQLObject]
      public class Language
      {
        [GitHubData.V4.GraphQLQueryGenerationIgnore]
        [DataMember(EmitDefaultValue = false, Name = "id")]
        public string Id { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }

        [GitHubData.V4.GraphQLQueryGenerationIgnore]
        [DataMember(EmitDefaultValue = false, Name = "byteSize")]
        public int ByteSize { get; set; }
      }

      [DataContract]
      [GitHubData.V4.GraphQLObject]
      public class RateLimit : IGitHubRateLimit
      {
        [DataMember(Name = "cost")]
        public int Cost { get; set; }

        [DataMember(Name = "limit")]
        public int Limit { get; set; }

        [DataMember(Name = "nodeCount")]
        public int NodeCount { get; set; }

        [DataMember(Name = "remaining")]
        public int Remaining { get; set; }

        [DataMember(Name = "resetAt")]
        public DateTime ResetAt { get; set; }
      }

      [DataContract]
      [GitHubData.V4.GraphQLObject]
      public class Ref
      {
        [DataMember(Name = "id")]
        public string Id { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "prefix")]
        public string Prefix { get; set; }

        [DataMember(Name = "target")]
        public GitHubData.V4.IGitObject Target { get; set; }

        [GitHubData.V4.GraphQLQueryGenerationIgnore]
        [DataMember(Name = "repository")]
        public GitHubData.V4.Repository Repository { get; set; }
      }

      [DataContract]
      [GitHubData.V4.GraphQLObject]
      public class DefaultBranchRef
      {
        [DataMember(Name = "id")]
        public string Id { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }
      }

      [DataContract]
      [GitHubData.V4.GraphQLObject]
      public class Repository
      {
        [DataMember(Name = "id")]
        public string Id { get; set; }

        [DataMember(Name = "databaseId")]
        public int RepositoryId { get; set; }

        [DataMember(Name = "isPrivate")]
        public bool IsPrivate { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "nameWithOwner")]
        public string NameWithOwner { get; set; }

        [DataMember(Name = "defaultBranchRef")]
        public GitHubData.V4.Ref DefaultBranchRef { get; set; }

        [DataMember(Name = "primaryLanguage")]
        public GitHubData.V4.Language PrimaryLanguage { get; set; }

        [GitHubData.V4.GraphQLArgument(Name = "qualifiedName", Type = typeof (string), IsRequired = true)]
        [DataMember(Name = "ref")]
        public GitHubData.V4.Ref Ref { get; set; }

        [GitHubData.V4.GraphQLQueryGenerationIgnore]
        [DataMember(Name = "refs")]
        public GitHubData.V4.Connection<GitHubData.V4.Ref> Refs { get; set; }

        [DataMember(Name = "url")]
        public Uri Url { get; set; }

        [DataMember]
        public bool ViewerCanAdminister { get; set; }

        [DataMember]
        public string ViewerPermission { get; set; }

        [GitHubData.V4.GraphQLArgument(Name = "number", Type = typeof (int), IsRequired = true)]
        [DataMember(Name = "pullRequest")]
        public GitHubData.V4.PullRequest PullRequest { get; set; }

        [GitHubData.V4.GraphQLQueryGenerationIgnore]
        [DataMember(Name = "createdAt")]
        public DateTime CreatedAt { get; set; }

        [GitHubData.V4.GraphQLQueryGenerationIgnore]
        [DataMember(Name = "updatedAt")]
        public DateTime UpdatedAt { get; set; }

        [GitHubData.V4.GraphQLQueryGenerationIgnore]
        [DataMember(Name = "pushedAt")]
        public DateTime PushedAt { get; set; }

        [GitHubData.V4.GraphQLQueryGenerationIgnore]
        [DataMember(Name = "owner")]
        public GitHubData.V4.IRepositoryOwner RepositoryOwner { get; set; }

        [GitHubData.V4.GraphQLQueryGenerationIgnore]
        [DataMember(Name = "description")]
        public string Description { get; set; }

        [GitHubData.V4.GraphQLQueryGenerationIgnore]
        [DataMember(Name = "diskUsage")]
        public ulong DiskUsage { get; set; }

        [GitHubData.V4.GraphQLQueryGenerationIgnore]
        [DataMember(Name = "isLocked")]
        public bool IsLocked { get; set; }

        [GitHubData.V4.GraphQLQueryGenerationIgnore]
        [DataMember(Name = "isDisabled")]
        public bool IsDisabled { get; set; }

        [GitHubData.V4.GraphQLQueryGenerationIgnore]
        [DataMember(Name = "isArchived")]
        public bool IsArchived { get; set; }

        [GitHubData.V4.GraphQLQueryGenerationIgnore]
        [DataMember(Name = "isFork")]
        public bool IsFork { get; set; }

        [GitHubData.V4.GraphQLQueryGenerationIgnore]
        [DataMember(Name = "forkCount")]
        public int ForkCount { get; set; }

        [GitHubData.V4.GraphQLQueryGenerationIgnore]
        [DataMember(Name = "parent")]
        public GitHubData.V4.Repository Parent { get; set; }

        [GitHubData.V4.GraphQLQueryGenerationIgnore]
        [DataMember(Name = "licenseInfo")]
        public GitHubData.V4.License License { get; set; }

        [GitHubData.V4.GraphQLQueryGenerationIgnore]
        [DataMember(Name = "languages")]
        public GitHubData.V4.Connection<GitHubData.V4.Language> Languages { get; set; }

        [GitHubData.V4.GraphQLQueryGenerationIgnore]
        [DataMember(Name = "collaborators")]
        public GitHubData.V4.Connection<GitHubData.V4.User> Collaborators { get; set; }

        [GitHubData.V4.GraphQLQueryGenerationIgnore]
        [DataMember(Name = "assignableUsers")]
        public GitHubData.V4.Connection<GitHubData.V4.User> AssignableUsers { get; set; }

        [GitHubData.V4.GraphQLQueryGenerationIgnore]
        [DataMember(Name = "mentionableUsers")]
        public GitHubData.V4.Connection<GitHubData.V4.User> MentionableUsers { get; set; }

        [GitHubData.V4.GraphQLQueryGenerationIgnore]
        [DataMember(Name = "stargazers")]
        public GitHubData.V4.Connection<GitHubData.V4.User> Stargazers { get; set; }
      }

      [DataContract]
      [GitHubData.V4.GraphQLObject]
      public class License
      {
        [GitHubData.V4.GraphQLQueryGenerationIgnore]
        [DataMember(EmitDefaultValue = false, Name = "id")]
        public string Id { get; set; }

        [GitHubData.V4.GraphQLQueryGenerationIgnore]
        [DataMember(EmitDefaultValue = false, Name = "key")]
        public string Key { get; set; }

        [GitHubData.V4.GraphQLQueryGenerationIgnore]
        [DataMember(EmitDefaultValue = false, Name = "name")]
        public string Name { get; set; }

        [GitHubData.V4.GraphQLQueryGenerationIgnore]
        [DataMember(EmitDefaultValue = false, Name = "nickname")]
        public string Nickname { get; set; }

        [GitHubData.V4.GraphQLQueryGenerationIgnore]
        [DataMember(EmitDefaultValue = false, Name = "pseudoLicense")]
        public string PseudoLicense { get; set; }
      }

      [DataContract]
      [GitHubData.V4.GraphQLObject]
      public class Status
      {
        [DataMember(EmitDefaultValue = false, Name = "id")]
        public string Id { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "state")]
        public GitHubData.V4.StatusState State { get; set; }
      }

      [DataContract]
      [GitHubData.V4.GraphQLObject]
      public class StatusCheckRollup
      {
        [DataMember(EmitDefaultValue = false, Name = "id")]
        public string Id { get; set; }

        [DataMember(Name = "state")]
        public string State { get; set; }

        [DataMember(Name = "contexts")]
        public GitHubData.V4.Connection<GitHubData.V4.IStatusCheckRollupContext> Contexts { get; set; }
      }

      [DataContract]
      [GitHubData.V4.GraphQLObject]
      public class StatusContext : GitHubData.V4.IStatusCheckRollupContext
      {
        [DataMember(EmitDefaultValue = false, Name = "id")]
        public string Id { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "commit")]
        public GitHubData.V4.Commit Commit { get; set; }
      }

      [DataContract]
      [GitHubData.V4.GraphQLObject]
      public class Tag : GitHubData.V4.IGitObject
      {
        [DataMember(Name = "abbreviatedOid")]
        public string AbbreviatedOid { get; set; }

        [DataMember(Name = "commitUrl")]
        public Uri CommitUrl { get; set; }

        [DataMember(Name = "id")]
        public string Id { get; set; }

        [DataMember(Name = "message")]
        public string Message { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "oid")]
        public string Oid { get; set; }
      }

      [DataContract]
      [GitHubData.V4.GraphQLObject]
      public class Tree : GitHubData.V4.IGitObject
      {
        [DataMember(Name = "id")]
        public string Id { get; set; }

        [DataMember(Name = "entries")]
        public GitHubData.V4.TreeEntry[] Entries { get; set; }

        [GitHubData.V4.GraphQLQueryGenerationIgnore]
        [DataMember(EmitDefaultValue = false, Name = "oid")]
        public string Oid { get; set; }
      }

      [DataContract]
      [GitHubData.V4.GraphQLObject]
      public class TreeEntry
      {
        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "type")]
        public string Type { get; set; }

        [DataMember(Name = "object")]
        public GitHubData.V4.IGitObject Object { get; set; }
      }

      [DataContract]
      [GitHubData.V4.GraphQLObject]
      public class RuleRollup
      {
        [DataMember(Name = "ruleType")]
        public string RuleType { get; set; }

        [DataMember(Name = "displayName")]
        public string DisplayName { get; set; }

        [DataMember(Name = "message")]
        public string Message { get; set; }

        [DataMember(Name = "result")]
        public string Result { get; set; }
      }

      [DataContract]
      [GitHubData.V4.GraphQLObject]
      public class Workflow
      {
        [DataMember(Name = "createdAt")]
        public DateTime? CreatedAt { get; set; }

        [DataMember(Name = "id")]
        public string Id { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "state")]
        public GitHubData.V4.WorkflowState? State { get; set; }

        [DataMember(Name = "updatedAt")]
        public DateTime? UpdatedAt { get; set; }
      }

      [DataContract]
      [GitHubData.V4.GraphQLObject]
      public class WorkflowRun
      {
        [DataMember(Name = "createdAt")]
        public DateTime? CreatedAt { get; set; }

        [DataMember(Name = "id")]
        public string Id { get; set; }

        [DataMember(Name = "updatedAt")]
        public DateTime? UpdatedAt { get; set; }

        [DataMember(Name = "workflow")]
        public GitHubData.V4.Workflow Workflow { get; set; }
      }

      public enum StatusState
      {
        Error,
        Expected,
        Failure,
        Pending,
        Success,
      }

      public enum SubscriptionState
      {
        Ignored,
        Subscribed,
        Unsubscribed,
      }

      public enum IssueState
      {
        Closed,
        Open,
      }

      public enum PullRequestState
      {
        Open,
        Closed,
        Merged,
      }

      public enum MergeableState
      {
        Mergeable,
        Conflicting,
        Unknown,
      }

      public enum ReviewDecision
      {
        Approved,
        Changes_Requested,
        Review_Required,
      }

      public enum WorkflowState
      {
        Active,
        Deleted,
        Disabled_Fork,
        Disabled_Inactivity,
        Disabled_Manually,
      }
    }
  }
}
