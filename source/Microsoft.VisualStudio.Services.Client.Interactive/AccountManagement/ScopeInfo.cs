// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Client.AccountManagement.ScopeInfo
// Assembly: Microsoft.VisualStudio.Services.Client.Interactive, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 00B1FD41-439C-4B93-A417-9D1E4874E657
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Client.Interactive.dll

using Microsoft.VisualStudio.Services.Common;
using Newtonsoft.Json;
using System;
using System.Diagnostics;

namespace Microsoft.VisualStudio.Services.Client.AccountManagement
{
  public sealed class ScopeInfo
  {
    public ScopeInfo(Guid tenantId) => this.TenantId = tenantId;

    public ScopeInfo(string domain)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(domain, nameof (domain));
      this.Domain = domain;
    }

    [JsonConstructor]
    [DebuggerStepThrough]
    public ScopeInfo(Guid tenantId, string domain)
    {
      this.Domain = domain;
      this.TenantId = tenantId;
    }

    public string Domain { get; private set; }

    public Guid TenantId { get; private set; }

    public void UpdateTenantIdForDomain(Guid tenantId) => this.TenantId = tenantId;
  }
}
