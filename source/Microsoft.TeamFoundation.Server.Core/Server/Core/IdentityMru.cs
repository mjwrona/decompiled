// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.IdentityMru
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types.Team;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.Identity.Mru;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Microsoft.TeamFoundation.Server.Core
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class IdentityMru
  {
    private string m_mruName;
    private TeamFoundationIdentity m_identity;
    private bool m_isUserIdentity;
    private int m_mruMaxSize;

    public IdentityMru(IVssRequestContext requestContext, string mruName)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ITeamFoundationIdentityService service1 = requestContext.GetService<ITeamFoundationIdentityService>();
      IVssRegistryService service2 = requestContext.GetService<IVssRegistryService>();
      this.m_identity = service1.ReadRequestIdentity(requestContext);
      this.m_isUserIdentity = IdentityHelper.IsUserIdentity(requestContext, (IReadOnlyVssIdentity) IdentityUtil.Convert(this.m_identity));
      this.m_mruName = mruName ?? "";
      this.m_mruMaxSize = service2.GetValue<int>(requestContext, (RegistryQuery) "/IdentityMruMaxSize", 50);
    }

    public IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity> Read(
      IVssRequestContext requestContext)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      if (!this.m_isUserIdentity)
        return Enumerable.Empty<Microsoft.VisualStudio.Services.Identity.Identity>();
      IEnumerable<Guid> source1 = IdentityMruView.ReadMru(requestContext, this.m_identity, this.m_mruName, this.m_mruMaxSize);
      if (source1 != null)
        return ((IEnumerable<TeamFoundationIdentity>) requestContext.GetService<TeamFoundationIdentityService>().ReadIdentities(requestContext, source1.ToArray<Guid>())).Where<TeamFoundationIdentity>((Func<TeamFoundationIdentity, bool>) (identity => identity != null)).Select<TeamFoundationIdentity, Microsoft.VisualStudio.Services.Identity.Identity>((Func<TeamFoundationIdentity, Microsoft.VisualStudio.Services.Identity.Identity>) (identity => IdentityUtil.Convert(requestContext, identity)));
      IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity> source2 = this.GetMruCandidates(requestContext).Take<Microsoft.VisualStudio.Services.Identity.Identity>(this.m_mruMaxSize);
      IdentityMruView.UpdateMru(requestContext, this.m_identity, this.m_mruName, source2.Select<Microsoft.VisualStudio.Services.Identity.Identity, Guid>((Func<Microsoft.VisualStudio.Services.Identity.Identity, Guid>) (identity => identity.Id)), this.m_mruMaxSize);
      return source2;
    }

    public void AddItems(IVssRequestContext requestContext, Guid[] identityIds)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<Guid[]>(identityIds, nameof (identityIds));
      if (((IEnumerable<Guid>) identityIds).Any<Guid>((Func<Guid, bool>) (x => x == Guid.Empty)))
        identityIds = ((IEnumerable<Guid>) identityIds).Where<Guid>((Func<Guid, bool>) (x => x != Guid.Empty)).ToArray<Guid>();
      if (!this.m_isUserIdentity || identityIds.Length == 0)
        return;
      this.AddToNewMruService(requestContext, identityIds);
      ITeamFoundationIdentityService service1 = requestContext.GetService<ITeamFoundationIdentityService>();
      List<Guid> mruUpdateList = ((IEnumerable<Guid>) identityIds).Distinct<Guid>().Take<Guid>(this.m_mruMaxSize).ToList<Guid>();
      IEnumerable<Guid> source = IdentityMruView.ReadMru(requestContext, this.m_identity, this.m_mruName, this.m_mruMaxSize);
      bool flag = false;
      List<Guid> guidList = new List<Guid>();
      if (source == null)
      {
        mruUpdateList.AddRange(this.GetMruCandidates(requestContext).Select<Microsoft.VisualStudio.Services.Identity.Identity, Guid>((Func<Microsoft.VisualStudio.Services.Identity.Identity, Guid>) (item => item.Id)).Where<Guid>((Func<Guid, bool>) (item => !mruUpdateList.Contains(item))));
        flag = true;
      }
      else if (!mruUpdateList.SequenceEqual<Guid>(source.Take<Guid>(mruUpdateList.Count)))
      {
        guidList = source.Where<Guid>((Func<Guid, bool>) (item => !mruUpdateList.Contains(item))).ToList<Guid>();
        mruUpdateList.AddRange((IEnumerable<Guid>) guidList);
        flag = true;
      }
      if (!flag)
        return;
      mruUpdateList = ((IEnumerable<TeamFoundationIdentity>) service1.ReadIdentities(requestContext, mruUpdateList.ToArray())).Where<TeamFoundationIdentity>((Func<TeamFoundationIdentity, bool>) (identity => identity != null && identity.IsActive)).Select<TeamFoundationIdentity, Guid>((Func<TeamFoundationIdentity, Guid>) (identity => identity.TeamFoundationId)).Distinct<Guid>().Take<Guid>(this.m_mruMaxSize).ToList<Guid>();
      IdentityMruView.UpdateMru(requestContext, this.m_identity, this.m_mruName, (IEnumerable<Guid>) mruUpdateList, this.m_mruMaxSize);
      List<Guid> list = mruUpdateList.Intersect<Guid>((IEnumerable<Guid>) guidList).ToList<Guid>();
      CustomerIntelligenceService service2 = requestContext.GetService<CustomerIntelligenceService>();
      if (list.Count <= 0)
        return;
      service2.Publish(requestContext, nameof (IdentityMru), "MruMiss", "mruMissCount", (double) list.Count);
      service2.Publish(requestContext, requestContext.ServiceHost.InstanceId, this.m_identity.DisplayName, this.m_identity.TeamFoundationId, this.m_identity.Cuid(), DateTime.UtcNow, nameof (IdentityMru), "MruMiss", "mruMissIdentities", string.Join<Guid>(",", (IEnumerable<Guid>) list));
    }

    private void AddToNewMruService(IVssRequestContext requestContext, Guid[] identityIds)
    {
      try
      {
        IVssRequestContext context = requestContext.Elevate();
        context.GetService<IdentityMruService>().AddMruIdentities(context, this.m_identity.TeamFoundationId, IdentityMruService.SharedDefaultContainerId, (IList<Guid>) identityIds);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(0, "WitIdentityMRU", nameof (AddToNewMruService), ex);
      }
    }

    private void RemoveFromNewMruService(IVssRequestContext requestContext, Guid[] identityIds)
    {
      try
      {
        IVssRequestContext context = requestContext.Elevate();
        context.GetService<IdentityMruService>().RemoveMruIdentities(context, this.m_identity.TeamFoundationId, IdentityMruService.SharedDefaultContainerId, (IList<Guid>) identityIds);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(0, "WitIdentityMRU", nameof (RemoveFromNewMruService), ex);
      }
    }

    private void AddItemsTask(IVssRequestContext requestContext, object taskArgs) => this.AddItems(requestContext, taskArgs as Guid[]);

    public void AsyncAddItems(IVssRequestContext requestContext, Guid[] identityIds) => requestContext.To(TeamFoundationHostType.Deployment).GetService<ITeamFoundationTaskService>().AddTask(requestContext, new TeamFoundationTask(new TeamFoundationTaskCallback(this.AddItemsTask), (object) identityIds, 0));

    public void UpdateItems(IVssRequestContext requestContext, Guid[] identityIds)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      if (!this.m_isUserIdentity)
        return;
      IEnumerable<Guid> updatedEntries = ((IEnumerable<TeamFoundationIdentity>) requestContext.GetService<TeamFoundationIdentityService>().ReadIdentities(requestContext, identityIds)).Where<TeamFoundationIdentity>((Func<TeamFoundationIdentity, bool>) (identity => identity != null && identity.IsActive)).Select<TeamFoundationIdentity, Guid>((Func<TeamFoundationIdentity, Guid>) (identity => identity.TeamFoundationId)).Distinct<Guid>();
      IdentityMruView.UpdateMru(requestContext, this.m_identity, this.m_mruName, updatedEntries, this.m_mruMaxSize);
    }

    public void DeleteItems(IVssRequestContext requestContext, Guid[] identityIds)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      if (!this.m_isUserIdentity)
        return;
      this.RemoveFromNewMruService(requestContext, identityIds);
      IEnumerable<Guid> updatedEntries = (IdentityMruView.ReadMru(requestContext, this.m_identity, this.m_mruName, this.m_mruMaxSize) ?? this.GetMruCandidates(requestContext).Select<Microsoft.VisualStudio.Services.Identity.Identity, Guid>((Func<Microsoft.VisualStudio.Services.Identity.Identity, Guid>) (item => item.Id))).Where<Guid>((Func<Guid, bool>) (item => !((IEnumerable<Guid>) identityIds).Contains<Guid>(item)));
      IdentityMruView.UpdateMru(requestContext, this.m_identity, this.m_mruName, updatedEntries, this.m_mruMaxSize);
    }

    public void DeleteMru(IVssRequestContext requestContext)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      if (!this.m_isUserIdentity || IdentityMruView.ReadMru(requestContext, this.m_identity, this.m_mruName, this.m_mruMaxSize) == null)
        return;
      IdentityMruView.DeleteMru(requestContext, this.m_identity, this.m_mruName);
    }

    private IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity> GetMruCandidates(
      IVssRequestContext requestContext)
    {
      ITeamService service = requestContext.GetService<ITeamService>();
      List<Microsoft.VisualStudio.Services.Identity.Identity> first = new List<Microsoft.VisualStudio.Services.Identity.Identity>();
      foreach (WebApiTeam queryMyTeamsIn in (IEnumerable<WebApiTeam>) service.QueryMyTeamsInCollection(requestContext, this.m_identity.Descriptor))
      {
        List<Microsoft.VisualStudio.Services.Identity.Identity> list = service.ReadTeamMembers(requestContext, queryMyTeamsIn.Identity, MembershipQuery.Expanded).Where<Microsoft.VisualStudio.Services.Identity.Identity>((Func<Microsoft.VisualStudio.Services.Identity.Identity, bool>) (identity => identity != null && identity.IsActive && !identity.IsContainer)).ToList<Microsoft.VisualStudio.Services.Identity.Identity>();
        first = first.Union<Microsoft.VisualStudio.Services.Identity.Identity>((IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) list, (IEqualityComparer<Microsoft.VisualStudio.Services.Identity.Identity>) new IdentityEqualityDisplayNameComparer()).ToList<Microsoft.VisualStudio.Services.Identity.Identity>();
        if (first.Count >= this.m_mruMaxSize)
          return (IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) first;
      }
      return (IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) first;
    }
  }
}
