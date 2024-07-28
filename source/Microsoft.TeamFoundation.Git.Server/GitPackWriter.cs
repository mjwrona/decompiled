// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.GitPackWriter
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.BusinessIntelligence;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Microsoft.TeamFoundation.Git.Server
{
  internal class GitPackWriter : IGitPackWriter
  {
    private readonly ITfsGitContentDB<TfsGitObjectLocation> m_contentDB;
    private readonly GitPackWriterThrottler m_throttler;
    private readonly ClientTraceData m_ctData;
    private readonly int m_streamBufferSize;
    private static readonly VssPerformanceCounter s_currentClone = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_Service_GitCurrentClones");

    public GitPackWriter(
      ITfsGitContentDB<TfsGitObjectLocation> contentDB,
      GitPackWriterThrottler throttler = null,
      ClientTraceData ctData = null,
      int streamBufferSize = 4096)
    {
      this.m_contentDB = contentDB;
      this.m_throttler = throttler;
      this.m_ctData = ctData;
      this.m_streamBufferSize = streamBufferSize;
    }

    public void Write(
      ISet<Sha1Id> objectsToSend,
      Stream output,
      ISet<Sha1Id> basesClientHas = null,
      Action beforeStreamingResponse = null,
      Predicate<ObjectIdAndGitPackIndexEntry> objectsToIncludeFilter = null)
    {
      Stopwatch stopwatch1 = Stopwatch.StartNew();
      List<ObjectIdAndGitPackIndexEntry> serialize = this.BuildObjectsToSerialize((IEnumerable<Sha1Id>) objectsToSend, objectsToIncludeFilter);
      stopwatch1.Stop();
      if (beforeStreamingResponse != null)
        beforeStreamingResponse();
      using (GitPackSerializer gitPackSerializer = new GitPackSerializer(output, serialize.Count, true, this.m_streamBufferSize))
      {
        long totalBytesWritten = 0;
        long totalBytesCompressed = 0;
        Stopwatch stopwatch2 = Stopwatch.StartNew();
        int count = serialize.Count;
        for (int index = 0; index < count; ++index)
        {
          ObjectIdAndGitPackIndexEntry gitPackIndexEntry = serialize[index];
          using (Stream rawContent = this.m_contentDB.GetRawContent(gitPackIndexEntry.Location))
          {
            GitPackObjectType type;
            GitServerUtils.ReadPackEntryHeader(rawContent, out type, out long _);
            if (type == GitPackObjectType.OfsDelta)
              throw new InvalidGitPackEntryHeaderException("OfsDelta");
            if (type == GitPackObjectType.RefDelta)
            {
              Sha1Id sha1Id;
              try
              {
                sha1Id = Sha1Id.FromStream(rawContent);
              }
              catch (Sha1IdStreamReadException ex)
              {
                throw new InvalidGitPackEntryHeaderException((Exception) ex);
              }
              if (objectsToSend.Contains(sha1Id) || basesClientHas != null && basesClientHas.Contains(sha1Id))
              {
                rawContent.Seek(0L, SeekOrigin.Begin);
                totalBytesWritten += gitPackSerializer.AddRaw(rawContent);
              }
              else
              {
                using (Stream content = this.m_contentDB.GetContent(gitPackIndexEntry.Location, gitPackIndexEntry.ObjectId))
                {
                  long num = gitPackSerializer.AddInflatedStreamWithTypeAndSize(content, gitPackIndexEntry.ObjectType, content.Length);
                  totalBytesCompressed += num;
                  totalBytesWritten += num;
                }
              }
            }
            else
            {
              rawContent.Seek(0L, SeekOrigin.Begin);
              totalBytesWritten += gitPackSerializer.AddRaw(rawContent);
            }
          }
          GitPackWriterThrottler throttler = this.m_throttler;
          int num1 = throttler != null ? throttler.WorkUnitSize : 32;
          if (index % num1 == 0)
            this.m_throttler?.ThrottleIfBusy();
        }
        stopwatch2.Stop();
        gitPackSerializer.Complete();
        this.WriteCIData(totalBytesWritten, totalBytesCompressed, stopwatch2.Elapsed, stopwatch1.Elapsed);
      }
    }

    private void WriteCIData(
      long totalBytesWritten,
      long totalBytesCompressed,
      TimeSpan elapsed,
      TimeSpan lookupElapsed)
    {
      if (this.m_ctData == null)
        return;
      this.m_ctData.Add("LookupObjectsMillis", (object) lookupElapsed.TotalMilliseconds);
      this.m_ctData.Add("NumberOfBytes", (object) totalBytesWritten);
      this.m_ctData.Add("NumberOfBytesRecompressed", (object) totalBytesCompressed);
      this.m_ctData.Add("PackWriteMillis", (object) elapsed.TotalMilliseconds);
      if (this.m_throttler == null)
        return;
      this.m_ctData.Add("PackWriteThrottleMillis", (object) this.m_throttler.ExpectedTotalDelayTimeMs);
      this.m_ctData.Add("PackWriteActualThrottleMillis", (object) this.m_throttler.ActualTotalDelayTimeMs);
      this.m_ctData.Add("PackWriteThrottleCount", (object) this.m_throttler.TotalDelayCount);
    }

    private List<ObjectIdAndGitPackIndexEntry> BuildObjectsToSerialize(
      IEnumerable<Sha1Id> objectsToSend,
      Predicate<ObjectIdAndGitPackIndexEntry> objectsToIncludeFilter)
    {
      IEnumerable<ObjectIdAndGitPackIndexEntry> source = objectsToSend.Select<Sha1Id, ObjectIdAndGitPackIndexEntry>((Func<Sha1Id, ObjectIdAndGitPackIndexEntry>) (objectId =>
      {
        GitPackObjectType packType;
        TfsGitObjectLocation rawKey;
        this.m_contentDB.LookupObject<TfsGitObjectLocation>(objectId, out packType, out rawKey);
        return new ObjectIdAndGitPackIndexEntry(objectId, packType, rawKey);
      }));
      if (objectsToIncludeFilter != null)
        source = source.Where<ObjectIdAndGitPackIndexEntry>((Func<ObjectIdAndGitPackIndexEntry, bool>) (x => objectsToIncludeFilter(x)));
      List<ObjectIdAndGitPackIndexEntry> list = source.ToList<ObjectIdAndGitPackIndexEntry>();
      list.Sort();
      return list;
    }
  }
}
