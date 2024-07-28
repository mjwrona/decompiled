// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.SoapService.IVsIdeService
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Galleries.Domain.Model;
using Microsoft.VisualStudio.Services.Gallery.Server.SoapService.Serializer;
using System;
using System.Collections.Generic;
using System.ServiceModel;
using VsGallery.WebServices;

namespace Microsoft.VisualStudio.Services.Gallery.Server.SoapService
{
  [ServiceContract]
  public interface IVsIdeService
  {
    [OperationContract]
    [FaultContract(typeof (Guid), Action = "http://galleries.msdn.microsoft.com/faults/generic")]
    [PreserveObjectReferences]
    ReleaseQueryResult SearchReleases(
      string searchText,
      string whereClause,
      string orderByClause,
      int? locale,
      int? skip,
      int? take);

    [OperationContract(Name = "SearchReleases2")]
    [FaultContract(typeof (Guid), Action = "http://galleries.msdn.microsoft.com/faults/generic")]
    [PreserveObjectReferences]
    ReleaseQueryResult SearchReleases(
      string searchText,
      string whereClause,
      string orderByClause,
      int? skip,
      int? take,
      IDictionary<string, string> requestContext);

    [OperationContract]
    [FaultContract(typeof (Guid), Action = "http://galleries.msdn.microsoft.com/faults/generic")]
    [PreserveObjectReferences]
    string[] GetCurrentVersionsForVsixList(
      string[] vsixIds,
      IDictionary<string, string> requestContext);

    [OperationContract]
    [FaultContract(typeof (Guid), Action = "http://galleries.msdn.microsoft.com/faults/generic")]
    [PreserveObjectReferences]
    IdeCategory GetCategoryTree(
      Guid categoryId,
      int level,
      string projectType,
      string templateType,
      string[] skus,
      string[] subSkus,
      int[] templateGroupIds,
      int[] vsVersions,
      string cultureName);

    [OperationContract]
    [FaultContract(typeof (Guid), Action = "http://galleries.msdn.microsoft.com/faults/generic")]
    [PreserveObjectReferences]
    ICollection<IdeCategory> GetRootCategories(string cultureName);

    [OperationContract(Name = "GetRootCategories2")]
    [FaultContract(typeof (Guid), Action = "http://galleries.msdn.microsoft.com/faults/generic")]
    [PreserveObjectReferences]
    ICollection<IdeCategory> GetRootCategories(IDictionary<string, string> requestContext);

    [OperationContract(Name = "GetCategoryTree2")]
    [FaultContract(typeof (Guid), Action = "http://galleries.msdn.microsoft.com/faults/generic")]
    [PreserveObjectReferences]
    IdeCategory GetCategoryTree(
      Guid categoryId,
      int level,
      IDictionary<string, string> requestContext);
  }
}
