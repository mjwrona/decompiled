// Decompiled with JetBrains decompiler
// Type: Nest.IndicesModuleSettingsDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class IndicesModuleSettingsDescriptor : 
    DescriptorBase<IndicesModuleSettingsDescriptor, IIndicesModuleSettings>,
    IIndicesModuleSettings
  {
    ICircuitBreakerSettings IIndicesModuleSettings.CircuitBreakerSettings { get; set; }

    FielddataSettings IIndicesModuleSettings.FielddataSettings { get; set; }

    string IIndicesModuleSettings.QeueriesCacheSize { get; set; }

    IIndicesRecoverySettings IIndicesModuleSettings.RecoverySettings { get; set; }

    public IndicesModuleSettingsDescriptor CircuitBreaker(
      Func<CircuitBreakerSettingsDescriptor, ICircuitBreakerSettings> selector)
    {
      return this.Assign<Func<CircuitBreakerSettingsDescriptor, ICircuitBreakerSettings>>(selector, (Action<IIndicesModuleSettings, Func<CircuitBreakerSettingsDescriptor, ICircuitBreakerSettings>>) ((a, v) => a.CircuitBreakerSettings = v != null ? v(new CircuitBreakerSettingsDescriptor()) : (ICircuitBreakerSettings) null));
    }

    public IndicesModuleSettingsDescriptor IndicesRecovery(
      Func<IndicesRecoverySettingsDescriptor, IIndicesRecoverySettings> selector)
    {
      return this.Assign<Func<IndicesRecoverySettingsDescriptor, IIndicesRecoverySettings>>(selector, (Action<IIndicesModuleSettings, Func<IndicesRecoverySettingsDescriptor, IIndicesRecoverySettings>>) ((a, v) => a.RecoverySettings = v != null ? v(new IndicesRecoverySettingsDescriptor()) : (IIndicesRecoverySettings) null));
    }
  }
}
