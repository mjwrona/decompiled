// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Ingestion.CentralFeedServices.ShouldCallTerrapinSetting
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Facades;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Settings;


#nullable enable
namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Ingestion.CentralFeedServices
{
  public class ShouldCallTerrapinSetting : IOrgLevelPackagingSetting<bool>
  {
    private readonly IOrgLevelPackagingSetting<bool> shouldCallTerrapinFeatureSetting;
    private readonly IExecutionEnvironment executionEnvironment;
    private readonly ITracerService tracerService;

    public ShouldCallTerrapinSetting(
      IOrgLevelPackagingSetting<bool> shouldCallTerrapinFeatureSetting,
      IExecutionEnvironment executionEnvironment,
      ITracerService tracerService)
    {
      this.shouldCallTerrapinFeatureSetting = shouldCallTerrapinFeatureSetting;
      this.executionEnvironment = executionEnvironment;
      this.tracerService = tracerService;
    }

    public bool Get()
    {
      using (ITracerBlock tracerBlock = this.tracerService.Enter((object) this, nameof (Get)))
      {
        if (!this.shouldCallTerrapinFeatureSetting.Get())
        {
          tracerBlock.TraceInfo("'Should call Terrapin' setting is disabled because the feature is disabled");
          return false;
        }
        if ((this.executionEnvironment.IsCollectionInMicrosoftTenant() ? 1 : (this.executionEnvironment.IsDevFabric() ? 1 : 0)) == 0)
        {
          tracerBlock.TraceInfo("'Should call terrapin' setting is disabled because the collection is not in the MSFT tenant");
          return false;
        }
        tracerBlock.TraceInfo("'Should call Terrapin' setting is enabled");
        return true;
      }
    }
  }
}
