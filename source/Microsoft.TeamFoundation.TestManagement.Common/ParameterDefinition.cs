// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Common.ParameterDefinition
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
  [JsonConverter(typeof (SharedParameterJsonConverter))]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class ParameterDefinition
  {
    public const string c_localParamNameString = "localParamName";

    public ParameterDefinition()
    {
    }

    protected ParameterDefinition(string localParamName, TestCaseParameterType parameterType)
    {
      this.LocalParamName = localParamName;
      this.ParameterType = parameterType;
    }

    [JsonProperty(PropertyName = "localParamName")]
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public string LocalParamName { get; set; }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public TestCaseParameterType ParameterType { get; set; }

    public override string ToString() => string.Format("LocalParamName : {0}, ParameterType : {1}", (object) this.LocalParamName, (object) this.ParameterType);
  }
}
