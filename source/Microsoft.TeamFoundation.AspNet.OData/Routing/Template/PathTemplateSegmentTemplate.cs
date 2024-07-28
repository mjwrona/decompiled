// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Routing.Template.PathTemplateSegmentTemplate
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.AspNet.OData.Common;
using Microsoft.AspNet.OData.Formatter;
using Microsoft.AspNet.OData.Routing.Conventions;
using Microsoft.OData.Edm;
using Microsoft.OData.UriParser;
using System.Collections.Generic;

namespace Microsoft.AspNet.OData.Routing.Template
{
  public class PathTemplateSegmentTemplate : ODataPathSegmentTemplate
  {
    public PathTemplateSegmentTemplate(PathTemplateSegment segment)
    {
      this.TemplateSegment = segment != null ? segment : throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (segment));
      string str;
      this.SegmentName = segment.TranslatePathTemplateSegment(out str);
      this.PropertyName = str;
      this.TreatPropertyNameAsParameterName = false;
      if (!RoutingConventionHelpers.IsRouteParameter(this.PropertyName))
        return;
      this.PropertyName = this.PropertyName.Substring(1, this.PropertyName.Length - 2);
      this.TreatPropertyNameAsParameterName = true;
      if (!string.IsNullOrEmpty(this.PropertyName))
        return;
      Microsoft.AspNet.OData.Common.Error.Format(SRResources.EmptyParameterAlias, (object) this.PropertyName, (object) segment.LiteralText);
    }

    public string PropertyName { get; private set; }

    public string SegmentName { get; private set; }

    private bool TreatPropertyNameAsParameterName { get; set; }

    public PathTemplateSegment TemplateSegment { get; private set; }

    public override bool TryMatch(ODataPathSegment pathSegment, IDictionary<string, object> values)
    {
      if (!(pathSegment is DynamicPathSegment dynamicPathSegment))
        return false;
      if (this.TreatPropertyNameAsParameterName)
      {
        values[this.PropertyName] = (object) dynamicPathSegment.Identifier;
        values["DF908045-6922-46A0-82F2-2F6E7F43D1B1_" + this.PropertyName] = (object) new ODataParameterValue((object) dynamicPathSegment.Identifier, (IEdmTypeReference) EdmLibHelpers.GetEdmPrimitiveTypeReferenceOrNull(typeof (string)));
        return true;
      }
      return this.PropertyName == dynamicPathSegment.Identifier;
    }
  }
}
