// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Common.TestCaseParameterDataInfo
// Assembly: Microsoft.TeamFoundation.TestManagement.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1401105B-6771-499A-8DF3-F3CBE1BB3AE4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.TestManagement.Common.dll

using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Text;

namespace Microsoft.TeamFoundation.TestManagement.Common
{
  [DataContract]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class TestCaseParameterDataInfo
  {
    public TestCaseParameterDataInfo()
    {
    }

    public TestCaseParameterDataInfo(
      List<ParameterDefinition> parameterMap,
      List<TestCaseParameterDataRow> parameterDataRows = null,
      int[] sharedParameterDataSetIds = null,
      SharedParametersRowsMappingType rowMappingType = SharedParametersRowsMappingType.MapAllRows)
    {
      this.ParameterMap = parameterMap;
      this.ParameterDataRows = parameterDataRows;
      this.SharedParameterDataSetIds = sharedParameterDataSetIds;
      this.RowMappingType = rowMappingType;
    }

    [DataMember(Name = "parameterMap", IsRequired = false)]
    [JsonProperty("parameterMap", TypeNameHandling = TypeNameHandling.None)]
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public List<ParameterDefinition> ParameterMap { get; set; }

    [DataMember(Name = "parameterDataRows", IsRequired = false)]
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public List<TestCaseParameterDataRow> ParameterDataRows { get; set; }

    [DataMember(Name = "sharedParameterDataSetIds")]
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public int[] SharedParameterDataSetIds { get; set; }

    [DataMember(Name = "rowMappingType", IsRequired = false)]
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public SharedParametersRowsMappingType RowMappingType { get; set; }

    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder();
      if (this.ParameterMap != null)
      {
        foreach (ParameterDefinition parameter in this.ParameterMap)
          stringBuilder.Append(parameter.ToString());
      }
      if (this.SharedParameterDataSetIds != null)
      {
        for (int index = 0; index < this.SharedParameterDataSetIds.Length; ++index)
          stringBuilder.Append(string.Format("SharedParameterDataSetId: {0}", (object) this.SharedParameterDataSetIds[index]));
      }
      stringBuilder.Append(string.Format("RowMapingType : {0}", (object) this.RowMappingType));
      return stringBuilder.ToString();
    }
  }
}
