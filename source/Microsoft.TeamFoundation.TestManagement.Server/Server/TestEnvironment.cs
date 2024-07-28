// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestEnvironment
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Core;
using Microsoft.TeamFoundation.TestManagement.Common;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [ClassVisibility(ClientVisibility.Internal)]
  public class TestEnvironment
  {
    private List<NameValuePair> m_properties;
    private List<MachineRole> m_machineRoles;

    internal TestEnvironment()
    {
    }

    private TestEnvironment(
      TestManagementRequestContext context,
      CatalogResource resource,
      CatalogNode controllerNode)
    {
      string property = CatalogServiceHelper.GetProperty(resource, "TcmInternal_EnvironmentId");
      this.Id = string.IsNullOrEmpty(property) ? Guid.Empty : new Guid(property);
      this.Name = resource.DisplayName;
      this.Description = resource.Description;
      this.ProjectName = CatalogServiceHelper.GetProperty(resource, "TcmInternal_ProjectName");
      this.m_machineRoles = MachineRole.FromXml(CatalogServiceHelper.GetProperty(resource, "TcmInternal_MachineRoles"));
      this.SetProperties(context.RequestContext, resource.Properties);
      this.ControllerName = CatalogServiceHelper.GetProperty(controllerNode.Resource, "TcmInternal_ControllerName");
      this.ControllerDisplayName = CatalogServiceHelper.GetProperty(controllerNode.Resource, "__Reserved_ControllerDisplayName");
    }

    internal TestEnvironment(
      IVssRequestContext context,
      Guid id,
      string name,
      string description,
      string projectName,
      string controllerName,
      string controllerDisplayName,
      IDictionary<string, string> properties)
    {
      this.Id = id;
      this.Name = name;
      this.Description = description;
      this.ProjectName = projectName;
      this.SetProperties(context, properties);
      this.ControllerName = controllerName;
      this.ControllerDisplayName = controllerDisplayName;
    }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private)]
    public Guid Id { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private)]
    public string Name { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private)]
    public string Description { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private)]
    public string ProjectName { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private)]
    public string ControllerName { get; set; }

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

    [XmlIgnore]
    internal KeyValuePair<string, string> IdKeyValue => new KeyValuePair<string, string>("TcmInternal_EnvironmentId", this.Id.ToString());

    [XmlArray]
    [XmlArrayItem(Type = typeof (MachineRole))]
    [ClientProperty(ClientVisibility.Private)]
    public List<MachineRole> MachineRoles
    {
      get
      {
        if (this.m_machineRoles == null)
          this.m_machineRoles = new List<MachineRole>();
        return this.m_machineRoles;
      }
    }

    internal long LabEnvironmentId { get; private set; }

    internal void Internal_SetLabEnvironmentId(long environmentId) => this.LabEnvironmentId = environmentId;

    public override string ToString() => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "TestEnvironment Name={0} Description={1}", (object) this.Name, (object) this.Description);

    internal static void Register(
      TfsTestManagementRequestContext context,
      List<TestEnvironment> environments,
      string parentName,
      EnvironmentParentTypes parentType,
      Guid teamProjectCollectionCatalogResourceId)
    {
      TestEnvironment.CheckAdministerPermission((TestManagementRequestContext) context, parentName, parentType);
      CatalogNode parent = TestEnvironment.GetParent((TestManagementRequestContext) context, teamProjectCollectionCatalogResourceId, parentName, parentType);
      CatalogTransactionContext transactionContext = context.CatalogService.CreateTransactionContext();
      HashSet<string> uniqueNames = new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      foreach (TestEnvironment environment in environments)
      {
        Validator.CheckUniqueName(environment.Name, uniqueNames);
        CatalogNode catalogNode1;
        CatalogNode catalogNode2;
        if (parent.Resource.ResourceType.Identifier == CatalogResourceTypes.TestController)
        {
          catalogNode1 = parent;
          catalogNode2 = TestEnvironment.GetParent((TestManagementRequestContext) context, teamProjectCollectionCatalogResourceId, environment.ProjectName, EnvironmentParentTypes.TeamProject);
        }
        else
        {
          catalogNode1 = TestEnvironment.GetParent((TestManagementRequestContext) context, teamProjectCollectionCatalogResourceId, environment.ControllerName, EnvironmentParentTypes.TestController);
          catalogNode2 = parent;
        }
        TestEnvironment.CheckUniqueName((TestManagementRequestContext) context, catalogNode1.FullPath, EnvironmentParentTypes.TestController, environment.Name);
        TestEnvironment.CheckUniqueName((TestManagementRequestContext) context, catalogNode2.FullPath, EnvironmentParentTypes.TeamProject, environment.Name);
        CatalogNode child1 = catalogNode2.CreateChild(context.RequestContext.To(TeamFoundationHostType.Application).Elevate(), CatalogResourceTypes.TestEnvironment, environment.Name);
        if (environment.Id == Guid.Empty)
          environment.Id = Guid.NewGuid();
        foreach (MachineRole machineRole in environment.MachineRoles)
          machineRole.Id = Guid.NewGuid();
        child1.Resource.Description = environment.Description;
        environment.SetPropertyDictionary((TestManagementRequestContext) context, child1.Resource.Properties, MachineRole.ToXml(environment.MachineRoles));
        transactionContext.AttachNode(child1);
        CatalogNode child2 = catalogNode1.CreateChild(context.RequestContext.To(TeamFoundationHostType.Application).Elevate(), child1.Resource);
        transactionContext.AttachNode(child2);
        child1.Dependencies.Singletons.Add(catalogNode1.Resource.DisplayName, catalogNode1);
        child2.Dependencies.Singletons.Add(catalogNode2.Resource.DisplayName, catalogNode2);
        environment.ControllerDisplayName = CatalogServiceHelper.GetProperty(catalogNode1.Resource, "__Reserved_ControllerDisplayName");
      }
      CatalogServiceHelper.SaveBatch((TestManagementRequestContext) context, transactionContext);
      for (int index = 0; index < environments.Count; ++index)
      {
        TestEnvironment environment = environments[index];
      }
    }

    internal static void Unregister(
      TestManagementRequestContext context,
      List<TestEnvironment> environments,
      string parentName,
      EnvironmentParentTypes parentType,
      Guid teamProjectCollectionCatalogResourceId)
    {
      TestEnvironment.CheckAdministerPermission(context, parentName, parentType);
      CatalogNode parent = TestEnvironment.GetParent(context, teamProjectCollectionCatalogResourceId, parentName, parentType);
      TfsTestManagementRequestContext context1 = context as TfsTestManagementRequestContext;
      CatalogTransactionContext transactionContext = context1.CatalogService.CreateTransactionContext();
      foreach (TestEnvironment environment in environments)
      {
        CatalogResource resourceByProperty = CatalogServiceHelper.FindResourceByProperty(context1, parent.FullPath, CatalogResourceTypes.TestEnvironment, environment.IdKeyValue);
        if (resourceByProperty != null)
        {
          foreach (CatalogNode nodeReference in resourceByProperty.NodeReferences)
            transactionContext.AttachDelete(nodeReference, true);
        }
      }
      CatalogServiceHelper.SaveBatch(context, transactionContext);
      using (TestPlanningDatabase planningDatabase = TestPlanningDatabase.Create(context))
        planningDatabase.RemoveDeletedEnvironmentIds(environments.Select<TestEnvironment, Guid>((Func<TestEnvironment, Guid>) (environment => environment.Id)));
    }

    internal static void Update(
      TestManagementRequestContext context,
      List<TestEnvironment> environments,
      string parentName,
      EnvironmentParentTypes parentType,
      Guid teamProjectCollectionCatalogResourceId)
    {
      ArgumentUtility.CheckForNull<List<TestEnvironment>>(environments, nameof (environments), context.RequestContext.ServiceName);
      TestEnvironment.CheckAdministerPermission(context, parentName, parentType);
      TfsTestManagementRequestContext managementRequestContext = context as TfsTestManagementRequestContext;
      CatalogTransactionContext transactionContext = managementRequestContext.CatalogService.CreateTransactionContext();
      HashSet<string> uniqueNames = new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      foreach (TestEnvironment environment in environments)
      {
        List<CatalogResource> catalogResourceList = managementRequestContext.CatalogService.QueryResources(context.RequestContext.To(TeamFoundationHostType.Application).Elevate(), (IEnumerable<Guid>) new Guid[1]
        {
          CatalogResourceTypes.TestEnvironment
        }, (IEnumerable<KeyValuePair<string, string>>) new KeyValuePair<string, string>[1]
        {
          environment.IdKeyValue
        }, CatalogQueryOptions.IncludeParents);
        CatalogResource catalogResource = catalogResourceList.Count != 0 ? catalogResourceList[0] : throw new TestManagementInvalidOperationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.TestEnvironmentNotFound, (object) environment.Name));
        CatalogNode parameter1 = (CatalogNode) null;
        CatalogNode catalogNode1 = (CatalogNode) null;
        CatalogNode parameter2 = (CatalogNode) null;
        CatalogNode catalogNode2 = (CatalogNode) null;
        foreach (CatalogNode nodeReference in catalogResource.NodeReferences)
        {
          context.IfNullThenTraceAndDebugFail("BusinessLayer", (object) nodeReference.ParentNode, "nodeReference.ParentNode");
          if (nodeReference.ParentNode.Resource.ResourceType.Identifier == CatalogResourceTypes.TestController)
          {
            parameter1 = nodeReference;
            catalogNode1 = nodeReference.ParentNode;
          }
          else if (nodeReference.ParentNode.Resource.ResourceType.Identifier == CatalogResourceTypes.TeamProject)
          {
            parameter2 = nodeReference;
            catalogNode2 = nodeReference.ParentNode;
          }
          else
            context.TraceAndDebugFail("BusinessLayer", "Unknown node reference: " + nodeReference.ParentNode.Resource.ChangeType.ToString());
        }
        context.IfNullThenTraceAndDebugFail("BusinessLayer", (object) parameter1, "controllerChild");
        context.IfNullThenTraceAndDebugFail("BusinessLayer", (object) parameter2, "projectChild");
        if (!catalogResource.DisplayName.Equals(environment.Name, StringComparison.OrdinalIgnoreCase))
        {
          Validator.CheckUniqueName(environment.Name, uniqueNames);
          TestEnvironment.CheckUniqueName(context, catalogNode1.FullPath, EnvironmentParentTypes.TestController, environment.Name);
          TestEnvironment.CheckUniqueName(context, catalogNode2.FullPath, EnvironmentParentTypes.TeamProject, environment.Name);
        }
        List<MachineRole> machineRoleList = MachineRole.FromXml(CatalogServiceHelper.GetProperty(catalogResource, "TcmInternal_MachineRoles"));
        MachineRole.UpdateExistingRoles(machineRoleList, environment.MachineRoles);
        catalogResource.DisplayName = environment.Name;
        catalogResource.Description = environment.Description;
        environment.SetPropertyDictionary(context, catalogResource.Properties, MachineRole.ToXml(machineRoleList));
        transactionContext.AttachResource(catalogResource);
      }
      CatalogServiceHelper.SaveBatch(context, transactionContext);
    }

    internal static List<TestEnvironment> Query(
      TestManagementRequestContext context,
      string parentName,
      EnvironmentParentTypes parentType,
      Guid teamProjectCollectionCatalogResourceId)
    {
      return new List<TestEnvironment>();
    }

    private static TestEnvironment Find(TestManagementRequestContext context, Guid environmentId)
    {
      KeyValuePair<string, string> property = new KeyValuePair<string, string>("TcmInternal_EnvironmentId", environmentId.ToString());
      return TestEnvironment.Find(context, property);
    }

    private static TestEnvironment Find(
      TestManagementRequestContext context,
      KeyValuePair<string, string> property)
    {
      TestEnvironment testEnvironment = (TestEnvironment) null;
      List<CatalogResource> catalogResourceList = (context as TfsTestManagementRequestContext).CatalogService.QueryResources(context.RequestContext.To(TeamFoundationHostType.Application).Elevate(), (IEnumerable<Guid>) new Guid[1]
      {
        CatalogResourceTypes.TestEnvironment
      }, (IEnumerable<KeyValuePair<string, string>>) new KeyValuePair<string, string>[1]
      {
        property
      }, CatalogQueryOptions.IncludeParents);
      if (catalogResourceList.Count != 0)
      {
        CatalogResource resource = catalogResourceList[0];
        foreach (CatalogNode nodeReference in resource.NodeReferences)
        {
          if (nodeReference.ParentNode.Resource.ResourceTypeIdentifier == CatalogResourceTypes.TestController)
          {
            testEnvironment = new TestEnvironment(context, resource, nodeReference.ParentNode);
            context.TraceVerbose("BusinessLayer", "TestEnvironment.Find succeeded");
            break;
          }
        }
      }
      return testEnvironment;
    }

    private static void CheckAdministerPermission(
      TestManagementRequestContext context,
      string parentName,
      EnvironmentParentTypes parentType)
    {
      if (parentType == EnvironmentParentTypes.TeamProject)
      {
        context.SecurityManager.CheckManageTestEnvironmentsPermission(context, Validator.CheckAndGetProjectUriFromName(context, parentName));
      }
      else
      {
        context.TraceAndDebugAssert("BusinessLayer", parentType == EnvironmentParentTypes.TestController, "Incorrect environment parent type.");
        context.SecurityManager.CheckManageTestControllersPermission(context);
      }
    }

    private static CatalogNode GetParent(
      TestManagementRequestContext context,
      Guid teamProjectCollectionCatalogResourceId,
      string parentName,
      EnvironmentParentTypes parentType)
    {
      TfsTestManagementRequestContext context1 = context as TfsTestManagementRequestContext;
      if (parentType == EnvironmentParentTypes.TeamProject)
        return CatalogServiceHelper.GetEnvironmentParentNode(context1, teamProjectCollectionCatalogResourceId, CatalogResourceTypes.TeamProject, new KeyValuePair<string, string>("ProjectName", parentName));
      context.TraceAndDebugAssert("BusinessLayer", parentType == EnvironmentParentTypes.TestController, "Incorrect environment parent type.");
      return CatalogServiceHelper.GetEnvironmentParentNode(context1, teamProjectCollectionCatalogResourceId, CatalogResourceTypes.TestController, new KeyValuePair<string, string>("TcmInternal_ControllerName", parentName));
    }

    private static void CheckUniqueName(
      TestManagementRequestContext context,
      string parentPath,
      EnvironmentParentTypes parentType,
      string name)
    {
      CatalogResource resourceByProperty = CatalogServiceHelper.FindResourceByProperty(context as TfsTestManagementRequestContext, parentPath, CatalogResourceTypes.TestEnvironment, new KeyValuePair<string, string>("TcmInternal_EnvironmentName", name));
      if (resourceByProperty != null)
      {
        if (parentType == EnvironmentParentTypes.TeamProject)
        {
          string property = CatalogServiceHelper.GetProperty(resourceByProperty, "TcmInternal_EnvironmentId");
          if (!string.IsNullOrEmpty(property))
          {
            TestEnvironment testEnvironment = TestEnvironment.Find(context, new Guid(property));
            TestManagementInvalidOperationException operationException = new TestManagementInvalidOperationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.TestEnvironmentAlreadyExistsOnTestController, (object) name, (object) testEnvironment.ControllerDisplayName));
            operationException.ErrorCode = 6;
            throw operationException;
          }
        }
        throw new TestManagementInvalidOperationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.TestEnvironmentAlreadyExists, (object) name));
      }
    }

    private void SetPropertyDictionary(
      TestManagementRequestContext context,
      IDictionary<string, string> propertyDictionary,
      string rolesXml)
    {
      context.IfNullThenTraceAndDebugFail("BusinessLayer", (object) propertyDictionary, nameof (propertyDictionary));
      propertyDictionary.Clear();
      propertyDictionary["TcmInternal_ProjectName"] = this.ProjectName;
      propertyDictionary["TcmInternal_ControllerName"] = this.ControllerName;
      propertyDictionary["TcmInternal_EnvironmentName"] = this.Name;
      propertyDictionary["TcmInternal_EnvironmentId"] = this.Id.ToString();
      propertyDictionary["TcmInternal_MachineRoles"] = rolesXml;
      foreach (NameValuePair property in this.Properties)
        propertyDictionary[property.Name] = property.Value;
    }

    private void SetProperties(
      IVssRequestContext context,
      IDictionary<string, string> propertyDictionary)
    {
      context.IfNullThenTraceAndDebugFail("BusinessLayer", (object) propertyDictionary, nameof (propertyDictionary));
      propertyDictionary.Remove("TcmInternal_ControllerName");
      propertyDictionary.Remove("TcmInternal_ProjectName");
      propertyDictionary.Remove("TcmInternal_EnvironmentName");
      propertyDictionary.Remove("TcmInternal_EnvironmentId");
      propertyDictionary.Remove("TcmInternal_MachineRoles");
      foreach (KeyValuePair<string, string> property in (IEnumerable<KeyValuePair<string, string>>) propertyDictionary)
        this.Properties.Add(new NameValuePair(property.Key, property.Value));
    }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private)]
    public string ControllerDisplayName { get; set; }
  }
}
