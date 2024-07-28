// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.HostMigration.HostMigrationStepDetail
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Globalization;
using System.Xml.Serialization;

namespace Microsoft.VisualStudio.Services.Cloud.HostMigration
{
  public class HostMigrationStepDetail : ServicingStepDetail
  {
    [XmlIgnore]
    public ServicingStepLogEntryKind EntryKind { get; set; }

    [XmlAttribute("ek")]
    [ClientProperty(ClientVisibility.Private, ClientVisibility.Private)]
    public int EntryKindDataTransfer
    {
      get => (int) this.EntryKind;
      set => this.EntryKind = (ServicingStepLogEntryKind) value;
    }

    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private)]
    public string Message { get; set; }

    public override string ToLogEntryLine()
    {
      string str;
      switch (this.EntryKind)
      {
        case ServicingStepLogEntryKind.Informational:
          str = FrameworkResources.ServicingStepLogEntryKindInformational();
          break;
        case ServicingStepLogEntryKind.Warning:
          str = FrameworkResources.ServicingStepLogEntryKindWarning();
          break;
        case ServicingStepLogEntryKind.Error:
          str = FrameworkResources.ServicingStepLogEntryKindError();
          break;
        case ServicingStepLogEntryKind.StepDuration:
          str = FrameworkResources.ServicingStepLogEntryKindStepDuration();
          break;
        case ServicingStepLogEntryKind.GroupDuration:
          str = FrameworkResources.ServicingStepLogEntryKindGroupDuration();
          break;
        case ServicingStepLogEntryKind.OperationDuration:
          str = FrameworkResources.ServicingStepLogEntryKindOperationDuration();
          break;
        case ServicingStepLogEntryKind.SleepDuration:
          str = FrameworkResources.ServicingStepLogEntryKindSleepDuration();
          break;
        default:
          str = FrameworkResources.ServicingStepLogEntryKindUnknown();
          break;
      }
      return string.Format((IFormatProvider) CultureInfo.CurrentUICulture, "[{0:u}][{1}] {2}", (object) this.DetailTime.ToUniversalTime(), (object) str, (object) this.Message);
    }

    public override string ToString() => this.ToLogEntryLine();
  }
}
