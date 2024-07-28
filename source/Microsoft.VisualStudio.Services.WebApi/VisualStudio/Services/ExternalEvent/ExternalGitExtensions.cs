// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ExternalEvent.ExternalGitExtensions
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.ExternalEvent
{
  public static class ExternalGitExtensions
  {
    public static string NodeId(this ExternalGitPullRequest externalGitPullRequest) => externalGitPullRequest.GetAdditionalProperty<ExternalGitPullRequest, string>("nodeId") ?? externalGitPullRequest.Id;

    public static string NodeId(this ExternalGitRepo externalGitRepo) => (externalGitRepo != null ? externalGitRepo.GetAdditionalProperty<ExternalGitRepo, string>("nodeId") : (string) null) ?? externalGitRepo.Id;

    public static string NodeId<T>(this T artifact) where T : IAdditionalProperties => (object) artifact != null ? artifact.GetAdditionalProperty<T, string>("nodeId") : (string) null;

    public static T SetNodeId<T>(this T artifact, string nodeId) where T : IAdditionalProperties => (object) artifact != null ? artifact.SetAdditionalProperty<T>(nameof (nodeId), (object) nodeId) : artifact;

    public static string Name(this ExternalGitUser user) => user == null ? (string) null : user.GetAdditionalProperty<ExternalGitUser, string>("name");

    public static ExternalGitUser SetName(this ExternalGitUser user, string name) => user == null ? (ExternalGitUser) null : user.SetAdditionalProperty<ExternalGitUser>(nameof (name), (object) name);

    public static string Login(this ExternalGitUser user) => user == null ? (string) null : user.GetAdditionalProperty<ExternalGitUser, string>("login");

    public static ExternalGitUser SetLogin(this ExternalGitUser user, string login) => user == null ? (ExternalGitUser) null : user.SetAdditionalProperty<ExternalGitUser>(nameof (login), (object) login);

    public static bool IsBot(this ExternalGitUser user)
    {
      if (user == null)
        return true;
      return !string.IsNullOrEmpty(user.Type) && user.Type.Equals("Bot", StringComparison.OrdinalIgnoreCase);
    }

    public static string RepoNameWithOwner(this ExternalGitRepo repo) => repo.GetAdditionalProperty<ExternalGitRepo, string>("repoNameWithOwner") ?? repo.Name;

    public static ExternalGitRepo SetRepoNameWithOwner(
      this ExternalGitRepo externalGitRepo,
      string repoNameWithOwner)
    {
      return externalGitRepo.SetAdditionalProperty<ExternalGitRepo>(nameof (repoNameWithOwner), (object) repoNameWithOwner);
    }

    public static Guid GetRepoInternalId(this ExternalGitRepo artifact) => artifact.GetAdditionalProperty<ExternalGitRepo, Guid>("repoInternalId");

    public static ExternalGitRepo SetRepoInternalId(
      this ExternalGitRepo artifact,
      Guid repoInternalId)
    {
      return artifact.SetAdditionalProperty<ExternalGitRepo>(nameof (repoInternalId), (object) repoInternalId);
    }

    public static T GetMetadata<T>(this ExternalGitRepo externalGitRepo)
    {
      object obj;
      return externalGitRepo.AdditionalProperties != null && externalGitRepo.AdditionalProperties.TryGetValue("metadata", out obj) && obj is string str ? JsonConvert.DeserializeObject<T>(str) : default (T);
    }

    public static ExternalGitRepo SetMetadata(this ExternalGitRepo externalGitRepo, object data)
    {
      if (data != null)
      {
        if (externalGitRepo.AdditionalProperties == null)
          externalGitRepo.AdditionalProperties = (IDictionary<string, object>) new Dictionary<string, object>();
        externalGitRepo.AdditionalProperties["metadata"] = data is string str ? (object) str : (object) JsonConvert.SerializeObject(data);
      }
      else if (externalGitRepo.AdditionalProperties != null)
        externalGitRepo.AdditionalProperties.Remove("metadata");
      return externalGitRepo;
    }

    public static string GetSerializedMetadata(this ExternalGitRepo externalGitRepo)
    {
      object obj;
      return externalGitRepo.AdditionalProperties == null || !externalGitRepo.AdditionalProperties.TryGetValue("metadata", out obj) || !(obj is string str) ? (string) null : str;
    }

    public static string GetCommentDatabaseId(this ExternalGitComment externalGitComment) => externalGitComment.GetAdditionalProperty<ExternalGitComment, string>("commentDatabaseId") ?? externalGitComment.Id;

    public static ExternalGitComment SetCommentDatabaseId(
      this ExternalGitComment externalGitComment,
      string commentDatabaseId)
    {
      return externalGitComment.SetAdditionalProperty<ExternalGitComment>(nameof (commentDatabaseId), (object) commentDatabaseId);
    }

    public static bool IsDeleteAction(this IExternalAction action) => !string.IsNullOrEmpty(action.Action) && "deleted".Equals(action.Action, StringComparison.OrdinalIgnoreCase);

    private static S GetAdditionalProperty<T, S>(this T artifact, string key) where T : IAdditionalProperties
    {
      object obj;
      return artifact.AdditionalProperties != null && artifact.AdditionalProperties.TryGetValue(key, out obj) && obj is S s ? s : default (S);
    }

    private static T SetAdditionalProperty<T>(this T artifact, string key, object value) where T : IAdditionalProperties
    {
      if (artifact.AdditionalProperties == null)
      {
        ref T local = ref artifact;
        if ((object) default (T) == null)
        {
          T obj = local;
          local = ref obj;
        }
        Dictionary<string, object> dictionary = new Dictionary<string, object>();
        local.AdditionalProperties = (IDictionary<string, object>) dictionary;
      }
      artifact.AdditionalProperties[key] = value;
      return artifact;
    }
  }
}
