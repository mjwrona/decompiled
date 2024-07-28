// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.PipelineCoverageService
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.Coverage.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  internal class PipelineCoverageService : 
    TeamFoundationTestManagementService,
    IPipelineCoverageService,
    IVssFrameworkService
  {
    private static readonly JsonSerializerSettings DefaultSerializerSettings = new JsonSerializerSettings()
    {
      ContractResolver = (IContractResolver) new CamelCasePropertyNamesContractResolver()
    };
    private PipelineCoverageStorageFactory _pipelineCoverageStorageFactory;
    private IPipelineCoverageStorage _pipelineCoverageStorage;

    public PipelineCoverageService()
    {
      this._pipelineCoverageStorageFactory = new PipelineCoverageStorageFactory();
      this._pipelineCoverageStorage = this._pipelineCoverageStorageFactory.GetPipelineCoverageStorage();
    }

    public IEnumerable<CoverageScope> GetCoverageScopes(
      TestManagementRequestContext tcmRequestContext,
      Guid projectId,
      int pipelineInstanceId,
      PipelineCoverageDataType pipelineCoverageDataType)
    {
      Dictionary<string, object> dictionary = new Dictionary<string, object>()
      {
        {
          "ProjectId",
          (object) projectId
        },
        {
          "PipelineInstanceId",
          (object) pipelineInstanceId
        },
        {
          "PipelineCoverageDataType",
          (object) pipelineCoverageDataType
        }
      };
      IEnumerable<CoverageScope> source = (IEnumerable<CoverageScope>) null;
      try
      {
        using (new SimpleTimer(tcmRequestContext.RequestContext, nameof (GetCoverageScopes), dictionary))
        {
          source = this._pipelineCoverageStorage.GetCoverageScopes(tcmRequestContext, projectId, pipelineInstanceId, pipelineCoverageDataType);
          dictionary.Add("CoverageScopesCount", (object) (source != null ? new int?(source.Count<CoverageScope>()) : new int?()));
        }
      }
      catch (Exception ex)
      {
        tcmRequestContext.Logger.Error(1015808, string.Format("Error while getting coverage scopes: {0}", (object) ex));
        dictionary.Add("Exception", (object) ex.Message);
        throw;
      }
      finally
      {
        CustomerIntelligenceData cid = new CustomerIntelligenceData((IDictionary<string, object>) dictionary);
        TelemetryLogger.Instance.PublishData(tcmRequestContext.RequestContext, nameof (GetCoverageScopes), cid);
      }
      return source;
    }

    public async Task<FileCoverageDetailsResult> GetFileCoverageDetailsAsync(
      TestManagementRequestContext tcmRequestContext,
      Guid projectId,
      int pipelineInstanceId,
      CoverageScope coverageScope,
      PipelineCoverageDataType pipelineCoverageDataType,
      CoverageDetailsFileType coverageDetailsFileType,
      string continuationToken)
    {
      Dictionary<string, object> ciData = new Dictionary<string, object>()
      {
        {
          "ProjectId",
          (object) projectId
        },
        {
          "PipelineInstanceId",
          (object) pipelineInstanceId
        },
        {
          "CoverageScope",
          (object) coverageScope.Name
        },
        {
          "PipelineCoverageDataType",
          (object) pipelineCoverageDataType
        }
      };
      FileCoverageDetailsResult coverageDetailsAsync1;
      try
      {
        using (new SimpleTimer(tcmRequestContext.RequestContext, "GetFileCoverageDetails", ciData))
        {
          FileCoverageDetailsResult coverageDetailsAsync2 = await this._pipelineCoverageStorage.GetFileCoverageDetailsAsync(tcmRequestContext, projectId, pipelineInstanceId, coverageScope, pipelineCoverageDataType, coverageDetailsFileType, continuationToken);
          ciData.Add("FileCoverageDetailsCount", (object) coverageDetailsAsync2.FileCoverageDetailsList.Count<FileCoverageDetails>());
          coverageDetailsAsync1 = coverageDetailsAsync2;
        }
      }
      catch (Exception ex)
      {
        tcmRequestContext.Logger.Error(1015810, string.Format("Error while getting file coverage details: {0}", (object) ex));
        ciData.Add("Exception", (object) ex.Message);
        throw;
      }
      finally
      {
        CustomerIntelligenceData cid = new CustomerIntelligenceData((IDictionary<string, object>) ciData);
        TelemetryLogger.Instance.PublishData(tcmRequestContext.RequestContext, "GetFileCoverageDetails", cid);
      }
      ciData = (Dictionary<string, object>) null;
      return coverageDetailsAsync1;
    }

    public async Task UploadCoverageScopes(
      TestManagementRequestContext tcmRequestContext,
      Guid projectId,
      int pipelineInstanceId,
      IEnumerable<CoverageScope> coverageScopes,
      PipelineCoverageDataType pipelineCoverageDataType)
    {
      Dictionary<string, object> dictionary = new Dictionary<string, object>();
      dictionary.Add("ProjectId", (object) projectId);
      dictionary.Add("PipelineInstanceId", (object) pipelineInstanceId);
      dictionary.Add("PipelineCoverageDataType", (object) pipelineCoverageDataType);
      IEnumerable<CoverageScope> source = coverageScopes;
      dictionary.Add("CoverageScopesCount", (object) (source != null ? new int?(source.Count<CoverageScope>()) : new int?()));
      Dictionary<string, object> ciData = dictionary;
      try
      {
        using (new SimpleTimer(tcmRequestContext.RequestContext, nameof (UploadCoverageScopes), ciData))
        {
          if (coverageScopes == null)
          {
            ciData = (Dictionary<string, object>) null;
            return;
          }
          if (!coverageScopes.Any<CoverageScope>())
          {
            ciData = (Dictionary<string, object>) null;
            return;
          }
          await this._pipelineCoverageStorage.UploadCoverageScopes(tcmRequestContext, projectId, pipelineInstanceId, coverageScopes, pipelineCoverageDataType);
        }
        ciData = (Dictionary<string, object>) null;
      }
      catch (Exception ex)
      {
        tcmRequestContext.Logger.Error(1015806, string.Format("Error while uploading coverage scopes: {0}", (object) ex));
        ciData.Add("Exception", (object) ex.Message);
        throw;
      }
      finally
      {
        CustomerIntelligenceData cid = new CustomerIntelligenceData((IDictionary<string, object>) ciData);
        TelemetryLogger.Instance.PublishData(tcmRequestContext.RequestContext, nameof (UploadCoverageScopes), cid);
      }
    }

    public async Task UploadFileCoverageDetails(
      TestManagementRequestContext tcmRequestContext,
      Guid projectId,
      int pipelineInstanceId,
      IEnumerable<FileCoverageDetails> fileCoverageDetails,
      CoverageScope coverageScope,
      PipelineCoverageDataType pipelineCoverageDataType,
      CoverageDetailsFileType coverageDetailsFileType,
      string fileName = null)
    {
      Dictionary<string, object> dictionary = new Dictionary<string, object>();
      dictionary.Add("ProjectId", (object) projectId);
      dictionary.Add("PipelineInstanceId", (object) pipelineInstanceId);
      dictionary.Add("CoverageScope", (object) coverageScope.Name);
      dictionary.Add("PipelineCoverageDataType", (object) pipelineCoverageDataType);
      IEnumerable<FileCoverageDetails> source = fileCoverageDetails;
      dictionary.Add("FileCoverageDetailsCount", (object) (source != null ? new int?(source.Count<FileCoverageDetails>()) : new int?()));
      Dictionary<string, object> ciData = dictionary;
      try
      {
        using (new SimpleTimer(tcmRequestContext.RequestContext, nameof (UploadFileCoverageDetails), ciData))
          await this._pipelineCoverageStorage.UploadFileCoverageDetails(tcmRequestContext, projectId, pipelineInstanceId, fileCoverageDetails, coverageScope, pipelineCoverageDataType, coverageDetailsFileType, fileName);
      }
      catch (Exception ex)
      {
        tcmRequestContext.Logger.Error(1015809, string.Format("Error while uploading file coverage details: {0}", (object) ex));
        ciData.Add("Exception", (object) ex.Message);
        throw;
      }
      finally
      {
        CustomerIntelligenceData cid = new CustomerIntelligenceData((IDictionary<string, object>) ciData);
        TelemetryLogger.Instance.PublishData(tcmRequestContext.RequestContext, nameof (UploadFileCoverageDetails), cid);
      }
      ciData = (Dictionary<string, object>) null;
    }

    public async Task GetDirectoryCoverageSummaryStreamAsync(
      TestManagementRequestContext tcmRequestContext,
      Guid projectId,
      int pipelineInstanceId,
      string path,
      CoverageScope coverageScope,
      Stream targetStream)
    {
      Dictionary<string, object> ciData = new Dictionary<string, object>()
      {
        {
          "ProjectId",
          (object) projectId
        },
        {
          "PipelineInstanceId",
          (object) pipelineInstanceId
        },
        {
          "CoverageScope",
          (object) coverageScope.Name
        }
      };
      try
      {
        using (new SimpleTimer(tcmRequestContext.RequestContext, nameof (GetDirectoryCoverageSummaryStreamAsync), ciData))
        {
          await this._pipelineCoverageStorage.GetDirectoryCoverageSummaryStreamAsync(tcmRequestContext, projectId, pipelineInstanceId, coverageScope, PipelineCoverageDataType.Final, path, targetStream);
          targetStream.Position = 0L;
        }
      }
      catch (Exception ex)
      {
        tcmRequestContext.Logger.Error(1015915, string.Format("Error while downloading the directory coverage summary: {0}", (object) ex));
        ciData.Add("Exception", (object) ex.Message);
        throw;
      }
      finally
      {
        CustomerIntelligenceData cid = new CustomerIntelligenceData((IDictionary<string, object>) ciData);
        TelemetryLogger.Instance.PublishData(tcmRequestContext.RequestContext, nameof (GetDirectoryCoverageSummaryStreamAsync), cid);
      }
      ciData = (Dictionary<string, object>) null;
    }

    public async Task UploadDirectoryCoverageSummary(
      TestManagementRequestContext tcmRequestContext,
      Guid projectId,
      int pipelineInstanceId,
      int? pipelineId,
      IEnumerable<DirectoryCoverageSummary> directoriesCoverageSummary,
      int childrenCount,
      CoverageScope coverageScope,
      string filePath)
    {
      Dictionary<string, object> ciData = new Dictionary<string, object>()
      {
        {
          "ProjectId",
          (object) projectId
        },
        {
          "PipelineInstanceId",
          (object) pipelineInstanceId
        },
        {
          "PipelineId",
          (object) pipelineId
        },
        {
          "CoverageScope",
          (object) coverageScope.Name
        },
        {
          "ChildrenCount",
          (object) childrenCount
        }
      };
      try
      {
        using (new SimpleTimer(tcmRequestContext.RequestContext, nameof (UploadDirectoryCoverageSummary), ciData))
          await this._pipelineCoverageStorage.UploadDirectoryCoverageSummary(tcmRequestContext, projectId, pipelineInstanceId, directoriesCoverageSummary, filePath);
      }
      catch (Exception ex)
      {
        tcmRequestContext.Logger.Error(1015907, string.Format("Error while uploading directory coverage summary: {0}", (object) ex));
        throw;
      }
      finally
      {
        CustomerIntelligenceData cid = new CustomerIntelligenceData((IDictionary<string, object>) ciData);
        TelemetryLogger.Instance.PublishData(tcmRequestContext.RequestContext, nameof (UploadDirectoryCoverageSummary), cid);
      }
      ciData = (Dictionary<string, object>) null;
    }

    public async Task UploadDirectoryCoverageSummaryIndex(
      TestManagementRequestContext tcmRequestContext,
      Guid projectId,
      int pipelineInstanceId,
      IEnumerable<DirectoryCoverageSummaryIndex> directoryCoverageSummaryIndexList,
      CoverageScope coverageScope)
    {
      Dictionary<string, object> ciData = new Dictionary<string, object>()
      {
        {
          "ProjectId",
          (object) projectId
        },
        {
          "PipelineInstanceId",
          (object) pipelineInstanceId
        },
        {
          "CoverageScope",
          (object) coverageScope.Name
        },
        {
          "DirectoryCoverageSummaryIndexListCount",
          (object) directoryCoverageSummaryIndexList.Count<DirectoryCoverageSummaryIndex>()
        }
      };
      try
      {
        using (new SimpleTimer(tcmRequestContext.RequestContext, nameof (UploadDirectoryCoverageSummaryIndex), ciData))
          await this._pipelineCoverageStorage.UploadDirectoryCoverageSummaryIndex(tcmRequestContext, projectId, pipelineInstanceId, directoryCoverageSummaryIndexList, coverageScope);
      }
      catch (Exception ex)
      {
        tcmRequestContext.Logger.Error(1015908, string.Format("Error while uploading directory coverage summary index: {0}", (object) ex));
        throw;
      }
      finally
      {
        CustomerIntelligenceData cid = new CustomerIntelligenceData((IDictionary<string, object>) ciData);
        TelemetryLogger.Instance.PublishData(tcmRequestContext.RequestContext, "UploadFileCoverageDetailsIndex", cid);
      }
      ciData = (Dictionary<string, object>) null;
    }

    public async Task<CoverageChangeSummary> GetFileCoverageChangeSummaryAsync(
      TestManagementRequestContext tcmRequestContext,
      Guid projectId,
      int pipelineInstanceId,
      CoverageScope coverageScope,
      string continuationToken)
    {
      MemoryStream targetStream = new MemoryStream();
      await this.GetFileCoverageChangeSummaryStreamAsync(tcmRequestContext, projectId, pipelineInstanceId, coverageScope, (Stream) targetStream, continuationToken);
      targetStream.Position = 0L;
      CoverageChangeSummary changeSummaryAsync;
      using (StreamReader streamReader = new StreamReader((Stream) targetStream))
        changeSummaryAsync = JsonConvert.DeserializeObject<CoverageChangeSummary>(streamReader.ReadToEnd(), PipelineCoverageService.DefaultSerializerSettings);
      targetStream = (MemoryStream) null;
      return changeSummaryAsync;
    }

    public async Task UploadFileCoverageChangeSummary(
      TestManagementRequestContext tcmRequestContext,
      Guid projectId,
      int pipelineInstanceId,
      CoverageChangeSummary coverageChangeSummary,
      CoverageScope coverageScope,
      string fileName)
    {
      Dictionary<string, object> ciData = new Dictionary<string, object>()
      {
        {
          "ProjectId",
          (object) projectId
        },
        {
          "PipelineInstanceId",
          (object) pipelineInstanceId
        },
        {
          "CoverageScope",
          (object) coverageScope.Name
        },
        {
          "FileName",
          (object) fileName
        },
        {
          "FileCoverageChangesCount",
          (object) coverageChangeSummary?.FileCoverageChanges?.Count
        }
      };
      try
      {
        using (new SimpleTimer(tcmRequestContext.RequestContext, nameof (UploadFileCoverageChangeSummary), ciData))
          await this._pipelineCoverageStorage.UploadFileCoverageChangeSummary(tcmRequestContext, projectId, pipelineInstanceId, coverageChangeSummary, coverageScope, fileName);
      }
      catch (Exception ex)
      {
        tcmRequestContext.Logger.Error(1015816, string.Format("Error while uploading file coverage change summary: {0}", (object) ex));
        ciData.Add("Exception", (object) ex.Message);
        throw;
      }
      finally
      {
        CustomerIntelligenceData cid = new CustomerIntelligenceData((IDictionary<string, object>) ciData);
        TelemetryLogger.Instance.PublishData(tcmRequestContext.RequestContext, nameof (UploadFileCoverageChangeSummary), cid);
      }
      ciData = (Dictionary<string, object>) null;
    }

    public PipelineCoverageSummary GetPipelineCoverageSummary(
      TestManagementRequestContext tcmRequestContext,
      Guid projectId,
      int pipelineInstanceId,
      CoverageScope coverageScope)
    {
      Dictionary<string, object> dictionary = new Dictionary<string, object>()
      {
        {
          "ProjectId",
          (object) projectId
        },
        {
          "PipelineInstanceId",
          (object) pipelineInstanceId
        },
        {
          "CoverageScope",
          (object) coverageScope.Name
        }
      };
      try
      {
        using (new SimpleTimer(tcmRequestContext.RequestContext, nameof (GetPipelineCoverageSummary), dictionary))
          return this._pipelineCoverageStorage.GetPipelineCoverageSummary(tcmRequestContext, projectId, pipelineInstanceId, coverageScope);
      }
      catch (Exception ex)
      {
        tcmRequestContext.Logger.Error(1015812, string.Format("Error while getting pipeline coverage summary: {0}", (object) ex));
        dictionary.Add("Exception", (object) ex.Message);
        throw;
      }
      finally
      {
        CustomerIntelligenceData cid = new CustomerIntelligenceData((IDictionary<string, object>) dictionary);
        TelemetryLogger.Instance.PublishData(tcmRequestContext.RequestContext, nameof (GetPipelineCoverageSummary), cid);
      }
    }

    public async Task UpdatePipelineCoverageSummary(
      TestManagementRequestContext tcmRequestContext,
      Guid projectId,
      int pipelineInstanceId,
      PipelineCoverageSummary pipelineCoverageSummary,
      CoverageScope coverageScope)
    {
      Dictionary<string, object> ciData = new Dictionary<string, object>()
      {
        {
          "ProjectId",
          (object) projectId
        },
        {
          "PipelineInstanceId",
          (object) pipelineInstanceId
        },
        {
          "CoverageScope",
          (object) coverageScope.Name
        }
      };
      try
      {
        using (new SimpleTimer(tcmRequestContext.RequestContext, nameof (UpdatePipelineCoverageSummary), ciData))
          await this._pipelineCoverageStorage.UpdatePipelineCoverageSummary(tcmRequestContext, projectId, pipelineInstanceId, pipelineCoverageSummary, coverageScope);
      }
      catch (Exception ex)
      {
        tcmRequestContext.Logger.Error(1015811, string.Format("Error while updating pipeline coverage summary: {0}", (object) ex));
        ciData.Add("Exception", (object) ex.Message);
        throw;
      }
      finally
      {
        CustomerIntelligenceData cid = new CustomerIntelligenceData((IDictionary<string, object>) ciData);
        TelemetryLogger.Instance.PublishData(tcmRequestContext.RequestContext, nameof (UpdatePipelineCoverageSummary), cid);
      }
      ciData = (Dictionary<string, object>) null;
    }

    public async Task UploadFileCoverageDetailsIndex(
      TestManagementRequestContext tcmRequestContext,
      Guid projectId,
      int pipelineInstanceId,
      List<FileCoverageDetailsIndex> fileCoverageDetailsIndex,
      CoverageScope coverageScope,
      PipelineCoverageDataType pipelineCoverageDataType,
      CoverageDetailsFileType coverageDetailsFileType)
    {
      Dictionary<string, object> dictionary = new Dictionary<string, object>();
      dictionary.Add("ProjectId", (object) projectId);
      dictionary.Add("PipelineInstanceId", (object) pipelineInstanceId);
      dictionary.Add("CoverageScope", (object) coverageScope.Name);
      dictionary.Add("PipelineCoverageDataType", (object) pipelineCoverageDataType);
      List<FileCoverageDetailsIndex> source = fileCoverageDetailsIndex;
      dictionary.Add("FileCoverageDetailsIndexCount", (object) (source != null ? new int?(source.Count<FileCoverageDetailsIndex>()) : new int?()));
      Dictionary<string, object> ciData = dictionary;
      try
      {
        using (new SimpleTimer(tcmRequestContext.RequestContext, nameof (UploadFileCoverageDetailsIndex), ciData))
          await this._pipelineCoverageStorage.UploadFileCoverageDetailsIndex(tcmRequestContext, projectId, pipelineInstanceId, (IEnumerable<FileCoverageDetailsIndex>) fileCoverageDetailsIndex, coverageScope, pipelineCoverageDataType, coverageDetailsFileType);
      }
      catch (Exception ex)
      {
        tcmRequestContext.Logger.Error(1015814, string.Format("Error while uploading file coverage details index: {0}", (object) ex));
        ciData.Add("UploadFileCoverageDetailsIndex.Exception", (object) ex.Message);
        throw;
      }
      finally
      {
        CustomerIntelligenceData cid = new CustomerIntelligenceData((IDictionary<string, object>) ciData);
        TelemetryLogger.Instance.PublishData(tcmRequestContext.RequestContext, nameof (UploadFileCoverageDetailsIndex), cid);
      }
      ciData = (Dictionary<string, object>) null;
    }

    public async Task GetFileCoverageDetailsStreamAsync(
      TestManagementRequestContext tcmRequestContext,
      Guid projectId,
      int pipelineInstanceId,
      string filePath,
      CoverageScope coverageScope,
      Stream targetStream)
    {
      Dictionary<string, object> ciData = new Dictionary<string, object>()
      {
        {
          "ProjectId",
          (object) projectId
        },
        {
          "PipelineInstanceId",
          (object) pipelineInstanceId
        },
        {
          "CoverageScope",
          (object) coverageScope.Name
        }
      };
      try
      {
        using (new SimpleTimer(tcmRequestContext.RequestContext, nameof (GetFileCoverageDetailsStreamAsync), ciData))
        {
          await this._pipelineCoverageStorage.GetFileCoverageDetailsStream(tcmRequestContext, projectId, pipelineInstanceId, coverageScope, PipelineCoverageDataType.Final, filePath, targetStream);
          targetStream.Position = 0L;
        }
      }
      catch (Exception ex)
      {
        tcmRequestContext.Logger.Error(1015914, string.Format("Error while Downloading file coverage detail : {0}", (object) ex));
        ciData.Add("Exception", (object) ex.Message);
        throw;
      }
      finally
      {
        CustomerIntelligenceData cid = new CustomerIntelligenceData((IDictionary<string, object>) ciData);
        TelemetryLogger.Instance.PublishData(tcmRequestContext.RequestContext, nameof (GetFileCoverageDetailsStreamAsync), cid);
      }
      ciData = (Dictionary<string, object>) null;
    }

    public IList<CoverageSummary> GetCoverageSummaryList(
      TestManagementRequestContext tcmRequestContext,
      Guid projectId,
      int pipelineInstanceId,
      CoverageScope coverageScope,
      ref string continuationToken)
    {
      Dictionary<string, object> dictionary = new Dictionary<string, object>()
      {
        {
          "ProjectId",
          (object) projectId
        },
        {
          "PipelineInstanceId",
          (object) pipelineInstanceId
        },
        {
          "CoverageScope",
          (object) coverageScope.Name
        },
        {
          "ContinuationToken",
          (object) continuationToken
        }
      };
      try
      {
        using (new SimpleTimer(tcmRequestContext.RequestContext, nameof (GetCoverageSummaryList), dictionary))
        {
          IList<CoverageSummary> coverageSummaryList = this._pipelineCoverageStorage.GetCoverageSummaryList(tcmRequestContext, projectId, pipelineInstanceId, coverageScope, ref continuationToken);
          dictionary.Add("NextContinuationToken", (object) continuationToken);
          dictionary.Add("CoverageSummaryCount", (object) coverageSummaryList.Count<CoverageSummary>());
          return coverageSummaryList;
        }
      }
      catch (Exception ex)
      {
        tcmRequestContext.Logger.Error(1015820, string.Format("Error while getting coverage summary list: {0}", (object) ex));
        dictionary.Add("Exception", (object) ex.Message);
        throw;
      }
      finally
      {
        CustomerIntelligenceData cid = new CustomerIntelligenceData((IDictionary<string, object>) dictionary);
        TelemetryLogger.Instance.PublishData(tcmRequestContext.RequestContext, nameof (GetCoverageSummaryList), cid);
      }
    }

    public string GetContinuationTokenForCoverageChangeSummary(
      TestManagementRequestContext tcmRequestContext,
      Guid projectId,
      int pipelineInstanceId,
      CoverageScope coverageScope,
      string continuationToken)
    {
      Dictionary<string, object> dictionary = new Dictionary<string, object>()
      {
        {
          "ProjectId",
          (object) projectId
        },
        {
          "PipelineInstanceId",
          (object) pipelineInstanceId
        },
        {
          "CoverageScope",
          (object) coverageScope.Name
        }
      };
      try
      {
        using (new SimpleTimer(tcmRequestContext.RequestContext, "GetNextContinuationTokenForCoverageChanges", dictionary))
        {
          string forCoverageChanges = this._pipelineCoverageStorage.GetNextContinuationTokenForCoverageChanges(tcmRequestContext, projectId, pipelineInstanceId, coverageScope, continuationToken);
          dictionary.Add("ContinuationToken", (object) forCoverageChanges);
          return forCoverageChanges;
        }
      }
      catch (Exception ex)
      {
        tcmRequestContext.Logger.Error(1015816, string.Format("Error while getting next continuation token for coverage changes: {0}", (object) ex));
        dictionary.Add("Exception", (object) ex.Message);
        throw;
      }
      finally
      {
        CustomerIntelligenceData cid = new CustomerIntelligenceData((IDictionary<string, object>) dictionary);
        TelemetryLogger.Instance.PublishData(tcmRequestContext.RequestContext, "GetContinuationTokenForCoverageChanges", cid);
      }
    }

    public async Task GetFileCoverageChangeSummaryStreamAsync(
      TestManagementRequestContext tcmRequestContext,
      Guid projectId,
      int pipelineInstanceId,
      CoverageScope coverageScope,
      Stream targetStream,
      string continuationToken)
    {
      Dictionary<string, object> ciData = new Dictionary<string, object>()
      {
        {
          "ProjectId",
          (object) projectId
        },
        {
          "PipelineInstanceId",
          (object) pipelineInstanceId
        },
        {
          "CoverageScope",
          (object) coverageScope.Name
        },
        {
          "ContinuationToken",
          (object) continuationToken
        }
      };
      try
      {
        using (new SimpleTimer(tcmRequestContext.RequestContext, nameof (GetFileCoverageChangeSummaryStreamAsync), ciData))
          await this._pipelineCoverageStorage.GetFileCoverageChangeSummary(tcmRequestContext, projectId, pipelineInstanceId, coverageScope, targetStream, continuationToken);
      }
      catch (Exception ex)
      {
        tcmRequestContext.Logger.Error(1015815, string.Format("Error while getting file coverage change summary: {0}", (object) ex));
        ciData.Add("Exception", (object) ex.Message);
        throw;
      }
      finally
      {
        CustomerIntelligenceData cid = new CustomerIntelligenceData((IDictionary<string, object>) ciData);
        TelemetryLogger.Instance.PublishData(tcmRequestContext.RequestContext, "GetFileCoverageDetails", cid);
      }
      ciData = (Dictionary<string, object>) null;
    }

    public async Task UploadFileCoverageSummaryList(
      TestManagementRequestContext tcmRequestContext,
      Guid projectId,
      int pipelineInstanceId,
      IEnumerable<CoverageSummary> coverageSummaryList,
      CoverageScope coverageScope,
      string filePath)
    {
      Dictionary<string, object> ciData = new Dictionary<string, object>()
      {
        {
          "ProjectId",
          (object) projectId
        },
        {
          "PipelineInstanceId",
          (object) pipelineInstanceId
        },
        {
          "CoverageScope",
          (object) coverageScope.Name
        },
        {
          "FilePath",
          (object) filePath
        },
        {
          "CoverageSummaryCount",
          (object) coverageSummaryList.Count<CoverageSummary>()
        }
      };
      try
      {
        using (new SimpleTimer(tcmRequestContext.RequestContext, nameof (UploadFileCoverageSummaryList), ciData))
          await this._pipelineCoverageStorage.UploadFileCoverageSummaryList(tcmRequestContext, projectId, pipelineInstanceId, coverageSummaryList, coverageScope, filePath);
      }
      catch (Exception ex)
      {
        tcmRequestContext.Logger.Error(1015819, string.Format("Error while uploading file coverage summary list: {0}", (object) ex));
        ciData.Add("Exception", (object) ex.Message);
        throw;
      }
      finally
      {
        CustomerIntelligenceData cid = new CustomerIntelligenceData((IDictionary<string, object>) ciData);
        TelemetryLogger.Instance.PublishData(tcmRequestContext.RequestContext, nameof (UploadFileCoverageSummaryList), cid);
      }
      ciData = (Dictionary<string, object>) null;
    }
  }
}
