// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Edm.Csdl.Serialization.EdmModelReferenceElementsVisitor
// Assembly: Microsoft.TeamFoundation.OData.Edm, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 75C95BF5-2544-413A-959F-F9F18C11D35F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Edm.dll

using System;
using System.Collections.Generic;
using System.Xml;

namespace Microsoft.OData.Edm.Csdl.Serialization
{
  internal class EdmModelReferenceElementsVisitor
  {
    private readonly EdmModelCsdlSchemaWriter schemaWriter;

    internal EdmModelReferenceElementsVisitor(
      IEdmModel model,
      XmlWriter xmlWriter,
      Version edmxVersion)
    {
      this.schemaWriter = new EdmModelCsdlSchemaWriter(model, model.GetNamespaceAliases(), xmlWriter, edmxVersion);
    }

    internal void VisitEdmReferences(IEdmModel model)
    {
      IEnumerable<IEdmReference> edmReferences = model.GetEdmReferences();
      if (model == null || edmReferences == null)
        return;
      foreach (IEdmReference reference in edmReferences)
      {
        this.schemaWriter.WriteReferenceElementHeader(reference);
        if (reference.Includes != null)
        {
          foreach (IEdmInclude include in reference.Includes)
            this.schemaWriter.WriteIncludeElement(include);
        }
        if (reference.IncludeAnnotations != null)
        {
          foreach (IEdmIncludeAnnotations includeAnnotation in reference.IncludeAnnotations)
            this.schemaWriter.WriteIncludeAnnotationsElement(includeAnnotation);
        }
        this.schemaWriter.WriteEndElement();
      }
    }
  }
}
