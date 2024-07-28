// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.CatalogComponent
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Location;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;

namespace Microsoft.TeamFoundation.Server.Core
{
  internal class CatalogComponent : TeamFoundationSqlResourceComponent
  {
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[6]
    {
      (IComponentCreator) new ComponentCreator<CatalogComponent>(1),
      (IComponentCreator) new ComponentCreator<CatalogComponent2>(2),
      (IComponentCreator) new ComponentCreator<CatalogComponent3>(3),
      (IComponentCreator) new ComponentCreator<CatalogComponent4>(4),
      (IComponentCreator) new ComponentCreator<CatalogComponent5>(5),
      (IComponentCreator) new ComponentCreator<CatalogComponent6>(6)
    }, "Catalog");
    private static readonly Dictionary<int, SqlExceptionFactory> s_sqlExceptionFactories = new Dictionary<int, SqlExceptionFactory>()
    {
      [800016] = new SqlExceptionFactory(typeof (InvalidCatalogSaveResourceException)),
      [800020] = new SqlExceptionFactory(typeof (InvalidCatalogDeleteNodeException)),
      [800040] = new SqlExceptionFactory(typeof (InvalidCatalogSaveNodeException)),
      [800046] = new SqlExceptionFactory(typeof (CatalogNodeDoesNotExistException))
    };
    private static readonly SqlMetaData[] typ_CatalogResourceTypeTable = new SqlMetaData[3]
    {
      new SqlMetaData("Identifier", SqlDbType.UniqueIdentifier),
      new SqlMetaData("DisplayName", SqlDbType.NVarChar, 256L),
      new SqlMetaData("Description", SqlDbType.NVarChar, -1L)
    };
    private static readonly SqlMetaData[] typ_CatalogResourceTable = new SqlMetaData[4]
    {
      new SqlMetaData("Identifier", SqlDbType.UniqueIdentifier),
      new SqlMetaData("Description", SqlDbType.NVarChar, 256L),
      new SqlMetaData("DisplayName", SqlDbType.NVarChar, -1L),
      new SqlMetaData("TypeIdentifier", SqlDbType.UniqueIdentifier)
    };
    private static readonly SqlMetaData[] typ_ServiceReferenceTable = new SqlMetaData[4]
    {
      new SqlMetaData("Identifier", SqlDbType.UniqueIdentifier),
      new SqlMetaData("Reference", SqlDbType.NVarChar, 256L),
      new SqlMetaData("ServiceIdentifier", SqlDbType.UniqueIdentifier),
      new SqlMetaData("ServiceType", SqlDbType.NVarChar, 256L)
    };
    private static readonly SqlMetaData[] typ_ResourcePropertyTable = new SqlMetaData[3]
    {
      new SqlMetaData("Identifier", SqlDbType.UniqueIdentifier),
      new SqlMetaData("Key", SqlDbType.NVarChar, 256L),
      new SqlMetaData("Value", SqlDbType.NVarChar, -1L)
    };
    private static readonly SqlMetaData[] typ_CatalogNodeTable = new SqlMetaData[5]
    {
      new SqlMetaData("FullPath", SqlDbType.NVarChar, -1L),
      new SqlMetaData("ResourceIdentifier", SqlDbType.UniqueIdentifier),
      new SqlMetaData("TypeIdentifier", SqlDbType.UniqueIdentifier),
      new SqlMetaData("IsDefault", SqlDbType.Bit),
      new SqlMetaData("DependenciesIncluded", SqlDbType.Bit)
    };
    private static readonly SqlMetaData[] typ_CatalogDependencyTable = new SqlMetaData[4]
    {
      new SqlMetaData("FullPath", SqlDbType.NVarChar, -1L),
      new SqlMetaData("Key", SqlDbType.NVarChar, -1L),
      new SqlMetaData("Value", SqlDbType.NVarChar, -1L),
      new SqlMetaData("IsSingleton", SqlDbType.Bit)
    };
    private static readonly SqlMetaData[] typ_CatalogNodeDeleteTable = new SqlMetaData[2]
    {
      new SqlMetaData("FullPath", SqlDbType.NVarChar, -1L),
      new SqlMetaData("Recursive", SqlDbType.Bit)
    };
    private static readonly SqlMetaData[] typ_CatalogPathSpecTable = new SqlMetaData[2]
    {
      new SqlMetaData("Path", SqlDbType.NVarChar, 260L),
      new SqlMetaData("Recursion", SqlDbType.Int)
    };

    public void SaveCatalogResourceTypes(IEnumerable<CatalogResourceType> resourceTypes)
    {
      this.PrepareStoredProcedure("prc_SaveCatalogResourceTypes");
      this.BindCatalogResourceTypeTable("@resourceTypeTable", resourceTypes);
      this.BindGuid("@authorId", this.Author);
      this.ExecuteNonQuery();
    }

    public ResultCollection QueryCatalogResourceTypes(IEnumerable<Guid> resourceTypeIdentifiers)
    {
      this.PrepareStoredProcedure("prc_QueryCatalogResourceTypes");
      this.BindGuidTable("@identifier", resourceTypeIdentifiers);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), "prc_QueryCatalogResourceTypes", this.RequestContext);
      resultCollection.AddBinder<CatalogResourceType>((ObjectBinder<CatalogResourceType>) new CatalogResourceTypeColumns());
      return resultCollection;
    }

    public virtual ResultCollection SaveCatalogChanges(
      IEnumerable<CatalogResource> resourceChanges,
      IEnumerable<CatalogNode> nodeChanges,
      IEnumerable<KeyValuePair<CatalogNode, bool>> nodeDeletes,
      IEnumerable<KeyValuePair<string, string>> nodeMoves,
      CatalogQueryOptions queryOptions,
      bool preview,
      bool allowAllDeletes)
    {
      string internalPath = this.GetInternalPath();
      this.PrepareStoredProcedure("prc_SaveCatalogChanges");
      this.BindCatalogResourceTable("@resources", resourceChanges);
      this.BindServiceReferenceTable("@serviceDefinitionRef", resourceChanges);
      this.BindResourcePropertyTable("@properties", resourceChanges);
      this.BindCatalogNodeTable("@nodes", nodeChanges, internalPath);
      this.BindCatalogDependencyTable("@nodeDependencies", nodeChanges, internalPath);
      this.BindKeyValuePairStringTable("@nodeMoves", (nodeMoves ?? Enumerable.Empty<KeyValuePair<string, string>>()).Select<KeyValuePair<string, string>, KeyValuePair<string, string>>((System.Func<KeyValuePair<string, string>, KeyValuePair<string, string>>) (strings => new KeyValuePair<string, string>(CatalogComponent.TranslateNodePath(strings.Key, CatalogRoots.OrganizationalPath, internalPath), CatalogComponent.TranslateNodePath(strings.Value, CatalogRoots.OrganizationalPath, internalPath)))));
      this.BindCatalogNodeDeleteTable("@nodeDeletes", nodeDeletes, internalPath);
      bool flag = (queryOptions & CatalogQueryOptions.ExpandDependencies) == CatalogQueryOptions.ExpandDependencies;
      this.BindBoolean("@expandDependencies", flag);
      this.BindBoolean("@includeParents", (queryOptions & CatalogQueryOptions.IncludeParents) == CatalogQueryOptions.IncludeParents);
      this.BindBoolean("@preview", preview);
      this.BindGuid("@namespaceId", FrameworkSecurity.CatalogNamespaceId);
      this.BindBoolean("@allowAllDeletes", allowAllDeletes);
      this.BindDataspaceId();
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), "prc_SaveCatalogChanges", this.RequestContext);
      resultCollection.AddBinder<CatalogResource>((ObjectBinder<CatalogResource>) new CatalogResourceColumns());
      resultCollection.AddBinder<CatalogNode>((ObjectBinder<CatalogNode>) new CatalogNodeColumns(false, internalPath));
      resultCollection.AddBinder<CatalogResource>((ObjectBinder<CatalogResource>) new CatalogResourceColumns());
      if (!preview)
      {
        resultCollection.AddBinder<CatalogResource>((ObjectBinder<CatalogResource>) new CatalogResourceColumns());
        resultCollection.AddBinder<CatalogServiceReference>((ObjectBinder<CatalogServiceReference>) new CatalogServiceReferenceColumns());
        resultCollection.AddBinder<CatalogNode>((ObjectBinder<CatalogNode>) new CatalogNodeColumns(flag, internalPath));
        if (flag)
          resultCollection.AddBinder<CatalogNodeDependency>((ObjectBinder<CatalogNodeDependency>) new CatalogNodeDependencyColumns(internalPath));
      }
      return resultCollection;
    }

    protected virtual void BindDataspaceId()
    {
    }

    public virtual ResultCollection QueryCatalogResources(
      IEnumerable<Guid> resourceIdentifiers,
      CatalogQueryOptions queryOptions)
    {
      string internalPath = this.GetInternalPath();
      bool flag = (queryOptions & CatalogQueryOptions.ExpandDependencies) == CatalogQueryOptions.ExpandDependencies;
      this.PrepareStoredProcedure("prc_QueryCatalogResources");
      this.BindGuidTable("@identifier", resourceIdentifiers);
      this.BindBoolean("@expandDependencies", flag);
      this.BindBoolean("@includeParents", (queryOptions & CatalogQueryOptions.IncludeParents) == CatalogQueryOptions.IncludeParents);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), "prc_QueryCatalogResources", this.RequestContext);
      resultCollection.AddBinder<CatalogResource>((ObjectBinder<CatalogResource>) new CatalogResourceColumns());
      resultCollection.AddBinder<CatalogServiceReference>((ObjectBinder<CatalogServiceReference>) new CatalogServiceReferenceColumns());
      resultCollection.AddBinder<CatalogNode>((ObjectBinder<CatalogNode>) new CatalogNodeColumns(flag, internalPath));
      if (flag)
        resultCollection.AddBinder<CatalogNodeDependency>((ObjectBinder<CatalogNodeDependency>) new CatalogNodeDependencyColumns(internalPath));
      return resultCollection;
    }

    public virtual ResultCollection QueryCatalogResourcesByTypeAndArtifactId(
      IEnumerable<Guid> resourceTypes,
      IEnumerable<int> artifactIds,
      CatalogQueryOptions queryOptions)
    {
      string internalPath = this.GetInternalPath();
      bool flag = (queryOptions & CatalogQueryOptions.ExpandDependencies) == CatalogQueryOptions.ExpandDependencies;
      this.PrepareStoredProcedure("prc_QueryCatalogResourcesByType");
      this.BindGuidTable("@resourceType", resourceTypes);
      this.BindInt32Table("@artifactIdFilter", artifactIds);
      this.BindBoolean("@expandDependencies", flag);
      this.BindBoolean("@includeParents", (queryOptions & CatalogQueryOptions.IncludeParents) == CatalogQueryOptions.IncludeParents);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), "prc_QueryCatalogResourcesByType", this.RequestContext);
      resultCollection.AddBinder<CatalogResource>((ObjectBinder<CatalogResource>) new CatalogResourceColumns());
      resultCollection.AddBinder<CatalogServiceReference>((ObjectBinder<CatalogServiceReference>) new CatalogServiceReferenceColumns());
      resultCollection.AddBinder<CatalogNode>((ObjectBinder<CatalogNode>) new CatalogNodeColumns(flag, internalPath));
      if (flag)
        resultCollection.AddBinder<CatalogNodeDependency>((ObjectBinder<CatalogNodeDependency>) new CatalogNodeDependencyColumns(internalPath));
      return resultCollection;
    }

    public virtual ResultCollection QueryCatalogNodes(
      IEnumerable<string> pathSpecs,
      IEnumerable<Guid> resourceTypeFilters,
      IEnumerable<int> artifactIdFilters,
      CatalogQueryOptions queryOptions)
    {
      string internalPath = this.GetInternalPath();
      if (!this.RequestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
        pathSpecs = (IEnumerable<string>) new CatalogComponent.CatalogPathSpecEnumerable(pathSpecs);
      bool flag = (queryOptions & CatalogQueryOptions.ExpandDependencies) == CatalogQueryOptions.ExpandDependencies;
      this.PrepareStoredProcedure("prc_QueryCatalogNodes");
      this.BindCatalogPathSpecTable("@pathSpecItems", pathSpecs, internalPath);
      this.BindGuidTable("@resourceTypeFilter", resourceTypeFilters);
      this.BindInt32Table("@artifactIdFilter", artifactIdFilters);
      this.BindBoolean("@expandDependencies", flag);
      this.BindBoolean("@includeParents", (queryOptions & CatalogQueryOptions.IncludeParents) == CatalogQueryOptions.IncludeParents);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), "prc_QueryCatalogNodes", this.RequestContext);
      resultCollection.AddBinder<CatalogResource>((ObjectBinder<CatalogResource>) new CatalogResourceColumns());
      resultCollection.AddBinder<CatalogServiceReference>((ObjectBinder<CatalogServiceReference>) new CatalogServiceReferenceColumns());
      resultCollection.AddBinder<CatalogNode>((ObjectBinder<CatalogNode>) new CatalogNodeColumns(flag, internalPath));
      if (flag)
        resultCollection.AddBinder<CatalogNodeDependency>((ObjectBinder<CatalogNodeDependency>) new CatalogNodeDependencyColumns(internalPath));
      return resultCollection;
    }

    internal ResultCollection QueryCatalogParents(
      Guid resourceIdentifier,
      IEnumerable<string> pathFilters,
      IEnumerable<Guid> resourceTypeFilters,
      bool recurseToRoot,
      CatalogQueryOptions queryOptions)
    {
      string internalPath = this.GetInternalPath();
      bool flag = (queryOptions & CatalogQueryOptions.ExpandDependencies) == CatalogQueryOptions.ExpandDependencies;
      this.PrepareStoredProcedure("prc_QueryCatalogParents");
      this.BindGuid("@resourceIdentifier", resourceIdentifier);
      this.BindStringTable("@pathFilters", (pathFilters ?? Enumerable.Empty<string>()).Select<string, string>((System.Func<string, string>) (pf => CatalogComponent.TranslateNodePath(pf, CatalogRoots.OrganizationalPath, internalPath))));
      this.BindGuidTable("@typeFilters", resourceTypeFilters);
      this.BindBoolean("@recurseToRoot", recurseToRoot);
      this.BindBoolean("@expandDependencies", flag);
      this.BindBoolean("@includeParents", (queryOptions & CatalogQueryOptions.IncludeParents) == CatalogQueryOptions.IncludeParents);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), "prc_QueryCatalogNodes", this.RequestContext);
      resultCollection.AddBinder<CatalogResource>((ObjectBinder<CatalogResource>) new CatalogResourceColumns());
      resultCollection.AddBinder<CatalogServiceReference>((ObjectBinder<CatalogServiceReference>) new CatalogServiceReferenceColumns());
      resultCollection.AddBinder<CatalogNode>((ObjectBinder<CatalogNode>) new CatalogNodeColumns(flag, internalPath));
      if (flag)
        resultCollection.AddBinder<CatalogNodeDependency>((ObjectBinder<CatalogNodeDependency>) new CatalogNodeDependencyColumns(internalPath));
      return resultCollection;
    }

    public ResultCollection QueryCatalogDependents(string path, CatalogQueryOptions queryOptions)
    {
      string internalPath = this.GetInternalPath();
      this.PrepareStoredProcedure("prc_QueryCatalogDependents");
      bool flag = (queryOptions & CatalogQueryOptions.ExpandDependencies) == CatalogQueryOptions.ExpandDependencies;
      path = CatalogComponent.TranslateNodePath(path, CatalogRoots.OrganizationalPath, internalPath);
      this.BindString("@path", path, 888, false, SqlDbType.VarChar);
      this.BindBoolean("@expandDependencies", flag);
      this.BindBoolean("@includeParents", (queryOptions & CatalogQueryOptions.IncludeParents) == CatalogQueryOptions.IncludeParents);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), "prc_QueryCatalogDependents", this.RequestContext);
      resultCollection.AddBinder<CatalogResource>((ObjectBinder<CatalogResource>) new CatalogResourceColumns());
      resultCollection.AddBinder<CatalogServiceReference>((ObjectBinder<CatalogServiceReference>) new CatalogServiceReferenceColumns());
      resultCollection.AddBinder<CatalogNode>((ObjectBinder<CatalogNode>) new CatalogNodeColumns(flag, internalPath));
      if (flag)
        resultCollection.AddBinder<CatalogNodeDependency>((ObjectBinder<CatalogNodeDependency>) new CatalogNodeDependencyColumns(internalPath));
      return resultCollection;
    }

    protected override IDictionary<int, SqlExceptionFactory> TranslatedExceptions => (IDictionary<int, SqlExceptionFactory>) CatalogComponent.s_sqlExceptionFactories;

    protected virtual string GetInternalPath() => this.RequestContext.ServiceHost.Is(TeamFoundationHostType.Application) ? Convert.ToBase64String(this.RequestContext.ServiceHost.InstanceId.ToByteArray()) : CatalogRoots.OrganizationalPath;

    internal static string TranslateNodePath(
      string catalogNodePath,
      string srcPath,
      string tgtPath)
    {
      ArgumentUtility.CheckForNull<string>(catalogNodePath, nameof (catalogNodePath));
      ArgumentUtility.CheckForNull<string>(srcPath, nameof (srcPath));
      ArgumentUtility.CheckForNull<string>(tgtPath, nameof (tgtPath));
      return catalogNodePath.StartsWith(srcPath, false, CultureInfo.InvariantCulture) ? tgtPath + catalogNodePath.Substring(srcPath.Length) : catalogNodePath;
    }

    protected SqlParameter BindCatalogResourceTypeTable(
      string parameterName,
      IEnumerable<CatalogResourceType> rows)
    {
      if (rows == null)
        rows = Enumerable.Empty<CatalogResourceType>();
      System.Func<CatalogResourceType, SqlDataRecord> selector = (System.Func<CatalogResourceType, SqlDataRecord>) (resourceType =>
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(CatalogComponent.typ_CatalogResourceTypeTable);
        sqlDataRecord.SetGuid(0, resourceType.Identifier);
        sqlDataRecord.SetString(1, resourceType.DisplayName);
        if (resourceType.Description != null)
          sqlDataRecord.SetString(2, resourceType.Description);
        else
          sqlDataRecord.SetDBNull(2);
        return sqlDataRecord;
      });
      return this.BindTable(parameterName, "typ_CatalogResourceTypeTable", rows.Select<CatalogResourceType, SqlDataRecord>(selector));
    }

    protected virtual SqlParameter BindCatalogResourceTable(
      string parameterName,
      IEnumerable<CatalogResource> rows)
    {
      if (rows == null)
        rows = Enumerable.Empty<CatalogResource>();
      System.Func<CatalogResource, SqlDataRecord> selector = (System.Func<CatalogResource, SqlDataRecord>) (resource =>
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(CatalogComponent.typ_CatalogResourceTable);
        Guid guid = resource.Identifier == Guid.Empty ? resource.TempCorrelationId : resource.Identifier;
        sqlDataRecord.SetGuid(0, guid);
        if (resource.Description != null)
          sqlDataRecord.SetString(1, resource.Description);
        else
          sqlDataRecord.SetDBNull(1);
        sqlDataRecord.SetString(2, resource.DisplayName);
        sqlDataRecord.SetGuid(3, resource.ResourceType.Identifier);
        return sqlDataRecord;
      });
      return this.BindTable(parameterName, "typ_CatalogResourceTable", rows.Select<CatalogResource, SqlDataRecord>(selector));
    }

    protected SqlParameter BindServiceReferenceTable(
      string parameterName,
      IEnumerable<CatalogResource> rows)
    {
      rows = rows ?? Enumerable.Empty<CatalogResource>();
      return this.BindTable(parameterName, "typ_ServiceReferenceTable", this.BindServiceReferenceRows(rows));
    }

    private IEnumerable<SqlDataRecord> BindServiceReferenceRows(
      IEnumerable<CatalogResource> resources)
    {
      foreach (CatalogResource resource in resources)
      {
        Guid identifier = resource.Identifier == Guid.Empty ? resource.TempCorrelationId : resource.Identifier;
        foreach (KeyValuePair<string, ServiceDefinition> serviceReference in (IEnumerable<KeyValuePair<string, ServiceDefinition>>) resource.ServiceReferences)
        {
          SqlDataRecord sqlDataRecord = new SqlDataRecord(CatalogComponent.typ_ServiceReferenceTable);
          sqlDataRecord.SetGuid(0, identifier);
          sqlDataRecord.SetString(1, serviceReference.Key);
          sqlDataRecord.SetGuid(2, serviceReference.Value.Identifier);
          sqlDataRecord.SetString(3, serviceReference.Value.ServiceType);
          yield return sqlDataRecord;
        }
        identifier = new Guid();
      }
    }

    protected SqlParameter BindResourcePropertyTable(
      string parameterName,
      IEnumerable<CatalogResource> rows)
    {
      rows = rows ?? Enumerable.Empty<CatalogResource>();
      return this.BindTable(parameterName, "typ_ResourcePropertyTable", this.BindResourcePropertyRows(rows));
    }

    private IEnumerable<SqlDataRecord> BindResourcePropertyRows(
      IEnumerable<CatalogResource> resources)
    {
      foreach (CatalogResource resource in resources)
      {
        Guid identifier = resource.Identifier == Guid.Empty ? resource.TempCorrelationId : resource.Identifier;
        foreach (KeyValuePair<string, string> property in (IEnumerable<KeyValuePair<string, string>>) resource.Properties)
        {
          SqlDataRecord sqlDataRecord = new SqlDataRecord(CatalogComponent.typ_ResourcePropertyTable);
          sqlDataRecord.SetGuid(0, identifier);
          sqlDataRecord.SetString(1, property.Key);
          if (property.Value != null)
            sqlDataRecord.SetString(2, property.Value);
          else
            sqlDataRecord.SetDBNull(2);
          yield return sqlDataRecord;
        }
        identifier = new Guid();
      }
    }

    protected SqlParameter BindCatalogNodeTable(
      string parameterName,
      IEnumerable<CatalogNode> rows,
      string internalPath)
    {
      rows = rows ?? Enumerable.Empty<CatalogNode>();
      System.Func<CatalogNode, SqlDataRecord> selector = (System.Func<CatalogNode, SqlDataRecord>) (node =>
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(CatalogComponent.typ_CatalogNodeTable);
        string catalogNodePath = string.IsNullOrEmpty(node.FullPath) ? node.ParentPath + node.ChildItem : node.FullPath;
        Guid guid = node.Resource.Identifier == Guid.Empty ? node.Resource.TempCorrelationId : node.Resource.Identifier;
        sqlDataRecord.SetString(0, CatalogComponent.TranslateNodePath(catalogNodePath, CatalogRoots.OrganizationalPath, internalPath));
        sqlDataRecord.SetGuid(1, guid);
        sqlDataRecord.SetGuid(2, node.Resource.ResourceType.Identifier);
        sqlDataRecord.SetBoolean(3, node.IsDefault);
        sqlDataRecord.SetBoolean(4, node.NodeDependenciesIncluded);
        return sqlDataRecord;
      });
      return this.BindTable(parameterName, "typ_CatalogNodeTable", rows.Select<CatalogNode, SqlDataRecord>(selector));
    }

    protected SqlParameter BindCatalogDependencyTable(
      string parameterName,
      IEnumerable<CatalogNode> rows,
      string internalPath)
    {
      rows = rows ?? Enumerable.Empty<CatalogNode>();
      return this.BindTable(parameterName, "typ_CatalogDependencyTable", this.BindCatalogDependencyRows(rows, internalPath));
    }

    private IEnumerable<SqlDataRecord> BindCatalogDependencyRows(
      IEnumerable<CatalogNode> rows,
      string internalPath)
    {
      foreach (CatalogNode node in rows)
      {
        string fullPath = !string.IsNullOrEmpty(node.FullPath) ? node.FullPath : node.ParentPath + node.ChildItem;
        fullPath = CatalogComponent.TranslateNodePath(fullPath, CatalogRoots.OrganizationalPath, internalPath);
        foreach (KeyValuePair<string, CatalogNode> singleton in (IEnumerable<KeyValuePair<string, CatalogNode>>) node.Dependencies.Singletons)
        {
          SqlDataRecord sqlDataRecord = new SqlDataRecord(CatalogComponent.typ_CatalogDependencyTable);
          string str = CatalogComponent.TranslateNodePath(singleton.Value.ParentPath + singleton.Value.ChildItem, CatalogRoots.OrganizationalPath, internalPath);
          sqlDataRecord.SetString(0, fullPath);
          sqlDataRecord.SetString(1, singleton.Key);
          sqlDataRecord.SetString(2, str);
          sqlDataRecord.SetBoolean(3, true);
          yield return sqlDataRecord;
        }
        foreach (KeyValuePair<string, IList<CatalogNode>> set in (IEnumerable<KeyValuePair<string, IList<CatalogNode>>>) node.Dependencies.Sets)
        {
          KeyValuePair<string, IList<CatalogNode>> dependencySet = set;
          foreach (CatalogNode catalogNode in (IEnumerable<CatalogNode>) dependencySet.Value)
          {
            SqlDataRecord sqlDataRecord = new SqlDataRecord(CatalogComponent.typ_CatalogDependencyTable);
            string str = CatalogComponent.TranslateNodePath(catalogNode.ParentPath + catalogNode.ChildItem, CatalogRoots.OrganizationalPath, internalPath);
            sqlDataRecord.SetString(0, fullPath);
            sqlDataRecord.SetString(1, dependencySet.Key);
            sqlDataRecord.SetString(2, str);
            sqlDataRecord.SetBoolean(3, false);
            yield return sqlDataRecord;
          }
          dependencySet = new KeyValuePair<string, IList<CatalogNode>>();
        }
        fullPath = (string) null;
      }
    }

    protected SqlParameter BindCatalogNodeDeleteTable(
      string parameterName,
      IEnumerable<KeyValuePair<CatalogNode, bool>> rows,
      string internalPath)
    {
      rows = rows ?? Enumerable.Empty<KeyValuePair<CatalogNode, bool>>();
      System.Func<KeyValuePair<CatalogNode, bool>, SqlDataRecord> selector = (System.Func<KeyValuePair<CatalogNode, bool>, SqlDataRecord>) (deleteInfo =>
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(CatalogComponent.typ_CatalogNodeDeleteTable);
        sqlDataRecord.SetString(0, CatalogComponent.TranslateNodePath(deleteInfo.Key.FullPath, CatalogRoots.OrganizationalPath, internalPath));
        sqlDataRecord.SetBoolean(1, deleteInfo.Value);
        return sqlDataRecord;
      });
      return this.BindTable(parameterName, "typ_CatalogNodeDeleteTable", rows.Select<KeyValuePair<CatalogNode, bool>, SqlDataRecord>(selector));
    }

    protected SqlParameter BindCatalogPathSpecTable(
      string parameterName,
      IEnumerable<string> rows,
      string internalPath)
    {
      rows = rows ?? Enumerable.Empty<string>();
      System.Func<string, SqlDataRecord> selector = (System.Func<string, SqlDataRecord>) (pathSpec =>
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(CatalogComponent.typ_CatalogPathSpecTable);
        string path;
        int recursion;
        CatalogPathSpec.ParsePathSpec(pathSpec, out path, out recursion);
        sqlDataRecord.SetString(0, CatalogComponent.TranslateNodePath(path, CatalogRoots.OrganizationalPath, internalPath));
        sqlDataRecord.SetInt32(1, recursion);
        return sqlDataRecord;
      });
      return this.BindTable(parameterName, "typ_CatalogPathSpecTable", rows.Select<string, SqlDataRecord>(selector));
    }

    private class CatalogPathSpecEnumerable : IEnumerable<string>, IEnumerable
    {
      private IEnumerable<string> m_pathSpecs;

      public CatalogPathSpecEnumerable(IEnumerable<string> pathSpecs) => this.m_pathSpecs = (IEnumerable<string>) ((object) pathSpecs ?? (object) Array.Empty<string>());

      public IEnumerator<string> GetEnumerator() => (IEnumerator<string>) new CatalogComponent.CatalogPathSpecEnumerable.CatalogPathSpecEnumerator(this.m_pathSpecs.GetEnumerator());

      IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) new CatalogComponent.CatalogPathSpecEnumerable.CatalogPathSpecEnumerator(this.m_pathSpecs.GetEnumerator());

      private class CatalogPathSpecEnumerator : IEnumerator<string>, IDisposable, IEnumerator
      {
        private IEnumerator<string> m_pathSpecs;
        private Stack<string> m_queuedSpec;
        private string m_currentSpec;

        public CatalogPathSpecEnumerator(IEnumerator<string> pathSpecs) => this.m_pathSpecs = pathSpecs;

        public string Current => this.m_currentSpec;

        public void Dispose() => this.m_pathSpecs.Dispose();

        bool IEnumerator.MoveNext() => this.MoveNext();

        void IEnumerator.Reset() => this.Reset();

        object IEnumerator.Current => (object) this.m_currentSpec;

        public bool MoveNext()
        {
          bool flag;
          if (this.m_queuedSpec != null && this.m_queuedSpec.Count > 0)
          {
            flag = true;
            this.m_currentSpec = this.m_queuedSpec.Pop();
          }
          else if (flag = this.m_pathSpecs.MoveNext())
          {
            this.m_currentSpec = this.m_pathSpecs.Current;
            if (this.m_currentSpec.Equals("...", StringComparison.OrdinalIgnoreCase))
            {
              if (this.m_queuedSpec == null)
                this.m_queuedSpec = new Stack<string>();
              this.m_queuedSpec.Push(CatalogRoots.InfrastructurePath + "...");
              this.m_queuedSpec.Push(CatalogRoots.InfrastructurePath);
              this.m_queuedSpec.Push(CatalogRoots.OrganizationalPath);
              this.m_currentSpec = CatalogRoots.OrganizationalPath + "...";
            }
          }
          return flag;
        }

        public void Reset()
        {
          this.m_queuedSpec = (Stack<string>) null;
          this.m_pathSpecs.Reset();
        }
      }
    }
  }
}
