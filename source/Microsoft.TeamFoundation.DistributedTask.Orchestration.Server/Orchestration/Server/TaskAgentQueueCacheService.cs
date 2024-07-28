// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.TaskAgentQueueCacheService
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.DataAccess;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common.Internal;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.Orchestration.Server
{
  internal sealed class TaskAgentQueueCacheService : VssCacheService
  {
    private VssMemoryCacheList<string, object> m_cache;
    private Lazy<XmlSerializer> s_queueSerializer = new Lazy<XmlSerializer>((Func<XmlSerializer>) (() => new XmlSerializer(typeof (TaskAgentQueue))));

    public bool Remove(Guid projectId, int queueId) => this.m_cache.Remove(this.GetKeyToken(projectId, queueId));

    public void Set(Guid projectId, int queueId, TaskAgentQueue queue) => this.m_cache[this.GetKeyToken(projectId, queueId)] = (object) queue;

    public bool TryGetValue(Guid projectId, int queueId, out TaskAgentQueue queue)
    {
      object obj;
      if (this.m_cache.TryGetValue(this.GetKeyToken(projectId, queueId), out obj))
      {
        queue = obj as TaskAgentQueue;
        return true;
      }
      queue = (TaskAgentQueue) null;
      return false;
    }

    public bool TryGetHostedQueues(Guid projectId, out IList<TaskAgentQueue> queues)
    {
      object obj;
      if (this.m_cache.TryGetValue(this.GetHostedKeyToken(projectId), out obj))
      {
        queues = obj as IList<TaskAgentQueue>;
        return true;
      }
      queues = (IList<TaskAgentQueue>) null;
      return false;
    }

    public void SetHostedQueues(Guid projectId, IList<TaskAgentQueue> queues) => this.m_cache[this.GetHostedKeyToken(projectId)] = (object) queues;

    protected override void ServiceStart(IVssRequestContext systemRequestContext)
    {
      base.ServiceStart(systemRequestContext);
      int num = new Random(Guid.NewGuid().GetHashCode()).Next(0, 120);
      TimeSpan timeSpan1 = TimeSpan.FromHours(1.0).Add(TimeSpan.FromSeconds((double) num));
      TimeSpan timeSpan2 = TimeSpan.FromMinutes(5.0);
      this.m_cache = new VssMemoryCacheList<string, object>((IVssCachePerformanceProvider) this, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase, 1024, new VssCacheExpiryProvider<string, object>(Capture.Create<TimeSpan>(timeSpan1), Capture.Create<TimeSpan>(timeSpan2)));
      ITeamFoundationSqlNotificationService service = systemRequestContext.GetService<ITeamFoundationSqlNotificationService>();
      service.RegisterNotification(systemRequestContext, "Default", SqlNotificationEventIds.AgentQueueCreated, new SqlNotificationCallback(this.QueueChangedCallback), false);
      service.RegisterNotification(systemRequestContext, "Default", SqlNotificationEventIds.AgentQueueDeleted, new SqlNotificationCallback(this.QueueChangedCallback), false);
      service.RegisterNotification(systemRequestContext, "Default", SqlNotificationEventIds.AgentQueuesDeleted, new SqlNotificationCallback(this.QueuesChangedCallback), false);
    }

    protected override void ServiceEnd(IVssRequestContext systemRequestContext)
    {
      this.m_cache.Clear();
      base.ServiceEnd(systemRequestContext);
    }

    private void QueueChangedCallback(
      IVssRequestContext requestContext,
      Guid eventClass,
      string eventData)
    {
      TaskAgentQueue taskAgentQueue = new TaskAgentQueue();
      requestContext.GetService<IDataspaceService>();
      using (StringReader stringReader = new StringReader(eventData))
      {
        StringReader input = stringReader;
        using (XmlReader reader = XmlReader.Create((TextReader) input, new XmlReaderSettings()
        {
          DtdProcessing = DtdProcessing.Prohibit,
          XmlResolver = (XmlResolver) null
        }))
        {
          try
          {
            int content = (int) reader.MoveToContent();
            taskAgentQueue = TaskAgentQueueCacheService.DeserializeTaskAgentQueue(requestContext, reader);
          }
          catch (Exception ex)
          {
            requestContext.Trace(10015519, TraceLevel.Error, "DistributedTask", "TaskAgentQueues", "Error: Unable to deserialize task agent queues in event data, eventData: {0}, exception: {1}", (object) eventData, (object) ex);
            return;
          }
        }
      }
      string keyToken = this.GetKeyToken(taskAgentQueue.ProjectId, taskAgentQueue.Id);
      if (eventClass == SqlNotificationEventIds.AgentQueueCreated)
        this.m_cache[keyToken] = (object) taskAgentQueue;
      else
        this.m_cache.Remove(keyToken);
      this.m_cache.Remove(this.GetHostedKeyToken(taskAgentQueue.ProjectId));
    }

    private void QueuesChangedCallback(
      IVssRequestContext requestContext,
      Guid eventClass,
      string eventData)
    {
      List<TaskAgentQueue> taskAgentQueueList = new List<TaskAgentQueue>();
      using (StringReader stringReader = new StringReader(eventData))
      {
        StringReader input = stringReader;
        using (XmlReader reader1 = XmlReader.Create((TextReader) input, new XmlReaderSettings()
        {
          DtdProcessing = DtdProcessing.Prohibit,
          IgnoreWhitespace = true,
          XmlResolver = (XmlResolver) null
        }))
        {
          try
          {
            int content = (int) reader1.MoveToContent();
            taskAgentQueueList.AddRange((IEnumerable<TaskAgentQueue>) XmlUtility.ArrayOfObjectFromXml<TaskAgentQueue>(reader1, "TaskAgentQueue", false, (Func<XmlReader, TaskAgentQueue>) (reader => TaskAgentQueueCacheService.DeserializeTaskAgentQueue(requestContext, reader))));
          }
          catch (Exception ex)
          {
            taskAgentQueueList = new List<TaskAgentQueue>();
            requestContext.Trace(10015519, TraceLevel.Error, "DistributedTask", "TaskAgentQueues", "Error: Unable to deserialize task agent queues in event data, eventData: {0}, exception: {1}", (object) eventData, (object) ex);
          }
        }
      }
      foreach (TaskAgentQueue taskAgentQueue in taskAgentQueueList)
      {
        string keyToken = this.GetKeyToken(taskAgentQueue.ProjectId, taskAgentQueue.Id);
        if (eventClass == SqlNotificationEventIds.AgentQueueCreated)
          this.m_cache[keyToken] = (object) taskAgentQueue;
        else
          this.m_cache.Remove(keyToken);
        this.m_cache.Remove(this.GetHostedKeyToken(taskAgentQueue.ProjectId));
      }
    }

    internal static TaskAgentQueue DeserializeTaskAgentQueue(
      IVssRequestContext requestContext,
      XmlReader reader)
    {
      int num = reader.IsEmptyElement ? 1 : 0;
      TaskAgentQueue taskAgentQueue = new TaskAgentQueue();
      IDataspaceService service = requestContext.GetService<IDataspaceService>();
      reader.Read();
      if (num == 0)
      {
        while (reader.NodeType == XmlNodeType.Element)
        {
          switch (reader.Name)
          {
            case "Id":
              taskAgentQueue.Id = XmlUtility.Int32FromXmlElement(reader);
              continue;
            case "ProjectId":
              taskAgentQueue.ProjectId = XmlUtility.GuidFromXmlElement(reader);
              continue;
            case "DataspaceId":
              int dataspaceId = XmlUtility.Int32FromXmlElement(reader);
              Dataspace dataspace = service.QueryDataspace(requestContext, dataspaceId);
              taskAgentQueue.ProjectId = dataspace.DataspaceIdentifier;
              continue;
            case "Name":
              taskAgentQueue.Name = XmlUtility.StringFromXmlElement(reader);
              continue;
            case "Pool":
              taskAgentQueue.Pool = TaskAgentQueueCacheService.DeserializePoolReference(reader);
              continue;
            default:
              reader.ReadOuterXml();
              continue;
          }
        }
        reader.ReadEndElement();
      }
      return taskAgentQueue;
    }

    private static TaskAgentPoolReference DeserializePoolReference(XmlReader reader)
    {
      int num = reader.IsEmptyElement ? 1 : 0;
      TaskAgentPoolReference agentPoolReference = new TaskAgentPoolReference();
      reader.Read();
      if (num == 0)
      {
        while (reader.NodeType == XmlNodeType.Element)
        {
          switch (reader.Name)
          {
            case "Id":
              agentPoolReference.Id = XmlUtility.Int32FromXmlElement(reader);
              continue;
            case "Name":
              agentPoolReference.Name = XmlUtility.StringFromXmlElement(reader);
              continue;
            case "Scope":
              agentPoolReference.Scope = XmlUtility.GuidFromXmlElement(reader);
              continue;
            case "IsHosted":
              agentPoolReference.IsHosted = XmlUtility.BooleanFromXmlElement(reader);
              continue;
            case "PoolType":
              agentPoolReference.PoolType = XmlUtility.EnumFromXmlElement<TaskAgentPoolType>(reader);
              continue;
            case "Size":
              agentPoolReference.Size = XmlUtility.Int32FromXmlElement(reader);
              continue;
            default:
              reader.ReadOuterXml();
              continue;
          }
        }
        reader.ReadEndElement();
      }
      return agentPoolReference;
    }

    internal string GetKeyToken(Guid projectId, int queueId) => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}/{1}", (object) projectId.ToString(), (object) queueId);

    internal string GetHostedKeyToken(Guid projectId) => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}/HostedQueues", (object) projectId);
  }
}
