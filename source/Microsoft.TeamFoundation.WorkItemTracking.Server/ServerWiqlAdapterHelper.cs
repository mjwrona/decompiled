// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.ServerWiqlAdapterHelper
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Client.QueryLanguage;
using Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql;
using Microsoft.TeamFoundation.WorkItemTracking.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Common.Metadata;
using Microsoft.TeamFoundation.WorkItemTracking.Internals;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Identity;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Legacy;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  internal class ServerWiqlAdapterHelper : IWiqlAdapterHelper
  {
    private string c_forwardLinkTypeSuffix = "-Forward";
    private string c_reverseLinkTypeSuffix = "-Reverse";
    private IVssRequestContext m_context;
    protected string m_userDisplayName;
    private WorkItemTrackingFieldService m_fields;
    private WorkItemTrackingLinkService m_links;
    protected TimeZone m_timeZone;
    protected CultureInfo m_userAwareCultureInfo;
    private ProjectInfo m_project;
    private WebApiTeam m_team;
    private bool m_validationOnly;

    public ServerWiqlAdapterHelper(
      IVssRequestContext context,
      ProjectInfo project = null,
      WebApiTeam team = null,
      bool validationOnly = false)
    {
      this.m_context = context;
      this.m_project = project;
      this.m_team = team;
      this.m_validationOnly = validationOnly;
    }

    public CultureInfo CultureInfo => CultureInfo.CurrentCulture;

    public TimeZone TimeZone
    {
      get
      {
        if (this.m_timeZone == null)
          this.RetrieveUserAndAccountPreferences();
        return this.m_timeZone;
      }
    }

    private void RetrieveUserAndAccountPreferences()
    {
      TimeZoneInfo timeZone = this.m_context.GetService<IUserPreferencesService>().GetUserPreferences(this.m_context).TimeZone;
      if (timeZone != null)
      {
        this.m_timeZone = (TimeZone) new CustomTimeZone(timeZone);
        this.m_context.Trace(908601, TraceLevel.Info, "Query", "QueryProcessor", "Using User Profile TimeZone: {0}.", (object) this.m_timeZone.StandardName);
      }
      else
      {
        TimeZoneInfo collectionTimeZone = this.m_context.GetService<ICollectionPreferencesService>().GetCollectionTimeZone(this.m_context);
        if (collectionTimeZone != null)
        {
          this.m_timeZone = (TimeZone) new CustomTimeZone(collectionTimeZone);
          this.m_context.Trace(908602, TraceLevel.Info, "Query", "QueryProcessor", "Using Account Profile TimeZone: {0}.", (object) this.m_timeZone.StandardName);
        }
        else
        {
          this.m_timeZone = TimeZone.CurrentTimeZone;
          this.m_context.Trace(908603, TraceLevel.Info, "Query", "QueryProcessor", "Using Server TimeZone: {0}.", (object) this.m_timeZone.StandardName);
        }
      }
    }

    public string UserDisplayName
    {
      get
      {
        if (this.m_userDisplayName == null)
          this.m_userDisplayName = this.m_context.WitContext().RequestIdentity.GetLegacyDistinctDisplayName();
        return this.m_userDisplayName;
      }
    }

    public int GetTreeID(string path, TreeStructureType type) => this.m_context.WitContext().TreeService.LegacyGetTreeNodeIdFromPath(this.m_context, path, type);

    public bool IsSupported(string feature) => true;

    public object FindField(string name, string prefix, object tableTag)
    {
      FieldEntry field;
      if (!this.Fields.TryGetField(this.m_context, name, out field))
        return (object) null;
      if (tableTag == null)
        return (object) field;
      InternalFieldUsages internalFieldUsages = InternalFieldUsages.None;
      if ((Microsoft.TeamFoundation.WorkItemTracking.Internals.LinkQueryMode) tableTag == Microsoft.TeamFoundation.WorkItemTracking.Internals.LinkQueryMode.WorkItems)
      {
        if (string.IsNullOrEmpty(prefix))
          internalFieldUsages = field.Usage != InternalFieldUsages.WorkItemTypeExtension ? InternalFieldUsages.WorkItem : InternalFieldUsages.WorkItemTypeExtension;
      }
      else if (string.IsNullOrEmpty(prefix))
        internalFieldUsages = InternalFieldUsages.WorkItemLink;
      else if (string.Equals(prefix, "Source", StringComparison.OrdinalIgnoreCase) || string.Equals(prefix, "Target", StringComparison.OrdinalIgnoreCase))
        internalFieldUsages = field.Usage != InternalFieldUsages.WorkItemTypeExtension ? InternalFieldUsages.WorkItem : InternalFieldUsages.WorkItemTypeExtension;
      return internalFieldUsages != field.Usage ? (object) null : (object) field;
    }

    public int GetFieldId(object fieldTag) => (fieldTag as FieldEntry).FieldId;

    public string GetFieldReferenceName(object fieldTag) => (fieldTag as FieldEntry).ReferenceName;

    public string GetFieldFriendlyName(object fieldTag) => (fieldTag as FieldEntry).Name;

    public bool GetFieldIsQueryable(object fieldTag) => (fieldTag as FieldEntry).IsQueryable;

    public bool GetFieldCanSortBy(object fieldTag) => (fieldTag as FieldEntry).CanSortBy;

    public InternalFieldUsages GetFieldUsage(object fieldTag) => (fieldTag as FieldEntry).Usage;

    public InternalFieldType GetFieldType(object fieldTag) => (fieldTag as FieldEntry).FieldType;

    public Type GetFieldSystemType(object fieldTag) => (fieldTag as FieldEntry).SystemType;

    public bool GetFieldSupportsTextQuery(object fieldTag) => (fieldTag as FieldEntry).SupportsTextQuery;

    public bool GetFieldIsLongText(object fieldTag) => (fieldTag as FieldEntry).IsLongText;

    public string GetFieldFriendlyName(string fieldName) => this.Fields.GetField(this.m_context, fieldName).Name;

    public int GetFieldPsFieldType(string fieldName) => this.Fields.GetField(this.m_context, fieldName).FieldDataType;

    public InternalFieldType GetFieldType(string fieldName) => this.Fields.GetField(this.m_context, fieldName).FieldType;

    public bool HasLinkType(string linkTypeName)
    {
      bool isReverse;
      MDWorkItemLinkType linkType;
      if (this.TryGetLinkTypeByReferenceName(linkTypeName, out isReverse, out linkType))
      {
        if (linkType.IsRemote)
          return false;
        return !isReverse || linkType.IsDirectional;
      }
      bool linkTypeByName = this.LinkTypes.TryGetLinkTypeByName(this.m_context, linkTypeName, out linkType);
      return !linkTypeByName ? linkTypeByName : !linkType.IsRemote;
    }

    public int GetLinkTypeId(string linkTypeName)
    {
      bool isReverse;
      MDWorkItemLinkType linkType;
      if (this.TryGetLinkTypeByReferenceName(linkTypeName, out isReverse, out linkType))
      {
        if (linkType.IsRemote)
          throw new WorkItemTrackingLinkTypeNotSupportedException(linkTypeName);
        return isReverse ? linkType.ReverseId : linkType.ForwardId;
      }
      WorkItemLinkTypeEnd linkTypeEndByName = this.LinkTypes.GetLinkTypeEndByName(this.m_context, linkTypeName);
      if (linkTypeEndByName.LinkType.IsRemote)
        throw new WorkItemTrackingLinkTypeNotSupportedException(linkTypeName);
      return linkTypeEndByName.Id;
    }

    public bool GetLinkTypeIsForward(string linkTypeName)
    {
      WorkItemLinkTypeEnd linkType;
      if (this.LinkTypes.TryGetLinkTypeEndByName(this.m_context, linkTypeName, out linkType))
      {
        if (linkType.LinkType.IsRemote)
          throw new WorkItemTrackingLinkTypeNotSupportedException(linkTypeName);
        return linkType.IsForwardEnd;
      }
      bool isReverse;
      this.GetReferenceLinkTypeNameFromRefWithSuffixName(linkTypeName, out isReverse);
      return !isReverse;
    }

    public int GetLinkTypeTopology(string linkTypeName)
    {
      MDWorkItemLinkType linkType;
      if (this.TryGetLinkTypeByReferenceName(linkTypeName, out bool _, out linkType))
        return !linkType.IsRemote ? linkType.InternalLinkTopologies : throw new WorkItemTrackingLinkTypeNotSupportedException(linkTypeName);
      MDWorkItemLinkType linkTypeByName = this.LinkTypes.GetLinkTypeByName(this.m_context, linkTypeName);
      return !linkTypeByName.IsRemote ? linkTypeByName.InternalLinkTopologies : throw new WorkItemTrackingLinkTypeNotSupportedException(linkTypeName);
    }

    public IEnumerable<int> GetAllLinkTypeIds() => this.LinkTypes.GetLinkTypeIds(this.m_context, false);

    public List<object> GetSortFieldList(NodeSelect nodeSelect)
    {
      List<object> sortFieldList = new List<object>();
      NodeFieldList orderBy = nodeSelect.OrderBy;
      if (orderBy != null)
      {
        for (int i = 0; i < orderBy.Count; ++i)
        {
          NodeFieldName nodeFieldName = orderBy[i];
          string referenceName = ((FieldEntry) nodeFieldName.Tag).ReferenceName;
          sortFieldList.Add((object) new QuerySortOrderEntry()
          {
            Ascending = (nodeFieldName.Direction != Direction.Descending),
            NullsFirst = nodeFieldName.NullsFirst,
            ColumnName = referenceName
          });
        }
      }
      return sortFieldList;
    }

    public List<object> GetDisplayFieldList(NodeSelect nodeSelect)
    {
      List<object> displayFieldList = new List<object>();
      NodeFieldList fields = nodeSelect.Fields;
      if (fields != null)
      {
        for (int i = 0; i < fields.Count; ++i)
        {
          NodeFieldName nodeFieldName = fields[i];
          displayFieldList.Add((object) ((FieldEntry) nodeFieldName.Tag).ReferenceName);
        }
      }
      else if ((Microsoft.TeamFoundation.WorkItemTracking.Internals.LinkQueryMode) nodeSelect.From.Tag == Microsoft.TeamFoundation.WorkItemTracking.Internals.LinkQueryMode.WorkItems)
      {
        foreach (FieldEntry allField in this.Fields.GetAllFields(this.m_context))
        {
          if (!allField.IsIgnored && (allField.Usage & InternalFieldUsages.WorkItem) != InternalFieldUsages.None)
            displayFieldList.Add((object) allField.ReferenceName);
        }
      }
      else
      {
        displayFieldList.Add((object) "System.Id");
        displayFieldList.Add((object) "System.Links.LinkType");
      }
      return displayFieldList;
    }

    public void SetDisplayFieldList(NodeSelect nodeSelect, IEnumerable<object> list) => throw new NotImplementedException();

    public void SetSortFieldList(NodeSelect nodeSelect, IEnumerable<object> list) => throw new NotImplementedException();

    private bool TryGetLinkTypeByReferenceName(
      string linkTypeRefName,
      out bool isReverse,
      out MDWorkItemLinkType linkType)
    {
      return this.LinkTypes.TryGetLinkTypeByReferenceName(this.m_context, this.GetReferenceLinkTypeNameFromRefWithSuffixName(linkTypeRefName, out isReverse), out linkType);
    }

    private WorkItemTrackingFieldService Fields
    {
      get
      {
        if (this.m_fields == null)
          this.m_fields = this.m_context.GetService<WorkItemTrackingFieldService>();
        return this.m_fields;
      }
    }

    private WorkItemTrackingLinkService LinkTypes
    {
      get
      {
        if (this.m_links == null)
          this.m_links = this.m_context.GetService<WorkItemTrackingLinkService>();
        return this.m_links;
      }
    }

    private string GetReferenceLinkTypeNameFromRefWithSuffixName(
      string linkTypeName,
      out bool isReverse)
    {
      string refWithSuffixName = linkTypeName;
      isReverse = false;
      if (linkTypeName.EndsWith(this.c_forwardLinkTypeSuffix, StringComparison.Ordinal))
        refWithSuffixName = linkTypeName.Substring(0, linkTypeName.Length - this.c_forwardLinkTypeSuffix.Length);
      else if (linkTypeName.EndsWith(this.c_reverseLinkTypeSuffix, StringComparison.Ordinal))
      {
        refWithSuffixName = linkTypeName.Substring(0, linkTypeName.Length - this.c_reverseLinkTypeSuffix.Length);
        isReverse = true;
      }
      return refWithSuffixName;
    }

    public virtual DataType GetVariableType(string name) => this.m_context.To(TeamFoundationHostType.Deployment).GetService<QueryMacroExtensionService>().GetDataType(name);

    public bool DoesMacroExtensionHandleOffset(string macroName) => this.m_context.To(TeamFoundationHostType.Deployment).GetService<QueryMacroExtensionService>().DoesMacroExtensionHandleOffset(macroName);

    public virtual object GetVariableValue(string name, NodeParameters parameters)
    {
      object defaultValue = (object) null;
      QueryMacroExtensionService service = this.m_context.To(TeamFoundationHostType.Deployment).GetService<QueryMacroExtensionService>();
      return this.m_validationOnly && service.IsSupportedMacro(this.m_context, name, out defaultValue) ? defaultValue : service.GetValue(this.m_context, name, this.m_project, this.m_team, parameters, this.TimeZone, this.CultureInfo);
    }

    public bool RewriteCondition(NodeCondition condition, out Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node rewritten) => this.m_context.To(TeamFoundationHostType.Deployment).GetService<QueryMacroExtensionService>().RewriteCondition(this.m_context, condition, out rewritten);

    public void ValidateParameters(
      string macroName,
      NodeTableName tableContext,
      NodeFieldName fieldContext,
      NodeParameters parameters)
    {
      this.m_context.To(TeamFoundationHostType.Deployment).GetService<QueryMacroExtensionService>().ValidateParameters(this.m_context, macroName, tableContext, fieldContext, parameters);
    }

    public bool IsSupportedMacro(string name)
    {
      object defaultValue = (object) null;
      return this.m_context.To(TeamFoundationHostType.Deployment).GetService<QueryMacroExtensionService>().IsSupportedMacro(this.m_context, name, out defaultValue);
    }
  }
}
