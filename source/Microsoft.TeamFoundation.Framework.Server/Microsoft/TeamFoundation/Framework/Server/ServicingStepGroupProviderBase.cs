// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ServicingStepGroupProviderBase
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public abstract class ServicingStepGroupProviderBase : IServicingStepGroupProvider
  {
    private bool m_throwOnDuplicates = true;
    private static readonly XmlSerializer s_stepGroupSerializer = new XmlSerializer(typeof (ServicingStepGroup));
    private ServicingStepGroup[] m_stepGroups;
    private readonly IServicingStepGroupProvider m_fallbackProvider;
    private readonly ITFLogger m_logger;
    private readonly object m_lock = new object();

    static ServicingStepGroupProviderBase()
    {
      ServicingStepGroupProviderBase.s_stepGroupSerializer.UnknownElement += new XmlElementEventHandler(ServicingStepGroupProviderBase.OnUnknownXmlElement);
      ServicingStepGroupProviderBase.s_stepGroupSerializer.UnknownAttribute += new XmlAttributeEventHandler(ServicingStepGroupProviderBase.OnUnknownXmlAttribute);
    }

    private static void OnUnknownXmlElement(object sender, XmlElementEventArgs e) => throw new TeamFoundationDeserializationException(FrameworkResources.UnknownXmlElementError((object) e.Element.Name, (object) e.LineNumber, (object) e.LinePosition));

    private static void OnUnknownXmlAttribute(object sender, XmlAttributeEventArgs e)
    {
      if (!string.Equals(e.Attr.Name, "usedExternally", StringComparison.Ordinal))
        throw new TeamFoundationDeserializationException(FrameworkResources.UnknownXmlAttributeError((object) e.Attr.Name, (object) e.LineNumber, (object) e.LinePosition));
    }

    public ServicingStepGroupProviderBase(
      IServicingStepGroupProvider fallbackProvider,
      ITFLogger logger)
    {
      this.m_fallbackProvider = fallbackProvider;
      this.m_logger = logger ?? (ITFLogger) new NullLogger();
    }

    public ServicingStepGroup[] GetServicingStepGroups()
    {
      if (this.m_stepGroups == null)
      {
        lock (this.m_lock)
        {
          if (this.m_stepGroups == null)
          {
            string[] groupResourceNames = this.GetServicingStepGroupResourceNames();
            Dictionary<string, ServicingStepGroup> dictionary1 = new Dictionary<string, ServicingStepGroup>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
            Dictionary<string, string> dictionary2 = new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
            for (int index = 0; index < groupResourceNames.Length; ++index)
            {
              using (Stream stream = this.OpenServicingStepGroupSteam(groupResourceNames[index]))
              {
                ServicingStepGroup servicingStepGroup;
                try
                {
                  servicingStepGroup = this.ReadServicingStepGroup(stream);
                  servicingStepGroup.Resource = groupResourceNames[index];
                }
                catch (Exception ex)
                {
                  throw new TeamFoundationServicingException(FrameworkResources.ErrorDeserializingStepGroup((object) groupResourceNames[index]), ex);
                }
                try
                {
                  dictionary1.Add(servicingStepGroup.Name, servicingStepGroup);
                  dictionary2.Add(servicingStepGroup.Name, groupResourceNames[index]);
                }
                catch (ArgumentException ex)
                {
                  throw new TeamFoundationServicingException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "'{0}' servicing step group is defined more than once. First resource: {1}, Second resource: {2}.", (object) servicingStepGroup.Name, (object) dictionary2[servicingStepGroup.Name], (object) groupResourceNames[index]));
                }
              }
            }
            if (this.m_fallbackProvider != null)
            {
              foreach (ServicingStepGroup servicingStepGroup in this.m_fallbackProvider.GetServicingStepGroups())
              {
                if (!dictionary1.ContainsKey(servicingStepGroup.Name))
                {
                  dictionary1.Add(servicingStepGroup.Name, servicingStepGroup);
                }
                else
                {
                  string message = string.Format("Duplicate step group loaded.  Name: '{0}'. Loaded from '{1}' and '{2}'.", (object) servicingStepGroup.Name, (object) dictionary1[servicingStepGroup.Name].Resource, (object) servicingStepGroup.Resource);
                  this.Logger.Info(message);
                  if (this.ThrowOnDuplicates)
                    throw new TeamFoundationServicingException(message);
                  this.Logger.Info("Resource for step group '{0}' loaded from '{1}' is ignored.", (object) servicingStepGroup.Name, (object) servicingStepGroup.Resource);
                }
              }
            }
            this.m_stepGroups = dictionary1.Values.OrderBy<ServicingStepGroup, string>((Func<ServicingStepGroup, string>) (stepGroup => stepGroup.Name), (IComparer<string>) StringComparer.OrdinalIgnoreCase).ToArray<ServicingStepGroup>();
          }
        }
      }
      return this.m_stepGroups;
    }

    public ServicingStepGroup GetServicingStepGroup(string stepGroupName)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(stepGroupName, nameof (stepGroupName));
      ServicingStepGroup[] servicingStepGroups = this.GetServicingStepGroups();
      int num1 = 0;
      int num2 = servicingStepGroups.Length;
      while (num1 < num2)
      {
        int index = (num1 + num2) / 2;
        int num3 = string.Compare(stepGroupName, servicingStepGroups[index].Name, StringComparison.OrdinalIgnoreCase);
        if (num3 > 0)
        {
          num1 = index + 1;
        }
        else
        {
          if (num3 >= 0)
            return servicingStepGroups[index];
          num2 = index;
        }
      }
      return (ServicingStepGroup) null;
    }

    protected ITFLogger Logger => this.m_logger;

    private ServicingStepGroup ReadServicingStepGroup(Stream stream)
    {
      XmlReaderSettings settings = new XmlReaderSettings()
      {
        IgnoreComments = true,
        IgnoreProcessingInstructions = true,
        IgnoreWhitespace = true,
        DtdProcessing = DtdProcessing.Prohibit,
        XmlResolver = (XmlResolver) null
      };
      using (XmlReader xmlReader = XmlReader.Create(stream, settings))
      {
        xmlReader.Read();
        if (xmlReader.NodeType == XmlNodeType.XmlDeclaration)
          xmlReader.Read();
        return (ServicingStepGroup) ServicingStepGroupProviderBase.s_stepGroupSerializer.Deserialize(xmlReader);
      }
    }

    public bool ThrowOnDuplicates
    {
      get => this.m_throwOnDuplicates;
      protected set => this.m_throwOnDuplicates = value;
    }

    protected abstract string[] GetServicingStepGroupResourceNames();

    protected abstract Stream OpenServicingStepGroupSteam(string resourceName);
  }
}
