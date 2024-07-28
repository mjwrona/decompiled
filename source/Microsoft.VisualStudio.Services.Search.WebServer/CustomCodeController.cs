// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.WebServer.CustomCodeController
// Assembly: Microsoft.VisualStudio.Services.Search.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1112A012-BB03-4D21-B53E-3AFB00CCC7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.WebServer.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Entities;
using Microsoft.VisualStudio.Services.Search.Common.Entities.EntityProperties;
using Microsoft.VisualStudio.Services.Search.Common.Enums;
using Microsoft.VisualStudio.Services.Search.Index;
using Microsoft.VisualStudio.Services.Search.Indexer;
using Microsoft.VisualStudio.Services.Search.Query;
using Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Implementations;
using Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy;
using Microsoft.VisualStudio.Services.Search.WebApi;
using Microsoft.VisualStudio.Services.Search.WebApi.Legacy.Contracts.CustomRepository;
using Microsoft.VisualStudio.Services.WebApi.Internal;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Web.Http;
using System.Web.Http.Controllers;

namespace Microsoft.VisualStudio.Services.Search.WebServer
{
  [ClientIgnore]
  [VersionedApiControllerCustomName(Area = "search", ResourceName = "customCode")]
  public class CustomCodeController : SearchApiController
  {
    private ICustomCodeForwarder m_customCodeForwarder;

    public CustomCodeController()
    {
    }

    protected CustomCodeController(ICustomCodeForwarder customCodeForwarder) => this.m_customCodeForwarder = customCodeForwarder;

    [HttpPost]
    [ClientLocationId("06FE9117-FCFC-45A3-BDFE-77E19F1228B6")]
    public BulkCodeIndexResponse BulkCodeIndex(BulkCodeIndexRequest request)
    {
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.SetLogicalContext((IDiagnosticContext) new TfsDiagnosticContext(this.TfsRequestContext));
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceEnter(1080050, "REST-API", "REST-API", nameof (BulkCodeIndex));
      try
      {
        return this.HandleBulkCodeIndexRequest(this.TfsRequestContext, request);
      }
      finally
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceLeave(1080051, "REST-API", "REST-API", nameof (BulkCodeIndex));
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.ClearLogicalContext();
      }
    }

    [HttpGet]
    [ClientLocationId("D9875B42-48D0-4D6D-9075-52A09AD7E6BC")]
    public string GetFileContent(
      string projectName,
      string repositoryName,
      string branchName,
      string filePath)
    {
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.SetLogicalContext((IDiagnosticContext) new TfsDiagnosticContext(this.TfsRequestContext));
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceEnter(1080050, "REST-API", "REST-API", nameof (GetFileContent));
      try
      {
        return this.HandleGetFileContent(this.TfsRequestContext, projectName, repositoryName, branchName, filePath);
      }
      finally
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceLeave(1080051, "REST-API", "REST-API", nameof (GetFileContent));
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.ClearLogicalContext();
      }
    }

    [HttpGet]
    [ClientLocationId("74A47C72-DABB-411A-BA97-8B9FA5694709")]
    public OperationStatus GetOperationStatus(
      string projectName,
      string repositoryName,
      string branchName,
      string trackingId)
    {
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.SetLogicalContext((IDiagnosticContext) new TfsDiagnosticContext(this.TfsRequestContext));
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceEnter(1080050, "REST-API", "REST-API", nameof (GetOperationStatus));
      try
      {
        return this.HandleGetOperationStatus(this.TfsRequestContext, projectName, repositoryName, branchName, trackingId);
      }
      finally
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceLeave(1080051, "REST-API", "REST-API", nameof (GetOperationStatus));
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.ClearLogicalContext();
      }
    }

    [HttpPost]
    [ClientLocationId("531DF8DF-6DDF-4EDA-A4D3-84785E1518F6")]
    public FilesMetadataResponse PostFilesMetadata(
      string projectName,
      string repositoryName,
      string branchName,
      FilesMetadataRequest filesMetadataRequest)
    {
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.SetLogicalContext((IDiagnosticContext) new TfsDiagnosticContext(this.TfsRequestContext));
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceEnter(1080050, "REST-API", "REST-API", nameof (PostFilesMetadata));
      try
      {
        IVssRequestContext tfsRequestContext = this.TfsRequestContext;
        string projectName1 = projectName;
        string repositoryName1 = repositoryName;
        List<string> branchNamesList;
        if (!string.IsNullOrWhiteSpace(branchName))
          branchNamesList = new List<string>()
          {
            branchName
          };
        else
          branchNamesList = (List<string>) null;
        FilesMetadataRequest filesMetadataRequest1 = filesMetadataRequest;
        return this.HandleFilesMetadata(tfsRequestContext, projectName1, repositoryName1, branchNamesList, filesMetadataRequest1);
      }
      finally
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceLeave(1080051, "REST-API", "REST-API", nameof (PostFilesMetadata));
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.ClearLogicalContext();
      }
    }

    protected BulkCodeIndexResponse HandleBulkCodeIndexRequest(
      IVssRequestContext requestContext,
      BulkCodeIndexRequest request)
    {
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishKpiAndCi("NoOfIndexRequests", "Query Pipeline", 1.0);
      BulkCodeIndexResponse codeIndexResponse;
      try
      {
        if (!requestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection))
          throw new UnsupportedHostTypeException(requestContext.ServiceHost.HostType);
        if (!requestContext.GetConfigValue<bool>("/Service/ALMSearch/Settings/EnableCustomRepository", TeamFoundationHostType.ProjectCollection))
          throw new NotSupportedException(FormattableString.Invariant(FormattableStringFactory.Create("Registry setting '{0}' is not set to 'true' for this account.", (object) "/Service/ALMSearch/Settings/EnableCustomRepository")));
        IEnumerable<ErrorData> source = this.ValidateBulkCodeIndexRequest(request);
        if (source.Any<ErrorData>())
        {
          codeIndexResponse = new BulkCodeIndexResponse()
          {
            Accepted = false,
            Errors = source
          };
        }
        else
        {
          Stopwatch stopwatch = Stopwatch.StartNew();
          Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceVerbose(1080043, "REST-API", "REST-API", request.ToString());
          codeIndexResponse = this.m_customCodeForwarder.ForwardBulkIndexRequest(requestContext, request);
          Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceVerbose(1080043, "REST-API", "REST-API", codeIndexResponse.ToString());
          stopwatch.Stop();
          Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishKpiAndCi("NoOfFilesInIndexRequests", "Query Pipeline", (double) request.FileDetail.Count);
          Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishKpiAndCi("CustomBulkCodeIndexTime", "Query Pipeline", (double) stopwatch.ElapsedMilliseconds);
        }
      }
      catch (Exception ex)
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishKpiAndCi("NoOfFailedIndexRequests", "Query Pipeline", 1.0);
        switch (ex)
        {
          case UnsupportedHostTypeException _:
          case ArgumentNullException _:
          case NotSupportedException _:
          case InvalidCustomRequestException _:
            Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceWarning(1080052, "REST-API", "REST-API", ex.Message);
            break;
          default:
            Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceException(1080052, "REST-API", "REST-API", ex);
            break;
        }
        throw new InvalidCustomRequestException(ex.Message, ex);
      }
      return codeIndexResponse;
    }

    protected string HandleGetFileContent(
      IVssRequestContext requestContext,
      string projectName,
      string repositoryName,
      string branchName,
      string filePath)
    {
      string fileContentRequest;
      try
      {
        if (!requestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection))
          throw new UnsupportedHostTypeException(requestContext.ServiceHost.HostType);
        if (!requestContext.GetConfigValue<bool>("/Service/ALMSearch/Settings/EnableCustomRepository", TeamFoundationHostType.ProjectCollection))
          throw new NotSupportedException(FormattableString.Invariant(FormattableStringFactory.Create("Registry setting '{0}' is not set to 'true' for this account.", (object) "/Service/ALMSearch/Settings/EnableCustomRepository")));
        Stopwatch stopwatch = Stopwatch.StartNew();
        fileContentRequest = this.m_customCodeForwarder.ForwardGetFileContentRequest(requestContext, projectName, repositoryName, branchName, filePath);
        stopwatch.Stop();
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishKpiAndCi("GetCustomFileContentTime", "Query Pipeline", (double) stopwatch.ElapsedMilliseconds);
      }
      catch (Exception ex)
      {
        switch (ex)
        {
          case UnsupportedHostTypeException _:
          case ArgumentNullException _:
          case NotSupportedException _:
          case InvalidCustomRequestException _:
            Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceWarning(1080052, "REST-API", "REST-API", ex.Message);
            break;
          default:
            Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceException(1080052, "REST-API", "REST-API", ex);
            break;
        }
        throw new InvalidCustomRequestException(ex.Message, ex);
      }
      return fileContentRequest;
    }

    protected OperationStatus HandleGetOperationStatus(
      IVssRequestContext requestContext,
      string projectName,
      string repositoryName,
      string branchName,
      string trackingId)
    {
      OperationStatus operationStatusRequest;
      try
      {
        if (!requestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection))
          throw new UnsupportedHostTypeException(requestContext.ServiceHost.HostType);
        if (!requestContext.GetConfigValue<bool>("/Service/ALMSearch/Settings/EnableCustomRepository", TeamFoundationHostType.ProjectCollection))
          throw new NotSupportedException(FormattableString.Invariant(FormattableStringFactory.Create("Registry setting '{0}' is not set to 'true' for this account.", (object) "/Service/ALMSearch/Settings/EnableCustomRepository")));
        Stopwatch stopwatch = Stopwatch.StartNew();
        operationStatusRequest = this.m_customCodeForwarder.ForwardGetOperationStatusRequest(requestContext, projectName, repositoryName, branchName, trackingId);
        stopwatch.Stop();
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishKpiAndCi("GetCustomOperationStatusTime", "Query Pipeline", (double) stopwatch.ElapsedMilliseconds);
      }
      catch (Exception ex)
      {
        switch (ex)
        {
          case UnsupportedHostTypeException _:
          case ArgumentNullException _:
          case NotSupportedException _:
          case InvalidCustomRequestException _:
            Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceWarning(1080052, "REST-API", "REST-API", ex.Message);
            break;
          default:
            Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceException(1080052, "REST-API", "REST-API", ex);
            break;
        }
        throw new InvalidCustomRequestException(ex.Message, ex);
      }
      return operationStatusRequest;
    }

    protected FilesMetadataResponse HandleFilesMetadata(
      IVssRequestContext requestContext,
      string projectName,
      string repositoryName,
      List<string> branchNamesList,
      FilesMetadataRequest filesMetadataRequest)
    {
      FilesMetadataResponse metadataResponse;
      try
      {
        if (!requestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection))
          throw new UnsupportedHostTypeException(requestContext.ServiceHost.HostType);
        if (!requestContext.GetConfigValue<bool>("/Service/ALMSearch/Settings/EnableCustomRepository", TeamFoundationHostType.ProjectCollection))
          throw new NotSupportedException(FormattableString.Invariant(FormattableStringFactory.Create("Registry setting '{0}' is not set to 'true' for this account.", (object) "/Service/ALMSearch/Settings/EnableCustomRepository")));
        IEnumerable<IndexInfo> indexInfo = (IEnumerable<IndexInfo>) new List<IndexInfo>();
        IDocumentContractTypeService service = requestContext.GetService<IDocumentContractTypeService>();
        Stopwatch stopwatch;
        DocumentContractType contractType;
        if (filesMetadataRequest.CustomIndexingMode.Equals((object) CustomIndexingMode.ReindexingShadow))
        {
          Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit = DataAccessFactory.GetInstance().GetIndexingUnitDataAccess().GetIndexingUnit(requestContext, requestContext.GetCollectionID(), "Collection", (IEntityType) CodeEntityType.GetInstance());
          if (indexingUnit?.Properties is CollectionIndexingProperties properties)
            indexInfo = (IEnumerable<IndexInfo>) properties.IndexIndices;
          stopwatch = Stopwatch.StartNew();
          contractType = service.GetSupportedIndexDocumentContractTypeDuringZLRI(requestContext, indexingUnit, true);
        }
        else
        {
          indexInfo = new IndexMapper((IEntityType) CodeEntityType.GetInstance()).GetIndexInfo(requestContext);
          stopwatch = Stopwatch.StartNew();
          contractType = service.GetSupportedQueryDocumentContractType(requestContext, (IEntityType) CodeEntityType.GetInstance());
        }
        metadataResponse = this.m_customCodeForwarder.ForwardFilesMetadataRequest(requestContext, indexInfo, projectName, repositoryName, branchNamesList, filesMetadataRequest, contractType);
        stopwatch.Stop();
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishKpiAndCi("GetCustomFilesMetadataTime", "Query Pipeline", (double) stopwatch.ElapsedMilliseconds);
      }
      catch (Exception ex)
      {
        switch (ex)
        {
          case UnsupportedHostTypeException _:
          case ArgumentNullException _:
          case NotSupportedException _:
          case InvalidCustomRequestException _:
            Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceWarning(1080052, "REST-API", "REST-API", ex.Message);
            break;
          default:
            Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceException(1080052, "REST-API", "REST-API", ex);
            break;
        }
        throw new InvalidCustomRequestException(ex.Message, ex);
      }
      return metadataResponse;
    }

    protected override void InitializeInternal(HttpControllerContext controllerContext)
    {
      base.InitializeInternal(controllerContext);
      if (this.m_customCodeForwarder != null)
        return;
      this.m_customCodeForwarder = (ICustomCodeForwarder) new CustomCodeForwarder(new IndexMapper((IEntityType) CodeEntityType.GetInstance()).GetESConnectionString(this.TfsRequestContext), this.TfsRequestContext.GetElasticsearchPlatformSettings("/Service/ALMSearch/Settings/CustomSearchPlatformSettings", "ConnectionTimeout=180"), SearchOptions.Faceting, this.TfsRequestContext.ExecutionEnvironment.IsOnPremisesDeployment);
    }

    private IEnumerable<ErrorData> ValidateBulkCodeIndexRequest(BulkCodeIndexRequest request)
    {
      IList<ErrorData> errorDataList = (IList<ErrorData>) new List<ErrorData>();
      if (request == null)
      {
        errorDataList.Add(new ErrorData()
        {
          ErrorCode = "InvalidRequest",
          ErrorType = ErrorType.Error,
          ErrorMessage = FormattableString.Invariant(FormattableStringFactory.Create("The request is null."))
        });
      }
      else
      {
        if (string.IsNullOrWhiteSpace(request.ProjectName))
          errorDataList.Add(new ErrorData()
          {
            ErrorCode = "InvalidRequest",
            ErrorType = ErrorType.Error,
            ErrorMessage = FormattableString.Invariant(FormattableStringFactory.Create("Invalid request: Project name details not available."))
          });
        if (string.IsNullOrWhiteSpace(request.RepositoryName))
          errorDataList.Add(new ErrorData()
          {
            ErrorCode = "InvalidRequest",
            ErrorType = ErrorType.Error,
            ErrorMessage = FormattableString.Invariant(FormattableStringFactory.Create("Invalid request: Repository name details not available."))
          });
        if (request.FileDetail == null)
        {
          errorDataList.Add(new ErrorData()
          {
            ErrorCode = "InvalidRequest",
            ErrorType = ErrorType.Error,
            ErrorMessage = FormattableString.Invariant(FormattableStringFactory.Create("Invalid request: File details not available."))
          });
        }
        else
        {
          foreach (FileDetail fileDetail in request.FileDetail)
          {
            if (string.IsNullOrWhiteSpace(fileDetail.Path))
            {
              errorDataList.Add(new ErrorData()
              {
                ErrorCode = "InvalidRequest",
                ErrorType = ErrorType.Error,
                ErrorMessage = FormattableString.Invariant(FormattableStringFactory.Create("Invalid request: One of the file detail does not contain path."))
              });
              break;
            }
            if (fileDetail.Branches == null || fileDetail.Branches.Count<string>() == 0)
            {
              errorDataList.Add(new ErrorData()
              {
                ErrorCode = "InvalidRequest",
                ErrorType = ErrorType.Error,
                ErrorMessage = FormattableString.Invariant(FormattableStringFactory.Create("Invalid request: One of the file detail does not contain branches."))
              });
              break;
            }
          }
        }
      }
      return (IEnumerable<ErrorData>) errorDataList;
    }
  }
}
