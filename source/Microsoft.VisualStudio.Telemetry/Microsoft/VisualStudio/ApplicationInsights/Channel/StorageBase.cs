// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.ApplicationInsights.Channel.StorageBase
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using Microsoft.VisualStudio.ApplicationInsights.Extensibility.Implementation.Tracing;
using Microsoft.VisualStudio.LocalLogger;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.ApplicationInsights.Channel
{
  internal abstract class StorageBase
  {
    protected IDictionary<string, string> peekedTransmissions;

    internal ulong CapacityInBytes { get; set; }

    internal uint MaxFiles { get; set; }

    internal abstract string FolderName { get; }

    internal abstract DirectoryInfo StorageFolder { get; }

    internal abstract StorageTransmission Peek();

    internal abstract IEnumerable<StorageTransmission> PeekAll(CancellationToken token);

    internal abstract void Delete(StorageTransmission transmission);

    internal abstract Task EnqueueAsync(Transmission transmission);

    protected void OnPeekedItemDisposed(string fileName)
    {
      try
      {
        if (LocalFileLoggerService.Default.Enabled)
          LocalFileLoggerService.Default.Log(LocalLoggerSeverity.Info, "Telemetry", string.Format((IFormatProvider) CultureInfo.InvariantCulture, "StorageBase.OnPeekedItemDisposed dispose file {0}", new object[1]
          {
            (object) fileName
          }));
        if (!this.peekedTransmissions.ContainsKey(fileName))
          return;
        this.peekedTransmissions.Remove(fileName);
      }
      catch (Exception ex)
      {
        CoreEventSource.Log.LogVerbose("Failed to remove the item from storage items. Exception: " + ex.ToString());
        LocalFileLoggerService.Default.Log(LocalLoggerSeverity.Error, "Telemetry", string.Format((IFormatProvider) CultureInfo.InvariantCulture, "StorageBase.OnPeekedItemDisposed exception dispose file {0}", new object[1]
        {
          (object) fileName
        }));
      }
    }
  }
}
