// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItemTrackingKpi
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  internal abstract class WorkItemTrackingKpi
  {
    protected const string c_kpiThresholdRegistryPath = "/Service/WorkItemTracking/Settings/Kpi/{0}";

    public virtual string Name { get; protected set; }

    public virtual double Value { get; protected set; }

    public virtual bool Skip { get; protected set; }

    public virtual int DefaultThreshold => 1;

    public virtual int DefaultSamplingRate => 1;

    public WorkItemTrackingKpi(IVssRequestContext requestContext, string name)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      this.Name = name;
      this.Value = requestContext.RequestTimer.LastTracedBlockSpan.TotalMilliseconds;
    }

    public WorkItemTrackingKpi(IVssRequestContext requestContext, string name, int resultCount)
      : this(requestContext, name)
    {
      this.Skip = resultCount > this.GetThreshold(requestContext, string.Format("/Service/WorkItemTracking/Settings/Kpi/{0}", (object) this.Name), this.DefaultThreshold);
    }

    public int GetSamplingRate(IVssRequestContext requestContext) => this.GetThreshold(requestContext, string.Format("/Service/WorkItemTracking/Settings/Kpi/{0}", (object) (this.Name + "Sampling")), this.DefaultSamplingRate);

    protected int GetThreshold(
      IVssRequestContext requestContext,
      string registryPath,
      int defaultThreshold)
    {
      return requestContext.GetService<IVssRegistryService>().GetValue<int>(requestContext, (RegistryQuery) registryPath, true, defaultThreshold);
    }
  }
}
