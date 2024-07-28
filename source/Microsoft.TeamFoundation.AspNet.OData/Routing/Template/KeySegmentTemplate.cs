// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Routing.Template.KeySegmentTemplate
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.AspNet.OData.Common;
using Microsoft.AspNet.OData.Routing.Conventions;
using Microsoft.OData;
using Microsoft.OData.UriParser;
using System.Collections.Generic;

namespace Microsoft.AspNet.OData.Routing.Template
{
  public class KeySegmentTemplate : ODataPathSegmentTemplate
  {
    public KeySegmentTemplate(KeySegment segment)
    {
      this.Segment = segment != null ? segment : throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (segment));
      this.ParameterMappings = KeySegmentTemplate.BuildKeyMappings(segment.Keys);
    }

    public KeySegment Segment { get; set; }

    public IDictionary<string, string> ParameterMappings { get; private set; }

    public override bool TryMatch(ODataPathSegment pathSegment, IDictionary<string, object> values) => pathSegment is KeySegment keySegment && keySegment.TryMatch(this.ParameterMappings, values);

    internal static IDictionary<string, string> BuildKeyMappings(
      IEnumerable<KeyValuePair<string, object>> keys)
    {
      Dictionary<string, string> dictionary = new Dictionary<string, string>();
      foreach (KeyValuePair<string, object> key in keys)
      {
        string parameterName = !(key.Value is UriTemplateExpression templateExpression) ? key.Value as string : templateExpression.LiteralText.Trim();
        string str = parameterName != null && RoutingConventionHelpers.IsRouteParameter(parameterName) ? parameterName.Substring(1, parameterName.Length - 2) : throw new ODataException(Microsoft.AspNet.OData.Common.Error.Format(SRResources.KeyTemplateMustBeInCurlyBraces, key.Value, (object) key.Key));
        dictionary[key.Key] = !string.IsNullOrEmpty(str) ? str : throw new ODataException(Microsoft.AspNet.OData.Common.Error.Format(SRResources.EmptyKeyTemplate, key.Value, (object) key.Key));
      }
      return (IDictionary<string, string>) dictionary;
    }
  }
}
