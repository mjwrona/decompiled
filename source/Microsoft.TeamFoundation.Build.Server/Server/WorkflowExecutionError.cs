// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.WorkflowExecutionError
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using Microsoft.VisualStudio.Services.Common;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Build.Server
{
  [GenerateInterface(false)]
  [DataContract(Namespace = "http://www.tempuri.org/Microsoft/TeamFoundation/Build/07")]
  internal sealed class WorkflowExecutionError
  {
    [DataMember(Name = "AssemblyName", EmitDefaultValue = true)]
    private string m_assemblyName;
    [DataMember(Name = "ErrorCode", EmitDefaultValue = true)]
    private string m_errorCode;
    [DataMember(Name = "ErrorMessage", EmitDefaultValue = true)]
    private string m_errorMessage;

    private WorkflowExecutionError()
    {
    }

    public override string ToString() => "WorkflowExecutionError instance " + this.GetHashCode().ToString() + "\r\n  AssemblyName: " + this.m_assemblyName + "\r\n  ErrorCode: " + this.m_errorCode + "\r\n  ErrorMessage: " + this.m_errorMessage + "\r\n";
  }
}
