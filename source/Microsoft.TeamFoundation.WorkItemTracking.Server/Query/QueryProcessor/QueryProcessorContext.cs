// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Query.QueryProcessor.QueryProcessorContext
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccess;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Query.QueryProcessor
{
  internal class QueryProcessorContext
  {
    public bool m_hasDeletedFilter;
    public IVssRequestContext m_requestContext;
    public readonly bool m_supportsFullTextSearch;
    private IFieldTypeDictionary m_fieldSnapshot;
    private ITreeDictionary m_treeSnapshot;
    private WorkItemTrackingLinkService m_linkService;
    public List<SqlParameter> m_parameters;
    public List<ITableValuedParameter> m_tableValuedParameters;
    public bool m_isLeftJoinIndexHintEnabled;
    public bool m_isFullTextIndexHintEnabled;
    public bool m_isMoveOrClauseUpEnabled;
    public bool m_isForceOrderEnabled;
    public bool m_isAllowNonClusteredColumnstoreIndexEnabled;
    public bool m_isFullTextResultInTempTableEnabled;
    public bool m_isFullTextJoinOptimizationEnabled;
    public bool m_isQueryHashTagDisabled;
    public bool m_isFullTextQuery;
    public bool m_isLongTextPredicateAppended;
    public bool m_isEverPredicateAppended;
    public bool m_isIdentityComparisonQuery;
    public bool m_isIdentityInGroupQuery;
    public bool m_isLowerLevelOrClauseQuery;
    public bool m_isDirectlyJoinOnLongTable;
    public string m_asOfParam;
    public bool m_isParentQuery;
    public bool m_isAsOfQuery;
    public bool m_isChartingQuery;
    private readonly bool m_isQueryIdentityConstIdOptimizationEnabled;
    public string m_asOfDatesParam;
    public bool m_hasForwardLinkType;
    public bool m_hasReverseLinkType;
    public StringBuilder m_queryText;
    public int m_tempTableCounter;
    public bool m_sortRhs;
    public int m_unhandledTreeReferenceCount;
    public Dictionary<WorkItemTrackingTreeService.ClassificationNodeId, TreeReferenceType> m_treeReference;
    public readonly int m_queryMAXDOPValue;
    public readonly int m_queryMaxGrantPercent;
    public readonly int m_queryInThreshold;
    public readonly int m_topLevelOrOptimizationMaxClauseNumber;
    public Dictionary<int, string> m_dataspaceIdToProjectNameMap;
    public string m_dataspaceIdToProjectNameMapTVPName;
    private readonly bool m_isUsingRowNumberForSortingFixEnabled;
    public Dictionary<string, int> m_tempTableMap;
    public Dictionary<object, string> m_paramNames;

    public QueryProcessorContext(IVssRequestContext requestContext)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      this.m_requestContext = requestContext;
      this.m_isLeftJoinIndexHintEnabled = false;
      this.m_isFullTextIndexHintEnabled = false;
      this.m_isFullTextJoinOptimizationEnabled = false;
      this.m_isForceOrderEnabled = false;
      this.m_isFullTextResultInTempTableEnabled = false;
      this.m_isMoveOrClauseUpEnabled = false;
      this.m_supportsFullTextSearch = this.m_requestContext.WitContext().ServerSettings.FullTextEnabled;
      this.m_queryMAXDOPValue = this.m_requestContext.WitContext().ServerSettings.MaxQueryDOPValue;
      this.m_queryMaxGrantPercent = this.m_requestContext.WitContext().ServerSettings.QueryMaxGrantPercent;
      this.m_queryInThreshold = this.m_requestContext.WitContext().ServerSettings.QueryInThreshold;
      this.m_topLevelOrOptimizationMaxClauseNumber = this.m_requestContext.WitContext().ServerSettings.TopLevelOrOptimizationMaxClauseNumber;
      this.m_isLongTextPredicateAppended = false;
      this.m_isEverPredicateAppended = false;
      this.m_isChartingQuery = false;
      this.m_isIdentityComparisonQuery = false;
      this.m_isIdentityInGroupQuery = false;
      this.m_isLowerLevelOrClauseQuery = false;
      this.m_isDirectlyJoinOnLongTable = false;
      this.m_isFullTextQuery = false;
      this.m_isAllowNonClusteredColumnstoreIndexEnabled = requestContext.GetService<IQueryExperimentService>().GetValueForTargetExperiment<bool>(requestContext, QueryExperiment.AllowNonClusteredColumnstoreIndex, true, this.m_requestContext.IsFeatureEnabled("WorkItemTracking.Server.Queries.AllowNonClusteredColumnstoreIndex"));
      this.m_isQueryIdentityConstIdOptimizationEnabled = requestContext.GetService<IQueryExperimentService>().GetValueForTargetExperiment<bool>(requestContext, QueryExperiment.EnableQueryIdentityConstIdOptimization, true, this.m_requestContext.IsFeatureEnabled("WorkItemTracking.Server.Queries.EnableQueryIdentityConstIdOptimization"));
      this.m_isQueryHashTagDisabled = this.m_requestContext.IsFeatureEnabled("WorkItemTracking.Server.Queries.M174.DisableQueryHashTag");
      this.m_isUsingRowNumberForSortingFixEnabled = this.m_requestContext.IsFeatureEnabled("WorkItemTracking.Server.UsingRowNumberForSortingFixEnabled");
      this.FieldsDoStringComparison = new HashSet<string>();
    }

    public void Reset()
    {
      this.m_asOfParam = (string) null;
      this.m_asOfDatesParam = (string) null;
      this.m_isAsOfQuery = false;
      this.m_isChartingQuery = false;
      this.m_isIdentityComparisonQuery = false;
      this.m_isIdentityInGroupQuery = false;
      this.m_isLowerLevelOrClauseQuery = false;
      this.m_isDirectlyJoinOnLongTable = false;
      this.m_isFullTextQuery = false;
      this.m_parameters = new List<SqlParameter>();
      this.m_tableValuedParameters = new List<ITableValuedParameter>();
      this.m_dataspaceIdToProjectNameMap = (Dictionary<int, string>) null;
      this.m_dataspaceIdToProjectNameMapTVPName = (string) null;
      this.m_hasForwardLinkType = false;
      this.m_hasReverseLinkType = false;
      this.m_queryText = new StringBuilder(1024);
      this.m_tempTableCounter = 0;
      this.m_sortRhs = false;
      this.m_unhandledTreeReferenceCount = 0;
      this.m_treeReference = new Dictionary<WorkItemTrackingTreeService.ClassificationNodeId, TreeReferenceType>();
      this.m_tempTableMap = new Dictionary<string, int>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      this.m_paramNames = new Dictionary<object, string>();
      this.m_isLongTextPredicateAppended = false;
      this.m_isEverPredicateAppended = false;
      this.m_isParentQuery = false;
      this.FieldsDoStringComparison = new HashSet<string>();
    }

    public bool IsLeftJoinIndexHintEnabled => this.m_isLeftJoinIndexHintEnabled;

    public bool IsFullTextIndexHintEnabled => this.m_isFullTextIndexHintEnabled;

    public bool IsQueryIdentityConstIdOptimizationEnabled => this.m_isQueryIdentityConstIdOptimizationEnabled;

    public bool IsQueryMAXDOPCapped => this.m_queryMAXDOPValue > 0;

    public bool IsQueryMaxGrantCapped => this.m_queryMaxGrantPercent > 0;

    public bool IsForceOrderEnabled => !this.m_isFullTextIndexHintEnabled && this.m_isForceOrderEnabled;

    public bool IsAllowNonClusteredColumnstoreIndexEnabled => this.m_isAllowNonClusteredColumnstoreIndexEnabled;

    public bool IsFullTextResultInTempTableEnabled => this.m_isFullTextResultInTempTableEnabled;

    public bool IsFullTextJoinOptimizationEnabled => this.m_isFullTextJoinOptimizationEnabled;

    public bool IsLongTextPredicateAppended => this.m_isLongTextPredicateAppended;

    public bool IsUsingRowNumberForSortingFixEnabled => this.m_isUsingRowNumberForSortingFixEnabled;

    public string PartitionId => this.m_requestContext.ServiceHost.PartitionId.ToString();

    public IFieldTypeDictionary FieldSnapshot
    {
      get
      {
        if (this.m_fieldSnapshot == null)
          this.m_fieldSnapshot = this.m_requestContext.GetService<WorkItemTrackingFieldService>().GetFieldsSnapshot(this.m_requestContext);
        return this.m_fieldSnapshot;
      }
    }

    public ITreeDictionary TreeSnapshot
    {
      get
      {
        if (this.m_treeSnapshot == null)
          this.m_treeSnapshot = this.m_requestContext.GetService<WorkItemTrackingTreeService>().GetSnapshot(this.m_requestContext);
        return this.m_treeSnapshot;
      }
    }

    public WorkItemTrackingLinkService LinkService
    {
      get
      {
        if (this.m_linkService == null)
          this.m_linkService = this.m_requestContext.GetService<WorkItemTrackingLinkService>();
        return this.m_linkService;
      }
    }

    public HashSet<string> FieldsDoStringComparison { get; private set; }
  }
}
