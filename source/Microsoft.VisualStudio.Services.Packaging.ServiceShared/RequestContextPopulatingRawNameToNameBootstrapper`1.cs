// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.RequestContextPopulatingRawNameToNameBootstrapper`1
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared
{
  public class RequestContextPopulatingRawNameToNameBootstrapper<TPackageName> : 
    IBootstrapper<IConverter<string, TPackageName>>
  {
    private readonly IVssRequestContext requestContext;
    private readonly IConverter<string, TPackageName> actualConverter;

    public RequestContextPopulatingRawNameToNameBootstrapper(
      IVssRequestContext requestContext,
      IConverter<string, TPackageName> actualConverter)
    {
      this.requestContext = requestContext;
      this.actualConverter = actualConverter;
    }

    public IConverter<string, TPackageName> Bootstrap() => (IConverter<string, TPackageName>) new PopulateRequestContextItemDelegatingConverter<string, TPackageName>(this.requestContext, "Packaging.PackageName", this.actualConverter);
  }
}
