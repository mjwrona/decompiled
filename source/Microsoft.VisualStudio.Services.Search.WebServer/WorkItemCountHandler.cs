// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.WebServer.WorkItemCountHandler
// Assembly: Microsoft.VisualStudio.Services.Search.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1112A012-BB03-4D21-B53E-3AFB00CCC7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.WebServer.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Arriba;
using Microsoft.VisualStudio.Services.Search.Common.Arriba.Expressions;
using Microsoft.VisualStudio.Services.Search.Common.Entities;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities.WorkItem;
using Microsoft.VisualStudio.Services.Search.Query;
using Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Contracts;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Search.WebServer
{
  public class WorkItemCountHandler : AbstractSearchCountHandler
  {
    public WorkItemCountHandler(ICountRequestForwarder forwarder)
      : base(forwarder)
    {
      this.IndexMapper = (IIndexMapper) new Microsoft.VisualStudio.Services.Search.Query.IndexMapper((IEntityType) WorkItemEntityType.GetInstance());
      this.EntityType = (IEntityType) WorkItemEntityType.GetInstance();
    }

    public WorkItemCountHandler()
    {
      this.IndexMapper = (IIndexMapper) new Microsoft.VisualStudio.Services.Search.Query.IndexMapper((IEntityType) WorkItemEntityType.GetInstance());
      this.EntityType = (IEntityType) WorkItemEntityType.GetInstance();
    }

    protected override void InitializeForwarder(IVssRequestContext requestContext)
    {
      if (this.CountRequestForwarder != null)
        return;
      this.CountRequestForwarder = (ICountRequestForwarder) new WorkItemCountRequestForwarder(this.IndexMapper.GetESConnectionString(requestContext), requestContext.GetElasticsearchPlatformSettings("/Service/ALMSearch/Settings/ATSearchPlatformSettings"), requestContext.ExecutionEnvironment.IsOnPremisesDeployment);
    }

    protected override IExpression CreateScopeFiltersExpression(
      IVssRequestContext requestContext,
      CountRequest query,
      out bool noResultAccessible,
      ProjectInfo projectInfo)
    {
      List<IExpression> expressionList = new List<IExpression>();
      noResultAccessible = false;
      IExpression expression1 = (IExpression) new TermsExpression("collectionId", Operator.In, (IEnumerable<string>) new string[1]
      {
        requestContext.GetCollectionID().ToString().ToLowerInvariant()
      });
      expressionList.Add(expression1);
      if (projectInfo != null)
      {
        IExpression expression2 = (IExpression) new TermsExpression("projectId", Operator.In, (IEnumerable<string>) new string[1]
        {
          projectInfo.Id.ToString().ToLowerInvariant()
        });
        expressionList.Add(expression2);
      }
      Stopwatch stopwatch = Stopwatch.StartNew();
      try
      {
        IWorkItemSecurityChecksService service = requestContext.GetService<IWorkItemSecurityChecksService>();
        IEnumerable<ClassificationNode> userAccessibleAreas = this.GetUserAccessibleAreas(requestContext, service, out noResultAccessible);
        if (userAccessibleAreas != null)
          expressionList.Add((IExpression) new TermsExpression(WorkItemContract.PlatformFieldNames.AreaId, Operator.In, userAccessibleAreas.Select<ClassificationNode, string>((Func<ClassificationNode, string>) (x => x.Id.ToString((IFormatProvider) CultureInfo.InvariantCulture)))));
      }
      finally
      {
        stopwatch.Stop();
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishKpiAndCi("WitSecurityChecksTime", "Query Pipeline", (double) stopwatch.ElapsedMilliseconds);
      }
      IExpression expression3 = (IExpression) new TermsExpression(WorkItemContract.PlatformFieldNames.IsDeleted, Operator.In, (IEnumerable<string>) new string[1]
      {
        bool.FalseString.ToLowerInvariant()
      });
      string currentHostConfigValue = requestContext.GetCurrentHostConfigValue<string>("/Service/SearchShared/Settings/SoftDeletedProjectIds");
      if (!string.IsNullOrWhiteSpace(currentHostConfigValue))
        expressionList.Add((IExpression) new NotExpression((IExpression) new TermsExpression("projectId", Operator.In, (IEnumerable<string>) ((IEnumerable<string>) currentHostConfigValue.Split(',')).Select<string, string>((Func<string, string>) (i => i.Trim())).Where<string>((Func<string, bool>) (i => !string.IsNullOrEmpty(i))).ToList<string>())));
      if (requestContext.GetConfigValue<bool>("/Service/ALMSearch/Settings/WorkItemQueryFilterPerfFix"))
      {
        expressionList.Add(expression3);
        TermsExpression termsExpression = new TermsExpression("isDiscussionOnly", Operator.In, (IEnumerable<string>) new string[1]
        {
          bool.FalseString.ToLowerInvariant()
        });
        expressionList.Add((IExpression) termsExpression);
        return expressionList.Count != 1 ? (IExpression) new AndExpression((IEnumerable<IExpression>) expressionList) : expressionList[0];
      }
      IExpression expression4 = (IExpression) new OrExpression(new IExpression[2]
      {
        (IExpression) new MissingFieldExpression(WorkItemContract.PlatformFieldNames.IsDeleted),
        expression3
      });
      expressionList.Add(expression4);
      return expressionList.Count != 1 ? expressionList.Aggregate<IExpression>((System.Func<IExpression, IExpression, IExpression>) ((current, filter) => (IExpression) new AndExpression(new IExpression[2]
      {
        current,
        filter
      }))) : expressionList[0];
    }

    protected override void ValidateUserPermission(IVssRequestContext requestContext) => requestContext.GetService<IWorkItemSecurityChecksService>().ValidateAndSetUserPermissionsForSearchService(requestContext);

    private IEnumerable<ClassificationNode> GetUserAccessibleAreas(
      IVssRequestContext requestContext,
      IWorkItemSecurityChecksService securityChecksService,
      out bool noResultAccessible)
    {
      bool allAreasAreAccessible;
      IEnumerable<ClassificationNode> userAccessibleAreas = securityChecksService.GetUserAccessibleAreas(requestContext, out allAreasAreAccessible);
      noResultAccessible = !allAreasAreAccessible && !userAccessibleAreas.Any<ClassificationNode>();
      return !allAreasAreAccessible ? userAccessibleAreas : (IEnumerable<ClassificationNode>) null;
    }
  }
}
