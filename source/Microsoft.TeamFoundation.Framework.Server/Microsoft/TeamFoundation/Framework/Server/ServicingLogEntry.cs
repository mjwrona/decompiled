// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ServicingLogEntry
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Framework.Server
{
  [DataContract(Namespace = "", Name = "ServicingLogEntry")]
  public class ServicingLogEntry : IExtensibleDataObject
  {
    public ServicingLogEntry(ServicingLogEntryType type, string message)
    {
      this.Type = type;
      this.Message = message;
    }

    public static ServicingLogEntryType FromEntryKind(ServicingStepLogEntryKind kind)
    {
      ServicingLogEntryType servicingLogEntryType = ServicingLogEntryType.None;
      switch (kind)
      {
        case ServicingStepLogEntryKind.Informational:
          servicingLogEntryType = ServicingLogEntryType.Info;
          break;
        case ServicingStepLogEntryKind.Warning:
          servicingLogEntryType = ServicingLogEntryType.Warning;
          break;
        case ServicingStepLogEntryKind.Error:
          servicingLogEntryType = ServicingLogEntryType.Error;
          break;
        case ServicingStepLogEntryKind.StepDuration:
          servicingLogEntryType = ServicingLogEntryType.StepDuration;
          break;
        case ServicingStepLogEntryKind.GroupDuration:
          servicingLogEntryType = ServicingLogEntryType.GroupDuration;
          break;
        case ServicingStepLogEntryKind.OperationDuration:
          servicingLogEntryType = ServicingLogEntryType.OperationDuration;
          break;
        case ServicingStepLogEntryKind.SleepDuration:
          servicingLogEntryType = ServicingLogEntryType.SleepDuration;
          break;
      }
      return servicingLogEntryType;
    }

    [DataMember(Name = "type", Order = 0)]
    public ServicingLogEntryType Type { get; set; }

    [DataMember(Name = "message", Order = 1)]
    public string Message { get; set; }

    public ExtensionDataObject ExtensionData { get; set; }
  }
}
