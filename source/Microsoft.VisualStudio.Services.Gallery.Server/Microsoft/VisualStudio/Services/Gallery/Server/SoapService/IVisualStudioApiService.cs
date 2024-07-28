// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.SoapService.IVisualStudioApiService
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Galleries.Domain.Model;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Gallery.Server.SoapService.Galleries;
using Microsoft.VisualStudio.Services.Gallery.Server.SoapService.VStudio;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using System;
using System.Collections.Generic;
using VsGallery.WebServices;

namespace Microsoft.VisualStudio.Services.Gallery.Server.SoapService
{
  [DefaultServiceImplementation(typeof (VisualStudioApiService))]
  public interface IVisualStudioApiService : IVssFrameworkService
  {
    ICollection<IdeCategory> GetRootCategories(
      IVssRequestContext requestContext,
      VisualStudioIdeVersion vsVersion,
      IDictionary<string, string> requestParameters);

    ICollection<IdeCategory> GetRootCategories(
      IVssRequestContext requestContext,
      VisualStudioIdeVersion vsVersion,
      string cultureName);

    IdeCategory GetCategoryTree(
      IVssRequestContext requestContext,
      VisualStudioIdeVersion vsVersion,
      Guid categoryId,
      int level,
      IDictionary<string, string> requestParameters);

    IdeCategory GetCategoryTree(
      IVssRequestContext requestContext,
      VisualStudioIdeVersion vsVersion,
      Guid categoryId,
      int level,
      string projectType,
      string templateType,
      string[] skus,
      string[] subSkus,
      int[] templateGroupIds,
      int[] vsVersions,
      string cultureName);

    string[] GetCurrentVersionsForVsixList(
      IVssRequestContext requestContext,
      VisualStudioIdeVersion vsVersion,
      string[] vsixIds,
      IDictionary<string, string> requestParameters);

    ReleaseQueryResult SearchReleases(
      IVssRequestContext requestContext,
      VisualStudioIdeVersion vsVersion,
      string searchText,
      string whereClause,
      string orderByClause,
      int? skip,
      int? take,
      IDictionary<string, string> requestParameters);

    ReleaseQueryResult SearchReleases(
      IVssRequestContext requestContext,
      VisualStudioIdeVersion vsVersion,
      string searchText,
      string whereClause,
      string orderByClause,
      int? locale,
      int? skip,
      int? take);

    ExtensionQueryResult SearchForVsIde(
      IVssRequestContext requestContext,
      ExtensionQuery extensionQuery);
  }
}
