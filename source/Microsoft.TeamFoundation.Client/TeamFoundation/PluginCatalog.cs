// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.PluginCatalog
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

namespace Microsoft.TeamFoundation
{
  public abstract class PluginCatalog
  {
    protected bool includeDisabled;

    public abstract string[] CategoryNames { get; }

    public bool IncludeDisabledPlugins
    {
      get => this.includeDisabled;
      set => this.includeDisabled = value;
    }

    public abstract PluginCategory this[string categoryName] { get; }

    public abstract void Refresh();
  }
}
