// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.RemoteSettings.ScopeParserFactory
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.RemoteSettings
{
  internal class ScopeParserFactory : IScopeParserFactory
  {
    private readonly IDictionary<string, IScopeFilterProvider> providedFilters = (IDictionary<string, IScopeFilterProvider>) new ConcurrentDictionary<string, IScopeFilterProvider>();
    private readonly IRemoteSettingsLogger logger;

    public ScopeParserFactory(RemoteSettingsInitializer initializer) => this.logger = initializer.RemoteSettingsLogger;

    public IDictionary<string, IScopeFilterProvider> ProvidedFilters => this.providedFilters;

    public bool Evaluate(string scopeExpression)
    {
      try
      {
        return new ScopeParser(scopeExpression, this.ProvidedFilters).Run();
      }
      catch (ScopeParserException ex)
      {
        this.logger.LogError("Scope parsing error", (Exception) ex);
        return false;
      }
    }

    public Task<bool> EvaluateAsync(string scopeExpression)
    {
      try
      {
        return new AsyncScopeParser(scopeExpression, this.ProvidedFilters).RunAsync();
      }
      catch (ScopeParserException ex)
      {
        this.logger.LogError("Scope parsing error", (Exception) ex);
        return Task.FromResult<bool>(false);
      }
    }
  }
}
