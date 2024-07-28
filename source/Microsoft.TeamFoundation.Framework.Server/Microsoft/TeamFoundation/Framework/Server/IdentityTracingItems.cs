// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.IdentityTracingItems
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class IdentityTracingItems
  {
    private IdentityTracingItems()
    {
    }

    public IdentityTracingItems(Guid cuid, Guid tenantId, Guid providerId)
    {
      this.Cuid = cuid;
      this.TenantId = tenantId;
      this.ProviderId = providerId;
    }

    public Guid Cuid { get; private set; }

    public Guid TenantId { get; private set; }

    public Guid ProviderId { get; private set; }
  }
}
