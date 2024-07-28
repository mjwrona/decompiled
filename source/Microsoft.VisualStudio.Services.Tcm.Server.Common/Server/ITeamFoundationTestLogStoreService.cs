// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.ITeamFoundationTestLogStoreService
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Cloud.AzureBlobGeoRedundancy;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [CLSCompliant(false)]
  [DefaultServiceImplementation(typeof (TeamFoundationTestLogStoreService))]
  public interface ITeamFoundationTestLogStoreService : IVssFrameworkService
  {
    ITestLogStorageConnection GetTestLogStorageConnection(
      IVssRequestContext requestContext,
      Guid projectId,
      ContainerScopeDetails scopeDetails,
      bool createIfNotExist = true);

    ITestLogStorageConnection GetOmegaTestLogStoreConnection(
      IVssRequestContext requestContext,
      Guid projectId,
      ContainerScopeDetails scopeDetails);

    IList<string> GetAllStorageAccountsName(IVssRequestContext requestContext);

    IList<ITestLogStorageConnection> GetTestLogStorageConnections(IVssRequestContext requestContext);

    IList<ITestLogStorageConnection> GetOmegaTestLogStorageConnections(
      IVssRequestContext requestContext);

    bool EnableCorsForStorageAccounts(IVssRequestContext requestContext, string corsDomainName);

    bool DisableCorsForStorageAccounts(IVssRequestContext requestContext);

    Dictionary<string, IList<string>> GetCorsAllowedHostListForStorageAccounts(
      IVssRequestContext requestContext);

    List<GeoRedundantStorageAccountSettings> GetGeoRedundantStorageAccountSettings(
      IVssRequestContext requestContext);
  }
}
