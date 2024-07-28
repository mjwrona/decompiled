// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.RUResource_DBCPU_RO
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Server;

namespace Microsoft.VisualStudio.Services.Cloud
{
  internal class RUResource_DBCPU_RO : RUResource
  {
    public override long GetRequestCost(IVssRequestContext requestContext) => 0;

    public override double GetTSTUConversionFactor() => 1.0 / 375.0;

    public override bool ShouldQueueZeroIncrement() => true;

    public override string GetName() => "DBCPU_RO";
  }
}
