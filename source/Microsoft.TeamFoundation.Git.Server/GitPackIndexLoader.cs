// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.GitPackIndexLoader
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Git.Server.PackIndex;
using Microsoft.TeamFoundation.Git.Server.Riff;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Microsoft.TeamFoundation.Git.Server
{
  internal class GitPackIndexLoader
  {
    private readonly ITraceRequest m_tracer;
    private readonly IGitDataFileProvider m_dataFilePrv;
    private readonly IGitKnownFilesProvider m_knownFilesPrv;
    private const string c_layer = "GitPackIndexLoader";

    public GitPackIndexLoader(
      ITraceRequest tracer,
      IGitDataFileProvider dataFilePrv,
      IGitKnownFilesProvider knownFilesPrv)
    {
      this.m_tracer = tracer;
      this.m_dataFilePrv = dataFilePrv;
      this.m_knownFilesPrv = knownFilesPrv;
    }

    internal ConcatGitPackIndex LoadIndex(Sha1Id? indexId)
    {
      if (!indexId.HasValue)
      {
        this.m_tracer.Trace(1013083, TraceLevel.Verbose, GitServerUtils.TraceArea, nameof (GitPackIndexLoader), "EmptyIndexFound");
        return ConcatGitPackIndex.Empty;
      }
      this.m_tracer.Trace(1013092, TraceLevel.Verbose, GitServerUtils.TraceArea, nameof (GitPackIndexLoader), "Initializing index after finding file");
      List<IGitPackIndex> subindexes = (List<IGitPackIndex>) null;
      try
      {
        IGitPackIndex gitPackIndex = this.LoadSubIndex(indexId.Value);
        subindexes = gitPackIndex.BaseIndexIds.Select<Sha1Id, IGitPackIndex>((Func<Sha1Id, IGitPackIndex>) (x => this.LoadSubIndex(x))).ToList<IGitPackIndex>();
        subindexes.Add(gitPackIndex);
        ConcatGitPackIndex full = ConcatGitPackIndex.CreateFull((IEnumerable<IGitPackIndex>) subindexes);
        subindexes = (List<IGitPackIndex>) null;
        this.m_tracer.Trace(1013093, TraceLevel.Verbose, GitServerUtils.TraceArea, nameof (GitPackIndexLoader), "Index initialized");
        return full;
      }
      finally
      {
        if (subindexes != null)
        {
          foreach (IDisposable disposable in subindexes)
            disposable.Dispose();
        }
      }
    }

    public IGitPackIndex LoadSubIndex(Sha1Id subindexId)
    {
      string odbFileName = StorageUtils.GetOdbFileName(subindexId, KnownFileType.Index);
      Stream stream = (Stream) null;
      RiffFile riff = (RiffFile) null;
      try
      {
        stream = this.m_dataFilePrv.GetMemoryMappedStream(odbFileName);
        if (RiffFile.TryLoad(stream, out riff, false))
        {
          stream = (Stream) null;
          M91GitPackIndex m91GitPackIndex = riff.ListType == 863528041U ? new M91GitPackIndex(riff, new Sha1Id?(subindexId)) : throw new InvalidGitIndexException((Exception) new InvalidOperationException(string.Format("RIFF form type = {0}", (object) riff.ListType)));
          riff = (RiffFile) null;
          return (IGitPackIndex) m91GitPackIndex;
        }
        stream.Position = 0L;
        M43GitPackIndex m43GitPackIndex = new M43GitPackIndex(stream, new Sha1Id?(subindexId), new Lazy<IReadOnlyDictionary<string, KnownFile>>((Func<IReadOnlyDictionary<string, KnownFile>>) (() => this.m_knownFilesPrv.Read()), false));
        stream = (Stream) null;
        return (IGitPackIndex) m43GitPackIndex;
      }
      finally
      {
        stream?.Dispose();
        riff?.Dispose();
      }
    }
  }
}
