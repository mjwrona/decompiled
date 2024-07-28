// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Query.Core.Metrics.TextTable
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Azure.Cosmos.Query.Core.Metrics
{
  internal sealed class TextTable
  {
    private const char CellLeftTop = '┌';
    private const char CellRightTop = '┐';
    private const char CellLeftBottom = '└';
    private const char CellRightBottom = '┘';
    private const char CellHorizontalJointTop = '┬';
    private const char CellHorizontalJointBottom = '┴';
    private const char CellVerticalJointLeft = '├';
    private const char CellTJoint = '┼';
    private const char CellVerticalJointRight = '┤';
    private const char CellHorizontalLine = '─';
    private const char CellVerticalLine = '│';
    private readonly List<TextTable.Column> columns;
    private readonly string rowFormatString;

    public TextTable(params TextTable.Column[] columns)
    {
      this.columns = new List<TextTable.Column>((IEnumerable<TextTable.Column>) columns);
      this.Header = string.Format(TextTable.BuildLineFormatString("{{{0},-{1}}}", (IEnumerable<TextTable.Column>) columns), (object[]) ((IEnumerable<TextTable.Column>) columns).Select<TextTable.Column, string>((Func<TextTable.Column, string>) (textTableColumn => textTableColumn.ColumnName)).ToArray<string>());
      this.TopLine = TextTable.BuildLine('┌', '┐', '┬', (IEnumerable<TextTable.Column>) columns);
      this.MiddleLine = TextTable.BuildLine('├', '┤', '┼', (IEnumerable<TextTable.Column>) columns);
      this.BottomLine = TextTable.BuildLine('└', '┘', '┴', (IEnumerable<TextTable.Column>) columns);
      this.rowFormatString = TextTable.BuildLineFormatString("{{{0},{1}}}", (IEnumerable<TextTable.Column>) columns);
    }

    public string Header { get; }

    public string TopLine { get; }

    public string MiddleLine { get; }

    public string BottomLine { get; }

    public string GetRow(params object[] cells)
    {
      if (cells.Length != this.columns.Count)
        throw new ArgumentException("Cells in a row needs to have exactly 1 element per column");
      return string.Format(this.rowFormatString, cells);
    }

    private static string BuildLine(
      char firstChar,
      char lastChar,
      char seperator,
      IEnumerable<TextTable.Column> columns)
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append(firstChar);
      foreach (TextTable.Column column in columns.Take<TextTable.Column>(columns.Count<TextTable.Column>() - 1))
      {
        stringBuilder.Append('─', column.ColumnWidth);
        stringBuilder.Append(seperator);
      }
      stringBuilder.Append('─', columns.Last<TextTable.Column>().ColumnWidth);
      stringBuilder.Append(lastChar);
      return stringBuilder.ToString();
    }

    private static string BuildLineFormatString(
      string cellFormatString,
      IEnumerable<TextTable.Column> columns)
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append('│');
      int num = 0;
      foreach (TextTable.Column column in columns)
      {
        stringBuilder.Append(string.Format(cellFormatString, (object) num++, (object) column.ColumnWidth));
        stringBuilder.Append('│');
      }
      return stringBuilder.ToString();
    }

    internal readonly struct Column
    {
      public readonly string ColumnName;
      public readonly int ColumnWidth;

      public Column(string columnName, int columnWidth)
      {
        this.ColumnName = columnName;
        this.ColumnWidth = columnWidth;
      }
    }
  }
}
