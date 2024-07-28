// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.Resolution.MigrationFromRequestContextFactory
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.Resolution
{
  public class MigrationFromRequestContextFactory : IFactory<IFeedRequest, Task<string>>
  {
    private readonly IVssRequestContext requestContext;
    private readonly IRegistryService registryService;

    public MigrationFromRequestContextFactory(
      IVssRequestContext requestContext,
      IRegistryService registryService)
    {
      this.requestContext = requestContext;
      this.registryService = registryService;
    }

    public Task<string> Get(IFeedRequest request) => this.registryService.GetValue<bool>(new RegistryQuery(CodeOnlyDeploymentsConstants.ReadMigrationHeaderQuery), false) && this.requestContext.Items.ContainsKey(CodeOnlyDeploymentsConstants.ReadMigrationHeaderKey) ? Task.FromResult<string>(this.requestContext.Items[CodeOnlyDeploymentsConstants.ReadMigrationHeaderKey] as string) : (Task<string>) null;
  }
}
