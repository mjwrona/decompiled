// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.OData.Infrastructure.FastODataResourceSerializer
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Formatter;
using Microsoft.AspNet.OData.Formatter.Serialization;
using Microsoft.OData;
using Microsoft.OData.Edm;
using Microsoft.VisualStudio.Services.Common;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Analytics.OData.Infrastructure
{
  public class FastODataResourceSerializer : ODataResourceSerializer
  {
    public FastODataResourceSerializer(ODataSerializerProvider serializerProvider)
      : base(serializerProvider)
    {
    }

    public override string CreateETag(ResourceContext resourceContext) => (string) null;

    public override ODataProperty CreateStructuralProperty(
      IEdmStructuralProperty structuralProperty,
      ResourceContext resourceContext)
    {
      ArgumentUtility.CheckForNull<IEdmStructuralProperty>(structuralProperty, nameof (structuralProperty));
      ArgumentUtility.CheckForNull<ResourceContext>(resourceContext, nameof (resourceContext));
      ODataSerializerContext serializerContext = resourceContext.SerializerContext;
      ODataEdmTypeSerializer edmTypeSerializer = this.SerializerProvider.GetEdmTypeSerializer(structuralProperty.Type);
      if (edmTypeSerializer == null)
        throw new SerializationException(AnalyticsResources.ODATA_TYPE_CANNOT_BE_SERIALIZED((object) structuralProperty.Type.FullName(), (object) typeof (ODataMediaTypeFormatter).Name));
      object propertyValue = resourceContext.GetPropertyValue(structuralProperty.Name);
      ODataProperty structuralProperty1 = new ODataProperty();
      structuralProperty1.Name = structuralProperty.Name;
      structuralProperty1.Value = (object) edmTypeSerializer.CreateODataValue(propertyValue, structuralProperty.Type, serializerContext);
      return structuralProperty1;
    }
  }
}
