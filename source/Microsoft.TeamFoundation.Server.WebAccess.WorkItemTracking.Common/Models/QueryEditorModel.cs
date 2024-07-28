// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.QueryEditorModel
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql;
using Microsoft.TeamFoundation.WorkItemTracking.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models
{
  [DataContract]
  public class QueryEditorModel
  {
    public QueryEditorModel(
      IVssRequestContext tfsRequestContext,
      string wiql,
      bool includeDisplayAndSortFields,
      IDictionary<string, int> columnSizeMap,
      string currentProjectName,
      bool useIsoDateFormat = false)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(tfsRequestContext, nameof (tfsRequestContext));
      ArgumentUtility.CheckStringForNullOrEmpty(wiql, nameof (wiql));
      this.Initialize(tfsRequestContext, wiql, includeDisplayAndSortFields, columnSizeMap, currentProjectName, useIsoDateFormat);
    }

    [IgnoreDataMember]
    public string Wiql { get; set; }

    [DataMember(Name = "fields", EmitDefaultValue = false)]
    public IEnumerable<QueryDisplayColumn> Fields { get; set; }

    [DataMember(Name = "sortFields", EmitDefaultValue = false)]
    public IEnumerable<QuerySortColumn> SortFields { get; set; }

    [DataMember(Name = "sourceFilter", EmitDefaultValue = false)]
    public QueryFilterModel SourceFilter { get; set; }

    [DataMember(Name = "linkTypes", EmitDefaultValue = false)]
    public string LinkTypes { get; set; }

    [DataMember(Name = "treeLinkTypes", EmitDefaultValue = false)]
    public string TreeLinkTypes { get; set; }

    [DataMember(Name = "linkTargetFilter", EmitDefaultValue = false)]
    public QueryFilterModel LinkTargetFilter { get; set; }

    [DataMember(Name = "treeTargetFilter", EmitDefaultValue = false)]
    public QueryFilterModel TreeTargetFilter { get; set; }

    [DataMember(Name = "mode", EmitDefaultValue = true)]
    public int QueryMode { get; set; }

    [DataMember(Name = "teamProject", EmitDefaultValue = false)]
    public string TeamProject { get; set; }

    private void Initialize(
      IVssRequestContext tfsRequestContext,
      string wiql,
      bool includeDisplayAndSortFields,
      IDictionary<string, int> columnSizeMap,
      string currentProjectName,
      bool useIsoDateFormat = false)
    {
      this.Wiql = wiql;
      NodeSelect wiqlNodes;
      Microsoft.TeamFoundation.WorkItemTracking.Internals.LinkQueryMode queryMode;
      string sourceFilter;
      string linkFilter;
      string targetFilter;
      WiqlHelper.SplitFilters(wiql, out wiqlNodes, out queryMode, out string _, out sourceFilter, out linkFilter, out targetFilter);
      this.QueryMode = (int) queryMode;
      QueryAdapter adapter = new QueryAdapter(tfsRequestContext, useIsoDateFormat);
      if (includeDisplayAndSortFields)
      {
        if (wiqlNodes.Fields != null)
        {
          WorkItemTrackingFieldService fieldTypes = tfsRequestContext.GetService<WorkItemTrackingFieldService>();
          this.Fields = (IEnumerable<QueryDisplayColumn>) wiqlNodes.Fields.Cast<NodeFieldName>().Select<NodeFieldName, QueryDisplayColumn>((Func<NodeFieldName, QueryDisplayColumn>) (nfn =>
          {
            string str = nfn.Value;
            try
            {
              FieldEntry field = fieldTypes.GetField(tfsRequestContext, str);
              return new QueryDisplayColumn()
              {
                Name = field.ReferenceName,
                Text = field.Name,
                FieldId = field.FieldId,
                CanSortBy = field.CanSortBy,
                IsIdentity = field.IsIdentity,
                Width = columnSizeMap == null || !columnSizeMap.ContainsKey(str) ? QueryResultModel.GetDefaultColumnWidth(field.FieldId, field.FieldType, field.IsIdentity) : columnSizeMap[str]
              };
            }
            catch (Exception ex)
            {
              TeamFoundationTracingService.TraceExceptionRaw(599999, TraceLevel.Error, "WebAccess", TfsTraceLayers.Controller, nameof (QueryEditorModel), ex);
              return new QueryDisplayColumn()
              {
                Name = str,
                Text = str,
                FieldId = int.MaxValue,
                CanSortBy = false,
                Width = columnSizeMap == null || !columnSizeMap.ContainsKey(str) ? QueryResultModel.GetDefaultColumnWidth(int.MaxValue, InternalFieldType.String) : columnSizeMap[str]
              };
            }
          })).ToArray<QueryDisplayColumn>();
        }
        if (wiqlNodes.OrderBy != null)
          this.SortFields = (IEnumerable<QuerySortColumn>) wiqlNodes.OrderBy.Cast<NodeFieldName>().Select<NodeFieldName, QuerySortColumn>((Func<NodeFieldName, QuerySortColumn>) (nfn => new QuerySortColumn()
          {
            FieldName = nfn.Value,
            Descending = nfn.Direction == Direction.Descending
          })).ToArray<QuerySortColumn>();
      }
      string str1 = (string) null;
      string str2 = (string) null;
      string str3 = (string) null;
      if (!string.IsNullOrEmpty(currentProjectName))
      {
        str3 = WiqlHelper.GetCommonTeamProject(adapter, sourceFilter);
        if (this.IsProjectNameForCurrentProject(adapter, currentProjectName, str3))
        {
          if (queryMode > Microsoft.TeamFoundation.WorkItemTracking.Internals.LinkQueryMode.WorkItems)
          {
            str2 = WiqlHelper.GetCommonTeamProject(adapter, targetFilter);
            if (this.IsProjectNameForCurrentProject(adapter, currentProjectName, str2))
              str1 = str3;
            else
              str2 = (string) null;
          }
          else
            str1 = str3;
        }
        if (string.IsNullOrEmpty(str1))
          str3 = (string) null;
      }
      this.TeamProject = str1;
      if (queryMode >= Microsoft.TeamFoundation.WorkItemTracking.Internals.LinkQueryMode.LinksRecursive)
      {
        this.TreeLinkTypes = adapter.GetSeparatedLinkTypeNames(linkFilter, true);
        this.SourceFilter = new QueryFilterModel(adapter, "Source", sourceFilter, str3);
        this.TreeTargetFilter = new QueryFilterModel(adapter, "Target", targetFilter, str2);
      }
      else if (queryMode > Microsoft.TeamFoundation.WorkItemTracking.Internals.LinkQueryMode.WorkItems)
      {
        this.LinkTypes = adapter.GetSeparatedLinkTypeNames(linkFilter, false);
        this.SourceFilter = new QueryFilterModel(adapter, "Source", sourceFilter, str3);
        this.LinkTargetFilter = new QueryFilterModel(adapter, "Target", targetFilter, str2);
      }
      else
        this.SourceFilter = new QueryFilterModel(adapter, string.Empty, sourceFilter, str3);
    }

    private bool IsProjectNameForCurrentProject(
      QueryAdapter adapter,
      string currentProject,
      string teamProjectName)
    {
      if (string.IsNullOrEmpty(currentProject) || string.IsNullOrEmpty(teamProjectName))
        return false;
      return adapter.Operators.IsProjectMacro(teamProjectName, false) || TFStringComparer.TeamProjectName.Equals(currentProject, teamProjectName);
    }
  }
}
