// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Builder.NavigationLinkBuilder
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.OData.Edm;
using System;

namespace Microsoft.AspNet.OData.Builder
{
  public class NavigationLinkBuilder
  {
    public NavigationLinkBuilder(
      Func<ResourceContext, IEdmNavigationProperty, Uri> navigationLinkFactory,
      bool followsConventions)
    {
      this.Factory = navigationLinkFactory != null ? navigationLinkFactory : throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (navigationLinkFactory));
      this.FollowsConventions = followsConventions;
    }

    public Func<ResourceContext, IEdmNavigationProperty, Uri> Factory { get; private set; }

    public bool FollowsConventions { get; private set; }
  }
}
