// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Routing.Template.OperationSegmentTemplate
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.AspNet.OData.Routing.Conventions;
using Microsoft.OData.Edm;
using Microsoft.OData.UriParser;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.AspNet.OData.Routing.Template
{
  public class OperationSegmentTemplate : ODataPathSegmentTemplate
  {
    public OperationSegmentTemplate(OperationSegment segment)
    {
      this.Segment = segment != null ? segment : throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (segment));
      IEdmOperation edmOperation = this.Segment.Operations.First<IEdmOperation>();
      if (!edmOperation.IsFunction())
        return;
      this.ParameterMappings = RoutingConventionHelpers.BuildParameterMappings(segment.Parameters, edmOperation.FullName());
    }

    public OperationSegment Segment { get; private set; }

    public IDictionary<string, string> ParameterMappings { get; private set; }

    public override bool TryMatch(ODataPathSegment pathSegment, IDictionary<string, object> values)
    {
      if (!(pathSegment is OperationSegment segment))
        return false;
      IEdmOperation edmOperation1 = this.Segment.Operations.First<IEdmOperation>();
      IEdmOperation edmOperation2 = segment.Operations.First<IEdmOperation>();
      if (edmOperation1.IsAction() && edmOperation2.IsAction())
        return edmOperation1 == edmOperation2;
      if (edmOperation1.IsFunction() && edmOperation2.IsFunction() && !(edmOperation1.FullName() != edmOperation2.FullName()))
      {
        IDictionary<string, object> parameters = (IDictionary<string, object>) new Dictionary<string, object>();
        foreach (OperationSegmentParameter parameter in segment.Parameters)
        {
          object parameterValue = segment.GetParameterValue(parameter.Name);
          parameters[parameter.Name] = parameterValue;
        }
        if (RoutingConventionHelpers.TryMatch(this.ParameterMappings, parameters, values))
        {
          foreach (OperationSegmentParameter parameter in segment.Parameters)
          {
            string name = parameter.Name;
            object paramValue = parameters[name];
            RoutingConventionHelpers.AddFunctionParameters((IEdmFunction) edmOperation2, name, paramValue, values, values, this.ParameterMappings);
          }
          return true;
        }
      }
      return false;
    }
  }
}
