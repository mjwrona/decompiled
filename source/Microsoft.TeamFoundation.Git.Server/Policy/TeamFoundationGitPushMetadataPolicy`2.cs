// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.Policy.TeamFoundationGitPushMetadataPolicy`2
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Git.Server.Protocol;
using Microsoft.TeamFoundation.Policy.Server;
using Microsoft.TeamFoundation.SourceControl.WebApi;

namespace Microsoft.TeamFoundation.Git.Server.Policy
{
  internal abstract class TeamFoundationGitPushMetadataPolicy<TSettings, TContext> : 
    TeamFoundationGitPushPolicy<TSettings, TContext>,
    ITeamFoundationGitPushMetadataPolicy,
    ITeamFoundationGitPushPolicy,
    ITeamFoundationGitRepositoryPolicy,
    ITeamFoundationPolicy
    where TSettings : TeamFoundationGitRepositoryPolicySettings
    where TContext : TeamFoundationPolicyEvaluationRecordContext
  {
    public abstract void VerifyGitPackObject(
      long length,
      long uncompressedLength,
      Sha1Id objectId,
      GitObjectType type);

    public virtual void AddDeserializerHandlers(
      IVssRequestContext requestContext,
      GitReceivePackDeserializer deserializer)
    {
    }
  }
}
