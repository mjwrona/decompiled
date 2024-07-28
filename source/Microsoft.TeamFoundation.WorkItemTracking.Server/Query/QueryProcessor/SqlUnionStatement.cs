// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Query.QueryProcessor.SqlUnionStatement
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
  internal class SqlUnionStatement : SqlStatement
  {
    private List<SqlStatement> m_innerStatements;

    public SqlUnionStatement(IEnumerable<SqlStatement> statements, string tableAlias)
    {
      this.TableAlias = tableAlias;
      this.m_innerStatements = new List<SqlStatement>(statements);
    }

    public override string GetSql()
    {
      StringBuilder stringBuilder1 = new StringBuilder(512);
      int? top;
      if (!this.m_joins.Any<string>() && this.m_where.Length <= 0 && !this.OrderBy.Any<string>())
      {
        top = this.Top;
        if (!top.HasValue)
          goto label_8;
      }
      stringBuilder1.Append("SELECT ");
      top = this.Top;
      if (top.HasValue)
      {
        StringBuilder stringBuilder2 = stringBuilder1;
        CultureInfo invariantCulture = CultureInfo.InvariantCulture;
        top = this.Top;
        // ISSUE: variable of a boxed type
        __Boxed<int> local = (ValueType) top.Value;
        stringBuilder2.AppendFormat((IFormatProvider) invariantCulture, "TOP {0} ", (object) local);
      }
      if (this.Select.Any<string>())
        stringBuilder1.AppendLine(string.Join(", ", (IEnumerable<string>) this.Select));
      else
        stringBuilder1.AppendLine("*");
      stringBuilder1.AppendLine("FROM (");
label_8:
      stringBuilder1.AppendLine(string.Join(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "UNION{0}", (object) Environment.NewLine), this.m_innerStatements.Select<SqlStatement, string>((Func<SqlStatement, string>) (s => s.GetSql()))));
      if (this.m_joins.Any<string>() || this.m_where.Length > 0 || this.OrderBy.Any<string>())
      {
        stringBuilder1.AppendFormat(") {0}", (object) this.TableAlias);
        stringBuilder1.AppendLine();
      }
      foreach (string join in this.m_joins)
        stringBuilder1.AppendLine(join);
      if (this.m_where.Length > 0)
      {
        stringBuilder1.Append("WHERE ");
        stringBuilder1.AppendLine(this.m_where.ToString());
      }
      if (this.OrderBy.Any<string>())
      {
        stringBuilder1.Append("ORDER BY ");
        stringBuilder1.AppendLine(string.Join(", ", (IEnumerable<string>) this.OrderBy));
      }
      if (this.Options.Any<string>())
      {
        stringBuilder1.Append("OPTION (");
        stringBuilder1.Append(string.Join(", ", (IEnumerable<string>) this.Options));
        stringBuilder1.AppendLine(")");
      }
      return stringBuilder1.ToString();
    }
  }
}
