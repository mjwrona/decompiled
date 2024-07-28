// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Edm.Csdl.SchemaWriter
// Assembly: Microsoft.TeamFoundation.OData.Edm, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 75C95BF5-2544-413A-959F-F9F18C11D35F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Edm.dll

using Microsoft.OData.Edm.Csdl.Serialization;
using Microsoft.OData.Edm.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace Microsoft.OData.Edm.Csdl
{
  public static class SchemaWriter
  {
    public static bool TryWriteSchema(
      this IEdmModel model,
      XmlWriter writer,
      out IEnumerable<EdmError> errors)
    {
      return SchemaWriter.TryWriteSchema(model, (Func<string, XmlWriter>) (x => writer), true, out errors);
    }

    public static bool TryWriteSchema(
      this IEdmModel model,
      Func<string, XmlWriter> writerProvider,
      out IEnumerable<EdmError> errors)
    {
      return SchemaWriter.TryWriteSchema(model, writerProvider, false, out errors);
    }

    internal static bool TryWriteSchema(
      IEdmModel model,
      Func<string, XmlWriter> writerProvider,
      bool singleFileExpected,
      out IEnumerable<EdmError> errors)
    {
      EdmUtil.CheckArgumentNull<IEdmModel>(model, nameof (model));
      EdmUtil.CheckArgumentNull<Func<string, XmlWriter>>(writerProvider, nameof (writerProvider));
      errors = model.GetSerializationErrors();
      if (errors.FirstOrDefault<EdmError>() != null)
        return false;
      IEnumerable<EdmSchema> schemas = new EdmModelSchemaSeparationSerializationVisitor(model).GetSchemas();
      if (schemas.Count<EdmSchema>() > 1 & singleFileExpected)
      {
        errors = (IEnumerable<EdmError>) new EdmError[1]
        {
          new EdmError((EdmLocation) new CsdlLocation(0, 0), EdmErrorCode.SingleFileExpected, Strings.Serializer_SingleFileExpected)
        };
        return false;
      }
      if (schemas.Count<EdmSchema>() == 0)
      {
        errors = (IEnumerable<EdmError>) new EdmError[1]
        {
          new EdmError((EdmLocation) new CsdlLocation(0, 0), EdmErrorCode.NoSchemasProduced, Strings.Serializer_NoSchemasProduced)
        };
        return false;
      }
      SchemaWriter.WriteSchemas(model, schemas, writerProvider);
      errors = Enumerable.Empty<EdmError>();
      return true;
    }

    internal static void WriteSchemas(
      IEdmModel model,
      IEnumerable<EdmSchema> schemas,
      Func<string, XmlWriter> writerProvider)
    {
      Version version = model.GetEdmVersion();
      if ((object) version == null)
        version = EdmConstants.EdmVersionDefault;
      Version edmVersion = version;
      foreach (EdmSchema schema in schemas)
      {
        XmlWriter xmlWriter = writerProvider(schema.Namespace);
        if (xmlWriter != null)
          new EdmModelCsdlSerializationVisitor(model, xmlWriter, edmVersion).VisitEdmSchema(schema, model.GetNamespacePrefixMappings());
      }
    }
  }
}
