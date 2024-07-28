// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.TeamFoundationCatalogHelper
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;

namespace Microsoft.TeamFoundation.Server.Core
{
  public static class TeamFoundationCatalogHelper
  {
    private static readonly string s_area = "Catalog";
    private static readonly string s_layer = "CatalogHelperMethods";

    public static CatalogNode QueryApplicationInstanceNode(IVssRequestContext requestContext)
    {
      CatalogNode appInstanceNode;
      if (!TeamFoundationCatalogHelper.TryQueryApplicationInstanceNode(requestContext, out appInstanceNode))
        throw new CatalogNodeDoesNotExistException(FrameworkResources.CatalogTFSInstanceNodeMissing((object) FrameworkResources.TeamFoundationServerInstance()));
      return appInstanceNode;
    }

    internal static bool TryQueryApplicationInstanceNode(
      IVssRequestContext requestContext,
      out CatalogNode appInstanceNode)
    {
      ITeamFoundationCatalogService service = requestContext.Elevate().GetService<ITeamFoundationCatalogService>();
      appInstanceNode = service.QueryNodes(requestContext, CatalogPath.MakeRecursive(CatalogPath.OrganizationalPath, false), CatalogResourceTypes.TeamFoundationServerInstance).FirstOrDefault<CatalogNode>();
      return appInstanceNode != null;
    }

    public static CatalogNode QueryCollectionCatalogNode(
      IVssRequestContext requestContext,
      Guid collectionId,
      CatalogQueryOptions queryOptions)
    {
      requestContext.Trace(54001, TraceLevel.Verbose, TeamFoundationCatalogHelper.s_area, TeamFoundationCatalogHelper.s_layer, "QueryCollectionCatalogNode: Searching for node for {0}", (object) collectionId);
      List<CatalogNode> source = requestContext.GetService<ITeamFoundationCatalogService>().QueryNodes(requestContext, CatalogPath.MakeRecursive(CatalogPath.OrganizationalPath, true), CatalogResourceTypes.ProjectCollection, (IEnumerable<KeyValuePair<string, string>>) new KeyValuePair<string, string>[1]
      {
        new KeyValuePair<string, string>("InstanceId", collectionId.ToString())
      }, queryOptions);
      if (source.Count > 1)
        requestContext.Trace(54002, TraceLevel.Error, TeamFoundationCatalogHelper.s_area, TeamFoundationCatalogHelper.s_layer, "{0} catalog nodes found for collection {0}", (object) source.Count, (object) collectionId);
      else
        requestContext.Trace(54003, TraceLevel.Verbose, TeamFoundationCatalogHelper.s_area, TeamFoundationCatalogHelper.s_layer, "{0} catalog nodes found for collection {0}", (object) source.Count, (object) collectionId);
      CatalogNode catalogNode = source.FirstOrDefault<CatalogNode>();
      if (catalogNode == null)
      {
        requestContext.Trace(54004, TraceLevel.Warning, TeamFoundationCatalogHelper.s_area, TeamFoundationCatalogHelper.s_layer, "No catalog node found for collection {0}", (object) collectionId);
        requestContext.Trace(54005, TraceLevel.Verbose, TeamFoundationCatalogHelper.s_area, TeamFoundationCatalogHelper.s_layer, "QueryCollectionCatalogNode: returning null for collection {0}", (object) collectionId);
      }
      else
        requestContext.Trace(54006, TraceLevel.Verbose, TeamFoundationCatalogHelper.s_area, TeamFoundationCatalogHelper.s_layer, "QueryCollectionCatalogNode: returning {0} for collection {1}", (object) catalogNode.FullPath, (object) collectionId);
      return catalogNode;
    }

    public static List<CatalogNode> QueryCollectionCatalogNodes(
      IVssRequestContext requestContext,
      CatalogQueryOptions queryOptions)
    {
      return requestContext.GetService<ITeamFoundationCatalogService>().QueryNodes(requestContext, CatalogPath.MakeRecursive(CatalogPath.OrganizationalPath, true), CatalogResourceTypes.ProjectCollection, (IEnumerable<KeyValuePair<string, string>>) null, queryOptions);
    }

    public static CatalogResource QueryCollectionCatalogResource(
      IVssRequestContext requestContext,
      string collectionId,
      CatalogQueryOptions queryOptions)
    {
      return requestContext.Elevate().GetService<ITeamFoundationCatalogService>().QueryResources(requestContext, (IEnumerable<Guid>) new Guid[1]
      {
        CatalogResourceTypes.ProjectCollection
      }, (IEnumerable<KeyValuePair<string, string>>) new KeyValuePair<string, string>[1]
      {
        new KeyValuePair<string, string>("InstanceId", collectionId)
      }, queryOptions).FirstOrDefault<CatalogResource>();
    }

    internal static void UpdateCollectionNameAndDescriptionInCatalog(
      IVssRequestContext requestContext,
      Guid collectionId,
      string name,
      string description)
    {
      CatalogResource catalogResource = TeamFoundationCatalogHelper.QueryCollectionCatalogResource(requestContext, collectionId.ToString("D", (IFormatProvider) CultureInfo.InvariantCulture), CatalogQueryOptions.None);
      if (catalogResource == null)
        return;
      catalogResource.DisplayName = name;
      catalogResource.Description = description;
      CatalogTransactionContext transactionContext = requestContext.Elevate().GetService<ITeamFoundationCatalogService>().CreateTransactionContext();
      transactionContext.AttachResource(catalogResource);
      transactionContext.Save(requestContext, false);
    }

    internal static CatalogNode EnsureMachineExistsInCatalog(
      IVssRequestContext requestContext,
      string machineName)
    {
      TeamFoundationTracingService.TraceEnterRaw(57150, TeamFoundationCatalogHelper.s_area, TeamFoundationCatalogHelper.s_layer, nameof (EnsureMachineExistsInCatalog));
      try
      {
        ITeamFoundationCatalogService service = requestContext.GetService<ITeamFoundationCatalogService>();
        CatalogNode catalogNode1 = service.QueryRootNode(requestContext, CatalogTree.Infrastructure);
        int num = 0;
label_2:
        try
        {
          CatalogNode catalogNode2 = TeamFoundationCatalogHelper.QueryMachineCatalogNode(requestContext, machineName, CatalogQueryOptions.None);
          if (catalogNode2 == null)
          {
            CatalogTransactionContext transactionContext = service.CreateTransactionContext();
            catalogNode2 = catalogNode1.CreateChild(requestContext, CatalogResourceTypes.Machine, machineName);
            catalogNode2.Resource.Properties["MachineName"] = machineName;
            transactionContext.AttachNode(catalogNode2);
            transactionContext.Save(requestContext, false);
          }
          return catalogNode2;
        }
        catch (InvalidCatalogSaveResourceException ex)
        {
          TeamFoundationTracingService.TraceExceptionRaw(57154, TraceLevel.Warning, TeamFoundationCatalogHelper.s_area, TeamFoundationCatalogHelper.s_layer, (Exception) ex);
          if (num++ > 2)
            throw;
          else
            goto label_2;
        }
      }
      catch (Exception ex)
      {
        TeamFoundationTracingService.TraceExceptionRaw(57154, TraceLevel.Error, TeamFoundationCatalogHelper.s_area, TeamFoundationCatalogHelper.s_layer, ex);
        throw;
      }
      finally
      {
        TeamFoundationTracingService.TraceLeaveRaw(57155, TeamFoundationCatalogHelper.s_area, TeamFoundationCatalogHelper.s_layer, nameof (EnsureMachineExistsInCatalog));
      }
    }

    private static CatalogNode QueryMachineCatalogNode(
      IVssRequestContext requestContext,
      string machineName,
      CatalogQueryOptions queryOptions)
    {
      TeamFoundationTracingService.TraceEnterRaw(57190, TeamFoundationCatalogHelper.s_area, TeamFoundationCatalogHelper.s_layer, nameof (QueryMachineCatalogNode));
      try
      {
        return requestContext.GetService<ITeamFoundationCatalogService>().QueryNodes(requestContext, (IEnumerable<string>) new string[1]
        {
          CatalogRoots.InfrastructurePath + CatalogConstants.SingleRecurseStar
        }, (IEnumerable<Guid>) new Guid[1]
        {
          CatalogResourceTypes.Machine
        }, (IEnumerable<KeyValuePair<string, string>>) new KeyValuePair<string, string>[1]
        {
          new KeyValuePair<string, string>("MachineName", machineName)
        }, queryOptions).FirstOrDefault<CatalogNode>();
      }
      catch (Exception ex)
      {
        TeamFoundationTracingService.TraceExceptionRaw(57194, TraceLevel.Error, TeamFoundationCatalogHelper.s_area, TeamFoundationCatalogHelper.s_layer, ex);
        throw;
      }
      finally
      {
        TeamFoundationTracingService.TraceLeaveRaw(57195, TeamFoundationCatalogHelper.s_area, TeamFoundationCatalogHelper.s_layer, nameof (QueryMachineCatalogNode));
      }
    }
  }
}
