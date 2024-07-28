// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItemTemplateService
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.Azure.Boards.CssNodes;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types.Team;
using Microsoft.TeamFoundation.WorkItemTracking.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Internals;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Identity;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  public class WorkItemTemplateService : IWorkItemTemplateService, IVssFrameworkService
  {
    private const string c_AddTagsPseudoRefName = "System.Tags-Add";
    private const string c_RemoveTagsPseudoRefName = "System.Tags-Remove";

    void IVssFrameworkService.ServiceStart(IVssRequestContext requestContext)
    {
      if (!requestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection))
        throw new InvalidOperationException(FrameworkResources.UnexpectedHostType((object) requestContext.ServiceHost.HostType.ToString()));
    }

    void IVssFrameworkService.ServiceEnd(IVssRequestContext requestContext)
    {
    }

    public WorkItemTemplate CreateTemplate(
      IVssRequestContext requestContext,
      WorkItemTemplate template)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<WorkItemTemplate>(template, nameof (template));
      if (!Guid.Empty.Equals(template.Id))
        throw new InvalidWorkItemTemplateIdException(ServerResources.WorkItemTemplateIdNotNull());
      using (requestContext.TraceBlock(15116022, 15116023, "Services", nameof (WorkItemTemplateService), nameof (CreateTemplate)))
      {
        IDictionary<string, FieldEntry> workItemTypeFields;
        this.ValidateTemplate(requestContext, template, out workItemTypeFields);
        this.CheckWritePermission(requestContext, template.ProjectId, template.OwnerId);
        int num = requestContext.GetService<IVssRegistryService>().GetValue<int>(requestContext, (RegistryQuery) "/Service/WorkItemTracking/Settings/MaxNumberOfTemplatesPerWorkItemType", true, WorkItemTemplateComponent.MAX_TEMPLATES_PER_TYPE);
        IEnumerable<WorkItemTemplateDescriptor> templates = this.GetTemplates(requestContext, template.ProjectId, template.OwnerId, template.WorkItemTypeName);
        if (templates != null && templates.Count<WorkItemTemplateDescriptor>() >= num)
          throw new WorkItemTemplateLimitPerTypeExceededException(ServerResources.WorkItemTemplateLimitExceeded((object) num));
        Guid templateId = Guid.NewGuid();
        template = template.CloneWithId(templateId);
        WorkItemTemplate template1 = template.Clone();
        this.TranslateCssFieldValuesToRawIds(requestContext, template.ProjectId, template1.Fields);
        this.TranslateIdentityFieldValuesToRawIds(requestContext, template1.Fields, workItemTypeFields);
        using (WorkItemTemplateComponent component = requestContext.CreateComponent<WorkItemTemplateComponent>())
          component.CreateTemplate(template1);
        requestContext.GetService<WorkItemTemplateCacheService>().Remove(requestContext, Tuple.Create<Guid, Guid>(template.ProjectId, template.OwnerId));
        return template;
      }
    }

    public void UpdateTemplate(IVssRequestContext requestContext, WorkItemTemplate template)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<WorkItemTemplate>(template, nameof (template));
      using (requestContext.TraceBlock(15116024, 15116025, "Services", nameof (WorkItemTemplateService), nameof (UpdateTemplate)))
      {
        IDictionary<string, FieldEntry> workItemTypeFields;
        this.ValidateTemplate(requestContext, template, out workItemTypeFields);
        this.CheckWritePermission(requestContext, template.ProjectId, template.OwnerId);
        WorkItemTemplate template1 = template.Clone();
        this.TranslateCssFieldValuesToRawIds(requestContext, template.ProjectId, template1.Fields);
        this.TranslateIdentityFieldValuesToRawIds(requestContext, template1.Fields, workItemTypeFields);
        using (WorkItemTemplateComponent component = requestContext.CreateComponent<WorkItemTemplateComponent>())
        {
          try
          {
            component.UpdateTemplate(template1);
          }
          catch (WorkItemTemplateNotFoundException ex)
          {
            throw new WorkItemTemplateNotFoundException(ServerResources.WorkItemTemplateNotFound((object) template.Id));
          }
        }
        requestContext.GetService<WorkItemTemplateCacheService>().Remove(requestContext, Tuple.Create<Guid, Guid>(template.ProjectId, template.OwnerId));
      }
    }

    public WorkItemTemplate GetTemplate(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid templateId)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId));
      ArgumentUtility.CheckForEmptyGuid(templateId, nameof (templateId));
      using (requestContext.TraceBlock(15116026, 15116027, "Services", nameof (WorkItemTemplateService), nameof (GetTemplate)))
      {
        WorkItemTemplate template = (WorkItemTemplate) null;
        using (WorkItemTemplateComponent component = requestContext.CreateComponent<WorkItemTemplateComponent>())
          template = component.GetTemplate(projectId, templateId);
        if (template == null)
          throw new WorkItemTemplateNotFoundException(ServerResources.WorkItemTemplateNotFound((object) templateId));
        this.ResolveCssFieldValues(requestContext, projectId, template.Fields);
        if (this.DoesContainAnyGuidFieldValue(template))
        {
          string workItemTypeName;
          this.ResolveIdentityFieldValues(requestContext, template.Fields, this.GetWorkItemTypeFields(requestContext, template, out workItemTypeName));
          template.SetWorkItemTypeName(workItemTypeName);
        }
        return template;
      }
    }

    public IEnumerable<WorkItemTemplateDescriptor> GetTemplates(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid ownerId)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId));
      ArgumentUtility.CheckForEmptyGuid(ownerId, nameof (ownerId));
      using (requestContext.TraceBlock(15116028, 15116029, "Services", nameof (WorkItemTemplateService), nameof (GetTemplates)))
      {
        IEnumerable<WorkItemTemplateDescriptor> templates = (IEnumerable<WorkItemTemplateDescriptor>) null;
        WorkItemTemplateCacheService service = requestContext.GetService<WorkItemTemplateCacheService>();
        if (service.TryGetValue(requestContext, Tuple.Create<Guid, Guid>(projectId, ownerId), out templates))
          return templates;
        using (WorkItemTemplateComponent component = requestContext.CreateComponent<WorkItemTemplateComponent>())
          templates = component.GetTemplates(projectId, ownerId);
        if (templates == null)
          templates = Enumerable.Empty<WorkItemTemplateDescriptor>();
        service.Set(requestContext, Tuple.Create<Guid, Guid>(projectId, ownerId), templates);
        return templates;
      }
    }

    public IEnumerable<WorkItemTemplateDescriptor> GetTemplates(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid ownerId,
      string workItemTypeName)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId));
      ArgumentUtility.CheckForEmptyGuid(ownerId, nameof (ownerId));
      ArgumentUtility.CheckStringForNullOrEmpty(workItemTypeName, nameof (workItemTypeName));
      using (requestContext.TraceBlock(15116028, 15116029, "Services", nameof (WorkItemTemplateService), "GetTemplatesByWorkItemType"))
        return (IEnumerable<WorkItemTemplateDescriptor>) this.GetTemplates(requestContext, projectId, ownerId).Where<WorkItemTemplateDescriptor>((Func<WorkItemTemplateDescriptor, bool>) (t => TFStringComparer.WorkItemTypeName.Equals(t.WorkItemTypeName, workItemTypeName))).ToArray<WorkItemTemplateDescriptor>();
    }

    public void DeleteTemplate(IVssRequestContext requestContext, Guid projectId, Guid templateId)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId));
      ArgumentUtility.CheckForEmptyGuid(templateId, nameof (templateId));
      using (requestContext.TraceBlock(15116030, 15116031, "Services", nameof (WorkItemTemplateService), nameof (DeleteTemplate)))
      {
        WorkItemTemplate workItemTemplate = (WorkItemTemplate) null;
        using (WorkItemTemplateComponent component = requestContext.CreateComponent<WorkItemTemplateComponent>())
        {
          workItemTemplate = component.GetTemplate(projectId, templateId);
          if (workItemTemplate == null)
            return;
        }
        this.CheckWritePermission(requestContext, workItemTemplate.ProjectId, workItemTemplate.OwnerId);
        using (WorkItemTemplateComponent component = requestContext.CreateComponent<WorkItemTemplateComponent>())
          component.DeleteTemplate(projectId, templateId);
        requestContext.GetService<WorkItemTemplateCacheService>().Remove(requestContext, Tuple.Create<Guid, Guid>(workItemTemplate.ProjectId, workItemTemplate.OwnerId));
      }
    }

    public void DeleteTemplatesByOwner(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid ownerId)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId));
      ArgumentUtility.CheckForEmptyGuid(ownerId, nameof (ownerId));
      using (requestContext.TraceBlock(15116032, 15116033, "Services", nameof (WorkItemTemplateService), nameof (DeleteTemplatesByOwner)))
      {
        this.CheckWritePermission(requestContext, projectId, ownerId);
        using (WorkItemTemplateComponent component = requestContext.CreateComponent<WorkItemTemplateComponent>())
          component.DeleteTemplatesByOwner(projectId, ownerId);
        requestContext.GetService<WorkItemTemplateCacheService>().Remove(requestContext, Tuple.Create<Guid, Guid>(projectId, ownerId));
      }
    }

    public void DeleteAllTemplatesInProject(IVssRequestContext requestContext, Guid projectId)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId));
      using (requestContext.TraceBlock(15116034, 15116035, "Services", nameof (WorkItemTemplateService), nameof (DeleteAllTemplatesInProject)))
      {
        this.CheckProjectPermission(requestContext, projectId);
        using (WorkItemTemplateComponent component = requestContext.CreateComponent<WorkItemTemplateComponent>())
          component.DeleteAllTemplatesInProject(projectId);
      }
    }

    protected virtual void CheckWritePermission(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid ownerId)
    {
      using (requestContext.TraceBlock(15116020, 15116021, "Services", nameof (WorkItemTemplateService), nameof (CheckWritePermission)))
      {
        ITeamService service = requestContext.GetService<ITeamService>();
        WebApiTeam teamInProject = service.GetTeamInProject(requestContext, projectId, ownerId.ToString());
        if (teamInProject == null)
          throw new Microsoft.Azure.Devops.Teams.Service.TeamNotFoundException(ownerId.ToString());
        if (!service.HasTeamPermission(requestContext, teamInProject.Identity, 2, false))
          throw Microsoft.Azure.Devops.Teams.Service.TeamSecurityException.CreateGenericWritePermissionException();
      }
    }

    protected virtual void CheckProjectPermission(IVssRequestContext requestContext, Guid projectId)
    {
      IVssSecurityNamespace securityNamespace = requestContext.GetService<TeamFoundationSecurityService>().GetSecurityNamespace(requestContext, FrameworkSecurity.TeamProjectNamespaceId);
      string token = securityNamespace.NamespaceExtension.HandleIncomingToken(requestContext, securityNamespace, CommonStructureUtils.GetProjectUri(projectId));
      securityNamespace.CheckPermission(requestContext, token, TeamProjectPermissions.Delete);
    }

    internal void ResolveCssFieldValues(
      IVssRequestContext requestContext,
      Guid projectId,
      IDictionary<string, string> fieldValues)
    {
      this.ResolveCssFieldValue(requestContext, projectId, fieldValues, "System.AreaPath");
      this.ResolveCssFieldValue(requestContext, projectId, fieldValues, "System.IterationPath");
    }

    internal void ResolveCssFieldValue(
      IVssRequestContext requestContext,
      Guid projectId,
      IDictionary<string, string> fieldValues,
      string fieldReferenceName)
    {
      string input;
      Guid result;
      if (!fieldValues.TryGetValue(fieldReferenceName, out input) || !Guid.TryParse(input, out result))
        return;
      TreeNode treeNode = requestContext.GetService<WorkItemTrackingTreeService>().GetTreeNode(requestContext, projectId, result, false);
      if (treeNode != null)
      {
        fieldValues[fieldReferenceName] = treeNode.GetPath(requestContext);
      }
      else
      {
        requestContext.Trace(15116042, TraceLevel.Info, "Services", nameof (WorkItemTemplateService), string.Format("Could not resolve css node {0}", (object) result));
        fieldValues[fieldReferenceName] = ServerResources.WorkItemTemplateUnknownPath();
      }
    }

    internal void ResolveIdentityFieldValues(
      IVssRequestContext requestContext,
      IDictionary<string, string> fieldValues,
      IDictionary<string, FieldEntry> workItemTypeFields)
    {
      using (requestContext.TraceBlock(15116037, 15116038, "Services", nameof (WorkItemTemplateService), nameof (ResolveIdentityFieldValues)))
      {
        string[] array1 = workItemTypeFields.Where<KeyValuePair<string, FieldEntry>>((Func<KeyValuePair<string, FieldEntry>, bool>) (pair => pair.Value.IsIdentity)).Select<KeyValuePair<string, FieldEntry>, string>((Func<KeyValuePair<string, FieldEntry>, string>) (pair => pair.Key)).ToArray<string>();
        Dictionary<string, Guid> dictionary1 = new Dictionary<string, Guid>((IEqualityComparer<string>) TFStringComparer.WorkItemFieldReferenceName);
        foreach (string key in array1)
        {
          string input;
          Guid result;
          if (fieldValues.TryGetValue(key, out input) && Guid.TryParse(input, out result))
            dictionary1[key] = result;
        }
        Guid[] array2 = dictionary1.Values.Distinct<Guid>().ToArray<Guid>();
        if (array2.Length == 0)
          return;
        IList<Microsoft.VisualStudio.Services.Identity.Identity> source = requestContext.GetService<IdentityService>().ReadIdentities(requestContext, (IList<Guid>) array2, QueryMembership.None, (IEnumerable<string>) null);
        Dictionary<Guid, string> dictionary2 = source != null ? source.Where<Microsoft.VisualStudio.Services.Identity.Identity>((Func<Microsoft.VisualStudio.Services.Identity.Identity, bool>) (e => e != null)).ToDictionary<Microsoft.VisualStudio.Services.Identity.Identity, Guid, string>((Func<Microsoft.VisualStudio.Services.Identity.Identity, Guid>) (e => e.Id), (Func<Microsoft.VisualStudio.Services.Identity.Identity, string>) (e => e.GetLegacyDistinctDisplayName())) : new Dictionary<Guid, string>();
        foreach (KeyValuePair<string, Guid> keyValuePair in dictionary1)
        {
          string str;
          if (dictionary2.TryGetValue(keyValuePair.Value, out str))
          {
            fieldValues[keyValuePair.Key] = str;
          }
          else
          {
            requestContext.Trace(15116036, TraceLevel.Info, "Services", nameof (WorkItemTemplateService), string.Format("Could not resolve identity {0}", (object) keyValuePair.Value));
            fieldValues[keyValuePair.Key] = ServerResources.WorkItemTemplateUnknownUser();
          }
        }
      }
    }

    internal void TranslateCssFieldValuesToRawIds(
      IVssRequestContext requestContext,
      Guid projectId,
      IDictionary<string, string> fieldValues)
    {
      this.TranslateCssFieldValuesToRawId(requestContext, projectId, fieldValues, TreeStructureType.Area, "System.AreaPath");
      this.TranslateCssFieldValuesToRawId(requestContext, projectId, fieldValues, TreeStructureType.Iteration, "System.IterationPath");
    }

    internal void TranslateCssFieldValuesToRawId(
      IVssRequestContext requestContext,
      Guid projectId,
      IDictionary<string, string> fieldValues,
      TreeStructureType structureType,
      string fieldReferenceName)
    {
      string str;
      if (!fieldValues.TryGetValue(fieldReferenceName, out str))
        return;
      if (string.IsNullOrWhiteSpace(str))
        return;
      try
      {
        string relativePath = "";
        int startIndex = str.IndexOf('\\');
        if (startIndex >= 0)
          relativePath = str.Substring(startIndex);
        TreeNode treeNode = requestContext.GetService<WorkItemTrackingTreeService>().GetTreeNode(requestContext, projectId, structureType, relativePath, false);
        if (treeNode == null)
          return;
        if (!TFStringComparer.CssTreePathName.Equals(treeNode.GetPath(requestContext).Trim('\\'), str.Trim().Trim('\\')))
          return;
        fieldValues[fieldReferenceName] = treeNode.CssNodeId.ToString();
      }
      catch (Exception ex)
      {
        requestContext.TraceException(15116041, TraceLevel.Error, "Services", nameof (WorkItemTemplateService), ex);
      }
    }

    internal void TranslateIdentityFieldValuesToRawIds(
      IVssRequestContext requestContext,
      IDictionary<string, string> fieldValues,
      IDictionary<string, FieldEntry> workItemTypeFields)
    {
      using (requestContext.TraceBlock(15116039, 15116040, "Services", nameof (WorkItemTemplateService), nameof (TranslateIdentityFieldValuesToRawIds)))
      {
        string[] array1 = workItemTypeFields.Where<KeyValuePair<string, FieldEntry>>((Func<KeyValuePair<string, FieldEntry>, bool>) (pair => pair.Value.IsIdentity)).Select<KeyValuePair<string, FieldEntry>, string>((Func<KeyValuePair<string, FieldEntry>, string>) (pair => pair.Key)).ToArray<string>();
        Dictionary<string, string> dictionary1 = new Dictionary<string, string>((IEqualityComparer<string>) TFStringComparer.WorkItemFieldReferenceName);
        foreach (string key in array1)
        {
          string str;
          if (fieldValues.TryGetValue(key, out str) && !string.IsNullOrWhiteSpace(str))
            dictionary1[key] = str;
        }
        string[] array2 = dictionary1.Values.Distinct<string>((IEqualityComparer<string>) TFStringComparer.WorkItemIdentityName).ToArray<string>();
        if (array2.Length == 0)
          return;
        IdentityService service = requestContext.GetService<IdentityService>();
        List<Microsoft.VisualStudio.Services.Identity.Identity> identityList = new List<Microsoft.VisualStudio.Services.Identity.Identity>(array2.Length);
        foreach (string factorValue in array2)
          identityList.Add(service.ReadIdentities(requestContext, IdentitySearchFilter.General, factorValue, QueryMembership.None, (IEnumerable<string>) null).FirstOrDefault<Microsoft.VisualStudio.Services.Identity.Identity>());
        Dictionary<string, Guid> dictionary2 = new Dictionary<string, Guid>((IEqualityComparer<string>) TFStringComparer.WorkItemIdentityName);
        for (int index = 0; index < array2.Length; ++index)
        {
          if (identityList != null && identityList[index] != null)
            dictionary2[array2[index]] = identityList[index].Id;
        }
        foreach (KeyValuePair<string, string> keyValuePair in dictionary1)
        {
          Guid guid;
          if (dictionary2.TryGetValue(keyValuePair.Value, out guid))
            fieldValues[keyValuePair.Key] = guid.ToString();
        }
      }
    }

    private void ValidateTemplate(
      IVssRequestContext requestContext,
      WorkItemTemplate template,
      out IDictionary<string, FieldEntry> workItemTypeFields)
    {
      int currentMaxFieldValueLength = requestContext.GetService<IVssRegistryService>().GetValue<int>(requestContext, (RegistryQuery) "/Service/WorkItemTracking/Settings/TemplatesFieldValueMaxLength", true, WorkItemTemplateComponent.MAX_FIELD_VALUE_LENGTH);
      if (string.IsNullOrWhiteSpace(template.Name))
        throw new InvalidWorkItemTemplateNameException(ServerResources.WorkItemTemplateNameNullOrEmpty());
      if (template.Name.Length > WorkItemTemplateComponent.MAX_WORK_ITEM_TEMPLATE_NAME_LENGTH)
        throw new InvalidWorkItemTemplateNameException(ServerResources.WorkItemTemplateNameTooLong((object) WorkItemTemplateComponent.MAX_WORK_ITEM_TEMPLATE_NAME_LENGTH));
      if (string.IsNullOrWhiteSpace(template.WorkItemTypeName))
        throw new InvalidWorkItemTypeNameException(ServerResources.WorkItemTemplateTypeNameNullOrEmpty());
      if (template.WorkItemTypeName.Length > WorkItemTemplateComponent.MAX_WORK_ITEM_TYPE_NAME_LENGTH)
        throw new InvalidWorkItemTypeNameException(ServerResources.WorkItemTemplateWorkItemTypeNameTooLong((object) WorkItemTemplateComponent.MAX_WORK_ITEM_TYPE_NAME_LENGTH));
      if (!string.IsNullOrEmpty(template.Description) && template.Description.Length > WorkItemTemplateComponent.MAX_DESCRIPTION_LENGTH)
        throw new InvalidWorkItemTemplateDescriptionException(ServerResources.WorkItemTemplateDescriptionTooLong((object) WorkItemTemplateComponent.MAX_DESCRIPTION_LENGTH));
      if (template.Fields == null)
        throw new InvalidWorkItemTemplateFieldsException(ServerResources.WorkItemTemplateFieldsNotSpecified());
      KeyValuePair<string, string> keyValuePair1 = template.Fields.FirstOrDefault<KeyValuePair<string, string>>((Func<KeyValuePair<string, string>, bool>) (f => f.Key.Length > WorkItemTemplateComponent.MAX_FIELD_REF_NAME_LENGTH));
      if (keyValuePair1.Key != null)
        throw new InvalidWorkItemTemplateFieldRefNameException(ServerResources.WorkItemTemplateFieldRefNameTooLong((object) keyValuePair1.Key, (object) WorkItemTemplateComponent.MAX_FIELD_REF_NAME_LENGTH));
      KeyValuePair<string, string> keyValuePair2 = template.Fields.FirstOrDefault<KeyValuePair<string, string>>((Func<KeyValuePair<string, string>, bool>) (f => f.Key.Any<char>((Func<char, bool>) (c => char.IsWhiteSpace(c)))));
      if (keyValuePair2.Key != null)
        throw new InvalidWorkItemTemplateFieldRefNameException(ServerResources.WorkItemTemplateFieldRefNameContainsWhiteSpace((object) keyValuePair2.Key));
      KeyValuePair<string, string> keyValuePair3 = template.Fields.FirstOrDefault<KeyValuePair<string, string>>((Func<KeyValuePair<string, string>, bool>) (f => f.Value.Length > currentMaxFieldValueLength));
      if (keyValuePair3.Key != null)
        throw new InvalidWorkItemTemplateFieldValueException(ServerResources.WorkItemTemplateFieldValueTooLong((object) keyValuePair3.Key, (object) currentMaxFieldValueLength));
      string workItemTypeName = string.Empty;
      IDictionary<string, FieldEntry> fieldDictionary = this.GetWorkItemTypeFields(requestContext, template, out workItemTypeName);
      template.SetWorkItemTypeName(workItemTypeName);
      KeyValuePair<string, string> keyValuePair4 = template.Fields.FirstOrDefault<KeyValuePair<string, string>>((Func<KeyValuePair<string, string>, bool>) (fv => !fieldDictionary.ContainsKey(fv.Key) && !TFStringComparer.WorkItemFieldFriendlyName.Equals(fv.Key, "System.Tags-Add") && !TFStringComparer.WorkItemFieldFriendlyName.Equals(fv.Key, "System.Tags-Remove")));
      if (!keyValuePair4.Equals((object) new KeyValuePair<string, string>()))
        throw new InvalidWorkItemTemplateFieldRefNameException(ServerResources.FieldCouldNotBeFound((object) keyValuePair4.Key));
      foreach (string key in template.Fields.Keys.ToArray<string>())
      {
        FieldEntry fieldEntry = (FieldEntry) null;
        if (fieldDictionary.TryGetValue(key, out fieldEntry) && (fieldEntry.FieldType == InternalFieldType.History || fieldEntry.FieldType == InternalFieldType.Html && !WorkItemFieldData.c_SafeHtmlExcludedFields.Contains(key)))
          template.Fields[key] = SafeHtmlWrapper.MakeSafe(template.Fields[key]);
      }
      workItemTypeFields = fieldDictionary;
    }

    private IDictionary<string, FieldEntry> GetWorkItemTypeFields(
      IVssRequestContext requestContext,
      WorkItemTemplate template,
      out string workItemTypeName)
    {
      WorkItemType workItemType = (WorkItemType) null;
      workItemTypeName = template.WorkItemTypeName;
      if (!requestContext.GetService<IWorkItemTypeService>().TryGetWorkItemTypeByName(requestContext, template.ProjectId, template.WorkItemTypeName, out workItemType))
        throw new InvalidWorkItemTypeNameException(ServerResources.WorkItemTypeNotFound((object) template.ProjectId, (object) template.WorkItemTypeName));
      if (workItemType == null)
        return (IDictionary<string, FieldEntry>) new Dictionary<string, FieldEntry>();
      workItemTypeName = workItemType.Name;
      return (IDictionary<string, FieldEntry>) workItemType.GetFields(requestContext, true).ToDictionary<FieldEntry, string>((Func<FieldEntry, string>) (fe => fe.ReferenceName), (IEqualityComparer<string>) TFStringComparer.WorkItemFieldReferenceName);
    }

    private bool DoesContainAnyGuidFieldValue(WorkItemTemplate template) => template.Fields.Values.Any<string>((Func<string, bool>) (v => Guid.TryParse(v, out Guid _)));
  }
}
