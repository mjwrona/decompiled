// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.IdentityMapComponent6
// Assembly: Microsoft.VisualStudio.Services.Identity, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1372DA81-8681-4BFE-8E91-D1AB4333F834
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Identity.dll

namespace Microsoft.VisualStudio.Services.Identity
{
  internal class IdentityMapComponent6 : IdentityMapComponent5
  {
    internal override bool IdentityMapLocked()
    {
      if (this.RequestContext.ExecutionEnvironment.IsHostedDeployment)
        return false;
      this.PrepareStoredProcedure("prc_IdentityMapLocked", false);
      return (int) this.ExecuteScalar() == 1;
    }
  }
}
