// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Edm.Csdl.SchemaReader
// Assembly: Microsoft.TeamFoundation.OData.Edm, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 75C95BF5-2544-413A-959F-F9F18C11D35F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Edm.dll

using Microsoft.OData.Edm.Csdl.CsdlSemantics;
using Microsoft.OData.Edm.Csdl.Parsing;
using Microsoft.OData.Edm.Csdl.Parsing.Ast;
using Microsoft.OData.Edm.Validation;
using Microsoft.OData.Edm.Vocabularies;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace Microsoft.OData.Edm.Csdl
{
  public static class SchemaReader
  {
    public static bool TryParse(
      IEnumerable<XmlReader> readers,
      out IEdmModel model,
      out IEnumerable<EdmError> errors)
    {
      return SchemaReader.TryParse(readers, Enumerable.Empty<IEdmModel>(), out model, out errors);
    }

    public static bool TryParse(
      IEnumerable<XmlReader> readers,
      IEdmModel reference,
      out IEdmModel model,
      out IEnumerable<EdmError> errors)
    {
      return SchemaReader.TryParse(readers, (IEnumerable<IEdmModel>) new IEdmModel[1]
      {
        reference
      }, out model, out errors);
    }

    public static bool TryParse(
      IEnumerable<XmlReader> readers,
      IEnumerable<IEdmModel> references,
      out IEdmModel model,
      out IEnumerable<EdmError> errors)
    {
      return SchemaReader.TryParse(readers, references, true, out model, out errors);
    }

    public static bool TryParse(
      IEnumerable<XmlReader> readers,
      IEnumerable<IEdmModel> references,
      bool includeDefaultVocabularies,
      out IEdmModel model,
      out IEnumerable<EdmError> errors)
    {
      CsdlModel entityModel;
      if (CsdlParser.TryParse(readers, out entityModel, out errors))
      {
        CsdlSemanticsModel csdlSemanticsModel = new CsdlSemanticsModel(entityModel, (IEdmDirectValueAnnotationsManager) new CsdlSemanticsDirectValueAnnotationsManager(), references, includeDefaultVocabularies);
        model = (IEdmModel) csdlSemanticsModel;
        return true;
      }
      model = (IEdmModel) null;
      return false;
    }
  }
}
