// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.InstanceManagement.Server.InstanceManagementResourceIds
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;

namespace Microsoft.VisualStudio.Services.InstanceManagement.Server
{
  public static class InstanceManagementResourceIds
  {
    public const string AreaId = "{F9A59873-859A-43F6-8329-967916B14736}";
    public const string AreaName = "InstanceManagement";
    public static readonly Guid HostInstanceMappingLocationId = Guid.Parse("{F094ABE5-BCAD-42F1-A7A4-80342713D924}");
    public const string HostInstanceMappingResource = "HostInstanceMappings";
    public static readonly Guid ServiceDomainsLocationId = Guid.Parse("{2B9F17C1-E7CF-4F6A-8352-1560BFF8F1CF}");
    public const string ServiceDomainsResource = "ServiceDomains";
  }
}
