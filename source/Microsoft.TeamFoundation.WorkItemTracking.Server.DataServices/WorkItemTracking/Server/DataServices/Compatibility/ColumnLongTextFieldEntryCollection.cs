// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.DataServices.Compatibility.ColumnLongTextFieldEntryCollection
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server.DataServices, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 929F0284-16B2-4277-9F4A-B615689A77D1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Server.DataServices.dll

using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.DataServices.Compatibility
{
  internal class ColumnLongTextFieldEntryCollection
  {
    public ColumnLongTextFieldEntryCollection(
      WorkItemTrackingRequestContext witRequestContext,
      IEnumerable<string> columns,
      IEnumerable<int> longTextColumns,
      params int[] fieldsToAdd)
    {
      IFieldTypeDictionary fieldDict = witRequestContext != null ? witRequestContext.FieldDictionary : throw new ArgumentNullException(nameof (witRequestContext));
      FieldEntry[] fieldEntryArray = columns == null || !columns.Any<string>() ? Array.Empty<FieldEntry>() : columns.Select<string, FieldEntry>((Func<string, FieldEntry>) (fname => fieldDict.GetField(fname))).ToArray<FieldEntry>();
      this.ColumnFields = (IEnumerable<FieldEntry>) new ReadOnlyCollection<FieldEntry>((IList<FieldEntry>) fieldEntryArray);
      HashSet<int> intSet1 = new HashSet<int>(((IEnumerable<FieldEntry>) fieldEntryArray).Select<FieldEntry, int>((Func<FieldEntry, int>) (f => f.FieldId)));
      if (fieldsToAdd != null && ((IEnumerable<int>) fieldsToAdd).Any<int>())
      {
        foreach (int num in fieldsToAdd)
          intSet1.Add(num);
      }
      this.FieldIds = (IEnumerable<int>) intSet1;
      FieldEntry[] list;
      if (longTextColumns != null)
      {
        int[] array = longTextColumns.Distinct<int>().ToArray<int>();
        intSet1.UnionWith((IEnumerable<int>) array);
        List<FieldEntry> fieldEntryList = new List<FieldEntry>();
        foreach (int id in array)
        {
          FieldEntry field;
          if (fieldDict.TryGetField(id, out field))
            fieldEntryList.Add(field);
        }
        list = fieldEntryList.ToArray();
      }
      else
        list = Array.Empty<FieldEntry>();
      this.LongTextFields = (IEnumerable<FieldEntry>) new ReadOnlyCollection<FieldEntry>((IList<FieldEntry>) list);
      HashSet<int> intSet2 = new HashSet<int>();
      foreach (int fieldId in this.FieldIds)
      {
        if (fieldDict.TryGetField(fieldId, out FieldEntry _))
          intSet2.Add(fieldId);
      }
      this.ExistingFieldIds = (IEnumerable<int>) intSet2;
    }

    public IEnumerable<FieldEntry> ColumnFields { get; private set; }

    public IEnumerable<int> ExistingFieldIds { get; private set; }

    public IEnumerable<int> FieldIds { get; private set; }

    public IEnumerable<FieldEntry> LongTextFields { get; private set; }
  }
}
