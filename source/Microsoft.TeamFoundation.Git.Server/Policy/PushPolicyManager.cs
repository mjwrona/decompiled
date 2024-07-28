// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.Policy.PushPolicyManager
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.BusinessIntelligence;
using Microsoft.TeamFoundation.Git.Server.Protocol;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Microsoft.TeamFoundation.Git.Server.Policy
{
  internal class PushPolicyManager
  {
    private readonly IList<ITeamFoundationGitPushContentPolicy> m_contentPushPolicies = (IList<ITeamFoundationGitPushContentPolicy>) new List<ITeamFoundationGitPushContentPolicy>();
    private readonly IList<ITeamFoundationGitPushMetadataPolicy> m_metadataPushPolicies = (IList<ITeamFoundationGitPushMetadataPolicy>) new List<ITeamFoundationGitPushMetadataPolicy>();
    private readonly IVssRequestContext m_requestContext;
    private const string c_layer = "PushPolicyManager";

    public PushPolicyManager(
      IVssRequestContext requestContext,
      IReadOnlyList<ITeamFoundationGitPushPolicy> pushPolicies)
    {
      this.m_requestContext = requestContext;
      if (pushPolicies == null)
        return;
      StringBuilder stringBuilder = new StringBuilder();
      foreach (ITeamFoundationGitPushPolicy pushPolicy in (IEnumerable<ITeamFoundationGitPushPolicy>) pushPolicies)
      {
        stringBuilder.AppendFormat("{0}, ", (object) pushPolicy.Id);
        if (pushPolicy is ITeamFoundationGitPushContentPolicy pushContentPolicy)
          this.m_contentPushPolicies.Add(pushContentPolicy);
        if (pushPolicy is ITeamFoundationGitPushMetadataPolicy pushMetadataPolicy)
          this.m_metadataPushPolicies.Add(pushMetadataPolicy);
      }
      this.m_requestContext.Trace(1013850, TraceLevel.Info, GitServerUtils.TraceArea, nameof (PushPolicyManager), string.Format("Created Push Policy Manager with {0} policies with the following ids: {1}", (object) pushPolicies.Count, (object) stringBuilder.ToString()));
    }

    public bool CanApplyContentPolicies => this.m_contentPushPolicies.Any<ITeamFoundationGitPushContentPolicy>((Func<ITeamFoundationGitPushContentPolicy, bool>) (policy => policy.FoundObjectsToApplyPolicy));

    public bool CanApplyMetadataPolicies => this.m_metadataPushPolicies.Count > 0;

    public void VerifyGitPackObject(
      long length,
      long uncompressedLength,
      Sha1Id objectId,
      GitObjectType type)
    {
      int count = this.m_metadataPushPolicies.Count;
      for (int index = 0; index < count; ++index)
        this.m_metadataPushPolicies[index].VerifyGitPackObject(length, uncompressedLength, objectId, type);
    }

    public void VerifyGitPackObjectContent(
      ReceivePackTempRepo tempRepo,
      IObserver<GitPushPolicyEvaluationProgress> progressObserver,
      int blobCount,
      Action checkIfCanceled = null)
    {
      Stopwatch timer = new Stopwatch();
      foreach (ITeamFoundationGitPushContentPolicy contentPushPolicy in (IEnumerable<ITeamFoundationGitPushContentPolicy>) this.m_contentPushPolicies)
      {
        ITeamFoundationGitPushContentPolicy policy = contentPushPolicy;
        timer.Restart();
        ClientTraceData eventData = new ClientTraceData();
        eventData.PublishContentPolicyCtData(this.m_requestContext, policy.RepositoryId, policy.Id, (Action) (() => policy.VerifyGitPackObjectContent(this.m_requestContext, tempRepo, eventData, blobCount, progressObserver, checkIfCanceled)), timer);
      }
      timer.Stop();
    }

    public void AddDeserializerHandlers(GitReceivePackDeserializer deserializer)
    {
      foreach (ITeamFoundationGitPushContentPolicy contentPushPolicy in (IEnumerable<ITeamFoundationGitPushContentPolicy>) this.m_contentPushPolicies)
        contentPushPolicy.AddDeserializerHandlers(this.m_requestContext, deserializer);
      foreach (ITeamFoundationGitPushMetadataPolicy metadataPushPolicy in (IEnumerable<ITeamFoundationGitPushMetadataPolicy>) this.m_metadataPushPolicies)
        metadataPushPolicy.AddDeserializerHandlers(this.m_requestContext, deserializer);
    }
  }
}
