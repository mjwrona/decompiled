// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Common.Contracts.TaskInputValidation
// Assembly: Microsoft.TeamFoundation.DistributedTask.Common.Contracts, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F0CB8220-D93B-49F7-B603-A5F8DA1FAAC3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.DistributedTask.Common.Contracts.dll

using Microsoft.VisualStudio.Services.WebApi;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.Common.Contracts
{
  [DataContract]
  public class TaskInputValidation : BaseSecuredObject
  {
    public TaskInputValidation()
    {
    }

    private TaskInputValidation(TaskInputValidation toClone, ISecuredObject securedObject)
      : base(securedObject)
    {
      if (toClone == null)
        return;
      this.Expression = toClone.Expression;
      this.Message = toClone.Message;
    }

    [DataMember(EmitDefaultValue = false)]
    public string Expression { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string Message { get; set; }

    public override int GetHashCode() => this.Expression.GetHashCode() ^ this.Message.GetHashCode();

    public TaskInputValidation Clone() => this.Clone((ISecuredObject) null);

    public TaskInputValidation Clone(ISecuredObject securedObject) => new TaskInputValidation(this, securedObject);
  }
}
