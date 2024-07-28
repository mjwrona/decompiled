// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Utils.ResourceTimer`1
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Diagnostics;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Utils
{
  public class ResourceTimer<T> where T : class
  {
    private Stopwatch stopwatch;

    public ResourceTimer(T resource)
    {
      this.Resource = resource;
      this.stopwatch = new Stopwatch();
    }

    public T Resource { get; }

    public long ElapsedMilliseconds => this.stopwatch.ElapsedMilliseconds;

    public DisposableAction TimerBlock()
    {
      this.stopwatch.Start();
      return new DisposableAction((Action) (() => this.stopwatch.Stop()));
    }
  }
}
