// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer.Sql.CustomRepositoryDataAccess
// Assembly: Microsoft.VisualStudio.Services.Search.Server.DataAccess, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3B684226-797D-4C9F-9AC1-E10D39E316D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Server.DataAccess.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer.Sql.Common;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer.Sql
{
  internal class CustomRepositoryDataAccess : SqlAzureDataAccess, ICustomRepositoryDataAccess
  {
    public CustomRepositoryDataAccess()
    {
    }

    protected CustomRepositoryDataAccess(ITableAccessPlatform tableAccessPlatform)
      : base(tableAccessPlatform)
    {
    }

    public void AddRepository(IVssRequestContext requestContext, CustomRepositoryEntity repository)
    {
      this.ValidateNotEmptyGuid("CollectionId", repository.CollectionId);
      this.ValidateNotEmptyGuid("RepositoryId", repository.RepositoryId);
      this.ValidateNotNullOrWhiteSpace("ProjectName", repository.ProjectName);
      this.ValidateNotNullOrWhiteSpace("RepositoryName", repository.RepositoryName);
      Stopwatch stopwatch = new Stopwatch();
      try
      {
        using (ITable<CustomRepositoryEntity> table = this.GetTable(requestContext))
          this.InvokeTableOperation<CustomRepositoryEntity>((Func<CustomRepositoryEntity>) (() => table.Insert(repository)));
      }
      catch (Exception ex)
      {
        SqlAzureDataAccessUtility.WrapAndThrowException(ex);
      }
    }

    public void UpdateRepository(
      IVssRequestContext requestContext,
      CustomRepositoryEntity repository)
    {
      this.ValidateNotEmptyGuid("CollectionId", repository.CollectionId);
      this.ValidateNotEmptyGuid("RepositoryId", repository.RepositoryId);
      this.ValidateNotNullOrWhiteSpace("ProjectName", repository.ProjectName);
      this.ValidateNotNullOrWhiteSpace("RepositoryName", repository.RepositoryName);
      Stopwatch stopwatch = new Stopwatch();
      try
      {
        using (ITable<CustomRepositoryEntity> table = this.GetTable(requestContext))
          this.InvokeTableOperation<CustomRepositoryEntity>((Func<CustomRepositoryEntity>) (() => table.Update(repository)));
      }
      catch (Exception ex)
      {
        SqlAzureDataAccessUtility.WrapAndThrowException(ex);
      }
    }

    public IEnumerable<string> GetProjects(IVssRequestContext requestContext, Guid collectionId)
    {
      this.ValidateNotEmptyGuid(nameof (collectionId), collectionId);
      TableEntityFilterList filters = new TableEntityFilterList();
      filters.Add(new TableEntityFilter("CollectionId", "eq", collectionId.ToString()));
      using (ITable<CustomRepositoryEntity> table = this.GetTable(requestContext))
      {
        List<CustomRepositoryEntity> source = this.InvokeTableOperation<List<CustomRepositoryEntity>>((Func<List<CustomRepositoryEntity>>) (() => table.RetriveTableEntityList(-1, filters)));
        return (source != null ? source.Select<CustomRepositoryEntity, string>((Func<CustomRepositoryEntity, string>) (entity => entity.ProjectName)).Distinct<string>() : (IEnumerable<string>) null) ?? (IEnumerable<string>) new List<string>();
      }
    }

    public IEnumerable<string> GetRepositories(
      IVssRequestContext requestContext,
      Guid collectionId,
      string projectName)
    {
      this.ValidateNotEmptyGuid(nameof (collectionId), collectionId);
      this.ValidateNotNullOrWhiteSpace(nameof (projectName), projectName);
      TableEntityFilterList repoFilters = new TableEntityFilterList();
      repoFilters.Add(new TableEntityFilter("CollectionId", "eq", collectionId.ToString()));
      repoFilters.Add(new TableEntityFilter("ProjectName", "eq", projectName));
      IEnumerable<string> repositories = (IEnumerable<string>) new List<string>();
      using (ITable<CustomRepositoryEntity> table = this.GetTable(requestContext))
      {
        List<CustomRepositoryEntity> source = this.InvokeTableOperation<List<CustomRepositoryEntity>>((Func<List<CustomRepositoryEntity>>) (() => table.RetriveTableEntityList(-1, repoFilters)));
        if (source != null)
          repositories = source.Select<CustomRepositoryEntity, string>((Func<CustomRepositoryEntity, string>) (entity => entity.RepositoryName));
        return repositories;
      }
    }

    public CustomRepositoryEntity GetRepository(
      IVssRequestContext requestContext,
      Guid collectionId,
      string projectName,
      string repositoryName)
    {
      this.ValidateNotEmptyGuid(nameof (collectionId), collectionId);
      this.ValidateNotNullOrWhiteSpace(nameof (projectName), projectName);
      this.ValidateNotNullOrWhiteSpace(nameof (repositoryName), repositoryName);
      TableEntityFilterList repoFilters = new TableEntityFilterList();
      repoFilters.Add(new TableEntityFilter("CollectionId", "eq", collectionId.ToString()));
      repoFilters.Add(new TableEntityFilter("ProjectName", "eq", projectName));
      repoFilters.Add(new TableEntityFilter("RepositoryName", "eq", repositoryName));
      using (ITable<CustomRepositoryEntity> table = this.GetTable(requestContext))
      {
        List<CustomRepositoryEntity> repositoryEntityList = this.InvokeTableOperation<List<CustomRepositoryEntity>>((Func<List<CustomRepositoryEntity>>) (() => table.RetriveTableEntityList(-1, repoFilters)));
        return repositoryEntityList != null && repositoryEntityList.Count == 1 ? repositoryEntityList[0] : (CustomRepositoryEntity) null;
      }
    }

    public Guid GetRepositoryId(
      IVssRequestContext requestContext,
      Guid collectionId,
      string projectName,
      string repositoryName)
    {
      CustomRepositoryEntity repository = this.GetRepository(requestContext, collectionId, projectName, repositoryName);
      return repository != null ? repository.RepositoryId : Guid.Empty;
    }

    public void DeleteRepository(
      IVssRequestContext requestContext,
      Guid collectionId,
      Guid repositoryId)
    {
      this.ValidateNotEmptyGuid(nameof (collectionId), collectionId);
      this.ValidateNotEmptyGuid(nameof (repositoryId), repositoryId);
      using (CustomRepositoryInfoTableV2 component = requestContext.CreateComponent<CustomRepositoryInfoTableV2>())
        component.Delete(new CustomRepositoryEntity(collectionId, (string) null, (string) null, repositoryId, (string) null, (Func<string, Type, object>) null));
    }

    internal virtual ITable<CustomRepositoryEntity> GetTable(IVssRequestContext requestContext) => (ITable<CustomRepositoryEntity>) requestContext.CreateComponent<CustomRepositoryInfoTable>();
  }
}
