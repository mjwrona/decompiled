// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Common.Diagnostics.VssTraceActivity
// Assembly: Microsoft.VisualStudio.Services.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9C46641-2F1F-44C4-9C2E-4328CD5FFC63
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Common.dll

using System;
using System.Diagnostics;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Common.Diagnostics
{
  [DataContract]
  [Serializable]
  public sealed class VssTraceActivity
  {
    public const string PropertyName = "MS.VSS.Diagnostics.TraceActivity";
    private static Lazy<VssTraceActivity> s_empty = new Lazy<VssTraceActivity>((Func<VssTraceActivity>) (() => new VssTraceActivity(Guid.Empty)));

    private VssTraceActivity()
    {
    }

    private VssTraceActivity(Guid activityId) => this.Id = activityId;

    [DataMember]
    public Guid Id { get; private set; }

    public static VssTraceActivity Current
    {
      get => CallContext.LogicalGetData("MS.VSS.Diagnostics.TraceActivity") as VssTraceActivity;
      private set => CallContext.LogicalSetData("MS.VSS.Diagnostics.TraceActivity", (object) value);
    }

    public static VssTraceActivity Empty => VssTraceActivity.s_empty.Value;

    public IDisposable EnterCorrelationScope() => (IDisposable) new VssTraceActivity.CorrelationScope(this);

    public static VssTraceActivity GetOrCreate()
    {
      if (VssTraceActivity.Current != null)
        return VssTraceActivity.Current;
      return Trace.CorrelationManager.ActivityId == Guid.Empty ? new VssTraceActivity(Guid.NewGuid()) : new VssTraceActivity(Trace.CorrelationManager.ActivityId);
    }

    public static VssTraceActivity New(Guid activityId = default (Guid)) => new VssTraceActivity(activityId == new Guid() ? Guid.NewGuid() : activityId);

    private sealed class CorrelationScope : IDisposable
    {
      private bool m_swap;
      private VssTraceActivity m_previousActivity;

      public CorrelationScope(VssTraceActivity activity)
      {
        this.m_previousActivity = VssTraceActivity.Current;
        if (this.m_previousActivity != null && !(this.m_previousActivity.Id != activity.Id))
          return;
        this.m_swap = true;
        VssTraceActivity.Current = activity;
      }

      public void Dispose()
      {
        if (!this.m_swap)
          return;
        try
        {
          this.m_swap = false;
        }
        finally
        {
          VssTraceActivity.Current = this.m_previousActivity;
        }
      }
    }
  }
}
