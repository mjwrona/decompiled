// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ServicingStep
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.Framework.Server
{
  [DebuggerDisplay("Name: {Name}, StepType: {StepType}, Performer: {StepPerformer}")]
  public class ServicingStep
  {
    private ServicingStep()
    {
    }

    public ServicingStep(
      string groupName,
      string name,
      string performer,
      string stepType,
      string stepData)
    {
      this.GroupName = groupName;
      this.Name = name;
      this.StepPerformer = performer;
      this.StepType = stepType;
      if (string.IsNullOrEmpty(stepData))
        return;
      this.StepData = TeamFoundationSerializationUtility.SerializeToXml(stepData);
    }

    [XmlAttribute("name")]
    public string Name { get; set; }

    [XmlAttribute("stepPerformer")]
    public string StepPerformer { get; set; }

    [XmlAttribute("stepType")]
    public string StepType { get; set; }

    [XmlAttribute("executeAlways")]
    public bool ExecuteAlways
    {
      get => (this.Options & ServicingStep.StepOptions.ExecuteAllways) != 0;
      set
      {
        if (value)
          this.Options |= ServicingStep.StepOptions.ExecuteAllways;
        else
          this.Options &= ~ServicingStep.StepOptions.ExecuteAllways;
      }
    }

    [XmlAttribute("hostedOnly")]
    public bool HostedOnly
    {
      get => (this.Options & ServicingStep.StepOptions.HostedOnly) != 0;
      set
      {
        if (value)
          this.Options |= ServicingStep.StepOptions.HostedOnly;
        else
          this.Options &= ~ServicingStep.StepOptions.HostedOnly;
      }
    }

    [XmlAttribute("onPremOnly")]
    public bool OnPremOnly
    {
      get => (this.Options & ServicingStep.StepOptions.OnPremOnly) != 0;
      set
      {
        if (value)
          this.Options |= ServicingStep.StepOptions.OnPremOnly;
        else
          this.Options &= ~ServicingStep.StepOptions.OnPremOnly;
      }
    }

    [XmlAttribute("detachedOnly")]
    public bool DetachedOnly
    {
      get => (this.Options & ServicingStep.StepOptions.DetachedOnly) != 0;
      set
      {
        if (value)
          this.Options |= ServicingStep.StepOptions.DetachedOnly;
        else
          this.Options &= ~ServicingStep.StepOptions.DetachedOnly;
      }
    }

    [XmlAttribute("l2Only")]
    public bool L2Only
    {
      get => (this.Options & ServicingStep.StepOptions.L2Only) != 0;
      set
      {
        if (value)
          this.Options |= ServicingStep.StepOptions.L2Only;
        else
          this.Options &= ~ServicingStep.StepOptions.L2Only;
      }
    }

    [ClientProperty(ClientVisibility.Private, ClientVisibility.Private)]
    public XmlNode StepData { get; set; }

    public ServicingStep[] Steps { get; set; }

    internal string GroupName { get; set; }

    internal ServicingStep.StepOptions Options { get; set; }

    public static T DeserializeStepData<T>(string stepData)
    {
      try
      {
        return TeamFoundationSerializationUtility.Deserialize<T>(stepData, UnknownXmlNodeProcessing.ThrowException);
      }
      catch (Exception ex)
      {
        throw new TeamFoundationServicingException(FrameworkResources.ServicingDeserializationError((object) typeof (T).FullName), ex);
      }
    }

    internal static string SerializeStepData(object stepData)
    {
      Type type = stepData.GetType();
      try
      {
        XmlSerializer xmlSerializer = new XmlSerializer(type);
        XmlWriterSettings settings = new XmlWriterSettings();
        settings.OmitXmlDeclaration = true;
        settings.Indent = true;
        using (StringWriter output = new StringWriter((IFormatProvider) CultureInfo.InvariantCulture))
        {
          using (XmlWriter xmlWriter = XmlWriter.Create((TextWriter) output, settings))
            xmlSerializer.Serialize(xmlWriter, stepData);
          return output.ToString();
        }
      }
      catch (Exception ex)
      {
        throw new TeamFoundationServicingException(FrameworkResources.ServicingSerializationError((object) type.FullName), ex);
      }
    }

    [Flags]
    public enum StepOptions
    {
      None = 0,
      ExecuteAllways = 1,
      HostedOnly = 2,
      OnPremOnly = 4,
      DetachedOnly = 16, // 0x00000010
      L2Only = 32, // 0x00000020
    }
  }
}
