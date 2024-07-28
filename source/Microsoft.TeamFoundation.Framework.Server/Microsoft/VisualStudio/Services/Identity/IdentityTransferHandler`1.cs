// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.IdentityTransferHandler`1
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Identity
{
  internal abstract class IdentityTransferHandler<T> : ITfsIdentityServicingHelper, IDisposable
  {
    protected IdentityTransferHandler(
      IVssRequestContext requestContext,
      IServicingContext servicingContext,
      IDictionary<string, IIdentityProvider> syncAgents,
      T identityStore)
    {
      this.RequestContext = requestContext;
      this.ServicingContext = servicingContext;
      this.SyncAgents = syncAgents;
      this.IdentityStore = identityStore;
      this.HostDomain = new IdentityDomain(requestContext.ServiceHost.InstanceId);
    }

    public virtual void Dispose()
    {
    }

    public virtual void Execute()
    {
      try
      {
        if (this.IsExecutedBefore())
        {
          this.ServicingContext.LogInfo("Step is executed before. Skipping.");
        }
        else
        {
          this.PreTransferSteps();
          this.Transfer();
          this.PostTransferSteps();
        }
      }
      catch (Exception ex)
      {
        this.OnException();
        throw;
      }
    }

    protected virtual bool IsExecutedBefore() => false;

    protected abstract void PreTransferSteps();

    protected virtual void Transfer()
    {
      List<KeyValuePair<Guid, Guid>> mappings = new List<KeyValuePair<Guid, Guid>>();
      IEnumerable<IdentityScope> scopes;
      IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity> groups;
      IEnumerable<GroupMembership> memberships;
      IEnumerable<Guid> identityIds;
      this.ReadSnapshotFromSource(out scopes, out groups, out memberships, out identityIds);
      IdentityScope identityScope = scopes.FirstOrDefault<IdentityScope>((Func<IdentityScope, bool>) (scope => scope.IsGlobal));
      if (identityScope != null)
        identityScope.Name = this.RequestContext.ServiceHost.Name;
      Dictionary<Guid, Guid> dictionary = new Dictionary<Guid, Guid>();
      foreach (Microsoft.VisualStudio.Services.Identity.Identity identity1 in groups)
      {
        identity1.Descriptor = this.MapIfTeamFoundationType(identity1.Descriptor);
        Microsoft.VisualStudio.Services.Identity.Identity identity2 = this.ReadIdentitiesFromTarget((IList<IdentityDescriptor>) new IdentityDescriptor[1]
        {
          identity1.Descriptor
        })[0];
        Guid id = identity1.Id;
        Guid guid = identity1.Id;
        if (identity2 == null)
        {
          if (this.ChangeDomainIds)
            guid = Guid.NewGuid();
        }
        else if (identity2.Id != identity1.Id)
          guid = identity2.Id;
        if (id != guid)
        {
          identity1.Id = guid;
          mappings.Add(new KeyValuePair<Guid, Guid>(id, guid));
        }
        dictionary.Add(id, guid);
      }
      foreach (GroupMembership membership in memberships)
      {
        membership.Descriptor = this.MapIfTeamFoundationType(membership.Descriptor);
        Guid guid;
        if (!dictionary.TryGetValue(membership.Id, out guid))
        {
          Microsoft.VisualStudio.Services.Identity.Identity identity3 = this.ReadIdentitiesFromSource((IList<Guid>) new Guid[1]
          {
            membership.Id
          })[0];
          if (identity3 != null)
          {
            this.TransformIdentity(identity3);
            Microsoft.VisualStudio.Services.Identity.Identity targetIdentity = this.ReadIdentitiesFromTarget((IList<IdentityDescriptor>) new IdentityDescriptor[1]
            {
              identity3.Descriptor
            })[0];
            if (targetIdentity != null && targetIdentity.IsActive)
              membership.Id = targetIdentity.Id;
            else if (identity3.Descriptor.IdentityType == "Microsoft.TeamFoundation.ServiceIdentity")
            {
              this.HandleServiceIdentity(membership, identity3, targetIdentity);
            }
            else
            {
              Microsoft.VisualStudio.Services.Identity.Identity identity4 = this.SyncIdentity(identity3.Descriptor, identity3.DisplayName);
              if (identity4 != null && identity4.IsActive)
              {
                membership.Id = identity4.Id;
              }
              else
              {
                if (this.ChangeDomainIds && IdentityValidation.IsTeamFoundationType(identity3.Descriptor))
                {
                  Guid id = identity3.Id;
                  guid = Guid.NewGuid();
                  identity3.Id = guid;
                  identity3.MasterId = guid;
                  dictionary.Add(id, guid);
                }
                this.HandleInactiveMember(identity3, targetIdentity, membership);
              }
            }
          }
        }
        else
          membership.Id = guid;
      }
      this.SaveSnapshotInTarget(scopes, groups, memberships);
      List<Microsoft.VisualStudio.Services.Identity.Identity> identities1 = new List<Microsoft.VisualStudio.Services.Identity.Identity>();
      List<Guid> identities2 = new List<Guid>();
      List<Guid> list1 = identityIds.ToList<Guid>();
      IEnumerable<Guid> source = (IEnumerable<Guid>) list1;
      int count = 5000;
      for (int index1 = 0; index1 < list1.Count; index1 += count)
      {
        IList<Microsoft.VisualStudio.Services.Identity.Identity> list2 = (IList<Microsoft.VisualStudio.Services.Identity.Identity>) this.ReadIdentitiesFromSource((IList<Guid>) source.Take<Guid>(count).ToList<Guid>()).Where<Microsoft.VisualStudio.Services.Identity.Identity>((Func<Microsoft.VisualStudio.Services.Identity.Identity, bool>) (sourceIdentity => sourceIdentity != null)).ToList<Microsoft.VisualStudio.Services.Identity.Identity>();
        IList<Microsoft.VisualStudio.Services.Identity.Identity> identityList1 = this.ReadIdentitiesFromTarget((IList<Guid>) list2.Select<Microsoft.VisualStudio.Services.Identity.Identity, Guid>((Func<Microsoft.VisualStudio.Services.Identity.Identity, Guid>) (sourceIdentity => sourceIdentity.Id)).ToList<Guid>());
        foreach (Microsoft.VisualStudio.Services.Identity.Identity identity in (IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) list2)
          this.TransformIdentity(identity);
        IList<Microsoft.VisualStudio.Services.Identity.Identity> identityList2 = this.ReadIdentitiesFromTarget((IList<IdentityDescriptor>) list2.Select<Microsoft.VisualStudio.Services.Identity.Identity, IdentityDescriptor>((Func<Microsoft.VisualStudio.Services.Identity.Identity, IdentityDescriptor>) (sourceIdentity => sourceIdentity.Descriptor)).ToList<IdentityDescriptor>());
        for (int index2 = 0; index2 < list2.Count; ++index2)
        {
          Microsoft.VisualStudio.Services.Identity.Identity identity5 = list2[index2];
          Microsoft.VisualStudio.Services.Identity.Identity identity6 = identityList2[index2];
          Microsoft.VisualStudio.Services.Identity.Identity identity7 = identityList1[index2];
          Guid id = identity5.Id;
          Guid guid = identity5.Id;
          if (identity6 == null)
          {
            if (this.ChangeDomainIds && (IdentityValidation.IsTeamFoundationType(identity5.Descriptor) || IdentityValidation.IsUnauthenticatedType(identity5.Descriptor)))
              guid = Guid.NewGuid();
            else if (identity7 != null)
              guid = Guid.NewGuid();
            identities1.Add(identity5);
          }
          else if (identity6.Id != identity5.Id)
            guid = identity6.Id;
          if (id != guid)
          {
            identity5.Id = guid;
            identity5.MasterId = guid;
            mappings.Add(new KeyValuePair<Guid, Guid>(id, guid));
          }
          identities2.Add(guid);
        }
        if (index1 + count < list1.Count)
          source = source.Skip<Guid>(count);
      }
      if (identities1.Count > 0)
        this.UpdateIdentitiesInTarget((IList<Microsoft.VisualStudio.Services.Identity.Identity>) identities1, true);
      this.EnsureIdentitiesRooted((IList<Guid>) identities2);
      this.UpdateIdentityMap((IEnumerable<KeyValuePair<Guid, Guid>>) mappings);
    }

    public virtual void TransformIdentity(Microsoft.VisualStudio.Services.Identity.Identity identity) => identity.Descriptor = this.MapIfTeamFoundationType(identity.Descriptor);

    private void HandleServiceIdentity(
      GroupMembership membership,
      Microsoft.VisualStudio.Services.Identity.Identity sourceIdentity,
      Microsoft.VisualStudio.Services.Identity.Identity targetIdentity)
    {
      if (targetIdentity != null)
      {
        membership.Id = targetIdentity.Id;
      }
      else
      {
        this.UpdateIdentitiesInTarget((IList<Microsoft.VisualStudio.Services.Identity.Identity>) new Microsoft.VisualStudio.Services.Identity.Identity[1]
        {
          sourceIdentity
        }, true);
        membership.Id = sourceIdentity.Id;
      }
    }

    protected abstract void PostTransferSteps();

    protected virtual void OnException()
    {
    }

    protected virtual bool ChangeDomainIds => true;

    protected abstract void ReadSnapshotFromSource(
      out IEnumerable<IdentityScope> scopes,
      out IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity> groups,
      out IEnumerable<GroupMembership> memberships,
      out IEnumerable<Guid> identityIds);

    protected abstract void SaveSnapshotInTarget(
      IEnumerable<IdentityScope> scopes,
      IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity> groups,
      IEnumerable<GroupMembership> memberships);

    protected abstract IList<Microsoft.VisualStudio.Services.Identity.Identity> ReadIdentitiesFromSource(
      IList<Guid> identityIds);

    protected abstract IList<Microsoft.VisualStudio.Services.Identity.Identity> ReadIdentitiesFromTarget(
      IList<IdentityDescriptor> descriptors);

    protected abstract IList<Microsoft.VisualStudio.Services.Identity.Identity> ReadIdentitiesFromTarget(
      IList<Guid> identityIds);

    protected abstract Microsoft.VisualStudio.Services.Identity.Identity SyncIdentity(
      IdentityDescriptor descriptor,
      string displayName);

    protected abstract void HandleInactiveMember(
      Microsoft.VisualStudio.Services.Identity.Identity sourceIdentity,
      Microsoft.VisualStudio.Services.Identity.Identity targetIdentity,
      GroupMembership membership);

    protected abstract void UpdateIdentitiesInTarget(
      IList<Microsoft.VisualStudio.Services.Identity.Identity> identities,
      bool favorCurrentlyActive);

    protected abstract void EnsureIdentitiesRooted(IList<Guid> identities);

    protected abstract void UpdateIdentityMap(IEnumerable<KeyValuePair<Guid, Guid>> mappings);

    public abstract IdentityDescriptor MapIfTeamFoundationType(IdentityDescriptor descriptor);

    protected IVssRequestContext RequestContext { get; }

    protected IServicingContext ServicingContext { get; private set; }

    protected IDictionary<string, IIdentityProvider> SyncAgents { get; private set; }

    protected T IdentityStore { get; private set; }

    protected IdentityDomain HostDomain { get; private set; }
  }
}
