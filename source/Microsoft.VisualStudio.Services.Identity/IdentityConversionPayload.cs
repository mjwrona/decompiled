// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.IdentityConversionPayload
// Assembly: Microsoft.VisualStudio.Services.Identity, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1372DA81-8681-4BFE-8E91-D1AB4333F834
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Identity.dll

using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Identity
{
  public class IdentityConversionPayload
  {
    public List<Microsoft.VisualStudio.Services.Identity.Identity> ActiveIdentities { get; set; }

    public List<Microsoft.VisualStudio.Services.Identity.Identity> InactiveIdentities { get; set; }

    public Guid TenantId { get; set; } = Guid.Empty;

    public Guid EventId { get; set; } = Guid.Empty;

    public bool IsFullTracing { get; set; }
  }
}
