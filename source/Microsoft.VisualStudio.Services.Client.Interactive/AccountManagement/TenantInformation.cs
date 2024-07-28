// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Client.AccountManagement.TenantInformation
// Assembly: Microsoft.VisualStudio.Services.Client.Interactive, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 00B1FD41-439C-4B93-A417-9D1E4874E657
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Client.Interactive.dll

using Microsoft.VisualStudio.Services.Common;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Client.AccountManagement
{
  public sealed class TenantInformation
  {
    [JsonConstructor]
    public TenantInformation(
      HashSet<string> uniqueIds,
      string tenantId,
      string friendlyName,
      bool isOwned)
    {
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) uniqueIds, nameof (uniqueIds));
      ArgumentUtility.CheckStringForNullOrEmpty(tenantId, nameof (tenantId));
      this.UniqueIds = uniqueIds.ToArray<string>();
      this.TenantId = tenantId;
      this.IsOwned = isOwned;
      this.FriendlyName = friendlyName;
    }

    public string[] UniqueIds { get; private set; }

    public string TenantId { get; private set; }

    public string FriendlyName { get; private set; }

    public bool IsOwned { get; private set; }
  }
}
