// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.StepDetailLogger
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server.Internal;
using System.Collections.Generic;
using System.Threading;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class StepDetailLogger : IServicingStepDetailLogger
  {
    private readonly ITFLogger m_logger;
    private readonly List<ServicingStepLogEntry> m_warnings = new List<ServicingStepLogEntry>();
    private int m_errorCount;
    private bool m_prefixDuration;

    public StepDetailLogger(ITFLogger logger)
    {
      this.m_logger = logger;
      this.m_prefixDuration = !(logger is ISupportDurationLogging);
    }

    public int WarningCount => this.m_warnings.Count;

    public int ErrorCount => this.m_errorCount;

    public IReadOnlyCollection<ServicingStepLogEntry> Warnings => (IReadOnlyCollection<ServicingStepLogEntry>) this.m_warnings;

    public string FailedStepGroupName { get; protected set; }

    public string FailedStepName { get; protected set; }

    public void AddServicingStepDetail(ServicingStepDetail stepDetail)
    {
      if (stepDetail is ServicingStepLogEntry servicingStepLogEntry)
      {
        if (servicingStepLogEntry.EntryKind == ServicingStepLogEntryKind.Error)
        {
          this.FailedStepGroupName = stepDetail.ServicingStepGroupId;
          this.FailedStepName = stepDetail.ServicingStepId;
        }
        switch (servicingStepLogEntry.EntryKind)
        {
          case ServicingStepLogEntryKind.Informational:
            this.m_logger.Info(servicingStepLogEntry.Message);
            break;
          case ServicingStepLogEntryKind.Warning:
            lock (this.m_warnings)
            {
              this.m_warnings.Add(servicingStepLogEntry);
              this.m_logger.Warning(servicingStepLogEntry.Message);
              break;
            }
          case ServicingStepLogEntryKind.Error:
            Interlocked.Increment(ref this.m_errorCount);
            this.m_logger.Error(servicingStepLogEntry.Message);
            break;
          case ServicingStepLogEntryKind.StepDuration:
          case ServicingStepLogEntryKind.GroupDuration:
          case ServicingStepLogEntryKind.OperationDuration:
          case ServicingStepLogEntryKind.SleepDuration:
            if (this.m_prefixDuration)
            {
              this.m_logger.Info("[" + servicingStepLogEntry.EntryKind.ToString() + "] " + servicingStepLogEntry.Message);
              break;
            }
            this.m_logger.Info(servicingStepLogEntry.Message);
            break;
        }
      }
      else
        this.m_logger.Info(stepDetail.ToLogEntryLine());
    }
  }
}
