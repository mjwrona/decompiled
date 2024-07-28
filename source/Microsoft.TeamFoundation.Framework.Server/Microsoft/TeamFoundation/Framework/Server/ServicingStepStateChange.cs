// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ServicingStepStateChange
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Common.Internal;
using Microsoft.TeamFoundation.Framework.Common;
using System;
using System.Globalization;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class ServicingStepStateChange : ServicingStepDetail
  {
    [XmlIgnore]
    public ServicingStepState StepState { get; set; }

    [XmlAttribute("state")]
    [ClientProperty(ClientVisibility.Private, ClientVisibility.Private)]
    public int StepStateDataTransfer
    {
      get => (int) this.StepState;
      set => this.StepState = (ServicingStepState) value;
    }

    [XmlIgnore]
    public string Message
    {
      get
      {
        switch (this.StepState)
        {
          case ServicingStepState.NotExecuted:
            return TFCommonResources.ServicingStepNotExecuted((object) this.ServicingStepId, (object) this.ServicingOperation, (object) this.ServicingStepGroupId);
          case ServicingStepState.Validating:
            return TFCommonResources.ValidatingServicingStep((object) this.ServicingStepId, (object) this.ServicingOperation, (object) this.ServicingStepGroupId);
          case ServicingStepState.Validated:
            return TFCommonResources.ServicingStepValidated((object) this.ServicingStepId, (object) this.ServicingOperation, (object) this.ServicingStepGroupId);
          case ServicingStepState.ValidatedWithWarnings:
            return TFCommonResources.ServicingStepValidatedWithWarnings((object) this.ServicingStepId, (object) this.ServicingOperation, (object) this.ServicingStepGroupId);
          case ServicingStepState.Executing:
            return TFCommonResources.ExecutingServicingStep((object) this.ServicingStepId, (object) this.ServicingOperation, (object) this.ServicingStepGroupId);
          case ServicingStepState.Failed:
            return TFCommonResources.ServicingStepFailed((object) this.ServicingStepId, (object) this.ServicingOperation, (object) this.ServicingStepGroupId);
          case ServicingStepState.Skipped:
            return TFCommonResources.ServicingStepSkipped((object) this.ServicingStepId, (object) this.ServicingOperation, (object) this.ServicingStepGroupId);
          case ServicingStepState.Passed:
            return TFCommonResources.ServicingStepPassed((object) this.ServicingStepId, (object) this.ServicingOperation, (object) this.ServicingStepGroupId);
          case ServicingStepState.PassedWithWarnings:
            return TFCommonResources.ServicingStepPassedWithWarnings((object) this.ServicingStepId, (object) this.ServicingOperation, (object) this.ServicingStepGroupId);
          case ServicingStepState.PassedWithSkipChildren:
            return TFCommonResources.ServicingStepPassedWithSkipChildren((object) this.ServicingStepId, (object) this.ServicingOperation, (object) this.ServicingStepGroupId);
          default:
            return TFCommonResources.ServicingStepStateChange((object) this.ServicingStepId, (object) this.StepState, (object) this.ServicingOperation, (object) this.ServicingStepGroupId);
        }
      }
    }

    public override string ToLogEntryLine() => string.Format((IFormatProvider) CultureInfo.CurrentUICulture, "[{0:u}] {1}", (object) this.DetailTime.ToUniversalTime(), (object) this.Message);

    public override string ToString() => this.ToLogEntryLine();
  }
}
