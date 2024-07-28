// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Indexer.TracerCICorrelationDetailsFactory
// Assembly: Microsoft.VisualStudio.Services.Search.Indexer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 167B1EA6-4D18-408E-89C6-597B8290976F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Indexer.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Entities.EntityProperties;
using Microsoft.VisualStudio.Services.Search.Common.Entities.EntityProperties.Code;
using Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry;
using System;
using System.Globalization;

namespace Microsoft.VisualStudio.Services.Search.Indexer
{
  public static class TracerCICorrelationDetailsFactory
  {
    public static ITracerCICorrelationDetails GetCICorrelationDetails(
      string correlationId,
      string operation,
      int jobTrigger)
    {
      string currentContext = "Collection";
      return (ITracerCICorrelationDetails) new TracerCICorrelationForDeploymentDetails(correlationId, currentContext)
      {
        Trigger = jobTrigger,
        Operation = operation
      };
    }

    public static ITracerCICorrelationDetails GetCICorrelationDetails(
      IVssRequestContext requestContext,
      string operation,
      int jobTrigger)
    {
      string correlationId = requestContext.ActivityId.ToString();
      string currentContext = "Collection";
      if (requestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection))
      {
        TracerIndexingUnitData dataForOrganization = TracerCICorrelationDetailsFactory.GetTracerIndexingUnitDataForOrganization(requestContext);
        TracerIndexingUnitData dataForCollection = TracerCICorrelationDetailsFactory.GetTracerIndexingUnitDataForCollection(requestContext);
        TracerCICorrelationForCollectionDetails correlationDetails = new TracerCICorrelationForCollectionDetails(correlationId, currentContext, dataForOrganization, dataForCollection);
        correlationDetails.Trigger = jobTrigger;
        correlationDetails.Operation = operation;
        return (ITracerCICorrelationDetails) correlationDetails;
      }
      if (!requestContext.ServiceHost.Is(TeamFoundationHostType.Application))
        return TracerCICorrelationDetailsFactory.GetCICorrelationDetails(correlationId, operation, jobTrigger);
      TracerIndexingUnitData dataForOrganization1 = TracerCICorrelationDetailsFactory.GetTracerIndexingUnitDataForOrganization(requestContext);
      TracerCICorrelationForOrganizationDetails correlationDetails1 = new TracerCICorrelationForOrganizationDetails(correlationId, currentContext, dataForOrganization1);
      correlationDetails1.Trigger = jobTrigger;
      correlationDetails1.Operation = operation;
      return (ITracerCICorrelationDetails) correlationDetails1;
    }

    public static ITracerCICorrelationDetails GetCICorrelationDetails(
      IVssRequestContext requestContext,
      string operation,
      int jobTrigger,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit,
      IIndexingUnitDataAccess indexingUnitDataAccess = null)
    {
      string correlationId = requestContext.ActivityId.ToString();
      ITracerCICorrelationDetails correlationDetails = (ITracerCICorrelationDetails) null;
      if (indexingUnitDataAccess == null)
        indexingUnitDataAccess = DataAccessFactory.GetInstance().GetIndexingUnitDataAccess();
      string indexingUnitType = indexingUnit.IndexingUnitType;
      if (indexingUnitType != null)
      {
        switch (indexingUnitType.Length)
        {
          case 7:
            if (indexingUnitType == "Project")
            {
              correlationDetails = (ITracerCICorrelationDetails) TracerCICorrelationDetailsFactory.GetTracerCICorrelationDetailsForProject(correlationId, indexingUnit, requestContext);
              break;
            }
            break;
          case 10:
            if (indexingUnitType == "Collection")
            {
              correlationDetails = (ITracerCICorrelationDetails) TracerCICorrelationDetailsFactory.GetTracerCICorrelationDetailsForCollection(correlationId, indexingUnit, requestContext);
              break;
            }
            break;
          case 12:
            if (indexingUnitType == "Organization")
            {
              correlationDetails = (ITracerCICorrelationDetails) TracerCICorrelationDetailsFactory.GetTracerCICorrelationDetailsForOrganization(correlationId, indexingUnit, requestContext);
              break;
            }
            break;
          case 14:
            if (indexingUnitType == "Git_Repository")
            {
              correlationDetails = (ITracerCICorrelationDetails) TracerCICorrelationDetailsFactory.GetTracerCICorrelationDetailsForRepository(correlationId, indexingUnit, requestContext, indexingUnitDataAccess);
              break;
            }
            break;
          case 15:
            if (indexingUnitType == "TFVC_Repository")
            {
              correlationDetails = (ITracerCICorrelationDetails) TracerCICorrelationDetailsFactory.GetTracerCICorrelationDetailsForRepository(correlationId, indexingUnit, requestContext, indexingUnitDataAccess);
              break;
            }
            break;
          case 16:
            if (indexingUnitType == "CustomRepository")
            {
              correlationDetails = (ITracerCICorrelationDetails) TracerCICorrelationDetailsFactory.GetTracerCICorrelationDetailsForRepository(correlationId, indexingUnit, requestContext, indexingUnitDataAccess);
              break;
            }
            break;
          case 18:
            if (indexingUnitType == "ScopedIndexingUnit")
            {
              correlationDetails = (ITracerCICorrelationDetails) TracerCICorrelationDetailsFactory.GetTracerCICorrelationDetailsForScopedIndexingUnit(correlationId, indexingUnit, requestContext, indexingUnitDataAccess);
              break;
            }
            break;
          case 21:
            if (indexingUnitType == "TemporaryIndexingUnit")
            {
              correlationDetails = (ITracerCICorrelationDetails) TracerCICorrelationDetailsFactory.GetTracerCICorrelationDetailsForTemporaryIndexingUnit(correlationId, indexingUnit, requestContext, indexingUnitDataAccess);
              break;
            }
            break;
        }
      }
      if (correlationDetails != null)
      {
        correlationDetails.Operation = operation;
        correlationDetails.Trigger = jobTrigger;
      }
      return correlationDetails;
    }

    public static ITracerCICorrelationDetails GetCICorrelationDetails(
      IVssRequestContext requestContext,
      ChangeEventData changeEventData,
      string operation,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit,
      IIndexingUnitDataAccess indexingUnitDataAccess = null)
    {
      string correlationId = changeEventData.CorrelationId;
      ITracerCICorrelationDetails correlationDetails = (ITracerCICorrelationDetails) null;
      if (indexingUnitDataAccess == null)
        indexingUnitDataAccess = DataAccessFactory.GetInstance().GetIndexingUnitDataAccess();
      string indexingUnitType = indexingUnit.IndexingUnitType;
      if (indexingUnitType != null)
      {
        switch (indexingUnitType.Length)
        {
          case 7:
            if (indexingUnitType == "Project")
            {
              correlationDetails = (ITracerCICorrelationDetails) TracerCICorrelationDetailsFactory.GetTracerCICorrelationDetailsForProject(correlationId, indexingUnit, requestContext);
              break;
            }
            break;
          case 10:
            if (indexingUnitType == "Collection")
            {
              correlationDetails = (ITracerCICorrelationDetails) TracerCICorrelationDetailsFactory.GetTracerCICorrelationDetailsForCollection(correlationId, indexingUnit, requestContext);
              break;
            }
            break;
          case 12:
            if (indexingUnitType == "Organization")
            {
              correlationDetails = (ITracerCICorrelationDetails) TracerCICorrelationDetailsFactory.GetTracerCICorrelationDetailsForOrganization(correlationId, indexingUnit, requestContext);
              break;
            }
            break;
          case 14:
            if (indexingUnitType == "Git_Repository")
            {
              correlationDetails = (ITracerCICorrelationDetails) TracerCICorrelationDetailsFactory.GetTracerCICorrelationDetailsForRepository(correlationId, indexingUnit, requestContext, indexingUnitDataAccess);
              break;
            }
            break;
          case 15:
            if (indexingUnitType == "TFVC_Repository")
            {
              correlationDetails = (ITracerCICorrelationDetails) TracerCICorrelationDetailsFactory.GetTracerCICorrelationDetailsForRepository(correlationId, indexingUnit, requestContext, indexingUnitDataAccess);
              break;
            }
            break;
          case 16:
            if (indexingUnitType == "CustomRepository")
            {
              correlationDetails = (ITracerCICorrelationDetails) TracerCICorrelationDetailsFactory.GetTracerCICorrelationDetailsForRepository(correlationId, indexingUnit, requestContext, indexingUnitDataAccess);
              break;
            }
            break;
          case 18:
            if (indexingUnitType == "ScopedIndexingUnit")
            {
              correlationDetails = (ITracerCICorrelationDetails) TracerCICorrelationDetailsFactory.GetTracerCICorrelationDetailsForScopedIndexingUnit(correlationId, indexingUnit, requestContext, indexingUnitDataAccess);
              break;
            }
            break;
          case 21:
            if (indexingUnitType == "TemporaryIndexingUnit")
            {
              correlationDetails = (ITracerCICorrelationDetails) TracerCICorrelationDetailsFactory.GetTracerCICorrelationDetailsForTemporaryIndexingUnit(correlationId, indexingUnit, requestContext, indexingUnitDataAccess);
              break;
            }
            break;
        }
      }
      if (correlationDetails != null)
      {
        correlationDetails.Operation = operation;
        correlationDetails.Trigger = changeEventData.Trigger;
        correlationDetails.TriggerTimeUtc = changeEventData.TriggerTimeUtc;
      }
      return correlationDetails;
    }

    private static TracerCICorrelationForRepositoryDetails GetTracerCICorrelationDetailsForRepository(
      string correlationId,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit,
      IVssRequestContext requestContext,
      IIndexingUnitDataAccess indexingUnitDataAccess)
    {
      string indexingUnitType = indexingUnit.IndexingUnitType;
      string repositoryName = indexingUnit.IndexingUnitId.ToString((IFormatProvider) CultureInfo.InvariantCulture);
      Guid tfsEntityId = indexingUnit.TFSEntityId;
      switch (indexingUnit.IndexingUnitType)
      {
        case "CustomRepository":
          repositoryName = (indexingUnit.TFSEntityAttributes as CustomRepoCodeTFSAttributes).RepositoryName;
          break;
        case "TFVC_Repository":
          repositoryName = (indexingUnit.TFSEntityAttributes as TfvcCodeRepoTFSAttributes).RepositoryName;
          break;
        case "Git_Repository":
          if (indexingUnit.TFSEntityAttributes is GitCodeRepoTFSAttributes)
          {
            repositoryName = (indexingUnit.TFSEntityAttributes as GitCodeRepoTFSAttributes).RepositoryName;
            break;
          }
          break;
      }
      TracerIndexingUnitData repositoryData = new TracerIndexingUnitData(repositoryName, tfsEntityId, indexingUnit.IndexingUnitId);
      TracerIndexingUnitData projectData = (TracerIndexingUnitData) null;
      if (indexingUnit.IndexingUnitType != "CustomRepository")
        projectData = TracerCICorrelationDetailsFactory.GetTracerIndexingUnitDataForProject(indexingUnit.ParentUnitId, requestContext, indexingUnitDataAccess);
      TracerIndexingUnitData dataForCollection = TracerCICorrelationDetailsFactory.GetTracerIndexingUnitDataForCollection(requestContext);
      TracerIndexingUnitData dataForOrganization = TracerCICorrelationDetailsFactory.GetTracerIndexingUnitDataForOrganization(requestContext);
      return new TracerCICorrelationForRepositoryDetails(correlationId, indexingUnitType, dataForOrganization, dataForCollection, projectData, repositoryData);
    }

    private static TracerCICorrelationForScopedIndexingUnitDetails GetTracerCICorrelationDetailsForScopedIndexingUnit(
      string correlationId,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit,
      IVssRequestContext requestContext,
      IIndexingUnitDataAccess indexingUnitDataAccess)
    {
      string indexingUnitType = indexingUnit.IndexingUnitType;
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit1 = indexingUnitDataAccess.GetIndexingUnit(requestContext, indexingUnit.ParentUnitId);
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit2 = indexingUnitDataAccess.GetIndexingUnit(requestContext, indexingUnit1.ParentUnitId);
      TracerIndexingUnitData scopedIndexingUnit = TracerCICorrelationDetailsFactory.GetTracerIndexingUnitDataForScopedIndexingUnit(indexingUnit);
      TracerIndexingUnitData dataForRepository = TracerCICorrelationDetailsFactory.GetTracerIndexingUnitDataForRepository(indexingUnit1);
      TracerIndexingUnitData unitDataForProject = TracerCICorrelationDetailsFactory.GetTracerIndexingUnitDataForProject(indexingUnit2);
      TracerIndexingUnitData dataForCollection = TracerCICorrelationDetailsFactory.GetTracerIndexingUnitDataForCollection(requestContext);
      TracerIndexingUnitData dataForOrganization = TracerCICorrelationDetailsFactory.GetTracerIndexingUnitDataForOrganization(requestContext);
      return new TracerCICorrelationForScopedIndexingUnitDetails(correlationId, indexingUnitType, dataForOrganization, dataForCollection, unitDataForProject, dataForRepository, scopedIndexingUnit);
    }

    private static TracerCICorrelationForTemporaryIndexingUnitDetails GetTracerCICorrelationDetailsForTemporaryIndexingUnit(
      string correlationId,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit,
      IVssRequestContext requestContext,
      IIndexingUnitDataAccess indexingUnitDataAccess)
    {
      string indexingUnitType = indexingUnit.IndexingUnitType;
      Guid tfsEntityId = indexingUnit.TFSEntityId;
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit1 = indexingUnitDataAccess.GetIndexingUnit(requestContext, indexingUnit.ParentUnitId);
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit2 = indexingUnitDataAccess.GetIndexingUnit(requestContext, indexingUnit1.ParentUnitId);
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit3 = indexingUnitDataAccess.GetIndexingUnit(requestContext, indexingUnit2.ParentUnitId);
      TracerIndexingUnitData tempIndexingUnitData = new TracerIndexingUnitData("TemporaryIndexingUnit", tfsEntityId, indexingUnit.IndexingUnitId);
      TracerIndexingUnitData scopedIndexingUnit = TracerCICorrelationDetailsFactory.GetTracerIndexingUnitDataForScopedIndexingUnit(indexingUnit1);
      TracerIndexingUnitData dataForRepository = TracerCICorrelationDetailsFactory.GetTracerIndexingUnitDataForRepository(indexingUnit2);
      TracerIndexingUnitData unitDataForProject = TracerCICorrelationDetailsFactory.GetTracerIndexingUnitDataForProject(indexingUnit3);
      TracerIndexingUnitData dataForCollection = TracerCICorrelationDetailsFactory.GetTracerIndexingUnitDataForCollection(requestContext);
      TracerIndexingUnitData dataForOrganization = TracerCICorrelationDetailsFactory.GetTracerIndexingUnitDataForOrganization(requestContext);
      return new TracerCICorrelationForTemporaryIndexingUnitDetails(correlationId, indexingUnitType, dataForOrganization, dataForCollection, unitDataForProject, dataForRepository, scopedIndexingUnit, tempIndexingUnitData);
    }

    private static TracerCICorrelationForProjectDetails GetTracerCICorrelationDetailsForProject(
      string correlationId,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit,
      IVssRequestContext requestContext)
    {
      string indexingUnitType = indexingUnit.IndexingUnitType;
      TracerIndexingUnitData unitDataForProject = TracerCICorrelationDetailsFactory.GetTracerIndexingUnitDataForProject(indexingUnit);
      TracerIndexingUnitData dataForCollection = TracerCICorrelationDetailsFactory.GetTracerIndexingUnitDataForCollection(requestContext);
      TracerIndexingUnitData dataForOrganization = TracerCICorrelationDetailsFactory.GetTracerIndexingUnitDataForOrganization(requestContext);
      return new TracerCICorrelationForProjectDetails(correlationId, indexingUnitType, dataForOrganization, dataForCollection, unitDataForProject);
    }

    private static TracerCICorrelationForCollectionDetails GetTracerCICorrelationDetailsForCollection(
      string correlationId,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit,
      IVssRequestContext requestContext)
    {
      string indexingUnitType = indexingUnit.IndexingUnitType;
      TracerIndexingUnitData dataForCollection = TracerCICorrelationDetailsFactory.GetTracerIndexingUnitDataForCollection(requestContext);
      TracerIndexingUnitData dataForOrganization = TracerCICorrelationDetailsFactory.GetTracerIndexingUnitDataForOrganization(requestContext);
      return new TracerCICorrelationForCollectionDetails(correlationId, indexingUnitType, dataForOrganization, dataForCollection);
    }

    private static TracerCICorrelationForOrganizationDetails GetTracerCICorrelationDetailsForOrganization(
      string correlationId,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit,
      IVssRequestContext requestContext)
    {
      string indexingUnitType = indexingUnit.IndexingUnitType;
      TracerIndexingUnitData dataForOrganization = TracerCICorrelationDetailsFactory.GetTracerIndexingUnitDataForOrganization(requestContext);
      return new TracerCICorrelationForOrganizationDetails(correlationId, indexingUnitType, dataForOrganization);
    }

    private static TracerIndexingUnitData GetTracerIndexingUnitDataForScopedIndexingUnit(
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit)
    {
      indexingUnit.IndexingUnitId.ToString((IFormatProvider) CultureInfo.InvariantCulture);
      Guid tfsEntityId = indexingUnit.TFSEntityId;
      return new TracerIndexingUnitData((indexingUnit.TFSEntityAttributes as ScopedGitRepositoryAttributes).ScopePath, tfsEntityId, indexingUnit.IndexingUnitId);
    }

    private static TracerIndexingUnitData GetTracerIndexingUnitDataForRepository(
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit)
    {
      string repositoryName = indexingUnit.IndexingUnitId.ToString((IFormatProvider) CultureInfo.InvariantCulture);
      Guid tfsEntityId = indexingUnit.TFSEntityId;
      switch (indexingUnit.IndexingUnitType)
      {
        case "CustomRepository":
          repositoryName = (indexingUnit.TFSEntityAttributes as CustomRepoCodeTFSAttributes).RepositoryName;
          break;
        case "TFVC_Repository":
          repositoryName = (indexingUnit.TFSEntityAttributes as TfvcCodeRepoTFSAttributes).RepositoryName;
          break;
        case "Git_Repository":
          repositoryName = (indexingUnit.TFSEntityAttributes as GitCodeRepoTFSAttributes).RepositoryName;
          break;
      }
      return new TracerIndexingUnitData(repositoryName, tfsEntityId, indexingUnit.IndexingUnitId);
    }

    private static TracerIndexingUnitData GetTracerIndexingUnitDataForProject(
      int indexingUnitId,
      IVssRequestContext requestContext,
      IIndexingUnitDataAccess indexingUnitDataAccess)
    {
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit = indexingUnitDataAccess.GetIndexingUnit(requestContext, indexingUnitId);
      return indexingUnit == null ? (TracerIndexingUnitData) null : TracerCICorrelationDetailsFactory.GetTracerIndexingUnitDataForProject(indexingUnit);
    }

    private static TracerIndexingUnitData GetTracerIndexingUnitDataForProject(
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit)
    {
      string name = string.Empty;
      if (indexingUnit.TFSEntityAttributes is ProjectCodeTFSAttributes)
        name = (indexingUnit.TFSEntityAttributes as ProjectCodeTFSAttributes).ProjectName;
      else if (indexingUnit.TFSEntityAttributes is ProjectWorkItemTFSAttributes)
        name = (indexingUnit.TFSEntityAttributes as ProjectWorkItemTFSAttributes).ProjectName;
      return new TracerIndexingUnitData(name, indexingUnit.TFSEntityId, indexingUnit.IndexingUnitId);
    }

    private static TracerIndexingUnitData GetTracerIndexingUnitDataForCollection(
      IVssRequestContext requestContext)
    {
      return new TracerIndexingUnitData(requestContext.GetCollectionName(), requestContext.GetCollectionID());
    }

    private static TracerIndexingUnitData GetTracerIndexingUnitDataForOrganization(
      IVssRequestContext requestContext)
    {
      return new TracerIndexingUnitData(requestContext.GetOrganizationName(), requestContext.GetOrganizationID());
    }
  }
}
