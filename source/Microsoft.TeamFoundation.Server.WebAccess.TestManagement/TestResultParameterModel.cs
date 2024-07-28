// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.TestManagement.TestResultParameterModel
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.TestManagement, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2E4165D5-898A-42D9-B816-9FABF135E4DA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.TestManagement.dll

using Microsoft.TeamFoundation.TestManagement.Server;
using System.Runtime.Serialization;
using System.Text;

namespace Microsoft.TeamFoundation.Server.WebAccess.TestManagement
{
  [DataContract]
  public class TestResultParameterModel
  {
    private static readonly Encoding s_encoding = (Encoding) new UnicodeEncoding(false, false, false);

    public TestResultParameterModel()
    {
    }

    public TestResultParameterModel(TestResultParameter parameter)
    {
      this.TestRunId = parameter.TestRunId;
      this.TestResultId = parameter.TestResultId;
      this.IterationId = parameter.IterationId;
      this.ActionPath = parameter.ActionPath;
      this.ParameterName = parameter.ParameterName;
      this.DataType = parameter.DataType;
      this.Expected = TestResultParameterModel.BytesToValue(parameter.Expected);
      this.Actual = TestResultParameterModel.BytesToValue(parameter.Actual);
    }

    public TestResultParameterModel(
      int testRunId,
      int testResultId,
      Microsoft.TeamFoundation.TestManagement.WebApi.TestResultParameterModel parameter)
    {
      this.TestRunId = testRunId;
      this.TestResultId = testResultId;
      this.IterationId = parameter.IterationId;
      this.ActionPath = parameter.ActionPath;
      this.ParameterName = parameter.ParameterName;
      this.DataType = parameter.DataType;
      this.Expected = parameter.Value;
    }

    [DataMember(Name = "testRunId")]
    public int TestRunId { get; set; }

    [DataMember(Name = "testResultId")]
    public int TestResultId { get; set; }

    [DataMember(Name = "iterationId")]
    public int IterationId { get; set; }

    [DataMember(Name = "actionPath")]
    public string ActionPath { get; set; }

    [DataMember(Name = "parameterName")]
    public string ParameterName { get; set; }

    [DataMember(Name = "dataType")]
    public byte DataType { get; set; }

    [DataMember(Name = "expected")]
    public string Expected { get; set; }

    [DataMember(Name = "actual")]
    public string Actual { get; set; }

    internal TestResultParameter CreateFromModel() => new TestResultParameter()
    {
      TestRunId = this.TestRunId,
      TestResultId = this.TestResultId,
      IterationId = this.IterationId,
      ActionPath = this.ActionPath,
      ParameterName = this.ParameterName,
      DataType = this.DataType,
      Expected = TestResultParameterModel.ValueToBytes(this.Expected),
      Actual = TestResultParameterModel.ValueToBytes(this.Actual)
    };

    private static byte[] ValueToBytes(string value) => value != null ? TestResultParameterModel.s_encoding.GetBytes(value) : (byte[]) null;

    private static string BytesToValue(byte[] bytes) => bytes == null ? (string) null : TestResultParameterModel.s_encoding.GetString(bytes);
  }
}
