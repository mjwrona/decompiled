// Decompiled with JetBrains decompiler
// Type: Azure.Core.Pipeline.DiagnosticScopeFactory
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;


#nullable enable
namespace Azure.Core.Pipeline
{
  internal class DiagnosticScopeFactory
  {
    private static Dictionary<string, DiagnosticListener>? listeners;
    private readonly string? resourceProviderNamespace;
    private readonly DiagnosticListener? source;

    public DiagnosticScopeFactory(
      string clientNamespace,
      string? resourceProviderNamespace,
      bool isActivityEnabled)
    {
      this.resourceProviderNamespace = resourceProviderNamespace;
      this.IsActivityEnabled = isActivityEnabled;
      if (!this.IsActivityEnabled)
        return;
      Dictionary<string, DiagnosticListener> dictionary = LazyInitializer.EnsureInitialized<Dictionary<string, DiagnosticListener>>(ref DiagnosticScopeFactory.listeners);
      lock (dictionary)
      {
        if (dictionary.TryGetValue(clientNamespace, out this.source))
          return;
        this.source = new DiagnosticListener(clientNamespace);
        dictionary[clientNamespace] = this.source;
      }
    }

    public bool IsActivityEnabled { get; }

    public DiagnosticScope CreateScope(string name, DiagnosticScope.ActivityKind kind = DiagnosticScope.ActivityKind.Client)
    {
      if (this.source == null)
        return new DiagnosticScope();
      DiagnosticScope scope = new DiagnosticScope(this.source.Name, name, this.source, kind);
      if (this.resourceProviderNamespace != null)
        scope.AddAttribute("az.namespace", this.resourceProviderNamespace);
      return scope;
    }
  }
}
