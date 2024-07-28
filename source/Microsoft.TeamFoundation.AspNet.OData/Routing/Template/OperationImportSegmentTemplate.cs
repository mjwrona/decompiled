// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Routing.Template.OperationImportSegmentTemplate
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
  public class OperationImportSegmentTemplate : ODataPathSegmentTemplate
  {
    public OperationImportSegmentTemplate(OperationImportSegment segment)
    {
      this.Segment = segment != null ? segment : throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (segment));
      IEdmOperationImport operationImport = this.Segment.OperationImports.First<IEdmOperationImport>();
      if (!operationImport.IsFunctionImport())
        return;
      this.ParameterMappings = RoutingConventionHelpers.BuildParameterMappings(segment.Parameters, operationImport.Name);
    }

    public OperationImportSegment Segment { get; private set; }

    public IDictionary<string, string> ParameterMappings { get; private set; }

    public override bool TryMatch(ODataPathSegment pathSegment, IDictionary<string, object> values)
    {
      if (!(pathSegment is OperationImportSegment segment))
        return false;
      IEdmOperationImport operationImport1 = this.Segment.OperationImports.First<IEdmOperationImport>();
      IEdmOperationImport operationImport2 = segment.OperationImports.First<IEdmOperationImport>();
      if (operationImport1.IsActionImport() && operationImport2.IsActionImport())
        return operationImport1 == operationImport2;
      if (operationImport1.IsFunctionImport() && operationImport2.IsFunctionImport() && !(operationImport1.Name != operationImport2.Name))
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
            RoutingConventionHelpers.AddFunctionParameters((IEdmFunction) operationImport2.Operation, name, paramValue, values, values, this.ParameterMappings);
          }
          return true;
        }
      }
      return false;
    }
  }
}
