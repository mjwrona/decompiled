// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Builder.EntitySetConfiguration`1
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.AspNet.OData.Common;
using System;

namespace Microsoft.AspNet.OData.Builder
{
  public class EntitySetConfiguration<TEntityType> : NavigationSourceConfiguration<TEntityType> where TEntityType : class
  {
    internal EntitySetConfiguration(ODataModelBuilder modelBuilder, string name)
      : base(modelBuilder, (NavigationSourceConfiguration) new EntitySetConfiguration(modelBuilder, typeof (TEntityType), name))
    {
    }

    internal EntitySetConfiguration(
      ODataModelBuilder modelBuilder,
      EntitySetConfiguration configuration)
      : base(modelBuilder, (NavigationSourceConfiguration) configuration)
    {
    }

    internal EntitySetConfiguration EntitySet => (EntitySetConfiguration) this.Configuration;

    public virtual void HasFeedSelfLink(
      Func<ResourceSetContext, string> feedSelfLinkFactory)
    {
      if (feedSelfLinkFactory == null)
        throw Error.ArgumentNull(nameof (feedSelfLinkFactory));
      this.EntitySet.HasFeedSelfLink((Func<ResourceSetContext, Uri>) (feedContext => new Uri(feedSelfLinkFactory(feedContext))));
    }

    public virtual void HasFeedSelfLink(Func<ResourceSetContext, Uri> feedSelfLinkFactory)
    {
      if (feedSelfLinkFactory == null)
        throw Error.ArgumentNull(nameof (feedSelfLinkFactory));
      this.EntitySet.HasFeedSelfLink(feedSelfLinkFactory);
    }
  }
}
