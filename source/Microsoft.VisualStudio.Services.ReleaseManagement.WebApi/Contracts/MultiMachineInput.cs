// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.MultiMachineInput
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AE7F604E-30D7-44A7-BE7B-AB7FB5A67B31
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.dll

using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts
{
  [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Multi", Justification = "Well known word in VSTS")]
  [DataContract]
  public class MultiMachineInput : ParallelExecutionInputBase
  {
    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Reviewed")]
    public MultiMachineInput(int maxNumberOfAgents = 1, bool continueOnError = true)
      : base(ParallelExecutionTypes.MultiMachine, maxNumberOfAgents, continueOnError)
    {
    }

    [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "As per design")]
    public override bool Equals(ExecutionInput other) => other is MultiMachineInput && base.Equals(other);

    public override object Clone() => (object) new MultiMachineInput(this.MaxNumberOfAgents, this.ContinueOnError);
  }
}
