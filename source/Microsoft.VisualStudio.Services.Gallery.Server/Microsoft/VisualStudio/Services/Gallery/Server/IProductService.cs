// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.IProductService
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.VisualStudio.Services.Gallery.Server
{
  [DefaultServiceImplementation(typeof (ProductService))]
  public interface IProductService : IVssFrameworkService
  {
    IDictionary<InstallationTarget, ProductFamily> QueryProductFamilies(
      IVssRequestContext requestContext,
      IList<InstallationTarget> targets);

    IDictionary<InstallationTarget, ProductFamily> QueryProductFamilies(
      IVssRequestContext requestContext,
      PublishedExtension extension);

    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Issue is for return type. Don't want to create three more class for the return types")]
    IDictionary<InstallationTarget, IList<Product>> QueryProducts(
      IVssRequestContext requestContext,
      IList<InstallationTarget> targets);

    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Issue is for return type. Don't want to create three more class for the return types")]
    IDictionary<InstallationTarget, IList<Product>> QueryProducts(
      IVssRequestContext requestContext,
      PublishedExtension extension);

    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Issue is for return type. Don't want to create three more class for the return types")]
    IDictionary<InstallationTarget, IList<ProductRelease>> QueryReleases(
      IVssRequestContext requestContext,
      IList<InstallationTarget> targets);

    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Issue is for return type. Don't want to create three more class for the return types")]
    IDictionary<InstallationTarget, IList<ProductRelease>> QueryReleases(
      IVssRequestContext requestContext,
      PublishedExtension extension,
      string productType);
  }
}
