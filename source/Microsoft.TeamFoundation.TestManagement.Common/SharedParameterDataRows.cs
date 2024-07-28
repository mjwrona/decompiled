// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Common.SharedParameterDataRows
// Assembly: Microsoft.TeamFoundation.TestManagement.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1401105B-6771-499A-8DF3-F3CBE1BB3AE4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.TestManagement.Common.dll

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.TestManagement.Common
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class SharedParameterDataRows : ISharedParameterDataRows
  {
    [XmlAttribute("lastId")]
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public int LastId { get; set; }

    [XmlIgnore]
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public List<ISharedParameterDataRow> Rows => this.DataRows.Select<SharedParameterDataRow, ISharedParameterDataRow>((Func<SharedParameterDataRow, ISharedParameterDataRow>) (row => (ISharedParameterDataRow) row)).ToList<ISharedParameterDataRow>();

    [XmlElement("dataRow")]
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public List<SharedParameterDataRow> DataRows { get; set; }

    public SharedParameterDataRows()
    {
      this.DataRows = new List<SharedParameterDataRow>();
      this.LastId = 0;
    }

    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append("SharedParameterDataRows: \n");
      stringBuilder.Append(string.Format("Last id : {0}", (object) this.LastId));
      if (this.DataRows != null)
      {
        foreach (ISharedParameterDataRow dataRow in this.DataRows)
          stringBuilder.Append(dataRow.ToString() + "\n");
      }
      return stringBuilder.ToString();
    }
  }
}
