// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.RestApiHelper
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.TestManagement.Client;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.Operations;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Globalization;
using System.Linq;
using System.Xml;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [CLSCompliant(false)]
  public abstract class RestApiHelper
  {
    public const string RootSuiteTitle = "<root>";
    private ITelemetryLogger _telemetryLogger;
    protected ITestManagementObjectHelper m_objectFactory;
    protected IVssRequestContext m_requestContext;
    protected TestManagementRequestContext m_testManagementRequestContext;
    private ITeamFoundationTestManagementRunService m_testManagementRunService;
    private ITeamFoundationOneMRXSessionService m_testManagementSessionService;
    private ITeamFoundationTestManagementVariableService m_testManagementVariableService;
    private ITeamFoundationTestManagementConfigurationService m_testManagementConfigurationService;
    private ITeamFoundationTestManagementResultService m_testManagementResultService;
    private ITestManagementLinkedWorkItemService m_testManagementLinkedWorkItemService;
    private ITeamFoundationTestManagementAttachmentsService m_testManagementAttachmentsService;
    private ITeamFoundationTestManagementCodeCoverageService m_testManagementCodeCoverageService;
    private IBuildServiceHelper m_BuildServiceHelper;
    private IWorkItemServiceHelper m_WorkItemServiceHelper;
    private IProjectServiceHelper m_ProjectServiceHelper;
    private IReleaseServiceHelper m_ReleaseServiceHelper;
    protected IWorkItemFieldDataHelper m_workItemFieldDataHelper;

    public RestApiHelper(TestManagementRequestContext requestContext)
    {
      this.m_requestContext = requestContext.RequestContext;
      this.m_testManagementRequestContext = requestContext;
    }

    public static List<Microsoft.TeamFoundation.TestManagement.WebApi.ShallowReference> GetIdentitiesShallowReferences(
      List<Guid> identities,
      Dictionary<Guid, Tuple<string, string>> identitiesIdToNameMap)
    {
      List<Microsoft.TeamFoundation.TestManagement.WebApi.ShallowReference> shallowReferences = new List<Microsoft.TeamFoundation.TestManagement.WebApi.ShallowReference>();
      for (int index = 0; index < identities.Count; ++index)
      {
        Tuple<string, string> tuple;
        if (identitiesIdToNameMap.TryGetValue(identities[index], out tuple))
        {
          Microsoft.TeamFoundation.TestManagement.WebApi.ShallowReference shallowReference = new Microsoft.TeamFoundation.TestManagement.WebApi.ShallowReference()
          {
            Id = identities[index].ToString(),
            Name = IdentityHelper.GetDistinctDisplayName(tuple.Item1, tuple.Item2)
          };
          shallowReferences.Add(shallowReference);
        }
      }
      return shallowReferences;
    }

    public Dictionary<int, Microsoft.TeamFoundation.TestManagement.WebApi.ShallowReference> GetLinkedWorkItemsInfoFromIds(
      TestManagementRequestContext tcmRequestContext,
      Guid projectId,
      List<int> workItemIds,
      IEnumerable<string> workItemTypeNamesForBugs,
      bool getAllWorkItems = false)
    {
      using (PerfManager.Measure(tcmRequestContext.RequestContext, "RestLayer", "RestApiHelper.GetLinkedWorkItemsInfoFromIds"))
      {
        Dictionary<int, Microsoft.TeamFoundation.TestManagement.WebApi.ShallowReference> itemsInfoFromIds = new Dictionary<int, Microsoft.TeamFoundation.TestManagement.WebApi.ShallowReference>();
        if (workItemIds.Any<int>())
        {
          IEnumerable<int> source1 = workItemIds.Distinct<int>();
          string[] source2 = new string[3]
          {
            "System.Id",
            "System.Title",
            "System.WorkItemType"
          };
          IList<WorkItem> workItems = tcmRequestContext.WorkItemServiceHelper.GetWorkItems(projectId, (IList<int>) source1.ToList<int>(), (IList<string>) ((IEnumerable<string>) source2).ToList<string>(), WorkItemExpand.None, WorkItemErrorPolicy.Omit);
          if (workItems != null)
          {
            foreach (WorkItem workItem in (IEnumerable<WorkItem>) workItems)
            {
              string field1 = workItem.Fields[WorkItemFieldRefNames.Title] as string;
              int num = workItem.Id.Value;
              string field2 = workItem.Fields[WorkItemFieldRefNames.WorkItemType] as string;
              if (workItemTypeNamesForBugs != null && workItemTypeNamesForBugs.Contains<string>(field2, (IEqualityComparer<string>) StringComparer.InvariantCultureIgnoreCase) | getAllWorkItems)
              {
                Microsoft.TeamFoundation.TestManagement.WebApi.ShallowReference shallowReference = this.WorkItemServiceHelper.GetWorkItemShallowReference(num, field1);
                itemsInfoFromIds.Add(num, shallowReference);
              }
            }
          }
        }
        return itemsInfoFromIds;
      }
    }

    public static ResultDetails IncludeVariableToResultDetails(
      bool includeIterationDetails,
      bool includeAssociatedBugs,
      bool includeSubResultDetails)
    {
      return (ResultDetails) (0 | (includeIterationDetails ? 1 : 0) & 1 | (includeAssociatedBugs ? 2 : 0) & 2 | (includeSubResultDetails ? 4 : 0) & 4);
    }

    public static DateTime ParseDate(string date, string propertyName)
    {
      DateTime result;
      if (DateTime.TryParse(date, (IFormatProvider) CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal, out result))
        return result;
      throw new InvalidPropertyException(propertyName, ServerResources.InvalidPropertyMessage);
    }

    public static List<int> GetListOfIds(
      IVssRequestContext requestContext,
      string ids,
      string fieldName)
    {
      List<int> listOfIds = new List<int>();
      try
      {
        string[] source = ids.Split(new string[1]{ "," }, StringSplitOptions.RemoveEmptyEntries);
        if (source != null)
        {
          if (((IEnumerable<string>) source).Count<string>() > 0)
            listOfIds = ((IEnumerable<string>) source).ToList<string>().ConvertAll<int>((Converter<string, int>) (pointId => int.Parse(pointId)));
        }
      }
      catch (Exception ex)
      {
        switch (ex)
        {
          case NullReferenceException _:
          case ArgumentNullException _:
          case OverflowException _:
          case FormatException _:
            throw new InvalidPropertyException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, string.Format(ServerResources.InValidListOfIds, (object) fieldName)));
          default:
            throw;
        }
      }
      return listOfIds;
    }

    public static bool QueuePublishTestResultJob(
      IVssRequestContext context,
      ProjectInfo projectInfo,
      int runId,
      int attachmentId,
      TestResultDocument document,
      bool queueTcmJob,
      out TestResultDocument testResultDocument)
    {
      using (PerfManager.Measure(context, "CrossService", TraceUtils.GetActionName("TryQueuePublishTestResultJob", "Tcm")))
      {
        TestResultDocument result = new TestResultDocument();
        int num = RestApiHelper.InvokeAction((Func<bool>) (() =>
        {
          TeamProjectReference referenceFromProjectInfo = RestApiHelper.GetProjectReferenceFromProjectInfo(projectInfo);
          XmlNode xml = TeamFoundationSerializationUtility.SerializeToXml((object) new TestCaseResultsContext()
          {
            ProjectReference = referenceFromProjectInfo,
            AttachmentId = attachmentId,
            RunId = runId
          });
          string extensionName = queueTcmJob ? "Microsoft.VisualStudio.Services.Tcm.Plugins.Jobs.TcmPublishTestResultsJob" : "Microsoft.TeamFoundation.TestManagement.Server.Jobs.PublishTestResultsJob";
          OperationReference operationReference = JobOperationsUtility.GetOperationReference(context, context.GetService<ITeamFoundationJobService>().QueueOneTimeJob(context, "Publish test results.", extensionName, xml, JobPriorityLevel.Normal));
          result.OperationReference = new TestOperationReference()
          {
            Id = operationReference.Id.ToString(),
            Status = operationReference.Status.ToString(),
            Url = operationReference.Url
          };
          result.Payload = new TestResultPayload()
          {
            Name = document.Payload.Name
          };
          return true;
        })) ? 1 : 0;
        testResultDocument = result;
        return num != 0;
      }
    }

    internal ITelemetryLogger TelemetryLogger
    {
      get => this._telemetryLogger ?? (ITelemetryLogger) Microsoft.TeamFoundation.TestManagement.Server.TelemetryLogger.Instance;
      set => this._telemetryLogger = value;
    }

    protected Microsoft.TeamFoundation.Build.WebApi.Build GetBuildDetail(
      string buildUri,
      Guid projectId)
    {
      return this.ExecuteAction<Microsoft.TeamFoundation.Build.WebApi.Build>("RestApiHelper.GetBuildDetail", (Func<Microsoft.TeamFoundation.Build.WebApi.Build>) (() => this.ObjectFactory.GetBuildDetailFromUri(buildUri, this.RequestContext, projectId, this.BuildServiceHelper)), 1015050, "TestManagement");
    }

    protected string GetBuildUriFromId(string buildId) => LinkingUtilities.EncodeUri(new ArtifactId()
    {
      ToolSpecificId = buildId,
      ArtifactType = "Build",
      Tool = "Build"
    });

    protected bool TryGetTestRunFromRunId(string projectName, int runId, out TestRun testRun)
    {
      testRun = this.ExecuteAction<TestRun>("RestApiHelper.TryGetTestRunFromRunId", (Func<TestRun>) (() => this.TestManagementRequestContext.RequestContext.GetService<ITeamFoundationTestManagementRunService>().GetTestRunById(this.TestManagementRequestContext, runId, new TeamProjectReference()
      {
        Name = projectName
      })), 1015050, "TestManagement");
      return testRun != null;
    }

    protected virtual Microsoft.TeamFoundation.TestManagement.WebApi.ShallowReference GetRunRepresentation(
      string projectName,
      TestRun run)
    {
      return new DataContractConverter(this.TestManagementRequestContext).GetRunRepresentation(projectName, run);
    }

    protected virtual Microsoft.TeamFoundation.TestManagement.WebApi.ShallowReference GetRunRepresentation(
      string projectName,
      int runId)
    {
      TestRun testRun;
      return this.TryGetTestRunFromRunId(projectName, runId, out testRun) ? this.GetRunRepresentation(projectName, testRun) : throw new TestObjectNotFoundException(this.RequestContext, runId, ObjectTypes.TestRun);
    }

    protected Microsoft.TeamFoundation.TestManagement.WebApi.ReleaseReference GetReleaseReference(
      ReleaseReference releaseRef)
    {
      if (releaseRef == null || releaseRef.ReleaseId <= 0)
        return (Microsoft.TeamFoundation.TestManagement.WebApi.ReleaseReference) null;
      return new Microsoft.TeamFoundation.TestManagement.WebApi.ReleaseReference()
      {
        Id = releaseRef.ReleaseId,
        Name = releaseRef.ReleaseName,
        EnvironmentId = releaseRef.ReleaseEnvId,
        EnvironmentName = releaseRef.ReleaseEnvName,
        DefinitionId = releaseRef.ReleaseDefId,
        EnvironmentDefinitionId = releaseRef.ReleaseEnvDefId
      };
    }

    protected virtual Microsoft.TeamFoundation.TestManagement.WebApi.ShallowReference GetTestResultRepresentation(
      string projectName,
      int runId,
      int resultId)
    {
      RestApiResourceDetails resourceMapping = this.TestManagementRequestContext.ResourceMappings[ResourceMappingConstants.TestResult];
      return new Microsoft.TeamFoundation.TestManagement.WebApi.ShallowReference()
      {
        Id = resultId.ToString(),
        Url = UrlBuildHelper.GetResourceUrl(this.RequestContext, resourceMapping.ServiceInstanceType, resourceMapping.Area, resourceMapping.ResourceId, (object) new
        {
          runId = runId,
          testCaseResultId = resultId,
          project = projectName
        })
      };
    }

    protected Microsoft.VisualStudio.Services.Identity.Identity ReadIdentityByDisplayName(
      string displayName)
    {
      return this.ExecuteAction<Microsoft.VisualStudio.Services.Identity.Identity>("RestApiHelper.ReadIdentityByDisplayName", (Func<Microsoft.VisualStudio.Services.Identity.Identity>) (() =>
      {
        if (!string.IsNullOrEmpty(displayName))
        {
          Guid accountId = IdentityHelper.SearchUserByDisplayName(this.TestManagementRequestContext, displayName).FirstOrDefault<Guid>();
          if (accountId != Guid.Empty)
            return this.ReadIdentityByAccountId(accountId);
        }
        return (Microsoft.VisualStudio.Services.Identity.Identity) null;
      }), 1015050, "TestManagement");
    }

    public Microsoft.VisualStudio.Services.Identity.Identity ReadIdentityByAccountId(Guid accountId) => this.ExecuteAction<Microsoft.VisualStudio.Services.Identity.Identity>("RestApiHelper.ReadIdentityByAccountId", (Func<Microsoft.VisualStudio.Services.Identity.Identity>) (() =>
    {
      if (accountId != Guid.Empty)
      {
        Microsoft.VisualStudio.Services.Identity.Identity[] source = this.ReadIdentityByAccounts(new Guid[1]
        {
          accountId
        });
        if (source != null)
          return ((IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) source).FirstOrDefault<Microsoft.VisualStudio.Services.Identity.Identity>();
      }
      return (Microsoft.VisualStudio.Services.Identity.Identity) null;
    }), 1015050, "TestManagement");

    protected Microsoft.VisualStudio.Services.Identity.Identity[] ReadIdentityByAccounts(
      Guid[] accountIds)
    {
      return this.ExecuteAction<Microsoft.VisualStudio.Services.Identity.Identity[]>("RestApiHelper.ReadIdentityByAccounts", (Func<Microsoft.VisualStudio.Services.Identity.Identity[]>) (() =>
      {
        if (!((IEnumerable<Guid>) accountIds).IsNullOrEmpty<Guid>())
        {
          IList<Microsoft.VisualStudio.Services.Identity.Identity> source = this.TestManagementRequestContext.IdentityService.ReadIdentities(this.TestManagementRequestContext.RequestContext, (IList<Guid>) accountIds, QueryMembership.None, (IEnumerable<string>) null);
          if (source != null)
            return source.ToArray<Microsoft.VisualStudio.Services.Identity.Identity>();
        }
        return (Microsoft.VisualStudio.Services.Identity.Identity[]) null;
      }), 1015050, "TestManagement");
    }

    protected Dictionary<Guid, IdentityRef> GetIdentityMapping(List<Guid> ids)
    {
      Dictionary<Guid, IdentityRef> identityMapping = new Dictionary<Guid, IdentityRef>();
      if (ids != null)
      {
        ids = ids.Where<Guid>((Func<Guid, bool>) (id => id != Guid.Empty)).ToList<Guid>();
        Microsoft.VisualStudio.Services.Identity.Identity[] identityArray = this.ReadIdentityByAccounts(ids.ToArray());
        if (identityArray != null)
        {
          int index = 0;
          foreach (Microsoft.VisualStudio.Services.Identity.Identity identity in identityArray)
          {
            if (identity != null)
              identityMapping[ids[index]] = this.CreateTeamFoundationIdentityReference(identity);
            ++index;
          }
        }
      }
      return identityMapping;
    }

    protected IdentityRef GetIdentity(Guid identity, Dictionary<Guid, IdentityRef> lastUpdatedBy = null)
    {
      IdentityRef identity1 = (IdentityRef) null;
      if (identity == Guid.Empty)
        return (IdentityRef) null;
      if (lastUpdatedBy != null && lastUpdatedBy.TryGetValue(identity, out identity1))
        return identity1;
      Microsoft.VisualStudio.Services.Identity.Identity identity2 = this.ReadIdentityByAccountId(identity);
      return identity2 != null ? this.CreateTeamFoundationIdentityReference(identity2) : (IdentityRef) null;
    }

    protected T ExecuteAction<T>(
      string methodName,
      Func<T> action,
      int tracePoint,
      string traceArea,
      string traceLayer = "RestLayer")
    {
      try
      {
        this.RequestContext.TraceEnter(tracePoint, traceArea, traceLayer, methodName);
        return action();
      }
      catch (Exception ex)
      {
        this.RequestContext.TraceException("RestLayer", ex);
        switch (ex)
        {
          case TestObjectNotFoundException _:
            throw new Microsoft.TeamFoundation.TestManagement.WebApi.TestObjectNotFoundException(ex.Message, ex);
          case TeamProjectNotFoundException _:
            throw new Microsoft.TeamFoundation.TestManagement.WebApi.TeamProjectNotFoundException(ex.Message, ex);
          case ProjectDoesNotExistWithNameException _:
            throw new Microsoft.TeamFoundation.TestManagement.WebApi.TeamProjectNotFoundException(ex.Message, ex);
          case AccessDeniedException _:
            throw new Microsoft.TeamFoundation.TestManagement.WebApi.AccessDeniedException(ex.Message, ex);
          case TestObjectInUseException _:
            throw new Microsoft.TeamFoundation.TestManagement.WebApi.TestObjectInUseException(ex.Message, ex);
          default:
            throw;
        }
      }
      finally
      {
        this.RequestContext.TraceLeave(tracePoint, traceArea, traceLayer, methodName);
      }
    }

    internal virtual void CheckForViewTestResultPermission(string projectName) => this.CheckForViewTestResultPermissionInternal(this.TestManagementRequestContext.ProjectServiceHelper.GetProjectFromName(projectName).Uri);

    public void CheckForViewTestResultPermission(Guid projectId) => this.CheckForViewTestResultPermissionInternal(this.TestManagementRequestContext.ProjectServiceHelper.GetProjectFromGuid(projectId).Uri);

    private void CheckForViewTestResultPermissionInternal(string projectUri)
    {
      if (!this.TestManagementRequestContext.SecurityManager.HasViewTestResultsPermission(this.TestManagementRequestContext, projectUri))
        throw new AccessDeniedException(ServerResources.TestPlanViewTestResultPermission);
    }

    protected virtual void CheckForWorkItemDeletePermission(string projectName)
    {
    }

    public IdentityRef CreateTeamFoundationIdentityReference(Microsoft.VisualStudio.Services.Identity.Identity identity) => identity.ToIdentityRef(this.RequestContext);

    protected IdentityRef[] CreateTeamFoundationIdentitiesReferences(Microsoft.VisualStudio.Services.Identity.Identity[] identities) => ((IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) identities).ToIdentityRefs(this.RequestContext);

    protected IdentityRef ToIdentityRef(string identityGuidString) => this.ReadIdentityByAccountId(new Guid(identityGuidString)).ToIdentityRef(this.RequestContext);

    internal virtual TeamProjectReference GetProjectReference(string projectIdentifier) => this.ExecuteAction<TeamProjectReference>("RestApiHelper.GetProjectReference", (Func<TeamProjectReference>) (() =>
    {
      Guid result;
      ProjectInfo projectInfo = !Guid.TryParse(projectIdentifier, out result) ? this.TestManagementRequestContext.ProjectServiceHelper.GetProjectFromName(projectIdentifier) : this.TestManagementRequestContext.ProjectServiceHelper.GetProjectFromGuid(result);
      if (projectInfo == null)
        return (TeamProjectReference) null;
      return new TeamProjectReference()
      {
        Id = projectInfo.Id,
        Name = projectInfo.Name
      };
    }), 1015050, "TestManagement");

    internal virtual TeamProjectReference GetProjectReference(Guid projectIdentifier) => this.ExecuteAction<TeamProjectReference>("RestApiHelper.GetProjectReference", (Func<TeamProjectReference>) (() =>
    {
      ProjectInfo projectFromGuid = this.TestManagementRequestContext.ProjectServiceHelper.GetProjectFromGuid(projectIdentifier);
      if (projectFromGuid == null)
        return (TeamProjectReference) null;
      return new TeamProjectReference()
      {
        Id = projectFromGuid.Id,
        Name = projectFromGuid.Name
      };
    }), 1015050, "TestManagement");

    public int ValidateAndSetMaxPageSizeForRunArtifacts(
      int? pageSize,
      bool includeDetails,
      string propertyName,
      int defaultPageSize = 2147483647,
      int defaultPageSizeIncludeDetail = 200)
    {
      int num1 = defaultPageSize;
      if (pageSize.HasValue)
      {
        int? nullable;
        if (includeDetails)
        {
          nullable = pageSize;
          int num2 = defaultPageSizeIncludeDetail;
          if (nullable.GetValueOrDefault() > num2 & nullable.HasValue)
            throw new InvalidPropertyException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.MaxTestResultsLimitCrossedError, (object) defaultPageSizeIncludeDetail));
        }
        if (!includeDetails)
        {
          nullable = pageSize;
          int num3 = Math.Max(defaultPageSize, 10000);
          if (nullable.GetValueOrDefault() > num3 & nullable.HasValue)
            throw new InvalidPropertyException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.MaxTestResultsLimitCrossedError, (object) Math.Max(defaultPageSize, 10000)));
        }
        num1 = pageSize.Value;
      }
      else if (includeDetails)
        num1 = this.RequestContext.GetService<IVssRegistryService>().GetValue<int>(this.RequestContext, (RegistryQuery) "/Service/TestManagement/Settings/TestResultsPageSize", defaultPageSizeIncludeDetail);
      return num1;
    }

    private static TeamProjectReference GetProjectReferenceFromProjectInfo(ProjectInfo projectInfo) => new TeamProjectReference()
    {
      Name = projectInfo.Name,
      Id = projectInfo.Id,
      State = projectInfo.State,
      Description = projectInfo.Description,
      Revision = projectInfo.Revision,
      Abbreviation = projectInfo.Abbreviation
    };

    private static bool InvokeAction(Func<bool> func)
    {
      try
      {
        return func();
      }
      catch (AggregateException ex)
      {
        if (ex.InnerException != null)
          throw ex.InnerException;
        throw;
      }
    }

    protected internal virtual ITestManagementObjectHelper ObjectFactory
    {
      get
      {
        if (this.m_objectFactory == null)
          this.m_objectFactory = (ITestManagementObjectHelper) new TestManagementObjectHelper();
        return this.m_objectFactory;
      }
      set => this.m_objectFactory = value;
    }

    protected internal virtual TestManagementRequestContext TestManagementRequestContext
    {
      get => this.m_testManagementRequestContext;
      set => this.m_testManagementRequestContext = value;
    }

    protected internal virtual IWorkItemFieldDataHelper WorkItemFieldDataHelper
    {
      get
      {
        if (this.m_workItemFieldDataHelper == null)
          this.m_workItemFieldDataHelper = (IWorkItemFieldDataHelper) new DefaultWorkItemDataHelper();
        return this.m_workItemFieldDataHelper;
      }
      set => this.m_workItemFieldDataHelper = value;
    }

    protected internal IReleaseServiceHelper ReleaseServiceHelper
    {
      get
      {
        if (this.m_ReleaseServiceHelper == null)
          this.m_ReleaseServiceHelper = (IReleaseServiceHelper) new Microsoft.TeamFoundation.TestManagement.Server.ReleaseServiceHelper();
        return this.m_ReleaseServiceHelper;
      }
      set => this.m_ReleaseServiceHelper = value;
    }

    protected internal IBuildServiceHelper BuildServiceHelper
    {
      get
      {
        if (this.m_BuildServiceHelper == null)
          this.m_BuildServiceHelper = (IBuildServiceHelper) new Microsoft.TeamFoundation.TestManagement.Server.BuildServiceHelper();
        return this.m_BuildServiceHelper;
      }
      set => this.m_BuildServiceHelper = value;
    }

    protected internal IWorkItemServiceHelper WorkItemServiceHelper
    {
      get
      {
        if (this.m_WorkItemServiceHelper == null)
          this.m_WorkItemServiceHelper = (IWorkItemServiceHelper) new Microsoft.TeamFoundation.TestManagement.Server.WorkItemServiceHelper(this.m_requestContext);
        return this.m_WorkItemServiceHelper;
      }
      set => this.m_WorkItemServiceHelper = value;
    }

    protected internal IProjectServiceHelper ProjectServiceHelper
    {
      get
      {
        if (this.m_ProjectServiceHelper == null)
          this.m_ProjectServiceHelper = (IProjectServiceHelper) new Microsoft.TeamFoundation.TestManagement.Server.ProjectServiceHelper(this.m_requestContext);
        return this.m_ProjectServiceHelper;
      }
      set => this.m_ProjectServiceHelper = value;
    }

    protected internal IVssRequestContext RequestContext => this.m_requestContext;

    protected ITeamFoundationOneMRXSessionService TeamFoundationOneMRXSessionService
    {
      get
      {
        if (this.m_testManagementSessionService == null)
          this.m_testManagementSessionService = this.RequestContext.GetService<ITeamFoundationOneMRXSessionService>();
        return this.m_testManagementSessionService;
      }
    }

    protected ITeamFoundationTestManagementRunService TestManagementRunService
    {
      get
      {
        if (this.m_testManagementRunService == null)
          this.m_testManagementRunService = this.RequestContext.GetService<ITeamFoundationTestManagementRunService>();
        return this.m_testManagementRunService;
      }
    }

    protected ITeamFoundationTestManagementVariableService TestManagementVariableService
    {
      get
      {
        if (this.m_testManagementVariableService == null)
          this.m_testManagementVariableService = this.RequestContext.GetService<ITeamFoundationTestManagementVariableService>();
        return this.m_testManagementVariableService;
      }
    }

    protected ITeamFoundationTestManagementConfigurationService TestManagementConfigurationService
    {
      get
      {
        if (this.m_testManagementConfigurationService == null)
          this.m_testManagementConfigurationService = this.RequestContext.GetService<ITeamFoundationTestManagementConfigurationService>();
        return this.m_testManagementConfigurationService;
      }
    }

    protected ITeamFoundationTestManagementResultService TestManagementResultService
    {
      get
      {
        if (this.m_testManagementResultService == null)
          this.m_testManagementResultService = this.RequestContext.GetService<ITeamFoundationTestManagementResultService>();
        return this.m_testManagementResultService;
      }
    }

    protected ITestManagementLinkedWorkItemService TestManagementLinkedWorkItemService
    {
      get
      {
        if (this.m_testManagementLinkedWorkItemService == null)
          this.m_testManagementLinkedWorkItemService = this.RequestContext.GetService<ITestManagementLinkedWorkItemService>();
        return this.m_testManagementLinkedWorkItemService;
      }
    }

    protected ITeamFoundationTestManagementAttachmentsService TestManagementAttachmentsService
    {
      get
      {
        if (this.m_testManagementAttachmentsService == null)
          this.m_testManagementAttachmentsService = this.RequestContext.GetService<ITeamFoundationTestManagementAttachmentsService>();
        return this.m_testManagementAttachmentsService;
      }
    }

    protected ITeamFoundationTestManagementCodeCoverageService TestManagementCodeCoverageService
    {
      get
      {
        if (this.m_testManagementCodeCoverageService == null)
          this.m_testManagementCodeCoverageService = this.RequestContext.GetService<ITeamFoundationTestManagementCodeCoverageService>();
        return this.m_testManagementCodeCoverageService;
      }
    }

    protected DateTime MinSqlTime => SqlDateTime.MinValue.Value;

    protected DateTime MaxSqlTime => SqlDateTime.MaxValue.Value;
  }
}
