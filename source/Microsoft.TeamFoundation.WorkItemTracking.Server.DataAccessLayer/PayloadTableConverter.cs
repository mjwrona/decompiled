// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.PayloadTableConverter
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 815CF582-E66F-43C7-9B50-57E1C71BBC84
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer.dll

using System;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  internal class PayloadTableConverter
  {
    private Dictionary<int, PayloadTableConverter.PayloadTableReadActionSet> m_readActionsByFieldId = new Dictionary<int, PayloadTableConverter.PayloadTableReadActionSet>();
    private Dictionary<string, PayloadTableConverter.PayloadTableReadActionSet> m_readActionsByFieldName = new Dictionary<string, PayloadTableConverter.PayloadTableReadActionSet>();
    private Dictionary<int, List<PayloadTableWriteAction>> m_writeActionsByFieldId = new Dictionary<int, List<PayloadTableWriteAction>>();
    private Dictionary<string, List<PayloadTableWriteAction>> m_writeActionsByFieldName = new Dictionary<string, List<PayloadTableWriteAction>>();
    private SortedDictionary<int, PayloadTableColumnDescriptor> m_addedColumns = new SortedDictionary<int, PayloadTableColumnDescriptor>();
    private List<string> m_removedColumns = new List<string>();
    private List<ProcessColumnCallback> m_processColumnCallbackList;
    private PayloadConverter m_owner;

    public void AddNewColumn(
      string columnName,
      Type columnType,
      int payloadIndex,
      object defaultValue)
    {
      if (string.IsNullOrEmpty(columnName))
        throw new ArgumentNullException(nameof (columnName));
      if (payloadIndex < 0)
        throw new ArgumentOutOfRangeException(nameof (payloadIndex));
      if (columnType == (Type) null)
        throw new ArgumentNullException(nameof (columnType));
      this.m_addedColumns.Add(payloadIndex, new PayloadTableColumnDescriptor(columnName, columnType, payloadIndex, defaultValue));
    }

    public void AddNewColumn(string columnName, Type columnType, int payloadIndex) => this.AddNewColumn(columnName, columnType, payloadIndex, (object) null);

    public void RemoveExistingColumn(string columnName)
    {
      if (string.IsNullOrEmpty(columnName))
        throw new ArgumentNullException(nameof (columnName));
      if (this.m_removedColumns.Contains(columnName))
        throw new ArgumentOutOfRangeException(nameof (columnName));
      this.m_removedColumns.Add(columnName);
    }

    public void AddReadAction(
      int dsFieldIndex,
      PayloadTableReadAction readAction,
      bool overrideDefaultReadAction)
    {
      if (readAction == null)
        throw new ArgumentNullException(nameof (readAction));
      if (!this.m_readActionsByFieldId.ContainsKey(dsFieldIndex))
        this.m_readActionsByFieldId.Add(dsFieldIndex, new PayloadTableConverter.PayloadTableReadActionSet());
      this.m_readActionsByFieldId[dsFieldIndex].ReadActions.Add(readAction);
      if (!overrideDefaultReadAction)
        return;
      this.m_readActionsByFieldId[dsFieldIndex].OverrideDefaultReadAction = true;
    }

    public void AddReadAction(
      string columnName,
      PayloadTableReadAction readAction,
      bool overrideDefaultReadAction)
    {
      if (readAction == null)
        throw new ArgumentNullException(nameof (readAction));
      if (string.IsNullOrEmpty(columnName))
        throw new ArgumentNullException(nameof (columnName));
      if (!this.m_readActionsByFieldName.ContainsKey(columnName))
        this.m_readActionsByFieldName.Add(columnName, new PayloadTableConverter.PayloadTableReadActionSet());
      this.m_readActionsByFieldName[columnName].ReadActions.Add(readAction);
      if (!overrideDefaultReadAction)
        return;
      this.m_readActionsByFieldName[columnName].OverrideDefaultReadAction = true;
    }

    public void AddWriteAction(string columnName, PayloadTableWriteAction writeAction)
    {
      if (writeAction == null)
        throw new ArgumentNullException(nameof (writeAction));
      if (string.IsNullOrEmpty(columnName))
        throw new ArgumentNullException(nameof (columnName));
      if (!this.m_writeActionsByFieldName.ContainsKey(columnName))
        this.m_writeActionsByFieldName.Add(columnName, new List<PayloadTableWriteAction>());
      this.m_writeActionsByFieldName[columnName].Add(writeAction);
    }

    public void AddWriteAction(int fieldIndex, PayloadTableWriteAction writeAction)
    {
      if (writeAction == null)
        throw new ArgumentNullException(nameof (writeAction));
      if (!this.m_writeActionsByFieldId.ContainsKey(fieldIndex))
        this.m_writeActionsByFieldId.Add(fieldIndex, new List<PayloadTableWriteAction>());
      this.m_writeActionsByFieldId[fieldIndex].Add(writeAction);
    }

    public void AddProcessColumnCallback(ProcessColumnCallback processColumnCallback)
    {
      if (processColumnCallback == null)
        throw new ArgumentNullException(nameof (processColumnCallback));
      if (this.m_processColumnCallbackList == null)
        this.m_processColumnCallbackList = new List<ProcessColumnCallback>();
      this.m_processColumnCallbackList.Add(processColumnCallback);
    }

    public PayloadConverter PayloadConverter
    {
      internal set => this.m_owner = value;
      get => this.m_owner;
    }

    internal void RunReadActions(
      IDataReader reader,
      PayloadTable table,
      int dsFieldIndex,
      PayloadTable.PayloadRow row,
      out bool overrideDefaultReadAction)
    {
      overrideDefaultReadAction = false;
      PayloadTableConverter.PayloadTableReadActionSet tableReadActionSet;
      if (this.m_readActionsByFieldId.TryGetValue(dsFieldIndex, out tableReadActionSet))
      {
        overrideDefaultReadAction = tableReadActionSet.OverrideDefaultReadAction;
        foreach (PayloadTableReadAction readAction in tableReadActionSet.ReadActions)
          readAction(reader, table, dsFieldIndex, row);
      }
      if (!this.m_readActionsByFieldName.TryGetValue(table.Columns.GetDatasetColumnNameByDatasetColumnIndex(dsFieldIndex), out tableReadActionSet))
        return;
      if (!overrideDefaultReadAction)
        overrideDefaultReadAction = tableReadActionSet.OverrideDefaultReadAction;
      foreach (PayloadTableReadAction readAction in tableReadActionSet.ReadActions)
        readAction(reader, table, dsFieldIndex, row);
    }

    internal void RunWriteActions(
      PayloadTable table,
      int payloadFieldIndex,
      PayloadTable.PayloadRow row)
    {
      if (this.m_writeActionsByFieldId.ContainsKey(payloadFieldIndex))
      {
        foreach (PayloadTableWriteAction tableWriteAction in this.m_writeActionsByFieldId[payloadFieldIndex])
          tableWriteAction(table, payloadFieldIndex, row);
      }
      string name = table.Columns[payloadFieldIndex].Name;
      if (!this.m_writeActionsByFieldName.ContainsKey(name))
        return;
      foreach (PayloadTableWriteAction tableWriteAction in this.m_writeActionsByFieldName[name])
        tableWriteAction(table, payloadFieldIndex, row);
    }

    internal bool IsRemoved(string columnName) => !string.IsNullOrEmpty(columnName) && this.m_removedColumns.Contains(columnName);

    internal IEnumerator<KeyValuePair<int, PayloadTableColumnDescriptor>> GetAddedColumnsEnumerator() => (IEnumerator<KeyValuePair<int, PayloadTableColumnDescriptor>>) this.m_addedColumns.GetEnumerator();

    internal PayloadTableColumnDescriptor GetAddedColumnByIndex(int payloadIndex) => !this.IsAddedColumn(payloadIndex) ? (PayloadTableColumnDescriptor) null : this.m_addedColumns[payloadIndex];

    internal bool IsAddedColumn(int payloadIndex) => this.m_addedColumns.ContainsKey(payloadIndex);

    internal bool IsDefaultReadActionOverriden(PayloadTable table, int dsFieldIndex)
    {
      if (this.m_readActionsByFieldId.ContainsKey(dsFieldIndex) && this.m_readActionsByFieldId[dsFieldIndex].OverrideDefaultReadAction)
        return true;
      string datasetColumnIndex = table.Columns.GetDatasetColumnNameByDatasetColumnIndex(dsFieldIndex);
      return this.m_readActionsByFieldName.ContainsKey(datasetColumnIndex) && this.m_readActionsByFieldName[datasetColumnIndex].OverrideDefaultReadAction;
    }

    internal void ExecuteProcessColumnCallbacks(PayloadTable table, PayloadColumn column)
    {
      if (table == null)
        throw new ArgumentNullException(nameof (table));
      if (column == null)
        throw new ArgumentNullException(nameof (column));
      if (this.m_processColumnCallbackList == null)
        return;
      foreach (ProcessColumnCallback processColumnCallback in this.m_processColumnCallbackList)
        processColumnCallback(table, column);
    }

    private class PayloadTableReadActionSet
    {
      public List<PayloadTableReadAction> ReadActions = new List<PayloadTableReadAction>();
      public bool OverrideDefaultReadAction;
    }
  }
}
