// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Common.SharedParameterData
// Assembly: Microsoft.TeamFoundation.TestManagement.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1401105B-6771-499A-8DF3-F3CBE1BB3AE4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.TestManagement.Common.dll

using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.TestManagement.Common
{
  [XmlRoot(ElementName = "parameterSet", IsNullable = false)]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class SharedParameterData : ISharedParameterData
  {
    [XmlArray("paramNames")]
    [XmlArrayItem("param")]
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public List<string> ParameterNames { get; set; }

    [XmlIgnore]
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public ISharedParameterDataRows ParameterValues => (ISharedParameterDataRows) this.ParameterData;

    [XmlElement(ElementName = "paramData")]
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public SharedParameterDataRows ParameterData { get; set; }

    public SharedParameterData()
    {
      this.ParameterNames = new List<string>();
      this.ParameterData = new SharedParameterDataRows();
    }

    public SharedParameterData(params string[] names)
    {
      this.ParameterNames = new List<string>((IEnumerable<string>) names);
      this.ParameterData = new SharedParameterDataRows();
    }

    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append("SharedParameterData : \n");
      stringBuilder.Append("ParameterNames:\n");
      if (this.ParameterNames != null)
      {
        foreach (string parameterName in this.ParameterNames)
          stringBuilder.Append(string.Format("ParamName : {0}", (object) parameterName));
      }
      if (this.ParameterData != null)
        stringBuilder.Append(this.ParameterData.ToString());
      return stringBuilder.ToString();
    }
  }
}
