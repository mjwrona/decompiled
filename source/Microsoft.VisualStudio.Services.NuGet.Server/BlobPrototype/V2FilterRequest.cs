// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype.V2FilterRequest
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using Microsoft.VisualStudio.Services.NuGet.Server.Controllers.V2;
using System.Collections.Generic;
using System.Web.Http.OData.Query;

namespace Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype
{
  public class V2FilterRequest
  {
    public V2FilterRequest(IEnumerable<ServerV2FeedPackage> packages, ODataQueryOptions options)
    {
      this.Packages = packages;
      this.Options = options;
    }

    public IEnumerable<ServerV2FeedPackage> Packages { get; }

    public ODataQueryOptions Options { get; }
  }
}
