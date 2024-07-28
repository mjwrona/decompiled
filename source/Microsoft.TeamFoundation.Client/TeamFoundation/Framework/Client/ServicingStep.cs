// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Client.ServicingStep
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.VisualStudio.Services.Common.Internal;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.Framework.Client
{
  [DebuggerDisplay("Name: {Name}, StepType: {StepType}, Performer: {StepPerformer}")]
  public sealed class ServicingStep
  {
    private bool m_detachedOnly;
    private bool m_executeAlways;
    private string m_name;
    private bool m_simpleRecoveryModel;
    private bool m_sqlAzureOnly;
    private bool m_sqlServerOnly;
    private XmlNode m_stepData;
    private string m_stepPerformer;
    private string m_stepType;

    public ServicingStep(string name, string performer, string stepType, string stepData)
    {
      this.Name = name;
      this.StepPerformer = performer;
      this.StepType = stepType;
      if (string.IsNullOrEmpty(stepData))
        return;
      XmlDocument xmlDocument = new XmlDocument();
      XmlReaderSettings settings = new XmlReaderSettings()
      {
        DtdProcessing = DtdProcessing.Prohibit,
        XmlResolver = (XmlResolver) null
      };
      using (StringReader input = new StringReader(stepData))
      {
        using (XmlReader reader = XmlReader.Create((TextReader) input, settings))
        {
          xmlDocument.Load(reader);
          this.StepData = (XmlNode) xmlDocument.DocumentElement;
        }
      }
    }

    public ServicingStep(string name, string performer, string stepType, object stepData)
      : this(name, performer, stepType, ServicingStep.SerializeStepData(stepData))
    {
    }

    public XmlNode StepData { get; set; }

    internal static string SerializeStepData(object stepData)
    {
      XmlSerializer xmlSerializer = new XmlSerializer(stepData.GetType());
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

    private ServicingStep()
    {
    }

    public bool DetachedOnly
    {
      get => this.m_detachedOnly;
      set => this.m_detachedOnly = value;
    }

    public bool ExecuteAlways
    {
      get => this.m_executeAlways;
      set => this.m_executeAlways = value;
    }

    public string Name
    {
      get => this.m_name;
      set => this.m_name = value;
    }

    public bool SimpleRecoveryModel
    {
      get => this.m_simpleRecoveryModel;
      set => this.m_simpleRecoveryModel = value;
    }

    public bool SqlAzureOnly
    {
      get => this.m_sqlAzureOnly;
      set => this.m_sqlAzureOnly = value;
    }

    public bool SqlServerOnly
    {
      get => this.m_sqlServerOnly;
      set => this.m_sqlServerOnly = value;
    }

    public string StepPerformer
    {
      get => this.m_stepPerformer;
      set => this.m_stepPerformer = value;
    }

    public string StepType
    {
      get => this.m_stepType;
      set => this.m_stepType = value;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static ServicingStep FromXml(IServiceProvider serviceProvider, XmlReader reader)
    {
      ServicingStep servicingStep = new ServicingStep();
      bool isEmptyElement = reader.IsEmptyElement;
      if (reader.HasAttributes)
      {
        while (reader.MoveToNextAttribute())
        {
          string name = reader.Name;
          if (name != null)
          {
            switch (name.Length)
            {
              case 4:
                if (name == "name")
                {
                  servicingStep.m_name = XmlUtility.StringFromXmlAttribute(reader);
                  continue;
                }
                continue;
              case 8:
                if (name == "stepType")
                {
                  servicingStep.m_stepType = XmlUtility.StringFromXmlAttribute(reader);
                  continue;
                }
                continue;
              case 12:
                switch (name[0])
                {
                  case 'd':
                    if (name == "detachedOnly")
                    {
                      servicingStep.m_detachedOnly = XmlUtility.BooleanFromXmlAttribute(reader);
                      continue;
                    }
                    continue;
                  case 's':
                    if (name == "sqlAzureOnly")
                    {
                      servicingStep.m_sqlAzureOnly = XmlUtility.BooleanFromXmlAttribute(reader);
                      continue;
                    }
                    continue;
                  default:
                    continue;
                }
              case 13:
                switch (name[1])
                {
                  case 'q':
                    if (name == "sqlServerOnly")
                    {
                      servicingStep.m_sqlServerOnly = XmlUtility.BooleanFromXmlAttribute(reader);
                      continue;
                    }
                    continue;
                  case 't':
                    if (name == "stepPerformer")
                    {
                      servicingStep.m_stepPerformer = XmlUtility.StringFromXmlAttribute(reader);
                      continue;
                    }
                    continue;
                  case 'x':
                    if (name == "executeAlways")
                    {
                      servicingStep.m_executeAlways = XmlUtility.BooleanFromXmlAttribute(reader);
                      continue;
                    }
                    continue;
                  default:
                    continue;
                }
              case 19:
                if (name == "simpleRecoveryModel")
                {
                  servicingStep.m_simpleRecoveryModel = XmlUtility.BooleanFromXmlAttribute(reader);
                  continue;
                }
                continue;
              default:
                continue;
            }
          }
        }
      }
      reader.Read();
      if (!isEmptyElement)
      {
        while (reader.NodeType == XmlNodeType.Element)
        {
          if (reader.Name == "StepData")
            servicingStep.m_stepData = XmlUtility.XmlNodeFromXmlElement(reader);
          else
            reader.ReadOuterXml();
        }
        reader.ReadEndElement();
      }
      return servicingStep;
    }

    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.AppendLine("ServicingStep instance " + this.GetHashCode().ToString());
      stringBuilder.AppendLine("  DetachedOnly: " + this.m_detachedOnly.ToString());
      stringBuilder.AppendLine("  ExecuteAlways: " + this.m_executeAlways.ToString());
      stringBuilder.AppendLine("  Name: " + this.m_name);
      stringBuilder.AppendLine("  SimpleRecoveryModel: " + this.m_simpleRecoveryModel.ToString());
      stringBuilder.AppendLine("  SqlAzureOnly: " + this.m_sqlAzureOnly.ToString());
      stringBuilder.AppendLine("  SqlServerOnly: " + this.m_sqlServerOnly.ToString());
      stringBuilder.AppendLine("  StepData: " + this.m_stepData?.ToString());
      stringBuilder.AppendLine("  StepPerformer: " + this.m_stepPerformer);
      stringBuilder.AppendLine("  StepType: " + this.m_stepType);
      return stringBuilder.ToString();
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public void ToXml(XmlWriter writer, string element)
    {
      writer.WriteStartElement(element);
      if (this.m_detachedOnly)
        XmlUtility.ToXmlAttribute(writer, "detachedOnly", this.m_detachedOnly);
      if (this.m_executeAlways)
        XmlUtility.ToXmlAttribute(writer, "executeAlways", this.m_executeAlways);
      if (this.m_name != null)
        XmlUtility.ToXmlAttribute(writer, "name", this.m_name);
      if (this.m_simpleRecoveryModel)
        XmlUtility.ToXmlAttribute(writer, "simpleRecoveryModel", this.m_simpleRecoveryModel);
      if (this.m_sqlAzureOnly)
        XmlUtility.ToXmlAttribute(writer, "sqlAzureOnly", this.m_sqlAzureOnly);
      if (this.m_sqlServerOnly)
        XmlUtility.ToXmlAttribute(writer, "sqlServerOnly", this.m_sqlServerOnly);
      if (this.m_stepPerformer != null)
        XmlUtility.ToXmlAttribute(writer, "stepPerformer", this.m_stepPerformer);
      if (this.m_stepType != null)
        XmlUtility.ToXmlAttribute(writer, "stepType", this.m_stepType);
      if (this.m_stepData != null)
        XmlUtility.ToXmlElement(writer, "StepData", this.m_stepData);
      writer.WriteEndElement();
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void ToXml(XmlWriter writer, string element, ServicingStep obj) => obj.ToXml(writer, element);
  }
}
