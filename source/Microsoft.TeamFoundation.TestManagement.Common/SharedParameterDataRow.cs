// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Common.SharedParameterDataRow
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
  public class SharedParameterDataRow : ISharedParameterDataRow
  {
    [XmlAttribute("id")]
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public int Id { get; set; }

    [XmlIgnore]
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public List<IKeyValuePair> ParameterValues => this.KeyValuePairs.Select<KeyValuePair, IKeyValuePair>((Func<KeyValuePair, IKeyValuePair>) (kvp => (IKeyValuePair) kvp)).ToList<IKeyValuePair>();

    [XmlElement("kvp")]
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public List<KeyValuePair> KeyValuePairs { get; set; }

    public SharedParameterDataRow(int id)
    {
      this.Id = id;
      this.KeyValuePairs = new List<KeyValuePair>();
    }

    public SharedParameterDataRow() => this.KeyValuePairs = new List<KeyValuePair>();

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public string this[string key]
    {
      get
      {
        IKeyValuePair keyValuePair = this.ParameterValues.Find((Predicate<IKeyValuePair>) (paramValue => paramValue.Key == key));
        return keyValuePair != null ? keyValuePair.Value : "";
      }
    }

    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append("SharedParameterDataRow : \n");
      if (this.KeyValuePairs != null)
      {
        foreach (IKeyValuePair keyValuePair in this.KeyValuePairs)
          stringBuilder.Append(string.Format("Key : {0}, Value ={1}", (object) keyValuePair.Key, (object) keyValuePair.Value));
      }
      return stringBuilder.ToString();
    }
  }
}
