// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Configuration.Table
// Assembly: Microsoft.VisualStudio.Services.Configuration, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AB461A1-8255-4EAB-B12B-E1D379571DC1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Configuration.dll

using Microsoft.TeamFoundation.Common.Internal;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Text;

namespace Microsoft.VisualStudio.Services.Configuration
{
  public class Table
  {
    public Table()
    {
      this.Columns = new List<Column>();
      this.Indexes = new List<Index>();
    }

    public string Name { get; set; }

    public string Schema { get; set; }

    public bool IsSystem { get; set; }

    public List<Column> Columns { get; private set; }

    public List<Index> Indexes { get; private set; }

    public int TableTextInRowLimit { get; set; }

    public string FullyQualifiedName => this.Schema + "." + this.Name;

    public Column IdentityColumn => this.Columns.Find((Predicate<Column>) (column => column.IsIdentity));

    public Column PartitionIdColumn => this.Columns.Find((Predicate<Column>) (column => string.Equals(column.Name, "PartitionId", StringComparison.OrdinalIgnoreCase)));

    public Column TimestampColumn => this.Columns.Find((Predicate<Column>) (column => column.IsTimestamp));

    public Index PrimaryKey => this.Indexes.Find((Predicate<Index>) (index => index.IsPrimaryKey));

    public Index ClusteredIndex => this.Indexes.Find((Predicate<Index>) (index => index.Type == IndexType.Clustered));

    public string ScriptCreate() => this.ScriptCreate(0);

    public string ScriptCreate(int offset)
    {
      StringBuilder stringBuilder = new StringBuilder();
      StringCollection queries = new StringCollection();
      this.ScriptCreate(queries, offset);
      foreach (string str in queries)
        stringBuilder.Append(str).Append("\n");
      return stringBuilder.ToString();
    }

    public void ScriptCreate(StringCollection queries, int offset)
    {
      bool flag = false;
      string str1 = new string(' ', offset);
      queries.Add(str1 + string.Format((IFormatProvider) CultureInfo.InvariantCulture, "IF NOT EXISTS(SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = {0} AND TABLE_SCHEMA = {1})", (object) StringUtil.QuoteName(this.Name, '\''), (object) StringUtil.QuoteName(this.Schema, '\'')));
      queries.Add(str1 + "BEGIN");
      string str2 = str1 + "END";
      offset += 4;
      string str3 = new string(' ', offset);
      queries.Add(str3 + string.Format((IFormatProvider) CultureInfo.InvariantCulture, "CREATE TABLE {0}.{1} (", (object) StringUtil.QuoteName(this.Schema), (object) StringUtil.QuoteName(this.Name)));
      for (int index = 0; index < this.Columns.Count; ++index)
      {
        Column column = this.Columns[index];
        string lower = column.DataType.ToLower(CultureInfo.InvariantCulture);
        if (lower == "text" || lower == "ntext" || lower == "image")
          flag = true;
        if (index != 0)
          queries[queries.Count - 1] = queries[queries.Count - 1] + ",";
        queries.Add(column.ScriptDdl(offset + 4));
      }
      foreach (Index index in this.Indexes)
      {
        if (index.IsSystemNamed)
        {
          queries.Add("");
          queries[queries.Count - 1] = queries[queries.Count - 1] + ",";
          index.ScriptConstraint(queries, offset + 4);
        }
      }
      queries.Add(str3 + ")");
      queries.Add(str2);
      offset -= 4;
      foreach (Index index in this.Indexes)
      {
        if (!index.IsSystemNamed)
        {
          queries.Add("");
          if (index.IsConstraint)
            index.ScriptAlterTable(queries, offset);
          else
            index.ScriptCreateIndex(queries, offset);
        }
      }
      if (!(this.TableTextInRowLimit != 0 & flag))
        return;
      string str4 = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "EXECUTE sp_tableoption @TableNamePattern = N{0}, @OptionName = N'text in row', @OptionValue = N'{1}'", (object) StringUtil.QuoteName(this.Schema + "." + this.Name, '\''), (object) this.TableTextInRowLimit);
      queries.Add("");
      queries.Add(new string(' ', offset) + str4);
    }

    public string GetColumnList(
      string columnPrefix,
      bool skipComputedColumns,
      bool skipTimestampColumns,
      Dictionary<string, string> fieldExpressions = null)
    {
      StringBuilder stringBuilder = new StringBuilder();
      bool flag = true;
      foreach (Column column in this.Columns)
      {
        if (!(column.IsComputed & skipComputedColumns) && !(column.IsTimestamp & skipTimestampColumns))
        {
          if (!flag)
            stringBuilder.AppendLine(", ");
          else
            flag = false;
          string str = (string) null;
          fieldExpressions?.TryGetValue(column.Name, out str);
          if (string.IsNullOrEmpty(str))
            stringBuilder.Append(columnPrefix);
          else
            stringBuilder.Append(str).Append(" ");
          stringBuilder.Append(StringUtil.QuoteName(column.Name));
        }
      }
      return stringBuilder.ToString();
    }
  }
}
