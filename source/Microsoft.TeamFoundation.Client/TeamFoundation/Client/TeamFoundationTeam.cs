// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Client.TeamFoundationTeam
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.TeamFoundation.Framework.Client;
using Microsoft.TeamFoundation.Framework.Common;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Client
{
  public class TeamFoundationTeam
  {
    private Dictionary<Type, object> m_viewCache = new Dictionary<Type, object>();
    private object m_cacheLock = new object();

    internal TeamFoundationTeam(TeamFoundationIdentity team) => this.Identity = team.TryGetProperty(TeamConstants.TeamPropertyName, out object _) ? team : throw new IdentityPropertyRequiredException("Team");

    public TeamFoundationIdentity Identity { get; private set; }

    public string Project => this.Identity.GetAttribute("Domain", (string) null);

    public string Name
    {
      get => this.Identity.GetAttribute("Account", (string) null);
      set
      {
        this.IsNameDirty = true;
        this.Identity.SetAttribute("Account", value);
      }
    }

    public string Description
    {
      get => this.Identity.GetAttribute(nameof (Description), (string) null);
      set
      {
        this.IsDescriptionDirty = true;
        this.Identity.SetAttribute(nameof (Description), value);
      }
    }

    public bool TryGetProperty(string name, out object value) => this.Identity.TryGetProperty(name, out value);

    public object GetProperty(string name) => this.Identity.GetProperty(name);

    public void RemoveProperty(string name) => this.SetProperty(name, (object) null);

    public IEnumerable<KeyValuePair<string, object>> GetProperties() => this.Identity.GetProperties();

    public void SetProperty(string name, object value) => this.Identity.SetProperty(IdentityPropertyScope.Local, name, value);

    public TeamFoundationIdentity[] GetMembers(TfsConnection tfs, MembershipQuery queryMembership)
    {
      IIdentityManagementService2 service = tfs.GetService<IIdentityManagementService2>();
      TeamFoundationIdentity readIdentity = service.ReadIdentities(new IdentityDescriptor[1]
      {
        this.Identity.Descriptor
      }, queryMembership, ReadIdentityOptions.None, (IEnumerable<string>) null, IdentityPropertyScope.None)[0];
      return readIdentity == null ? Array.Empty<TeamFoundationIdentity>() : service.ReadIdentities(readIdentity.Members, MembershipQuery.None, ReadIdentityOptions.None);
    }

    internal bool IsNameDirty { get; private set; }

    internal bool IsDescriptionDirty { get; private set; }

    public T GetTeamPropertiesView<T>(TfsConnection tfs) where T : TeamPropertiesView
    {
      object obj;
      if (!this.m_viewCache.TryGetValue(typeof (T), out obj))
      {
        lock (this.m_cacheLock)
        {
          if (!this.m_viewCache.TryGetValue(typeof (T), out obj))
          {
            obj = new object();
            this.m_viewCache.Add(typeof (T), obj);
          }
        }
      }
      object teamPropertiesView = obj;
      if (!(obj is TeamPropertiesView))
      {
        lock (obj)
        {
          teamPropertiesView = this.m_viewCache[typeof (T)];
          if (!(teamPropertiesView is TeamPropertiesView))
          {
            teamPropertiesView = Activator.CreateInstance(typeof (T));
            ((T) teamPropertiesView).Initialize(tfs, this);
            lock (this.m_cacheLock)
              this.m_viewCache[typeof (T)] = teamPropertiesView;
          }
        }
      }
      return (T) teamPropertiesView;
    }
  }
}
