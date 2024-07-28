// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Client.TeamProjectCollectionService
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.Client.Internal;
using Microsoft.TeamFoundation.Common.Internal;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Threading;
using System.Xml;

namespace Microsoft.TeamFoundation.Framework.Client
{
  internal class TeamProjectCollectionService : 
    ITeamProjectCollectionService,
    ITeamProjectCollectionServicing
  {
    private TeamProjectCollectionWebService m_service;
    private XmlReaderSettings m_xmlReaderSettings;
    private static readonly int s_jobPollingInterval = 5000;

    public TeamProjectCollectionService(TfsConfigurationServer application)
    {
      this.m_service = new TeamProjectCollectionWebService(application);
      this.m_xmlReaderSettings = new XmlReaderSettings();
      this.m_xmlReaderSettings.IgnoreComments = true;
      this.m_xmlReaderSettings.IgnoreProcessingInstructions = true;
      this.m_xmlReaderSettings.IgnoreWhitespace = true;
      this.m_xmlReaderSettings.DtdProcessing = DtdProcessing.Prohibit;
      this.m_xmlReaderSettings.XmlResolver = (XmlResolver) null;
    }

    public IList<TeamProjectCollection> GetCollections(IEnumerable<Guid> collectionIds) => this.GetCollections(collectionIds, ServiceHostFilterFlags.None);

    public IList<TeamProjectCollection> GetCollections(
      IEnumerable<Guid> collectionIds,
      ServiceHostFilterFlags filterFlags)
    {
      TeamProjectCollectionProperties[] collectionProperties = this.m_service.GetCollectionProperties(collectionIds, filterFlags);
      TeamProjectCollection[] collections = new TeamProjectCollection[collectionProperties.Length];
      for (int index = 0; index < collectionProperties.Length; ++index)
      {
        if (collectionProperties[index] != null)
          collections[index] = new TeamProjectCollection(this.m_service, collectionProperties[index]);
      }
      return (IList<TeamProjectCollection>) collections;
    }

    public TeamProjectCollection GetCollection(Guid collectionId) => this.GetCollection(collectionId, ServiceHostFilterFlags.None);

    public TeamProjectCollection GetCollection(
      Guid collectionId,
      ServiceHostFilterFlags filterFlags)
    {
      return this.GetCollections((IEnumerable<Guid>) new Guid[1]
      {
        collectionId
      }, filterFlags)[0] ?? throw new CollectionDoesNotExistException(TFCommonResources.CollectionDoesNotExist((object) collectionId));
    }

    public IList<TeamProjectCollection> GetCollections() => this.GetCollections(ServiceHostFilterFlags.None);

    public IList<TeamProjectCollection> GetCollections(ServiceHostFilterFlags filterFlags)
    {
      IList<TeamProjectCollection> collections1 = this.GetCollections((IEnumerable<Guid>) null, filterFlags);
      List<TeamProjectCollection> collections2 = new List<TeamProjectCollection>(collections1.Count);
      foreach (TeamProjectCollection projectCollection in (IEnumerable<TeamProjectCollection>) collections1)
      {
        if (projectCollection != null)
          collections2.Add(projectCollection);
      }
      return (IList<TeamProjectCollection>) collections2;
    }

    public TeamProjectCollection GetDefaultCollection()
    {
      TeamProjectCollection defaultCollection = (TeamProjectCollection) null;
      Guid defaultCollectionId = this.m_service.GetDefaultCollectionId();
      if (!defaultCollectionId.Equals(Guid.Empty))
      {
        try
        {
          defaultCollection = this.GetCollection(defaultCollectionId);
        }
        catch (Exception ex)
        {
        }
      }
      return defaultCollection;
    }

    public TeamProjectCollection WaitForCollectionServicingToComplete(
      ServicingJobDetail jobDetail,
      TimeSpan timeout)
    {
      TeamProjectCollection collection = this.GetCollection(jobDetail.HostId, ServiceHostFilterFlags.IncludeAllServicingDetails);
      ServicingJobDetail servicingJobDetail = (ServicingJobDetail) null;
      Stopwatch stopwatch = Stopwatch.StartNew();
      bool flag = true;
      while (!(stopwatch.Elapsed > timeout))
      {
        if (flag)
        {
          flag = false;
        }
        else
        {
          Thread.Sleep(TeamProjectCollectionService.s_jobPollingInterval);
          collection.Refresh();
        }
        foreach (ServicingJobDetail servicingDetail in collection.ServicingDetails)
        {
          if (servicingDetail.JobId == jobDetail.JobId)
          {
            servicingJobDetail = servicingDetail;
            break;
          }
        }
        if (servicingJobDetail == null || servicingJobDetail.JobStatus != ServicingJobStatus.Queued && servicingJobDetail.JobStatus != ServicingJobStatus.Running)
        {
          if (servicingJobDetail.Result == ServicingJobResult.Failed || servicingJobDetail.JobStatus == ServicingJobStatus.Failed)
            throw new CollectionServicingJobDidNotSucceedException(ClientResources.CollectionServicingJobDidNotSucceed());
          return collection;
        }
      }
      throw new TimeoutException(ClientResources.WaitForCollectionServicingTimeout());
    }

    public TeamProjectCollection WaitForCollectionServicingToComplete(ServicingJobDetail jobDetail) => this.WaitForCollectionServicingToComplete(jobDetail, TimeSpan.MaxValue);

    public ServicingJobDetail QueueCreateCollection(
      string name,
      string description,
      bool isDefault,
      string virtualDirectory,
      TeamFoundationServiceHostStatus state,
      IDictionary<string, string> servicingTokens,
      string dataTierConnectionString,
      string defaultConnectionString,
      IDictionary<string, string> databaseCategoryConnectionStrings)
    {
      return this.m_service.QueueCreateCollection(new TeamProjectCollectionProperties(Guid.Empty, name, description, isDefault, virtualDirectory, state, servicingTokens, defaultConnectionString, databaseCategoryConnectionStrings), dataTierConnectionString);
    }

    public ServicingJobDetail QueueCreateCollection(
      string name,
      string description,
      bool isDefault,
      string virtualDirectory,
      TeamFoundationServiceHostStatus state,
      IDictionary<string, string> servicingTokens)
    {
      return this.QueueCreateCollection(name, description, isDefault, virtualDirectory, state, servicingTokens, (string) null, (string) null, (IDictionary<string, string>) null);
    }

    public ServicingJobDetail QueueAttachCollection(
      string databaseConnectionString,
      IDictionary<string, string> servicingTokens,
      bool cloneCollection)
    {
      return this.QueueAttachCollection(databaseConnectionString, servicingTokens, cloneCollection, (string) null, (string) null, (string) null);
    }

    public ServicingJobDetail QueueAttachCollection(
      string databaseConnectionString,
      IDictionary<string, string> servicingTokens,
      bool cloneCollection,
      string name,
      string description,
      string virtualDirectory)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(databaseConnectionString, nameof (databaseConnectionString));
      try
      {
        SqlConnectionStringBuilder connectionStringBuilder = new SqlConnectionStringBuilder(databaseConnectionString);
      }
      catch (ArgumentException ex)
      {
        throw new ArgumentException(ex.Message, nameof (databaseConnectionString));
      }
      TeamProjectCollectionProperties collectionProperties = new TeamProjectCollectionProperties(Guid.Empty, name, description, false, virtualDirectory, TeamFoundationServiceHostStatus.Starting, servicingTokens, (string) null, (IDictionary<string, string>) null);
      collectionProperties.SetDatabaseCategoryConnectionStrings((IDictionary<string, string>) new Dictionary<string, string>((IEqualityComparer<string>) VssStringComparer.DatabaseCategory)
      {
        {
          "Framework",
          databaseConnectionString
        }
      });
      return this.m_service.QueueAttachCollection(collectionProperties, cloneCollection);
    }

    public ServicingJobDetail QueueDetachCollection(
      Guid collectionId,
      IDictionary<string, string> servicingTokens,
      string collectionStoppedMessage,
      out string detachedConnectionString)
    {
      ArgumentUtility.CheckForEmptyGuid(collectionId, nameof (collectionId));
      return this.m_service.QueueDetachCollection(new TeamProjectCollectionProperties(collectionId, string.Empty, string.Empty, false, string.Empty, TeamFoundationServiceHostStatus.Starting, servicingTokens, (string) null, (IDictionary<string, string>) null), collectionStoppedMessage, out detachedConnectionString);
    }

    public ServicingJobDetail QueueDetachCollection(
      TeamProjectCollection teamProjectCollection,
      IDictionary<string, string> servicingTokens,
      string collectionStoppedMessage,
      out string detachedConnectionString)
    {
      ArgumentUtility.CheckForNull<TeamProjectCollection>(teamProjectCollection, nameof (teamProjectCollection));
      return this.QueueDetachCollection(teamProjectCollection.Id, servicingTokens, collectionStoppedMessage, out detachedConnectionString);
    }

    public ServicingJobDetail DeleteProject(
      Guid collectionId,
      string projectUri,
      Dictionary<string, string> servicingTokens)
    {
      ServicingJobDetail servicingJobDetail = this.QueueDeleteProject(collectionId, projectUri, (IDictionary<string, string>) servicingTokens);
      TeamProjectCollection collection = this.GetCollection(collectionId);
      while (collection.IsBeingServiced)
      {
        Thread.Sleep(TeamProjectCollectionService.s_jobPollingInterval);
        collection.Refresh();
      }
      return servicingJobDetail;
    }

    public ServicingJobDetail QueueDeleteProject(
      Guid collectionId,
      string projectUri,
      IDictionary<string, string> servicingTokens)
    {
      ArgumentUtility.CheckForEmptyGuid(collectionId, nameof (collectionId));
      return this.m_service.QueueDeleteProject(new TeamProjectCollectionProperties(collectionId, string.Empty, string.Empty, false, string.Empty, TeamFoundationServiceHostStatus.Starting, servicingTokens, (string) null, (IDictionary<string, string>) null), projectUri);
    }
  }
}
