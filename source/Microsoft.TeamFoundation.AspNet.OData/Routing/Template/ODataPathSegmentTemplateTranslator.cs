// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Routing.Template.ODataPathSegmentTemplateTranslator
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.AspNet.OData.Common;
using Microsoft.OData;
using Microsoft.OData.UriParser;

namespace Microsoft.AspNet.OData.Routing.Template
{
  public class ODataPathSegmentTemplateTranslator : PathSegmentTranslator<ODataPathSegmentTemplate>
  {
    public override ODataPathSegmentTemplate Translate(TypeSegment segment) => (ODataPathSegmentTemplate) new TypeSegmentTemplate(segment);

    public override ODataPathSegmentTemplate Translate(NavigationPropertySegment segment) => (ODataPathSegmentTemplate) new NavigationPropertySegmentTemplate(segment);

    public override ODataPathSegmentTemplate Translate(EntitySetSegment segment) => (ODataPathSegmentTemplate) new EntitySetSegmentTemplate(segment);

    public override ODataPathSegmentTemplate Translate(SingletonSegment segment) => (ODataPathSegmentTemplate) new SingletonSegmentTemplate(segment);

    public override ODataPathSegmentTemplate Translate(KeySegment segment) => (ODataPathSegmentTemplate) new KeySegmentTemplate(segment);

    public override ODataPathSegmentTemplate Translate(PropertySegment segment) => (ODataPathSegmentTemplate) new PropertySegmentTemplate(segment);

    public override ODataPathSegmentTemplate Translate(PathTemplateSegment segment) => (ODataPathSegmentTemplate) new PathTemplateSegmentTemplate(segment);

    public override ODataPathSegmentTemplate Translate(OperationImportSegment segment) => (ODataPathSegmentTemplate) new OperationImportSegmentTemplate(segment);

    public override ODataPathSegmentTemplate Translate(OperationSegment segment) => (ODataPathSegmentTemplate) new OperationSegmentTemplate(segment);

    public override ODataPathSegmentTemplate Translate(DynamicPathSegment segment) => (ODataPathSegmentTemplate) new DynamicSegmentTemplate(segment);

    public override ODataPathSegmentTemplate Translate(NavigationPropertyLinkSegment segment) => (ODataPathSegmentTemplate) new NavigationPropertyLinkSegmentTemplate(segment);

    public override ODataPathSegmentTemplate Translate(CountSegment segment) => (ODataPathSegmentTemplate) new ODataPathSegmentTemplate<CountSegment>();

    public override ODataPathSegmentTemplate Translate(ValueSegment segment) => (ODataPathSegmentTemplate) new ODataPathSegmentTemplate<ValueSegment>();

    public override ODataPathSegmentTemplate Translate(BatchSegment segment) => (ODataPathSegmentTemplate) new ODataPathSegmentTemplate<BatchSegment>();

    public override ODataPathSegmentTemplate Translate(MetadataSegment segment) => (ODataPathSegmentTemplate) new ODataPathSegmentTemplate<MetadataSegment>();

    public override ODataPathSegmentTemplate Translate(BatchReferenceSegment segment) => throw new ODataException(Microsoft.AspNet.OData.Common.Error.Format(SRResources.TargetKindNotImplemented, (object) "ODataPathSegment", (object) "BatchReferenceSegment"));
  }
}
