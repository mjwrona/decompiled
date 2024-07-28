// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Builder.SelfLinkBuilder`1
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.AspNet.OData.Common;
using System;

namespace Microsoft.AspNet.OData.Builder
{
  public class SelfLinkBuilder<T>
  {
    public SelfLinkBuilder(Func<ResourceContext, T> linkFactory, bool followsConventions)
    {
      this.Factory = linkFactory != null ? linkFactory : throw Error.ArgumentNull(nameof (linkFactory));
      this.FollowsConventions = followsConventions;
    }

    public Func<ResourceContext, T> Factory { get; private set; }

    public bool FollowsConventions { get; private set; }
  }
}
