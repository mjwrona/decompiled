// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Common.SharedParameterDefinition
// Assembly: Microsoft.TeamFoundation.TestManagement.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1401105B-6771-499A-8DF3-F3CBE1BB3AE4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.TestManagement.Common.dll

using Newtonsoft.Json;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.TestManagement.Common
{
  [DataContract]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class SharedParameterDefinition : ParameterDefinition
  {
    public const string c_sharedParamNameString = "sharedParameterName";
    public const string c_sharedParameterDataSetIdString = "sharedParameterDataSetId";

    public SharedParameterDefinition()
    {
    }

    public SharedParameterDefinition(
      string localParamName,
      string sharedParameterName,
      int sharedParameterDataSetId,
      TestCaseParameterType parameterType = TestCaseParameterType.String)
      : base(localParamName, parameterType)
    {
      this.SharedParameterName = sharedParameterName;
      this.SharedParameterDataSetId = sharedParameterDataSetId;
    }

    [JsonProperty(PropertyName = "sharedParameterName")]
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public string SharedParameterName { get; set; }

    [JsonProperty(PropertyName = "sharedParameterDataSetId")]
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public int SharedParameterDataSetId { get; set; }

    public override string ToString() => base.ToString() + string.Format("SharedParameterName : {0}, SharedParameterDataSetId : {1}", (object) this.SharedParameterName, (object) this.SharedParameterDataSetId);
  }
}
