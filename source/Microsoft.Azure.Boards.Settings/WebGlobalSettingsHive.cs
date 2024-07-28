// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Boards.Settings.WebGlobalSettingsHive
// Assembly: Microsoft.Azure.Boards.Settings, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2AC3574E-9414-4605-BAB7-1F6B28A75804
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.Boards.Settings.dll

using Microsoft.TeamFoundation.Framework.Server;

namespace Microsoft.Azure.Boards.Settings
{
  public class WebGlobalSettingsHive : RootSettingsHive
  {
    private CachedRegistryService m_tfsRegistry;

    public WebGlobalSettingsHive(IVssRequestContext requestContext)
      : this(requestContext, (string) null)
    {
    }

    public WebGlobalSettingsHive(IVssRequestContext requestContext, string cachePattern)
      : base(requestContext, cachePattern)
    {
      this.m_tfsRegistry = requestContext.GetService<CachedRegistryService>();
    }

    protected override string Prefix => "/WebAccess/GlobalSettings";
  }
}
