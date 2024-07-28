// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.Model.Infrastructure.DatabaseHideNavigationPropertyConvention
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

using Microsoft.VisualStudio.Services.Analytics.OData.Infrastructure;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration.Configuration;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Reflection;

namespace Microsoft.VisualStudio.Services.Analytics.Model.Infrastructure
{
  internal class DatabaseHideNavigationPropertyConvention : 
    ForeignKeyPrimitivePropertyAttributeConvention
  {
    public DatabaseHideNavigationPropertyConvention(int dbServiceVersion) => this.DbServiceVersion = dbServiceVersion;

    private int DbServiceVersion { get; }

    public override void Apply(
      PropertyInfo propertyInfo,
      ConventionTypeConfiguration configuration,
      ForeignKeyAttribute attribute)
    {
      DatabaseHideAttribute customAttribute = propertyInfo.GetCustomAttribute<DatabaseHideAttribute>();
      if (customAttribute == null || !customAttribute.ShouldApply(this.DbServiceVersion))
        return;
      configuration.Ignore(propertyInfo);
    }
  }
}
