// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.TrackedSecurityCollection
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class TrackedSecurityCollection
  {
    private readonly Dictionary<TrackedSecurityCollection.TrackedSecurityItemKey, TrackedSecurityCollection.TrackedSecurityItemValue> m_trackedItems;
    private readonly Dictionary<Guid, IVssSecurityNamespace> m_namespaces;
    private readonly IVssRequestContext m_requestContext;
    private readonly bool m_enforceDataspaceRestrictions;
    private static readonly HashSet<Guid> s_byPassDataspaceNamespaces = new HashSet<Guid>((IEnumerable<Guid>) new Guid[3]
    {
      new Guid("2DAB47F9-BD70-49ED-9BD5-8EB051E59C02"),
      GraphSecurityConstants.NamespaceId,
      new Guid("0582EB05-C896-449A-B933-AA3D99E121D6")
    });

    public TrackedSecurityCollection(
      IVssRequestContext requestContext,
      bool enforceDataspaceRestrictions = true)
    {
      this.m_requestContext = requestContext;
      this.m_enforceDataspaceRestrictions = !requestContext.ServiceHost.IsProduction && enforceDataspaceRestrictions;
      this.m_trackedItems = new Dictionary<TrackedSecurityCollection.TrackedSecurityItemKey, TrackedSecurityCollection.TrackedSecurityItemValue>();
      this.m_namespaces = new Dictionary<Guid, IVssSecurityNamespace>();
    }

    public void Add(
      IVssSecurityNamespace securityNamespace,
      string token,
      int requestedPermissions)
    {
      if (!this.m_namespaces.ContainsKey(securityNamespace.Description.NamespaceId))
        this.m_namespaces.Add(securityNamespace.Description.NamespaceId, securityNamespace);
      TrackedSecurityCollection.TrackedSecurityItemKey key = new TrackedSecurityCollection.TrackedSecurityItemKey(securityNamespace.Description.NamespaceId, token);
      TrackedSecurityCollection.TrackedSecurityItemValue securityItemValue;
      if (this.m_trackedItems.TryGetValue(key, out securityItemValue))
      {
        if ((securityItemValue.RequestedPermissions & requestedPermissions) == requestedPermissions)
          return;
        this.m_trackedItems[key] = new TrackedSecurityCollection.TrackedSecurityItemValue(securityItemValue.DataspaceIdentifier, securityItemValue.RequestedPermissions | requestedPermissions, securityItemValue.EnforceDataspaceRestrictions);
      }
      else
      {
        Guid dataspaceIdentifier = Guid.Empty;
        bool enforceDataspaceRestrictions = this.m_enforceDataspaceRestrictions && !TrackedSecurityCollection.s_byPassDataspaceNamespaces.Contains(securityNamespace.Description.NamespaceId);
        bool flag;
        if (((!enforceDataspaceRestrictions ? 0 : (this.m_requestContext.RootContext.TryGetItem<bool>(RequestContextItemsKeys.BypassDataspaceRestrictionForMembers, out flag) ? 1 : 0)) & (flag ? 1 : 0)) != 0 && !this.m_requestContext.IsPublicUser() && !this.m_requestContext.IsAnonymousPrincipal())
          enforceDataspaceRestrictions = false;
        if (enforceDataspaceRestrictions)
        {
          if (this.m_requestContext.DataspaceIdentifier == Guid.Empty)
            enforceDataspaceRestrictions = false;
          else if (securityNamespace is ILocalSecurityNamespace securityNamespace1)
          {
            if (securityNamespace1.DataspaceMapper != null)
              dataspaceIdentifier = securityNamespace1.DataspaceMapper.DataspaceIdentifierFromToken(this.m_requestContext, token);
          }
          else
            enforceDataspaceRestrictions = false;
        }
        this.m_trackedItems.Add(key, new TrackedSecurityCollection.TrackedSecurityItemValue(dataspaceIdentifier, requestedPermissions, enforceDataspaceRestrictions));
      }
    }

    public void Validate(object o)
    {
      this.ValidateNonNullSecuredObject(o);
      if (!(o is ISecuredObject securedObject))
      {
        string message = string.Format("{0} must implement ISecuredObject", (object) o.GetType());
        this.m_requestContext.Trace(2122135760, TraceLevel.Error, "ServerJsonSerializationHelper", "AnonymousAccessKalypsoAlert", message);
        throw new InvalidOperationException(message);
      }
      this.Validate(securedObject);
    }

    public virtual void Validate(ISecuredObject securedObject)
    {
      this.ValidateNonNullSecuredObject((object) securedObject);
      Guid namespaceId = securedObject.NamespaceId;
      IVssSecurityNamespace securityNamespace;
      if (!this.m_namespaces.TryGetValue(namespaceId, out securityNamespace))
      {
        string message = string.Format("Cannot serialize type {0}, because the security requirements are not met Namespace:{1}", (object) securedObject.GetType(), (object) namespaceId);
        this.m_requestContext.Trace(2122135760, TraceLevel.Error, nameof (TrackedSecurityCollection), "AnonymousAccessKalypsoAlert", message);
        throw new InvalidOperationException(message);
      }
      int requiredPermissions = securedObject.RequiredPermissions;
      string token = securityNamespace.CheckAndCanonicalizeToken(securedObject.GetToken());
      TrackedSecurityCollection.TrackedSecurityItemValue securityItemValue;
      bool flag = this.m_trackedItems.TryGetValue(new TrackedSecurityCollection.TrackedSecurityItemKey(namespaceId, token), out securityItemValue);
      if (!flag || (securityItemValue.RequestedPermissions & requiredPermissions) != requiredPermissions || securityItemValue.EnforceDataspaceRestrictions && securityItemValue.DataspaceIdentifier != this.m_requestContext.DataspaceIdentifier)
      {
        string message1 = string.Format("Cannot serialize type {0}, because the security requirements are not met  Namespace:{1}, RequestedPermissions:{2}, DataspaceIdentifier: {3}", (object) securedObject.GetType(), (object) namespaceId, (object) requiredPermissions, (object) this.m_requestContext.DataspaceIdentifier);
        string message2 = string.Format("{0}, Token: {1}, ItemTracked: {2}", (object) message1, (object) token, (object) flag);
        if (flag)
          message2 = string.Format("{0}, TrackedPermissions: {1}, TrackedDataspaceIdentifier: {2}, DataspaceRestrictionsEnforced: {3}", (object) message2, (object) securityItemValue.RequestedPermissions, (object) securityItemValue.DataspaceIdentifier, (object) securityItemValue.EnforceDataspaceRestrictions);
        this.m_requestContext.Trace(2122135760, TraceLevel.Error, nameof (TrackedSecurityCollection), "AnonymousAccessKalypsoAlert", message2);
        throw new InvalidOperationException(message1);
      }
    }

    private void ValidateNonNullSecuredObject(object o)
    {
      if (o == null)
      {
        string message = "Expected non-null ISecuredObject.";
        this.m_requestContext.Trace(444950878, TraceLevel.Error, "ServerJsonSerializationHelper", "AnonymousAccessKalypsoAlert", message);
        throw new InvalidOperationException(message);
      }
    }

    public bool EnforceDataspaceRestrictions => this.m_enforceDataspaceRestrictions;

    private struct TrackedSecurityItemKey
    {
      public TrackedSecurityItemKey(Guid namespaceId, string token)
      {
        this.NamespaceId = namespaceId;
        this.Token = token;
      }

      public Guid NamespaceId { get; private set; }

      public string Token { get; private set; }

      public override int GetHashCode() => this.NamespaceId.GetHashCode() ^ StringComparer.OrdinalIgnoreCase.GetHashCode(this.Token);

      public override bool Equals(object obj) => obj is TrackedSecurityCollection.TrackedSecurityItemKey trackedSecurityItemKey && trackedSecurityItemKey.NamespaceId.Equals(this.NamespaceId) && string.Equals(this.Token, trackedSecurityItemKey.Token, StringComparison.OrdinalIgnoreCase);
    }

    private struct TrackedSecurityItemValue
    {
      public TrackedSecurityItemValue(
        Guid dataspaceIdentifier,
        int requestedPermissions,
        bool enforceDataspaceRestrictions)
      {
        this.DataspaceIdentifier = dataspaceIdentifier;
        this.RequestedPermissions = requestedPermissions;
        this.EnforceDataspaceRestrictions = enforceDataspaceRestrictions;
      }

      public int RequestedPermissions { get; set; }

      public Guid DataspaceIdentifier { get; private set; }

      public bool EnforceDataspaceRestrictions { get; private set; }
    }
  }
}
