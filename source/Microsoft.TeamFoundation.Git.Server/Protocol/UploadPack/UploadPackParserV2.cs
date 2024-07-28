// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.Protocol.UploadPack.UploadPackParserV2
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.BusinessIntelligence;
using Microsoft.TeamFoundation.Git.Server.Protocol.UploadPack.Filter;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace Microsoft.TeamFoundation.Git.Server.Protocol.UploadPack
{
  public class UploadPackParserV2 : UploadPackParser
  {
    private readonly IVssRequestContext m_rc;
    private readonly ITfsGitRepository m_repo;
    private readonly AnnotatedTagPeeler m_tagPeeler;
    private readonly Stream m_input;
    private readonly Stream m_output;
    private readonly bool m_isOptimized;
    private readonly ClientTraceData m_ctData;
    private bool m_bodyStarted;
    private const string c_layer = "UploadPackParserV2";

    public UploadPackParserV2(
      IVssRequestContext rc,
      ITfsGitRepository repo,
      Stream inputStream,
      Stream output,
      bool isOptimized,
      ClientTraceData ctData,
      HashSet<Sha1Id> wants = null)
      : base(rc, GitServerUtils.GetOdb(repo), repo, output, ctData, isOptimized, true, wants)
    {
      this.m_rc = rc;
      this.m_repo = repo;
      this.m_input = inputStream;
      this.m_output = output;
      this.m_isOptimized = isOptimized;
      this.m_ctData = ctData;
      this.m_tagPeeler = new AnnotatedTagPeeler(repo);
    }

    public override bool BodyStarted => this.m_bodyStarted;

    public override void UploadPack()
    {
      byte[] numArray;
      while ((numArray = ProtocolHelper.ReadLineBytes(this.m_input)) != null)
      {
        if (ProtocolHelper.IsPrefixMatch(numArray, ProtocolHelper.MagicBytes.Command))
        {
          switch (UploadPackParserV2.ParseCommand(numArray))
          {
            case UploadPackParserV2.Protocolv2Command.LsRefs:
              (bool peel, bool symrefs, List<byte[]> refPrefixes) lsRefsArguments = this.ParseLsRefsArguments(numArray);
              this.AdvertiseRefs(lsRefsArguments.refPrefixes, lsRefsArguments.peel, lsRefsArguments.symrefs);
              return;
            case UploadPackParserV2.Protocolv2Command.Fetch:
              (CommitIdSet haves, HashSet<Sha1Id> wants, Shallows shallows, GitObjectFilter filter, GitPackFeature features, bool packTransmitted) fetchArguments = this.ParseFetchArguments(numArray);
              CommitIdSet haves = fetchArguments.haves;
              HashSet<Sha1Id> wants = fetchArguments.wants;
              Shallows shallows = fetchArguments.shallows;
              GitObjectFilter filter = fetchArguments.filter;
              GitPackFeature features = fetchArguments.features;
              if (fetchArguments.packTransmitted)
                return;
              this.ProcessFetch(haves, wants, shallows, filter, features);
              return;
            default:
              throw new GitProtocolException("Unsupported command: " + GitEncodingUtil.SafeUtf8NoBom.GetString(numArray));
          }
        }
      }
    }

    private void ProcessFetch(
      CommitIdSet haves,
      HashSet<Sha1Id> wants,
      Shallows shallows,
      GitObjectFilter filter,
      GitPackFeature clientOptions)
    {
      if (wants.Count == 0)
        return;
      this.m_bodyStarted = true;
      if (shallows.EnforceMaxDepth)
      {
        ProtocolHelper.WriteLine(this.m_output, "shallow-info");
        ProtocolHelper.WriteDelimLine(this.m_output);
        shallows.WriteShallowInfo(wants, this.m_output);
      }
      List<Sha1Id> wants1 = new List<Sha1Id>();
      if (!shallows.EnforceMaxDepth)
        wants1.AddRange(wants.Select<Sha1Id, TfsGitCommit>((Func<Sha1Id, TfsGitCommit>) (x => this.m_repo.LookupObject(x).TryResolveToCommit())).Where<TfsGitCommit>((Func<TfsGitCommit, bool>) (x => x != null)).Select<TfsGitCommit, Sha1Id>((Func<TfsGitCommit, Sha1Id>) (x => x.ObjectId)));
      ProtocolHelper.WriteLine(this.m_output, "acknowledgments");
      if (haves.Count > 0 && this.AllWantsConnectedToHaves((IReadOnlyCollection<Sha1Id>) haves, wants1))
      {
        ProtocolHelper.WriteLine(this.m_output, "ready");
        ProtocolHelper.WriteDelimLine(this.m_output);
        this.TransmitPackfile(haves, wants, shallows, filter, clientOptions);
      }
      else
      {
        this.m_ctData?.Add("Action", (object) "FetchNegotiation");
        if (haves.Count == 0)
        {
          ProtocolHelper.WriteLine(this.m_output, "NAK");
        }
        else
        {
          foreach (Sha1Id have in haves)
            ProtocolHelper.WriteLine(this.m_output, FormattableString.Invariant(FormattableStringFactory.Create("ACK {0}", (object) have)));
        }
      }
    }

    private static UploadPackParserV2.Protocolv2Command ParseCommand(byte[] incomingMessage)
    {
      if (GitUtils.CompareByteArrays(incomingMessage, ProtocolHelper.MagicBytes.Command.Length, ProtocolHelper.MagicBytes.LsRefs, 0, ProtocolHelper.MagicBytes.LsRefs.Length))
        return UploadPackParserV2.Protocolv2Command.LsRefs;
      return GitUtils.CompareByteArrays(incomingMessage, ProtocolHelper.MagicBytes.Command.Length, ProtocolHelper.MagicBytes.Fetch, 0, ProtocolHelper.MagicBytes.Fetch.Length) ? UploadPackParserV2.Protocolv2Command.Fetch : UploadPackParserV2.Protocolv2Command.Unknown;
    }

    private static byte[] ParseRefPrefix(byte[] incomingMessage)
    {
      byte[] refPrefix = (byte[]) null;
      if (incomingMessage != null && ProtocolHelper.IsPrefixMatch(incomingMessage, ProtocolHelper.MagicBytes.RefPrefix))
        refPrefix = ((IEnumerable<byte>) incomingMessage).Skip<byte>(ProtocolHelper.MagicBytes.RefPrefix.Length).ToArray<byte>();
      return refPrefix;
    }

    private void AdvertiseRefs(List<byte[]> referenceList, bool peel, bool symrefs)
    {
      List<string> searchRefs = new List<string>();
      foreach (byte[] reference in referenceList)
      {
        string str = Encoding.UTF8.GetString(reference);
        searchRefs.Add(str.Trim());
      }
      List<TfsGitRef> source;
      if (this.m_isOptimized)
      {
        source = this.m_repo.Refs.Limited();
        if (searchRefs.Count > 0)
          source = source.Where<TfsGitRef>((Func<TfsGitRef, bool>) (r => searchRefs.Any<string>((Func<string, bool>) (sr => r.Name.StartsWith(sr, StringComparison.Ordinal))))).ToList<TfsGitRef>();
      }
      else
        source = searchRefs.Count <= 0 ? this.m_repo.Refs.All() : this.m_repo.Refs.MatchingNames((IEnumerable<string>) searchRefs, GitRefSearchType.StartsWith);
      if (source.Count != 0)
      {
        if (symrefs)
        {
          TfsGitRef tfsGitRef = source.FirstOrDefault<TfsGitRef>((Func<TfsGitRef, bool>) (s => s.IsDefaultBranch)) ?? source.FirstOrDefault<TfsGitRef>((Func<TfsGitRef, bool>) (s => s.Name.Equals("refs/heads/master", StringComparison.Ordinal))) ?? source.FirstOrDefault<TfsGitRef>((Func<TfsGitRef, bool>) (s => s.Name.StartsWith("refs/heads/", StringComparison.Ordinal)));
          if (tfsGitRef != null)
            ProtocolHelper.WriteLine(this.m_output, FormattableString.Invariant(FormattableStringFactory.Create("{0} HEAD symref-target:{1}", (object) tfsGitRef.ObjectId, (object) tfsGitRef.Name)));
        }
        foreach (TfsGitRef tagRef in source)
        {
          string line = FormattableString.Invariant(FormattableStringFactory.Create("{0} {1}", (object) tagRef.ObjectId, (object) tagRef.Name));
          TfsGitObject peeledObject;
          if (peel && this.m_tagPeeler.TryPeelTagRef(tagRef, out peeledObject))
            ProtocolHelper.WriteLine(this.m_output, line + FormattableString.Invariant(FormattableStringFactory.Create(" peeled:{0}", (object) peeledObject.ObjectId)));
          else
            ProtocolHelper.WriteLine(this.m_output, line);
        }
      }
      ProtocolHelper.WriteLine(this.m_output, (string) null);
    }

    private bool TryParseFeature(byte[] line, out GitPackFeature? feature)
    {
      feature = new GitPackFeature?();
      if (ProtocolHelper.IsPrefixMatch(line, ProtocolHelper.MagicBytes.ThinPack))
        feature = new GitPackFeature?(GitPackFeature.ThinPack);
      else if (ProtocolHelper.IsPrefixMatch(line, ProtocolHelper.MagicBytes.NoProgress))
        feature = new GitPackFeature?(GitPackFeature.NoProgress);
      else if (ProtocolHelper.IsPrefixMatch(line, ProtocolHelper.MagicBytes.IncludeTag))
        feature = new GitPackFeature?(GitPackFeature.IncludeTag);
      else if (ProtocolHelper.IsPrefixMatch(line, ProtocolHelper.MagicBytes.OfsDelta))
        feature = new GitPackFeature?(GitPackFeature.OfsDelta);
      else if (ProtocolHelper.IsPrefixMatch(line, ProtocolHelper.MagicBytes.Shallow))
        feature = new GitPackFeature?(GitPackFeature.Shallow);
      else
        ProtocolHelper.IsPrefixMatch(line, ProtocolHelper.MagicBytes.Agent);
      return feature.HasValue;
    }

    private protected override void TransmitPackfile(
      CommitIdSet haves,
      HashSet<Sha1Id> wants,
      Shallows shallows,
      GitObjectFilter filter,
      GitPackFeature clientOptions)
    {
      ProtocolHelper.WriteLine(this.m_output, "packfile");
      base.TransmitPackfile(haves, wants, shallows, filter, clientOptions);
    }

    private (CommitIdSet haves, HashSet<Sha1Id> wants, Shallows shallows, GitObjectFilter filter, GitPackFeature features, bool packTransmitted) ParseFetchArguments(
      byte[] line)
    {
      while (line.Length != 1)
        line = ProtocolHelper.ReadLineBytes(this.m_input);
      bool flag1 = this.m_rc.IsFeatureEnabled("Git.EnablePartialClone");
      CommitIdSet haves = new CommitIdSet();
      HashSet<Sha1Id> sha1IdSet = new HashSet<Sha1Id>();
      Shallows shallows = new Shallows(this.m_rc.RequestTracer, this.m_repo.Objects);
      GitObjectFilter filter = new GitObjectFilter();
      bool flag2 = false;
      GitPackFeature clientOptions = GitPackFeature.SideBand | GitPackFeature.SideBand64K;
      while ((line = ProtocolHelper.ReadLineBytes(this.m_input)) != null)
      {
        if (ProtocolHelper.IsPrefixMatch(line, ProtocolHelper.MagicBytes.WantPrefix))
          sha1IdSet.Add(new Sha1Id(GitUtils.ObjectIdFromUTF8Bytes(line, ProtocolHelper.MagicBytes.WantPrefix.Length)));
        else if (ProtocolHelper.IsPrefixMatch(line, ProtocolHelper.MagicBytes.HavePrefix))
        {
          TfsGitObject haveObject = this.m_repo.TryLookupObject(new Sha1Id(GitUtils.ObjectIdFromUTF8Bytes(line, ProtocolHelper.MagicBytes.HavePrefix.Length)));
          if (haveObject != null)
            this.ProcessHave(haves, haveObject);
        }
        else if (ProtocolHelper.IsPrefixMatch(line, ProtocolHelper.MagicBytes.ShallowPrefix))
        {
          Sha1Id shallowId = new Sha1Id(GitUtils.ObjectIdFromUTF8Bytes(line, ProtocolHelper.MagicBytes.ShallowPrefix.Length));
          shallows.AddClientShallow(shallowId);
        }
        else if (ProtocolHelper.IsPrefixMatch(line, ProtocolHelper.MagicBytes.DeepenPrefix))
        {
          int length = ProtocolHelper.MagicBytes.DeepenPrefix.Length;
          string depthStr = GitEncodingUtil.SafeUtf8NoBom.GetString(line, length, line.Length - length);
          shallows.SetDeepenParams(depthStr);
        }
        else if (flag1 && ProtocolHelper.IsPrefixMatch(line, ProtocolHelper.MagicBytes.FilterPrefix))
        {
          byte[] filterPrefix = ProtocolHelper.MagicBytes.FilterPrefix;
          filter = FilterSpecParser.Parse(GitEncodingUtil.SafeUtf8NoBom.GetString(line, filterPrefix.Length, line.Length - filterPrefix.Length));
        }
        else
        {
          if (ProtocolHelper.IsPrefixMatch(line, ProtocolHelper.MagicBytes.Done))
          {
            flag2 = true;
            this.TransmitPackfile(haves, sha1IdSet, shallows, filter, clientOptions);
            break;
          }
          GitPackFeature? feature;
          if (!this.TryParseFeature(line, out feature))
            throw new GitProtocolException("Unknown input");
          clientOptions |= feature.Value;
        }
      }
      if (this.m_rc.IsFeatureEnabled("Git.DisallowShallowAndPartialClone") && shallows.EnforceMaxDepth && !filter.FilterIsNoop())
        filter = new GitObjectFilter();
      if (this.m_wants != null)
        this.m_wants.AddRange<Sha1Id, HashSet<Sha1Id>>((IEnumerable<Sha1Id>) sha1IdSet);
      return (haves, sha1IdSet, shallows, filter, clientOptions, flag2);
    }

    private (bool peel, bool symrefs, List<byte[]> refPrefixes) ParseLsRefsArguments(
      byte[] incomingLine)
    {
      bool flag1 = false;
      bool flag2 = false;
      List<byte[]> numArrayList = new List<byte[]>();
      while ((incomingLine = ProtocolHelper.ReadLineBytes(this.m_input)) != null)
      {
        byte[] refPrefix;
        if ((refPrefix = UploadPackParserV2.ParseRefPrefix(incomingLine)) != null)
          numArrayList.Add(refPrefix);
        if (ProtocolHelper.IsPrefixMatch(incomingLine, ProtocolHelper.MagicBytes.Peel))
          flag1 = true;
        if (ProtocolHelper.IsPrefixMatch(incomingLine, ProtocolHelper.MagicBytes.Symrefs))
          flag2 = true;
      }
      return (flag1, flag2, numArrayList);
    }

    public override void WriteError(string errorMessage)
    {
      errorMessage = SecretUtility.ScrubSecrets(errorMessage);
      ProtocolHelper.WriteSideband(this.m_output, SidebandChannel.Error, errorMessage);
    }

    private enum Protocolv2Command : byte
    {
      LsRefs,
      Fetch,
      Unknown,
    }
  }
}
