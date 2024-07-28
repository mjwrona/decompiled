// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.LockFreeFieldDictionary
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata
{
  public class LockFreeFieldDictionary : IFieldTypeDictionary
  {
    private IVssRequestContext m_requestContext;
    private Lazy<WorkItemTrackingFieldService> m_lazyFieldDictionary;
    private IFieldTypeDictionary m_snapshot;

    public LockFreeFieldDictionary(IVssRequestContext requestContext)
    {
      this.m_requestContext = requestContext;
      this.m_lazyFieldDictionary = new Lazy<WorkItemTrackingFieldService>((Func<WorkItemTrackingFieldService>) (() => this.m_requestContext.GetService<WorkItemTrackingFieldService>()));
    }

    private IFieldTypeDictionary CurrentSnapshot
    {
      get
      {
        if (this.m_snapshot == null)
          this.m_snapshot = this.m_lazyFieldDictionary.Value.GetFieldsSnapshot(this.m_requestContext);
        return this.m_snapshot;
      }
    }

    public bool TryGetField(string name, out FieldEntry field)
    {
      if (this.CurrentSnapshot.TryGetField(name, out field))
        return true;
      this.RefreshSnapshot();
      return this.CurrentSnapshot.TryGetField(name, out field);
    }

    public bool TryGetField(int id, out FieldEntry field)
    {
      if (this.CurrentSnapshot.TryGetField(id, out field))
        return true;
      this.RefreshSnapshot();
      return this.CurrentSnapshot.TryGetField(id, out field);
    }

    public bool TryGetFieldByNameOrId(string nameOrId, out FieldEntry field)
    {
      if (this.CurrentSnapshot.TryGetFieldByNameOrId(nameOrId, out field))
        return true;
      this.RefreshSnapshot();
      return this.CurrentSnapshot.TryGetFieldByNameOrId(nameOrId, out field);
    }

    public FieldEntry GetField(string name)
    {
      FieldEntry field;
      if (this.CurrentSnapshot.TryGetField(name, out field))
        return field;
      this.RefreshSnapshot();
      return this.CurrentSnapshot.GetField(name);
    }

    public FieldEntry GetField(int id)
    {
      FieldEntry field;
      if (this.CurrentSnapshot.TryGetField(id, out field))
        return field;
      this.RefreshSnapshot();
      return this.CurrentSnapshot.GetField(id);
    }

    public FieldEntry GetFieldByNameOrId(string nameOrId)
    {
      FieldEntry field;
      if (this.CurrentSnapshot.TryGetFieldByNameOrId(nameOrId, out field))
        return field;
      this.RefreshSnapshot();
      return this.CurrentSnapshot.GetFieldByNameOrId(nameOrId);
    }

    public IReadOnlyCollection<FieldEntry> GetAllFields() => this.CurrentSnapshot.GetAllFields();

    public IReadOnlyCollection<FieldEntry> GetCoreFields() => this.CurrentSnapshot.GetCoreFields();

    public ISet<int> GetHistoryDisabledFieldIds() => this.CurrentSnapshot.GetHistoryDisabledFieldIds();

    private void RefreshSnapshot() => this.m_snapshot = this.m_lazyFieldDictionary.Value.GetFieldsSnapshot(this.m_requestContext);
  }
}
