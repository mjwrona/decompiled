// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.HostInfoServiceFacade
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype
{
  public class HostInfoServiceFacade : IHostInfoService
  {
    private readonly IVssRequestContext requestContext;
    private readonly TeamFoundationHostManagementService hostManagementService;

    public HostInfoServiceFacade(
      IVssRequestContext requestContext,
      TeamFoundationHostManagementService hostManagementService)
    {
      this.requestContext = requestContext;
      this.hostManagementService = hostManagementService;
    }

    public IEnumerable<HostProperties> GetActiveCollectionHosts() => this.hostManagementService.QueryServiceHostProperties(this.requestContext, new DateTime?(), new DateTime?()).Where<HostProperties>((Func<HostProperties, bool>) (p => p.HostType == TeamFoundationHostType.ProjectCollection && p.Status == TeamFoundationServiceHostStatus.Started));
  }
}
