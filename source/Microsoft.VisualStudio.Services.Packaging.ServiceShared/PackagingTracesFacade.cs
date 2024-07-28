// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackagingTracesFacade
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Server;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared
{
  public class PackagingTracesFacade : IPackagingTraces
  {
    private IVssRequestContext requestContext;

    public PackagingTracesFacade(IVssRequestContext requestContext) => this.requestContext = requestContext;

    public void AddProperty(string key, object value) => this.requestContext.AddPackagingTracesProperty(key, value);

    public object GetProperty(string key) => this.requestContext.GetPackagingTracesProperty(key);
  }
}
