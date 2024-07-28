// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.Policy.TeamFoundationGitPushContentPolicy`2
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.BusinessIntelligence;
using Microsoft.TeamFoundation.Git.Server.Protocol;
using Microsoft.TeamFoundation.Policy.Server;
using System;

namespace Microsoft.TeamFoundation.Git.Server.Policy
{
  internal abstract class TeamFoundationGitPushContentPolicy<TSettings, TContext> : 
    TeamFoundationGitPushPolicy<TSettings, TContext>,
    ITeamFoundationGitPushContentPolicy,
    ITeamFoundationGitPushPolicy,
    ITeamFoundationGitRepositoryPolicy,
    ITeamFoundationPolicy
    where TSettings : TeamFoundationGitRepositoryPolicySettings
    where TContext : TeamFoundationPolicyEvaluationRecordContext
  {
    public abstract bool FoundObjectsToApplyPolicy { get; }

    public abstract void VerifyGitPackObjectContent(
      IVssRequestContext requestContext,
      ReceivePackTempRepo tempRepo,
      ClientTraceData ctData,
      int blobCount,
      IObserver<GitPushPolicyEvaluationProgress> progressObserver = null,
      Action checkIfCanceled = null);

    public abstract void AddDeserializerHandlers(
      IVssRequestContext requestContext,
      GitReceivePackDeserializer deserializer);
  }
}
