// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.ODataJsonSchemaSelectItemTranslator
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

using Microsoft.OData.Edm;
using Microsoft.OData.UriParser;
using Microsoft.VisualStudio.Services.Analytics.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Analytics
{
  public class ODataJsonSchemaSelectItemTranslator : 
    SelectItemTranslator<KeyValuePair<string, ODataJsonSchema>?>
  {
    protected IEdmModel model;
    private PathSegmentTranslator<ODataJsonSchema> pathSegmentTranslator;

    public ODataJsonSchemaSelectItemTranslator(IEdmModel model)
    {
      this.model = model;
      this.pathSegmentTranslator = (PathSegmentTranslator<ODataJsonSchema>) new ODataJsonSchemaPropertySegmentTranslator(model);
    }

    private void ThrowIfMoreThanOnePathSegment(ODataSelectPath path)
    {
      if (path.Count > 1)
        throw new ViewSchemaMultipleSelectPathSegmentsException(string.Join("/", path.Select<ODataPathSegment, string>((Func<ODataPathSegment, string>) (segment => segment.Identifier))));
    }

    public override KeyValuePair<string, ODataJsonSchema>? Translate(
      ExpandedNavigationSelectItem item)
    {
      ODataPathSegment lastSegment = item.PathToNavigationProperty.LastSegment;
      ODataJsonSchema odataJsonSchema = lastSegment.TranslateWith<ODataJsonSchema>(this.pathSegmentTranslator);
      odataJsonSchema.Properties = (IDictionary<string, ODataJsonSchema>) new Dictionary<string, ODataJsonSchema>();
      foreach (SelectItem selectedItem in item.SelectAndExpand.SelectedItems)
      {
        KeyValuePair<string, ODataJsonSchema>? nullable = selectedItem.TranslateWith<KeyValuePair<string, ODataJsonSchema>?>((SelectItemTranslator<KeyValuePair<string, ODataJsonSchema>?>) this);
        if (nullable.HasValue)
          odataJsonSchema.Properties.Add(nullable.Value);
      }
      return new KeyValuePair<string, ODataJsonSchema>?(new KeyValuePair<string, ODataJsonSchema>(lastSegment.Identifier, odataJsonSchema));
    }

    public override KeyValuePair<string, ODataJsonSchema>? Translate(PathSelectItem item)
    {
      this.ThrowIfMoreThanOnePathSegment(item.SelectedPath);
      if (item.SelectedPath.LastSegment is NavigationPropertySegment)
        return new KeyValuePair<string, ODataJsonSchema>?();
      ODataJsonSchema odataJsonSchema = item.SelectedPath.FirstSegment.TranslateWith<ODataJsonSchema>(this.pathSegmentTranslator);
      return odataJsonSchema != null ? new KeyValuePair<string, ODataJsonSchema>?(new KeyValuePair<string, ODataJsonSchema>(item.SelectedPath.FirstSegment.Identifier, odataJsonSchema)) : new KeyValuePair<string, ODataJsonSchema>?();
    }
  }
}
