// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Client.SecurityService
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.Client.Internal;
using Microsoft.TeamFoundation.Server;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Threading;

namespace Microsoft.TeamFoundation.Framework.Client
{
  internal class SecurityService : ISecurityService
  {
    private IIdentityManagementService m_ims;
    private TfsConnection m_server;
    private Dictionary<Guid, SecurityNamespace> m_namespaces;
    private ReaderWriterLockSlim m_accessLock;
    private SecurityWebService m_securityProxy;

    internal SecurityService(TfsConnection server)
    {
      this.m_accessLock = new ReaderWriterLockSlim();
      this.m_namespaces = (Dictionary<Guid, SecurityNamespace>) null;
      this.m_server = server;
      this.m_ims = this.m_server.GetService<IIdentityManagementService>();
      this.m_securityProxy = new SecurityWebService(server);
    }

    public SecurityNamespace CreateSecurityNamespace(SecurityNamespaceDescription description)
    {
      if (this.m_securityProxy == null)
        throw new NotSupportedException(ClientResources.OperationNotSuportedPreFramework());
      try
      {
        this.EnsureNamespacesLoaded();
        this.m_securityProxy.CreateSecurityNamespace(description);
        this.m_accessLock.EnterWriteLock();
        SecurityNamespace securityNamespace = (SecurityNamespace) new FrameworkSecurityNamespace(this.m_server, description);
        this.m_namespaces[securityNamespace.Description.NamespaceId] = securityNamespace;
        return securityNamespace;
      }
      finally
      {
        if (this.m_accessLock.IsWriteLockHeld)
          this.m_accessLock.ExitWriteLock();
      }
    }

    public bool DeleteSecurityNamespace(Guid namespaceId)
    {
      if (this.m_securityProxy == null)
        throw new NotSupportedException(ClientResources.OperationNotSuportedPreFramework());
      try
      {
        this.EnsureNamespacesLoaded();
        this.m_securityProxy.DeleteSecurityNamespace(namespaceId);
        this.m_accessLock.EnterWriteLock();
        if (!this.m_namespaces.Remove(namespaceId))
          return false;
        this.m_accessLock.ExitWriteLock();
        this.m_securityProxy.DeleteSecurityNamespace(namespaceId);
        return true;
      }
      finally
      {
        if (this.m_accessLock.IsWriteLockHeld)
          this.m_accessLock.ExitWriteLock();
      }
    }

    public SecurityNamespace GetSecurityNamespace(Guid namespaceId)
    {
      try
      {
        this.EnsureNamespacesLoaded();
        this.m_accessLock.EnterReadLock();
        SecurityNamespace securityNamespace;
        this.m_namespaces.TryGetValue(namespaceId, out securityNamespace);
        return securityNamespace;
      }
      finally
      {
        if (this.m_accessLock.IsReadLockHeld)
          this.m_accessLock.ExitReadLock();
      }
    }

    public ReadOnlyCollection<SecurityNamespace> GetSecurityNamespaces()
    {
      try
      {
        this.EnsureNamespacesLoaded();
        this.m_accessLock.EnterReadLock();
        return new List<SecurityNamespace>((IEnumerable<SecurityNamespace>) this.m_namespaces.Values).AsReadOnly();
      }
      finally
      {
        if (this.m_accessLock.IsReadLockHeld)
          this.m_accessLock.ExitReadLock();
      }
    }

    internal void SetServerSimulationMode(bool simulateVersionTwoServer)
    {
      this.m_namespaces = (Dictionary<Guid, SecurityNamespace>) null;
      if (simulateVersionTwoServer)
        this.m_securityProxy = (SecurityWebService) null;
      else
        this.m_securityProxy = new SecurityWebService(this.m_server);
    }

    private void EnsureNamespacesLoaded()
    {
      if (this.m_namespaces != null)
        return;
      try
      {
        this.m_accessLock.EnterWriteLock();
        if (this.m_namespaces != null)
          return;
        if (this.m_securityProxy != null)
        {
          SecurityNamespaceDescription[] namespaceDescriptionArray = this.m_securityProxy.QuerySecurityNamespaces(Guid.Empty);
          this.m_namespaces = new Dictionary<Guid, SecurityNamespace>();
          foreach (SecurityNamespaceDescription description in (IEnumerable<SecurityNamespaceDescription>) namespaceDescriptionArray)
          {
            SecurityNamespace securityNamespace = (SecurityNamespace) new FrameworkSecurityNamespace(this.m_server, description);
            this.m_namespaces[securityNamespace.Description.NamespaceId] = securityNamespace;
          }
        }
        else
        {
          if (!(this.m_server is TfsTeamProjectCollection))
            return;
          Type type = Assembly.Load("Microsoft.TeamFoundation.VersionControl.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a").GetType("Microsoft.TeamFoundation.VersionControl.Client.PreFrameworkSecurityNamespaceFactory");
          MethodInfo method = type.GetMethod("GetSecurityNamespaces", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
          List<SecurityNamespace> securityNamespaceList = new List<SecurityNamespace>();
          securityNamespaceList.AddRange((IEnumerable<SecurityNamespace>) (SecurityNamespace[]) method.Invoke((object) type, new object[1]
          {
            (object) (TfsTeamProjectCollection) this.m_server
          }));
          securityNamespaceList.AddRange(AuthorizationSecurityNamespaceFactory.GetSecurityNamespaces((TfsTeamProjectCollection) this.m_server));
          this.m_namespaces = new Dictionary<Guid, SecurityNamespace>();
          foreach (SecurityNamespace securityNamespace in securityNamespaceList)
            this.m_namespaces[securityNamespace.Description.NamespaceId] = securityNamespace;
        }
      }
      finally
      {
        if (this.m_accessLock.IsWriteLockHeld)
          this.m_accessLock.ExitWriteLock();
      }
    }
  }
}
