// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.Protocol.UploadPack.Shallows
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;

namespace Microsoft.TeamFoundation.Git.Server.Protocol.UploadPack
{
  internal class Shallows
  {
    private readonly ITraceRequest m_tracer;
    private readonly IGitObjectSet m_objectSet;
    private readonly Dictionary<Sha1Id, bool> m_oldShallows;
    private const string c_layer = "Shallows";

    public Shallows(ITraceRequest tracer, IGitObjectSet objectSet)
    {
      this.m_tracer = tracer;
      this.m_objectSet = objectSet;
      this.m_oldShallows = new Dictionary<Sha1Id, bool>();
    }

    public int Depth { get; private set; }

    public bool EnforceMaxDepth { get; private set; }

    public ICollection<Sha1Id> ClientShallows => (ICollection<Sha1Id>) this.m_oldShallows.Keys;

    public void SetDeepenParams(string depthStr)
    {
      try
      {
        this.Depth = int.Parse(depthStr);
      }
      catch (Exception ex)
      {
        throw new GitProtocolException("depth must be an integer. " + ex.Message);
      }
      this.m_tracer.Trace(1013168, TraceLevel.Verbose, GitServerUtils.TraceArea, nameof (Shallows), "Client depth: {0}", (object) depthStr);
      if (this.Depth < 1)
        return;
      this.EnforceMaxDepth = true;
    }

    public void AddClientShallow(Sha1Id shallowId) => this.m_oldShallows[shallowId] = false;

    public void WriteShallowInfo(HashSet<Sha1Id> wants, Stream output)
    {
      if (!this.EnforceMaxDepth)
        throw new InvalidOperationException("Shallow info should only be reported if EnforceMaxDepth.");
      foreach (Sha1Id newShallow in this.ComputeNewShallows(wants))
      {
        if (!this.m_oldShallows.ContainsKey(newShallow))
          ProtocolHelper.WriteSideband(output, SidebandChannel.None, FormattableString.Invariant(FormattableStringFactory.Create("shallow {0}", (object) newShallow)));
      }
      foreach (KeyValuePair<Sha1Id, bool> oldShallow in this.m_oldShallows)
      {
        if (oldShallow.Value)
          ProtocolHelper.WriteSideband(output, SidebandChannel.None, FormattableString.Invariant(FormattableStringFactory.Create("unshallow {0}", (object) oldShallow.Key)));
      }
    }

    private HashSet<Sha1Id> ComputeNewShallows(HashSet<Sha1Id> wants)
    {
      HashSet<Sha1Id> newShallows = new HashSet<Sha1Id>();
      HashSet<TfsGitCommit> tfsGitCommitSet1 = new HashSet<TfsGitCommit>((IEqualityComparer<TfsGitCommit>) TfsGitObjectEqualityComparer.Instance);
      foreach (Sha1Id want in wants)
      {
        TfsGitObject gitObject = this.m_objectSet.TryLookupObject(want);
        TfsGitCommit tfsGitCommit = (TfsGitCommit) null;
        if (gitObject != null)
          tfsGitCommit = gitObject.TryResolveToCommit();
        if (tfsGitCommit != null)
          tfsGitCommitSet1.Add(tfsGitCommit);
      }
      for (int index = 0; index < this.Depth - 1 && tfsGitCommitSet1.Count > 0; ++index)
      {
        HashSet<TfsGitCommit> tfsGitCommitSet2 = new HashSet<TfsGitCommit>((IEqualityComparer<TfsGitCommit>) TfsGitObjectEqualityComparer.Instance);
        foreach (TfsGitCommit tfsGitCommit in tfsGitCommitSet1)
        {
          if (!this.m_oldShallows.ContainsKey(tfsGitCommit.ObjectId))
            wants.Add(tfsGitCommit.ObjectId);
          else
            this.m_oldShallows[tfsGitCommit.ObjectId] = true;
          foreach (TfsGitCommit parent in (IEnumerable<TfsGitCommit>) tfsGitCommit.GetParents())
          {
            if (!wants.Contains(parent.ObjectId))
              tfsGitCommitSet2.Add(parent);
          }
        }
        tfsGitCommitSet1 = tfsGitCommitSet2;
      }
      foreach (TfsGitCommit tfsGitCommit in tfsGitCommitSet1)
      {
        if (!this.m_oldShallows.ContainsKey(tfsGitCommit.ObjectId))
          newShallows.Add(tfsGitCommit.ObjectId);
        wants.Add(tfsGitCommit.ObjectId);
      }
      return newShallows;
    }
  }
}
