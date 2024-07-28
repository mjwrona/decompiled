// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.RemoteSettings.LocalTestProvider
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.RemoteSettings
{
  internal class LocalTestProvider : TargetedNotificationsProviderBase
  {
    private readonly IEnumerable<IDirectoryReader> directories;
    private readonly ILocalTestParser localTestParser;

    public LocalTestProvider(RemoteSettingsInitializer initializer)
      : base(initializer.LocalTestRemoteSettingsStorageHandler, initializer)
    {
      this.directories = (IEnumerable<IDirectoryReader>) initializer.LocalTestDirectories.OrderBy<IDirectoryReader, int>((Func<IDirectoryReader, int>) (x => x.Priority));
      this.localTestParser = initializer.LocalTestParser;
    }

    public override string Name => "LocalTestTargetedNotifications";

    protected override async Task<ActionResponseBag> GetTargetedNotificationActionsAsync()
    {
      LocalTestProvider localTestProvider = this;
      // ISSUE: reference to a compiler-generated method
      IEnumerable<Task<IEnumerable<ActionResponse>>> tasks = localTestProvider.directories.SelectMany<IDirectoryReader, DirectoryReaderContext>((Func<IDirectoryReader, IEnumerable<DirectoryReaderContext>>) (d => d.ReadAllFiles())).Select<DirectoryReaderContext, Task<IEnumerable<ActionResponse>>>(new Func<DirectoryReaderContext, Task<IEnumerable<ActionResponse>>>(localTestProvider.\u003CGetTargetedNotificationActionsAsync\u003Eb__5_1));
      ActionResponseBag notificationActionsAsync = new ActionResponseBag();
      ActionResponseBag actionResponseBag = notificationActionsAsync;
      actionResponseBag.Actions = ((IEnumerable<IEnumerable<ActionResponse>>) await Task.WhenAll<IEnumerable<ActionResponse>>(tasks)).SelectMany<IEnumerable<ActionResponse>, ActionResponse>((Func<IEnumerable<ActionResponse>, IEnumerable<ActionResponse>>) (x => x));
      return notificationActionsAsync;
    }
  }
}
