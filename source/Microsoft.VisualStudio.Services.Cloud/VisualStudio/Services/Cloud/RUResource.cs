// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.RUResource
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Cloud
{
  public abstract class RUResource
  {
    public static RUResource GetRUResourceFromString(string resource)
    {
      switch (resource)
      {
        case "Concurrency":
          return (RUResource) new RUResource_Concurrency();
        case "ATCPU":
          return (RUResource) new RUResource_ATCPU();
        case "DBCPU":
          return (RUResource) new RUResource_DBCPU();
        case "DBCPU_RO":
          return (RUResource) new RUResource_DBCPU_RO();
        case "Count":
          return (RUResource) new RUResource_Count();
        default:
          return (RUResource) null;
      }
    }

    public abstract long GetRequestCost(IVssRequestContext requestContext);

    public abstract double GetTSTUConversionFactor();

    public virtual RUStage WhenToMeasureResourceConsumption { get; internal set; } = RUStage.EndRequest;

    public virtual bool ShouldQueueZeroIncrement() => false;

    public virtual void FlagThrottleInfo(
      IVssRequestContext requestContext,
      ref ThrottleInfo throttleInfo)
    {
    }

    public virtual string GetName() => ((IEnumerable<string>) this.ToString().Split('_')).LastOrDefault<string>();
  }
}
