// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.Rules.RuleMembershipFilter
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Identity;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.Rules
{
  public class RuleMembershipFilter : IRuleMembershipFilter, IWorkItemRuleFilter
  {
    private Lazy<Dictionary<string, bool>> m_stringCache = new Lazy<Dictionary<string, bool>>((Func<Dictionary<string, bool>>) (() => new Dictionary<string, bool>()));
    private Lazy<Dictionary<Guid, bool>> m_vsIdCache = new Lazy<Dictionary<Guid, bool>>((Func<Dictionary<Guid, bool>>) (() => new Dictionary<Guid, bool>()));

    public RuleMembershipFilter(IVssRequestContext requestContext) => this.TfsRequestContext = requestContext;

    public IVssRequestContext TfsRequestContext { get; set; }

    public bool IsCurrentUserMemberOfGroup(Guid group)
    {
      bool flag1;
      if (this.m_vsIdCache.Value.TryGetValue(group, out flag1))
        return flag1;
      IdentityService service = this.TfsRequestContext.GetService<IdentityService>();
      Microsoft.VisualStudio.Services.Identity.Identity identity = service.ReadIdentities(this.TfsRequestContext, (IList<Guid>) new Guid[1]
      {
        group
      }, QueryMembership.None, (IEnumerable<string>) null).FirstOrDefault<Microsoft.VisualStudio.Services.Identity.Identity>();
      bool flag2 = identity != null && service.IsMemberOrSame(this.TfsRequestContext, identity.Descriptor);
      this.m_vsIdCache.Value[group] = flag2;
      return flag2;
    }

    public bool IsCurrentUserMemberOfGroup(string group)
    {
      bool flag1;
      if (this.m_stringCache.Value.TryGetValue(group, out flag1))
        return flag1;
      IdentityService service = this.TfsRequestContext.GetService<IdentityService>();
      Microsoft.VisualStudio.Services.Identity.Identity identity = service.ReadIdentities(this.TfsRequestContext, IdentitySearchFilter.AccountName, this.ConvertGroupToSearchFactor(this.TfsRequestContext, group), QueryMembership.None, (IEnumerable<string>) null).FirstOrDefault<Microsoft.VisualStudio.Services.Identity.Identity>();
      bool flag2 = identity != null && service.IsMemberOrSame(this.TfsRequestContext, identity.Descriptor);
      this.m_stringCache.Value[group] = flag2;
      return flag2;
    }

    public string ConvertGroupToSearchFactor(IVssRequestContext requestContext, string group)
    {
      string searchFactor = group;
      if (group.Length > 36)
      {
        string[] strArray = group.Split('\\');
        Guid result = Guid.Empty;
        if (Guid.TryParse(strArray[0], out result))
        {
          string str;
          if (StringComparer.OrdinalIgnoreCase.Equals(strArray[0], "488bb442-0beb-4c1e-98b6-4eddc604bd9e"))
            str = TFCommonUtil.GetIdentityDomainScope(requestContext.ServiceHost.InstanceId);
          else if (StringComparer.OrdinalIgnoreCase.Equals(strArray[0], "b36ad70a-0d79-49c8-a165-30b643926fef"))
            str = TFCommonUtil.GetIdentityDomainScope(requestContext.ServiceHost.OrganizationServiceHost.InstanceId);
          else if (this.IsProjectScoped(result))
          {
            str = LinkingUtilities.EncodeUri(new ArtifactId("Classification", "TeamProject", strArray[0]));
          }
          else
          {
            Guid instanceId = requestContext.ServiceHost.OrganizationServiceHost.InstanceId;
            str = !StringComparer.OrdinalIgnoreCase.Equals(instanceId.ToString(), strArray[0]) ? strArray[0] : TFCommonUtil.GetIdentityDomainScope(instanceId);
          }
          searchFactor = str + "\\" + strArray[1];
        }
      }
      return searchFactor;
    }

    private bool IsProjectScoped(Guid scopeGuid)
    {
      IProjectService service = this.TfsRequestContext.GetService<IProjectService>();
      try
      {
        return service.TryGetProject(this.TfsRequestContext, scopeGuid, out ProjectInfo _);
      }
      catch (Exception ex)
      {
        if (!(ex is UnauthorizedAccessException))
          this.TfsRequestContext.TraceException(900408, "Identity", "ForNotGroupMembershipCacheService", ex);
        return false;
      }
    }

    private string ConvertGroupIdentityToName(Microsoft.VisualStudio.Services.Identity.Identity groupIdentity)
    {
      Guid property = groupIdentity.GetProperty<Guid>("LocalScopeId", Guid.Empty);
      return (property == Guid.Empty ? string.Empty : property.ToString()) + "\\" + groupIdentity.GetProperty<string>("Account", string.Empty);
    }

    public bool IsApplicable(WorkItemRule rule)
    {
      bool flag1 = false;
      bool flag2 = false;
      if (!Guid.Empty.Equals(rule.ForVsId))
      {
        flag1 = true;
        if (!this.IsCurrentUserMemberOfGroup(rule.ForVsId))
          return false;
      }
      if (!Guid.Empty.Equals(rule.NotVsId))
      {
        flag2 = true;
        if (this.IsCurrentUserMemberOfGroup(rule.NotVsId))
          return false;
      }
      return (flag1 || string.IsNullOrEmpty(rule.For) || this.IsCurrentUserMemberOfGroup(rule.For)) && (flag2 || string.IsNullOrEmpty(rule.Not) || !this.IsCurrentUserMemberOfGroup(rule.Not));
    }
  }
}
