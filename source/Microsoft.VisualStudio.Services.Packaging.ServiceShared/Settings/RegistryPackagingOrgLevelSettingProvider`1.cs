// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Settings.RegistryPackagingOrgLevelSettingProvider`1
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Settings
{
  public class RegistryPackagingOrgLevelSettingProvider<TValue> : IOrgLevelPackagingSetting<TValue>
  {
    private readonly OrgLevelRegistrySettingDefinition<TValue> definition;
    private readonly IRegistryService registryService;

    public RegistryPackagingOrgLevelSettingProvider(
      OrgLevelRegistrySettingDefinition<TValue> definition,
      IRegistryService registryService)
    {
      this.definition = definition;
      this.registryService = registryService;
    }

    public TValue Get() => this.registryService.GetValue<TValue>(this.definition.RegistryQuery, this.definition.DefaultValue);
  }
}
