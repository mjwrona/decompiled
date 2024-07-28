// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.Protocol.GitReceivePackDeserializer
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.BusinessIntelligence;
using Microsoft.TeamFoundation.Git.Server.PackIndex;
using Microsoft.TeamFoundation.Git.Server.Policy;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Microsoft.TeamFoundation.Git.Server.Protocol
{
  internal sealed class GitReceivePackDeserializer
  {
    private readonly ITraceRequest m_tracer;
    private readonly long m_initialPosition;
    private readonly PushPolicyManager m_pushPolicyManager;
    private readonly IObserver<GitPackDeserializerProgress> m_progressObserver;
    private readonly IObjectOrderer m_objectOrderer;
    private readonly List<Sha1Id> m_currReferencedObjects;
    private List<Sha1Id> m_currParentCommits;
    private const string c_layer = "GitReceivePackDeserializer";

    public GitReceivePackDeserializer(
      IVssRequestContext rc,
      Stream stream,
      IBufferStreamFactory bufferStreamFactory,
      IGitObjectSet objectSet,
      TreeFsckOptions fsckOptions,
      CommitParserOptions commitParserOptions,
      bool packHasShallows,
      int packSoftCapSize,
      IObjectOrderer objectOrderer,
      ClientTraceData ctData,
      PushPolicyManager pushPolicyManager,
      IObserver<GitPackDeserializerProgress> progressObserver = null)
    {
      this.m_tracer = rc.RequestTracer;
      this.m_initialPosition = stream.Position;
      this.Deserializer = new GitPackDeserializer(rc.RequestTracer, stream, objectSet, bufferStreamFactory, (Action) (() => rc.RequestContextInternal().CheckCanceled()), ctData, false, false);
      this.ObjectParser = new GitPackDeserializerObjectParserTrait(fsckOptions, commitParserOptions, packHasShallows);
      this.Deserializer.AddTrait((IGitPackDeserializerTrait) this.ObjectParser);
      this.Splitter = new GitPackDeserializerSplitterTrait(packSoftCapSize);
      this.Deserializer.AddTrait((IGitPackDeserializerTrait) this.Splitter);
      this.AddReceivePackSpecificBehavior();
      this.m_progressObserver = progressObserver;
      this.m_pushPolicyManager = pushPolicyManager;
      this.m_pushPolicyManager?.AddDeserializerHandlers(this);
      this.m_objectOrderer = objectOrderer;
      this.m_currReferencedObjects = new List<Sha1Id>();
      this.IncludedCommits = new List<TfsIncludedGitCommit>();
      this.ObjectSizeMap = new List<ObjectIdAndSize>();
      this.m_currParentCommits = new List<Sha1Id>(1);
    }

    private void AddReceivePackSpecificBehavior()
    {
      this.ObjectParser.ObjectInfo += new GitPackDeserializerObjectParserTrait.ObjectInfoHandler(this.ObjectParser_ObjectInfo);
      this.ObjectParser.Deserializer.DeserializationComplete += new GitPackDeserializer.DeserializationCompleteHandler(this.Deserializer_DeserializationComplete);
      this.ObjectParser.CommitHandler.Tree += (EventCommitParserHandler.TreeEvent) (tree => this.m_currReferencedObjects.Add(tree));
      this.ObjectParser.CommitHandler.Parent += (EventCommitParserHandler.ParentEvent) (parent =>
      {
        this.m_currReferencedObjects.Add(parent);
        this.m_currParentCommits.Add(parent);
      });
      this.ObjectParser.TreeEntryHandler += (TreeParserEntryHandler) (entry =>
      {
        if (entry.PackType == GitPackObjectType.Commit)
          return;
        this.m_currReferencedObjects.Add(entry.ObjectId);
      });
      this.ObjectParser.TagHandler.ReferencedObject += (EventTagParserHandler.ReferencedObjectEvent) (o => this.m_currReferencedObjects.Add(o.ObjectId));
    }

    public GitPackDeserializer Deserializer { get; }

    public GitPackDeserializerObjectParserTrait ObjectParser { get; }

    public GitPackDeserializerSplitterTrait Splitter { get; }

    public List<TfsIncludedGitCommit> IncludedCommits { get; }

    public List<ObjectIdAndSize> ObjectSizeMap { get; }

    private void ObjectParser_ObjectInfo(
      GitPackDeserializerProgress progress,
      Sha1Id objectId,
      GitPackObjectType objectType,
      long objectLength,
      long offsetInPack,
      long lengthInPack,
      bool isFirstInPack)
    {
      ObjectIdAndType objectIdAndType = new ObjectIdAndType(objectId, objectType);
      IEnumerable<Sha1Id> parentCommitIds;
      if (this.m_currParentCommits.Count == 0)
      {
        parentCommitIds = Enumerable.Empty<Sha1Id>();
      }
      else
      {
        parentCommitIds = (IEnumerable<Sha1Id>) this.m_currParentCommits;
        this.m_currParentCommits = new List<Sha1Id>(1);
      }
      if (isFirstInPack)
      {
        if (this.ObjectParser.Deserializer.BaseObjects.TryLookupObjectType(objectId) == GitObjectType.Bad)
        {
          PushPolicyManager pushPolicyManager = this.m_pushPolicyManager;
          if ((pushPolicyManager != null ? (pushPolicyManager.CanApplyMetadataPolicies ? 1 : 0) : 0) != 0)
            this.m_pushPolicyManager.VerifyGitPackObject(lengthInPack, objectLength, objectId, objectType.GetObjectType());
        }
        if (GitPackObjectType.Commit == objectType)
          this.IncludedCommits.Add(new TfsIncludedGitCommit(objectId, parentCommitIds));
        this.ObjectSizeMap.Add(new ObjectIdAndSize(objectId, objectLength));
      }
      this.m_objectOrderer?.EnqueueObject(objectId, (IEnumerable<Sha1Id>) this.m_currReferencedObjects);
      this.m_currReferencedObjects.Clear();
      this.m_progressObserver?.OnNext(progress);
    }

    private void Deserializer_DeserializationComplete(
      Sha1Id packHash,
      Stream packStream,
      long packLength)
    {
      this.m_tracer.Trace(1013050, TraceLevel.Verbose, GitServerUtils.TraceArea, nameof (GitReceivePackDeserializer), "Deserialization Complete");
      packStream.Seek(this.m_initialPosition, SeekOrigin.Begin);
    }
  }
}
