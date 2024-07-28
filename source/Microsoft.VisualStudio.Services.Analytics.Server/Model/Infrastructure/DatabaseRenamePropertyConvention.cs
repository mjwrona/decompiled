// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.Model.Infrastructure.DatabaseRenamePropertyConvention
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

using Microsoft.VisualStudio.Services.Analytics.OData.Infrastructure;
using System.Data.Entity.ModelConfiguration.Configuration;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Reflection;

namespace Microsoft.VisualStudio.Services.Analytics.Model.Infrastructure
{
  internal class DatabaseRenamePropertyConvention : 
    PropertyAttributeConfigurationConvention<DatabaseRenameAttribute>
  {
    public DatabaseRenamePropertyConvention(int dbServiceVersion) => this.DbServiceVersion = dbServiceVersion;

    private int DbServiceVersion { get; }

    public override void Apply(
      PropertyInfo propertyInfo,
      ConventionTypeConfiguration configuration,
      DatabaseRenameAttribute attribute)
    {
      if (!attribute.ShouldApply(this.DbServiceVersion))
        return;
      configuration.Property(propertyInfo).HasColumnName(attribute.Name);
    }
  }
}
