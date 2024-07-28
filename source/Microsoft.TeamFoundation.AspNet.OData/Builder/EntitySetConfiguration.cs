// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Builder.EntitySetConfiguration
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using System;

namespace Microsoft.AspNet.OData.Builder
{
  public class EntitySetConfiguration : NavigationSourceConfiguration
  {
    private Func<ResourceSetContext, Uri> _feedSelfLinkFactory;

    public EntitySetConfiguration()
    {
    }

    public EntitySetConfiguration(ODataModelBuilder modelBuilder, Type entityClrType, string name)
      : base(modelBuilder, entityClrType, name)
    {
    }

    public EntitySetConfiguration(
      ODataModelBuilder modelBuilder,
      EntityTypeConfiguration entityType,
      string name)
      : base(modelBuilder, entityType, name)
    {
    }

    public virtual NavigationSourceConfiguration HasFeedSelfLink(
      Func<ResourceSetContext, Uri> feedSelfLinkFactory)
    {
      this._feedSelfLinkFactory = feedSelfLinkFactory;
      return (NavigationSourceConfiguration) this;
    }

    public virtual Func<ResourceSetContext, Uri> GetFeedSelfLink() => this._feedSelfLinkFactory;
  }
}
