// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Configuration.Index
// Assembly: Microsoft.VisualStudio.Services.Configuration, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AB461A1-8255-4EAB-B12B-E1D379571DC1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Configuration.dll

using Microsoft.TeamFoundation.Common.Internal;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Microsoft.VisualStudio.Services.Configuration
{
  public class Index
  {
    public Index() => this.Columns = new List<IndexColumn>();

    public string Name { get; set; }

    public string TableName { get; set; }

    public string TableSchema { get; set; }

    public bool IsPrimaryKey { get; set; }

    public bool IsSystemNamed { get; set; }

    public bool IsConstraint { get; set; }

    public IndexType Type { get; set; }

    public bool IsUnique { get; set; }

    public bool IsUniqueConstraint { get; set; }

    public bool IgnoreDupKey { get; set; }

    public byte FillFactor { get; set; }

    public bool IsPadded { get; set; }

    public bool IsDisabled { get; set; }

    public bool IsHypothetical { get; set; }

    public bool AllowRowLocks { get; set; }

    public bool AllowPageLocks { get; set; }

    public bool HasFilter { get; set; }

    public string FilterDefinition { get; set; }

    public bool NoRecompute { get; set; }

    public int PartitionCount { get; set; }

    public DataCompression DataCompression { get; set; }

    public bool ScriptDropExisting { get; set; }

    public string PreviousConstraintName { get; set; }

    public List<IndexColumn> Columns { get; private set; }

    public void ScriptConstraint(StringCollection queries, int offset)
    {
      string prefix = new string(' ', offset);
      string str1 = this.IsSystemNamed ? new string(' ', offset - 1) : string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}CONSTRAINT {1}", (object) prefix, (object) StringUtil.QuoteName(this.Name));
      if (this.IsPrimaryKey)
        str1 += " PRIMARY KEY";
      else if (this.IsUnique)
        str1 += " UNIQUE";
      string str2 = (this.Type != IndexType.Clustered ? str1 + " NONCLUSTERED" : str1 + " CLUSTERED") + " (";
      queries.Add(str2);
      this.ScriptConstraintColumns(prefix, queries);
      queries.Add(prefix + ") " + this.ScriptOptions());
    }

    private static string GetOnOff(bool value) => !value ? "OFF" : "ON";

    private string ScriptOptions()
    {
      StringBuilder stringBuilder = new StringBuilder();
      if (this.IsPadded)
        stringBuilder.AppendFormat("WITH (PAD_INDEX = {0},", (object) Index.GetOnOff(this.IsPadded));
      else
        stringBuilder.Append("WITH (");
      stringBuilder.AppendFormat(" STATISTICS_NORECOMPUTE = {0}", (object) Index.GetOnOff(this.NoRecompute));
      stringBuilder.AppendFormat(", IGNORE_DUP_KEY = {0}", (object) Index.GetOnOff(this.IgnoreDupKey));
      if (!this.AllowRowLocks)
        stringBuilder.AppendFormat(", ALLOW_ROW_LOCKS = {0}", (object) Index.GetOnOff(this.AllowRowLocks));
      if (!this.AllowPageLocks)
        stringBuilder.AppendFormat(", ALLOW_PAGE_LOCKS = {0}", (object) Index.GetOnOff(this.AllowPageLocks));
      if (this.FillFactor != (byte) 0)
        stringBuilder.AppendFormat(", FILLFACTOR = {0}", (object) this.FillFactor);
      if (this.DataCompression != DataCompression.None)
        stringBuilder.AppendFormat(", DATA_COMPRESSION = {0}", (object) this.DataCompression.ToString().ToUpperInvariant());
      if (this.ScriptDropExisting && !this.IsConstraint)
        stringBuilder.Append(", DROP_EXISTING = ON");
      stringBuilder.Append(")");
      return stringBuilder.ToString();
    }

    private void ScriptConstraintColumns(string prefix, StringCollection queries)
    {
      IndexColumn[] array = this.Columns.Where<IndexColumn>((Func<IndexColumn, bool>) (indexColumn => !indexColumn.IsIncludedColumn)).ToArray<IndexColumn>();
      for (int index = 0; index < array.Length; ++index)
      {
        IndexColumn indexColumn = array[index];
        string str = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}{1} {2}{3}", (object) prefix, (object) StringUtil.QuoteName(indexColumn.Name), indexColumn.IsDescending ? (object) "DESC" : (object) "ASC", index == array.Length - 1 ? (object) "" : (object) ",");
        queries.Add(str);
      }
    }

    private void ScriptIncludedColumns(string prefix, StringCollection queries)
    {
      IndexColumn[] array = this.Columns.Where<IndexColumn>((Func<IndexColumn, bool>) (indexColumn => indexColumn.IsIncludedColumn)).ToArray<IndexColumn>();
      for (int index = 0; index < array.Length; ++index)
      {
        IndexColumn indexColumn = array[index];
        string str = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}{1} {2}", (object) prefix, (object) StringUtil.QuoteName(indexColumn.Name), index == array.Length - 1 ? (object) "" : (object) ",");
        queries.Add(str);
      }
    }

    private void ScriptIfConstraintExist(StringCollection queries, int offset)
    {
      if (this.ScriptDropExisting)
        return;
      string str = new string(' ', offset);
      queries.Add(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}IF NOT EXISTS(select * FROM sys.indexes WHERE name = '{1}')", (object) str, (object) this.Name));
    }

    public void ScriptAlterTable(StringCollection queries, int offset)
    {
      this.ScriptIfConstraintExist(queries, offset);
      string str1 = new string(' ', offset);
      queries.Add(str1 + "BEGIN");
      string str2 = str1 + "END";
      offset += 4;
      string str3 = new string(' ', offset);
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append(str3);
      stringBuilder.AppendFormat("ALTER TABLE {0}.{1} WITH NOCHECK ADD", (object) StringUtil.QuoteName(this.TableSchema), (object) StringUtil.QuoteName(this.TableName));
      queries.Add(stringBuilder.ToString());
      this.ScriptConstraint(queries, offset + 4);
      queries.Add(str2);
    }

    public void ScriptCreateIndex(StringCollection queries, int offset)
    {
      this.ScriptIfConstraintExist(queries, offset);
      string str1 = new string(' ', offset);
      queries.Add(str1 + "BEGIN");
      string str2 = str1 + "END";
      offset += 4;
      string str3 = new string(' ', offset);
      queries.Add(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}CREATE {1}{2} INDEX {3} ON {4}.{5}", (object) str3, this.IsUnique ? (object) "UNIQUE " : (object) "", this.Type == IndexType.Clustered ? (object) "CLUSTERED" : (object) "NONCLUSTERED", (object) StringUtil.QuoteName(this.Name), (object) StringUtil.QuoteName(this.TableSchema), (object) StringUtil.QuoteName(this.TableName)));
      queries.Add(str3 + "(");
      this.ScriptConstraintColumns(str3 + "  ", queries);
      queries.Add(str3 + ")");
      if (this.HasIncludedColumns)
      {
        queries.Add(str3 + "INCLUDE");
        queries.Add(str3 + "(");
        this.ScriptIncludedColumns(str3 + "  ", queries);
        queries.Add(str3 + ")");
      }
      if (this.HasFilter)
        queries.Add(str3 + " WHERE " + this.FilterDefinition + " ");
      queries.Add(str3 + this.ScriptOptions());
      queries.Add(str2);
      if (!this.IsDisabled)
        return;
      queries.Add("");
      string str4 = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}ALTER INDEX {1} ON {2}.{3} DISABLE", (object) str3, (object) StringUtil.QuoteName(this.Name), (object) StringUtil.QuoteName(this.TableSchema), (object) StringUtil.QuoteName(this.TableName));
      queries.Add(str4);
    }

    private bool HasIncludedColumns => this.Columns.Any<IndexColumn>((Func<IndexColumn, bool>) (indexColumn => indexColumn.IsIncludedColumn));

    public static string GetIndexTypeDescription(IndexType indexType)
    {
      MemberInfo[] member = indexType.GetType().GetMember(indexType.ToString());
      if (member != null && member.Length != 0)
      {
        object[] customAttributes = member[0].GetCustomAttributes(typeof (DescriptionAttribute), false);
        if (customAttributes != null && customAttributes.Length != 0)
          return ((DescriptionAttribute) customAttributes[0]).Description;
      }
      return indexType.ToString();
    }
  }
}
