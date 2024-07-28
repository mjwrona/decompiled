// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.DataServices.Compatibility.WorkItemUpdateDeserializer
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server.DataServices, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 929F0284-16B2-4277-9F4A-B615689A77D1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Server.DataServices.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Common.Metadata;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Xml.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.DataServices.Compatibility
{
  public class WorkItemUpdateDeserializer
  {
    public readonly int ClientVersion;

    public WorkItemUpdateDeserializer(int clientVersion) => this.ClientVersion = clientVersion;

    public WorkItemUpdateDeserializeResult Deserialize(
      IVssRequestContext requestContext,
      XElement packageElement)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<XElement>(packageElement, nameof (packageElement));
      return this.CreateUpdates(requestContext, packageElement);
    }

    private WorkItemUpdateDeserializeResult CreateUpdates(
      IVssRequestContext requestContext,
      XElement element)
    {
      if (requestContext == null)
        throw new ArgumentNullException(nameof (requestContext));
      if (element == null)
        throw new ArgumentNullException(nameof (element));
      List<XElement> nodesToRemove = new List<XElement>();
      WorkItemTrackingLinkService service = requestContext.GetService<WorkItemTrackingLinkService>();
      if (service == null)
        throw new ArgumentException("The link type dictionary can not be found");
      IFieldTypeDictionary fieldTypeDictionary = requestContext.GetService<WorkItemTrackingFieldService>().GetFieldsSnapshot(requestContext);
      int maxTempId = ((IEnumerable<int>) new int[1]).Concat<int>(element.Elements((XName) "InsertWorkItem").Select<XElement, int>((Func<XElement, int>) (x => x.Attribute<int>((XName) "TempID")))).Max();
      Dictionary<int, WorkItemUpdateWrapper> dictionary = element.Elements((XName) "InsertWorkItem").Select<XElement, WorkItemUpdateWrapper>((Func<XElement, WorkItemUpdateWrapper>) (x =>
      {
        WorkItemUpdateWrapper workItemUpdate = this.CreateWorkItemUpdate(requestContext, fieldTypeDictionary, x, ref maxTempId);
        nodesToRemove.Add(x);
        return workItemUpdate;
      })).ToDictionary<WorkItemUpdateWrapper, int>((Func<WorkItemUpdateWrapper, int>) (wup => wup.Id));
      foreach (WorkItemUpdateWrapper itemUpdateWrapper in element.Elements((XName) "UpdateWorkItem").Select<XElement, WorkItemUpdateWrapper>((Func<XElement, WorkItemUpdateWrapper>) (x =>
      {
        WorkItemUpdateWrapper workItemUpdate = this.CreateWorkItemUpdate(requestContext, fieldTypeDictionary, x, ref maxTempId);
        nodesToRemove.Add(x);
        return workItemUpdate;
      })))
        dictionary.Add(itemUpdateWrapper.Id, itemUpdateWrapper);
      foreach (XElement element1 in element.Elements().Where<XElement>((Func<XElement, bool>) (x => VssStringComparer.LinkName.Equals(x.Name.LocalName, "InsertWorkItemLink") || VssStringComparer.LinkName.Equals(x.Name.LocalName, "UpdateWorkItemLink") || VssStringComparer.LinkName.Equals(x.Name.LocalName, "DeleteWorkItemLink"))))
      {
        LinkUpdateType linkUpdateType = TypeHelper.GetLinkUpdateType(element1.Name.LocalName);
        int num1 = element1.Attribute<int>((XName) "SourceID");
        int key = element1.Attribute<int>((XName) "TargetID");
        int id = element1.Attribute<int>((XName) "LinkType");
        bool? nullable = element1.Attribute<bool?>((XName) "Lock");
        string str = element1.Attribute<string>((XName) "Comment");
        bool flag = false;
        WorkItemUpdateWrapper itemUpdateWrapper1;
        if (dictionary.TryGetValue(num1, out itemUpdateWrapper1))
        {
          flag = true;
          WorkItemUpdateWrapper itemUpdateWrapper2 = itemUpdateWrapper1;
          WorkItemLinkUpdate linkUpdate = new WorkItemLinkUpdate();
          linkUpdate.SourceWorkItemId = num1;
          linkUpdate.TargetWorkItemId = key;
          linkUpdate.LinkType = id;
          linkUpdate.Locked = nullable;
          linkUpdate.Comment = str;
          linkUpdate.UpdateType = linkUpdateType;
          linkUpdate.CorrelationId = element1.GetCorrelationId();
          itemUpdateWrapper2.AttachLink(linkUpdate);
        }
        if (dictionary.TryGetValue(key, out itemUpdateWrapper1))
        {
          flag = true;
          MDWorkItemLinkType linkTypeById = service.GetLinkTypeById(requestContext, id);
          if (linkTypeById == null)
            throw new InvalidOperationException("Can not locate the link information");
          int num2 = id == linkTypeById.ForwardId ? linkTypeById.ReverseId : linkTypeById.ForwardId;
          WorkItemUpdateWrapper itemUpdateWrapper3 = itemUpdateWrapper1;
          WorkItemLinkUpdate linkUpdate = new WorkItemLinkUpdate();
          linkUpdate.SourceWorkItemId = key;
          linkUpdate.TargetWorkItemId = num1;
          linkUpdate.LinkType = num2;
          linkUpdate.Locked = nullable;
          linkUpdate.Comment = str;
          linkUpdate.UpdateType = linkUpdateType;
          linkUpdate.CorrelationId = element1.GetCorrelationId();
          itemUpdateWrapper3.AttachLink(linkUpdate);
        }
        if (!flag)
        {
          itemUpdateWrapper1 = new WorkItemUpdateWrapper(num1, WorkItemUpdateWrapper.WorkItemWrapperUpdateType.Insert, (IEnumerable<string>) null, false, false, new bool?(), element1.GetCorrelationId());
          WorkItemUpdateWrapper itemUpdateWrapper4 = itemUpdateWrapper1;
          WorkItemLinkUpdate linkUpdate = new WorkItemLinkUpdate();
          linkUpdate.SourceWorkItemId = num1;
          linkUpdate.TargetWorkItemId = key;
          linkUpdate.LinkType = id;
          linkUpdate.Locked = nullable;
          linkUpdate.Comment = str;
          linkUpdate.UpdateType = linkUpdateType;
          linkUpdate.CorrelationId = element1.GetCorrelationId();
          itemUpdateWrapper4.AttachLink(linkUpdate);
          dictionary.Add(itemUpdateWrapper1.Id, itemUpdateWrapper1);
        }
        nodesToRemove.Add(element1);
      }
      for (int index = 0; index < nodesToRemove.Count; ++index)
        nodesToRemove[index].Remove();
      return new WorkItemUpdateDeserializeResult(dictionary, element);
    }

    private WorkItemUpdateWrapper CreateWorkItemUpdate(
      IVssRequestContext requestContext,
      IFieldTypeDictionary fieldTypeDictionary,
      XElement element,
      ref int tempId)
    {
      if (requestContext == null)
        throw new ArgumentNullException(nameof (requestContext));
      if (fieldTypeDictionary == null)
        throw new ArgumentNullException(nameof (fieldTypeDictionary));
      if (element == null)
        throw new ArgumentNullException(nameof (element));
      int? nullable = new int?();
      bool hasTempId = false;
      int id;
      WorkItemUpdateWrapper.WorkItemWrapperUpdateType updateType;
      switch (element.Name.LocalName)
      {
        case "InsertWorkItem":
          int num = element.Attribute<int>((XName) "TempID");
          if (num == 0)
            num = ++tempId;
          else
            hasTempId = true;
          id = -num;
          updateType = WorkItemUpdateWrapper.WorkItemWrapperUpdateType.Insert;
          break;
        case "UpdateWorkItem":
          id = element.Attribute<int>((XName) "WorkItemID");
          nullable = new int?(element.Attribute<int>((XName) "Revision"));
          updateType = WorkItemUpdateWrapper.WorkItemWrapperUpdateType.Update;
          break;
        default:
          throw new ArgumentException("Do not know how to handle this element yet");
      }
      bool? bypassRules = this.ProcessBypassRules(element);
      IEnumerable<string> computedColumns = this.ProcessComputedColumns(element);
      WorkItemUpdateWrapper workItemUpdate = new WorkItemUpdateWrapper(id, updateType, computedColumns, true, hasTempId, bypassRules, element.GetCorrelationId());
      if (nullable.HasValue)
        workItemUpdate.Rev = nullable.Value;
      this.ProcessColumns(element, workItemUpdate, fieldTypeDictionary);
      this.ProcessInsertText(element, workItemUpdate);
      this.ProcessResourceLinks(requestContext, element, workItemUpdate);
      this.ProcessRelationLinks(element, workItemUpdate);
      return workItemUpdate;
    }

    protected virtual IEnumerable<string> ProcessComputedColumns(XElement element)
    {
      XElement xelement = element != null ? element.Element((XName) "ComputedColumns") : throw new ArgumentNullException(nameof (element));
      return xelement != null ? (IEnumerable<string>) xelement.Elements((XName) "ComputedColumn").Select<XElement, string>((Func<XElement, string>) (computedColumn => computedColumn.Attribute<string>((XName) "Column"))).ToList<string>() : Enumerable.Empty<string>();
    }

    protected virtual void ProcessInsertText(XElement element, WorkItemUpdateWrapper workItem)
    {
      if (element == null)
        throw new ArgumentNullException(nameof (element));
      if (workItem == null)
        throw new ArgumentNullException(nameof (workItem));
      foreach (XElement element1 in element.Elements((XName) "InsertText").Where<XElement>((Func<XElement, bool>) (x => !TFStringComparer.TFSName.Equals(x.Name.ToString(), "System.Tags"))))
      {
        string fieldRef = element1.Attribute<string>((XName) "FieldName");
        if (string.IsNullOrEmpty(fieldRef))
          throw new InvalidOperationException("The field name is empty for the element");
        workItem.AddFieldUpdate(fieldRef, (object) element1.Value);
      }
    }

    protected virtual void ProcessColumns(
      XElement element,
      WorkItemUpdateWrapper workItem,
      IFieldTypeDictionary fieldTypeDictionary)
    {
      if (element == null)
        throw new ArgumentNullException(nameof (element));
      if (fieldTypeDictionary == null)
        throw new ArgumentNullException(nameof (fieldTypeDictionary));
      if (element.Element((XName) "Columns") == null)
        return;
      foreach (XElement element1 in element.Element((XName) "Columns").Elements((XName) "Column"))
      {
        string str1 = element1.Attribute<string>((XName) "Column");
        ColumnType columnType = TypeHelper.GetColumnType(element1.Attribute<string>((XName) "Type"));
        FieldEntry field = fieldTypeDictionary.GetField(str1);
        object obj;
        switch (columnType)
        {
          case ColumnType.ServerDateTime:
            obj = (object) new ServerDefaultFieldValue(ServerDefaultType.ServerDateTime);
            break;
          case ColumnType.ServerRandomGuid:
            obj = (object) new ServerDefaultFieldValue(ServerDefaultType.RandomGuid);
            break;
          default:
            string str2 = (string) null;
            if (element1.HasElements)
              str2 = element1.Value;
            obj = new Column(workItem.Id, field, columnType, str2).Value;
            break;
        }
        workItem.AddFieldUpdate(str1, obj);
      }
    }

    protected virtual void ProcessResourceLinks(
      IVssRequestContext requestContext,
      XElement element,
      WorkItemUpdateWrapper workItemUpdate)
    {
      if (element == null)
        throw new ArgumentNullException(nameof (element));
      if (workItemUpdate == null)
        throw new ArgumentNullException(nameof (workItemUpdate));
      List<WorkItemResourceLinkUpdate> list1 = element.Elements((XName) "InsertFile").Select<XElement, WorkItemResourceLinkUpdate>((Func<XElement, WorkItemResourceLinkUpdate>) (x =>
      {
        DateTime? nullable1 = WorkItemUpdateDeserializer.ConvertResourceLinkCreationDateTime(requestContext, x.Attribute<string>((XName) "CreationDate"));
        DateTime? nullable2 = WorkItemUpdateDeserializer.ConvertResourceLinkLastWriteDateTime(requestContext, x.Attribute<string>((XName) "LastWriteDate"));
        return new WorkItemResourceLinkUpdate()
        {
          SourceWorkItemId = workItemUpdate.Id,
          UpdateType = LinkUpdateType.Add,
          Type = new ResourceLinkType?(ResourceLinkType.Attachment),
          Name = x.Attribute<string>((XName) "OriginalName"),
          Location = x.Attribute<string>((XName) "FileName"),
          Length = x.Attribute<int?>((XName) "FileSize"),
          CreationDate = nullable1,
          LastModifiedDate = nullable2,
          Comment = x.Attribute<string>((XName) "Comment"),
          CorrelationId = x.GetCorrelationId()
        };
      })).ToList<WorkItemResourceLinkUpdate>();
      workItemUpdate.AttachResourceLinks((IEnumerable<WorkItemResourceLinkUpdate>) list1);
      List<WorkItemResourceLinkUpdate> list2 = element.Elements((XName) "RemoveFile").Select<XElement, WorkItemResourceLinkUpdate>((Func<XElement, WorkItemResourceLinkUpdate>) (x =>
      {
        return new WorkItemResourceLinkUpdate()
        {
          SourceWorkItemId = workItemUpdate.Id,
          UpdateType = LinkUpdateType.Delete,
          Type = new ResourceLinkType?(ResourceLinkType.Attachment),
          ResourceId = x.Attribute<int?>((XName) "FileID"),
          CorrelationId = x.GetCorrelationId()
        };
      })).ToList<WorkItemResourceLinkUpdate>();
      workItemUpdate.AttachResourceLinks((IEnumerable<WorkItemResourceLinkUpdate>) list2);
      List<WorkItemResourceLinkUpdate> list3 = element.Elements((XName) "InsertResourceLink").Select<XElement, WorkItemResourceLinkUpdate>((Func<XElement, WorkItemResourceLinkUpdate>) (x =>
      {
        string fieldName = x.Attribute<string>((XName) "FieldName");
        WorkItemResourceLinkUpdate resourceLinkUpdate1 = new WorkItemResourceLinkUpdate()
        {
          SourceWorkItemId = workItemUpdate.Id,
          UpdateType = LinkUpdateType.Add,
          Type = new ResourceLinkType?(TypeHelper.GetResourceLinkTypeFromField(fieldName)),
          Location = x.Attribute<string>((XName) "Location"),
          Comment = x.Attribute<string>((XName) "Comment"),
          CorrelationId = x.GetCorrelationId()
        };
        WorkItemResourceLinkUpdate resourceLinkUpdate2 = resourceLinkUpdate1;
        XElement element1 = x;
        ResourceLinkType? type = resourceLinkUpdate1.Type;
        ResourceLinkType resourceLinkType = ResourceLinkType.ArtifactLink;
        XName name = (XName) (type.GetValueOrDefault() == resourceLinkType & type.HasValue ? "LinkType" : "DisplayName");
        string str = element1.Attribute<string>(name);
        resourceLinkUpdate2.Name = str;
        return resourceLinkUpdate1;
      })).ToList<WorkItemResourceLinkUpdate>();
      workItemUpdate.AttachResourceLinks((IEnumerable<WorkItemResourceLinkUpdate>) list3);
      List<WorkItemResourceLinkUpdate> list4 = element.Elements((XName) "RemoveResourceLink").Select<XElement, WorkItemResourceLinkUpdate>((Func<XElement, WorkItemResourceLinkUpdate>) (x =>
      {
        return new WorkItemResourceLinkUpdate()
        {
          SourceWorkItemId = workItemUpdate.Id,
          UpdateType = LinkUpdateType.Delete,
          ResourceId = x.Attribute<int?>((XName) "LinkID"),
          CorrelationId = x.GetCorrelationId()
        };
      })).ToList<WorkItemResourceLinkUpdate>();
      workItemUpdate.AttachResourceLinks((IEnumerable<WorkItemResourceLinkUpdate>) list4);
      List<WorkItemResourceLinkUpdate> list5 = element.Elements((XName) "UpdateResourceLink").Select<XElement, WorkItemResourceLinkUpdate>((Func<XElement, WorkItemResourceLinkUpdate>) (x =>
      {
        return new WorkItemResourceLinkUpdate()
        {
          SourceWorkItemId = workItemUpdate.Id,
          ResourceId = new int?(x.Attribute<int>((XName) "LinkID")),
          UpdateType = LinkUpdateType.Update,
          Comment = x.Attribute<string>((XName) "Comment"),
          CorrelationId = x.GetCorrelationId()
        };
      })).ToList<WorkItemResourceLinkUpdate>();
      workItemUpdate.AttachResourceLinks((IEnumerable<WorkItemResourceLinkUpdate>) list5);
    }

    protected virtual void ProcessRelationLinks(
      XElement element,
      WorkItemUpdateWrapper workItemUpdate)
    {
      if (element == null)
        throw new ArgumentNullException(nameof (element));
      if (workItemUpdate == null)
        throw new ArgumentNullException(nameof (workItemUpdate));
      workItemUpdate.AttachLinks(element.Elements().Where<XElement>((Func<XElement, bool>) (x => x.Name.LocalName.Contains("Relation"))).Select<XElement, WorkItemLinkUpdate>((Func<XElement, WorkItemLinkUpdate>) (xlink =>
      {
        int num;
        try
        {
          num = xlink.Attribute<int>((XName) "WorkItemID");
        }
        catch (Exception ex)
        {
          throw new ArgumentException(DalResourceStrings.Get("UpdateMissingRequiredIdException"));
        }
        if (num == 0)
          throw new ArgumentException(DalResourceStrings.Get("UpdateMissingRequiredIdException"));
        WorkItemLinkUpdate workItemLinkUpdate = new WorkItemLinkUpdate()
        {
          SourceWorkItemId = workItemUpdate.Id,
          LinkType = 1,
          CorrelationId = xlink.GetCorrelationId(),
          TargetWorkItemId = num
        };
        switch (xlink.Name.LocalName)
        {
          case "CreateRelation":
            workItemLinkUpdate.UpdateType = LinkUpdateType.Add;
            break;
          case "RemoveRelation":
            workItemLinkUpdate.UpdateType = LinkUpdateType.Delete;
            break;
          case "UpdateRelation":
            workItemLinkUpdate.UpdateType = LinkUpdateType.Update;
            break;
          default:
            throw new ArgumentException("Can not locate the relation type");
        }
        return workItemLinkUpdate;
      })));
    }

    protected virtual bool? ProcessBypassRules(XElement element)
    {
      bool? nullable = new bool?();
      string str = element.Attribute<string>((XName) "BypassRules");
      if (!string.IsNullOrEmpty(str))
      {
        bool result = false;
        switch (str)
        {
          case "0":
          case "-1":
            nullable = new bool?(false);
            break;
          case "1":
            nullable = new bool?(true);
            break;
          default:
            if (bool.TryParse(str, out result))
            {
              nullable = new bool?(result);
              break;
            }
            break;
        }
      }
      return nullable;
    }

    private static DateTime? ConvertResourceLinkCreationDateTime(
      IVssRequestContext requestContext,
      string creationDate)
    {
      if (requestContext == null)
        throw new ArgumentNullException(nameof (requestContext));
      requestContext.Trace(900228, TraceLevel.Verbose, "DataAccessLayer", "NestedActions", "creationDate: {0}", (object) creationDate);
      if (string.IsNullOrWhiteSpace(creationDate))
        return new DateTime?();
      try
      {
        return new DateTime?(SqlBatchBuilder.ConvertToDateTime(creationDate));
      }
      catch (FormatException ex)
      {
        requestContext.Trace(900214, TraceLevel.Error, "DataAccessLayer", "NestedActions", "FormatException");
        throw new ArgumentException(DalResourceStrings.Get("UpdateInsertFileInvalidCreationDate"), "updateElement");
      }
      catch (OverflowException ex)
      {
        requestContext.Trace(900215, TraceLevel.Error, "DataAccessLayer", "NestedActions", "OverflowException");
        throw new ArgumentException(DalResourceStrings.Get("UpdateInsertFileInvalidCreationDate"), "updateElement");
      }
      catch (ArgumentOutOfRangeException ex)
      {
        requestContext.Trace(900216, TraceLevel.Error, "DataAccessLayer", "NestedActions", "ArgumentOutOfRangeException");
        throw new ArgumentException(DalResourceStrings.Get("UpdateInsertFileInvalidCreationDate"), "updateElement");
      }
    }

    private static DateTime? ConvertResourceLinkLastWriteDateTime(
      IVssRequestContext requestContext,
      string lastWriteDate)
    {
      if (requestContext == null)
        throw new ArgumentNullException(nameof (requestContext));
      requestContext.Trace(900229, TraceLevel.Verbose, "DataAccessLayer", "NestedActions", "lastWriteDate:{0}", (object) lastWriteDate);
      if (string.IsNullOrWhiteSpace(lastWriteDate))
        return new DateTime?();
      try
      {
        return new DateTime?(SqlBatchBuilder.ConvertToDateTime(lastWriteDate));
      }
      catch (FormatException ex)
      {
        requestContext.Trace(900217, TraceLevel.Error, "DataAccessLayer", "NestedActions", "FormatException");
        throw new ArgumentException(DalResourceStrings.Get("UpdateInsertFileInvalidCreationDate"), "updateElement");
      }
      catch (OverflowException ex)
      {
        requestContext.Trace(900218, TraceLevel.Error, "DataAccessLayer", "NestedActions", "OverflowException");
        throw new ArgumentException(DalResourceStrings.Get("UpdateInsertFileInvalidCreationDate"), "updateElement");
      }
      catch (ArgumentOutOfRangeException ex)
      {
        requestContext.Trace(900219, TraceLevel.Error, "DataAccessLayer", "NestedActions", "ArgumentOutOfRangeException");
        throw new ArgumentException(DalResourceStrings.Get("UpdateInsertFileInvalidCreationDate"), "updateElement");
      }
    }
  }
}
