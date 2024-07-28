// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.RemoteSettings.FlightScopeFilterProvider
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using Microsoft.VisualStudio.Experimentation;
using Microsoft.VisualStudio.Utilities.Internal;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.RemoteSettings
{
  internal sealed class FlightScopeFilterProvider : 
    IMultiValueScopeFilterAsyncProvider<BoolScopeValue>,
    IMultiValueScopeFilterProvider<BoolScopeValue>,
    IScopeFilterProvider
  {
    private readonly IExperimentationService experimentationService;

    public FlightScopeFilterProvider(IExperimentationService experimentationService)
    {
      experimentationService.RequiresArgumentNotNull<IExperimentationService>(nameof (experimentationService));
      this.experimentationService = experimentationService;
    }

    public string Name => "Flight";

    public BoolScopeValue Provide(string key) => new BoolScopeValue(this.experimentationService.IsCachedFlightEnabled(key));

    public async Task<BoolScopeValue> ProvideAsync(string key) => new BoolScopeValue(await this.experimentationService.IsFlightEnabledAsync(key, CancellationToken.None));
  }
}
