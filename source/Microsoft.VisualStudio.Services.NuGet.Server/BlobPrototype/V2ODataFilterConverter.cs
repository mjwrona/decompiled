// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype.V2ODataFilterConverter
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using Microsoft.VisualStudio.Services.NuGet.Server.Controllers.V2;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http.OData.Query;

namespace Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype
{
  public class V2ODataFilterConverter : 
    IConverter<V2FilterRequest, IEnumerable<ServerV2FeedPackage>>,
    IHaveInputType<V2FilterRequest>,
    IHaveOutputType<IEnumerable<ServerV2FeedPackage>>
  {
    private bool checkSkip;

    public V2ODataFilterConverter(bool checkSkip = false) => this.checkSkip = checkSkip;

    public IEnumerable<ServerV2FeedPackage> Convert(V2FilterRequest input)
    {
      IEnumerable<ServerV2FeedPackage> source = input.Packages;
      ODataQueryOptions options = input.Options;
      if (options == null)
        return source;
      if (options.Filter != null)
        source = (IEnumerable<ServerV2FeedPackage>) Queryable.Cast<ServerV2FeedPackage>(options.Filter.ApplyTo((IQueryable) source.AsQueryable<ServerV2FeedPackage>(), new ODataQuerySettings()
        {
          EnsureStableOrdering = true
        }));
      if (this.checkSkip && options.Skip != null)
        source = source.Skip<ServerV2FeedPackage>(options.Skip.Value);
      return (IEnumerable<ServerV2FeedPackage>) source.ToList<ServerV2FeedPackage>();
    }
  }
}
