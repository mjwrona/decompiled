// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Promote.AgilePromoteStepFactory
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using Microsoft.Azure.Boards.ProcessTemplates;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Common.Internal;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Promote
{
  public class AgilePromoteStepFactory : IProjectPromoteStepFactory
  {
    private static IProjectPromoteStepFactory s_instance;

    private AgilePromoteStepFactory()
    {
    }

    public static IProjectPromoteStepFactory Instance
    {
      get
      {
        if (AgilePromoteStepFactory.s_instance == null)
          AgilePromoteStepFactory.s_instance = (IProjectPromoteStepFactory) new AgilePromoteStepFactory();
        return AgilePromoteStepFactory.s_instance;
      }
    }

    public IEnumerable<IProjectPromoteStep> GenerateSteps(
      IVssRequestContext requestContext,
      IProcessTemplate source,
      IProcessTemplate destination,
      StringBuilder log)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<IProcessTemplate>(destination, nameof (destination));
      ArgumentUtility.CheckForNull<StringBuilder>(log, nameof (log));
      string xml1 = (string) null;
      if (source != null)
      {
        XmlDocument trackingProcessData = ProcessTemplateHelper.GetWorkItemTrackingProcessData(requestContext, source);
        if (trackingProcessData != null)
          xml1 = AgilePromoteStepFactory.GetProcessConfiguration(requestContext, source, trackingProcessData);
      }
      XmlDocument trackingProcessData1 = ProcessTemplateHelper.GetWorkItemTrackingProcessData(requestContext, destination);
      if (trackingProcessData1 == null)
      {
        log.AppendLine("Empty work item tracking process data is not supported.");
        throw new InvalidOperationException();
      }
      string processConfiguration = AgilePromoteStepFactory.GetProcessConfiguration(requestContext, destination, trackingProcessData1);
      log.AppendLine("Diff for Process Configuration.");
      return !XmlUtility.CompareXmlDocuments(xml1, processConfiguration) ? (IEnumerable<IProjectPromoteStep>) Enumerable.Repeat<AgilePromoteStepFactory.AgilePromoteStep>(new AgilePromoteStepFactory.AgilePromoteStep(processConfiguration), 1) : Enumerable.Empty<IProjectPromoteStep>();
    }

    public void ValidateSteps(
      IVssRequestContext requestContext,
      IEnumerable<IProjectPromoteStep> steps,
      Guid projectId,
      StringBuilder log)
    {
    }

    private static string GetProcessConfiguration(
      IVssRequestContext requestContext,
      IProcessTemplate processTemplate,
      XmlDocument processData)
    {
      XmlNode xmlNode = processData.SelectSingleNode("//PROCESSCONFIGURATION").SelectSingleNode("./ProjectConfiguration");
      if (xmlNode == null)
        return string.Empty;
      string resourceName = xmlNode.Attributes["fileName"].Value;
      using (StreamReader streamReader = new StreamReader(processTemplate.GetResource(resourceName)))
        return streamReader.ReadToEnd();
    }

    private class AgilePromoteStep : IProjectPromoteStep
    {
      public AgilePromoteStep(string processConfiguration) => this.ProcessConfiguration = processConfiguration;

      public string ProcessConfiguration { get; private set; }

      public void Execute(
        IVssRequestContext requestContext,
        ProjectInfo project,
        StringBuilder log)
      {
        log.AppendLine("Importing process configuration.");
        ProjectProcessConfiguration settings = TeamFoundationSerializationUtility.Deserialize<ProjectProcessConfiguration>(this.ProcessConfiguration);
        requestContext.GetService<ProjectConfigurationService>().SetProcessSettings(requestContext, project.Uri, settings);
      }
    }
  }
}
