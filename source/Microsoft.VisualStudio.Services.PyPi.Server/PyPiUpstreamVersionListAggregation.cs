// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.PyPi.Server.PyPiUpstreamVersionListAggregation
// Assembly: Microsoft.VisualStudio.Services.PyPi.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AC58CC2C-9A83-4CAE-B2C4-C90763B36046
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.PyPi.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.UpstreamVersionList;
using Microsoft.VisualStudio.Services.PyPi.Server.PackageIdentity;
using System;

namespace Microsoft.VisualStudio.Services.PyPi.Server
{
  public class PyPiUpstreamVersionListAggregation : 
    UpstreamVersionListAggregation<PyPiPackageName, PyPiPackageVersion>
  {
    public static readonly AggregationDefinition StaticDefinition = UpstreamVersionListAggregation<PyPiPackageName, PyPiPackageVersion>.MakeDefinition((IProtocol) Protocol.PyPi);
    public static readonly PyPiUpstreamVersionListAggregation V1 = new PyPiUpstreamVersionListAggregation();

    public override AggregationDefinition Definition => PyPiUpstreamVersionListAggregation.StaticDefinition;

    public override IAggregationAccessor Bootstrap(IVssRequestContext requestContext) => this.BootstrapCore(requestContext, ByFuncConverter.Create<string, PyPiPackageVersion>((Func<string, PyPiPackageVersion>) (s => PyPiPackageVersionParser.Parse(s))));
  }
}
