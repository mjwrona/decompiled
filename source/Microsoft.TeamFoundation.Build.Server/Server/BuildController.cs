// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.BuildController
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using Microsoft.TeamFoundation.Build.Common;
using Microsoft.TeamFoundation.Build.Server.DataAccess;
using Microsoft.TeamFoundation.Build.Server.ServiceProxies;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Core;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.TeamFoundation.VersionControl.Common;
using Microsoft.TeamFoundation.VersionControl.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.FileContainer;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.ServiceModel.Channels;
using System.Threading;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.Build.Server
{
  [RequiredClientService("BuildServer")]
  [CallOnDeserialization("AfterDeserialize")]
  [ClassVisibility(ClientVisibility.Internal, ClientVisibility.Internal)]
  [XmlType(Namespace = "http://schemas.microsoft.com/TeamFoundation/2010/Build")]
  public sealed class BuildController : IValidatable, IPropertyProvider
  {
    private int? m_version;
    private List<string> m_tags = new List<string>();
    private List<PropertyValue> m_properties = new List<PropertyValue>();

    public BuildController() => this.Status = ControllerStatus.Unavailable;

    [XmlAttribute]
    [ClientType(typeof (System.Uri))]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Internal)]
    public string Uri { get; set; }

    [XmlAttribute]
    [ClientType(typeof (System.Uri))]
    [ClientProperty(ClientVisibility.Internal, ClientVisibility.Private)]
    public string ServiceHostUri { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Public)]
    public string Name { get; set; }

    [ClientProperty(ClientVisibility.Public, ClientVisibility.Public)]
    public string Description { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Public)]
    public string CustomAssemblyPath { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Public)]
    public int MaxConcurrentBuilds { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private, Direction = ClientPropertySerialization.ServerToClientOnly)]
    public int QueueCount { get; set; }

    [XmlAttribute]
    [DefaultValue(ControllerStatus.Unavailable)]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Public)]
    public ControllerStatus Status { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Public)]
    public string StatusMessage { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Public)]
    public bool Enabled { get; set; }

    [XmlAttribute]
    [ClientType(typeof (System.Uri))]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Internal)]
    public string Url { get; set; }

    [XmlAttribute]
    [ClientType(typeof (System.Uri))]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private)]
    public string MessageQueueUrl { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private)]
    public DateTime DateCreated { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private)]
    public DateTime DateUpdated { get; set; }

    [ClientProperty(ClientVisibility.Internal, PropertyName = "InternalProperties")]
    public List<PropertyValue> Properties => this.m_properties;

    [ClientProperty(ClientVisibility.Private, ClientVisibility.Private, PropertyName = "InternalTags", Direction = ClientPropertySerialization.ServerToClientOnly)]
    public List<string> Tags => this.m_tags;

    internal int Version
    {
      get
      {
        if (!this.m_version.HasValue)
          this.m_version = new int?(BuildServiceHost.GetVersion(this.Url));
        return this.m_version.Value;
      }
    }

    internal void DeleteBuildDrop(IVssRequestContext requestContext, BuildDetail build)
    {
      requestContext.TraceEnter(0, "Build", "Service", nameof (DeleteBuildDrop));
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<BuildDetail>(build, nameof (build));
      string error = (string) null;
      string dropLocation = build.DropLocation;
      if (string.IsNullOrEmpty(dropLocation) || !Microsoft.TeamFoundation.Build.Common.Validation.IsValidDropLocation(ref dropLocation, out error))
      {
        requestContext.Trace(0, TraceLevel.Warning, "Build", "Service", "Bad drop location '{0}': '{1}'", (object) dropLocation, (object) error);
      }
      else
      {
        if (Microsoft.TeamFoundation.Build.Common.Validation.IsValidUncPath(dropLocation, out error))
        {
          if (dropLocation.Split(new char[1]{ '\\' }, StringSplitOptions.RemoveEmptyEntries).Length <= 2)
          {
            requestContext.Trace(0, TraceLevel.Warning, "Build", "Service", "Bad UNC drop location '{0}': 'Cannot delete a share'", (object) dropLocation);
            return;
          }
          bool flag = true;
          Message message1 = (Message) null;
          try
          {
            message1 = BuildControllerService.DeleteBuildDrop(build.DropLocation);
            message1.Headers.To = new System.Uri(this.MessageQueueUrl);
            TeamFoundationBuildResourceService service = requestContext.GetService<TeamFoundationBuildResourceService>();
            requestContext.Trace(0, TraceLevel.Verbose, "Build", "Service", "Enqueuing message '{0}'", (object) message1);
            IVssRequestContext requestContext1 = requestContext;
            Message message2 = message1;
            string url = this.Url;
            service.QueueMessage(requestContext1, message2, url);
            flag = false;
          }
          catch (MessageQueueNotFoundException ex)
          {
            requestContext.TraceException(0, "Build", "Service", (Exception) ex);
          }
          finally
          {
            if (flag && message1 != null)
            {
              requestContext.Trace(0, TraceLevel.Verbose, "Build", "Service", "Closing message '{0}'", (object) message1);
              message1.Close();
            }
          }
        }
        else if (VersionControlPath.IsServerItem(dropLocation))
        {
          string parent = VersionControlPath.Combine(VersionControlPath.PrependRootIfNeeded(build.TeamProject), "Drops");
          if (!VersionControlPath.IsSubItem(dropLocation, parent))
          {
            requestContext.Trace(0, TraceLevel.Warning, "Build", "Service", "Ignoring drop location '{0}' because it is not a sub item of the configured drop root '{1}' for team project '{2}'", (object) dropLocation, (object) parent, (object) build.TeamProject);
          }
          else
          {
            TeamFoundationVersionControlService service = requestContext.GetService<TeamFoundationVersionControlService>();
            try
            {
              using (service.Destroy(requestContext.Elevate(), new ItemSpec(dropLocation, RecursionType.Full), (VersionSpec) new LatestVersionSpec(), (VersionSpec) null, 8))
                ;
            }
            catch (ItemNotFoundException ex)
            {
              requestContext.Trace(0, TraceLevel.Warning, "Build", "Service", "Item '{0}' was not found in source control", (object) dropLocation);
            }
            requestContext.Trace(0, TraceLevel.Verbose, "Build", "Service", "Destroyed item path '{0}'", (object) dropLocation);
          }
        }
        else if (BuildContainerPath.IsServerPath(dropLocation))
        {
          try
          {
            TeamFoundationFileContainerService service = requestContext.GetService<TeamFoundationFileContainerService>();
            long containerId1 = -1;
            string itemPath;
            BuildContainerPath.GetContainerIdAndPath(dropLocation, out containerId1, out itemPath);
            if (containerId1 > 0L)
            {
              Guid projectId = requestContext.GetService<IProjectService>().GetProjectId(requestContext, build.TeamProject);
              TeamFoundationFileContainerService containerService = service;
              IVssRequestContext requestContext2 = requestContext.Elevate();
              long containerId2 = containerId1;
              List<string> paths = new List<string>();
              paths.Add(itemPath);
              Guid scopeIdentifier = projectId;
              containerService.DeleteItems(requestContext2, containerId2, (IList<string>) paths, scopeIdentifier);
            }
          }
          catch (InvalidPathException ex)
          {
            requestContext.Trace(0, TraceLevel.Warning, "Build", "Service", "Container location '{0}' is not formatted correctly", (object) build.DropLocation);
          }
          catch (FileContainerException ex)
          {
            requestContext.Trace(0, TraceLevel.Info, "Build", "Service", "Container '{0}' does not exist in the file container service", (object) build.DropLocationRoot);
          }
          catch (ProjectDoesNotExistWithNameException ex)
          {
            requestContext.Trace(0, TraceLevel.Warning, "Build", "Security", "Could not find project {0}", (object) build.TeamProject);
          }
          requestContext.Trace(0, TraceLevel.Verbose, "Build", "Service", "Destroyed file container '{0}'", (object) build.DropLocationRoot);
        }
        requestContext.TraceLeave(0, "Build", "Service", nameof (DeleteBuildDrop));
      }
    }

    internal void DeleteBuildSymbols(
      IVssRequestContext requestContext,
      IList<SymbolStoreData> symbolStores)
    {
      requestContext.TraceEnter(0, "Build", "Service", nameof (DeleteBuildSymbols));
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<IList<SymbolStoreData>>(symbolStores, nameof (symbolStores));
      if (symbolStores.Count == 0)
      {
        requestContext.Trace(0, TraceLevel.Warning, "Build", "Service", "No symbol stores to delete");
      }
      else
      {
        foreach (SymbolStoreData symbolStore in (IEnumerable<SymbolStoreData>) symbolStores)
        {
          bool flag = true;
          Message message1 = (Message) null;
          try
          {
            message1 = BuildControllerService.DeleteBuildSymbols(symbolStore.StorePath, symbolStore.TransactionId);
            message1.Headers.To = new System.Uri(this.MessageQueueUrl);
            TeamFoundationBuildResourceService service = requestContext.GetService<TeamFoundationBuildResourceService>();
            requestContext.Trace(0, TraceLevel.Verbose, "Build", "Service", "Enqueuing message '{0}'", (object) message1);
            IVssRequestContext requestContext1 = requestContext;
            Message message2 = message1;
            string url = this.Url;
            service.QueueMessage(requestContext1, message2, url);
            flag = false;
          }
          catch (MessageQueueNotFoundException ex)
          {
            requestContext.TraceException(0, "Build", "Service", (Exception) ex);
          }
          finally
          {
            if (flag && message1 != null)
            {
              requestContext.Trace(0, TraceLevel.Verbose, "Build", "Service", "Closing message '{0}'", (object) message1);
              message1.Close();
            }
          }
        }
        requestContext.TraceLeave(0, "Build", "Service", nameof (DeleteBuildSymbols));
      }
    }

    internal static void NotifyAgentsAvailable(
      IVssRequestContext requestContext,
      IEnumerable<AgentReservationData> reservations)
    {
      requestContext.TraceEnter(0, "BuildAdministration", "Service", nameof (NotifyAgentsAvailable));
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<IEnumerable<AgentReservationData>>(reservations, nameof (reservations));
      TeamFoundationBuildResourceService service = requestContext.GetService<TeamFoundationBuildResourceService>();
      foreach (AgentReservationData reservation in reservations)
      {
        Message message = (Message) null;
        try
        {
          message = BuildControllerService.NotifyAgentAvailable(reservation.BuildUri, reservation.ReservationId, reservation.ReservedAgentUri);
          message.Headers.To = new System.Uri(reservation.MessageQueueUrl);
          requestContext.Trace(0, TraceLevel.Verbose, "BuildAdministration", "Service", "Enqueuing message '{0}'", (object) message);
          service.QueueMessage(requestContext.Elevate(), message, reservation.EndpointUrl);
        }
        catch (MessageQueueNotFoundException ex)
        {
          requestContext.TraceException(0, "BuildAdministration", "Service", (Exception) ex);
        }
        finally
        {
          if (message != null)
          {
            requestContext.Trace(0, TraceLevel.Verbose, "BuildAdministration", "Service", "Closing message '{0}'", (object) message);
            message.Close();
          }
        }
      }
      requestContext.TraceLeave(0, "BuildAdministration", "Service", nameof (NotifyAgentsAvailable));
    }

    internal void StartBuilds(IVssRequestContext requestContext, IEnumerable<BuildDetail> builds) => BuildController.StartBuilds(requestContext, (IList<StartBuildData>) builds.Select<BuildDetail, StartBuildData>((Func<BuildDetail, StartBuildData>) (x => new StartBuildData()
    {
      BuildUri = x.Uri,
      MessageQueueUrl = this.MessageQueueUrl,
      QueueId = x.QueueIds.FirstOrDefault<int>()
    })).ToArray<StartBuildData>());

    internal static void StartBuilds(
      IVssRequestContext requestContext,
      IList<StartBuildData> startBuildData,
      IDictionary<string, BuildDefinition> definitionLookup = null,
      IDictionary<string, BuildController> controllerLookup = null,
      IDictionary<string, BuildServiceHost> serviceHostLookup = null)
    {
      requestContext.TraceEnter(0, "Build", "Service", nameof (StartBuilds));
      if (startBuildData.Count == 0)
      {
        requestContext.Trace(0, TraceLevel.Verbose, "Build", "Service", "No builds to start");
        requestContext.TraceLeave(0, "Build", "Service", nameof (StartBuilds));
      }
      else
      {
        TeamFoundationBuildService service1 = requestContext.GetService<TeamFoundationBuildService>();
        if (definitionLookup == null)
        {
          definitionLookup = (IDictionary<string, BuildDefinition>) new BuildDefinitionDictionary();
          controllerLookup = (IDictionary<string, BuildController>) new BuildControllerDictionary();
          serviceHostLookup = (IDictionary<string, BuildServiceHost>) new BuildServiceHostDictionary();
          string[] array = startBuildData.Select<StartBuildData, string>((Func<StartBuildData, string>) (x => x.DefinitionUri)).Where<string>((Func<string, bool>) (x => !string.IsNullOrEmpty(x))).Distinct<string>().ToArray<string>();
          if (array.Length != 0)
          {
            BuildDefinitionQueryResult definitionQueryResult = service1.QueryBuildDefinitionsByUri(requestContext.Elevate(), (IList<string>) array, (IList<string>) null, QueryOptions.Controllers, new Guid());
            foreach (BuildDefinition definition in definitionQueryResult.Definitions)
              definitionLookup.Add(definition.Uri, definition);
            foreach (BuildController controller in definitionQueryResult.Controllers)
              controllerLookup.Add(controller.Uri, controller);
            foreach (BuildServiceHost serviceHost in definitionQueryResult.ServiceHosts)
              serviceHostLookup.Add(serviceHost.Uri, serviceHost);
          }
        }
        if (controllerLookup == null)
          controllerLookup = (IDictionary<string, BuildController>) new BuildControllerDictionary();
        TeamFoundationBuildResourceService service2 = requestContext.GetService<TeamFoundationBuildResourceService>();
        string[] array1 = startBuildData.Select<StartBuildData, string>((Func<StartBuildData, string>) (x => x.ControllerUri)).Except<string>((IEnumerable<string>) controllerLookup.Keys).Distinct<string>().ToArray<string>();
        if (array1.Length != 0)
        {
          BuildControllerQueryResult controllerQueryResult = service2.QueryBuildControllersByUri(requestContext.Elevate(), (IList<string>) array1, (IList<string>) null, false);
          foreach (BuildController controller in controllerQueryResult.Controllers)
            controllerLookup.Add(controller.Uri, controller);
          foreach (BuildServiceHost serviceHost in controllerQueryResult.ServiceHosts)
            serviceHostLookup.Add(serviceHost.Uri, serviceHost);
        }
        using (IDisposableReadOnlyList<IBuildQueueExtension> extensions = requestContext.GetExtensions<IBuildQueueExtension>())
          startBuildData = (IList<StartBuildData>) startBuildData.Where<StartBuildData>((Func<StartBuildData, bool>) (s =>
          {
            foreach (IBuildQueueExtension buildQueueExtension in (IEnumerable<IBuildQueueExtension>) extensions)
            {
              try
              {
                BuildDefinition definition = definitionLookup[s.DefinitionUri];
                BuildServiceHost serviceHost = serviceHostLookup[controllerLookup[s.ControllerUri].ServiceHostUri];
                if (!buildQueueExtension.BuildStarting(requestContext.Elevate(), s, serviceHost, definition))
                {
                  requestContext.Trace(0, TraceLevel.Warning, "Build", "Service", "StartBuild {0} rejected by plugin", (object) s.BuildUri);
                  return false;
                }
              }
              catch (Exception ex)
              {
                requestContext.TraceException(0, "Build", "Service", ex);
              }
            }
            return true;
          })).ToList<StartBuildData>();
        TeamFoundationFileContainerService service3 = requestContext.GetService<TeamFoundationFileContainerService>();
        foreach (StartBuildData startData in (IEnumerable<StartBuildData>) startBuildData.OrderBy<StartBuildData, int>((Func<StartBuildData, int>) (x => x.StartOrder)))
        {
          Message message = (Message) null;
          try
          {
            BuildController.CreateBuildContainer(requestContext, definitionLookup, service1, service3, startData);
            message = BuildControllerService.StartBuild(startData.BuildUri, startData.QueueId);
            message.Headers.To = new System.Uri(startData.MessageQueueUrl);
            requestContext.Trace(0, TraceLevel.Verbose, "Build", "Service", "Enqueuing message '{0}'", (object) message);
            service2.QueueMessage(requestContext, message, startData.EndpointUrl);
          }
          catch (MessageQueueNotFoundException ex)
          {
            requestContext.TraceException(0, "Build", "Service", (Exception) ex);
          }
          catch (Exception ex)
          {
            requestContext.TraceException(0, "Build", "Service", ex);
            BuildController.StopUnstartedBuild(requestContext.Elevate(), startData.BuildUri, ex.Message);
          }
          finally
          {
            if (message != null)
            {
              requestContext.Trace(0, TraceLevel.Verbose, "Build", "Service", "Closing message '{0}'", (object) message);
              message.Close();
            }
          }
        }
        requestContext.TraceLeave(0, "Build", "Service", nameof (StartBuilds));
      }
    }

    private static void CreateBuildContainer(
      IVssRequestContext requestContext,
      IDictionary<string, BuildDefinition> definitionLookup,
      TeamFoundationBuildService buildService,
      TeamFoundationFileContainerService containerService,
      StartBuildData startData)
    {
      if (string.IsNullOrEmpty(startData.DefinitionUri))
        return;
      string name = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "BUILD_{0}", (object) LinkingUtilities.DecodeUri(startData.BuildUri).ToolSpecificId);
      long container = containerService.CreateContainer(requestContext.Elevate(), new System.Uri(startData.BuildUri), definitionLookup[startData.DefinitionUri].SecurityToken, name, string.Empty, definitionLookup[startData.DefinitionUri].TeamProject.Id, ContainerOptions.None);
      requestContext.Trace(0, TraceLevel.Verbose, "Build", "Service", "Created container {0} for build {1}", (object) container, (object) startData.BuildUri);
      BuildUpdateOptions buildUpdateOptions = new BuildUpdateOptions()
      {
        Fields = BuildUpdate.ContainerId,
        ContainerId = new long?(container),
        Uri = startData.BuildUri,
        ProjectId = definitionLookup[startData.DefinitionUri].TeamProject.Id
      };
      if (BuildContainerPath.IsServerPath(startData.DropLocation) || string.IsNullOrEmpty(startData.DropLocation))
      {
        string str = BuildContainerPath.Combine(BuildContainerPath.RootFolder, container.ToString((IFormatProvider) CultureInfo.InvariantCulture));
        if (BuildContainerPath.IsServerPath(startData.DropLocation))
        {
          buildUpdateOptions.Fields |= BuildUpdate.DropLocation | BuildUpdate.DropLocationRoot;
          buildUpdateOptions.DropLocation = BuildContainerPath.Combine(str, "drop");
          buildUpdateOptions.DropLocationRoot = str;
          FileContainerItem fileContainerItem = new FileContainerItem();
          fileContainerItem.ContainerId = container;
          fileContainerItem.ItemType = ContainerItemType.Folder;
          fileContainerItem.Path = "drop";
          TeamFoundationFileContainerService containerService1 = containerService;
          IVssRequestContext requestContext1 = requestContext.Elevate();
          long containerId = container;
          List<FileContainerItem> items = new List<FileContainerItem>();
          items.Add(fileContainerItem);
          Guid id = definitionLookup[startData.DefinitionUri].TeamProject.Id;
          containerService1.CreateItems(requestContext1, containerId, (IList<FileContainerItem>) items, id);
        }
        buildUpdateOptions.Fields |= BuildUpdate.LogLocation;
        buildUpdateOptions.LogLocation = BuildContainerPath.Combine(str, "logs");
      }
      using (BuildComponent component = requestContext.CreateComponent<BuildComponent>())
      {
        TeamFoundationIdentityService service = requestContext.GetService<TeamFoundationIdentityService>();
        component.UpdateBuilds((ICollection<BuildUpdateOptions>) new BuildUpdateOptions[1]
        {
          buildUpdateOptions
        }, service.ReadRequestIdentity(requestContext), buildService.WriterId);
      }
    }

    internal void StopBuilds(
      IVssRequestContext requestContext,
      IdentityDescriptor requestedFor,
      IList<BuildDetail> builds)
    {
      requestContext.TraceEnter(0, "Build", "Service", nameof (StopBuilds));
      List<string> list = builds.Select<BuildDetail, string>((Func<BuildDetail, string>) (x => x.Uri)).ToList<string>();
      Microsoft.TeamFoundation.Framework.Common.TimeoutHelper timeoutHelper = new Microsoft.TeamFoundation.Framework.Common.TimeoutHelper(TimeSpan.FromMinutes(1.0));
      TeamFoundationBuildService service1 = requestContext.GetService<TeamFoundationBuildService>();
      TeamFoundationBuildResourceService service2 = requestContext.GetService<TeamFoundationBuildResourceService>();
      foreach (BuildDetail build in (IEnumerable<BuildDetail>) builds)
      {
        bool flag = true;
        Message message = (Message) null;
        try
        {
          message = BuildControllerService.StopBuild(build);
          message.Headers.To = new System.Uri(this.MessageQueueUrl);
          requestContext.Trace(0, TraceLevel.Verbose, "Build", "Service", "Enqueuing message '{0}'", (object) message);
          service2.QueueMessage(requestContext, message, this.Url);
          flag = false;
        }
        catch (MessageQueueNotFoundException ex)
        {
          requestContext.TraceException(0, "Build", "Service", (Exception) ex);
        }
        finally
        {
          if (flag && message != null)
          {
            requestContext.Trace(0, TraceLevel.Verbose, "Build", "Service", "Closing message '{0}'", (object) message);
            message.Close();
          }
        }
      }
      TimeSpan timeout = TimeSpan.FromSeconds(5.0);
      TeamFoundationIdentity requestedFor1 = requestContext.GetService<TeamFoundationIdentityService>().ReadIdentity(requestContext, requestedFor, MembershipQuery.None, ReadIdentityOptions.None);
      bool flag1;
      do
      {
        flag1 = true;
        using (TeamFoundationDataReader foundationDataReader = service1.QueryBuildsByUri(requestContext, (IList<string>) list, (IList<string>) new string[2]
        {
          InformationTypes.ActivityTracking,
          InformationTypes.AgentScopeActivityTracking
        }, QueryOptions.Controllers, QueryDeletedOption.ExcludeDeleted, new Guid(), false))
        {
          List<BuildDetail> source = new List<BuildDetail>();
          BuildQueryResult buildQueryResult = foundationDataReader.Current<BuildQueryResult>();
          foreach (BuildDetail build1 in buildQueryResult.Builds)
          {
            BuildDetail build = build1;
            if (build != null)
            {
              if (!BuildCommonUtil.IsDefaultDateTime(build.FinishTime))
              {
                BuildController.AddStoppedByInformation(requestContext, build, requestedFor1);
              }
              else
              {
                BuildController controller = buildQueryResult.Controllers.First<BuildController>((Func<BuildController, bool>) (x => x.Uri == build.BuildControllerUri));
                if (buildQueryResult.ServiceHosts.First<BuildServiceHost>((Func<BuildServiceHost, bool>) (x => x.Uri == controller.ServiceHostUri)).Status == ServiceHostStatus.Online || controller.Version < 400)
                  source.Add(build);
                else
                  BuildController.StopBuildForcefully(requestContext, build, requestedFor1, AdministrationResources.BuildStoppedMachineOffline());
                requestContext.Trace(0, TraceLevel.Info, "Build", "Service", "Build '{0}' not stopped", (object) build.Uri);
              }
            }
          }
          if (source.Count == 0)
          {
            flag1 = true;
          }
          else
          {
            TimeSpan timeSpan = timeoutHelper.RemainingTime();
            if (timeSpan <= TimeSpan.Zero)
            {
              foreach (BuildDetail build in source)
                BuildController.StopBuildForcefully(requestContext, build, requestedFor1, AdministrationResources.BuildStoppedMachineNonResponsive());
              flag1 = true;
            }
            else
            {
              if (timeSpan < timeout)
                timeout = timeSpan;
              flag1 = false;
              Thread.Sleep(timeout);
              list.Clear();
              list.AddRange(source.Select<BuildDetail, string>((Func<BuildDetail, string>) (x => x.Uri)));
            }
          }
        }
      }
      while (!flag1);
      requestContext.TraceLeave(0, "BuildAdministration", "Service", nameof (StopBuilds));
    }

    internal static void StopBuildForcefully(
      IVssRequestContext requestContext,
      BuildDetail build,
      TeamFoundationIdentity requestedFor,
      string errorMessage,
      bool serverInitiated = false)
    {
      List<BuildUpdateOptions> updateOptions = new List<BuildUpdateOptions>();
      List<InformationChangeRequest> changes = new List<InformationChangeRequest>();
      while (build.Information.MoveNext())
      {
        InformationField informationField = (InformationField) null;
        BuildInformationNode current = build.Information.Current;
        foreach (InformationField field in current.Fields)
        {
          if (field.Name == InformationFields.FinishTime)
          {
            informationField = field;
            break;
          }
        }
        if (informationField != null && BuildCommonUtil.IsDefaultDateTime(CommonInformationHelper.ToDateTime(informationField.Value)))
        {
          List<InformationChangeRequest> informationChangeRequestList = changes;
          InformationEditRequest informationEditRequest = new InformationEditRequest();
          informationEditRequest.BuildUri = build.Uri;
          informationEditRequest.NodeId = current.NodeId;
          informationEditRequest.Fields.Add(new InformationField(InformationFields.FinishTime, CommonInformationHelper.ToString(DateTime.UtcNow)));
          informationEditRequest.Fields.Add(new InformationField(InformationFields.State, "Canceled"));
          informationEditRequest.Options = InformationEditOptions.MergeFields;
          informationChangeRequestList.Add((InformationChangeRequest) informationEditRequest);
        }
      }
      List<InformationChangeRequest> informationChangeRequestList1 = changes;
      InformationAddRequest informationAddRequest1 = new InformationAddRequest();
      informationAddRequest1.BuildUri = build.Uri;
      informationAddRequest1.NodeId = -1;
      informationAddRequest1.NodeType = InformationTypes.BuildError;
      informationAddRequest1.ParentId = 0;
      informationAddRequest1.Fields.Add(new InformationField(InformationFields.Timestamp, CommonInformationHelper.ToString(DateTime.UtcNow)));
      informationAddRequest1.Fields.Add(new InformationField(InformationFields.Message, errorMessage));
      informationChangeRequestList1.Add((InformationChangeRequest) informationAddRequest1);
      if (requestedFor != null)
      {
        List<InformationChangeRequest> informationChangeRequestList2 = changes;
        InformationAddRequest informationAddRequest2 = new InformationAddRequest();
        informationAddRequest2.BuildUri = build.Uri;
        informationAddRequest2.NodeId = -2;
        informationAddRequest2.NodeType = InformationTypes.BuildWarning;
        informationAddRequest2.ParentId = 0;
        informationAddRequest2.Fields.Add(new InformationField(InformationFields.Message, BuildTypeResource.BuildStoppedBy((object) requestedFor.DisplayName)));
        informationChangeRequestList2.Add((InformationChangeRequest) informationAddRequest2);
      }
      updateOptions.Add(new BuildUpdateOptions()
      {
        Fields = BuildUpdate.FinishTime | BuildUpdate.Status,
        Status = BuildStatus.Stopped,
        Uri = build.Uri
      });
      TeamFoundationBuildService service = requestContext.GetService<TeamFoundationBuildService>();
      if (changes.Count > 0)
        service.UpdateBuildInformation(requestContext, (IList<InformationChangeRequest>) changes);
      if (updateOptions.Count <= 0)
        return;
      service.UpdateBuilds(requestContext, (IList<BuildUpdateOptions>) updateOptions, serverInitiated);
    }

    private static void AddStoppedByInformation(
      IVssRequestContext requestContext,
      BuildDetail build,
      TeamFoundationIdentity requestedFor)
    {
      InformationAddRequest informationAddRequest = new InformationAddRequest();
      informationAddRequest.BuildUri = build.Uri;
      informationAddRequest.NodeId = -1;
      informationAddRequest.NodeType = InformationTypes.BuildWarning;
      informationAddRequest.Fields.Add(new InformationField(InformationFields.Message, BuildTypeResource.BuildStoppedBy((object) requestedFor.DisplayName)));
      try
      {
        requestContext.GetService<TeamFoundationBuildService>().UpdateBuildInformation(requestContext, (IList<InformationChangeRequest>) new InformationChangeRequest[1]
        {
          (InformationChangeRequest) informationAddRequest
        });
      }
      catch (InvalidBuildUriException ex)
      {
        requestContext.TraceException(0, "Build", "Service", (Exception) ex);
      }
    }

    internal static void StopUnstartedBuild(
      IVssRequestContext requestContext,
      string buildUri,
      string message)
    {
      TeamFoundationBuildService service = requestContext.GetService<TeamFoundationBuildService>();
      List<string> list = ((IEnumerable<string>) new string[1]
      {
        buildUri
      }).ToList<string>();
      BuildDetail build;
      using (TeamFoundationDataReader foundationDataReader = service.QueryBuildsByUri(requestContext, (IList<string>) list, (IList<string>) null, QueryOptions.None, QueryDeletedOption.ExcludeDeleted, new Guid(), false))
        build = foundationDataReader.Current<BuildQueryResult>().Builds.FirstOrDefault<BuildDetail>();
      if (build == null)
        return;
      BuildController.StopBuildForcefully(requestContext, build, (TeamFoundationIdentity) null, ResourceStrings.FailedToStartBuild((object) message), true);
    }

    internal void RequestIntermediateLogs(
      IVssRequestContext requestContext,
      string buildUri,
      string dropLocation,
      TeamFoundationIdentity requestorIdentity,
      Guid requestIdentifier)
    {
      requestContext.TraceEnter(0, "Build", "Service", nameof (RequestIntermediateLogs));
      if (this.Version < 400)
        throw new NotSupportedException(ResourceStrings.IntermediateLogsNotSupported((object) this.Name));
      TeamFoundationBuildResourceService service = requestContext.GetService<TeamFoundationBuildResourceService>();
      Message message = (Message) null;
      try
      {
        message = BuildControllerService.RequestIntermediateLogs(buildUri, dropLocation, requestorIdentity.DisplayName, requestIdentifier);
        message.Headers.To = new System.Uri(this.MessageQueueUrl);
        requestContext.Trace(0, TraceLevel.Verbose, "Build", "Service", "Enqueuing message '{0}'", (object) message);
        service.QueueMessage(requestContext.Elevate(), message, this.Url);
      }
      catch (MessageQueueNotFoundException ex)
      {
        requestContext.TraceException(0, "Build", "Service", (Exception) ex);
      }
      finally
      {
        if (message != null)
        {
          requestContext.Trace(0, TraceLevel.Verbose, "Build", "Service", "Closing message '{0}'", (object) message);
          message.Close();
        }
      }
      requestContext.TraceLeave(0, "Build", "Service", nameof (RequestIntermediateLogs));
    }

    void IValidatable.Validate(IVssRequestContext requestContext, ValidationContext context)
    {
      Microsoft.TeamFoundation.Build.Common.Validation.CheckValidControllerName(this.Name, false);
      Validation.CheckDescription("Description", this.Description, true);
      ArgumentValidation.CheckUri("ServiceHostUri", this.ServiceHostUri, "ServiceHost", false, (string) null);
      ArgumentValidation.CheckBound("MaxConcurrentBuilds", this.MaxConcurrentBuilds, 0, int.MaxValue);
      string customAssemblyPath = this.CustomAssemblyPath;
      Validation.CheckVersionControlPath("CustomAssemblyPath", ref customAssemblyPath, true);
      this.CustomAssemblyPath = customAssemblyPath;
      if (!string.IsNullOrEmpty(this.CustomAssemblyPath) && VersionControlPath.Equals(this.CustomAssemblyPath, "$/"))
        throw new ArgumentException(ResourceStrings.InvalidCustomAssemblyPath());
    }

    internal void TestControllerAvailability(
      IVssRequestContext requestContext,
      BuildServiceHost host)
    {
      using (BuildControllerServiceProxy2010 serviceProxy2010 = new BuildControllerServiceProxy2010(requestContext, host.GetUrlForService(this.Uri), host.RequireClientCertificates))
      {
        try
        {
          serviceProxy2010.TestConnection();
        }
        catch (Exception ex)
        {
          this.MarkControllerUnavailable(requestContext, ex);
        }
      }
    }

    private void MarkControllerOffline(IVssRequestContext requestContext, Exception e) => this.SetControllerStatus(requestContext, ControllerStatus.Offline, e.Message);

    private void MarkControllerUnavailable(IVssRequestContext requestContext, Exception e) => this.SetControllerStatus(requestContext, ControllerStatus.Unavailable, e.Message);

    private void MarkControllerAvailable(IVssRequestContext requestContext) => this.SetControllerStatus(requestContext, ControllerStatus.Available, AdministrationResources.BuildControllerReenabled());

    internal void SetControllerStatus(
      IVssRequestContext requestContext,
      ControllerStatus status,
      string statusMessage)
    {
      using (Microsoft.TeamFoundation.Build.Server.DataAccess.AdministrationComponent component = requestContext.CreateComponent<Microsoft.TeamFoundation.Build.Server.DataAccess.AdministrationComponent>("Build"))
        component.UpdateBuildControllers((IList<BuildControllerUpdateOptions>) new BuildControllerUpdateOptions[1]
        {
          new BuildControllerUpdateOptions()
          {
            Uri = this.Uri,
            Fields = BuildControllerUpdate.Status | BuildControllerUpdate.StatusMessage,
            Status = status,
            StatusMessage = AdministrationResources.BuildControllerStatusAutomaticallyChanged((object) statusMessage, (object) DateTime.UtcNow)
          }
        });
    }

    public override string ToString() => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "[BuildController Uri={0} Name={1} ServiceHostUri={2} Status={3}]", (object) this.Uri, (object) this.Name, (object) this.ServiceHostUri, (object) this.Status);
  }
}
