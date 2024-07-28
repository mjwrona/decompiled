// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.OData.Infrastructure.MyValidator
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

using Microsoft.AspNet.OData.Query;
using Microsoft.AspNet.OData.Query.Validators;
using Microsoft.OData.Edm;
using Microsoft.OData.UriParser;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Analytics.OData.Infrastructure
{
  public class MyValidator : FilterQueryValidator
  {
    private ISet<string> _filterTypes;

    public bool NavigatesToTag { get; private set; }

    public MyValidator(DefaultQuerySettings defaultQuerySettings, ISet<string> filterTypes)
      : base(defaultQuerySettings)
    {
      this._filterTypes = filterTypes;
    }

    public override void ValidateNavigationPropertyNode(
      QueryNode sourceNode,
      IEdmNavigationProperty navigationProperty,
      ODataValidationSettings settings)
    {
      if (this._filterTypes.Contains(navigationProperty.Type.Definition.AsElementType().FullTypeName()))
        this.NavigatesToTag = true;
      base.ValidateNavigationPropertyNode(sourceNode, navigationProperty, settings);
    }
  }
}
