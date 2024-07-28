// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TeamFoundationTestManagementService
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.TeamFoundation.TestManagement.Client;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [CLSCompliant(false)]
  public abstract class TeamFoundationTestManagementService : IVssFrameworkService
  {
    private readonly TestManagementRequestContext m_testManagementRequestContext;
    private IBuildServiceHelper _buildServiceHelper;

    protected TeamFoundationTestManagementService()
    {
    }

    protected TeamFoundationTestManagementService(TestManagementRequestContext requestContext) => this.m_testManagementRequestContext = requestContext;

    public virtual void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public virtual void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    protected TestManagementRequestContext GetTestManagementRequestContext() => this.m_testManagementRequestContext;

    protected virtual TestManagementRequestContext GetTestManagementRequestContext(
      IVssRequestContext context)
    {
      return this.m_testManagementRequestContext ?? new TestManagementRequestContext(context);
    }

    protected T ExecuteAction<T>(
      IVssRequestContext requestContext,
      string methodName,
      Func<T> action,
      int tracePoint,
      string traceArea,
      string traceLayer = "RestLayer")
    {
      try
      {
        requestContext.TraceEnter(tracePoint, traceArea, traceLayer, methodName);
        return action();
      }
      finally
      {
        requestContext.TraceLeave(tracePoint, traceArea, traceLayer, methodName);
      }
    }

    protected void ExecuteAction(
      IVssRequestContext requestContext,
      string methodName,
      Action action,
      int tracePoint,
      string traceArea,
      string traceLayer = "RestLayer")
    {
      try
      {
        requestContext.TraceEnter(tracePoint, traceArea, traceLayer, methodName);
        action();
      }
      finally
      {
        requestContext.TraceLeave(tracePoint, traceArea, traceLayer, methodName);
      }
    }

    protected string GetErrorMessage(TestObjectNotFoundException e)
    {
      switch (e.ObjectType)
      {
        case ObjectTypes.TestRun:
          return ServerResources.TestRunNotFoundError;
        case ObjectTypes.TestPoint:
          return ServerResources.TestPointNotFoundError;
        case ObjectTypes.TestResult:
          return ServerResources.TestCaseResultNotFoundError;
        case ObjectTypes.TestCase:
          return ServerResources.TestCaseNotFoundError;
        default:
          return string.Empty;
      }
    }

    protected virtual TeamProjectReference GetProjectReference(
      IVssRequestContext requestContext,
      string projectIdentifier)
    {
      using (PerfManager.Measure(requestContext, "CrossService", TraceUtils.GetActionName(nameof (GetProjectReference), "Project")))
      {
        IProjectService service = requestContext.GetService<IProjectService>();
        Guid result;
        if (Guid.TryParse(projectIdentifier, out result))
        {
          ProjectInfo project = service.GetProject(requestContext, result);
          return project != null ? this.ToTeamProjectReference(requestContext, project) : (TeamProjectReference) null;
        }
        ProjectInfo project1 = service.GetProject(requestContext, projectIdentifier);
        return project1 != null ? this.ToTeamProjectReference(requestContext, project1) : (TeamProjectReference) null;
      }
    }

    protected virtual Dictionary<string, Microsoft.VisualStudio.Services.Identity.Identity> GetTfsIdentitiesByNames(
      TestManagementRequestContext requestContext,
      List<string> displayNames)
    {
      using (PerfManager.Measure(requestContext.RequestContext, "BusinessLayer", "TeamFoundationTestManagementService.GetTfsIdentitiesByNames"))
        return this.ExecuteAction<Dictionary<string, Microsoft.VisualStudio.Services.Identity.Identity>>(requestContext.RequestContext, "TeamFoundationTestManagementService.GetTfsIdentitiesByNames", (Func<Dictionary<string, Microsoft.VisualStudio.Services.Identity.Identity>>) (() =>
        {
          if (displayNames != null)
          {
            if (displayNames.Any<string>())
            {
              try
              {
                return new TestManagementServiceUtility(requestContext).ReadIdentitesByNames(displayNames);
              }
              catch (Exception ex)
              {
                requestContext.RequestContext.TraceException("BusinessLayer", ex);
              }
            }
          }
          return new Dictionary<string, Microsoft.VisualStudio.Services.Identity.Identity>();
        }), 1015050, "TestManagement", "BusinessLayer");
    }

    protected virtual Dictionary<string, Microsoft.VisualStudio.Services.Identity.Identity> GetTfsIdentitiesByDirectoryAlias(
      TestManagementRequestContext requestContext,
      List<string> directoryAlias)
    {
      using (PerfManager.Measure(requestContext.RequestContext, "BusinessLayer", "TeamFoundationTestManagementService.GetTfsIdentitiesByDirectoryAlias"))
        return this.ExecuteAction<Dictionary<string, Microsoft.VisualStudio.Services.Identity.Identity>>(requestContext.RequestContext, "TeamFoundationTestManagementService.GetTfsIdentitiesByDirectoryAlias", (Func<Dictionary<string, Microsoft.VisualStudio.Services.Identity.Identity>>) (() =>
        {
          if (directoryAlias != null)
          {
            if (directoryAlias.Any<string>())
            {
              try
              {
                return new TestManagementServiceUtility(requestContext).ReadIdentitesByDirectoryAlias(directoryAlias);
              }
              catch (Exception ex)
              {
                requestContext.RequestContext.TraceException("BusinessLayer", ex);
              }
            }
          }
          return new Dictionary<string, Microsoft.VisualStudio.Services.Identity.Identity>();
        }), 1015050, "TestManagement", "BusinessLayer");
    }

    protected virtual Dictionary<Guid, string> GetTfsIdentitiesUniqueNamesByIds(
      TestManagementRequestContext requestContext,
      List<Guid> tfIds)
    {
      using (PerfManager.Measure(requestContext.RequestContext, "BusinessLayer", "TeamFoundationTestManagementService.GetTfsIdentitiesUniqueNamesByIds"))
        return this.ExecuteAction<Dictionary<Guid, string>>(requestContext.RequestContext, "TeamFoundationTestManagementService.GetTfsIdentitiesUniqueNamesByIds", (Func<Dictionary<Guid, string>>) (() =>
        {
          List<Guid> guidList = tfIds;
          // ISSUE: explicit non-virtual call
          if ((guidList != null ? (__nonvirtual (guidList.Count) > 0 ? 1 : 0) : 0) != 0)
          {
            try
            {
              return IdentityHelper.ResolveIdentitiesEx(requestContext, (IList<Guid>) tfIds).ToDictionary<KeyValuePair<Guid, Tuple<string, string>>, Guid, string>((Func<KeyValuePair<Guid, Tuple<string, string>>, Guid>) (x => x.Key), (Func<KeyValuePair<Guid, Tuple<string, string>>, string>) (x => x.Value == null ? (string) null : IdentityHelper.GetDistinctDisplayName(x.Value.Item1, x.Value.Item2)));
            }
            catch (Exception ex)
            {
              requestContext.RequestContext.TraceException("BusinessLayer", ex);
            }
          }
          return new Dictionary<Guid, string>();
        }), 1015050, "TestManagement", "BusinessLayer");
    }

    protected TeamProjectReference GetProjectReferenceFromProjectInfo(ProjectInfo projectInfo) => new TeamProjectReference()
    {
      Name = projectInfo.Name,
      Id = projectInfo.Id,
      State = projectInfo.State,
      Description = projectInfo.Description,
      Revision = projectInfo.Revision,
      Abbreviation = projectInfo.Abbreviation
    };

    protected byte ValidateAndGetEnumValue<TEnum>(string strEnumVal, TEnum defaultValue) where TEnum : struct, IConvertible
    {
      if (!typeof (TEnum).IsEnum)
        throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.OnlyEnumTypesSupportedError)).Expected("Test Results");
      byte enumValue = Convert.ToByte((object) defaultValue);
      if (!string.IsNullOrEmpty(strEnumVal))
      {
        TEnum result;
        if (!Enum.TryParse<TEnum>(strEnumVal, true, out result))
          throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.InvalidValueSpecified), typeof (TEnum).Name).Expected("Test Results");
        enumValue = Convert.ToByte((object) result);
      }
      return enumValue;
    }

    protected void CheckForViewTestResultPermission(
      TestManagementRequestContext requestContext,
      string projectName)
    {
      string projectUriFromName = Validator.CheckAndGetProjectUriFromName(requestContext, projectName);
      if (!requestContext.SecurityManager.HasViewTestResultsPermission(requestContext, projectUriFromName))
        throw new Microsoft.TeamFoundation.TestManagement.WebApi.AccessDeniedException(ServerResources.TestPlanViewTestResultPermission);
    }

    protected void CheckForViewTestResultPermission(
      TestManagementRequestContext requestContext,
      Guid projectId)
    {
      string projectUriFromId = Validator.CheckAndGetProjectUriFromId(requestContext, projectId);
      if (!requestContext.SecurityManager.HasViewTestResultsPermission(requestContext, projectUriFromId))
        throw new Microsoft.TeamFoundation.TestManagement.WebApi.AccessDeniedException(ServerResources.TestPlanViewTestResultPermission);
    }

    protected TeamProjectReference ToTeamProjectReference(
      IVssRequestContext requestContext,
      ProjectInfo projectInfo)
    {
      return new TeamProjectReference()
      {
        Id = projectInfo.Id,
        Abbreviation = projectInfo.Abbreviation,
        Name = projectInfo.Name,
        State = projectInfo.State,
        Description = projectInfo.Description,
        Revision = projectInfo.Revision,
        Visibility = projectInfo.Visibility
      };
    }

    protected ProjectInfo ToTeamProjectInfo(
      IVssRequestContext requestContext,
      TeamProjectReference projectReference)
    {
      return new ProjectInfo()
      {
        Id = projectReference.Id,
        Abbreviation = projectReference.Abbreviation,
        Name = projectReference.Name,
        State = projectReference.State,
        Description = projectReference.Description,
        Revision = projectReference.Revision,
        Visibility = projectReference.Visibility
      };
    }

    internal IBuildServiceHelper BuildServiceHelper
    {
      get => this._buildServiceHelper ?? (IBuildServiceHelper) new Microsoft.TeamFoundation.TestManagement.Server.BuildServiceHelper();
      set => this._buildServiceHelper = value;
    }
  }
}
