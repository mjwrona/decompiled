// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Builder.OperationLinkBuilder
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.AspNet.OData.Common;
using System;

namespace Microsoft.AspNet.OData.Builder
{
  public class OperationLinkBuilder
  {
    private Func<ResourceContext, Uri> _linkFactory;
    private readonly Func<ResourceSetContext, Uri> _feedLinkFactory;

    public OperationLinkBuilder(Func<ResourceContext, Uri> linkFactory, bool followsConventions)
    {
      this._linkFactory = linkFactory != null ? linkFactory : throw Error.ArgumentNull(nameof (linkFactory));
      this.FollowsConventions = followsConventions;
    }

    public OperationLinkBuilder(Func<ResourceSetContext, Uri> linkFactory, bool followsConventions)
    {
      this._feedLinkFactory = linkFactory != null ? linkFactory : throw Error.ArgumentNull(nameof (linkFactory));
      this.FollowsConventions = followsConventions;
    }

    internal Func<ResourceContext, Uri> LinkFactory => this._linkFactory;

    internal Func<ResourceSetContext, Uri> FeedLinkFactory => this._feedLinkFactory;

    public bool FollowsConventions { get; private set; }

    public virtual Uri BuildLink(ResourceContext context) => this._linkFactory == null ? (Uri) null : this._linkFactory(context);

    public virtual Uri BuildLink(ResourceSetContext context) => this._feedLinkFactory == null ? (Uri) null : this._feedLinkFactory(context);
  }
}
