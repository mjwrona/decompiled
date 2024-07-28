// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Edm.Csdl.CsdlWriter
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
  public class CsdlWriter
  {
    private readonly IEdmModel model;
    private readonly IEnumerable<EdmSchema> schemas;
    private readonly XmlWriter writer;
    private readonly Version edmxVersion;
    private readonly string edmxNamespace;
    private readonly CsdlTarget target;

    private CsdlWriter(
      IEdmModel model,
      IEnumerable<EdmSchema> schemas,
      XmlWriter writer,
      Version edmxVersion,
      CsdlTarget target)
    {
      this.model = model;
      this.schemas = schemas;
      this.writer = writer;
      this.edmxVersion = edmxVersion;
      this.target = target;
      this.edmxNamespace = CsdlConstants.SupportedEdmxVersions[edmxVersion];
    }

    public static bool TryWriteCsdl(
      IEdmModel model,
      XmlWriter writer,
      CsdlTarget target,
      out IEnumerable<EdmError> errors)
    {
      EdmUtil.CheckArgumentNull<IEdmModel>(model, nameof (model));
      EdmUtil.CheckArgumentNull<XmlWriter>(writer, nameof (writer));
      errors = model.GetSerializationErrors();
      if (errors.FirstOrDefault<EdmError>() != null)
        return false;
      Version edmxVersion = model.GetEdmxVersion();
      if (edmxVersion != (Version) null)
      {
        if (!CsdlConstants.SupportedEdmxVersions.ContainsKey(edmxVersion))
        {
          errors = (IEnumerable<EdmError>) new EdmError[1]
          {
            new EdmError((EdmLocation) new CsdlLocation(0, 0), EdmErrorCode.UnknownEdmxVersion, Strings.Serializer_UnknownEdmxVersion)
          };
          return false;
        }
      }
      else
      {
        Dictionary<Version, Version> edmToEdmxVersions = CsdlConstants.EdmToEdmxVersions;
        Version key = model.GetEdmVersion();
        if ((object) key == null)
          key = EdmConstants.EdmVersionDefault;
        ref Version local = ref edmxVersion;
        if (!edmToEdmxVersions.TryGetValue(key, out local))
        {
          errors = (IEnumerable<EdmError>) new EdmError[1]
          {
            new EdmError((EdmLocation) new CsdlLocation(0, 0), EdmErrorCode.UnknownEdmVersion, Strings.Serializer_UnknownEdmVersion)
          };
          return false;
        }
      }
      IEnumerable<EdmSchema> schemas = new EdmModelSchemaSeparationSerializationVisitor(model).GetSchemas();
      new CsdlWriter(model, schemas, writer, edmxVersion, target).WriteCsdl();
      errors = Enumerable.Empty<EdmError>();
      return true;
    }

    private void WriteCsdl()
    {
      switch (this.target)
      {
        case CsdlTarget.EntityFramework:
          this.WriteEFCsdl();
          break;
        case CsdlTarget.OData:
          this.WriteODataCsdl();
          break;
        default:
          throw new InvalidOperationException(Strings.UnknownEnumVal_CsdlTarget((object) this.target.ToString()));
      }
    }

    private void WriteODataCsdl()
    {
      this.WriteEdmxElement();
      this.WriteReferenceElements();
      this.WriteDataServicesElement();
      this.WriteSchemas();
      this.EndElement();
      this.EndElement();
    }

    private void WriteEFCsdl()
    {
      this.WriteEdmxElement();
      this.WriteRuntimeElement();
      this.WriteConceptualModelsElement();
      this.WriteSchemas();
      this.EndElement();
      this.EndElement();
      this.EndElement();
    }

    private void WriteEdmxElement()
    {
      this.writer.WriteStartElement("edmx", "Edmx", this.edmxNamespace);
      this.writer.WriteAttributeString("Version", CsdlWriter.GetVersionString(this.edmxVersion));
    }

    private void WriteRuntimeElement() => this.writer.WriteStartElement("edmx", "Runtime", this.edmxNamespace);

    private void WriteConceptualModelsElement() => this.writer.WriteStartElement("edmx", "ConceptualModels", this.edmxNamespace);

    private void WriteReferenceElements() => new EdmModelReferenceElementsVisitor(this.model, this.writer, this.edmxVersion).VisitEdmReferences(this.model);

    private void WriteDataServicesElement() => this.writer.WriteStartElement("edmx", "DataServices", this.edmxNamespace);

    private void WriteSchemas()
    {
      Version version = this.model.GetEdmVersion();
      if ((object) version == null)
        version = EdmConstants.EdmVersionDefault;
      Version edmVersion = version;
      foreach (EdmSchema schema in this.schemas)
        new EdmModelCsdlSerializationVisitor(this.model, this.writer, edmVersion).VisitEdmSchema(schema, this.model.GetNamespacePrefixMappings());
    }

    private void EndElement() => this.writer.WriteEndElement();

    private static string GetVersionString(Version version) => version == EdmConstants.EdmVersion401 ? "4.01" : version.ToString();
  }
}
