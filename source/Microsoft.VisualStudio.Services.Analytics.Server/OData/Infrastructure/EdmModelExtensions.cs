// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.OData.Infrastructure.EdmModelExtensions
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

using Microsoft.AspNet.OData;
using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Csdl;
using Microsoft.OData.Edm.Validation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml;

namespace Microsoft.VisualStudio.Services.Analytics.OData.Infrastructure
{
  public static class EdmModelExtensions
  {
    public static bool TryGetMetdataXml(
      this IEdmModel model,
      out string csdl,
      out IEnumerable<EdmError> errors)
    {
      using (StringWriter w = new StringWriter())
      {
        using (XmlTextWriter writer = new XmlTextWriter((TextWriter) w))
        {
          writer.Formatting = Formatting.Indented;
          if (CsdlWriter.TryWriteCsdl(model, (XmlWriter) writer, CsdlTarget.OData, out errors))
          {
            csdl = w.ToString();
            return true;
          }
          csdl = (string) null;
          return false;
        }
      }
    }

    public static string GetMetadataXml(this IEdmModel model)
    {
      string csdl;
      if (model.TryGetMetdataXml(out csdl, out IEnumerable<EdmError> _))
        return csdl;
      throw new Exception(AnalyticsResources.UNABLE_TO_SERIALIZE_THE_MODEL());
    }

    public static PropertyInfo GetPropertyInfo(this IEdmModel model, IEdmProperty property)
    {
      ClrPropertyInfoAnnotation annotationValue = model.GetAnnotationValue<ClrPropertyInfoAnnotation>((IEdmElement) property);
      return annotationValue != null ? annotationValue.ClrPropertyInfo : model.GetAnnotationValue<ClrTypeAnnotation>((IEdmElement) property.DeclaringType).ClrType.GetProperty(property.Name);
    }
  }
}
