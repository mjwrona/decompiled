// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.ProvisioningService
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 815CF582-E66F-43C7-9B50-57E1C71BBC84
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer.dll

using Microsoft.Azure.Boards.CssNodes;
using Microsoft.Azure.Boards.ProcessTemplates;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql;
using Microsoft.TeamFoundation.WorkItemTracking.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Common.Provision;
using Microsoft.TeamFoundation.WorkItemTracking.Internals;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Microsoft.TeamFoundation.WorkItemTracking.Server.QueryItems;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Common.Internal;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  public class ProvisioningService : IProvisioningService, IVssFrameworkService
  {
    private readonly LinkingUtilitiesWrapper m_linkingUtilities;
    private readonly ServiceFactory<IDataAccessLayer> m_dalServiceFactory;
    private readonly ServiceFactory<IdentityService> m_identityServiceFactory;
    private readonly ServiceFactory<ITeamFoundationSecurityService> m_securityServiceFactory;
    private const int c_retryCount = 3;

    public ProvisioningService()
      : this(new LinkingUtilitiesWrapper(), (ServiceFactory<IDataAccessLayer>) (x => (IDataAccessLayer) new DataAccessLayerImpl(x)), (ServiceFactory<IdentityService>) (x => x.GetService<IdentityService>()), (ServiceFactory<ITeamFoundationSecurityService>) (x => (ITeamFoundationSecurityService) x.GetService<TeamFoundationSecurityService>()))
    {
    }

    public ProvisioningService(
      LinkingUtilitiesWrapper linkingUtilities,
      ServiceFactory<IDataAccessLayer> dalServiceFactory,
      ServiceFactory<IdentityService> identityServiceFactory,
      ServiceFactory<ITeamFoundationSecurityService> securityServiceFactory)
    {
      this.m_linkingUtilities = linkingUtilities;
      this.m_dalServiceFactory = dalServiceFactory;
      this.m_identityServiceFactory = identityServiceFactory;
      this.m_securityServiceFactory = securityServiceFactory;
    }

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public XmlDocument ExportWorkItemType(
      IVssRequestContext requestContext,
      Guid projectGuid,
      string typeName,
      ExportMask exportMask = ExportMask.ExportGlobalLists)
    {
      return WitExporter.ExportWorkItemType(requestContext, projectGuid, typeName, exportMask);
    }

    public XmlDocument ExportGlobalWorkflow(
      IVssRequestContext requestContext,
      Guid projectGuid,
      ExportMask exportMask = ExportMask.ExportGlobalLists)
    {
      return WitExporter.ExportGlobalWorkflow(requestContext, projectGuid, exportMask);
    }

    public void ImportWorkItemType(
      IVssRequestContext requestContext,
      int projectId,
      string methodologyName,
      XmlElement typeElement,
      ProvisioningActionType actionType = ProvisioningActionType.Import,
      bool overwrite = true,
      ProvisioningImportEventsCallback importEventCallback = null,
      bool isPromote = false)
    {
      requestContext.TraceEnter(900666, "Services", nameof (ProvisioningService), nameof (ImportWorkItemType));
      typeElement = ProvisioningService.GetXml(typeElement, InternalSchemaType.WorkItemType, importEventCallback).DocumentElement;
      this.InternalImportWorkItemTypesOrGlobalWorkflows(requestContext, projectId, methodologyName, (IEnumerable<XmlElement>) new XmlElement[1]
      {
        typeElement
      }, actionType, InternalSchemaType.WorkItemType, (overwrite ? 1 : 0) != 0, importEventCallback, (isPromote ? 1 : 0) != 0);
      requestContext.TraceLeave(900667, "Services", nameof (ProvisioningService), nameof (ImportWorkItemType));
    }

    public void ImportWorkItemType(
      IVssRequestContext requestContext,
      int projectId,
      string methodologyName,
      string definition,
      ProvisioningActionType actionType = ProvisioningActionType.Import,
      bool overwrite = true,
      ProvisioningImportEventsCallback importEventCallback = null,
      bool isPromote = false)
    {
      requestContext.TraceEnter(900668, "Services", nameof (ProvisioningService), nameof (ImportWorkItemType));
      XmlDocument xml = ProvisioningService.GetXml(definition, InternalSchemaType.WorkItemType, importEventCallback);
      this.InternalImportWorkItemTypesOrGlobalWorkflows(requestContext, projectId, methodologyName, (IEnumerable<XmlElement>) new XmlElement[1]
      {
        xml.DocumentElement
      }, actionType, InternalSchemaType.WorkItemType, (overwrite ? 1 : 0) != 0, importEventCallback, (isPromote ? 1 : 0) != 0);
      requestContext.TraceLeave(900669, "Services", nameof (ProvisioningService), nameof (ImportWorkItemType));
    }

    public void ImportWorkItemTypes(
      IVssRequestContext requestContext,
      int projectId,
      string methodologyName,
      IEnumerable<string> definitions,
      ProvisioningActionType actionType = ProvisioningActionType.Import,
      bool overwrite = false,
      ProvisioningImportEventsCallback importEventCallback = null,
      bool isPromote = false)
    {
      requestContext.TraceEnter(900693, "Services", nameof (ProvisioningService), nameof (ImportWorkItemTypes));
      this.InternalImportWorkItemTypesOrGlobalWorkflows(requestContext, projectId, methodologyName, definitions.Select<string, XmlElement>((Func<string, XmlElement>) (definition => ProvisioningService.GetXml(definition, InternalSchemaType.WorkItemType, importEventCallback).DocumentElement)), actionType, InternalSchemaType.WorkItemType, overwrite, importEventCallback, isPromote);
      requestContext.TraceLeave(900694, "Services", nameof (ProvisioningService), nameof (ImportWorkItemTypes));
    }

    public void DestroyWorkItemType(IVssRequestContext requestContext, string name, int projectId)
    {
      if (string.IsNullOrEmpty(name))
        throw new ArgumentNullException(nameof (name));
      requestContext.TraceEnter(900670, "Services", nameof (ProvisioningService), nameof (DestroyWorkItemType));
      Guid cssNodeId = requestContext.GetService<WorkItemTrackingTreeService>().LegacyGetTreeNode(requestContext, projectId).CssNodeId;
      string projectName = requestContext.GetService<IProjectService>().GetProjectName(requestContext.Elevate(), cssNodeId);
      XmlDocument xmlDocument = AdminUpdatePackageHelpers.BuildDestroyWorkItemTypeUpdatePackage(name, projectName);
      this.m_dalServiceFactory(requestContext).Update(xmlDocument.DocumentElement);
      requestContext.TraceLeave(900675, "Services", nameof (ProvisioningService), nameof (DestroyWorkItemType));
    }

    public void RenameWorkItemType(
      IVssRequestContext requestContext,
      string name,
      string newName,
      string projectName)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(name, nameof (name));
      ArgumentUtility.CheckStringForNullOrEmpty(newName, nameof (newName));
      ArgumentUtility.CheckStringForNullOrEmpty(projectName, nameof (projectName));
      Guid projectId = requestContext.GetService<IProjectService>().GetProjectId(requestContext.Elevate(), projectName);
      this.RenameWorkItemType(requestContext, projectId, name, newName);
    }

    public void RenameWorkItemType(
      IVssRequestContext requestContext,
      Guid projectGuid,
      string name,
      string newName)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(name, nameof (name));
      ArgumentUtility.CheckStringForNullOrEmpty(newName, nameof (newName));
      requestContext.TraceEnter(900671, "Services", nameof (ProvisioningService), nameof (RenameWorkItemType));
      requestContext.GetService<IWorkItemTypeService>().RenameWorkItemType(requestContext, projectGuid, name, newName);
      requestContext.TraceLeave(900676, "Services", nameof (ProvisioningService), nameof (RenameWorkItemType));
    }

    public virtual void ImportWorkItemLinkType(
      IVssRequestContext requestContext,
      string definition,
      bool insertsOnly,
      ProvisioningActionType action = ProvisioningActionType.Import)
    {
      requestContext.TraceEnter(900672, "Services", nameof (ProvisioningService), nameof (ImportWorkItemLinkType));
      XmlDocument xml = ProvisioningService.GetXml(definition, InternalSchemaType.WorkItemLinkType);
      this.ImportValidateWorkItemLinkTypeInternal(requestContext, xml.DocumentElement, insertsOnly, action);
      requestContext.TraceLeave(900677, "Services", nameof (ProvisioningService), nameof (ImportWorkItemLinkType));
    }

    public virtual void ImportGlobalWorkflow(
      IVssRequestContext requestContext,
      int projectId,
      string definition,
      ProvisioningActionType actionType = ProvisioningActionType.Import,
      bool overwrite = true,
      ProvisioningImportEventsCallback importEventCallback = null)
    {
      requestContext.TraceEnter(900668, "Services", nameof (ProvisioningService), nameof (ImportGlobalWorkflow));
      XmlDocument xml = ProvisioningService.GetXml(definition, InternalSchemaType.GlobalWorkflow, importEventCallback);
      this.InternalImportWorkItemTypesOrGlobalWorkflows(requestContext, projectId, (string) null, (IEnumerable<XmlElement>) new XmlElement[1]
      {
        xml.DocumentElement
      }, actionType, InternalSchemaType.GlobalWorkflow, (overwrite ? 1 : 0) != 0, importEventCallback);
      requestContext.TraceLeave(900669, "Services", nameof (ProvisioningService), nameof (ImportGlobalWorkflow));
    }

    public void ImportQueries(
      IVssRequestContext requestContext,
      IProcessTemplate template,
      XmlNode queriesNode,
      Uri projectUri,
      ProvisioningActionType action = ProvisioningActionType.Import)
    {
      requestContext.TraceEnter(900673, "Services", nameof (ProvisioningService), nameof (ImportQueries));
      if (queriesNode != null)
      {
        string path = ServerResources.Manager.GetString("SharedQueries", requestContext.ServiceHost.GetCulture(requestContext));
        ProvisioningService.ImportQueriesContext queryCtx = new ProvisioningService.ImportQueriesContext(requestContext, template, action);
        ServerQueryItem folder;
        if (action == ProvisioningActionType.Import)
        {
          string toolSpecificId = this.m_linkingUtilities.DecodeUri(projectUri.AbsoluteUri).ToolSpecificId;
          Guid guid = new Guid(toolSpecificId);
          ServerQueryItem serverQueryItem = new ServerQueryItem(guid);
          serverQueryItem.SecurityToken = "$/" + toolSpecificId;
          IProjectService service = requestContext.GetService<IProjectService>();
          Guid id;
          CommonStructureUtils.CheckAndNormalizeUri(projectUri.AbsoluteUri, nameof (projectUri), true, out id);
          IVssRequestContext requestContext1 = requestContext;
          Guid projectId = id;
          ProjectInfo project = service.GetProject(requestContext1, projectId);
          queryCtx.ProjectName = project.Name;
          queryCtx.ProjectGuid = guid;
          queryCtx.ProjectUri = projectUri;
          Microsoft.TeamFoundation.WorkItemTracking.Server.QueryItems.QueryItem queryByPath = requestContext.GetService<ITeamFoundationQueryItemService>().GetQueryByPath(requestContext, guid, path, new int?(0), false);
          folder = new ServerQueryItem(queryByPath.Id);
          folder.Existing.QueryName = queryByPath.Name;
          folder.Existing.ParentId = guid;
          folder.Existing.Parent = serverQueryItem;
          folder.IsLoaded = true;
        }
        else
        {
          folder = new ServerQueryItem(Guid.NewGuid());
          folder.Existing.QueryName = path;
          folder.Existing.ParentId = Guid.NewGuid();
          folder.IsLoaded = true;
          queryCtx.ProjectName = Guid.NewGuid().ToString("N");
        }
        this.ProcessQueryFolderContent(queryCtx, queriesNode, folder);
        if (action == ProvisioningActionType.Import)
        {
          if (queryCtx.AccessControlLists.Count == 0)
            this.SetDefaultPermissions(queryCtx, folder);
          using (PerformanceTimer.StartMeasure(requestContext, "ProvisioningService.ImportQueries.SetPermissions"))
            this.SetPermissions(queryCtx);
          Dictionary<Guid, ServerQueryItem> queries = new Dictionary<Guid, ServerQueryItem>();
          foreach (ServerQueryItem processedQueryItem in queryCtx.ProcessedQueryItems)
            queries.Add(processedQueryItem.Id, processedQueryItem);
          queries.Add(folder.Id, folder);
          using (PerformanceTimer.StartMeasure(requestContext, "ProvisioningService.ImportQueries.DalUpdate"))
          {
            QueryItemUpdatePackageHelpers updatePackageHelpers = new QueryItemUpdatePackageHelpers((IQueryProvisioningHelper) new ServerQueryProvisioningHelper(requestContext, projectUri, queries));
            this.m_dalServiceFactory(requestContext).Update(updatePackageHelpers.Translate().DocumentElement);
          }
        }
      }
      PerformanceTimer.SendCustomerIntelligenceData(requestContext);
      requestContext.TraceLeave(900678, "Services", nameof (ProvisioningService), nameof (ImportQueries));
    }

    public void InsertWorkItemTypeUsageWithRules(
      IVssRequestContext requestContext,
      Guid projectGuid,
      int projectid,
      string workItemTypeRefName,
      IDictionary<string, XElement> fieldRefNameToXMLMap)
    {
      ServerMetadataProvisioningHelper metadata = new ServerMetadataProvisioningHelper(requestContext);
      int workItemTypeId = ProvisioningService.GetWorkItemTypeId(requestContext, projectGuid, workItemTypeRefName);
      UpdatePackageData updatePackageData = new UpdatePackageData((IMetadataProvisioningHelper) metadata, new MetaIDFactory(), projectid, (string) null);
      UpdatePackageFieldCollection packageFieldCollection = new UpdatePackageFieldCollection(updatePackageData);
      Microsoft.TeamFoundation.WorkItemTracking.Common.Provision.UpdatePackage batch = new Microsoft.TeamFoundation.WorkItemTracking.Common.Provision.UpdatePackage(updatePackageData);
      UpdatePackageRuleContext context = new UpdatePackageRuleContext((IMetadataProvisioningHelper) metadata);
      context.Push(new UpdatePackageCondition(updatePackageData, updatePackageData.MetaIDFactory.NewMetaID(25), batch.InsertConstant(workItemTypeRefName, (MetaID) null)));
      foreach (string key in (IEnumerable<string>) fieldRefNameToXMLMap.Keys)
      {
        int fieldId = ProvisioningService.GetFieldId(requestContext, key);
        if (fieldId != 0 && workItemTypeId != 0)
        {
          batch.InsertFieldUsage((MetaID) fieldId);
          batch.InsertWorkItemTypeUsage((MetaID) workItemTypeId, (MetaID) fieldId);
        }
        XElement fieldRefNameToXml = fieldRefNameToXMLMap[key];
        UpdatePackageField field = packageFieldCollection.GetField(key);
        if (fieldRefNameToXml != null && field != null)
        {
          field.Update(fieldRefNameToXml.ToXmlElement());
          field.AddToUpdatePackage(context, false, batch);
        }
      }
      this.m_dalServiceFactory(requestContext).Update(batch.Xml.DocumentElement);
    }

    public void RenameField(
      IVssRequestContext requestContext,
      string referenceName,
      string newName)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<string>(referenceName, nameof (referenceName));
      this.RenameFields(requestContext, Enumerable.Repeat<KeyValuePair<string, string>>(new KeyValuePair<string, string>(referenceName, newName), 1));
    }

    public void RenameFields(
      IVssRequestContext requestContext,
      IEnumerable<KeyValuePair<string, string>> fields)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) fields, nameof (fields));
      IFieldTypeDictionary fieldDict = requestContext.WitContext().FieldDictionary;
      this.RenameFields(requestContext, fields.Select<KeyValuePair<string, string>, KeyValuePair<int, string>>((Func<KeyValuePair<string, string>, KeyValuePair<int, string>>) (f => new KeyValuePair<int, string>(fieldDict.GetField(f.Key).FieldId, f.Value))));
    }

    public void RenameField(IVssRequestContext requestContext, int fieldId, string newName)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (newName));
      this.RenameFields(requestContext, Enumerable.Repeat<KeyValuePair<int, string>>(new KeyValuePair<int, string>(fieldId, newName), 1));
    }

    public void RenameFields(
      IVssRequestContext requestContext,
      IEnumerable<KeyValuePair<int, string>> fields)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) fields, nameof (fields));
      Microsoft.TeamFoundation.WorkItemTracking.Common.Provision.UpdatePackage updatePackage = new Microsoft.TeamFoundation.WorkItemTracking.Common.Provision.UpdatePackage((IMetadataProvisioningHelper) new ServerMetadataProvisioningHelper(requestContext), new MetaIDFactory());
      foreach (KeyValuePair<int, string> field in fields)
      {
        string name = field.Value;
        if (!ValidationMethods.IsValidFieldName(name))
          throw new LegacyValidationException(InternalsResourceStrings.Format("ErrorInvalidFieldName", (object) name));
        updatePackage.UpdateField(field.Key, new Dictionary<string, object>()
        {
          ["Name"] = (object) name
        });
      }
      this.m_dalServiceFactory(requestContext).Update(updatePackage.Xml.DocumentElement);
    }

    public void DeleteField(IVssRequestContext requestContext, string referenceName)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<string>(referenceName, nameof (referenceName));
      this.DeleteFields(requestContext, Enumerable.Repeat<string>(referenceName, 1));
    }

    public void DeleteFields(IVssRequestContext requestContext, IEnumerable<string> fields)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) fields, nameof (fields));
      IFieldTypeDictionary fieldDict = requestContext.WitContext().FieldDictionary;
      this.DeleteFields(requestContext, fields.Select<string, int>((Func<string, int>) (f => fieldDict.GetField(f).FieldId)));
    }

    public void DeleteField(IVssRequestContext requestContext, int fieldId)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      this.DeleteFields(requestContext, Enumerable.Repeat<int>(fieldId, 1));
    }

    public void DeleteFields(IVssRequestContext requestContext, IEnumerable<int> fields)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) fields, nameof (fields));
      XmlElement forFieldDeletion = ProvisioningService.CreateUpdatePackageForFieldDeletion(fields);
      this.m_dalServiceFactory(requestContext).Update(forFieldDeletion);
    }

    private static XmlElement CreateUpdatePackageForFieldDeletion(IEnumerable<int> fields)
    {
      XmlDocument xmlDocument = new XmlDocument();
      XmlElement element1 = xmlDocument.CreateElement("Package");
      element1.SetAttribute("Product", string.Empty);
      xmlDocument.AppendChild((XmlNode) element1);
      foreach (int field in fields)
      {
        XmlElement element2 = xmlDocument.CreateElement("DeleteField");
        element2.SetAttribute("FieldID", XmlConvert.ToString(field));
        element1.AppendChild((XmlNode) element2);
      }
      return xmlDocument.DocumentElement;
    }

    public void ImportCategories(
      IVssRequestContext requestContext,
      int projectId,
      string definition,
      bool overwrite = true,
      ProvisioningActionType action = ProvisioningActionType.Import)
    {
      XmlDocument doc = !string.IsNullOrEmpty(definition) ? ProvisioningService.GetXml(definition, InternalSchemaType.Categories) : throw new ArgumentNullException(nameof (definition));
      if (action != ProvisioningActionType.Import)
        return;
      this.ImportCategories(requestContext, projectId, doc, overwrite);
    }

    public void ImportCategories(
      IVssRequestContext requestContext,
      int projectId,
      XmlDocument doc,
      bool overwrite = true)
    {
      if (doc == null)
        throw new ArgumentNullException(nameof (doc));
      requestContext.TraceEnter(900674, "Services", nameof (ProvisioningService), nameof (ImportCategories));
      IWorkItemTypeCategoryService workItemTypeCategoryService = requestContext.GetService<IWorkItemTypeCategoryService>();
      TreeNode projectNode = requestContext.WitContext().TreeService.LegacyGetTreeNode(projectId);
      this.ImportUpdatePackageWithRetry(requestContext, (Action) (() =>
      {
        WorkItemTrackingRequestContext trackingRequestContext = requestContext.WitContext();
        MetadataTable[] tableNames = new MetadataTable[3]
        {
          MetadataTable.WorkItemTypeCategories,
          MetadataTable.WorkItemTypeCategoryMembers,
          MetadataTable.WorkItemTypes
        };
        requestContext.Items["WorkItemTracking/Provisioning/MetadataSnapshot"] = (object) trackingRequestContext.MetadataDbStamps((IEnumerable<MetadataTable>) tableNames);
      }), (Func<XmlDocument>) (() =>
      {
        Dictionary<string, ServerWitCategory> dictionary1 = ProvisioningService.CreateCategoriesList(requestContext, projectId, doc).ToDictionary<ServerWitCategory, string>((Func<ServerWitCategory, string>) (c => c.ReferenceName));
        Dictionary<string, WorkItemTypeCategory> dictionary2 = workItemTypeCategoryService.LegacyGetWorkItemTypeCategories(requestContext, projectNode.CssNodeId, false).ToDictionary<WorkItemTypeCategory, string>((Func<WorkItemTypeCategory, string>) (c => c.ReferenceName));
        Microsoft.TeamFoundation.WorkItemTracking.Common.Provision.UpdatePackage uPackage = new Microsoft.TeamFoundation.WorkItemTracking.Common.Provision.UpdatePackage((IMetadataProvisioningHelper) new ServerMetadataProvisioningHelper(requestContext), new MetaIDFactory());
        if (overwrite)
        {
          foreach (string key in dictionary1.Keys)
            this.InsertWorkItemTypeCategoryToUpdatePackage(dictionary1[key], uPackage);
        }
        else
        {
          foreach (string key in dictionary1.Keys.Except<string>((IEnumerable<string>) dictionary2.Keys, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase))
            this.InsertWorkItemTypeCategoryToUpdatePackage(dictionary1[key], uPackage);
          foreach (string key in dictionary2.Keys.Except<string>((IEnumerable<string>) dictionary1.Keys, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase))
            uPackage.DestroyWorkItemTypeCategory(dictionary2[key].Id.Value);
          foreach (string key in dictionary2.Keys.Intersect<string>((IEnumerable<string>) dictionary1.Keys, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase))
          {
            ServerWitCategory serverWitCategory = dictionary1[key];
            WorkItemTypeCategory itemTypeCategory = dictionary2[key];
            if (!TFStringComparer.WorkItemTypeName.Equals(serverWitCategory.DefaultWorkItemType.Name, itemTypeCategory.DefaultWorkItemTypeName) || !StringComparer.OrdinalIgnoreCase.Equals(serverWitCategory.Name, itemTypeCategory.Name))
              uPackage.UpdateWorkItemTypeCategory(dictionary2[key].Id.Value, new Hashtable()
              {
                {
                  (object) "Name",
                  (object) serverWitCategory.Name
                },
                {
                  (object) "DefaultWorkItemTypeID",
                  (object) serverWitCategory.DefaultWorkItemType.Id
                }
              });
            foreach (string str in serverWitCategory.WorkItemTypes.Select<LegacyWorkItemType, string>((Func<LegacyWorkItemType, string>) (t => t.Name)).Except<string>(itemTypeCategory.WorkItemTypeCategoryMembers.Select<WorkItemTypeCategoryMember, string>((Func<WorkItemTypeCategoryMember, string>) (m => m.WorkItemTypeName)), (IEqualityComparer<string>) TFStringComparer.WorkItemTypeName))
            {
              string workItemTypeName = str;
              uPackage.InsertWorkItemTypeCategoryMember(new MetaIDFactory().NewMetaID(itemTypeCategory.Id.Value), serverWitCategory.WorkItemTypes.FirstOrDefault<LegacyWorkItemType>((Func<LegacyWorkItemType, bool>) (t => t.Name == workItemTypeName)).Id);
            }
            foreach (string str in itemTypeCategory.WorkItemTypeCategoryMembers.Select<WorkItemTypeCategoryMember, string>((Func<WorkItemTypeCategoryMember, string>) (m => m.WorkItemTypeName)).Except<string>(serverWitCategory.WorkItemTypes.Select<LegacyWorkItemType, string>((Func<LegacyWorkItemType, string>) (t => t.Name)), (IEqualityComparer<string>) TFStringComparer.WorkItemTypeName))
            {
              string workItemTypeName = str;
              uPackage.DeleteWorkItemTypeCategoryMember(itemTypeCategory.WorkItemTypeCategoryMembers.FirstOrDefault<WorkItemTypeCategoryMember>((Func<WorkItemTypeCategoryMember, bool>) (m => m.WorkItemTypeName == workItemTypeName)).Id);
            }
          }
        }
        return uPackage.Xml;
      }), overwrite);
      requestContext.TraceLeave(900679, "Services", nameof (ProvisioningService), nameof (ImportCategories));
    }

    private void ImportUpdatePackageWithRetry(
      IVssRequestContext requestContext,
      Action saveMetadataSnapshot,
      Func<XmlDocument> createUpdatePackageXml,
      bool overwrite = true)
    {
      int num = 2;
      while (true)
      {
        if (!overwrite)
          saveMetadataSnapshot();
        XmlDocument xmlDocument = createUpdatePackageXml();
        if (xmlDocument.DocumentElement.ChildNodes.Count != 0)
        {
          try
          {
            this.m_dalServiceFactory(requestContext).Update(xmlDocument.DocumentElement, overwrite);
            break;
          }
          catch (LegacyValidationException ex)
          {
            if (!overwrite && ex.ErrorId == 600178 && num > 0)
            {
              requestContext.Trace(900766, TraceLevel.Verbose, "Services", nameof (ProvisioningService), "Import operation failed because metadata stamp is not up to date. Retries left: {0}", (object) (num - 1));
              requestContext.ResetMetadataDbStamps();
              --num;
            }
            else
              throw;
          }
        }
        else
          break;
      }
    }

    private void InsertWorkItemTypeCategoryToUpdatePackage(
      ServerWitCategory category,
      Microsoft.TeamFoundation.WorkItemTracking.Common.Provision.UpdatePackage uPackage)
    {
      MetaID catId = uPackage.InsertWorkItemTypeCategory(category.ProjectId, category.ReferenceName, category.Name, category.DefaultWorkItemType.Id);
      foreach (LegacyWorkItemType workItemType in category.WorkItemTypes)
        uPackage.InsertWorkItemTypeCategoryMember(catId, workItemType.Id);
    }

    internal static List<ServerWitCategory> CreateCategoriesList(
      IVssRequestContext requestContext,
      int projectId,
      XmlDocument doc,
      bool skipWorkItemTypeValidation = false)
    {
      List<ServerWitCategory> categoriesList = new List<ServerWitCategory>(doc.DocumentElement.ChildNodes.Count);
      HashSet<string> stringSet1 = new HashSet<string>((IEqualityComparer<string>) requestContext.GetService<WorkItemTrackingConfigurationSettingService>().GetConfigurationInfo(requestContext).ServerStringComparer);
      HashSet<string> stringSet2 = new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      for (int i = 0; i < doc.DocumentElement.ChildNodes.Count; ++i)
      {
        XmlElement childNode = (XmlElement) doc.DocumentElement.ChildNodes[i];
        ServerWitCategory serverWitCategory = new ServerWitCategory(requestContext, projectId, childNode, skipWorkItemTypeValidation);
        if (stringSet2.Contains(serverWitCategory.ReferenceName))
          throw new LegacyValidationException(DalResourceStrings.Format("ErrorDuplicateCatRefName", (object) serverWitCategory.ReferenceName));
        if (stringSet1.Contains(serverWitCategory.Name))
          throw new LegacyValidationException(DalResourceStrings.Format("ErrorDuplicateCatName", (object) serverWitCategory.Name));
        stringSet2.Add(serverWitCategory.ReferenceName);
        stringSet1.Add(serverWitCategory.Name);
        categoriesList.Add(serverWitCategory);
      }
      return categoriesList;
    }

    private void ProcessQueryFolderContent(
      ProvisioningService.ImportQueriesContext queryCtx,
      XmlNode node,
      ServerQueryItem folder)
    {
      this.ProcessQueryPermissions(queryCtx, node, folder);
      foreach (XmlNode selectNode in node.SelectNodes("Query"))
        this.ProcessQuery(queryCtx, selectNode, folder);
      foreach (XmlNode selectNode in node.SelectNodes("QueryFolder"))
        this.ProcessQueryFolder(queryCtx, selectNode, folder);
    }

    private string GetQueryName(XmlNode queryNode)
    {
      XmlAttribute attribute = queryNode.Attributes["name"];
      if (attribute == null || string.IsNullOrWhiteSpace(attribute.Value))
        throw new LegacyValidationException(DalResourceStrings.Get("ErrorMissingQueryName"));
      QueryItemHelper.CheckNameIsValid(attribute.Value);
      return attribute.Value;
    }

    private string GetQueryText(
      XmlNode queryNode,
      ProvisioningService.ImportQueriesContext queryCtx)
    {
      XmlAttribute attribute = queryNode.Attributes["fileName"];
      if (attribute == null || string.IsNullOrWhiteSpace(attribute.Value))
        throw new LegacyValidationException(DalResourceStrings.Get("ErrorMissingQueryText"));
      Stream input = (Stream) null;
      try
      {
        input = queryCtx.ProcessTemplate.GetResource(attribute.Value);
        XmlNode xmlNode = input != null ? XmlUtility.GetDocument(input).SelectSingleNode("WorkItemQuery/Wiql") : throw new LegacyValidationException(DalResourceStrings.Format("ErrorQueryFileDoesntExist", (object) attribute.Value));
        string queryText = xmlNode != null && !string.IsNullOrWhiteSpace(xmlNode.InnerText) ? xmlNode.InnerText : throw new LegacyValidationException(DalResourceStrings.Format("ErrorQueryFileMalformed", (object) attribute.Value));
        if (queryCtx.ActionType == ProvisioningActionType.Import)
          queryText = queryText.Replace(ProjectCreationSupportedMacros.ProjectName, queryCtx.ProjectName);
        return queryText;
      }
      finally
      {
        input?.Dispose();
      }
    }

    private void ProcessQueryFolder(
      ProvisioningService.ImportQueriesContext queryCtx,
      XmlNode node,
      ServerQueryItem parent)
    {
      ServerQueryItem serverQueryItem = new ServerQueryItem(Guid.NewGuid());
      serverQueryItem.New.Parent = parent;
      serverQueryItem.New.ParentId = parent.Id;
      serverQueryItem.New.QueryName = this.GetQueryName(node);
      if (queryCtx.ActionType == ProvisioningActionType.Import)
        queryCtx.AddProcessedQueryItem(serverQueryItem);
      this.ProcessQueryFolderContent(queryCtx, node, serverQueryItem);
    }

    private void ValidateWiql(ProvisioningService.ImportQueriesContext queryCtx, string queryText)
    {
      Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node syntax = (Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node) Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Parser.ParseSyntax(queryText);
      syntax.Bind((IExternal) null, (NodeTableName) null, (NodeFieldName) null);
      this.ValidateNode(queryCtx, syntax);
    }

    private void ValidateNode(ProvisioningService.ImportQueriesContext queryCtx, Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node node)
    {
      if (node == null)
        return;
      switch (node.NodeType)
      {
        case NodeType.FieldName:
          NodeFieldName nodeFieldName = (NodeFieldName) node;
          if (!ValidationMethods.IsValidReferenceFieldName(nodeFieldName.Value))
            throw new LegacyValidationException(DalResourceStrings.Format("ErrorWiqlInvalidFieldName", (object) nodeFieldName.Value));
          break;
        case NodeType.FieldCondition:
          NodeFieldName left = ((NodeCondition) node).Left;
          if (TFStringComparer.WorkItemFieldReferenceName.Equals(left.Value, "System.TeamProject"))
          {
            this.ValidateClassificationNodeReference(queryCtx, TreeStructureType.None, ((NodeCondition) node).Right);
            break;
          }
          if (TFStringComparer.WorkItemFieldReferenceName.Equals(left.Value, "System.AreaPath"))
          {
            this.ValidateClassificationNodeReference(queryCtx, TreeStructureType.Area, ((NodeCondition) node).Right);
            break;
          }
          if (TFStringComparer.WorkItemFieldReferenceName.Equals(left.Value, "System.IterationPath"))
          {
            this.ValidateClassificationNodeReference(queryCtx, TreeStructureType.Iteration, ((NodeCondition) node).Right);
            break;
          }
          break;
      }
      for (int i = 0; i < node.Count; ++i)
      {
        if (node[i] != null)
          this.ValidateNode(queryCtx, node[i]);
      }
    }

    private void ValidateClassificationNodeReference(
      ProvisioningService.ImportQueriesContext queryCtx,
      TreeStructureType type,
      Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node node)
    {
      if (node.NodeType == NodeType.ValueList)
      {
        for (int i = 0; i < node.Count; ++i)
          this.ValidateClassificationNodeReference(queryCtx, type, node[i]);
      }
      else
      {
        if (string.IsNullOrEmpty(node.ConstStringValue))
          return;
        string a = node.ConstStringValue.Replace(ProjectCreationSupportedMacros.ProjectName, string.Empty);
        if (a == string.Empty)
          return;
        if (type == TreeStructureType.None)
        {
          if (!string.Equals(a, "@project", StringComparison.OrdinalIgnoreCase))
            throw new LegacyValidationException(DalResourceStrings.Format("ErrorQueryReferencesInvalidPath", (object) node.ConstStringValue));
        }
        else if (!queryCtx.GetAvailableClassificationNodePaths(type, (Func<IDictionary<TreeStructureType, ISet<string>>>) (() => (IDictionary<TreeStructureType, ISet<string>>) this.ParseClassificationPaths(queryCtx.ProcessTemplate))).Contains(a))
          throw new LegacyValidationException(DalResourceStrings.Format("ErrorQueryReferencesInvalidPath", (object) node.ConstStringValue));
      }
    }

    private Dictionary<TreeStructureType, ISet<string>> ParseClassificationPaths(
      IProcessTemplate template)
    {
      Stream input1 = (Stream) null;
      Stream input2 = (Stream) null;
      HashSet<string> areas = new HashSet<string>((IEqualityComparer<string>) TFStringComparer.CssTreeNodeName);
      HashSet<string> iterations = new HashSet<string>((IEqualityComparer<string>) TFStringComparer.CssTreeNodeName);
      try
      {
        input1 = template.GetResource("ProcessTemplate.xml");
        if (input1 != null)
        {
          XmlNode xmlNode = XmlUtility.GetDocument(input1).SelectSingleNode("ProcessTemplate/groups/group[@id='Classification']/taskList/@filename");
          if (xmlNode != null)
          {
            if (!string.IsNullOrWhiteSpace(xmlNode.Value))
            {
              input2 = template.GetResource(xmlNode.Value);
              if (input2 != null)
              {
                foreach (XmlNode selectNode in XmlUtility.GetDocument(input2).SelectNodes("tasks/task/taskXml/Nodes/Node/Children/Node"))
                  this.ParseClassificationNode(selectNode, string.Empty, (ISet<string>) areas, (ISet<string>) iterations);
              }
            }
          }
        }
      }
      finally
      {
        input1?.Dispose();
        input2?.Dispose();
      }
      return new Dictionary<TreeStructureType, ISet<string>>()
      {
        {
          TreeStructureType.Area,
          (ISet<string>) areas
        },
        {
          TreeStructureType.Iteration,
          (ISet<string>) iterations
        }
      };
    }

    private void ParseClassificationNode(
      XmlNode node,
      string parentPath,
      ISet<string> areas,
      ISet<string> iterations)
    {
      XmlAttribute attribute1 = node.Attributes["Name"];
      if (attribute1 == null)
        return;
      string parentPath1 = parentPath + "\\" + attribute1.Value;
      XmlAttribute attribute2 = node.Attributes["StructureType"];
      if (attribute2 != null)
      {
        if (TFStringComparer.CssStructureType.Equals(attribute2.Value.Trim(), "ProjectModelHierarchy"))
          areas.Add(parentPath1);
        else if (TFStringComparer.CssStructureType.Equals(attribute2.Value.Trim(), "ProjectLifecycle"))
          iterations.Add(parentPath1);
      }
      foreach (XmlNode selectNode in node.SelectNodes("Children/Node"))
        this.ParseClassificationNode(selectNode, parentPath1, areas, iterations);
    }

    private void ProcessQuery(
      ProvisioningService.ImportQueriesContext queryCtx,
      XmlNode node,
      ServerQueryItem parent)
    {
      ServerQueryItem queryItem = new ServerQueryItem(Guid.NewGuid());
      queryItem.New.Parent = parent;
      queryItem.New.ParentId = parent.Id;
      queryItem.New.QueryName = this.GetQueryName(node);
      queryItem.New.QueryText = this.GetQueryText(node, queryCtx);
      if (queryCtx.ActionType == ProvisioningActionType.Import)
        queryCtx.AddProcessedQueryItem(queryItem);
      else if (queryCtx.ActionType == ProvisioningActionType.Validate)
        this.ValidateWiql(queryCtx, queryItem.New.QueryText);
      this.ProcessQueryPermissions(queryCtx, node, queryItem);
    }

    private int ParsePermission(
      ProvisioningService.ImportQueriesContext queryCtx,
      XmlNode node,
      bool isFolder,
      string attributeName)
    {
      int permission = 0;
      if (node.Attributes[attributeName] != null)
      {
        string[] strArray = node.Attributes[attributeName].Value.Split(ProcessTemplateConstants.PermissionSplitChars, StringSplitOptions.RemoveEmptyEntries);
        IDictionary<string, int> availablePermission = queryCtx.GetAvailablePermission(isFolder, (Func<bool, IList<string>>) (isFolderLocal => (IList<string>) QueryItemMethods.GetAclMetadata(queryCtx.RequestContext, isFolderLocal ? "STORED_QUERY_FOLDER" : "STORED_QUERY").PermissionNames));
        foreach (string key in strArray)
        {
          int num;
          if (!availablePermission.TryGetValue(key, out num))
            throw new LegacyValidationException(DalResourceStrings.Format("ErrorInvalidQueryPermissionName", (object) key));
          permission |= 1 << num;
        }
      }
      return permission;
    }

    private void ValidateProjectIdentity(
      ProvisioningService.ImportQueriesContext queryCtx,
      string identityName)
    {
      if (!queryCtx.GetAvailableProjectIdentities((Func<ISet<string>>) (() =>
      {
        Stream input1 = (Stream) null;
        Stream input2 = (Stream) null;
        try
        {
          HashSet<string> stringSet = new HashSet<string>((IEqualityComparer<string>) VssStringComparer.IdentityName);
          input1 = queryCtx.ProcessTemplate.GetResource("ProcessTemplate.xml");
          if (input1 != null)
          {
            XmlNode xmlNode = XmlUtility.GetDocument(input1).SelectSingleNode("ProcessTemplate/groups/group[@id='Groups']/taskList/@filename");
            if (xmlNode != null && !string.IsNullOrWhiteSpace(xmlNode.Value))
            {
              input2 = queryCtx.ProcessTemplate.GetResource(xmlNode.Value);
              if (input2 != null)
              {
                foreach (XmlNode selectNode in XmlUtility.GetDocument(input2).SelectNodes("tasks/task/taskXml/groups/group/@name"))
                {
                  if (!string.IsNullOrWhiteSpace(selectNode.Value) && !selectNode.Value.StartsWith("@"))
                    stringSet.Add(selectNode.Value);
                }
              }
            }
          }
          return (ISet<string>) stringSet;
        }
        finally
        {
          input1?.Dispose();
          input2?.Dispose();
        }
      })).Contains(identityName))
        throw new LegacyValidationException(DalResourceStrings.Format("ErrorProjectGroupNotDefined", (object) identityName));
    }

    private IdentityDescriptor ParseIdentity(
      ProvisioningService.ImportQueriesContext queryCtx,
      XmlNode node)
    {
      XmlAttribute attribute = node.Attributes["identity"];
      string str1 = attribute != null && !string.IsNullOrWhiteSpace(attribute.Value) ? attribute.Value : throw new LegacyValidationException(DalResourceStrings.Format("ErrorMissingIdentity"));
      string[] strArray = str1.Split(new string[1]{ "\\" }, StringSplitOptions.RemoveEmptyEntries);
      if (strArray.Length == 2)
      {
        string str2 = strArray[0].TrimStart('[').TrimEnd(']');
        string str3 = strArray[1];
        if (str2 == "SERVER" && (str3 == ProjectCreationSupportedMacros.ProjectCollectionAdministratorsGroupName || str3 == ProjectCreationSupportedMacros.ProjectCollectionAdministratorsCompatibilityGroupName))
          return GroupWellKnownIdentityDescriptors.NamespaceAdministratorsGroup;
        IdentityService dentityService = this.m_identityServiceFactory(queryCtx.RequestContext);
        Microsoft.VisualStudio.Services.Identity.Identity identity1 = (Microsoft.VisualStudio.Services.Identity.Identity) null;
        if (str3 == ProjectCreationSupportedMacros.ProjectAdministratorsGroupName && (str2 == queryCtx.ProjectName || str2 == ProjectCreationSupportedMacros.ProjectName))
        {
          if (queryCtx.ActionType == ProvisioningActionType.Import)
          {
            Microsoft.VisualStudio.Services.Identity.Identity administratorsGroup = ProjectUtilities.GetProjectAdministratorsGroup(queryCtx.RequestContext, queryCtx.ProjectUri.AbsoluteUri);
            if (administratorsGroup != null)
              return administratorsGroup.Descriptor;
          }
          else if (queryCtx.ActionType == ProvisioningActionType.Validate)
            return (IdentityDescriptor) null;
        }
        if (str2 == ProjectCreationSupportedMacros.ProjectName)
        {
          if (queryCtx.ActionType == ProvisioningActionType.Import)
            str1 = str1.Replace(ProjectCreationSupportedMacros.ProjectName, queryCtx.ProjectUri.AbsoluteUri);
          else if (queryCtx.ActionType == ProvisioningActionType.Validate)
          {
            this.ValidateProjectIdentity(queryCtx, str3);
            return (IdentityDescriptor) null;
          }
        }
        IdentityDescriptor identity2;
        if (queryCtx.RequestContext.Items.TryGetValue<IdentityDescriptor>(ProjectUtilities.GetProjectScopedGroupCacheKey(queryCtx.ProjectUri.ToString(), str3), out identity2))
          return identity2;
        using (PerformanceTimer.StartMeasure(queryCtx.RequestContext, "ProvisioningService.ParseIdentity.ReadIdentity", str1 ?? string.Empty))
          identity1 = dentityService.ReadIdentities(queryCtx.RequestContext, IdentitySearchFilter.AccountName, str1, QueryMembership.None, (IEnumerable<string>) null).FirstOrDefault<Microsoft.VisualStudio.Services.Identity.Identity>();
        if (identity1 != null)
          return identity1.Descriptor;
      }
      else if (VssStringComparer.IdentityName.Equals(str1, ProjectCreationSupportedMacros.CreatorOwner))
        return queryCtx.RequestContext.UserContext;
      throw new LegacyValidationException(DalResourceStrings.Format("UnresolvableQueryPermissionIdentity", (object) attribute.Value));
    }

    private void ProcessQueryPermissions(
      ProvisioningService.ImportQueriesContext queryCtx,
      XmlNode node,
      ServerQueryItem item)
    {
      XmlNodeList xmlNodeList = node.SelectNodes("Permission");
      if (xmlNodeList.Count == 0)
        return;
      IAccessControlList accessControlList = (IAccessControlList) null;
      if (queryCtx.ActionType == ProvisioningActionType.Import)
        accessControlList = (IAccessControlList) new AccessControlList(item.SecurityToken, true);
      bool isFolder = item.New.QueryText == null;
      foreach (XmlNode node1 in xmlNodeList)
      {
        IdentityDescriptor identity = this.ParseIdentity(queryCtx, node1);
        int permission1 = this.ParsePermission(queryCtx, node1, isFolder, "allow");
        int permission2 = this.ParsePermission(queryCtx, node1, isFolder, "deny");
        if (permission1 == 0 && permission2 == 0)
          throw new LegacyValidationException(DalResourceStrings.Format("ErrorNoneQueryPermission", (object) item.QueryName));
        if ((permission1 & permission2) != 0)
          throw new LegacyValidationException(DalResourceStrings.Format("ErrorInvalidQueryPermission", (object) item.QueryName));
        if (queryCtx.ActionType == ProvisioningActionType.Import)
          accessControlList.SetAccessControlEntry((IAccessControlEntry) new Microsoft.TeamFoundation.Framework.Server.AccessControlEntry(identity, permission1, permission2), false);
      }
      if (queryCtx.ActionType != ProvisioningActionType.Import)
        return;
      queryCtx.AccessControlLists.Add(accessControlList);
    }

    private void SetPermissions(ProvisioningService.ImportQueriesContext queryCtx)
    {
      if (queryCtx.AccessControlLists.Count <= 0)
        return;
      this.m_securityServiceFactory(queryCtx.RequestContext).GetSecurityNamespace(queryCtx.RequestContext, QueryItemSecurityConstants.NamespaceGuid).SetAccessControlLists(queryCtx.RequestContext, (IEnumerable<IAccessControlList>) queryCtx.AccessControlLists);
    }

    private void SetDefaultPermissions(
      ProvisioningService.ImportQueriesContext queryCtx,
      ServerQueryItem folder)
    {
      IdentityService dentityService = this.m_identityServiceFactory(queryCtx.RequestContext);
      IVssSecurityNamespace securityNamespace = this.m_securityServiceFactory(queryCtx.RequestContext).GetSecurityNamespace(queryCtx.RequestContext, FrameworkSecurity.TeamProjectNamespaceId);
      Guid guid = queryCtx.ProjectGuid;
      string str1 = guid.ToString();
      guid = folder.Id;
      string str2 = guid.ToString();
      IAccessControlList accessControlList = (IAccessControlList) new AccessControlList("$/" + str1 + "/" + str2, true);
      queryCtx.AccessControlLists.Add(accessControlList);
      IVssRequestContext requestContext = queryCtx.RequestContext;
      string absoluteUri = queryCtx.ProjectUri.AbsoluteUri;
      Microsoft.VisualStudio.Services.Identity.Identity identity = dentityService.ReadIdentities(requestContext, IdentitySearchFilter.AdministratorsGroup, absoluteUri, QueryMembership.None, (IEnumerable<string>) null).FirstOrDefault<Microsoft.VisualStudio.Services.Identity.Identity>();
      if (identity == null)
        return;
      IdentityDescriptor descriptor = identity.Descriptor;
      IdentityDescriptor administratorsGroup = GroupWellKnownIdentityDescriptors.NamespaceAdministratorsGroup;
      accessControlList.SetAccessControlEntry((IAccessControlEntry) new Microsoft.TeamFoundation.Framework.Server.AccessControlEntry(descriptor, 31, 0), false);
      accessControlList.SetAccessControlEntry((IAccessControlEntry) new Microsoft.TeamFoundation.Framework.Server.AccessControlEntry(administratorsGroup, 31, 0), false);
      foreach (IAccessControlEntry accessControlEntry in securityNamespace.QueryAccessControlList(queryCtx.RequestContext, securityNamespace.NamespaceExtension.HandleIncomingToken(queryCtx.RequestContext, securityNamespace, queryCtx.ProjectUri.AbsoluteUri), (IEnumerable<IdentityDescriptor>) null, false).AccessControlEntries)
      {
        if (!IdentityDescriptorComparer.Instance.Equals(accessControlEntry.Descriptor, descriptor) && !IdentityDescriptorComparer.Instance.Equals(accessControlEntry.Descriptor, administratorsGroup))
        {
          int allow = (accessControlEntry.Allow & TeamProjectPermissions.GenericRead) != 0 ? 1 : 0;
          int deny = (accessControlEntry.Deny & TeamProjectPermissions.GenericRead) != 0 ? 1 : 0;
          accessControlList.SetAccessControlEntry((IAccessControlEntry) new Microsoft.TeamFoundation.Framework.Server.AccessControlEntry(accessControlEntry.Descriptor, allow, deny), false);
        }
      }
    }

    private void ImportValidateWorkItemLinkTypeInternal(
      IVssRequestContext requestContext,
      XmlElement typeElement,
      bool insertsOnly,
      ProvisioningActionType action)
    {
      XmlDocument xmlDocument = new WorkItemLinkTypeImporter((IMetadataProvisioningHelper) new ServerMetadataProvisioningHelper(requestContext)).Translate(typeElement, insertsOnly);
      if (xmlDocument == null || action != ProvisioningActionType.Import)
        return;
      this.m_dalServiceFactory(requestContext).Update(xmlDocument.DocumentElement);
    }

    internal static WITImporter GetWorkItemTypeImporter(
      IVssRequestContext requestContext,
      IMetadataProvisioningHelper provisioningHelper,
      int projectId,
      string methodologyName,
      IEnumerable<XmlElement> typeElements,
      bool isPromote = false)
    {
      WorkItemTrackingRequestContext trackingRequestContext = requestContext.WitContext();
      string processName = (string) null;
      bool flag1 = false;
      bool flag2 = false;
      bool flag3 = false;
      bool flag4 = false;
      IWorkItemTrackingProcessService service = requestContext.GetService<IWorkItemTrackingProcessService>();
      TreeNode node;
      ProcessDescriptor processDescriptor;
      if (trackingRequestContext != null && trackingRequestContext.TreeService.LegacyTryGetTreeNode(projectId, out node) && node != null && service.TryGetLatestProjectProcessDescriptor(requestContext, node.CssNodeId, out processDescriptor) && processDescriptor != null)
      {
        processName = processDescriptor.Name;
        flag1 = !processDescriptor.IsCustom;
        flag2 = !isPromote && !processDescriptor.IsCustom;
        flag3 = !isPromote && !processDescriptor.IsCustom;
        flag4 = !isPromote && !processDescriptor.IsCustom;
      }
      WITImporter itemTypeImporter = new WITImporter(provisioningHelper, projectId, methodologyName, processName, !flag1, !flag2, !flag3, !flag4);
      foreach (XmlNode typeElement1 in typeElements)
      {
        XmlElement typeElement2 = (XmlElement) typeElement1.SelectSingleNode(ProvisionTags.WorkItemType);
        itemTypeImporter.AddWorkItemTypeDefinition(typeElement2);
        itemTypeImporter.AddGlobalLists((XmlElement) typeElement2.SelectSingleNode(ProvisionTags.GlobalLists));
      }
      return itemTypeImporter;
    }

    internal static WITImporter GetGlobalWorkflowImporter(
      IVssRequestContext requestContext,
      IMetadataProvisioningHelper provisioningHelper,
      int projectId,
      IEnumerable<XmlElement> typeElements)
    {
      WITImporter workflowImporter = new WITImporter(provisioningHelper, projectId);
      foreach (XmlElement typeElement in typeElements)
      {
        workflowImporter.AddWorkItemTypeDefinition(typeElement);
        workflowImporter.AddGlobalLists((XmlElement) typeElement.SelectSingleNode(ProvisionTags.GlobalLists));
      }
      return workflowImporter;
    }

    internal static int GetFieldId(IVssRequestContext context, string fieldRefName)
    {
      FieldEntry field;
      return context.GetService<WorkItemTrackingFieldService>().TryGetField(context, fieldRefName, out field) && field != null ? field.FieldId : 0;
    }

    internal static int GetWorkItemTypeId(
      IVssRequestContext context,
      Guid projectGuid,
      string workItemTypeRefName)
    {
      WorkItemType typeByReferenceName = context.GetService<IWorkItemTypeService>().GetWorkItemTypeByReferenceName(context, projectGuid, workItemTypeRefName);
      return typeByReferenceName != null && typeByReferenceName.Id.HasValue ? typeByReferenceName.Id.Value : 0;
    }

    internal static void CleanDefaultValues(
      IEnumerable<XmlElement> typeElements,
      Func<string, string> cleanerFunction)
    {
      foreach (XmlNode typeElement in typeElements)
      {
        foreach (XmlElement selectNode in typeElement.SelectNodes("//FIELD[@type='HTML']/DEFAULT"))
        {
          if (selectNode.GetAttribute("from") == "value" && selectNode.HasAttribute("value"))
          {
            string attribute = selectNode.GetAttribute("value");
            string str = cleanerFunction(attribute);
            selectNode.SetAttribute("value", str);
          }
        }
      }
    }

    private void InternalImportWorkItemTypes(
      IVssRequestContext requestContext,
      int projectNodeId,
      string methodologyName,
      IEnumerable<XmlElement> typeElements,
      ProvisioningActionType actionType,
      bool overwrite,
      ProvisioningImportEventsCallback importEventCallback,
      out XmlDocument updatePackage,
      bool isPromote = false)
    {
      ServerMetadataProvisioningHelper provisioningHelper = new ServerMetadataProvisioningHelper(requestContext, importEventCallback);
      ProvisioningService.CleanDefaultValues(typeElements, (Func<string, string>) (s => SafeHtmlWrapper.MakeSafe(s)));
      WITImporter itemTypeImporter = ProvisioningService.GetWorkItemTypeImporter(requestContext, (IMetadataProvisioningHelper) provisioningHelper, projectNodeId, methodologyName, typeElements, isPromote);
      updatePackage = itemTypeImporter.Translate();
      if (actionType != ProvisioningActionType.Import)
        return;
      Guid cssNodeId = requestContext.GetService<WorkItemTrackingTreeService>().LegacyGetTreeNode(requestContext, projectNodeId).CssNodeId;
      requestContext.GetService<WorkItemTypeService>().CreateProjectWorkItemType(requestContext, cssNodeId);
      this.m_dalServiceFactory(requestContext).Update(updatePackage.DocumentElement, overwrite, itemTypeImporter.ProvisionRules);
    }

    private void InternalImportGlobalWorkflows(
      IVssRequestContext requestContext,
      int projectId,
      IEnumerable<XmlElement> typeElements,
      ProvisioningActionType actionType,
      bool overwrite,
      ProvisioningImportEventsCallback importEventCallback,
      out XmlDocument updatePackage)
    {
      ServerMetadataProvisioningHelper provisioningHelper = new ServerMetadataProvisioningHelper(requestContext, importEventCallback);
      WITImporter workflowImporter = ProvisioningService.GetGlobalWorkflowImporter(requestContext, (IMetadataProvisioningHelper) provisioningHelper, projectId, typeElements);
      updatePackage = workflowImporter.Translate();
      if (actionType != ProvisioningActionType.Import)
        return;
      this.m_dalServiceFactory(requestContext).Update(updatePackage.DocumentElement, overwrite);
    }

    private void InternalImportWorkItemTypesOrGlobalWorkflows(
      IVssRequestContext requestContext,
      int projectId,
      string methodologyName,
      IEnumerable<XmlElement> typeElements,
      ProvisioningActionType actionType,
      InternalSchemaType schemaType,
      bool overwrite,
      ProvisioningImportEventsCallback importEventCallback,
      bool isPromote = false)
    {
      XmlDocument updatePackage = (XmlDocument) null;
      try
      {
        if (schemaType == InternalSchemaType.WorkItemType)
        {
          this.InternalImportWorkItemTypes(requestContext, projectId, methodologyName, typeElements, actionType, overwrite, importEventCallback, out updatePackage, isPromote);
        }
        else
        {
          if (schemaType != InternalSchemaType.GlobalWorkflow)
            return;
          this.InternalImportGlobalWorkflows(requestContext, projectId, typeElements, actionType, overwrite, importEventCallback, out updatePackage);
        }
      }
      catch (LegacyValidationException ex)
      {
        requestContext.Trace(900749, TraceLevel.Info, "Services", nameof (ProvisioningService), "ErrorId: {0}", (object) ex.ErrorId);
        if (updatePackage != null)
          requestContext.Trace(900750, TraceLevel.Info, "Services", nameof (ProvisioningService), updatePackage.OuterXml);
        bool flag = false;
        try
        {
          switch (ex.ErrorId)
          {
            case 600006:
            case 600010:
            case 600011:
            case 600016:
            case 600027:
            case 600075:
            case 600076:
            case 600082:
            case 600160:
            case 600161:
            case 600162:
            case 600163:
            case 600164:
            case 600165:
            case 600166:
            case 600167:
            case 600168:
            case 600169:
            case 600170:
              requestContext.ResetMetadataDbStamps();
              flag = true;
              break;
          }
        }
        catch
        {
        }
        if (!flag)
          throw;
        else if (schemaType == InternalSchemaType.WorkItemType)
        {
          this.InternalImportWorkItemTypes(requestContext, projectId, methodologyName, typeElements, actionType, overwrite, importEventCallback, out updatePackage, isPromote);
        }
        else
        {
          if (schemaType != InternalSchemaType.GlobalWorkflow)
            return;
          this.InternalImportGlobalWorkflows(requestContext, projectId, typeElements, actionType, overwrite, importEventCallback, out updatePackage);
        }
      }
    }

    internal static XmlDocument GetXml(
      Stream stream,
      InternalSchemaType type,
      ProvisioningImportEventsCallback importEventCallback = null)
    {
      using (StreamReader stream1 = new StreamReader(stream))
        return ProvisioningService.GetXml((TextReader) stream1, type, importEventCallback);
    }

    internal static XmlDocument GetXml(
      string xml,
      InternalSchemaType schema,
      ProvisioningImportEventsCallback importEventCallback = null)
    {
      using (MemoryStream memoryStream = new MemoryStream())
      {
        using (StreamWriter streamWriter = new StreamWriter((Stream) memoryStream))
        {
          streamWriter.Write(xml);
          streamWriter.Flush();
          memoryStream.Seek(0L, SeekOrigin.Begin);
          return ProvisioningService.GetXml((Stream) memoryStream, schema, importEventCallback);
        }
      }
    }

    internal static XmlDocument GetXml(
      XmlElement element,
      InternalSchemaType schema,
      ProvisioningImportEventsCallback importEventCallback = null)
    {
      return ProvisioningService.GetXml(element.OuterXml, schema, importEventCallback);
    }

    internal static XmlDocument GetXml(
      TextReader stream,
      InternalSchemaType type,
      ProvisioningImportEventsCallback importEventCallback = null)
    {
      XmlReaderSettings settings = new XmlReaderSettings();
      settings.IgnoreComments = true;
      settings.IgnoreProcessingInstructions = true;
      settings.IgnoreWhitespace = true;
      settings.ValidationType = ValidationType.Schema;
      settings.ValidationFlags |= XmlSchemaValidationFlags.ReportValidationWarnings;
      settings.DtdProcessing = DtdProcessing.Prohibit;
      settings.XmlResolver = (XmlResolver) null;
      InternalSchemas.InitSchemaSet(type, settings.Schemas);
      ValidationEventHandler validationEventHandler = (ValidationEventHandler) null;
      if (importEventCallback != null)
      {
        validationEventHandler = (ValidationEventHandler) ((sender, args) =>
        {
          if (args.Severity != XmlSeverityType.Error)
            return;
          importEventCallback.RaiseEvent(args.Message);
        });
        settings.ValidationEventHandler += validationEventHandler;
      }
      try
      {
        using (XmlReader reader = XmlReader.Create(stream, settings))
        {
          XmlDocument xml = new XmlDocument();
          xml.Load(reader);
          if (xml.SchemaInfo.Validity == XmlSchemaValidity.NotKnown)
            throw new LegacyValidationException(InternalsResourceStrings.Format("ErrorInvalidXmlNamespace", (object) InternalSchemas.GetSchema(type).TargetNamespace, (object) xml.NamespaceURI));
          if (xml.SchemaInfo.Validity == XmlSchemaValidity.Invalid)
            throw new LegacyValidationException(InternalsResourceStrings.Get("ErrorInvalidXml"));
          return xml;
        }
      }
      finally
      {
        if (validationEventHandler != null)
          settings.ValidationEventHandler -= validationEventHandler;
      }
    }

    private class ImportQueriesContext
    {
      private List<ServerQueryItem> m_processesQueryItems = new List<ServerQueryItem>();
      private List<IAccessControlList> m_acls = new List<IAccessControlList>();
      private Dictionary<string, int>[] m_queryPermissions = new Dictionary<string, int>[2];
      private ISet<string>[] m_classificationNodePaths = new ISet<string>[2];
      private ISet<string> m_projectIdentities;

      public ImportQueriesContext(
        IVssRequestContext requestContext,
        IProcessTemplate processTemplate,
        ProvisioningActionType action)
      {
        this.RequestContext = requestContext;
        this.ProcessTemplate = processTemplate;
        this.ActionType = action;
      }

      public string ProjectName { get; set; }

      public Guid ProjectGuid { get; set; }

      public Uri ProjectUri { get; set; }

      public IVssRequestContext RequestContext { get; private set; }

      public IProcessTemplate ProcessTemplate { get; private set; }

      public ProvisioningActionType ActionType { get; private set; }

      public void AddProcessedQueryItem(ServerQueryItem queryItem) => this.m_processesQueryItems.Add(queryItem);

      public IEnumerable<ServerQueryItem> ProcessedQueryItems => (IEnumerable<ServerQueryItem>) this.m_processesQueryItems;

      public IList<IAccessControlList> AccessControlLists => (IList<IAccessControlList>) this.m_acls;

      public IDictionary<string, int> GetAvailablePermission(
        bool isFolder,
        Func<bool, IList<string>> func)
      {
        int index1 = isFolder ? 1 : 0;
        if (this.m_queryPermissions[index1] == null)
        {
          Dictionary<string, int> dictionary = new Dictionary<string, int>();
          IList<string> stringList = func(isFolder);
          for (int index2 = 0; index2 < stringList.Count; ++index2)
            dictionary[stringList[index2]] = index2;
          this.m_queryPermissions[index1] = dictionary;
        }
        return (IDictionary<string, int>) this.m_queryPermissions[index1];
      }

      public ISet<string> GetAvailableProjectIdentities(Func<ISet<string>> func)
      {
        if (this.m_projectIdentities == null)
          this.m_projectIdentities = func();
        return this.m_projectIdentities;
      }

      public ISet<string> GetAvailableClassificationNodePaths(
        TreeStructureType type,
        Func<IDictionary<TreeStructureType, ISet<string>>> func)
      {
        int index = type != TreeStructureType.Area ? 1 : 0;
        if (this.m_classificationNodePaths[index] == null)
        {
          IDictionary<TreeStructureType, ISet<string>> dictionary = func();
          this.m_classificationNodePaths[0] = dictionary[TreeStructureType.Area];
          this.m_classificationNodePaths[1] = dictionary[TreeStructureType.Iteration];
        }
        return this.m_classificationNodePaths[index];
      }
    }
  }
}
