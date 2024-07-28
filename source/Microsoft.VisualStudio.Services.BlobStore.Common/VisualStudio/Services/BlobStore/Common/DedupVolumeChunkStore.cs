// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Common.DedupVolumeChunkStore
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FAFB0281-5CF2-4D3F-992C-49FBB9BEC906
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.BlobStore.Common.dll

using Microsoft.DataDeduplication.Interop;
using Microsoft.VisualStudio.Services.Content.Common;
using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.BlobStore.Common
{
  [CLSCompliant(false)]
  public static class DedupVolumeChunkStore
  {
    private static RunOnce<string, TaggedUnionValue<IDedupDataPortManager, string>> managers = new RunOnce<string, TaggedUnionValue<IDedupDataPortManager, string>>(true);
    private const int E_CLASSNOTREG = -2147221164;
    private const int CO_E_SERVER_EXEC_FAILURE = -2146959355;
    private const int E_NOINTERFACE = -2147467262;
    private const int E_ACCESSDENIED = -2147024891;

    private static async Task<TaggedUnionValue<IDedupDataPortManager, string>> CreateForVolumeAsync(
      string volume,
      CancellationToken cancellationToken,
      Action<string> logger)
    {
      int num = 5;
label_1:
      IDedupDataPortManager dataPortManager;
      try
      {
        dataPortManager = (IDedupDataPortManager) Activator.CreateInstance(Type.GetTypeFromCLSID(Guid.Parse("8f107207-1829-48b2-a64b-e61f8e0d9acb"), true));
      }
      catch (COMException ex) when (ex.HResult == -2147221164)
      {
        return new TaggedUnionValue<IDedupDataPortManager, string>(string.Format("Class not registered: E_CLASSNOTREG {0:x}", (object) -2147221164));
      }
      catch (COMException ex) when (ex.HResult == -2146959355)
      {
        --num;
        if (num <= 0)
          return new TaggedUnionValue<IDedupDataPortManager, string>(string.Format("Class not registered: CO_E_SERVER_EXEC_FAILURE {0:x}", (object) -2146959355));
        goto label_1;
      }
      catch (InvalidCastException ex) when (ex.HResult == -2147467262)
      {
        return new TaggedUnionValue<IDedupDataPortManager, string>(string.Format("Class not registered: E_NOINTERFACE {0:x}", (object) -2147467262));
      }
      catch (UnauthorizedAccessException ex) when (ex.HResult == -2147024891)
      {
        return new TaggedUnionValue<IDedupDataPortManager, string>("Dataport requires elevated access.");
      }
      DedupDataPortVolumeStatus volumeStatus;
      do
      {
        cancellationToken.ThrowIfCancellationRequested();
        dataPortManager.GetVolumeStatus(1U, volume, out volumeStatus);
        switch (volumeStatus)
        {
          case DedupDataPortVolumeStatus.NotEnabled:
          case DedupDataPortVolumeStatus.NotAvailable:
            return new TaggedUnionValue<IDedupDataPortManager, string>(string.Format("{0}. Try 'Enable-DedupVolume -Volume {1} -UsageType Backup'", (object) volumeStatus, (object) volume));
          case DedupDataPortVolumeStatus.Initializing:
          case DedupDataPortVolumeStatus.Maintenance:
            logger(string.Format("Current DataPort volume status: {0}. Will retry in 1 second.", (object) volumeStatus));
            await Task.Delay(TimeSpan.FromSeconds(1.0)).ConfigureAwait(false);
            goto case DedupDataPortVolumeStatus.Ready;
          case DedupDataPortVolumeStatus.Ready:
            continue;
          default:
            throw new InvalidOperationException(string.Format("DedupDataPortVolumeStatus:{0}", (object) volumeStatus));
        }
      }
      while (volumeStatus != DedupDataPortVolumeStatus.Ready);
      return new TaggedUnionValue<IDedupDataPortManager, string>(dataPortManager);
    }

    public static async Task<TaggedUnionValue<IDedupDataPort, string>> GetDataPortAsync(
      string pathOnVolume,
      CancellationToken cancellationToken,
      Action<string> logger)
    {
      string volume = VolumeHelper.GetVolumeRootFromPath(pathOnVolume);
      return (await DedupVolumeChunkStore.managers.RunOnceAsync(volume, (Func<Task<TaggedUnionValue<IDedupDataPortManager, string>>>) (() => DedupVolumeChunkStore.CreateForVolumeAsync(volume, cancellationToken, logger)))).Match<TaggedUnionValue<IDedupDataPort, string>>((Func<IDedupDataPortManager, TaggedUnionValue<IDedupDataPort, string>>) (manager =>
      {
        IDedupDataPort dataPort;
        manager.GetVolumeDataPort(1U, volume, out dataPort);
        dataPort.GetStatus(out DedupDataPortVolumeStatus _, out uint _);
        return new TaggedUnionValue<IDedupDataPort, string>(dataPort);
      }), (Func<string, TaggedUnionValue<IDedupDataPort, string>>) (error => new TaggedUnionValue<IDedupDataPort, string>("Could not initialize dataport for '" + volume + "' as '" + error + "'")));
    }
  }
}
