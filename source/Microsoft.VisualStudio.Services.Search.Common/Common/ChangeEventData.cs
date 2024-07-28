// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Common.ChangeEventData
// Assembly: Microsoft.VisualStudio.Services.Search.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8E09DCBA-148E-4EB7-BB73-B53B030BE93E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Common.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Search.Common
{
  [DataContract]
  public class ChangeEventData : IExtensibleDataObject
  {
    private long m_delayTicks;

    public ChangeEventData(ExecutionContext executionContext)
    {
      if (executionContext == null)
        throw new ArgumentNullException(nameof (executionContext));
      this.Trigger = executionContext.ExecutionTracerContext.Trigger;
      this.CorrelationId = executionContext.ExecutionTracerContext.TracerCICorrelationDetails.CorrelationId;
      this.TriggerTimeUtc = executionContext.ExecutionTracerContext.TracerCICorrelationDetails.TriggerTimeUtc;
    }

    internal ChangeEventData()
    {
      this.Trigger = 0;
      this.CorrelationId = Guid.Empty.ToString();
      this.TriggerTimeUtc = DateTime.UtcNow;
    }

    public ExtensionDataObject ExtensionData { get; set; }

    public int Trigger { get; set; }

    [DataMember(Order = 1)]
    public string CorrelationId { get; set; }

    [DataMember(Order = 2)]
    public TimeSpan Delay
    {
      get => TimeSpan.FromTicks(this.m_delayTicks);
      set => this.m_delayTicks = value.Ticks;
    }

    [DataMember(Order = 3)]
    public DateTime TriggerTimeUtc { get; set; }

    [DataMember(Order = 4)]
    public int RetryAttemptCount { get; set; }
  }
}
