// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.VssJobResult
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System.ComponentModel;

namespace Microsoft.TeamFoundation.Framework.Server
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public struct VssJobResult
  {
    public VssJobResult(TeamFoundationJobExecutionResult result)
      : this(result, (string) null)
    {
    }

    public VssJobResult(TeamFoundationJobExecutionResult result, string message)
    {
      this.Result = result;
      this.Message = message;
    }

    public string Message { get; set; }

    public TeamFoundationJobExecutionResult Result { get; set; }

    public static implicit operator VssJobResult(TeamFoundationJobExecutionResult result) => new VssJobResult(result);
  }
}
