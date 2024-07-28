// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestImpact.Server.Common.TestImpactBuildEventListner
// Assembly: Microsoft.TeamFoundation.TestImpact.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 079E4AEE-0642-4BDD-8B76-CECF38EBB798
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestImpact.Server.Common.dll

using Microsoft.TeamFoundation.Build.WebApi.Events;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.TestManagement.Server;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.TestImpact.Server.Common
{
  [CLSCompliant(false)]
  public class TestImpactBuildEventListner
  {
    public static XmlSerializer BuildsDeletedEventHandlerJobInputSerializer = new XmlSerializer(typeof (BuildsDeletedEventHandlerJobInput));

    public void HandleBuildsDeletedEvent(
      IVssRequestContext requestContext,
      BuildsDeletedEvent1 buildsDeletedEvent)
    {
      TestManagementRequestContext tcmRequestContext = new TestManagementRequestContext(requestContext);
      Dictionary<string, object> dictionary = new Dictionary<string, object>();
      try
      {
        dictionary.Add("ProjectId", (object) buildsDeletedEvent.ProjectId);
        dictionary.Add("DefinitionId", (object) buildsDeletedEvent.DefinitionId);
        dictionary.Add("BuildIds", (object) buildsDeletedEvent.BuildIds);
        bool flag = tcmRequestContext.IsFeatureEnabled("TestImpact.Server.BuildDataDeletion");
        dictionary.Add("IsFeatureEnabled", (object) flag);
        if (!flag)
          tcmRequestContext.Logger.Verbose(6203001, "The feature flag 'TestImpact.Server.BuildDataDeletion' is not enabled. Skipping handling build deletion event for test impact.");
        else
          this.QueueBuildsDeletedEventHandlerJob(tcmRequestContext, dictionary, buildsDeletedEvent);
      }
      catch (Exception ex)
      {
        string format = string.Format("TestImpactBuildEvenListner: HandleBuildsDeletedEvent failed with {0}", (object) ex);
        tcmRequestContext.Logger.Error(6203002, format);
        dictionary.Add("ErrorInHandleBuildsDeletedEvent", (object) ex);
      }
      finally
      {
        CustomerIntelligenceData intelligenceData = new CustomerIntelligenceData((IDictionary<string, object>) dictionary);
        CILogger.Instance.PublishCI(requestContext, TestImpactServiceCIFeature.BuildsDeletedEventHandler, dictionary);
      }
    }

    private void QueueBuildsDeletedEventHandlerJob(
      TestManagementRequestContext tcmRequestContext,
      Dictionary<string, object> ciData,
      BuildsDeletedEvent1 buildsDeletedEvent)
    {
      ITeamFoundationJobService service = tcmRequestContext.RequestContext.GetService<ITeamFoundationJobService>();
      BuildsDeletedEventHandlerJobInput input = new BuildsDeletedEventHandlerJobInput()
      {
        BuildIds = buildsDeletedEvent.BuildIds,
        DefinitionId = buildsDeletedEvent.DefinitionId,
        ProjectId = buildsDeletedEvent.ProjectId.ToString()
      };
      XmlNode handlerJobDetails = TestImpactBuildEventListner.GetBuildDeletedEventHandlerJobDetails(tcmRequestContext, input);
      IVssRequestContext requestContext = tcmRequestContext.RequestContext;
      XmlNode jobData = handlerJobDetails;
      Guid guid = service.QueueOneTimeJob(requestContext, "BuildsDeletedEventHandlerJob", "Microsoft.Azure.Pipelines.TestImpact.Server.Jobs.BuildsDeletedEventHandlerJob", jobData, JobPriorityLevel.Normal);
      ciData.Add("JobGuid", (object) guid);
    }

    private static XmlNode GetBuildDeletedEventHandlerJobDetails(
      TestManagementRequestContext tcmRequestContext,
      BuildsDeletedEventHandlerJobInput input)
    {
      if (input == null)
      {
        string str = "Builds Deleted Event Handler job can't be null";
        tcmRequestContext.Logger.Error(6203003, str);
        throw new ArgumentNullException(str);
      }
      using (MemoryStream input1 = new MemoryStream())
      {
        using (StreamWriter streamWriter = new StreamWriter((Stream) input1, Encoding.UTF8))
        {
          try
          {
            TestImpactBuildEventListner.BuildsDeletedEventHandlerJobInputSerializer.Serialize((TextWriter) streamWriter, (object) input);
            input1.Position = 0L;
            using (XmlReader reader = XmlReader.Create((Stream) input1))
            {
              XmlDocument xmlDocument = new XmlDocument();
              xmlDocument.Load(reader);
              return (XmlNode) xmlDocument.DocumentElement;
            }
          }
          catch (Exception ex)
          {
            tcmRequestContext.Logger.Error(6203004, string.Format("Error serializing the job data: {0}", (object) ex));
            throw;
          }
        }
      }
    }
  }
}
