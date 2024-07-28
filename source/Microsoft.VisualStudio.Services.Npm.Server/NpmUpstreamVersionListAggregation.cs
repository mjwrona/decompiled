// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Npm.Server.NpmUpstreamVersionListAggregation
// Assembly: Microsoft.VisualStudio.Services.Npm.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2F4F0262-1C1B-42F0-BCA7-1385424A0D51
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Npm.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.UpstreamVersionList;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Versioning;
using System;

namespace Microsoft.VisualStudio.Services.Npm.Server
{
  public class NpmUpstreamVersionListAggregation : 
    UpstreamVersionListAggregation<NpmPackageName, SemanticVersion>
  {
    public static readonly AggregationDefinition StaticDefinition = UpstreamVersionListAggregation<NpmPackageName, SemanticVersion>.MakeDefinition((IProtocol) Protocol.npm);
    public static readonly NpmUpstreamVersionListAggregation V1 = new NpmUpstreamVersionListAggregation();

    public override AggregationDefinition Definition => NpmUpstreamVersionListAggregation.StaticDefinition;

    public override IAggregationAccessor Bootstrap(IVssRequestContext requestContext) => this.BootstrapCore(requestContext, ByFuncConverter.Create<string, SemanticVersion>((Func<string, SemanticVersion>) (s => SemanticVersion.Parse(s))));
  }
}
