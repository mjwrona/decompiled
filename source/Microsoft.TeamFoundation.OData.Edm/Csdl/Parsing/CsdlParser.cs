// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Edm.Csdl.Parsing.CsdlParser
// Assembly: Microsoft.TeamFoundation.OData.Edm, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 75C95BF5-2544-413A-959F-F9F18C11D35F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Edm.dll

using Microsoft.OData.Edm.Csdl.Parsing.Ast;
using Microsoft.OData.Edm.Validation;
using System.Collections.Generic;
using System.Xml;

namespace Microsoft.OData.Edm.Csdl.Parsing
{
  internal class CsdlParser
  {
    private readonly List<EdmError> errorsList = new List<EdmError>();
    private readonly CsdlModel result = new CsdlModel();
    private bool success = true;

    public static bool TryParse(
      IEnumerable<XmlReader> csdlReaders,
      out CsdlModel entityModel,
      out IEnumerable<EdmError> errors)
    {
      EdmUtil.CheckArgumentNull<IEnumerable<XmlReader>>(csdlReaders, nameof (csdlReaders));
      CsdlParser csdlParser = new CsdlParser();
      int num = 0;
      foreach (XmlReader csdlReader in csdlReaders)
      {
        if (csdlReader != null)
        {
          try
          {
            csdlParser.AddReader(csdlReader);
          }
          catch (XmlException ex)
          {
            entityModel = (CsdlModel) null;
            errors = (IEnumerable<EdmError>) new EdmError[1]
            {
              new EdmError((EdmLocation) new CsdlLocation(ex.LineNumber, ex.LinePosition), EdmErrorCode.XmlError, ex.Message)
            };
            return false;
          }
          ++num;
        }
        else
        {
          entityModel = (CsdlModel) null;
          errors = (IEnumerable<EdmError>) new EdmError[1]
          {
            new EdmError((EdmLocation) null, EdmErrorCode.NullXmlReader, Strings.CsdlParser_NullXmlReader)
          };
          return false;
        }
      }
      if (num == 0)
      {
        entityModel = (CsdlModel) null;
        errors = (IEnumerable<EdmError>) new EdmError[1]
        {
          new EdmError((EdmLocation) null, EdmErrorCode.NoReadersProvided, Strings.CsdlParser_NoReadersProvided)
        };
        return false;
      }
      bool result = csdlParser.GetResult(out entityModel, out errors);
      if (!result)
        entityModel = (CsdlModel) null;
      return result;
    }

    public bool AddReader(XmlReader csdlReader, string source = null)
    {
      CsdlDocumentParser csdlDocumentParser = new CsdlDocumentParser(source ?? csdlReader.BaseURI, csdlReader);
      csdlDocumentParser.ParseDocumentElement();
      this.success &= !csdlDocumentParser.HasErrors;
      this.errorsList.AddRange(csdlDocumentParser.Errors);
      if (csdlDocumentParser.Result != null)
        this.result.AddSchema(csdlDocumentParser.Result.Value);
      return this.success;
    }

    public bool GetResult(out CsdlModel model, out IEnumerable<EdmError> errors)
    {
      model = this.result;
      errors = (IEnumerable<EdmError>) this.errorsList;
      return this.success;
    }
  }
}
