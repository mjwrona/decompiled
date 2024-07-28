// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.Controllers.V2.SecureNuGetODataSerializerProvider
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using Microsoft.Data.Edm;
using Microsoft.Data.OData;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Net.Http;
using System.Web.Http.OData.Formatter.Serialization;

namespace Microsoft.VisualStudio.Services.NuGet.Server.Controllers.V2
{
  public class SecureNuGetODataSerializerProvider : DefaultODataSerializerProvider
  {
    private readonly ODataEdmTypeSerializer secureEntitySerializer;
    private readonly WrappedExceptionErrorSerializer wrappedExceptionSerializer = new WrappedExceptionErrorSerializer();

    public SecureNuGetODataSerializerProvider() => this.secureEntitySerializer = (ODataEdmTypeSerializer) new SecureDelegatingODataEdmTypeSerializer((ODataEdmTypeSerializer) new NuGetEntityTypeSerializer((ODataSerializerProvider) this));

    public override System.Web.Http.OData.Formatter.Serialization.ODataSerializer GetODataPayloadSerializer(
      IEdmModel model,
      Type type,
      HttpRequestMessage request)
    {
      if (!ServerJsonSerializationHelper.IsExcludedType(type) && !typeof (IEdmModel).IsAssignableFrom(type) && !typeof (ODataWorkspace).IsAssignableFrom(type))
        ServerJsonSerializationHelper.EnsureValidRootType(type);
      return type == typeof (WrappedException) ? (System.Web.Http.OData.Formatter.Serialization.ODataSerializer) this.wrappedExceptionSerializer : base.GetODataPayloadSerializer(model, type, request);
    }

    public override ODataEdmTypeSerializer GetEdmTypeSerializer(IEdmTypeReference edmType) => edmType.IsEntity() ? this.secureEntitySerializer : base.GetEdmTypeSerializer(edmType);
  }
}
