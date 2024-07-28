// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItemTrackingRequestContext
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.Azure.Boards.ProcessTemplates;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Identity;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.Rules;
using Microsoft.TeamFoundation.WorkItemTracking.Server.ProcessValidator;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  public class WorkItemTrackingRequestContext
  {
    private Lazy<IFieldTypeDictionary> m_lazyFieldDictionary;
    private Lazy<ITreeDictionary> m_lazyTreeDictionary;
    private Lazy<IRuleMembershipFilter> m_lazyRuleMembershipFilter;
    private Lazy<IServerDefaultValueTransformer> m_lazyServerDefaultValueTransformer;
    internal IWITProcessReadPermissionCheckHelper m_processReadPermissionChecker;
    internal IPermissionCheckHelper m_workItemPermissionChecker;
    internal IPermissionCheckHelper m_workItemProjectPermissionChecker;
    private Lazy<WorkItemTrackingLinkService> m_lazyLinkTypeDictionary;
    private Lazy<IdentityService> m_lazyIdentityService;
    private Lazy<Microsoft.VisualStudio.Services.Identity.Identity> m_lazyRequestIdentity;
    private Lazy<IWorkItemTrackingConfigurationInfo> m_lazyWitConfigurationInfo;
    private Lazy<bool> m_lazyIsAadEnabledAccount;
    private Lazy<IProjectService> m_lazyProjectService;
    private Lazy<ITeamFoundationProcessService> m_lazyProcessService;
    private Lazy<bool> m_lazyIsCollectionAdministrator;
    private Dictionary<string, object> m_cache;

    public WorkItemTrackingRequestContext(IVssRequestContext requestContext)
    {
      this.RequestContext = requestContext;
      this.m_lazyFieldDictionary = new Lazy<IFieldTypeDictionary>((Func<IFieldTypeDictionary>) (() => this.GetFieldDictionary()));
      this.m_lazyTreeDictionary = new Lazy<ITreeDictionary>((Func<ITreeDictionary>) (() => this.GetTreeDictionary()));
      this.m_lazyLinkTypeDictionary = new Lazy<WorkItemTrackingLinkService>((Func<WorkItemTrackingLinkService>) (() => this.GetLinkTypeDictionary()));
      this.m_lazyRuleMembershipFilter = new Lazy<IRuleMembershipFilter>((Func<IRuleMembershipFilter>) (() => this.GetWorkItemRuleFilter()));
      this.m_lazyServerDefaultValueTransformer = new Lazy<IServerDefaultValueTransformer>((Func<IServerDefaultValueTransformer>) (() => this.GetServerValueTransformer()));
      this.m_lazyIdentityService = new Lazy<IdentityService>((Func<IdentityService>) (() => this.RequestContext.GetService<IdentityService>()));
      this.m_lazyRequestIdentity = new Lazy<Microsoft.VisualStudio.Services.Identity.Identity>((Func<Microsoft.VisualStudio.Services.Identity.Identity>) (() => this.RequestContext.GetUserIdentity()));
      this.m_lazyWitConfigurationInfo = new Lazy<IWorkItemTrackingConfigurationInfo>((Func<IWorkItemTrackingConfigurationInfo>) (() => this.RequestContext.GetService<WorkItemTrackingConfigurationSettingService>().GetConfigurationInfo(this.RequestContext)));
      this.m_lazyIsAadEnabledAccount = new Lazy<bool>((Func<bool>) (() => new Microsoft.TeamFoundation.WorkItemTracking.Server.Identity.AadIdentityHelper().IsAccountAadEnabled(this.RequestContext)));
      this.m_lazyProjectService = new Lazy<IProjectService>((Func<IProjectService>) (() => this.RequestContext.GetService<IProjectService>()));
      this.m_lazyProcessService = new Lazy<ITeamFoundationProcessService>((Func<ITeamFoundationProcessService>) (() => this.RequestContext.GetService<ITeamFoundationProcessService>()));
      this.m_lazyIsCollectionAdministrator = new Lazy<bool>((Func<bool>) (() => this.IdentityService.IsMember(this.RequestContext, GroupWellKnownIdentityDescriptors.NamespaceAdministratorsGroup, this.RequestContext.UserContext)));
    }

    public IVssRequestContext RequestContext { get; private set; }

    public IFieldTypeDictionary FieldDictionary => this.m_lazyFieldDictionary.Value;

    public virtual ITreeDictionary TreeService => this.m_lazyTreeDictionary.Value;

    public WorkItemTrackingLinkService LinkService => this.m_lazyLinkTypeDictionary.Value;

    public IRuleMembershipFilter RuleMembershipFilter => this.m_lazyRuleMembershipFilter.Value;

    public IServerDefaultValueTransformer ServerDefaultValueTransformer => this.m_lazyServerDefaultValueTransformer.Value;

    public IWITProcessReadPermissionCheckHelper ProcessReadPermissionChecker => this.m_processReadPermissionChecker ?? (this.m_processReadPermissionChecker = this.GetProcessReadPermissionChecker());

    public IPermissionCheckHelper WorkItemPermissionChecker => this.m_workItemPermissionChecker ?? (this.m_workItemPermissionChecker = this.GetWorkItemPermissionChecker());

    public IPermissionCheckHelper WorkItemProjectPermissionChecker => this.m_workItemProjectPermissionChecker ?? (this.m_workItemProjectPermissionChecker = this.GetWorkItemProjectPermissionChecker());

    public IdentityService IdentityService => this.m_lazyIdentityService.Value;

    public Microsoft.VisualStudio.Services.Identity.Identity RequestIdentity => this.m_lazyRequestIdentity.Value;

    public virtual IWorkItemTrackingConfigurationInfo ServerSettings => this.m_lazyWitConfigurationInfo.Value;

    public IWitProcessTemplateValidatorConfiguration TemplateValidatorConfiguration => this.ServerSettings.ValidatorConfiguration;

    public IProjectService ProjectService => this.m_lazyProjectService.Value;

    public ITeamFoundationProcessService ProcessService => this.m_lazyProcessService.Value;

    public bool IsCollectionAdministrator => this.m_lazyIsCollectionAdministrator.Value;

    private IPermissionCheckHelper GetWorkItemPermissionChecker() => (IPermissionCheckHelper) new PermissionCheckHelper(this.RequestContext);

    private IPermissionCheckHelper GetWorkItemProjectPermissionChecker() => (IPermissionCheckHelper) new WorkItemProjectPermissionCheckHelper(this.RequestContext);

    private IWITProcessReadPermissionCheckHelper GetProcessReadPermissionChecker() => (IWITProcessReadPermissionCheckHelper) new WITProcessReadPermissionCheckHelper(this.RequestContext);

    private IServerDefaultValueTransformer GetServerValueTransformer() => (IServerDefaultValueTransformer) new Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.ServerDefaultValueTransformer(this.RequestContext);

    private IRuleMembershipFilter GetWorkItemRuleFilter() => (IRuleMembershipFilter) new Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.Rules.RuleMembershipFilter(this.RequestContext);

    private WorkItemTrackingLinkService GetLinkTypeDictionary() => this.RequestContext.GetService<WorkItemTrackingLinkService>();

    private ITreeDictionary GetTreeDictionary() => (ITreeDictionary) new LockFreeTreeDictionary(this.RequestContext);

    private IFieldTypeDictionary GetFieldDictionary() => (IFieldTypeDictionary) new LockFreeFieldDictionary(this.RequestContext);

    public MetadataDBStamps MetadataDbStamps(IEnumerable<MetadataTable> tableNames) => this.RequestContext.MetadataDbStamps().SubSet(tableNames);

    public T GetCacheItem<T>(string key)
    {
      ArgumentUtility.CheckForNull<string>(key, nameof (key));
      return (T) this.m_cache[key];
    }

    public bool TryGetCacheItem<T>(string key, out T cachedItem)
    {
      ArgumentUtility.CheckForNull<string>(key, nameof (key));
      if (this.m_cache != null)
        return this.m_cache.TryGetValue<T>(key, out cachedItem);
      cachedItem = default (T);
      return false;
    }

    public bool TryGetOrAddCacheItem<T>(
      string key,
      Func<Action<T>, bool> valueFactory,
      out T value)
    {
      ArgumentUtility.CheckForNull<string>(key, nameof (key));
      ArgumentUtility.CheckForNull<Func<Action<T>, bool>>(valueFactory, nameof (valueFactory));
      if (this.m_cache == null)
      {
        this.m_cache = new Dictionary<string, object>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      }
      else
      {
        object obj;
        if (this.m_cache.TryGetValue(key, out obj))
        {
          value = (T) obj;
          return true;
        }
      }
      T valueToReturn = default (T);
      int num = valueFactory((Action<T>) (valueToCache =>
      {
        this.m_cache.Add(key, (object) valueToCache);
        valueToReturn = valueToCache;
      })) ? 1 : 0;
      value = valueToReturn;
      return num != 0;
    }

    public T GetOrAddCacheItem<T>(string key, Func<T> valueFactory)
    {
      ArgumentUtility.CheckForNull<string>(key, nameof (key));
      ArgumentUtility.CheckForNull<Func<T>>(valueFactory, nameof (valueFactory));
      if (this.m_cache == null)
      {
        this.m_cache = new Dictionary<string, object>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      }
      else
      {
        object orAddCacheItem;
        if (this.m_cache.TryGetValue(key, out orAddCacheItem))
          return (T) orAddCacheItem;
      }
      T orAddCacheItem1 = valueFactory();
      this.m_cache.Add(key, (object) orAddCacheItem1);
      return orAddCacheItem1;
    }

    public virtual bool IsAadBackedAccount => this.m_lazyIsAadEnabledAccount.Value;

    public bool IsAdmin()
    {
      bool flag = this.RequestContext.IsSystemContext;
      if (!flag)
        flag = ServicePrincipals.IsServicePrincipal(this.RequestContext, this.RequestContext.UserContext);
      if (!flag)
      {
        flag = this.IdentityService.IsMemberOrSame(this.RequestContext.Elevate(), GroupWellKnownIdentityDescriptors.NamespaceAdministratorsGroup);
        if (!flag)
          flag = this.IdentityService.IsMemberOrSame(this.RequestContext.Elevate(), GroupWellKnownIdentityDescriptors.ServiceUsersGroup);
      }
      return flag;
    }

    public IDictionary<string, Microsoft.VisualStudio.Services.Identity.Identity> GetIdentityMap() => this.GetOrAddCacheItem<IDictionary<string, Microsoft.VisualStudio.Services.Identity.Identity>>("IdentityMap", (Func<IDictionary<string, Microsoft.VisualStudio.Services.Identity.Identity>>) (() => (IDictionary<string, Microsoft.VisualStudio.Services.Identity.Identity>) new Dictionary<string, Microsoft.VisualStudio.Services.Identity.Identity>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase)));

    public string GetProjectName(Guid projectId) => this.GetOrAddCacheItem<string>("ProjectName/" + projectId.ToString(), (Func<string>) (() =>
    {
      try
      {
        return this.ProjectService.GetProjectName(this.RequestContext.Elevate(), projectId);
      }
      catch (ProjectDoesNotExistException ex)
      {
        IList<ProjectInfo> projectHistory = this.ProjectService.GetProjectHistory(this.RequestContext.Elevate(), projectId);
        if (projectHistory != null && projectHistory.Count > 0)
          return projectHistory.Last<ProjectInfo>().Name;
        throw;
      }
    }));

    internal static class WellKnownCacheKeys
    {
      public const string RulesCacheRefreshed = "RulesCacheRefreshed";
      public const string TokensToReplace = "TokensToReplace";
      public const string UrlsToReplace = "UrlsToReplace";
      public const string IdentityMap = "IdentityMap";
    }
  }
}
