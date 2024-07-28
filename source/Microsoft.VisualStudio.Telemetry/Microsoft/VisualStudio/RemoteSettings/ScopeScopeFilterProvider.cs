// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.RemoteSettings.ScopeScopeFilterProvider
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using Microsoft.VisualStudio.Utilities.Internal;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.RemoteSettings
{
  internal sealed class ScopeScopeFilterProvider : 
    IMultiValueScopeFilterAsyncProvider<BoolScopeValue>,
    IMultiValueScopeFilterProvider<BoolScopeValue>,
    IScopeFilterProvider
  {
    private readonly IScopesStorageHandler storage;
    private readonly IScopeParserFactory scopeParserFactory;

    public string Name => "Scope";

    public ScopeScopeFilterProvider(IScopesStorageHandler storage, IScopeParserFactory factory)
    {
      storage.RequiresArgumentNotNull<IScopesStorageHandler>(nameof (storage));
      factory.RequiresArgumentNotNull<IScopeParserFactory>(nameof (factory));
      this.storage = storage;
      this.scopeParserFactory = factory;
    }

    public BoolScopeValue Provide(string key) => new BoolScopeValue(this.scopeParserFactory.Evaluate(this.storage.GetScope(key) ?? throw new ScopeParserException("Could not find scope with name: " + key)));

    public async Task<BoolScopeValue> ProvideAsync(string key) => new BoolScopeValue(await this.scopeParserFactory.EvaluateAsync(this.storage.GetScope(key) ?? throw new ScopeParserException("Could not find scope with name: " + key)).ConfigureAwait(false));
  }
}
