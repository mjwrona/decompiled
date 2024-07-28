// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Maven.Server.MavenUpstreamVersionListAggregation
// Assembly: Microsoft.VisualStudio.Services.Maven.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AEBE02E-FDD2-41D8-89F7-5C54445DBFA7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Maven.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Maven.Server.Models;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.UpstreamVersionList;
using System;

namespace Microsoft.VisualStudio.Services.Maven.Server
{
  public class MavenUpstreamVersionListAggregation : 
    UpstreamVersionListAggregation<MavenPackageName, MavenPackageVersion>
  {
    public static readonly AggregationDefinition StaticDefinition = UpstreamVersionListAggregation<MavenPackageName, MavenPackageVersion>.MakeDefinition((IProtocol) Protocol.Maven);
    public static readonly MavenUpstreamVersionListAggregation V1 = new MavenUpstreamVersionListAggregation();

    public override AggregationDefinition Definition => MavenUpstreamVersionListAggregation.StaticDefinition;

    public override IAggregationAccessor Bootstrap(IVssRequestContext requestContext) => this.BootstrapCore(requestContext, ByFuncConverter.Create<string, MavenPackageVersion>((Func<string, MavenPackageVersion>) (s => new MavenPackageVersion(s))));
  }
}
