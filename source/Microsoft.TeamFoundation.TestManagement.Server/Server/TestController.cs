// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestController
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.TestManagement.Client;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [ClassVisibility(ClientVisibility.Internal)]
  public class TestController
  {
    private List<NameValuePair> m_properties;
    public static readonly long MaxHeartbeatIntervalInSeconds = 300;

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private)]
    public string Name { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private)]
    public string Description { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private)]
    public string GroupId { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private)]
    public TestControllerState State
    {
      get => (long) this.TimeSinceLastHeartbeat >= TestController.MaxHeartbeatIntervalInSeconds ? TestControllerState.Offline : TestControllerState.Online;
      set
      {
      }
    }

    [XmlArray]
    [XmlArrayItem(Type = typeof (NameValuePair))]
    [ClientProperty(ClientVisibility.Private)]
    public List<NameValuePair> Properties
    {
      get
      {
        if (this.m_properties == null)
          this.m_properties = new List<NameValuePair>();
        return this.m_properties;
      }
    }

    internal int TimeSinceLastHeartbeat { get; set; }

    public override string ToString() => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "TestController Name={0} Description={1}", (object) this.Name, (object) this.Description);

    internal static void Register(
      TestManagementRequestContext context,
      List<TestController> controllers)
    {
      ArgumentUtility.CheckForNull<List<TestController>>(controllers, nameof (controllers), context.RequestContext.ServiceName);
      context.SecurityManager.CheckManageTestControllersPermission(context);
      using (TestPlanningDatabase planningDatabase = TestPlanningDatabase.Create(context))
      {
        foreach (TestController controller in controllers)
        {
          try
          {
            ArgumentUtility.CheckForNull<TestController>(controller, "controller", context.RequestContext.ServiceName);
            planningDatabase.RegisterTestController(controller);
          }
          catch (TestObjectNotFoundException ex)
          {
            throw new TestManagementInvalidOperationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.TestControllerAlreadyExists, (object) controller.Name));
          }
        }
      }
    }

    internal static void Unregister(
      TestManagementRequestContext context,
      List<TestController> controllers)
    {
      ArgumentUtility.CheckForNull<List<TestController>>(controllers, nameof (controllers), context.RequestContext.ServiceName);
      context.SecurityManager.CheckManageTestControllersPermission(context);
      using (TestPlanningDatabase planningDatabase = TestPlanningDatabase.Create(context))
      {
        foreach (TestController controller in controllers)
        {
          ArgumentUtility.CheckStringForNullOrEmpty(controller.Name, "controllerName", context.RequestContext.ServiceName);
          planningDatabase.UnregisterTestController(controller.Name);
        }
      }
    }

    internal static void UpdateHeartbeat(
      TestManagementRequestContext context,
      string testControllerName)
    {
      if (context.RequestContext.IsFeatureEnabled("LabManagement.Server.BlockLabManagement"))
        throw new NotSupportedException(ServerResources.TestControllerNotSupported);
      context.SecurityManager.CheckManageTestControllersPermission(context);
      using (TestPlanningDatabase planningDatabase = TestPlanningDatabase.Create(context))
      {
        try
        {
          ArgumentUtility.CheckStringForNullOrEmpty(testControllerName, "controllerName", context.RequestContext.ServiceName);
          planningDatabase.UpdateTestControllerHeartbeatTime(testControllerName);
        }
        catch (TestObjectNotFoundException ex)
        {
          throw new TestManagementInvalidOperationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.TestControllerNotFound, (object) testControllerName));
        }
      }
    }

    internal static void Update(
      TestManagementRequestContext context,
      List<TestController> controllers)
    {
      ArgumentUtility.CheckForNull<List<TestController>>(controllers, nameof (controllers), context.RequestContext.ServiceName);
      context.SecurityManager.CheckManageTestControllersPermission(context);
      using (TestPlanningDatabase planningDatabase = TestPlanningDatabase.Create(context))
      {
        foreach (TestController controller in controllers)
        {
          try
          {
            ArgumentUtility.CheckForNull<TestController>(controller, "controller", context.RequestContext.ServiceName);
            planningDatabase.UpdateTestController(controller);
          }
          catch (TestObjectNotFoundException ex)
          {
            throw new TestManagementInvalidOperationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.TestControllerNotFound, (object) controller.Name));
          }
        }
      }
    }

    internal static List<TestController> Query(TfsTestManagementRequestContext context)
    {
      List<TestController> testControllerList = new List<TestController>();
      if (context.SecurityManager.HasGenericReadPermission((TestManagementRequestContext) context))
      {
        using (TestPlanningDatabase planningDatabase = TestPlanningDatabase.Create((TestManagementRequestContext) context))
          testControllerList = planningDatabase.QueryTestControllers((string) null, true);
      }
      return testControllerList;
    }

    internal static List<TestController> Query(TestManagementRequestContext context, string groupId)
    {
      List<TestController> testControllerList = new List<TestController>();
      if (context.SecurityManager.HasGenericReadPermission(context))
      {
        using (TestPlanningDatabase planningDatabase = TestPlanningDatabase.Create(context))
          testControllerList = planningDatabase.QueryTestControllers(groupId, false);
      }
      return testControllerList;
    }

    internal static TestController Find(TestManagementRequestContext context, string controllerName)
    {
      if (context.SecurityManager.HasGenericReadPermission(context))
      {
        using (TestPlanningDatabase planningDatabase = TestPlanningDatabase.Create(context))
        {
          List<TestController> testControllerList = planningDatabase.QueryTestControllers(controllerName, true);
          if (testControllerList.Count > 0)
          {
            context.TraceVerbose("BusinessLayer", "TestController.Find succeeded");
            return testControllerList[0];
          }
        }
      }
      context.TraceVerbose("BusinessLayer", "TestController.Find failed");
      return (TestController) null;
    }
  }
}
