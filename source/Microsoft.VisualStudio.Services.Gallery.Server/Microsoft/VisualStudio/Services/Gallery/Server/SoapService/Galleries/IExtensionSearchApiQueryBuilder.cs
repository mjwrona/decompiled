// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.SoapService.Galleries.IExtensionSearchApiQueryBuilder
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.VisualStudio.Services.Gallery.Server.SoapService.Models;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.VisualStudio.Services.Gallery.Server.SoapService.Galleries
{
  internal interface IExtensionSearchApiQueryBuilder
  {
    VsIdeExtensionQuery BuildQuery(
      string searchText,
      string whereClause,
      string orderByClause,
      IDictionary<string, string> context,
      int? skip,
      int? take);

    VsIdeExtensionQuery BuildQueryForCategories(
      string vsVersion,
      string serviceSource,
      string templateType,
      string[] skus,
      string[] subSkus,
      int? parentCategoryId,
      int? programmingLanguageId,
      CultureInfo cultureInfo,
      string productArchitecture = null);

    VsIdeExtensionQuery BuildQueryFromExtensionQuery(ExtensionQuery extensionQuery);
  }
}
