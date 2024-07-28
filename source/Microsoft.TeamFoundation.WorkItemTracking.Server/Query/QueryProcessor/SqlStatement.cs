// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Query.QueryProcessor.SqlStatement
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Query.QueryProcessor
{
  internal class SqlStatement
  {
    protected HashSet<string> m_joins;
    protected StringBuilder m_where;

    protected SqlStatement()
    {
      this.Select = new List<string>();
      this.OrderBy = new List<string>();
      this.Options = new List<string>();
      this.m_joins = new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      this.ResetPredicate();
    }

    public SqlStatement(string table, string tableAlias, bool forceLeftJoinIndexHint)
      : this()
    {
      this.Table = table;
      this.TableAlias = tableAlias;
      this.ForceLeftJoinIndexHint = forceLeftJoinIndexHint;
      this.ForceFullTextIndexHint = false;
    }

    public SqlStatement(
      string table,
      string tableAlias,
      bool forceLeftJoinIndexHint,
      bool forceFullTextIndexHint)
      : this()
    {
      this.Table = table;
      this.TableAlias = tableAlias;
      this.ForceLeftJoinIndexHint = forceLeftJoinIndexHint;
      this.ForceFullTextIndexHint = forceFullTextIndexHint;
    }

    public int? Top { get; set; }

    public string Table { get; set; }

    public string TableAlias { get; set; }

    public List<string> Select { get; set; }

    public List<string> OrderBy { get; set; }

    public List<string> Options { get; set; }

    public bool ForceLeftJoinIndexHint { get; private set; }

    public bool ForceFullTextIndexHint { get; private set; }

    public bool ForceLongTextJoin { get; set; }

    public string LongTextTable { get; set; }

    public string LongTextTableAlias { get; set; }

    public string LongTextPredicates { get; set; }

    public virtual void DefineJoin(string table, string tableAlias, string condition) => this.DefineJoin(string.Empty, table, tableAlias, condition);

    public virtual void DefineJoin(
      string joinKind,
      string table,
      string tableAlias,
      string condition)
    {
      StringBuilder stringBuilder = new StringBuilder();
      if (!string.IsNullOrWhiteSpace(joinKind))
      {
        stringBuilder.Append(joinKind);
        stringBuilder.Append(" ");
      }
      stringBuilder.AppendFormat("JOIN {0} {1}", (object) table, (object) tableAlias);
      if (this.ForceLeftJoinIndexHint && joinKind == "LEFT" && table == "dbo.tbl_WorkItemCustomLatest")
        stringBuilder.Append(" WITH (FORCESEEK(PK_tbl_WorkItemCustomLatest(PartitionId, Id, FieldId)))");
      stringBuilder.AppendLine();
      stringBuilder.Append("    ON ");
      stringBuilder.Append(condition);
      this.m_joins.Add(stringBuilder.ToString());
    }

    public void AppendPredicate(string format, params string[] param) => this.m_where.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, format, (object[]) param);

    public void Append(string value) => this.m_where.Append(value);

    public string GetPredicate() => this.m_where.ToString();

    public void ResetPredicate() => this.m_where = new StringBuilder(256);

    public virtual string GetSql()
    {
      StringBuilder stringBuilder = new StringBuilder(512);
      stringBuilder.Append("SELECT ");
      if (this.Top.HasValue)
        stringBuilder.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, "TOP {0} ", (object) this.Top.Value);
      if (this.Select.Any<string>())
        stringBuilder.AppendLine(string.Join(", ", (IEnumerable<string>) this.Select));
      else
        stringBuilder.AppendLine("*");
      if (this.ForceLongTextJoin)
      {
        stringBuilder.AppendFormat("FROM {0} {1}", (object) this.LongTextTable, (object) this.LongTextTableAlias);
        stringBuilder.AppendLine();
        stringBuilder.AppendFormat("LEFT JOIN {0} {1}", (object) this.Table, (object) this.TableAlias);
        stringBuilder.AppendLine();
        stringBuilder.AppendFormat("ON {0}.Id = {1}.Id AND {0}.PartitionId = {1}.PartitionId{2}", (object) this.TableAlias, (object) this.LongTextTableAlias, (object) this.LongTextPredicates);
      }
      else
        stringBuilder.AppendFormat("FROM {0} {1}{2}", (object) this.Table, (object) this.TableAlias, !(this.Table == "dbo.WorkItemLongTexts") || !this.ForceFullTextIndexHint ? (object) string.Empty : (object) " WITH (FORCESEEK(IX_WorkItemLongTexts_PartitionTextID(PartitionTextID)))");
      stringBuilder.AppendLine();
      foreach (string join in this.m_joins)
        stringBuilder.AppendLine(join);
      if (this.m_where.Length > 0)
      {
        stringBuilder.Append("WHERE ");
        stringBuilder.AppendLine(this.m_where.ToString());
      }
      if (this.OrderBy.Any<string>())
      {
        stringBuilder.Append("ORDER BY ");
        stringBuilder.AppendLine(string.Join(", ", (IEnumerable<string>) this.OrderBy));
      }
      if (this.Options.Any<string>())
      {
        stringBuilder.Append("OPTION (");
        stringBuilder.Append(string.Join(", ", (IEnumerable<string>) this.Options));
        stringBuilder.AppendLine(")");
      }
      return stringBuilder.ToString();
    }
  }
}
