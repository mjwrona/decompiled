// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Alerts.TfsSubscriptionAdapter
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Alerts, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0FF2CB39-6514-430A-A4E9-A45535A580D6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Alerts.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Core;
using Microsoft.VisualStudio.Services.Notifications;
using Microsoft.VisualStudio.Services.Notifications.Common;
using Microsoft.VisualStudio.Services.Notifications.Server;
using Microsoft.VisualStudio.Services.Notifications.WebApi;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Server.WebAccess.Alerts
{
  public abstract class TfsSubscriptionAdapter : XPathSubscriptionAdapter
  {
    private Guid m_scope = Guid.Empty;
    private string m_currentProjectUri;
    private bool m_allowUserRegexInMatchCondition = true;
    private bool m_adapterInitialized;
    protected const string s_area = "Notification";
    protected const string s_layer = "SubscriptionAdapters";
    protected static readonly List<string> EmptyStringList = new List<string>();
    protected static readonly Dictionary<string, string> EmptyLookup = new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.CurrentCultureIgnoreCase);

    public virtual bool IsLicensed(TfsWebContext tfsWebContext) => false;

    public SubscriptionType SubscriptionType { set; get; }

    public override string SubscriptionTypeName => this.SubscriptionType.ToString();

    public virtual string GetTeamProjectFieldName() => AlertsResources.FieldNameTeamProject;

    public override EventSerializerType GetSerializationFormatForEvent(
      IVssRequestContext requestContext,
      string eventType)
    {
      return EventSerializerType.Xml;
    }

    internal static TfsSubscriptionAdapter CreateAdapter(
      IVssRequestContext tfsRequestContext,
      TfsWebContext tfsWebContext,
      string subscriptionType)
    {
      subscriptionType = EventTypeMapper.ToLegacy(tfsRequestContext, subscriptionType);
      SubscriptionType result;
      if (!Enum.TryParse<SubscriptionType>(subscriptionType, true, out result))
        result = SubscriptionType.Unknown;
      return TfsSubscriptionAdapter.CreateAdapter(tfsRequestContext, tfsWebContext, result);
    }

    internal static TfsSubscriptionAdapter CreateAdapter(
      IVssRequestContext tfsRequestContext,
      TfsWebContext tfsWebContext,
      SubscriptionType subscriptionType)
    {
      TfsSubscriptionAdapter adapter = (TfsSubscriptionAdapter) null;
      foreach (ISubscriptionAdapter notificationExtention in (IEnumerable<ISubscriptionAdapter>) tfsRequestContext.GetStaticNotificationExtentions<ISubscriptionAdapter>())
      {
        if (notificationExtention is TfsSubscriptionAdapter)
        {
          TfsSubscriptionAdapter o = notificationExtention as TfsSubscriptionAdapter;
          if (o.SubscriptionType == subscriptionType)
          {
            adapter = ExtensionsUtil.CreateNewInstance((object) o) as TfsSubscriptionAdapter;
            break;
          }
        }
      }
      if (adapter == null)
        adapter = (TfsSubscriptionAdapter) new UnknownSubscriptionAdapter();
      adapter.SetCurrentProject(tfsWebContext);
      return adapter;
    }

    public Dictionary<string, ExpressionFilterField> GetFields(
      IVssRequestContext requestContext,
      TfsWebContext tfsWebContext)
    {
      if (!this.FieldInitialized)
        this.EnsureAdapterInitialized(requestContext, tfsWebContext);
      return this.GetFields(requestContext, SubscriptionScope.Default);
    }

    public override string ToConditionString(
      IVssRequestContext requestcontext,
      ExpressionFilterModel filter)
    {
      string conditionString = base.ToConditionString(requestcontext, filter);
      Guid? id = this.CurrentScope?.Id;
      Guid collectionScope = NotificationClientConstants.CollectionScope;
      if ((id.HasValue ? (id.HasValue ? (id.GetValueOrDefault() == collectionScope ? 1 : 0) : 1) : 0) != 0)
      {
        Guid projectId = this.GetProjectId(requestcontext, this.SubscriptionTypeName, conditionString);
        if (projectId != Guid.Empty)
        {
          if (this.RemoveTeamProjectClause(filter, true))
            conditionString = base.ToConditionString(requestcontext, filter);
          SubscriptionScope subscriptionScope = new SubscriptionScope();
          subscriptionScope.Id = projectId;
          this.CurrentScope = subscriptionScope;
        }
      }
      return conditionString;
    }

    protected override void LoadFields(IVssRequestContext requestContext)
    {
      this.ParseScope(requestContext);
      this.EnsureAdapterInitialized(requestContext, (TfsWebContext) null);
      this.FieldInitialized = true;
    }

    protected void ParseScope(IVssRequestContext requestContext)
    {
      if (this.IsCollectionScope())
        return;
      this.CurrentProjectName = this.GetProjectName(requestContext, this.CurrentScope.Id);
    }

    protected void SetCurrentProject(TfsWebContext tfsWebContext)
    {
      if (tfsWebContext == null || tfsWebContext.ProjectContext == null)
        return;
      this.CurrentProjectName = tfsWebContext.ProjectContext.Name;
      SubscriptionScope subscriptionScope = new SubscriptionScope();
      subscriptionScope.Id = tfsWebContext.ProjectContext.Id;
      this.CurrentScope = subscriptionScope;
    }

    protected bool IsCollectionScope() => this.CurrentScope == null || this.CurrentScope.Id == Guid.Empty || this.CurrentScope.Id == NotificationClientConstants.CollectionScope;

    protected ExpressionFilterField AddTeamProjectField(
      TfsWebContext tfsWebContext,
      string invariantFieldName,
      string localizedValue,
      IEnumerable<byte> operators)
    {
      return this.IsSubscriptionProjectScoped(tfsWebContext) ? this.AddFieldWithLookup(invariantFieldName, localizedValue, operators, (Func<IVssRequestContext, IDictionary<string, string>>) (s => this.GetProjectNameMacroLookup())) : this.AddField(invariantFieldName, localizedValue, operators);
    }

    protected bool IsSubscriptionProjectScoped(TfsWebContext tfsWebContext)
    {
      if (!string.IsNullOrEmpty(this.CurrentProjectName))
        return true;
      return tfsWebContext != null && tfsWebContext.ProjectContext != null;
    }

    protected virtual void InitializeAdapterTfs(
      IVssRequestContext tfsRequestContext,
      TfsWebContext tfsWebContext)
    {
      if (tfsRequestContext.ExecutionEnvironment.IsHostedDeployment)
        this.m_allowUserRegexInMatchCondition = tfsRequestContext.GetService<ITeamFoundationFeatureAvailabilityService>().IsFeatureEnabled(tfsRequestContext, "VisualStudio.Services.Notifications.AllowUserRegexInMatchCondition");
      if (tfsWebContext?.CurrentProjectUri != (Uri) null)
        this.CurrentProjectUri = tfsWebContext.CurrentProjectUri.AbsoluteUri;
      this.InitializeAdapter(tfsRequestContext);
    }

    protected IEnumerable<string> GetProjectNames()
    {
      if (string.IsNullOrEmpty(this.CurrentProjectName))
        return (IEnumerable<string>) new List<string>();
      return (IEnumerable<string>) new List<string>()
      {
        this.CurrentProjectName
      };
    }

    protected static bool CheckFeatureFlag(TfsWebContext tfsWebContext, Guid requiredFeature) => tfsWebContext != null && tfsWebContext.FeatureContext.IsFeatureAvailable(requiredFeature);

    public override NotificationSubscription ToNotificationSubscription(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Notifications.Server.Subscription subscription,
      SubscriptionQueryFlags queryFlags)
    {
      NotificationSubscription notificationSubscription = base.ToNotificationSubscription(requestContext, subscription, queryFlags);
      if (notificationSubscription.Scope.Id.Equals(Guid.Empty))
      {
        Guid projectId = this.GetProjectId(requestContext, subscription);
        if (projectId != Guid.Empty && projectId != subscription.ProjectId)
          subscription.ProjectId = projectId;
        notificationSubscription.Scope.Id = subscription.ProjectId != Guid.Empty ? subscription.ProjectId : NotificationClientConstants.CollectionScope;
        this.CurrentScope = notificationSubscription.Scope;
        if (queryFlags.HasFlag((Enum) SubscriptionQueryFlags.IncludeFilterDetails))
          this.RemoveTeamProjectClause(((ExpressionFilter) notificationSubscription.Filter).FilterModel, !this.IsCollectionScope());
      }
      return notificationSubscription;
    }

    public bool RemoveTeamProjectClause(ExpressionFilterModel filter, bool scoped)
    {
      ICollection<ExpressionFilterClause> expressionFilterClauses = (ICollection<ExpressionFilterClause>) new List<ExpressionFilterClause>();
      bool flag = false;
      foreach (ExpressionFilterClause clause in (IEnumerable<ExpressionFilterClause>) filter.Clauses)
      {
        if (!clause.FieldName.Equals(this.GetTeamProjectFieldName()) || !scoped || clause.Operator != "=")
        {
          clause.Index = expressionFilterClauses.Count + 1;
          if (!scoped && clause.Value.Equals("@@MyProjectName@@"))
            clause.Value = string.Empty;
          expressionFilterClauses.Add(clause);
        }
        else if (clause.FieldName.Equals(this.GetTeamProjectFieldName()))
        {
          flag = true;
          this.UpdateGroups(filter, clause.Index);
        }
      }
      filter.Clauses = expressionFilterClauses;
      return flag;
    }

    private void UpdateGroups(ExpressionFilterModel filterModel, int projectClauseIndex)
    {
      if (filterModel.Groups == null)
        return;
      foreach (ExpressionFilterGroup group in (IEnumerable<ExpressionFilterGroup>) filterModel.Groups)
      {
        if (projectClauseIndex < group.Start)
          --group.Start;
        if (projectClauseIndex < group.End)
          --group.End;
      }
    }

    private void EnsureAdapterInitialized(
      IVssRequestContext tfsRequestContext,
      TfsWebContext tfsWebContext)
    {
      if (this.m_adapterInitialized)
        return;
      this.InitializeAdapterTfs(tfsRequestContext, tfsWebContext);
      this.m_adapterInitialized = true;
    }

    protected string CurrentProjectUri
    {
      get
      {
        if (this.m_currentProjectUri == null && !this.IsCollectionScope())
          this.m_currentProjectUri = ProjectInfo.GetProjectUri(this.CurrentScope.Id);
        return this.m_currentProjectUri;
      }
      set => this.m_currentProjectUri = value;
    }

    protected internal string CurrentProjectName { get; set; }

    public string GetProjectName(IVssRequestContext requestContext, Guid projectId) => new SubscriptionProjectParser().GetProjectNameFromGuid(requestContext, projectId);

    public Guid GetProjectId(IVssRequestContext requestContext, Microsoft.VisualStudio.Services.Notifications.Server.Subscription subscription) => this.GetProjectId(requestContext, subscription.SubscriptionFilter.EventType, subscription.ConditionString);

    private Guid GetProjectId(
      IVssRequestContext requestContext,
      string eventType,
      string conditionString)
    {
      return new SubscriptionProjectParser().GetProjectId(requestContext, eventType, conditionString, Guid.Empty, true, out string _);
    }
  }
}
