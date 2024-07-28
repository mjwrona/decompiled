// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.ApplicationInsights.Extensibility.ApplicationStoppingEventArgs
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using Microsoft.VisualStudio.ApplicationInsights.Extensibility.Implementation.Tracing;
using System;
using System.Globalization;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.ApplicationInsights.Extensibility
{
  public class ApplicationStoppingEventArgs : EventArgs
  {
    internal static readonly ApplicationStoppingEventArgs Empty = new ApplicationStoppingEventArgs((Func<Func<Task>, Task>) (asyncMethod => asyncMethod()));
    private readonly Func<Func<Task>, Task> asyncMethodRunner;

    public ApplicationStoppingEventArgs(Func<Func<Task>, Task> asyncMethodRunner) => this.asyncMethodRunner = asyncMethodRunner != null ? asyncMethodRunner : throw new ArgumentNullException(nameof (asyncMethodRunner));

    public async void Run(Func<Task> asyncMethod)
    {
      try
      {
        await this.asyncMethodRunner(asyncMethod);
      }
      catch (Exception ex)
      {
        string msg = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Unexpected excption when handling IApplicationLifecycle.Stopping event:{0}", new object[1]
        {
          (object) ex.ToString()
        });
        CoreEventSource.Log.LogError(msg);
      }
    }
  }
}
