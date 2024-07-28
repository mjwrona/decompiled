// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.RUResource_Concurrency
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;

namespace Microsoft.VisualStudio.Services.Cloud
{
  internal class RUResource_Concurrency : RUResource
  {
    public override long GetRequestCost(IVssRequestContext requestContext) => 1;

    public override double GetTSTUConversionFactor() => throw new VssServiceException("This function should not be called - this indicates a code error");

    public override RUStage WhenToMeasureResourceConsumption { get; internal set; }
  }
}
