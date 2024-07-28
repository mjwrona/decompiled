// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.WebServices.ProcessTemplateWebService
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.Azure.Boards.ProcessTemplates;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common.Internal;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Services;
using System.Xml;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.Server.Core.WebServices
{
  [WebService(Namespace = "http://schemas.microsoft.com/TeamFoundation/2005/06/Services/ProcessTemplate/03", Description = "DevOps Process Template web service")]
  [ClientService(ComponentName = "Framework", RegistrationName = "Framework", ServerConfiguration = ServerConfiguration.TfsTeamProjectCollection, ServiceName = "ProcessTemplate", CollectionServiceIdentifier = "75AB998E-7F09-479E-9559-B86B5B06F688")]
  public class ProcessTemplateWebService : FrameworkWebService
  {
    private ITeamFoundationProcessService m_processTemplateService;

    public ProcessTemplateWebService() => this.m_processTemplateService = this.RequestContext.GetService<ITeamFoundationProcessService>();

    [WebMethod]
    [return: XmlArrayItem(ElementName = "TemplateHeader")]
    public FrameworkTemplateHeader[] TemplateHeaders()
    {
      try
      {
        this.EnterMethod(new MethodInformation(nameof (TemplateHeaders), MethodType.Normal, EstimatedMethodCost.Low));
        return this.ListProcessDescriptors();
      }
      catch (Exception ex)
      {
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }

    [WebMethod]
    [return: XmlArrayItem(ElementName = "TemplateHeader")]
    public FrameworkTemplateHeader[] DeleteTemplate(int templateId)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (DeleteTemplate), MethodType.Admin, EstimatedMethodCost.Moderate);
        methodInformation.AddParameter(nameof (templateId), (object) templateId);
        this.EnterMethod(methodInformation);
        this.m_processTemplateService.DeleteProcess(this.RequestContext, this.m_processTemplateService.GetSpecificProcessDescriptor(this.RequestContext, this.m_processTemplateService.GetSpecificProcessDescriptorIdByIntegerId(this.RequestContext, templateId)).TypeId);
        return this.ListProcessDescriptors();
      }
      catch (Exception ex)
      {
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }

    [WebMethod]
    [return: XmlArrayItem(ElementName = "TemplateHeader")]
    public FrameworkTemplateHeader[] MakeDefaultTemplate(int templateId)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (MakeDefaultTemplate), MethodType.Admin, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (templateId), (object) templateId);
        this.EnterMethod(methodInformation);
        this.m_processTemplateService.SetProcessAsDefault(this.RequestContext, this.m_processTemplateService.GetSpecificProcessDescriptor(this.RequestContext, this.m_processTemplateService.GetSpecificProcessDescriptorIdByIntegerId(this.RequestContext, templateId)).TypeId);
        return this.ListProcessDescriptors();
      }
      catch (Exception ex)
      {
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }

    [WebMethod]
    [return: XmlArrayItem(ElementName = "TemplateHeader")]
    public FrameworkTemplateHeader[] SetDefaultTemplate(Guid templateId)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation("MakeDefaultTemplate", MethodType.Admin, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (templateId), (object) templateId);
        this.EnterMethod(methodInformation);
        this.m_processTemplateService.SetProcessAsDefault(this.RequestContext, templateId);
        return this.ListProcessDescriptors();
      }
      catch (Exception ex)
      {
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }

    private FrameworkTemplateHeader[] ListProcessDescriptors()
    {
      Guid defaultTypeId = this.m_processTemplateService.GetDefaultProcessTypeId(this.RequestContext);
      IReadOnlyCollection<ProcessDescriptor> processDescriptors = this.m_processTemplateService.GetProcessDescriptors(this.RequestContext);
      int rank = 0;
      Func<ProcessDescriptor, bool> keySelector = (Func<ProcessDescriptor, bool>) (x => x.TypeId != defaultTypeId);
      return processDescriptors.OrderBy<ProcessDescriptor, bool>(keySelector).Select<ProcessDescriptor, FrameworkTemplateHeader>((Func<ProcessDescriptor, FrameworkTemplateHeader>) (descriptor => new FrameworkTemplateHeader()
      {
        Name = descriptor.Name,
        TemplateId = descriptor.IntegerId,
        State = "visible",
        Description = descriptor.Description ?? string.Empty,
        Metadata = this.GetMetadataXmlString(descriptor),
        Rank = rank++,
        Id = descriptor.RowId
      })).ToArray<FrameworkTemplateHeader>();
    }

    private string GetMetadataXmlString(ProcessDescriptor processTemplateDescriptor)
    {
      XmlDocument xmlDocument = new XmlDocument();
      XmlElement element1 = xmlDocument.CreateElement("metadata");
      xmlDocument.AppendChild((XmlNode) element1);
      XmlElement element2 = xmlDocument.CreateElement("name");
      element2.InnerText = processTemplateDescriptor.Name;
      xmlDocument.DocumentElement.AppendChild((XmlNode) element2);
      XmlElement element3 = xmlDocument.CreateElement("description");
      element3.InnerText = processTemplateDescriptor.Description;
      xmlDocument.DocumentElement.AppendChild((XmlNode) element3);
      XmlElement element4 = xmlDocument.CreateElement("version");
      element4.SetAttribute("type", processTemplateDescriptor.TypeId.ToString());
      element4.SetAttribute("major", processTemplateDescriptor.Version.Major.ToString());
      element4.SetAttribute("minor", processTemplateDescriptor.Version.Minor.ToString());
      xmlDocument.DocumentElement.AppendChild((XmlNode) element4);
      string pluginsXmlString;
      using (Stream processPackageContent = this.m_processTemplateService.GetLegacyProcessPackageContent(this.RequestContext, processTemplateDescriptor))
      {
        using (ZipArchiveProcessTemplatePackage processTemplatePackage = new ZipArchiveProcessTemplatePackage(processPackageContent, false, true))
          pluginsXmlString = processTemplatePackage.PluginsXmlString;
      }
      if (!string.IsNullOrEmpty(pluginsXmlString))
      {
        XmlDocument document = XmlUtility.GetDocument(pluginsXmlString);
        xmlDocument.DocumentElement.AppendChild(xmlDocument.ImportNode((XmlNode) document.DocumentElement, true));
      }
      return xmlDocument.InnerXml;
    }
  }
}
