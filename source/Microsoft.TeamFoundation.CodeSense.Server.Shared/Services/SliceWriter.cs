// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.CodeSense.Server.Services.SliceWriter
// Assembly: Microsoft.TeamFoundation.CodeSense.Server.Shared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 548902A5-AE61-4BC7-8D52-315B40AB5900
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.CodeSense.Server.Shared.dll

using Microsoft.TeamFoundation.CodeSense.Server.Data;
using Microsoft.TeamFoundation.CodeSense.Server.Extensions;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.TeamFoundation.CodeSense.Server.Services
{
  public class SliceWriter : IAggregateWriter, IDisposable
  {
    private readonly IVssRequestContext requestContext;
    private readonly IFileDataService fileDataService;
    private readonly SliceSource source;
    private readonly List<FileChangeSliceResultV3> slicesToAdd = new List<FileChangeSliceResultV3>();
    private readonly List<SliceDescriptor> sliceDescriptorsToAdd = new List<SliceDescriptor>();
    private readonly Stopwatch lastFlushStopWatch = new Stopwatch();
    private static readonly TimeSpan FlushFrequency = TimeSpan.FromMinutes(10.0);
    private readonly int MaxSliceBufferSize;
    private long storageGrowthCounter;

    public SliceWriter(
      IVssRequestContext requestContext,
      IFileDataService fileDataService,
      SliceSource source)
    {
      this.requestContext = requestContext;
      this.fileDataService = fileDataService;
      this.source = source;
      this.MaxSliceBufferSize = requestContext.GetService<IVssRegistryService>().GetSliceBufferSize(requestContext);
    }

    public void EnqueueSlice(FileChangeSliceResultV3 slice)
    {
      FileChangeSliceResultV3 changeSliceResultV3 = this.slicesToAdd.LastOrDefault<FileChangeSliceResultV3>();
      if (changeSliceResultV3 != null && changeSliceResultV3.ChangesId != slice.ChangesId || this.slicesToAdd.Count >= this.MaxSliceBufferSize)
        this.PersistSlices(changeSliceResultV3.ChangesId);
      this.slicesToAdd.Add(slice);
      this.FlushIfNeeded();
    }

    private void PersistSlices(int changesId)
    {
      long length;
      int fileId = this.fileDataService.PersistData<IEnumerable<FileChangeSliceResultV3>>(this.requestContext, (IEnumerable<FileChangeSliceResultV3>) this.slicesToAdd, out length);
      this.ManageStorageGrowth(length);
      string path = Guid.NewGuid().ToString();
      int changesId1 = changesId;
      int source = (int) this.source;
      this.sliceDescriptorsToAdd.Add(new SliceDescriptor(fileId, path, changesId1, (SliceSource) source));
      this.slicesToAdd.Clear();
    }

    private void ManageStorageGrowth(long increment, bool forceUpdate = false)
    {
      using (this.requestContext.AcquireExemptionLock())
      {
        this.storageGrowthCounter += increment;
        this.requestContext.GetService<TeamFoundationCounterService>().UpdateStorageGrowth(this.requestContext, ref this.storageGrowthCounter, forceUpdate);
      }
    }

    private void FlushIfNeeded()
    {
      this.lastFlushStopWatch.Start();
      if (!(this.lastFlushStopWatch.Elapsed > SliceWriter.FlushFrequency))
        return;
      this.Flush();
    }

    private void Flush()
    {
      if (this.slicesToAdd.Any<FileChangeSliceResultV3>())
        this.PersistSlices(this.slicesToAdd.Last<FileChangeSliceResultV3>().ChangesId);
      if (this.sliceDescriptorsToAdd.Any<SliceDescriptor>())
      {
        using (this.requestContext.AcquireExemptionLock())
        {
          using (ICodeSenseSqlResourceComponent component = this.requestContext.CreateComponent<ICodeSenseSqlResourceComponent, CodeSenseSqlResourceComponent>())
            component.AddSlices((IEnumerable<SliceDescriptor>) this.sliceDescriptorsToAdd);
        }
        this.sliceDescriptorsToAdd.Clear();
        AggregationStore.StartAggregatorJob(this.requestContext);
      }
      this.lastFlushStopWatch.Stop();
      this.lastFlushStopWatch.Reset();
    }

    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize((object) this);
    }

    private void Dispose(bool isDispose)
    {
      if (!isDispose)
        return;
      this.Flush();
      this.ManageStorageGrowth(0L, true);
    }
  }
}
