// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.DataspaceService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class DataspaceService : IDataspaceService, IVssFrameworkService
  {
    private const string s_area = "Dataspace";
    private const string s_layer = "DataspaceService";

    void IVssFrameworkService.ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    void IVssFrameworkService.ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public void CreateDataspace(
      IVssRequestContext requestContext,
      string dataspaceCategory,
      Guid dataspaceIdentifier,
      DataspaceState initialState)
    {
      this.CreateDataspaces(requestContext, new string[1]
      {
        dataspaceCategory
      }, dataspaceIdentifier, initialState);
    }

    public void CreateDataspaces(
      IVssRequestContext requestContext,
      string[] dataspaceCategories,
      Guid dataspaceIdentifier)
    {
      this.CreateDataspaces(requestContext, dataspaceCategories, dataspaceIdentifier, DataspaceState.Active);
    }

    public void CreateDataspaces(
      IVssRequestContext requestContext,
      string[] dataspaceCategories,
      Guid dataspaceIdentifier,
      DataspaceState initialState)
    {
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) dataspaceCategories, nameof (dataspaceCategories));
      if (initialState != DataspaceState.Creating && initialState != DataspaceState.Active)
        throw new ArgumentException(FrameworkResources.DataspaceMustBeCreatingOrActive(), nameof (initialState));
      foreach (string dataspaceCategory in dataspaceCategories)
        this.ValidateCategory(dataspaceCategory, "dataspaceCategory");
      IList<KeyValuePair<string, int>> categoryDatabaseMapping = this.ComputeDataspaceCategoryDatabaseMapping(requestContext, dataspaceCategories, dataspaceIdentifier);
      this.CreateDataspaces(requestContext, dataspaceIdentifier, (IEnumerable<KeyValuePair<string, int>>) categoryDatabaseMapping, initialState);
    }

    public void CreateDataspace(
      IVssRequestContext requestContext,
      string dataspaceCategory,
      Guid dataspaceIdentifier,
      int databaseId)
    {
      this.ValidateCategory(dataspaceCategory, nameof (dataspaceCategory));
      this.CreateDataspaces(requestContext, dataspaceIdentifier, (IEnumerable<KeyValuePair<string, int>>) new Dictionary<string, int>()
      {
        {
          dataspaceCategory,
          databaseId
        }
      }, DataspaceState.Active);
    }

    public void CreateDataspace(
      IVssRequestContext requestContext,
      string databaseCategory,
      string dataspaceCategory,
      Guid dataspaceIdentifier,
      int databaseId)
    {
      this.ValidateCategory(dataspaceCategory, nameof (dataspaceCategory));
      this.CreateDataspaces(requestContext, dataspaceIdentifier, (IEnumerable<KeyValuePair<string, int>>) new Dictionary<string, int>()
      {
        {
          dataspaceCategory,
          databaseId
        }
      }, DataspaceState.Active);
    }

    private IList<KeyValuePair<string, int>> ComputeDataspaceCategoryDatabaseMapping(
      IVssRequestContext requestContext,
      string[] dataspaceCategories,
      Guid dataspaceIdentifier)
    {
      IList<KeyValuePair<string, int>> categoryDatabaseMapping = (IList<KeyValuePair<string, int>>) new List<KeyValuePair<string, int>>(dataspaceCategories.Length);
      foreach (string dataspaceCategory in dataspaceCategories)
      {
        int databaseId = requestContext.ServiceHost.ServiceHostInternal().DatabaseId;
        if (dataspaceIdentifier != Guid.Empty && TeamFoundationDatabaseExtensions.BuildDataspaceIsSplit(requestContext))
        {
          Dataspace dataspace = this.QueryDataspace(requestContext, dataspaceCategory, Guid.Empty, false);
          if (dataspace != null)
            databaseId = dataspace.DatabaseId;
        }
        categoryDatabaseMapping.Add(new KeyValuePair<string, int>(dataspaceCategory.Trim(), databaseId));
      }
      return categoryDatabaseMapping;
    }

    private void ValidateCategory(string category, string name)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(category, name);
      if (category.Length > 260)
        throw new ArgumentException(name, "Dataspace category must be less than or equal to 260 characters.");
    }

    private void CreateDataspaces(
      IVssRequestContext requestContext,
      Guid dataspaceIdentifier,
      IEnumerable<KeyValuePair<string, int>> values,
      DataspaceState initialState)
    {
      DataspaceCacheService service = requestContext.GetService<DataspaceCacheService>();
      service.Synchronize<List<Dataspace>>(requestContext, (Func<List<Dataspace>>) (() =>
      {
        using (DataspaceComponent component = requestContext.CreateComponent<DataspaceComponent>())
        {
          requestContext.Trace(1002111, TraceLevel.Info, "Dataspace", nameof (DataspaceService), "Dataspace component version is {0}", (object) component.Version);
          return component.CreateDataspaces(dataspaceIdentifier, values, initialState);
        }
      }), (Action<DataspaceCacheData, List<Dataspace>>) ((cacheData, dataspaces) =>
      {
        foreach (Dataspace dataspace in dataspaces)
          cacheData.AddDataspace(requestContext, dataspace);
      }));
      if (requestContext.ServiceHost.HostType != TeamFoundationHostType.ProjectCollection)
        return;
      foreach (KeyValuePair<string, int> keyValuePair in values)
      {
        if (keyValuePair.Value != requestContext.ServiceHost.ServiceHostInternal().DatabaseId)
        {
          Dataspace dataspace = service.GetDataspace(requestContext, keyValuePair.Key, dataspaceIdentifier);
          using (DataspaceComponent component = requestContext.CreateComponent<DataspaceComponent>(keyValuePair.Key, new DatabaseConnectionType?(DatabaseConnectionType.Dbo)))
          {
            requestContext.Trace(1002111, TraceLevel.Info, "Dataspace", nameof (DataspaceService), "Creating dataspace with Id {0} in split DB {1}. Dataspace component version is {2}", (object) dataspace.DataspaceId, (object) keyValuePair.Key, (object) component.Version);
            component.CreateSplitDataspace(dataspace.DataspaceId, dataspace.DataspaceIdentifier, dataspace.DataspaceCategory, dataspace.DatabaseId, dataspace.State);
          }
        }
      }
    }

    public Dataspace QueryDataspace(
      IVssRequestContext requestContext,
      string dataspaceCategory,
      Guid dataspaceIdentifier)
    {
      return this.QueryDataspace(requestContext, dataspaceCategory, dataspaceIdentifier, false);
    }

    public Dataspace QueryDataspace(
      IVssRequestContext requestContext,
      string dataspaceCategory,
      Guid dataspaceIdentifier,
      bool throwOnMissing)
    {
      if (requestContext.DataspaceIdentifier != Guid.Empty && dataspaceIdentifier != Guid.Empty && requestContext.DataspaceIdentifier != dataspaceIdentifier && !requestContext.RootContext.Items.ContainsKey(RequestContextItemsKeys.AllowCrossDataspaceAccess))
      {
        requestContext.Trace(271571947, TraceLevel.Error, "Dataspace", "AnonymousAccessKalypsoAlert", string.Format("Performed cross dataspace operation request:{0}, ", (object) requestContext.DataspaceIdentifier) + string.Format("input:{0}, {1}", (object) dataspaceIdentifier, (object) EnvironmentWrapper.ToReadableStackTrace()));
        throw new CrossDataspaceAccessException(requestContext.DataspaceIdentifier, dataspaceIdentifier);
      }
      Dataspace dataspace = requestContext.GetService<DataspaceCacheService>().GetDataspace(requestContext, dataspaceCategory, dataspaceIdentifier);
      if (dataspace == null & throwOnMissing)
        throw new DataspaceNotFoundException(dataspaceCategory, dataspaceIdentifier);
      this.AssumeHostDatabaseIdIfUnspecified(requestContext, dataspace);
      return dataspace;
    }

    public Dataspace QueryDataspace(IVssRequestContext requestContext, int dataspaceId)
    {
      Dataspace dataspace = requestContext.GetService<DataspaceCacheService>().GetDataspace(requestContext, dataspaceId);
      if (dataspace == null)
        throw new DataspaceNotFoundException(dataspaceId);
      this.AssumeHostDatabaseIdIfUnspecified(requestContext, dataspace);
      return dataspace;
    }

    private void AssumeHostDatabaseIdIfUnspecified(
      IVssRequestContext requestContext,
      Dataspace dataspace)
    {
      int? databaseId = dataspace?.DatabaseId;
      int invalidDatabaseId = DatabaseManagementConstants.InvalidDatabaseId;
      if (!(databaseId.GetValueOrDefault() == invalidDatabaseId & databaseId.HasValue))
        return;
      dataspace.DatabaseId = requestContext.ServiceHost.ServiceHostInternal().DatabaseId;
    }

    public void UpdateDataspaces(
      IVssRequestContext requestContext,
      string dataspaceCategory,
      Guid? dataspaceIdentifier,
      int? newDatabaseId,
      DataspaceState? newDataspaceState)
    {
      requestContext.GetService<DataspaceCacheService>().Synchronize<List<Dataspace>>(requestContext, (Func<List<Dataspace>>) (() =>
      {
        using (DataspaceComponent component = requestContext.CreateComponent<DataspaceComponent>())
          return component.UpdateDataspaces(dataspaceCategory, dataspaceIdentifier, newDatabaseId, newDataspaceState);
      }), (Action<DataspaceCacheData, List<Dataspace>>) ((cacheData, dataspaces) =>
      {
        foreach (Dataspace dataspace in dataspaces)
          cacheData.UpdateDataspace(requestContext, dataspace);
      }));
    }

    public void DeleteDataspace(
      IVssRequestContext requestContext,
      string dataspaceCategory,
      Guid dataspaceIdentifier)
    {
      this.ValidateCategory(dataspaceCategory, nameof (dataspaceCategory));
      Dataspace dataspace = this.QueryDataspace(requestContext, dataspaceCategory, dataspaceIdentifier);
      if (dataspace == null)
        return;
      requestContext.GetService<DataspaceCacheService>().Synchronize<Dataspace>(requestContext, (Func<Dataspace>) (() =>
      {
        using (DataspaceComponent component = requestContext.CreateComponent<DataspaceComponent>())
          component.DeleteDataspace(dataspaceIdentifier, dataspaceCategory);
        return dataspace;
      }), (Action<DataspaceCacheData, Dataspace>) ((cacheData, ds) => cacheData.RemoveDataspace(requestContext, ds)));
    }

    public virtual ISqlConnectionInfo GetConnectionInfo(
      IVssRequestContext requestContext,
      int dataspaceId)
    {
      return this.GetDatabaseProperties(requestContext, dataspaceId).SqlConnectionInfo;
    }

    public virtual ISqlConnectionInfo GetConnectionInfo(
      IVssRequestContext requestContext,
      string dataspaceCategory,
      Guid dataspaceIdentifier)
    {
      return this.GetDatabaseProperties(requestContext, dataspaceCategory, dataspaceIdentifier).SqlConnectionInfo;
    }

    public virtual ITeamFoundationDatabaseProperties GetDatabaseProperties(
      IVssRequestContext requestContext,
      int dataspaceId)
    {
      Dataspace dataspace = this.QueryDataspace(requestContext, dataspaceId);
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      return vssRequestContext.GetService<TeamFoundationDatabaseManagementService>().GetDatabase(vssRequestContext, dataspace.DatabaseId, true);
    }

    public virtual ITeamFoundationDatabaseProperties GetDatabaseProperties(
      IVssRequestContext requestContext,
      string dataspaceCategory,
      Guid dataspaceIdentifier)
    {
      int databaseId;
      if (dataspaceCategory == "Default" && dataspaceIdentifier == Guid.Empty)
      {
        if (requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
          return requestContext.ServiceHost.DeploymentServiceHost.DatabaseProperties;
        databaseId = !(requestContext is VssRequestContext) ? this.QueryDataspace(requestContext, dataspaceCategory, dataspaceIdentifier, true).DatabaseId : requestContext.ServiceHost.ServiceHostInternal().DatabaseId;
      }
      else
        databaseId = this.QueryDataspace(requestContext, dataspaceCategory, dataspaceIdentifier, true).DatabaseId;
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      return vssRequestContext.GetService<TeamFoundationDatabaseManagementService>().GetDatabase(vssRequestContext, databaseId, true);
    }
  }
}
