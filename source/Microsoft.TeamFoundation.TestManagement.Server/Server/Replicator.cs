// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.Replicator
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using Microsoft.Azure.Boards.CssNodes;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Integration.Server;
using Microsoft.TeamFoundation.TestManagement.Common.Internal;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  internal class Replicator : ITestManagementReplicator
  {
    private DateTime m_nextCssUpdateTime = DateTime.MinValue;
    private const int c_lockTimeout = 5000;
    private const string c_defaultLockResourceName = "TestManagement.Replicator";
    private int m_appTierSequenceId = -1;
    private CssCache m_areaPathsCache;
    private CssCache m_iterationsCache;

    public virtual ICssCache GetAreaPathsCache(TestManagementRequestContext context)
    {
      if (this.m_areaPathsCache == null)
      {
        this.ForceUpdateCss(context);
        context.IfNullThenTraceAndDebugFail("Cache", (object) this.m_areaPathsCache, "m_areaPathsCache");
      }
      return (ICssCache) this.m_areaPathsCache;
    }

    public virtual ICssCache GetIterationsCache(TestManagementRequestContext context)
    {
      if (this.m_iterationsCache == null)
      {
        this.ForceUpdateCss(context);
        context.IfNullThenTraceAndDebugFail("Cache", (object) this.m_iterationsCache, "m_iterationsCache");
      }
      return (ICssCache) this.m_iterationsCache;
    }

    public virtual void UpdateCss(TestManagementRequestContext context)
    {
      try
      {
        context.TraceEnter("Framework", "Replicator.UpdateCss");
        using (PerfManager.Measure(context.RequestContext, "BusinessLayer", "Replicator.UpdateCss"))
        {
          if (!(this.m_nextCssUpdateTime < DateTime.Now))
            return;
          this.UpdateCss(context.RequestContext, (Action) (() =>
          {
            if (!(this.m_nextCssUpdateTime < DateTime.Now))
              return;
            this.PerformCssUpdate(context);
          }));
        }
      }
      finally
      {
        context.TraceLeave("Framework", "Replicator.UpdateCss");
      }
    }

    public virtual void ForceUpdateCss(
      TestManagementRequestContext context,
      string lockResourcePrefix = "",
      int? lockTimeout = null)
    {
      this.UpdateCss(context.RequestContext, (Action) (() => this.PerformCssUpdate(context)), lockResourcePrefix, lockTimeout);
    }

    private void UpdateCss(
      IVssRequestContext context,
      Action updateCss,
      string lockResourcePrefix = "",
      int? lockTimeout = null)
    {
      try
      {
        context.TraceEnter("Framework", "Replicator.UpdateCss ResourcePrefix: " + lockResourcePrefix);
        int timeoutInMs = context.GetService<IVssRegistryService>().GetValue<int>(context, (RegistryQuery) "/Service/TestManagement/Settings/UpdateCssLockTimeOut", lockTimeout ?? 5000);
        using (TeamFoundationLock teamFoundationLock = Replicator.AcquireLock(context, timeoutInMs, lockResourcePrefix + "TestManagement.Replicator"))
        {
          if (teamFoundationLock == null)
            throw new TestManagementServiceException(ServerResources.UpdateCssCacheFailedError);
          if (updateCss == null)
            return;
          updateCss();
        }
      }
      finally
      {
        context.TraceLeave("Framework", "Replicator.UpdateCss");
      }
    }

    private void PerformCssUpdate(TestManagementRequestContext context)
    {
      using (context.RequestContext.AllowCrossDataspaceAccess())
      {
        int cssSequenceId1;
        using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(context))
          managementDatabase.QueryReplicationState(out cssSequenceId1, out string _);
        if (this.m_appTierSequenceId == -1)
        {
          Dictionary<string, IdAndString> pathToUri1;
          Dictionary<string, IdAndString> pathToUri2;
          using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(context))
          {
            pathToUri1 = managementDatabase.QueryAreaPaths();
            pathToUri2 = managementDatabase.QueryIterations();
          }
          this.m_areaPathsCache = new CssCache(context, pathToUri1, CssCacheType.Area);
          this.m_iterationsCache = new CssCache(context, pathToUri2, CssCacheType.Iteration);
          this.m_appTierSequenceId = cssSequenceId1;
        }
        context.TraceInfo("Cache", "Replicator.PerformCssUpdate - AppTierSequenceId={0} DBSequenceId={1}", (object) this.m_appTierSequenceId, (object) cssSequenceId1);
        if (!context.SecurityManager.HasTestManagementPermission(context))
          return;
        ICommonStructureService service = context.RequestContext.GetService<ICommonStructureService>();
        bool hasMore = false;
        do
        {
          string changedNodes = service.GetChangedNodes(context.RequestContext.Elevate(), this.m_appTierSequenceId);
          int cssSequenceId2 = Replicator.ProcessChangedNodes(context, service, changedNodes, out hasMore, this.m_appTierSequenceId);
          context.TraceInfo("Cache", "Replicator.PerformCssUpdate - NewSequenceId={0}", (object) cssSequenceId2);
          if (cssSequenceId2 > cssSequenceId1)
          {
            using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(context))
              managementDatabase.UpdateReplicationState(cssSequenceId2, (string) null);
          }
          if (cssSequenceId2 > this.m_appTierSequenceId)
            this.m_appTierSequenceId = cssSequenceId2;
        }
        while (hasMore);
        this.m_nextCssUpdateTime = DateTime.Now.AddMinutes(5.0);
      }
    }

    private static int ProcessChangedNodes(
      TestManagementRequestContext context,
      ICommonStructureService css,
      string xml,
      out bool hasMore,
      int lastSequenceId)
    {
      IVssRequestContext requestContext = context.RequestContext;
      int sequenceId = -1;
      hasMore = false;
      using (XmlTextReader safeXmlTextReader = XmlUtility.CreateSafeXmlTextReader((TextReader) new StringReader(xml)))
      {
        List<string> stringList = (List<string>) null;
        List<string> parameter = (List<string>) null;
        while (safeXmlTextReader.Read())
        {
          if (safeXmlTextReader.IsStartElement())
          {
            if (VssStringComparer.XmlElement.Equals(safeXmlTextReader.Name, "StructureChanges"))
            {
              sequenceId = XmlConvert.ToInt32(safeXmlTextReader["MaxSequence"]);
              hasMore = XmlConvert.ToInt32(safeXmlTextReader["fMore"]) != 0;
              if (sequenceId <= lastSequenceId)
              {
                context.TraceAndDebugAssert("Cache", !hasMore, "CSS has more changes but new sequenceId is not changed.");
                return lastSequenceId;
              }
            }
            else if (VssStringComparer.XmlElement.Equals(safeXmlTextReader.Name, "StructureElement"))
            {
              string y = safeXmlTextReader["StructureId"];
              string x = safeXmlTextReader["Deleted"];
              string nodeUri = safeXmlTextReader["Id"];
              if (StringComparer.OrdinalIgnoreCase.Equals(x, "true"))
              {
                string str = safeXmlTextReader["ForwardingId"];
                if (stringList == null)
                {
                  stringList = new List<string>();
                  parameter = new List<string>();
                }
                stringList.Add(nodeUri);
                parameter.Add(str);
              }
              else if (TFStringComparer.StructureType.Equals("ProjectModelHierarchy", y) || TFStringComparer.StructureType.Equals("ProjectLifecycle", y))
              {
                string projectUri = safeXmlTextReader["ProjectId"];
                if (safeXmlTextReader["ParentId"] == null && TFStringComparer.StructureType.Equals("ProjectModelHierarchy", y))
                {
                  CommonStructureProjectInfo project = css.GetProject(requestContext.Elevate(), projectUri);
                  Guid id = project.ToProjectInfo().Id;
                  bool isNewProject = false;
                  using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(context))
                    managementDatabase.CreateProject(projectUri, id, project.Name, sequenceId, out isNewProject);
                }
                CommonStructureNodeInfo node = css.GetNode(requestContext.Elevate(), nodeUri);
                string tcmPath = context.CSSHelper.CssNodeToTCMPath(node != null ? node.ToTcmCommonStructureNodeInfo() : (TcmCommonStructureNodeInfo) null);
                if (TFStringComparer.StructureType.Equals("ProjectModelHierarchy", y))
                {
                  Dictionary<string, IdAndString> area;
                  using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(context))
                    area = managementDatabase.CreateArea(node.Uri, tcmPath, projectUri, sequenceId);
                  Replicator.UpdateCssCache(context.AreaPathsCache, area);
                }
                else
                {
                  Dictionary<string, IdAndString> iteration;
                  using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(context))
                    iteration = managementDatabase.CreateIteration(node.Uri, tcmPath, projectUri, sequenceId);
                  Replicator.UpdateCssCache(context.IterationsCache, iteration);
                }
              }
            }
          }
        }
        if (stringList != null)
        {
          context.IfNullThenTraceAndDebugFail("Cache", (object) parameter, "ForwardingUris");
          context.TraceAndDebugAssert("Cache", stringList.Count == parameter.Count, "The counts of deletedUris and forwardingUris lists are not equal.");
          for (int index = 0; index < stringList.Count; ++index)
          {
            using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(context))
              managementDatabase.UpdateCssNodeUri(stringList[index], parameter[index]);
            context.AreaPathsCache.RemoveByUri(stringList[index]);
            context.IterationsCache.RemoveByUri(stringList[index]);
          }
        }
        context.TraceAndDebugAssert("Cache", sequenceId >= 0, "Did not find <StructureChanged> tag");
      }
      return sequenceId;
    }

    private static void UpdateCssCache(ICssCache cache, Dictionary<string, IdAndString> values)
    {
      foreach (KeyValuePair<string, IdAndString> keyValuePair in values)
        cache.Update(keyValuePair.Key, keyValuePair.Value);
    }

    internal static string CssToWorkItemPath(string cssPath)
    {
      string workItemPath = string.Empty;
      if (!string.IsNullOrWhiteSpace(cssPath) && cssPath[0] == '\\')
      {
        int num = cssPath.IndexOf('\\', 1);
        if (num > 0)
        {
          int startIndex = cssPath.IndexOf('\\', num + 1);
          string str1 = cssPath.Substring(1, num - 1);
          if (startIndex > 0)
          {
            string str2 = cssPath.Substring(startIndex);
            workItemPath = str1 + str2;
          }
          else
            workItemPath = str1;
        }
      }
      return workItemPath;
    }

    internal static string WorkItemToCssPath(
      string wiPath,
      CssCacheType cacheType,
      string appendString)
    {
      string cssPath = string.Empty;
      if (!appendString.StartsWith("\\"))
        appendString = appendString.Insert(0, "\\");
      if (!string.IsNullOrWhiteSpace(wiPath))
      {
        cssPath = wiPath.IndexOf('\\') >= 0 ? wiPath.Insert(wiPath.IndexOf('\\', 0), appendString) : wiPath + appendString;
        if (!cssPath.StartsWith("\\"))
          cssPath = cssPath.Insert(0, "\\");
      }
      return cssPath;
    }

    private static TeamFoundationLock AcquireLock(
      IVssRequestContext context,
      int timeoutInMs,
      string lockResourceName = "TestManagement.Replicator")
    {
      try
      {
        context.TraceEnter("Framework", string.Format("Replicator.AcquireLock Timeout: {0}, Resource: {1}", (object) timeoutInMs, (object) lockResourceName));
        return timeoutInMs == 0 ? context.GetService<ITeamFoundationLockingService>().AcquireLock(context, TeamFoundationLockMode.Exclusive, lockResourceName) : context.GetService<ITeamFoundationLockingService>().AcquireLock(context, TeamFoundationLockMode.Exclusive, lockResourceName, timeoutInMs);
      }
      finally
      {
        context.TraceLeave("Framework", "Replicator.AcquireLock");
      }
    }

    public void RefreshCache(IVssRequestContext requestContext, Guid eventClass, string eventData) => this.m_appTierSequenceId = -1;
  }
}
