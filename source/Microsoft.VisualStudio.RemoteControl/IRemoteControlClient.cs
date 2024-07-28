// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.RemoteControl.IRemoteControlClient
// Assembly: Microsoft.VisualStudio.RemoteControl, Version=14.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4D9D0761-3208-49DD-A9E2-BF705DBE6B5D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.RemoteControl.dll

using System;
using System.IO;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.RemoteControl
{
  public interface IRemoteControlClient : IDisposable
  {
    string FullUrl { get; }

    int PollingIntervalMins { get; }

    Stream ReadFile(BehaviorOnStale staleBehavior);

    Task<Stream> ReadFileAsync(BehaviorOnStale staleBehavior);
  }
}
