// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.IdentityPropertiesView
// Assembly: Microsoft.TeamFoundation.Server.Types, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FC707FA3-32BF-41E4-BD8A-1BB971125382
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Types.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public abstract class IdentityPropertiesView
  {
    private string m_namespace;
    private Guid m_identityId;
    private Microsoft.VisualStudio.Services.Identity.Identity m_identity;
    private IdentityPropertiesView m_ownerView;
    private HashSet<string> m_propertyFilters;
    private HashSet<string> m_dirtyPropertiesLocal;
    private HashSet<string> m_dirtyPropertiesGlobal;
    private IdentityPropertyScope m_Scope;

    public virtual Microsoft.VisualStudio.Services.Identity.Identity Identity => this.m_ownerView != null ? this.m_ownerView.Identity : this.m_identity;

    protected virtual string Namespace
    {
      get
      {
        if (this.m_namespace == null)
          this.m_namespace = this.GetType().FullName + ".";
        return this.m_namespace;
      }
    }

    public string NamespaceSuffix { get; private set; }

    public string EffectiveNamespace { get; private set; }

    public IEnumerable<KeyValuePair<string, object>> GetProperties() => this.GetProperties(IdentityPropertyScope.Global);

    public IEnumerable<KeyValuePair<string, object>> GetProperties(
      IdentityPropertyScope propertyScope)
    {
      return this.m_ownerView != null ? this.m_ownerView.GetProperties(propertyScope) : (IEnumerable<KeyValuePair<string, object>>) this.m_identity.Properties;
    }

    public IEnumerable<KeyValuePair<string, object>> GetViewProperties() => this.GetViewProperties(IdentityPropertyScope.Global);

    public IEnumerable<KeyValuePair<string, object>> GetViewProperties(
      IdentityPropertyScope propertyScope)
    {
      return this.GetPropertiesStartsWith(propertyScope, this.EffectiveNamespace);
    }

    public IEnumerable<KeyValuePair<string, object>> GetPropertiesStartsWith(
      string propertyNamespace)
    {
      return this.GetPropertiesStartsWith(IdentityPropertyScope.Global, propertyNamespace);
    }

    public IEnumerable<KeyValuePair<string, object>> GetPropertiesStartsWith(
      IdentityPropertyScope propertyScope,
      string propertyNamespace)
    {
      IEnumerable<KeyValuePair<string, object>> properties = this.GetProperties(propertyScope);
      if (string.IsNullOrEmpty(propertyNamespace))
        return properties;
      int scopeLength = propertyNamespace.Length;
      return properties.Where<KeyValuePair<string, object>>((Func<KeyValuePair<string, object>, bool>) (pair => pair.Key.StartsWith(propertyNamespace, StringComparison.OrdinalIgnoreCase))).Select<KeyValuePair<string, object>, KeyValuePair<string, object>>((Func<KeyValuePair<string, object>, KeyValuePair<string, object>>) (pair => new KeyValuePair<string, object>(pair.Key.Substring(scopeLength), pair.Value)));
    }

    public bool TryGetProperty(string name, out object value) => this.TryGetProperty(IdentityPropertyScope.Global, name, out value);

    public bool TryGetProperty(IdentityPropertyScope propertyScope, string name, out object value) => this.m_ownerView != null ? this.m_ownerView.TryGetProperty(propertyScope, name, out value) : this.m_identity.TryGetProperty(name, out value);

    public object GetProperty(string name) => this.GetProperty(IdentityPropertyScope.Global, name);

    public object GetProperty(IdentityPropertyScope propertyScope, string name) => this.m_ownerView != null ? this.m_ownerView.GetProperty(propertyScope, name) : this.m_identity.GetProperty<object>(name, (object) null);

    public void SetProperty(string name, object value) => this.SetProperty(IdentityPropertyScope.Global, name, value);

    public void SetProperty(IdentityPropertyScope propertyScope, string name, object value)
    {
      if (this.m_ownerView != null)
      {
        this.m_ownerView.SetProperty(propertyScope, name, value);
      }
      else
      {
        this.m_identity.SetProperty(name, value);
        if (propertyScope == IdentityPropertyScope.Local)
        {
          if (this.m_dirtyPropertiesLocal == null)
            this.m_dirtyPropertiesLocal = new HashSet<string>((IEqualityComparer<string>) VssStringComparer.IdentityPropertyName);
          this.m_dirtyPropertiesLocal.Add(name);
        }
        else
        {
          if (this.m_dirtyPropertiesGlobal == null)
            this.m_dirtyPropertiesGlobal = new HashSet<string>((IEqualityComparer<string>) VssStringComparer.IdentityPropertyName);
          this.m_dirtyPropertiesGlobal.Add(name);
        }
      }
    }

    public void RemoveProperty(string name) => this.RemoveProperty(IdentityPropertyScope.Global, name);

    public void RemoveProperty(IdentityPropertyScope propertyScope, string name) => this.SetProperty(propertyScope, name, (object) null);

    public object GetViewProperty(string propertyName) => this.GetViewProperty(IdentityPropertyScope.Global, propertyName);

    public object GetViewProperty(IdentityPropertyScope propertyScope, string propertyName) => this.GetProperty(propertyScope, this.EffectiveNamespace + propertyName);

    public bool TryGetViewProperty(string propertyName, out object propertyValue) => this.TryGetViewProperty(IdentityPropertyScope.Global, propertyName, out propertyValue);

    public virtual bool TryGetViewProperty(
      IdentityPropertyScope propertyScope,
      string propertyName,
      out object propertyValue)
    {
      return this.TryGetProperty(propertyScope, this.EffectiveNamespace + propertyName, out propertyValue);
    }

    public void SetViewProperty(string propertyName, object propertyValue) => this.SetViewProperty(IdentityPropertyScope.Global, propertyName, propertyValue);

    public virtual void SetViewProperty(
      IdentityPropertyScope propertyScope,
      string propertyName,
      object propertyValue)
    {
      this.SetProperty(propertyScope, this.EffectiveNamespace + propertyName, propertyValue);
    }

    public void RemoveViewProperty(string propertyName) => this.RemoveViewProperty(IdentityPropertyScope.Global, propertyName);

    public void RemoveViewProperty(IdentityPropertyScope propertyScope, string propertyName) => this.SetViewProperty(propertyScope, propertyName, (object) null);

    public void ClearViewProperties() => this.ClearViewProperties(IdentityPropertyScope.Global);

    public void ClearViewProperties(IdentityPropertyScope propertyScope) => this.ClearPropertiesUnder(propertyScope, this.EffectiveNamespace);

    public void ClearPropertiesUnder(string propertyNamespace) => this.ClearPropertiesUnder(IdentityPropertyScope.Global, propertyNamespace);

    public void ClearPropertiesUnder(IdentityPropertyScope propertyScope, string propertyNamespace)
    {
      foreach (string str in this.GetPropertiesStartsWith(propertyScope, propertyNamespace).Select<KeyValuePair<string, object>, string>((Func<KeyValuePair<string, object>, string>) (pair => pair.Key)).ToArray<string>())
        this.RemoveProperty(propertyScope, propertyNamespace + str);
    }

    public virtual void Update(IVssRequestContext requestContext)
    {
      if (this.m_ownerView != null)
      {
        this.m_ownerView.Update(requestContext);
      }
      else
      {
        ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
        IdentityPropertyScope identityPropertyScope = this.m_dirtyPropertiesGlobal != null ? (this.m_dirtyPropertiesLocal == null ? IdentityPropertyScope.Global : IdentityPropertyScope.Both) : (this.m_dirtyPropertiesLocal == null ? IdentityPropertyScope.None : IdentityPropertyScope.Local);
        if (identityPropertyScope == IdentityPropertyScope.Both || identityPropertyScope == IdentityPropertyScope.Global)
        {
          int num = !requestContext.IsFeatureEnabled("WorkItemTracking.Server.CopyLocalPropertiesInIdentityConvertDisabled") ? 1 : 0;
          List<KeyValuePair<string, object>> localProps = (List<KeyValuePair<string, object>>) null;
          if (num != 0 && this.m_Scope == IdentityPropertyScope.Local)
            localProps = this.RemoveLocalProps(requestContext);
          requestContext.GetService<IdentityService>().UpdateIdentities(requestContext, (IList<Microsoft.VisualStudio.Services.Identity.Identity>) new List<Microsoft.VisualStudio.Services.Identity.Identity>()
          {
            this.Identity
          });
          if (num != 0 && this.m_Scope == IdentityPropertyScope.Local)
            this.RestoreLocalProps(localProps);
        }
        if (identityPropertyScope == IdentityPropertyScope.Both || identityPropertyScope == IdentityPropertyScope.Local)
          new IdentityPropertyHelper().UpdateExtendedProperties(requestContext, IdentityPropertyScope.Local, (IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) new List<Microsoft.VisualStudio.Services.Identity.Identity>()
          {
            this.Identity
          });
        this.m_dirtyPropertiesGlobal = (HashSet<string>) null;
        this.m_dirtyPropertiesLocal = (HashSet<string>) null;
      }
    }

    private List<KeyValuePair<string, object>> RemoveLocalProps(IVssRequestContext requestContext)
    {
      List<KeyValuePair<string, object>> keyValuePairList = new List<KeyValuePair<string, object>>();
      List<ArtifactSpec> artifactSpecList = new IdentityPropertyHelper().MakeArtifactSpecs(IdentityPropertyScope.Local, (IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) new List<Microsoft.VisualStudio.Services.Identity.Identity>()
      {
        this.Identity
      });
      if (artifactSpecList.Count > 0)
      {
        using (TeamFoundationDataReader properties = requestContext.GetService<ITeamFoundationPropertyService>().GetProperties(requestContext, (IEnumerable<ArtifactSpec>) artifactSpecList, (IEnumerable<string>) this.m_propertyFilters))
        {
          foreach (ArtifactPropertyValue artifactPropertyValue in properties)
          {
            foreach (PropertyValue propertyValue in artifactPropertyValue.PropertyValues)
            {
              object obj;
              if (this.TryGetProperty(propertyValue.PropertyName, out obj))
              {
                keyValuePairList.Add(new KeyValuePair<string, object>(propertyValue.PropertyName, obj));
                this.Identity.Properties.Remove(propertyValue.PropertyName);
              }
            }
          }
        }
      }
      if (!this.m_dirtyPropertiesLocal.IsNullOrEmpty<string>())
      {
        foreach (string str in this.m_dirtyPropertiesLocal)
        {
          object obj;
          if (this.TryGetProperty(str, out obj))
          {
            keyValuePairList.Add(new KeyValuePair<string, object>(str, obj));
            this.Identity.Properties.Remove(str);
          }
        }
      }
      return keyValuePairList;
    }

    private void RestoreLocalProps(List<KeyValuePair<string, object>> localProps) => this.Identity.Properties.AddRangeIfRangeNotNull<KeyValuePair<string, object>, PropertiesCollection>((IEnumerable<KeyValuePair<string, object>>) localProps);

    protected virtual IEnumerable<string> GetViewPropertyFilters()
    {
      string effectiveNamespace = this.EffectiveNamespace;
      if (string.IsNullOrEmpty(effectiveNamespace))
        return (IEnumerable<string>) Array.Empty<string>();
      return (IEnumerable<string>) new string[1]
      {
        effectiveNamespace
      };
    }

    protected virtual void Initialize(
      IVssRequestContext requestContext,
      Guid identityId,
      string namespaceSuffix)
    {
      this.m_identityId = identityId;
      this.NamespaceSuffix = namespaceSuffix ?? "";
      this.EffectiveNamespace = this.Namespace + this.NamespaceSuffix;
      this.EnsureIdentityAndProperties(requestContext, this.GetViewPropertyFilters(), IdentityPropertyScope.Local);
    }

    protected virtual void Initialize(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Identity.Identity identity,
      IEnumerable<string> existingPropertyFilters,
      string namespaceSuffix,
      IdentityPropertyScope propertyScope)
    {
      this.m_identity = identity;
      this.NamespaceSuffix = namespaceSuffix ?? "";
      this.EffectiveNamespace = this.Namespace + this.NamespaceSuffix;
      this.MergePropertyFilters(existingPropertyFilters);
      this.EnsureIdentityAndProperties(requestContext, this.GetViewPropertyFilters(), propertyScope);
    }

    protected virtual void Initialize(
      IVssRequestContext requestContext,
      IdentityPropertiesView ownerView,
      string namespaceSuffix)
    {
      this.m_ownerView = ownerView;
      this.NamespaceSuffix = namespaceSuffix ?? "";
      this.EffectiveNamespace = this.Namespace + this.NamespaceSuffix;
      this.EnsureIdentityAndProperties(requestContext, this.GetViewPropertyFilters(), IdentityPropertyScope.Local);
    }

    protected void EnsureIdentityAndProperties(
      IVssRequestContext requestContext,
      IEnumerable<string> viewPropertyFilters,
      IdentityPropertyScope propertyScope)
    {
      if (this.m_ownerView != null)
      {
        this.m_ownerView.EnsureIdentityAndProperties(requestContext, viewPropertyFilters, propertyScope);
      }
      else
      {
        bool flag = false;
        this.m_Scope = propertyScope;
        if (viewPropertyFilters != null && viewPropertyFilters.Any<string>() && viewPropertyFilters.All<string>((Func<string, bool>) (filter => !string.IsNullOrEmpty(filter))))
        {
          foreach (string viewPropertyFilter in viewPropertyFilters)
          {
            if (this.m_propertyFilters == null)
            {
              this.m_propertyFilters = new HashSet<string>();
              this.m_propertyFilters.Add(viewPropertyFilter + "*");
              flag = true;
            }
            else if (!this.m_propertyFilters.Contains("*") && !this.m_propertyFilters.Contains(viewPropertyFilter) && !this.m_propertyFilters.Contains(viewPropertyFilter + "*"))
            {
              this.m_propertyFilters.Add(viewPropertyFilter + "*");
              flag = true;
            }
          }
        }
        if (this.m_identity != null)
          this.m_identityId = this.m_identity.Id;
        else
          flag = true;
        if (!flag)
          return;
        Microsoft.VisualStudio.Services.Identity.Identity readIdentity = requestContext.GetService<IdentityService>().ReadIdentities(requestContext, (IList<Guid>) new Guid[1]
        {
          this.m_identityId
        }, QueryMembership.None, (IEnumerable<string>) this.m_propertyFilters)[0];
        IdentityPropertyHelper identityPropertyHelper = new IdentityPropertyHelper();
        IVssRequestContext requestContext1 = requestContext;
        List<Microsoft.VisualStudio.Services.Identity.Identity> identityList = new List<Microsoft.VisualStudio.Services.Identity.Identity>();
        identityList.Add(readIdentity);
        HashSet<string> propertyFilters = this.m_propertyFilters;
        int propertyScope1 = (int) propertyScope;
        identityPropertyHelper.ReadExtendedProperties(requestContext1, (IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) identityList, (IEnumerable<string>) propertyFilters, (IdentityPropertyScope) propertyScope1);
        if (this.m_identity != null)
        {
          if (this.m_dirtyPropertiesGlobal != null)
          {
            foreach (string name in this.m_dirtyPropertiesGlobal)
              readIdentity.SetProperty(name, (object) this.m_identity.GetProperty<string>(name, (string) null));
          }
          if (this.m_dirtyPropertiesLocal != null)
          {
            foreach (string name in this.m_dirtyPropertiesLocal)
              readIdentity.SetProperty(name, (object) this.m_identity.GetProperty<string>(name, (string) null));
          }
        }
        this.m_identity = readIdentity;
      }
    }

    protected void MergePropertyFilters(IEnumerable<string> propertyFilters)
    {
      if (propertyFilters == null || !propertyFilters.Any<string>())
        return;
      if (this.m_ownerView != null)
      {
        this.m_ownerView.MergePropertyFilters(propertyFilters);
      }
      else
      {
        if (this.m_propertyFilters == null)
          this.m_propertyFilters = new HashSet<string>();
        this.m_propertyFilters.UnionWith(propertyFilters);
      }
    }

    public static T CreateView<T>(IVssRequestContext requestContext, Guid identityId) where T : IdentityPropertiesView, new() => IdentityPropertiesView.CreateView<T>(requestContext, identityId, string.Empty);

    public static T CreateView<T>(
      IVssRequestContext requestContext,
      Guid identityId,
      string namespaceSuffix)
      where T : IdentityPropertiesView, new()
    {
      T view = new T();
      view.Initialize(requestContext, identityId, namespaceSuffix);
      return view;
    }

    public static T CreateView<T>(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Identity.Identity identity,
      IEnumerable<string> existingPropertyFilters)
      where T : IdentityPropertiesView, new()
    {
      return IdentityPropertiesView.CreateView<T>(requestContext, identity, existingPropertyFilters, IdentityPropertyScope.Local);
    }

    public static T CreateView<T>(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Identity.Identity identity,
      IEnumerable<string> existingPropertyFilters,
      IdentityPropertyScope propertyScope)
      where T : IdentityPropertiesView, new()
    {
      return IdentityPropertiesView.CreateView<T>(requestContext, identity, existingPropertyFilters, string.Empty, propertyScope);
    }

    public static T CreateView<T>(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Identity.Identity identity,
      IEnumerable<string> existingPropertyFilters,
      string namespaceSuffix)
      where T : IdentityPropertiesView, new()
    {
      return IdentityPropertiesView.CreateView<T>(requestContext, identity, existingPropertyFilters, string.Empty, IdentityPropertyScope.Local);
    }

    public static T CreateView<T>(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Identity.Identity identity,
      IEnumerable<string> existingPropertyFilters,
      string namespaceSuffix,
      IdentityPropertyScope propertyScope)
      where T : IdentityPropertiesView, new()
    {
      T view = new T();
      view.Initialize(requestContext, identity, existingPropertyFilters, namespaceSuffix, propertyScope);
      return view;
    }

    public static T CreateView<T>(
      IVssRequestContext requestContext,
      IdentityPropertiesView ownerView)
      where T : IdentityPropertiesView, new()
    {
      return IdentityPropertiesView.CreateView<T>(requestContext, ownerView, string.Empty);
    }

    public static T CreateView<T>(
      IVssRequestContext requestContext,
      IdentityPropertiesView ownerView,
      string namespaceSuffix)
      where T : IdentityPropertiesView, new()
    {
      T view = new T();
      view.Initialize(requestContext, ownerView, namespaceSuffix);
      return view;
    }
  }
}
