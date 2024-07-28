// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.DataServices.Compatibility.WorkItemUpdateWrapper
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server.DataServices, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 929F0284-16B2-4277-9F4A-B615689A77D1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Server.DataServices.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.DataServices.Compatibility
{
  public class WorkItemUpdateWrapper : WorkItemUpdate
  {
    private Lazy<IReadOnlyDictionary<string, object>> m_correlationObjects;
    private Lazy<SortedDictionary<string, WorkItemResourceLinkUpdate>> m_resourceLinkUpdates = new Lazy<SortedDictionary<string, WorkItemResourceLinkUpdate>>();
    private Lazy<SortedDictionary<string, WorkItemLinkUpdate>> m_linkUpdates = new Lazy<SortedDictionary<string, WorkItemLinkUpdate>>();

    internal WorkItemUpdateWrapper(
      int id,
      WorkItemUpdateWrapper.WorkItemWrapperUpdateType updateType,
      IEnumerable<string> computedColumns,
      bool isWorkItem,
      bool hasTempId,
      bool? bypassRules,
      string correlationId)
    {
      this.Id = id;
      this.Fields = (IEnumerable<KeyValuePair<string, object>>) new List<KeyValuePair<string, object>>();
      this.UpdateType = new WorkItemUpdateWrapper.WorkItemWrapperUpdateType?(updateType);
      this.ComputedColumns = computedColumns == null ? new ReadOnlyCollection<string>((IList<string>) new List<string>()) : computedColumns.ToList<string>().AsReadOnly();
      this.IsWorkItem = isWorkItem;
      this.BypassRules = bypassRules;
      this.HasTempId = hasTempId;
      this.m_correlationObjects = new Lazy<IReadOnlyDictionary<string, object>>(new Func<IReadOnlyDictionary<string, object>>(this.CreateCorrelationObjects));
      this.CorrelationId = correlationId;
    }

    public bool HasLinkUpdates => this.LinkUpdates != null && this.LinkUpdates.Any<WorkItemLinkUpdate>();

    public bool HasResourceLinkUpdates => this.ResourceLinkUpdates != null && this.ResourceLinkUpdates.Any<WorkItemResourceLinkUpdate>();

    public bool HasTagUpdates => this.Fields != null && this.Fields.Any<KeyValuePair<string, object>>((Func<KeyValuePair<string, object>, bool>) (p => TFStringComparer.WorkItemFieldReferenceName.Equals(p.Key, "System.Tags")));

    public ReadOnlyCollection<string> ComputedColumns { get; private set; }

    public WorkItemUpdateWrapper.WorkItemWrapperUpdateType? UpdateType { get; private set; }

    public bool IsWorkItem { get; private set; }

    public bool? BypassRules { get; private set; }

    public bool HasTempId { get; private set; }

    public string CorrelationId { get; private set; }

    public bool HasComputedColumn(string columnName)
    {
      ArgumentUtility.CheckForNull<string>(columnName, nameof (columnName));
      return this.ComputedColumns.Contains<string>(columnName, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    }

    public bool TryGetCorrelationObject<T>(string key, out T value) where T : class
    {
      ArgumentUtility.CheckForNull<string>(key, nameof (key));
      value = default (T);
      object obj = (object) null;
      if (this.ShouldRefreshCorrelationObjects())
        this.m_correlationObjects = new Lazy<IReadOnlyDictionary<string, object>>(new Func<IReadOnlyDictionary<string, object>>(this.CreateCorrelationObjects));
      if (!this.m_correlationObjects.Value.TryGetValue(key, out obj))
        return false;
      value = obj as T;
      return true;
    }

    private IReadOnlyDictionary<string, object> CreateCorrelationObjects()
    {
      Dictionary<string, object> dictionary = new Dictionary<string, object>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      if (this.HasLinkUpdates)
      {
        if (this.LinkUpdates.Any<WorkItemLinkUpdate>((Func<WorkItemLinkUpdate, bool>) (x => string.IsNullOrWhiteSpace(x.CorrelationId))))
          throw new ArgumentException("Never should have null correlation objects by this point");
        foreach (WorkItemLinkUpdate linkUpdate in this.LinkUpdates)
          dictionary.Add(linkUpdate.CorrelationId, (object) linkUpdate);
      }
      if (this.HasResourceLinkUpdates)
      {
        if (this.ResourceLinkUpdates.Any<WorkItemResourceLinkUpdate>((Func<WorkItemResourceLinkUpdate, bool>) (x => string.IsNullOrWhiteSpace(x.CorrelationId))))
          throw new ArgumentException("Never should have null correlation objects by this point");
        foreach (WorkItemResourceLinkUpdate resourceLinkUpdate in this.ResourceLinkUpdates)
          dictionary.Add(resourceLinkUpdate.CorrelationId, (object) resourceLinkUpdate);
      }
      return (IReadOnlyDictionary<string, object>) new ReadOnlyDictionary<string, object>((IDictionary<string, object>) dictionary);
    }

    private bool ShouldRefreshCorrelationObjects() => (this.HasLinkUpdates ? this.LinkUpdates.Count<WorkItemLinkUpdate>() : 0) + (this.HasResourceLinkUpdates ? this.ResourceLinkUpdates.Count<WorkItemResourceLinkUpdate>() : 0) != this.m_correlationObjects.Value.Count;

    internal void AttachResourceLinks(
      IEnumerable<WorkItemResourceLinkUpdate> linkUpdates)
    {
      if (linkUpdates == null)
        throw new ArgumentNullException(nameof (linkUpdates));
      if (!linkUpdates.Any<WorkItemResourceLinkUpdate>())
        return;
      foreach (WorkItemResourceLinkUpdate linkUpdate in linkUpdates)
        this.m_resourceLinkUpdates.Value.Add(linkUpdate.CorrelationId, linkUpdate);
      this.ResourceLinkUpdates = (IEnumerable<WorkItemResourceLinkUpdate>) this.m_resourceLinkUpdates.Value.Values;
    }

    internal void AttachLink(WorkItemLinkUpdate linkUpdate)
    {
      if (linkUpdate == null)
        throw new ArgumentNullException(nameof (linkUpdate));
      if (this.m_linkUpdates.Value.ContainsKey(linkUpdate.CorrelationId))
        throw new ArgumentException(string.Format("Link with correlation id {0} was already added.\r\nNew link: \r\n{1}\r\nExisting Link:\r\n{2}", (object) linkUpdate.CorrelationId, (object) linkUpdate.ToElement(), (object) this.m_linkUpdates.Value[linkUpdate.CorrelationId].ToElement()));
      this.m_linkUpdates.Value.Add(linkUpdate.CorrelationId, linkUpdate);
      this.LinkUpdates = (IEnumerable<WorkItemLinkUpdate>) this.m_linkUpdates.Value.Values;
    }

    internal void AttachLinks(IEnumerable<WorkItemLinkUpdate> linkUpdates)
    {
      if (linkUpdates == null)
        throw new ArgumentNullException(nameof (linkUpdates));
      foreach (WorkItemLinkUpdate linkUpdate in linkUpdates)
        this.AttachLink(linkUpdate);
    }

    internal void AddFieldUpdate(string fieldRef, object value) => (this.Fields as List<KeyValuePair<string, object>>).Add(new KeyValuePair<string, object>(fieldRef, value));

    protected override string DebuggerDisplay => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0},  UpdateType = {1}, IsWorkItem = {2}, CorrelationId = {3}, CorrelationIds.Count = {4}, ByPassRules = {5}, HasTempId = {6}, ComputedColumns.Count = {7}", (object) base.DebuggerDisplay, (object) this.UpdateType, (object) this.IsWorkItem, (object) this.CorrelationId, (object) (this.m_correlationObjects.IsValueCreated ? this.m_correlationObjects.Value.Count : 0), (object) this.BypassRules, (object) this.HasTempId, (object) (this.ComputedColumns != null ? this.ComputedColumns.Count : 0));

    public enum WorkItemWrapperUpdateType
    {
      Insert,
      Update,
      Delete,
    }
  }
}
