// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.AuditLog.DecorationIdentityMap
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.TeamFoundation.Framework.Server.AuditLog
{
  public class DecorationIdentityMap
  {
    private Dictionary<Guid, ResolvedIdentityRef> m_identityRefMap;
    private Dictionary<IdentityDescriptor, ResolvedIdentityRef> m_identityDescriptorMap;

    public DecorationIdentityMap()
    {
      this.m_identityRefMap = new Dictionary<Guid, ResolvedIdentityRef>();
      this.m_identityDescriptorMap = new Dictionary<IdentityDescriptor, ResolvedIdentityRef>();
      this.IdentityRefs = new ConcurrentHashSet<Guid>((IEqualityComparer<Guid>) new DecorationIdentityMap.GuidComparer());
      this.IdentityDescriptors = new ConcurrentHashSet<IdentityDescriptor>((IEqualityComparer<IdentityDescriptor>) IdentityDescriptorComparer.Instance);
    }

    public bool AnyDescriptors() => this.m_identityDescriptorMap.Any<KeyValuePair<IdentityDescriptor, ResolvedIdentityRef>>();

    public void AddGuid(Guid guid)
    {
      if (!(guid != Guid.Empty))
        return;
      this.IdentityRefs.Add(guid);
    }

    public void AddDescriptor(IdentityDescriptor descriptor)
    {
      if (!(descriptor != (IdentityDescriptor) null) || descriptor.IsUnknownIdentityType())
        return;
      this.IdentityDescriptors.Add(descriptor);
    }

    public bool TryGetIdentity(Guid guid, out ResolvedIdentityRef val) => this.m_identityRefMap.TryGetValue(guid, out val) && val != null;

    public bool TryGetIdentity(IdentityDescriptor descriptor, out ResolvedIdentityRef val) => this.m_identityDescriptorMap.TryGetValue(descriptor, out val) && val != null;

    public void PopulateIdentityMap(IVssRequestContext requestContext, bool includeUIElements)
    {
      Dictionary<Guid, (Microsoft.VisualStudio.Services.Identity.Identity, string)> dictionary = new Dictionary<Guid, (Microsoft.VisualStudio.Services.Identity.Identity, string)>();
      IdentityService service = requestContext.GetService<IdentityService>();
      ResolvedIdentityRef val1;
      List<Guid> list = this.IdentityRefs.Where<Guid>((Func<Guid, bool>) (id => !this.TryGetIdentity(id, out val1) || val1 == null)).ToList<Guid>();
      IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity> identities = service.ReadIdentities(requestContext, (IList<Guid>) list, QueryMembership.None, (IEnumerable<string>) null).Where<Microsoft.VisualStudio.Services.Identity.Identity>((Func<Microsoft.VisualStudio.Services.Identity.Identity, bool>) (x => x != null));
      this.IdentityRefs.Clear();
      IdentityRef[] identityRefs = (IdentityRef[]) null;
      if (includeUIElements)
        identityRefs = identities.ToIdentityRefs(requestContext);
      identities.ForEach<Microsoft.VisualStudio.Services.Identity.Identity>((Action<Microsoft.VisualStudio.Services.Identity.Identity>) (id =>
      {
        Dictionary<Guid, ResolvedIdentityRef> identityRefMap = this.m_identityRefMap;
        Guid id1 = id.Id;
        ResolvedIdentityRef resolvedIdentityRef = new ResolvedIdentityRef();
        resolvedIdentityRef.Identity = id;
        string str;
        if (!includeUIElements)
        {
          str = (string) null;
        }
        else
        {
          IdentityRef[] source = identityRefs;
          str = this.GetActorImageUrl(source != null ? ((IEnumerable<IdentityRef>) source).FirstOrDefault<IdentityRef>((Func<IdentityRef, bool>) (ir => VssStringComparer.IdentityDescriptor.Equals(ir.Id, id.Id.ToString()))) : (IdentityRef) null);
        }
        resolvedIdentityRef.ActorImageUri = str;
        identityRefMap.Add(id1, resolvedIdentityRef);
      }));
      if (!this.IdentityDescriptors.IsEmpty)
      {
        ResolvedIdentityRef val2;
        IdentityDescriptor[] array = this.IdentityDescriptors.Where<IdentityDescriptor>((Func<IdentityDescriptor, bool>) (descriptor => !this.TryGetIdentity(descriptor, out val2) || val2 == null)).ToArray<IdentityDescriptor>();
        IList<Microsoft.VisualStudio.Services.Identity.Identity> identityList = service.ReadIdentities(requestContext, (IList<IdentityDescriptor>) array, QueryMembership.None, (IEnumerable<string>) null);
        for (int index = 0; index < identityList.Count; ++index)
        {
          Microsoft.VisualStudio.Services.Identity.Identity identity = identityList[index];
          IdentityDescriptor identityDescriptor = array[index];
          if (identity == null)
            requestContext.TraceAlways(1428532310, TraceLevel.Error, "AuditLog", nameof (DecorationIdentityMap), "The identity for subjectDescriptor: " + identityDescriptor.ToString() + " could not be resolved");
          Dictionary<IdentityDescriptor, ResolvedIdentityRef> identityDescriptorMap = this.m_identityDescriptorMap;
          IdentityDescriptor key = identityDescriptor;
          ResolvedIdentityRef resolvedIdentityRef;
          if (identity == null)
          {
            resolvedIdentityRef = (ResolvedIdentityRef) null;
          }
          else
          {
            resolvedIdentityRef = new ResolvedIdentityRef();
            resolvedIdentityRef.Identity = identity;
            resolvedIdentityRef.ActorImageUri = string.Empty;
          }
          identityDescriptorMap.Add(key, resolvedIdentityRef);
        }
        this.IdentityDescriptors.Clear();
      }
      else
        requestContext.TraceAlways(1428532311, TraceLevel.Warning, "AuditLog", nameof (DecorationIdentityMap), "The IdentityDescriptors hashset was empty when CreateIdentityMap was called");
    }

    private string GetActorImageUrl(IdentityRef identityRef)
    {
      string actorImageUrl = string.Empty;
      object obj;
      if (identityRef?.Links != null && identityRef.Links.Links.TryGetValue("avatar", out obj))
        actorImageUrl = (obj as ReferenceLink).Href;
      return actorImageUrl;
    }

    internal ConcurrentHashSet<Guid> IdentityRefs { get; set; }

    internal ConcurrentHashSet<IdentityDescriptor> IdentityDescriptors { get; set; }

    private class GuidComparer : IEqualityComparer<Guid>
    {
      public bool Equals(Guid x, Guid y) => x == y;

      public int GetHashCode(Guid obj) => obj.GetHashCode();
    }
  }
}
