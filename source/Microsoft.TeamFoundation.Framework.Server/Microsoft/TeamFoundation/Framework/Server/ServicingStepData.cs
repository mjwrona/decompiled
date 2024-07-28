// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ServicingStepData
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System.Diagnostics;

namespace Microsoft.TeamFoundation.Framework.Server
{
  [DebuggerDisplay("StepName: {StepName}, GroupName: {GroupName}, StepType: {StepType}, StepPerformer: {StepPerformer}")]
  public struct ServicingStepData
  {
    public string StepName { get; set; }

    public string GroupName { get; set; }

    public int OrderNumber { get; set; }

    public string StepPerformer { get; set; }

    public string StepType { get; set; }

    public ServicingStep.StepOptions Options { get; set; }

    public string StepData { get; set; }
  }
}
