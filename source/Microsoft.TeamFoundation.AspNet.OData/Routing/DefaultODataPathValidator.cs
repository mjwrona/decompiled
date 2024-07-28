// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Routing.DefaultODataPathValidator
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.AspNet.OData.Common;
using Microsoft.AspNet.OData.Formatter;
using Microsoft.OData;
using Microsoft.OData.Edm;
using Microsoft.OData.UriParser;

namespace Microsoft.AspNet.OData.Routing
{
  public class DefaultODataPathValidator : PathSegmentHandler
  {
    private readonly IEdmModel _edmModel;

    public DefaultODataPathValidator(IEdmModel model) => this._edmModel = model != null ? model : throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (model));

    public override void Handle(EntitySetSegment segment)
    {
    }

    public override void Handle(KeySegment segment)
    {
    }

    public override void Handle(NavigationPropertyLinkSegment segment)
    {
      if (EdmLibHelpers.IsNotNavigable((IEdmProperty) segment.NavigationProperty, this._edmModel))
        throw new ODataException(Microsoft.AspNet.OData.Common.Error.Format(SRResources.NotNavigablePropertyUsedInNavigation, (object) segment.NavigationProperty.Name));
    }

    public override void Handle(NavigationPropertySegment segment)
    {
      if (EdmLibHelpers.IsNotNavigable((IEdmProperty) segment.NavigationProperty, this._edmModel))
        throw new ODataException(Microsoft.AspNet.OData.Common.Error.Format(SRResources.NotNavigablePropertyUsedInNavigation, (object) segment.NavigationProperty.Name));
    }

    public override void Handle(DynamicPathSegment segment)
    {
    }

    public override void Handle(OperationImportSegment segment)
    {
    }

    public override void Handle(OperationSegment segment)
    {
    }

    public override void Handle(PathTemplateSegment segment)
    {
      switch (segment.TranslatePathTemplateSegment(out string _))
      {
        case "dynamicproperty":
          break;
        default:
          throw new ODataException(Microsoft.AspNet.OData.Common.Error.Format(SRResources.InvalidAttributeRoutingTemplateSegment, (object) segment.LiteralText));
      }
    }

    public override void Handle(PropertySegment segment)
    {
    }

    public override void Handle(SingletonSegment segment)
    {
    }

    public override void Handle(TypeSegment segment)
    {
    }

    public override void Handle(ValueSegment segment)
    {
    }

    public override void Handle(CountSegment segment)
    {
    }

    public override void Handle(BatchSegment segment)
    {
    }

    public override void Handle(MetadataSegment segment)
    {
    }

    public virtual void Handle(UnresolvedPathSegment segment)
    {
    }
  }
}
