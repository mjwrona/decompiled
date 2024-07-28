// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Client.Catalog.Objects.CatalogObject
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Common.Catalog.Objects;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Microsoft.TeamFoundation.Framework.Client.Catalog.Objects
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class CatalogObject : INotifyPropertyChanged
  {
    protected static readonly object NullObject = new object();
    private bool m_preloaded;
    private CatalogObject m_parent;
    internal ICollection<CatalogObject> Children = (ICollection<CatalogObject>) new List<CatalogObject>();
    internal IDictionary<string, object> SingletonDependencies = (IDictionary<string, object>) new Dictionary<string, object>((IEqualityComparer<string>) VssStringComparer.CatalogNodeDependency);
    internal IDictionary<string, object> SetDependencies = (IDictionary<string, object>) new Dictionary<string, object>((IEqualityComparer<string>) VssStringComparer.CatalogNodeDependency);
    internal ICollection<CatalogObject> Dependents = (ICollection<CatalogObject>) new List<CatalogObject>();

    public CatalogNode CatalogNode { get; internal set; }

    public CatalogObjectContext Context { get; internal set; }

    public string DisplayName
    {
      get => this.CatalogNode != null ? this.CatalogNode.Resource.DisplayName : string.Empty;
      set
      {
        if (this.CatalogNode == null)
          return;
        this.CatalogNode.Resource.DisplayName = value;
        this.MarkAsDirty();
        this.NotifyPropertyChanged(nameof (DisplayName));
      }
    }

    public string DisplayPath
    {
      get
      {
        StringBuilder sb = new StringBuilder();
        this.BuildDisplayPath(sb);
        return sb.ToString();
      }
    }

    public string Description
    {
      get => this.CatalogNode != null ? this.CatalogNode.Resource.Description : string.Empty;
      set
      {
        if (this.CatalogNode == null)
          return;
        this.CatalogNode.Resource.Description = value;
        this.MarkAsDirty();
        this.NotifyPropertyChanged(nameof (Description));
      }
    }

    public string CatalogNodeFullPath => this.CatalogNode != null ? this.CatalogNode.FullPath : string.Empty;

    public Guid CatalogResourceIdentifier => this.CatalogNode != null ? this.CatalogNode.Resource.Identifier : Guid.Empty;

    public bool IsDefault => this.CatalogNode != null && this.CatalogNode.IsDefault;

    public CatalogObject Parent => this.m_parent == null && this.CatalogNode != null && this.CatalogNode.ParentNode != null ? this.CreateCatalogObject<CatalogObject>(this.CatalogNode.ParentNode) : this.m_parent;

    public void Delete() => this.Context.DeleteObject(this);

    private void MarkAsDirty() => this.Context.ModifyObject(this);

    public void Preload()
    {
      if (this.m_preloaded)
        return;
      ICollection<Guid> knownDescendantTypes = CatalogObjectUtilities.GetKnownDescendantTypes(this.GetType());
      this.Preload(new CatalogBulkData((ICollection<CatalogNode>) this.CatalogNode.QueryChildren((IEnumerable<Guid>) knownDescendantTypes, true, this.Context.QueryOptions), knownDescendantTypes));
      this.m_preloaded = true;
    }

    public void Refresh() => this.Refresh(this.m_preloaded);

    public void Refresh(bool preload)
    {
      if (this.CatalogNode != null && !string.IsNullOrEmpty(this.CatalogNode.FullPath))
        this.CatalogNode = this.Context.CatalogService.QueryNodes((IEnumerable<string>) new string[1]
        {
          this.CatalogNode.FullPath
        }, (IEnumerable<Guid>) null, this.Context.QueryOptions).FirstOrDefault<CatalogNode>() ?? throw new Exception("Refresh failed, node was no longer found");
      this.m_preloaded = false;
      this.Reset();
      this.OnRefresh();
      if (!preload)
        return;
      this.Preload();
    }

    protected virtual void OnRefresh()
    {
    }

    protected virtual void Reset()
    {
      this.Children.Clear();
      this.SingletonDependencies.Clear();
      this.SetDependencies.Clear();
      this.m_parent = (CatalogObject) null;
    }

    public virtual void Preload(CatalogBulkData bulkData)
    {
    }

    protected internal virtual void Initialize()
    {
    }

    public bool Is<T>() where T : CatalogObject => CatalogObject.IsNodeOfType<T>(this.CatalogNode);

    public T As<T>() where T : CatalogObject => !this.Is<T>() ? default (T) : this.CreateCatalogObject<T>(this.CatalogNode);

    public T CreateChild<T>(string displayName) where T : CatalogObject
    {
      if (string.IsNullOrEmpty(displayName))
        displayName = this.Context.GetDisplayName(typeof (T));
      T catalogObject = this.CreateCatalogObject<T>(this.CatalogNode.CreateChild(CatalogObjectUtilities.GetResourceTypeIdentifier<T>(), displayName));
      catalogObject.m_parent = this;
      this.Children.Add((CatalogObject) catalogObject);
      this.Context.AddObject((CatalogObject) catalogObject);
      this.Context.RefreshOnTransactionContextFailureOrReset(this);
      return catalogObject;
    }

    public T CreateChild<T>() where T : CatalogObject => this.CreateChild<T>((string) null);

    public void DeleteChild(CatalogObject child)
    {
      this.Context.DeleteObject(child);
      this.Children.Remove(child);
      this.Context.RefreshOnTransactionContextFailureOrReset(this);
    }

    public void DeleteChildren<T>() where T : CatalogObject
    {
      foreach (T child in this.Children.Where<CatalogObject>((Func<CatalogObject, bool>) (i => i.GetType() == typeof (T))).ToList<CatalogObject>())
        this.DeleteChild((CatalogObject) child);
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected void NotifyPropertyChanged(string propertyName)
    {
      if (this.PropertyChanged == null || propertyName == null)
        return;
      this.PropertyChanged((object) this, new PropertyChangedEventArgs(propertyName));
    }

    protected virtual void BuildDisplayPath(StringBuilder sb)
    {
      if (this.Parent != null)
        this.Parent.BuildDisplayPath(sb);
      sb.Append("\\");
      if (this.DisplayName == null)
        return;
      sb.Append(this.DisplayName);
    }

    protected void SetProperty<T>(string propertyName, T value)
    {
      string str = CatalogObjectUtilities.PropertyValueToString<T>(value);
      if (str != null)
        this.CatalogNode.Resource.Properties[propertyName] = str;
      else
        this.CatalogNode.Resource.Properties.Remove(propertyName);
      this.MarkAsDirty();
      this.NotifyPropertyChanged(propertyName);
    }

    protected T GetProperty<T>(string propertyName)
    {
      string stringValue = (string) null;
      return this.CatalogNode.Resource.Properties.TryGetValue(propertyName, out stringValue) ? CatalogObjectUtilities.PropertyValueFromString<T>(stringValue) : default (T);
    }

    protected void SetServiceRefence(string key, ServiceDefinition service)
    {
      if (service != null)
        this.CatalogNode.Resource.ServiceReferences[key] = service;
      else
        this.CatalogNode.Resource.ServiceReferences.Remove(key);
      this.MarkAsDirty();
      this.NotifyPropertyChanged(key);
    }

    protected ServiceDefinition GetServiceReference(string key)
    {
      ServiceDefinition serviceReference;
      this.CatalogNode.Resource.ServiceReferences.TryGetValue(key, out serviceReference);
      return serviceReference;
    }

    protected Uri LocationAsUrl(ServiceDefinition service)
    {
      Uri result = (Uri) null;
      if (service != null)
      {
        ILocationService locationService = this.Context.LocationService;
        if (locationService != null)
        {
          string uriString = locationService.LocationForCurrentConnection(service);
          if (!string.IsNullOrEmpty(uriString))
            Uri.TryCreate(uriString, UriKind.Absolute, out result);
        }
      }
      return result;
    }

    protected ICollection<T> GetChildCollection<T>(ref ICollection<T> result) where T : CatalogObject
    {
      if (result == null)
      {
        foreach (T queryChild in (IEnumerable<T>) this.QueryChildren<T>(false))
          this.Children.Add((CatalogObject) queryChild);
        result = (ICollection<T>) new CatalogObjectCollection<T>(this.Children);
      }
      return result;
    }

    protected T GetChild<T>(ref object result) where T : CatalogObject
    {
      if (result == CatalogObject.NullObject || result != null)
      {
        result = (object) this.Children.FirstOrDefault<CatalogObject>((Func<CatalogObject, bool>) (c => typeof (T) == c.GetType())) ?? CatalogObject.NullObject;
      }
      else
      {
        result = (object) this.Children.FirstOrDefault<CatalogObject>((Func<CatalogObject, bool>) (c => typeof (T) == c.GetType()));
        if (result == null)
        {
          result = (object) this.QueryChild<T>(false) ?? CatalogObject.NullObject;
          if (result != CatalogObject.NullObject)
            this.Children.Add((CatalogObject) result);
        }
      }
      return result == CatalogObject.NullObject ? default (T) : result as T;
    }

    protected ICollection<T> PreloadChildren<T>(CatalogBulkData bulkData) where T : CatalogObject
    {
      ICollection<T> objs = (ICollection<T>) null;
      Guid resourceTypeId = CatalogObjectUtilities.GetResourceTypeIdentifier<T>();
      if (bulkData.QueriedForResourceType(resourceTypeId))
      {
        foreach (CatalogNode catalogNode in bulkData.Nodes.Where<CatalogNode>((Func<CatalogNode, bool>) (node => node.Resource.ResourceType.Identifier == resourceTypeId && node.ParentPath == this.CatalogNode.FullPath)))
        {
          T catalogObject = this.Context.CreateCatalogObject<T>(catalogNode);
          this.Children.Add((CatalogObject) catalogObject);
          catalogObject.Preload(bulkData);
        }
        objs = (ICollection<T>) new CatalogObjectCollection<T>(this.Children);
      }
      return objs;
    }

    protected void PreloadChild<T>(CatalogBulkData bulkData, ref object result) where T : CatalogObject
    {
      T obj = default (T);
      Guid resourceTypeId = CatalogObjectUtilities.GetResourceTypeIdentifier<T>();
      if (bulkData.QueriedForResourceType(resourceTypeId))
      {
        obj = this.CreateCatalogObject<T>(bulkData.FindChildren(this.CatalogNodeFullPath).FirstOrDefault<CatalogNode>((Func<CatalogNode, bool>) (node => node.Resource.ResourceType.Identifier == resourceTypeId && node.ParentPath == this.CatalogNode.FullPath)));
        if ((object) obj != null)
        {
          this.Children.Add((CatalogObject) obj);
          obj.Preload(bulkData);
        }
      }
      result = (object) obj ?? CatalogObject.NullObject;
    }

    protected T GetParent<T>() where T : CatalogObject
    {
      if (this.m_parent == null)
        this.m_parent = (CatalogObject) this.QueryParent<T>(false);
      return this.m_parent == CatalogObject.NullObject ? default (T) : this.m_parent as T;
    }

    protected ICollection<T> GetDependencyCollection<T>(string dependencyName) where T : CatalogObject
    {
      ICollection<T> dependencyCollection;
      if (this.SetDependencies.ContainsKey(dependencyName))
      {
        dependencyCollection = (ICollection<T>) this.SetDependencies[dependencyName];
      }
      else
      {
        this.EnsureDependenciesExpanded();
        IEnumerable<CatalogNode> dependencySet = this.CatalogNode.Dependencies.GetDependencySet(dependencyName);
        dependencyCollection = (ICollection<T>) new DependencyCollection<T>(this, dependencyName, (ICollection<T>) dependencySet.Select<CatalogNode, T>((Func<CatalogNode, T>) (node => this.CreateCatalogObject<T>(node))).ToList<T>());
        this.SetDependencies.Add(dependencyName, (object) dependencyCollection);
      }
      return dependencyCollection;
    }

    protected T GetDependency<T>(string dependencyName) where T : CatalogObject
    {
      object obj;
      if (this.SingletonDependencies.ContainsKey(dependencyName))
      {
        obj = this.SingletonDependencies[dependencyName];
      }
      else
      {
        this.EnsureDependenciesExpanded();
        obj = (object) this.CreateCatalogObject<T>(this.CatalogNode.Dependencies.GetSingletonDependency(dependencyName)) ?? CatalogObject.NullObject;
        this.SingletonDependencies.Add(dependencyName, obj);
      }
      return obj == CatalogObject.NullObject ? default (T) : obj as T;
    }

    protected void SetDependency<T>(string dependencyName, T value) where T : CatalogObject
    {
      this.EnsureDependenciesExpanded();
      CatalogNode catalogNode = (object) value != null ? value.CatalogNode : (CatalogNode) null;
      if (catalogNode != null)
      {
        this.CatalogNode.Dependencies.SetSingletonDependency(dependencyName, catalogNode);
        this.SingletonDependencies[dependencyName] = (object) value;
      }
      else
      {
        this.CatalogNode.Dependencies.RemoveSingletonDependency(dependencyName);
        this.SingletonDependencies.Remove(dependencyName);
      }
      this.MarkAsDirty();
      this.NotifyPropertyChanged(dependencyName);
    }

    private void EnsureDependenciesExpanded()
    {
      if (this.CatalogNode.NodeDependenciesIncluded)
        return;
      this.CatalogNode.ExpandDependencies();
    }

    public override string ToString() => this.CatalogNode.Resource.DisplayName + " (" + this.CatalogNode.Resource.ResourceType.Identifier.ToString() + ")";

    protected T CreateCatalogObject<T>(CatalogNode catalogNode) where T : CatalogObject => this.Context.CreateCatalogObject<T>(catalogNode);

    protected ICollection<T> QueryChildren<T>(bool recurse) where T : CatalogObject => this.QueryChildren<T>((Dictionary<string, string>) null, recurse);

    protected ICollection<T> QueryChildren<T>(
      string propertyName,
      object propertyValue,
      bool recurse)
      where T : CatalogObject
    {
      return this.QueryChildren<T>(new Dictionary<string, string>()
      {
        [propertyName] = CatalogObjectUtilities.PropertyValueToString<object>(propertyValue)
      }, recurse);
    }

    protected ICollection<T> QueryChildren<T>(Dictionary<string, string> filters, bool recurse) where T : CatalogObject
    {
      ICollection<T> objs = (ICollection<T>) new List<T>();
      if (!string.IsNullOrEmpty(this.CatalogNode.FullPath))
        objs = (ICollection<T>) this.CatalogNode.QueryChildren((IEnumerable<Guid>) CatalogObjectUtilities.GetResourceTypeIdentifierFilter<T>(), (IEnumerable<KeyValuePair<string, string>>) filters, recurse, this.Context.QueryOptions).Select<CatalogNode, T>((Func<CatalogNode, T>) (c => this.Context.CreateCatalogObject<T>(c))).ToList<T>();
      return objs;
    }

    protected ICollection<T> QueryDependents<T>(bool includeParents, bool expandDependencies) where T : CatalogObject
    {
      CatalogQueryOptions queryOptions = CatalogQueryOptions.None;
      if (includeParents)
        queryOptions |= CatalogQueryOptions.IncludeParents;
      if (expandDependencies)
        queryOptions |= CatalogQueryOptions.ExpandDependencies;
      ICollection<CatalogNode> source = (ICollection<CatalogNode>) this.CatalogNode.QueryDependents(queryOptions);
      Guid resourceTypeId;
      return CatalogObjectUtilities.TryGetResourceTypeIdentifier(typeof (T), out resourceTypeId) ? (ICollection<T>) source.Where<CatalogNode>((Func<CatalogNode, bool>) (n => VssStringComparer.Guid.Equals((object) n.Resource.ResourceType.Identifier, (object) resourceTypeId))).Select<CatalogNode, T>((Func<CatalogNode, T>) (c => this.Context.CreateCatalogObject<T>(c))).ToList<T>() : (ICollection<T>) source.Select<CatalogNode, T>((Func<CatalogNode, T>) (c => this.Context.CreateCatalogObject<T>(c))).ToList<T>();
    }

    protected T QueryParent<T>(bool recurse) where T : CatalogObject
    {
      CatalogNode catalogNode = (CatalogNode) null;
      if (!recurse)
        catalogNode = this.CatalogNode.ParentNode;
      if (catalogNode == null)
        catalogNode = this.CatalogNode.QueryParents((IEnumerable<Guid>) CatalogObjectUtilities.GetResourceTypeIdentifierFilter<T>(), recurse, this.Context.QueryOptions).FirstOrDefault<CatalogNode>();
      return this.CreateCatalogObject<T>(catalogNode);
    }

    protected T QueryChild<T>(bool recurse) where T : CatalogObject => this.QueryChild<T>((Dictionary<string, string>) null, recurse);

    protected T QueryChild<T>(string propertyName, object propertyValue, bool recurse) where T : CatalogObject => this.QueryChild<T>(new Dictionary<string, string>()
    {
      [propertyName] = CatalogObjectUtilities.PropertyValueToString<object>(propertyValue)
    }, recurse);

    protected T QueryChild<T>(Dictionary<string, string> filters, bool recurse) where T : CatalogObject => this.CreateCatalogObject<T>(this.CatalogNode.QueryChildren((IEnumerable<Guid>) CatalogObjectUtilities.GetResourceTypeIdentifierFilter<T>(), (IEnumerable<KeyValuePair<string, string>>) filters, recurse, this.Context.QueryOptions).FirstOrDefault<CatalogNode>());

    protected ICollection<T> GetDependentsCollection<T>(ref ICollection<T> result) where T : CatalogObject
    {
      if (result == null)
        result = this.QueryDependents<T>();
      return result;
    }

    protected ICollection<T> QueryDependents<T>() where T : CatalogObject
    {
      if (!string.IsNullOrEmpty(this.CatalogNode.FullPath))
      {
        ReadOnlyCollection<CatalogNode> readOnlyCollection = this.CatalogNode.QueryDependents(this.Context.QueryOptions);
        Guid resourceTypeIdentifier = CatalogObjectUtilities.GetResourceTypeIdentifier<T>();
        foreach (CatalogNode catalogNode in (IEnumerable<CatalogNode>) readOnlyCollection)
        {
          if (catalogNode.Resource.ResourceType.Identifier == resourceTypeIdentifier)
            this.Dependents.Add((CatalogObject) this.Context.CreateCatalogObject<T>(catalogNode));
        }
      }
      return (ICollection<T>) new CatalogObjectCollection<T>(this.Dependents);
    }

    private static bool IsNodeOfType<T>(CatalogNode node)
    {
      if (node == null)
        return false;
      Guid resourceTypeIdentifier = CatalogObjectUtilities.GetResourceTypeIdentifier<T>();
      return node.Resource.ResourceType.Identifier == resourceTypeIdentifier;
    }
  }
}
