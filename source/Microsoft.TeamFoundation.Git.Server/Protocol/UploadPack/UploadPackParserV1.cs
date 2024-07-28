// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.Protocol.UploadPack.UploadPackParserV1
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.BusinessIntelligence;
using Microsoft.TeamFoundation.Git.Server.Protocol.UploadPack.Filter;
using Microsoft.TeamFoundation.Git.Server.Storage;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Microsoft.TeamFoundation.Git.Server.Protocol.UploadPack
{
  public class UploadPackParserV1 : UploadPackParser
  {
    private GitPackFeature m_clientFeatures;
    private bool m_bodyStarted;
    private readonly IVssRequestContext m_requestContext;
    private readonly ITfsGitRepository m_repository;
    private readonly Stream m_inputStream;
    private readonly Stream m_outputStream;
    private readonly bool m_isStateless;
    private readonly ClientTraceData m_ctData;
    private const string c_layer = "UploadPackParser";

    public UploadPackParserV1(
      IVssRequestContext requestContext,
      ITfsGitRepository repository,
      Stream inputStream,
      Stream outputStream,
      bool isOptimized,
      bool isStateless,
      ClientTraceData ctData,
      HashSet<Sha1Id> wants = null)
      : this(requestContext, repository, GitServerUtils.GetOdb(repository), inputStream, outputStream, isOptimized, isStateless, ctData, wants)
    {
    }

    internal UploadPackParserV1(
      IVssRequestContext requestContext,
      ITfsGitRepository repository,
      Odb repoStorage,
      Stream inputStream,
      Stream outputStream,
      bool isOptimized,
      bool isStateless,
      ClientTraceData ctData,
      HashSet<Sha1Id> wants)
      : base(requestContext, repoStorage, repository, outputStream, ctData, isOptimized, isStateless, wants)
    {
      this.m_requestContext = requestContext;
      this.m_repository = repository;
      this.m_inputStream = inputStream;
      this.m_outputStream = outputStream;
      this.m_isStateless = isStateless;
      this.m_ctData = ctData;
    }

    public override bool BodyStarted => this.m_bodyStarted;

    public override void UploadPack()
    {
      Shallows shallows = new Shallows(this.m_requestContext.RequestTracer, this.m_repository.Objects);
      GitObjectFilter filter = new GitObjectFilter();
      HashSet<Sha1Id> sha1IdSet = new HashSet<Sha1Id>();
      bool flag1 = this.m_requestContext.IsFeatureEnabled("Git.EnablePartialClone");
      using (this.m_requestContext.TraceBlock(1013539, 1013540, GitServerUtils.TraceArea, "UploadPackParser", "ReadWants"))
      {
        bool flag2 = true;
        bool firstMessage = true;
        int state = 0;
        while (true)
        {
          byte[] numArray;
          try
          {
            if ((numArray = ProtocolHelper.ReadLineBytes(this.m_inputStream)) == null)
              goto label_20;
          }
          catch (GitProtocolException ex) when (flag2 && !this.m_isStateless)
          {
            this.m_requestContext.Trace(1013719, TraceLevel.Info, GitServerUtils.TraceArea, "UploadPackParser", "Couldn't read first pkt-line: {0}", (object) ex);
            return;
          }
          flag2 = false;
          if (ProtocolHelper.IsPrefixMatch(numArray, ProtocolHelper.MagicBytes.WantPrefix))
          {
            this.ReadWant(numArray, sha1IdSet, state, firstMessage);
            state = 1;
            firstMessage = false;
          }
          else if (ProtocolHelper.IsPrefixMatch(numArray, ProtocolHelper.MagicBytes.ShallowPrefix))
          {
            if (state == 1 || state == 2)
            {
              state = 2;
              Sha1Id shallowId = new Sha1Id(GitUtils.ObjectIdFromUTF8Bytes(numArray, ProtocolHelper.MagicBytes.ShallowPrefix.Length));
              shallows.AddClientShallow(shallowId);
              this.m_requestContext.Trace(1013167, TraceLevel.Verbose, GitServerUtils.TraceArea, "UploadPackParser", "Client shallow: {0}", (object) shallowId);
            }
            else
              break;
          }
          else if (ProtocolHelper.IsPrefixMatch(numArray, ProtocolHelper.MagicBytes.DeepenPrefix))
          {
            if (state == 1 || state == 2)
            {
              state = 3;
              byte[] deepenPrefix = ProtocolHelper.MagicBytes.DeepenPrefix;
              string depthStr = GitEncodingUtil.SafeUtf8NoBom.GetString(numArray, deepenPrefix.Length, numArray.Length - deepenPrefix.Length);
              shallows.SetDeepenParams(depthStr);
            }
            else
              goto label_15;
          }
          else if (flag1 && ProtocolHelper.IsPrefixMatch(numArray, ProtocolHelper.MagicBytes.FilterPrefix))
          {
            byte[] filterPrefix = ProtocolHelper.MagicBytes.FilterPrefix;
            filter = FilterSpecParser.Parse(GitEncodingUtil.SafeUtf8NoBom.GetString(numArray, filterPrefix.Length, numArray.Length - filterPrefix.Length));
          }
          else
            goto label_19;
        }
        throw new GitProtocolException("All shallows must follow wants.");
label_15:
        throw new GitProtocolException("The depth requests must follow wants and shallows.");
label_19:
        throw new GitProtocolException("wants expected.");
label_20:
        if (this.m_requestContext.IsFeatureEnabled("Git.DisallowShallowAndPartialClone") && shallows.EnforceMaxDepth && !filter.FilterIsNoop())
          filter = new GitObjectFilter();
        if (sha1IdSet.Count == 0)
          return;
        if (this.m_wants != null)
          this.m_wants.AddRange<Sha1Id, HashSet<Sha1Id>>((IEnumerable<Sha1Id>) sha1IdSet);
        this.m_bodyStarted = true;
        if (shallows.EnforceMaxDepth)
        {
          shallows.WriteShallowInfo(sha1IdSet, this.m_outputStream);
          ProtocolHelper.WriteLine(this.m_outputStream, (string) null);
        }
      }
      CommitIdSet haves = new CommitIdSet();
      bool flag3 = false;
      List<Sha1Id> wants = new List<Sha1Id>();
      if (!shallows.EnforceMaxDepth)
        wants.AddRange(sha1IdSet.Select<Sha1Id, TfsGitCommit>((Func<Sha1Id, TfsGitCommit>) (x => this.m_repository.LookupObject(x).TryResolveToCommit())).Where<TfsGitCommit>((Func<TfsGitCommit, bool>) (x => x != null)).Select<TfsGitCommit, Sha1Id>((Func<TfsGitCommit, Sha1Id>) (x => x.ObjectId)));
      using (this.m_requestContext.TraceBlock(1013541, 1013542, GitServerUtils.TraceArea, "UploadPackParser", "Negotiation"))
      {
        while (true)
        {
          byte[] numArray;
          do
          {
            try
            {
              numArray = ProtocolHelper.ReadLineBytes(this.m_inputStream);
            }
            catch (GitProtocolException ex)
            {
              this.m_ctData?.Add("Action", (object) "FetchNegotiation");
              return;
            }
            if (numArray == null)
            {
              if (!flag3 && (this.m_clientFeatures & GitPackFeature.MultiAckDetailed) != GitPackFeature.None && haves.Count > 0 && (flag3 = this.AllWantsConnectedToHaves((IReadOnlyCollection<Sha1Id>) haves, wants)))
                ProtocolHelper.WriteLine(this.m_outputStream, string.Format("ACK {0} ready", (object) haves.LastAdded));
              ProtocolHelper.WriteLine(this.m_outputStream, "NAK");
              if ((((this.m_clientFeatures & GitPackFeature.NoDone) == GitPackFeature.None ? 0 : ((this.m_clientFeatures & GitPackFeature.MultiAckDetailed) != 0 ? 1 : 0)) & (flag3 ? 1 : 0)) != 0)
              {
                ProtocolHelper.WriteLine(this.m_outputStream, string.Format("ACK {0}", (object) haves.LastAdded));
                goto label_61;
              }
            }
            else
              goto label_47;
          }
          while (!this.m_isStateless);
          break;
label_47:
          if (numArray.Length != ProtocolHelper.MagicBytes.Done.Length || !ProtocolHelper.IsPrefixMatch(numArray, ProtocolHelper.MagicBytes.Done))
          {
            if (ProtocolHelper.IsPrefixMatch(numArray, ProtocolHelper.MagicBytes.HavePrefix))
            {
              Sha1Id objectId = new Sha1Id(GitUtils.ObjectIdFromUTF8Bytes(numArray, ProtocolHelper.MagicBytes.HavePrefix.Length));
              TfsGitObject haveObject = this.m_repository.TryLookupObject(objectId);
              if (haveObject != null)
              {
                this.m_requestContext.Trace(1013068, TraceLevel.Verbose, GitServerUtils.TraceArea, "UploadPackParser", "Common Object Found: {0}", (object) objectId);
                this.ProcessHave(haves, haveObject);
                if ((this.m_clientFeatures & GitPackFeature.MultiAckDetailed) != GitPackFeature.None)
                  ProtocolHelper.WriteLine(this.m_outputStream, string.Format("ACK {0} common", (object) objectId));
                else
                  ProtocolHelper.WriteLine(this.m_outputStream, string.Format("ACK {0} continue", (object) objectId));
              }
              else
                this.m_requestContext.Trace(1013069, TraceLevel.Verbose, GitServerUtils.TraceArea, "UploadPackParser", "Client has unknown object: {0}", (object) objectId);
            }
            else
              goto label_57;
          }
          else
            goto label_48;
        }
        this.m_ctData?.Add("Action", (object) "FetchNegotiation");
        return;
label_48:
        if (haves.Count == 0)
        {
          ProtocolHelper.WriteLine(this.m_outputStream, "NAK");
          goto label_61;
        }
        else
        {
          ProtocolHelper.WriteLine(this.m_outputStream, string.Format("ACK {0}", (object) haves.LastAdded));
          goto label_61;
        }
label_57:
        throw new GitProtocolException("Unknown Input");
      }
label_61:
      this.TransmitPackfile(haves, sha1IdSet, shallows, filter, this.m_clientFeatures);
    }

    private void ReadWant(
      byte[] incomingMessage,
      HashSet<Sha1Id> wants,
      int state,
      bool firstMessage)
    {
      if (state > 1)
        throw new GitProtocolException("Git protocol violation: all wants must come first.");
      byte[] wantPrefix = ProtocolHelper.MagicBytes.WantPrefix;
      Sha1Id sha1Id = new Sha1Id(GitUtils.ObjectIdFromUTF8Bytes(incomingMessage, wantPrefix.Length));
      wants.Add(sha1Id);
      this.m_requestContext.Trace(1013066, TraceLevel.Verbose, GitServerUtils.TraceArea, "UploadPackParser", "Client want: {0}", (object) sha1Id);
      if (!firstMessage)
        return;
      int index = wantPrefix.Length + 40 + 1;
      if (incomingMessage.Length <= index)
        return;
      string str = GitEncodingUtil.SafeUtf8NoBom.GetString(incomingMessage, index, incomingMessage.Length - index);
      char[] separator = new char[1]{ ' ' };
      foreach (string featureName in str.Split(separator, StringSplitOptions.RemoveEmptyEntries))
      {
        this.m_requestContext.Trace(1013067, TraceLevel.Verbose, GitServerUtils.TraceArea, "UploadPackParser", "Client Feature: {0}", (object) featureName);
        this.m_clientFeatures |= GitPackFeatureHelpers.FeatureFromString(featureName);
      }
      if (GitServerUtils.IsWindowsLibGit2Client(this.m_requestContext.UserAgent))
        this.m_clientFeatures &= ~GitPackFeature.ThinPack;
      if ((this.m_clientFeatures & GitPackFeature.MultiAck) == GitPackFeature.None)
        throw new GitProtocolException("Clients must support multi-ack.");
    }

    public override void WriteError(string errorMessage)
    {
      errorMessage = SecretUtility.ScrubSecrets(errorMessage);
      if ((GitPackFeature.SideBand & this.m_clientFeatures) != GitPackFeature.None)
        ProtocolHelper.WriteSideband(this.m_outputStream, SidebandChannel.Error, errorMessage);
      else
        ProtocolHelper.WriteLine(this.m_outputStream, errorMessage);
    }
  }
}
