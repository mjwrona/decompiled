// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.CatalogServiceHelper
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Core;
using Microsoft.TeamFoundation.TestManagement.Client;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  internal static class CatalogServiceHelper
  {
    internal static CatalogNode GetEnvironmentParentNode(
      TfsTestManagementRequestContext context,
      Guid teamProjectCollectionCatalogResourceId,
      Guid parentType,
      KeyValuePair<string, string> property)
    {
      CatalogNode projectCollectionNode = CatalogServiceHelper.GetProjectCollectionNode(context, teamProjectCollectionCatalogResourceId);
      CatalogNode nodeByProperty = CatalogServiceHelper.FindNodeByProperty(context, projectCollectionNode.FullPath, parentType, property);
      if (nodeByProperty != null)
        return nodeByProperty;
      if (parentType == CatalogResourceTypes.TeamProject)
        throw new TeamFoundationServiceException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.TeamProjectCatalogNodeNotFound, (object) property.Value));
      throw new TestObjectNotFoundException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.TestControllerNotFound, (object) property.Value), ObjectTypes.TestController);
    }

    internal static CatalogNode GetProjectCollectionNode(
      TfsTestManagementRequestContext context,
      Guid resourceId)
    {
      List<CatalogResource> catalogResourceList = context.CatalogService.QueryResources(context.RequestContext.To(TeamFoundationHostType.Application).Elevate(), (IEnumerable<Guid>) new Guid[1]
      {
        resourceId
      }, CatalogQueryOptions.None);
      if (catalogResourceList.Count != 0)
      {
        foreach (CatalogNode nodeReference in catalogResourceList[0].NodeReferences)
        {
          if (nodeReference.FullPath.StartsWith(CatalogRoots.OrganizationalPath, false, CultureInfo.InvariantCulture))
            return nodeReference;
        }
      }
      throw new TeamFoundationServiceException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.ProjectCollectionCatalogNodeNotFound, (object) resourceId));
    }

    internal static CatalogResource FindResourceByProperty(
      TfsTestManagementRequestContext context,
      string parentPath,
      Guid resourceType,
      KeyValuePair<string, string> property)
    {
      return CatalogServiceHelper.FindNodeByProperty(context, parentPath, resourceType, property)?.Resource;
    }

    internal static CatalogNode FindNodeByProperty(
      TfsTestManagementRequestContext context,
      string parentPath,
      Guid resourceType,
      KeyValuePair<string, string> property)
    {
      List<CatalogNode> catalogNodeList = context.CatalogService.QueryNodes(context.RequestContext.To(TeamFoundationHostType.Application).Elevate(), (IEnumerable<string>) new string[1]
      {
        parentPath + CatalogConstants.SingleRecurseStar
      }, (IEnumerable<Guid>) new Guid[1]{ resourceType }, (IEnumerable<KeyValuePair<string, string>>) new KeyValuePair<string, string>[1]
      {
        property
      }, CatalogQueryOptions.None);
      if (catalogNodeList.Count == 0)
        return (CatalogNode) null;
      context.TraceAndDebugAssert("BusinessLayer", catalogNodeList.Count == 1, "Count should be 1");
      return catalogNodeList[0];
    }

    internal static void SaveBatch(
      TestManagementRequestContext context,
      CatalogTransactionContext batch)
    {
      batch.Save(context.RequestContext.To(TeamFoundationHostType.Application).Elevate(), false, out List<CatalogResource> _, out List<CatalogNode> _);
    }

    internal static string GetProperty(CatalogResource resource, string propertyName)
    {
      string property;
      resource.Properties.TryGetValue(propertyName, out property);
      return property;
    }
  }
}
