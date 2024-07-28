// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.IInternalServicingContext
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using System;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal interface IInternalServicingContext : IServicingContext, ICancelable, IDisposable
  {
    void StartOperation(ServicingOperation servicingOperation);

    void FinishOperation();

    void StartStepGroup(string name, ServicingStepGroup stepGroup);

    void FinishStepGroup();

    void StartStep(string name);

    void SkipStep(string operationName, string groupName, string stepName);

    ServicingStepState FinishStep(Exception executionException);

    new IVssRequestContext DeploymentRequestContext { get; set; }
  }
}
