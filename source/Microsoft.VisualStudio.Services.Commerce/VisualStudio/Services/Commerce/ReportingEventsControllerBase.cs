// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.ReportingEventsControllerBase
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using Microsoft.Data.Edm;
using Microsoft.Data.Edm.Library;
using Microsoft.Data.OData;
using Microsoft.Data.OData.Query;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Commerce
{
  [ControllerApiVersion(3.2)]
  public abstract class ReportingEventsControllerBase : TfsApiController
  {
    [HttpGet]
    public virtual IEnumerable<ICommerceEvent> GetReportingEvents(
      string viewName,
      string resourceName,
      DateTime startTime,
      DateTime endTime)
    {
      return this.GetReportingEvents(viewName, resourceName, startTime, endTime, string.Empty);
    }

    [HttpGet]
    public virtual IEnumerable<ICommerceEvent> GetReportingEvents(
      string viewName,
      string resourceName,
      DateTime startTime,
      DateTime endTime,
      string filter)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(viewName, nameof (viewName));
      ArgumentUtility.CheckStringForNullOrEmpty(resourceName, nameof (resourceName));
      this.ValidateFilter(filter);
      IReportingService service = this.TfsRequestContext.GetService<IReportingService>();
      if (StringComparer.OrdinalIgnoreCase.Equals(viewName, "Publisher"))
        return (IEnumerable<ICommerceEvent>) service.GetReportingEvents<AzurePublisherCommerceEvent>(this.TfsRequestContext, viewName, resourceName, startTime, endTime, filter).Select<AzurePublisherCommerceEvent, CommerceEvent>((Func<AzurePublisherCommerceEvent, CommerceEvent>) (x => x.ToCommerceEvent()));
      if (StringComparer.OrdinalIgnoreCase.Equals(viewName, "Account"))
        return (IEnumerable<ICommerceEvent>) service.GetReportingEvents<AzureAccountCommerceEvent>(this.TfsRequestContext, viewName, resourceName, startTime, endTime, filter).Select<AzureAccountCommerceEvent, CommerceEvent>((Func<AzureAccountCommerceEvent, CommerceEvent>) (x => x.ToCommerceEvent()));
      throw new ReportingViewNotSupportedException("The view type " + viewName + " is not supported.");
    }

    private void ValidateFilter(string filter)
    {
      if (string.IsNullOrEmpty(filter))
        return;
      try
      {
        EdmEntityType elementType = new EdmEntityType("ReportingEventModel", "ReportingEvent");
        foreach (PropertyInfo property in typeof (ICommerceEvent).GetProperties(BindingFlags.Instance | BindingFlags.Public))
        {
          bool isNullable;
          EdmPrimitiveTypeKind primitiveTypeKind = this.GetEdmPrimitiveTypeKind(property, out isNullable);
          elementType.AddStructuralProperty(property.Name, primitiveTypeKind, isNullable);
        }
        EdmEntityContainer element = new EdmEntityContainer("ReportingEventModel", "ReportingContainer");
        element.AddEntitySet("ReportingEvent", (IEdmEntityType) elementType);
        EdmModel model = new EdmModel();
        model.AddElement((IEdmSchemaElement) element);
        ODataUriParser.ParseFilter(filter, (IEdmModel) model, (IEdmType) elementType);
      }
      catch (ODataException ex)
      {
        throw new ReportingViewInvalidFilterException(ex.Message, (Exception) ex);
      }
    }

    private EdmPrimitiveTypeKind GetEdmPrimitiveTypeKind(PropertyInfo property, out bool isNullable)
    {
      isNullable = false;
      Type type = property.PropertyType;
      if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof (Nullable<>))
      {
        type = type.GetGenericArguments()[0];
        isNullable = true;
      }
      EdmPrimitiveTypeKind result;
      if (!Enum.TryParse<EdmPrimitiveTypeKind>(type.Name, out result))
        throw new NotSupportedException("Type " + property.PropertyType.Name + " of " + property.Name + " property is not supported in Edm.");
      return result;
    }
  }
}
