// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.Model.Infrastructure.AnalyticsModelContainerConvention
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace Microsoft.VisualStudio.Services.Analytics.Model.Infrastructure
{
  internal class AnalyticsModelContainerConvention : 
    IConceptualModelConvention<EntityContainer>,
    IConvention
  {
    private string _name;

    public AnalyticsModelContainerConvention(string name) => this._name = name;

    public void Apply(EntityContainer item, DbModel model) => item.Name = this._name;
  }
}
