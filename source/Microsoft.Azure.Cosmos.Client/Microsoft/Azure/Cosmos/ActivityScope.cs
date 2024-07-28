// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.ActivityScope
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using System;
using System.Diagnostics;

namespace Microsoft.Azure.Cosmos
{
  internal sealed class ActivityScope : IDisposable
  {
    private readonly Guid ambientActivityId;

    public ActivityScope(Guid activityId)
    {
      this.ambientActivityId = Trace.CorrelationManager.ActivityId;
      Trace.CorrelationManager.ActivityId = activityId;
    }

    public void Dispose() => Trace.CorrelationManager.ActivityId = this.ambientActivityId;

    public static ActivityScope CreateIfDefaultActivityId() => Trace.CorrelationManager.ActivityId == Guid.Empty ? new ActivityScope(Guid.NewGuid()) : (ActivityScope) null;
  }
}
