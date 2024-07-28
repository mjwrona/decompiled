// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.WebApi.TestResultParameterModel
// Assembly: Microsoft.TeamFoundation.TestManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 10F0A812-3ECA-42B4-865D-429941F99EBE
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.TestManagement.WebApi.dll

using System.Runtime.Serialization;
using System.Text;

namespace Microsoft.TeamFoundation.TestManagement.WebApi
{
  [DataContract]
  public class TestResultParameterModel : TestManagementBaseSecuredObject
  {
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string Url;
    private static Encoding s_encoding = (Encoding) new UnicodeEncoding(false, false, false);

    public TestResultParameterModel()
    {
    }

    public TestResultParameterModel(
      int iterationId,
      string actionPath,
      string parameterName,
      string expected)
    {
      this.IterationId = iterationId;
      this.ActionPath = actionPath;
      this.ParameterName = parameterName;
      this.Value = expected;
    }

    public TestResultParameterModel(
      int iterationId,
      string actionPath,
      string parameterName,
      byte dataType,
      byte[] expected)
    {
      this.IterationId = iterationId;
      this.ActionPath = actionPath;
      this.ParameterName = parameterName;
      this.DataType = dataType;
      this.Value = TestResultParameterModel.BytesToValue(expected);
    }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public int IterationId { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string ActionPath { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string ParameterName { get; set; }

    public byte DataType { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string Value { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string StepIdentifier { get; set; }

    public static byte[] ValueToBytes(string value) => value != null ? TestResultParameterModel.s_encoding.GetBytes(value) : (byte[]) null;

    public static string BytesToValue(byte[] bytes) => bytes == null ? (string) null : TestResultParameterModel.s_encoding.GetString(bytes, 0, bytes.Length);
  }
}
