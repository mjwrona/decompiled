// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Client.ServicingStepStateChange
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.TeamFoundation.Common.Internal;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.VisualStudio.Services.Common.Internal;
using System;
using System.ComponentModel;
using System.Globalization;
using System.Text;
using System.Xml;

namespace Microsoft.TeamFoundation.Framework.Client
{
  public sealed class ServicingStepStateChange : ServicingStepDetail
  {
    private int m_stepStateDataTransfer;

    public ServicingStepState StepState => (ServicingStepState) this.m_stepStateDataTransfer;

    public override string ToLogEntryLine()
    {
      string str;
      switch (this.StepState)
      {
        case ServicingStepState.NotExecuted:
          str = TFCommonResources.ServicingStepNotExecuted((object) this.ServicingStepId, (object) this.ServicingOperation, (object) this.ServicingStepGroupId);
          break;
        case ServicingStepState.Validating:
          str = TFCommonResources.ValidatingServicingStep((object) this.ServicingStepId, (object) this.ServicingOperation, (object) this.ServicingStepGroupId);
          break;
        case ServicingStepState.Validated:
          str = TFCommonResources.ServicingStepValidated((object) this.ServicingStepId, (object) this.ServicingOperation, (object) this.ServicingStepGroupId);
          break;
        case ServicingStepState.ValidatedWithWarnings:
          str = TFCommonResources.ServicingStepValidatedWithWarnings((object) this.ServicingStepId, (object) this.ServicingOperation, (object) this.ServicingStepGroupId);
          break;
        case ServicingStepState.Executing:
          str = TFCommonResources.ExecutingServicingStep((object) this.ServicingStepId, (object) this.ServicingOperation, (object) this.ServicingStepGroupId);
          break;
        case ServicingStepState.Failed:
          str = TFCommonResources.ServicingStepFailed((object) this.ServicingStepId, (object) this.ServicingOperation, (object) this.ServicingStepGroupId);
          break;
        case ServicingStepState.Skipped:
          str = TFCommonResources.ServicingStepSkipped((object) this.ServicingStepId, (object) this.ServicingOperation, (object) this.ServicingStepGroupId);
          break;
        case ServicingStepState.Passed:
          str = TFCommonResources.ServicingStepPassed((object) this.ServicingStepId, (object) this.ServicingOperation, (object) this.ServicingStepGroupId);
          break;
        case ServicingStepState.PassedWithWarnings:
          str = TFCommonResources.ServicingStepPassedWithWarnings((object) this.ServicingStepId, (object) this.ServicingOperation, (object) this.ServicingStepGroupId);
          break;
        default:
          str = TFCommonResources.ServicingStepStateChange((object) this.ServicingStepId, (object) this.StepState, (object) this.ServicingOperation, (object) this.ServicingStepGroupId);
          break;
      }
      return string.Format((IFormatProvider) CultureInfo.CurrentUICulture, "[{0:u}] {1}", (object) this.DetailTime.ToUniversalTime(), (object) str);
    }

    private ServicingStepStateChange()
    {
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static ServicingStepStateChange FromXml(
      IServiceProvider serviceProvider,
      XmlReader reader)
    {
      ServicingStepStateChange servicingStepStateChange = new ServicingStepStateChange();
      bool isEmptyElement = reader.IsEmptyElement;
      if (reader.HasAttributes)
      {
        while (reader.MoveToNextAttribute())
        {
          switch (reader.Name)
          {
            case "did":
              servicingStepStateChange.m_detailId = XmlUtility.Int64FromXmlAttribute(reader);
              continue;
            case "dtime":
              servicingStepStateChange.m_detailTime = XmlUtility.DateTimeFromXmlAttribute(reader);
              continue;
            case "sop":
              servicingStepStateChange.m_servicingOperation = XmlUtility.StringFromXmlAttribute(reader);
              continue;
            case "gid":
              servicingStepStateChange.m_servicingStepGroupId = XmlUtility.StringFromXmlAttribute(reader);
              continue;
            case "sid":
              servicingStepStateChange.m_servicingStepId = XmlUtility.StringFromXmlAttribute(reader);
              continue;
            case "state":
              servicingStepStateChange.m_stepStateDataTransfer = XmlUtility.Int32FromXmlAttribute(reader);
              continue;
            default:
              continue;
          }
        }
      }
      reader.Read();
      if (!isEmptyElement)
      {
        while (reader.NodeType == XmlNodeType.Element)
        {
          string name = reader.Name;
          reader.ReadOuterXml();
        }
        reader.ReadEndElement();
      }
      return servicingStepStateChange;
    }

    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.AppendLine("ServicingStepStateChange instance " + this.GetHashCode().ToString());
      stringBuilder.AppendLine("  DetailId: " + this.m_detailId.ToString());
      stringBuilder.AppendLine("  DetailTime: " + this.m_detailTime.ToString());
      stringBuilder.AppendLine("  ServicingOperation: " + this.m_servicingOperation);
      stringBuilder.AppendLine("  ServicingStepGroupId: " + this.m_servicingStepGroupId);
      stringBuilder.AppendLine("  ServicingStepId: " + this.m_servicingStepId);
      stringBuilder.AppendLine("  StepStateDataTransfer: " + this.m_stepStateDataTransfer.ToString());
      return stringBuilder.ToString();
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public override void ToXml(XmlWriter writer, string element)
    {
      writer.WriteStartElement(element);
      if (element != nameof (ServicingStepStateChange))
        writer.WriteAttributeString("xsi", "type", "http://www.w3.org/2001/XMLSchema-instance", nameof (ServicingStepStateChange));
      if (this.m_detailId != 0L)
        XmlUtility.ToXmlAttribute(writer, "did", this.m_detailId);
      if (this.m_detailTime != DateTime.MinValue)
        XmlUtility.ToXmlAttribute(writer, "dtime", this.m_detailTime);
      if (this.m_servicingOperation != null)
        XmlUtility.ToXmlAttribute(writer, "sop", this.m_servicingOperation);
      if (this.m_servicingStepGroupId != null)
        XmlUtility.ToXmlAttribute(writer, "gid", this.m_servicingStepGroupId);
      if (this.m_servicingStepId != null)
        XmlUtility.ToXmlAttribute(writer, "sid", this.m_servicingStepId);
      if (this.m_stepStateDataTransfer != 0)
        XmlUtility.ToXmlAttribute(writer, "state", this.m_stepStateDataTransfer);
      writer.WriteEndElement();
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void ToXml(XmlWriter writer, string element, ServicingStepStateChange obj) => obj.ToXml(writer, element);
  }
}
