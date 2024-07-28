// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Server.Azure.Common.IStorageAccountConfigurationService
// Assembly: Microsoft.VisualStudio.Services.Content.Server.Azure, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7823E4AE-BEB6-4A7C-9914-276DEAE1FB1F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Server.Azure.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.BlobStore.Common;
using Microsoft.VisualStudio.Services.Content.Server.Common;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Content.Server.Azure.Common
{
  [DefaultServiceImplementation(typeof (StorageAccountConfigurationService))]
  public interface IStorageAccountConfigurationService : IVssFrameworkService
  {
    IEnumerable<StrongBoxConnectionString> GetStorageAccounts(
      IVssRequestContext requestContext,
      PhysicalDomainInfo physicalDomainInfo = null);

    Microsoft.Azure.Cosmos.Table.LocationMode? TableLocationMode { get; }

    Microsoft.Azure.Storage.RetryPolicies.LocationMode? BlobLocationMode { get; }
  }
}
