// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.FileImportWatermark
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.DataImport;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Microsoft.VisualStudio.Services.Cloud
{
  internal class FileImportWatermark
  {
    private long m_totalBytesWritten;
    private long m_bytesSinceLastLog;
    private long m_bytesSinceLastFileTransferProgressUpdate;
    private long m_numberOfFiles;
    private const int c_defaultMultiplier = 2;

    public Guid LastResourceId { get; private set; }

    public long TotalBytesWritten => this.m_totalBytesWritten;

    public long BytesSinceLastLogWritten => this.m_bytesSinceLastLog;

    public long BytesSinceLastFileTransferProgressUpdate => this.m_bytesSinceLastFileTransferProgressUpdate;

    public long NumberOfFilesProcessed => this.m_numberOfFiles;

    public int Multiplier { get; private set; }

    public static FileImportWatermark Read(IServicingContext servicingContext)
    {
      IVssRequestContext deploymentRequestContext = servicingContext.DeploymentRequestContext;
      FileImportWatermark fileImportWatermark = new FileImportWatermark();
      ISqlRegistryService service = deploymentRequestContext.GetService<ISqlRegistryService>();
      RegistryEntryCollection registryEntryCollection = service.ReadEntries(deploymentRequestContext, new RegistryQuery(servicingContext.GetDataImportSpecificKey("/Configuration/DataImport/{0}/**")));
      RegistryEntry entry;
      if (registryEntryCollection.TryGetValue(servicingContext.GetDataImportSpecificKey("/Configuration/DataImport/{0}/FileWatermark"), out entry))
        fileImportWatermark.LastResourceId = entry.GetValue<Guid>(Guid.Empty);
      if (registryEntryCollection.TryGetValue(servicingContext.GetDataImportSpecificKey("/Configuration/DataImport/{0}/TotalBytesWritten"), out entry))
        fileImportWatermark.IncrementTotalBytesWritten(entry.GetValue<long>(0L));
      fileImportWatermark.Multiplier = service.GetValue<int>(deploymentRequestContext, (RegistryQuery) "/Configuration/DataImport/MaxBlobCopyMultiplier", 2);
      return fileImportWatermark;
    }

    public void Save(IServicingContext servicingContext)
    {
      IVssRequestContext deploymentRequestContext = servicingContext.DeploymentRequestContext;
      RegistryEntry[] registryEntryArray = new RegistryEntry[2]
      {
        RegistryEntry.Create<Guid>(servicingContext.GetDataImportSpecificKey("/Configuration/DataImport/{0}/FileWatermark"), this.LastResourceId),
        RegistryEntry.Create<long>(servicingContext.GetDataImportSpecificKey("/Configuration/DataImport/{0}/TotalBytesWritten"), this.TotalBytesWritten)
      };
      deploymentRequestContext.GetService<ISqlRegistryService>().WriteEntries(deploymentRequestContext, (IEnumerable<RegistryEntry>) registryEntryArray);
    }

    internal void UpdateLastResourceId(Guid? resourceId)
    {
      if (!resourceId.HasValue)
        return;
      this.LastResourceId = resourceId.Value;
    }

    internal void IncrementCounters(long bytes)
    {
      Interlocked.Add(ref this.m_bytesSinceLastLog, bytes);
      Interlocked.Add(ref this.m_bytesSinceLastFileTransferProgressUpdate, bytes);
      this.IncrementTotalBytesWritten(bytes);
      Interlocked.Increment(ref this.m_numberOfFiles);
    }

    internal void ResetBytesSinceLastLog() => Interlocked.Exchange(ref this.m_bytesSinceLastLog, 0L);

    internal void ResetBytesSinceLastFileTransferProgressUpdate() => Interlocked.Exchange(ref this.m_bytesSinceLastFileTransferProgressUpdate, 0L);

    public override string ToString() => string.Format("{0}={1}\r\n{2}={3}\r\n{4}={5}", (object) "Multiplier", (object) this.Multiplier, (object) "LastResourceId", (object) this.LastResourceId, (object) "TotalBytesWritten", (object) this.TotalBytesWritten);

    private void IncrementTotalBytesWritten(long bytes) => Interlocked.Add(ref this.m_totalBytesWritten, bytes);
  }
}
