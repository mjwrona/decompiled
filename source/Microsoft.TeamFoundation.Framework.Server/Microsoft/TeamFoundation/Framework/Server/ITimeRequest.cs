// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ITimeRequest
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public interface ITimeRequest
  {
    DateTime StartTime { get; }

    DateTime EndTime { get; }

    TimeSpan DelaySpan { get; }

    TimeSpan ExecutionSpan { get; }

    long DelayTime { get; }

    long ConcurrencySemaphoreTime { get; }

    long ExecutionTime { get; }

    long QueueTime { get; }

    long TimeToFirstPage { get; }

    void SetTimeToFirstPageEnd();

    void SetTimeToFirstPageBegin();

    void PauseTimeToFirstPageTimer();

    void ResumeTimeToFirstPageTimer();

    TimeSpan LastTracedBlockSpan { get; set; }

    IDisposable CreateTimeToFirstPageExclusionBlock();

    long PreControllerTime { get; }

    long ControllerTime { get; }

    long PostControllerTime { get; }

    void SetPreControllerTime();

    void SetControllerTime();

    void SetPostControllerTime();
  }
}
