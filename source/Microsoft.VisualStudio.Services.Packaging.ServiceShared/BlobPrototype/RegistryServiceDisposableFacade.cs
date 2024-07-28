// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.RegistryServiceDisposableFacade
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype
{
  public class RegistryServiceDisposableFacade : 
    IDisposableRegistryWriterService,
    IDisposable,
    IRegistryWriterService,
    IRegistryService
  {
    private readonly IVssRequestContext requestContext;
    private readonly IVssRegistryService registryService;

    public RegistryServiceDisposableFacade(
      IVssRequestContext requestContext,
      IVssRegistryService registryService)
    {
      this.requestContext = requestContext;
      this.registryService = registryService;
    }

    public IEnumerable<RegistryItem> Read(RegistryQuery registryQuery) => this.registryService.Read(this.requestContext, in registryQuery);

    public T GetValue<T>(RegistryQuery registryQuery, T defaultValue, bool fallThru) => this.registryService.GetValue<T>(this.requestContext, in registryQuery, fallThru, defaultValue);

    public void Write(IEnumerable<RegistryItem> items) => this.registryService.Write(this.requestContext, items);

    public void SetValue<T>(string path, T value) => this.registryService.SetValue<T>(this.requestContext, path, value);

    public void Dispose() => this.requestContext.Dispose();
  }
}
