// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.Compatibility.BuildServiceHostMessageTranslator
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using Microsoft.TeamFoundation.Build.Common;
using Microsoft.TeamFoundation.Build.Server.ServiceProxies;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Xml;

namespace Microsoft.TeamFoundation.Build.Server.Compatibility
{
  internal static class BuildServiceHostMessageTranslator
  {
    private static Lazy<Dictionary<string, BuildServiceHostMessageTranslator.MessageTranslator>> s_messageTranslators = new Lazy<Dictionary<string, BuildServiceHostMessageTranslator.MessageTranslator>>(new Func<Dictionary<string, BuildServiceHostMessageTranslator.MessageTranslator>>(BuildServiceHostMessageTranslator.RegisterTranslators));

    internal static void QueueMessage(
      IVssRequestContext requestContext,
      Message message,
      BuildServiceHost serviceHost,
      BuildController controller)
    {
      requestContext.TraceEnter(0, "BuildAdministration", "Service", "BuildServiceHostMessageTranslator.QueueMessage");
      BuildServiceHostMessageTranslator.Context taskArgs = new BuildServiceHostMessageTranslator.Context()
      {
        message = message,
        serviceHost = serviceHost,
        controller = controller,
        doc = new XmlDocument()
      };
      requestContext.Trace(0, TraceLevel.Verbose, "BuildAdministration", "Service", "Parsing message {0}", (object) message.Headers.Action);
      taskArgs.doc.Load((XmlReader) message.GetReaderAtBodyContents());
      taskArgs.mgr = new XmlNamespaceManager(taskArgs.doc.NameTable);
      taskArgs.mgr.AddNamespace("tb", "http://schemas.microsoft.com/TeamFoundation/2010/Build/Hosting");
      BuildServiceHostMessageTranslator.MessageTranslator messageTranslator;
      if (BuildServiceHostMessageTranslator.s_messageTranslators.Value.TryGetValue(message.Headers.Action, out messageTranslator))
      {
        taskArgs.translator = messageTranslator;
        TeamFoundationTaskService service = requestContext.To(TeamFoundationHostType.Deployment).GetService<TeamFoundationTaskService>();
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        TeamFoundationTask teamFoundationTask = new TeamFoundationTask(BuildServiceHostMessageTranslator.\u003C\u003EO.\u003C0\u003E__DispatchMessage ?? (BuildServiceHostMessageTranslator.\u003C\u003EO.\u003C0\u003E__DispatchMessage = new TeamFoundationTaskCallback(BuildServiceHostMessageTranslator.DispatchMessage)), (object) taskArgs, 0);
        IVssRequestContext requestContext1 = requestContext;
        TeamFoundationTask task = teamFoundationTask;
        service.AddTask(requestContext1, task);
      }
      else
        requestContext.Trace(0, TraceLevel.Verbose, "BuildAdministration", "Service", "Skipping unhandled message {0}", (object) message.Headers.Action);
      requestContext.TraceLeave(0, "BuildAdministration", "Service", "BuildServiceHostMessageTranslator.QueueMessage");
    }

    private static void DispatchMessage(IVssRequestContext requestContext, object taskArgs)
    {
      requestContext.TraceEnter(0, "BuildAdministration", "Service", "BuildServiceHostMessageTranslator.DispatchMessage");
      BuildServiceHostMessageTranslator.Context context = (BuildServiceHostMessageTranslator.Context) taskArgs;
      context.requestContext = requestContext;
      try
      {
        context.translator(context);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(0, TraceLevel.Error, "BuildAdministration", "Service", ex);
      }
      requestContext.TraceLeave(0, "BuildAdministration", "Service", "BuildServiceHostMessageTranslator.DispatchMessage");
    }

    private static void TranslateControllerUpdated(BuildServiceHostMessageTranslator.Context c)
    {
      using (HostServiceProxy2010 serviceProxy2010 = new HostServiceProxy2010(c.requestContext, c.serviceHost.BaseUrl, c.serviceHost.RequireClientCertificates))
      {
        XmlNode xmlNode = c.doc.SelectSingleNode("/tb:ControllerUpdated/tb:serviceUri", c.mgr);
        ServiceAction action = (ServiceAction) Enum.Parse(typeof (ServiceAction), c.doc.SelectSingleNode("/tb:ControllerUpdated/tb:action", c.mgr).InnerText, true);
        IAsyncResult result = serviceProxy2010.BeginControllerUpdated(xmlNode.InnerText, action, (AsyncCallback) null, (object) null);
        serviceProxy2010.EndControllerUpdated(result);
      }
    }

    private static void TranslateAgentUpdated(BuildServiceHostMessageTranslator.Context c)
    {
      using (HostServiceProxy2010 serviceProxy2010 = new HostServiceProxy2010(c.requestContext, c.serviceHost.BaseUrl, c.serviceHost.RequireClientCertificates))
      {
        XmlNode xmlNode = c.doc.SelectSingleNode("/tb:AgentUpdated/tb:serviceUri", c.mgr);
        ServiceAction action = (ServiceAction) Enum.Parse(typeof (ServiceAction), c.doc.SelectSingleNode("/tb:AgentUpdated/tb:action", c.mgr).InnerText, true);
        IAsyncResult result = serviceProxy2010.BeginAgentUpdated(xmlNode.InnerText, action, (AsyncCallback) null, (object) null);
        serviceProxy2010.EndAgentUpdated(result);
      }
    }

    private static void TranslateResourceAcquired(BuildServiceHostMessageTranslator.Context c)
    {
      XmlNode xmlNode1 = c.doc.SelectSingleNode("/tb:SharedResourceAcquired/tb:lockedBy", c.mgr);
      XmlNode xmlNode2 = c.doc.SelectSingleNode("/tb:SharedResourceAcquired/tb:resourceName", c.mgr);
      XmlNode xmlNode3 = c.doc.SelectSingleNode("/tb:SharedResourceAcquired/tb:instanceId", c.mgr);
      XmlNode xmlNode4 = c.doc.SelectSingleNode("/tb:SharedResourceAcquired/tb:buildUri", c.mgr);
      string buildUri = (string) null;
      if (xmlNode1 == null)
        return;
      string innerText = xmlNode1.InnerText;
      if (xmlNode4 != null)
        buildUri = xmlNode4.InnerText;
      using (SharedResourceNotifyServiceProxy2010 serviceProxy2010 = new SharedResourceNotifyServiceProxy2010(c.requestContext, c.serviceHost.GetUrlForService(innerText), c.serviceHost.RequireClientCertificates))
        serviceProxy2010.NotifySharedResourceAcquired(xmlNode2.InnerText, xmlNode3.InnerText, buildUri);
    }

    public static void DeleteBuildOutput(
      IVssRequestContext requestContext,
      BuildServiceHost serviceHost,
      BuildController controller,
      Message message)
    {
      requestContext.TraceEnter(0, "BuildAdministration", "Service", "BuildServiceHostMessageTranslator.DeleteBuildOutput");
      BuildServiceHostMessageTranslator.Context c = new BuildServiceHostMessageTranslator.Context()
      {
        message = message,
        serviceHost = serviceHost,
        controller = controller,
        requestContext = requestContext,
        doc = new XmlDocument()
      };
      requestContext.Trace(0, TraceLevel.Verbose, "BuildAdministration", "Service", "Parsing message {0}", (object) message.Headers.Action);
      c.doc.Load((XmlReader) message.GetReaderAtBodyContents());
      c.mgr = new XmlNamespaceManager(c.doc.NameTable);
      c.mgr.AddNamespace("tb", "http://schemas.microsoft.com/TeamFoundation/2010/Build/Hosting");
      try
      {
        switch (message.Headers.Action)
        {
          case "http://schemas.microsoft.com/TeamFoundation/2010/Build/Hosting/Controller/DeleteBuildDrop":
            BuildServiceHostMessageTranslator.TranslateDeleteBuildDrop(c);
            break;
          case "http://schemas.microsoft.com/TeamFoundation/2010/Build/Hosting/Controller/DeleteBuildSymbols":
            BuildServiceHostMessageTranslator.TranslateDeleteBuildSymbols(c);
            break;
        }
      }
      catch (Exception ex)
      {
        requestContext.TraceException(0, TraceLevel.Error, "BuildAdministration", "Service", ex);
      }
      requestContext.TraceLeave(0, "BuildAdministration", "Service", "BuildServiceHostMessageTranslator.DeleteBuildOutput");
    }

    private static void TranslateDeleteBuildSymbols(BuildServiceHostMessageTranslator.Context c)
    {
      if (c.controller == null)
        return;
      using (BuildControllerServiceProxy2010 serviceProxy2010 = new BuildControllerServiceProxy2010(c.requestContext, c.serviceHost.GetUrlForService(c.controller.Uri), c.serviceHost.RequireClientCertificates))
      {
        XmlNode xmlNode1 = c.doc.SelectSingleNode("/tb:DeleteBuildSymbols/tb:storePath", c.mgr);
        XmlNode xmlNode2 = c.doc.SelectSingleNode("/tb:DeleteBuildSymbols/tb:transactionId", c.mgr);
        serviceProxy2010.DeleteBuildSymbols(xmlNode1.InnerText, xmlNode2.InnerText);
      }
    }

    private static void TranslateDeleteBuildDrop(BuildServiceHostMessageTranslator.Context c)
    {
      if (c.controller == null)
        return;
      using (BuildControllerServiceProxy2010 serviceProxy2010 = new BuildControllerServiceProxy2010(c.requestContext, c.serviceHost.GetUrlForService(c.controller.Uri), c.serviceHost.RequireClientCertificates))
      {
        XmlNode xmlNode = c.doc.SelectSingleNode("/tb:DeleteBuildDrop/tb:dropLocation", c.mgr);
        if (xmlNode == null)
          return;
        serviceProxy2010.DeleteBuildDrop(xmlNode.InnerText);
      }
    }

    private static void TranslateAgentAcquired(BuildServiceHostMessageTranslator.Context c)
    {
      if (c.controller == null)
        return;
      using (BuildAgentNotifyServiceProxy2010 serviceProxy2010 = new BuildAgentNotifyServiceProxy2010(c.requestContext, c.serviceHost.GetUrlForService(c.controller.Uri), c.serviceHost.RequireClientCertificates))
      {
        XmlNode xmlNode1 = c.doc.SelectSingleNode("/tb:AgentAcquired/tb:buildUri", c.mgr);
        XmlNode xmlNode2 = c.doc.SelectSingleNode("/tb:AgentAcquired/tb:reservationId", c.mgr);
        XmlNode xmlNode3 = c.doc.SelectSingleNode("/tb:AgentAcquired/tb:reservedAgentUri", c.mgr);
        string innerText = xmlNode1.InnerText;
        IAsyncResult result = serviceProxy2010.BeginNotifyAgentAvailable(innerText, int.Parse(xmlNode2.InnerText), xmlNode3.InnerText, (AsyncCallback) null, (object) null);
        serviceProxy2010.EndNotifyAgentAvailable(result);
      }
    }

    private static void TranslateStopBuild(BuildServiceHostMessageTranslator.Context c)
    {
      if (c.controller == null)
        return;
      using (BuildControllerServiceProxy2010 serviceProxy2010 = new BuildControllerServiceProxy2010(c.requestContext, c.serviceHost.GetUrlForService(c.controller.Uri), c.serviceHost.RequireClientCertificates))
      {
        XmlNode xmlNode = c.doc.SelectSingleNode("/tb:StopBuild/tb:buildUri", c.mgr);
        c.doc.SelectSingleNode("/tb:StopBuild/tb:queueId", c.mgr);
        if (xmlNode == null)
          return;
        string innerText = xmlNode.InnerText;
        serviceProxy2010.StopBuild(innerText);
      }
    }

    private static void TranslateStartBuild(BuildServiceHostMessageTranslator.Context c)
    {
      if (c.controller == null)
        return;
      using (BuildControllerServiceProxy2010 serviceProxy2010 = new BuildControllerServiceProxy2010(c.requestContext, c.serviceHost.GetUrlForService(c.controller.Uri), c.serviceHost.RequireClientCertificates))
      {
        XmlNode xmlNode = c.doc.SelectSingleNode("/tb:StartBuild/tb:buildUri", c.mgr);
        c.doc.SelectSingleNode("/tb:StartBuild/tb:queueId", c.mgr);
        if (xmlNode == null)
          return;
        string innerText = xmlNode.InnerText;
        try
        {
          serviceProxy2010.StartBuild(innerText);
        }
        catch (FaultException ex)
        {
          BuildController.StopUnstartedBuild(c.requestContext, innerText, ex.Message);
        }
      }
    }

    private static Dictionary<string, BuildServiceHostMessageTranslator.MessageTranslator> RegisterTranslators() => new Dictionary<string, BuildServiceHostMessageTranslator.MessageTranslator>()
    {
      {
        "http://schemas.microsoft.com/TeamFoundation/2010/Build/Hosting/Controller/StartBuild",
        BuildServiceHostMessageTranslator.\u003C\u003EO.\u003C1\u003E__TranslateStartBuild ?? (BuildServiceHostMessageTranslator.\u003C\u003EO.\u003C1\u003E__TranslateStartBuild = new BuildServiceHostMessageTranslator.MessageTranslator(BuildServiceHostMessageTranslator.TranslateStartBuild))
      },
      {
        "http://schemas.microsoft.com/TeamFoundation/2010/Build/Hosting/Controller/StopBuild",
        BuildServiceHostMessageTranslator.\u003C\u003EO.\u003C2\u003E__TranslateStopBuild ?? (BuildServiceHostMessageTranslator.\u003C\u003EO.\u003C2\u003E__TranslateStopBuild = new BuildServiceHostMessageTranslator.MessageTranslator(BuildServiceHostMessageTranslator.TranslateStopBuild))
      },
      {
        "http://schemas.microsoft.com/TeamFoundation/2010/Build/Hosting/Controller/AgentAcquired",
        BuildServiceHostMessageTranslator.\u003C\u003EO.\u003C3\u003E__TranslateAgentAcquired ?? (BuildServiceHostMessageTranslator.\u003C\u003EO.\u003C3\u003E__TranslateAgentAcquired = new BuildServiceHostMessageTranslator.MessageTranslator(BuildServiceHostMessageTranslator.TranslateAgentAcquired))
      },
      {
        "http://schemas.microsoft.com/TeamFoundation/2010/Build/Hosting/Controller/DeleteBuildDrop",
        BuildServiceHostMessageTranslator.\u003C\u003EO.\u003C4\u003E__TranslateDeleteBuildDrop ?? (BuildServiceHostMessageTranslator.\u003C\u003EO.\u003C4\u003E__TranslateDeleteBuildDrop = new BuildServiceHostMessageTranslator.MessageTranslator(BuildServiceHostMessageTranslator.TranslateDeleteBuildDrop))
      },
      {
        "http://schemas.microsoft.com/TeamFoundation/2010/Build/Hosting/Controller/DeleteBuildSymbols",
        BuildServiceHostMessageTranslator.\u003C\u003EO.\u003C5\u003E__TranslateDeleteBuildSymbols ?? (BuildServiceHostMessageTranslator.\u003C\u003EO.\u003C5\u003E__TranslateDeleteBuildSymbols = new BuildServiceHostMessageTranslator.MessageTranslator(BuildServiceHostMessageTranslator.TranslateDeleteBuildSymbols))
      },
      {
        "http://schemas.microsoft.com/TeamFoundation/2010/Build/Hosting/SharedResource/ResourceAcquired",
        BuildServiceHostMessageTranslator.\u003C\u003EO.\u003C6\u003E__TranslateResourceAcquired ?? (BuildServiceHostMessageTranslator.\u003C\u003EO.\u003C6\u003E__TranslateResourceAcquired = new BuildServiceHostMessageTranslator.MessageTranslator(BuildServiceHostMessageTranslator.TranslateResourceAcquired))
      },
      {
        "http://schemas.microsoft.com/TeamFoundation/2010/Build/Hosting/ServiceHost/AgentUpdated",
        BuildServiceHostMessageTranslator.\u003C\u003EO.\u003C7\u003E__TranslateAgentUpdated ?? (BuildServiceHostMessageTranslator.\u003C\u003EO.\u003C7\u003E__TranslateAgentUpdated = new BuildServiceHostMessageTranslator.MessageTranslator(BuildServiceHostMessageTranslator.TranslateAgentUpdated))
      },
      {
        "http://schemas.microsoft.com/TeamFoundation/2010/Build/Hosting/ServiceHost/ControllerUpdated",
        BuildServiceHostMessageTranslator.\u003C\u003EO.\u003C8\u003E__TranslateControllerUpdated ?? (BuildServiceHostMessageTranslator.\u003C\u003EO.\u003C8\u003E__TranslateControllerUpdated = new BuildServiceHostMessageTranslator.MessageTranslator(BuildServiceHostMessageTranslator.TranslateControllerUpdated))
      }
    };

    private class Context
    {
      internal IVssRequestContext requestContext;
      internal Message message;
      internal BuildServiceHost serviceHost;
      internal BuildController controller;
      internal XmlDocument doc;
      internal XmlNamespaceManager mgr;
      public BuildServiceHostMessageTranslator.MessageTranslator translator;
    }

    private delegate void MessageTranslator(BuildServiceHostMessageTranslator.Context context);
  }
}
