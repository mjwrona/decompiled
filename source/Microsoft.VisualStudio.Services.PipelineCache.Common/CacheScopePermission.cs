// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.PipelineCache.Common.CacheScopePermission
// Assembly: Microsoft.VisualStudio.Services.PipelineCache.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E063C74A-FCE9-47BF-84C0-7143B7075032
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.PipelineCache.Common.dll

namespace Microsoft.VisualStudio.Services.PipelineCache.Common
{
  public class CacheScopePermission
  {
    public CacheScopePermission(string scope, SecurityDefintions.Permissions permission)
    {
      this.Scope = scope;
      this.Permission = permission;
    }

    public string Scope { get; private set; }

    public SecurityDefintions.Permissions Permission { get; private set; }
  }
}
