// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.ResourceMigrationControllerBase
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Commerce
{
  public class ResourceMigrationControllerBase : TfsApiController
  {
    [HttpPost]
    [TraceFilter(5109115, 5109116)]
    [ClientMethodAccessModifier(ClientMethodAccessModifierAttribute.AccessModifier.Internal)]
    public void MigrateResources([FromBody] ResourceMigrationRequest migrationRequest) => this.TfsRequestContext.GetService<PlatformTargetResourceMigrationService>().MigrateResources(this.TfsRequestContext, migrationRequest, OperationType.Migration);

    [HttpPut]
    [TraceFilter(5109102, 5109103)]
    [ClientMethodAccessModifier(ClientMethodAccessModifierAttribute.AccessModifier.Internal)]
    public void CreateSubscriptionResources([FromBody] ResourceMigrationRequest migrationRequest) => this.TfsRequestContext.GetService<PlatformTargetResourceMigrationService>().MigrateResources(this.TfsRequestContext, migrationRequest, OperationType.Purchase);

    [HttpPut]
    [TraceFilter(5109113, 5109114)]
    [ClientMethodAccessModifier(ClientMethodAccessModifierAttribute.AccessModifier.Internal)]
    public void CreateOrDeleteAzureResourceAccounts(
      bool createResources,
      [FromBody] ResourceMigrationRequest migrationRequest)
    {
      PlatformTargetResourceMigrationService service = this.TfsRequestContext.GetService<PlatformTargetResourceMigrationService>();
      OperationType operationType = createResources ? OperationType.Link : OperationType.Unlink;
      IVssRequestContext tfsRequestContext = this.TfsRequestContext;
      ResourceMigrationRequest migrationRequest1 = migrationRequest;
      int num = (int) operationType;
      service.MigrateResources(tfsRequestContext, migrationRequest1, (OperationType) num);
    }

    [HttpPatch]
    [TraceFilter(5109113, 5109114)]
    [ClientMethodAccessModifier(ClientMethodAccessModifierAttribute.AccessModifier.Internal)]
    public void CreateAzureSubscriptionForDualWrites([FromBody] ResourceMigrationRequest migrationRequest) => this.TfsRequestContext.GetService<PlatformTargetResourceMigrationService>().MigrateResources(this.TfsRequestContext, migrationRequest, OperationType.CreateSubscription);
  }
}
