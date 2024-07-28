// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.DataServices.Compatibility.WorkItemUpdateElementExtensions
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server.DataServices, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 929F0284-16B2-4277-9F4A-B615689A77D1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Server.DataServices.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.DataServices.Compatibility
{
  internal static class WorkItemUpdateElementExtensions
  {
    public static XElement ToElement(
      this WorkItemUpdateResult result,
      WorkItemUpdateWrapper wrapper,
      IFieldTypeDictionary fieldTypeDictionary,
      bool hasErrors)
    {
      ArgumentUtility.CheckForNull<WorkItemUpdateWrapper>(wrapper, nameof (wrapper));
      if (!hasErrors)
      {
        ArgumentUtility.CheckForNull<WorkItemUpdateResult>(result, nameof (result));
        ArgumentUtility.CheckForNull<IFieldTypeDictionary>(fieldTypeDictionary, nameof (fieldTypeDictionary));
      }
      WorkItemUpdateWrapper.WorkItemWrapperUpdateType? nullable = wrapper.IsWorkItem ? wrapper.UpdateType : throw new ArgumentException("Object is not a work item");
      WorkItemUpdateWrapper.WorkItemWrapperUpdateType wrapperUpdateType1 = WorkItemUpdateWrapper.WorkItemWrapperUpdateType.Insert;
      if (!(nullable.GetValueOrDefault() == wrapperUpdateType1 & nullable.HasValue))
      {
        WorkItemUpdateWrapper.WorkItemWrapperUpdateType? updateType = wrapper.UpdateType;
        WorkItemUpdateWrapper.WorkItemWrapperUpdateType wrapperUpdateType2 = WorkItemUpdateWrapper.WorkItemWrapperUpdateType.Update;
        if (!(updateType.GetValueOrDefault() == wrapperUpdateType2 & updateType.HasValue))
          throw new ArgumentException("Unsupported updatetype specified");
      }
      WorkItemUpdateWrapper.WorkItemWrapperUpdateType? updateType1 = wrapper.UpdateType;
      WorkItemUpdateWrapper.WorkItemWrapperUpdateType wrapperUpdateType3 = WorkItemUpdateWrapper.WorkItemWrapperUpdateType.Insert;
      XElement element = new XElement((XName) (updateType1.GetValueOrDefault() == wrapperUpdateType3 & updateType1.HasValue ? "InsertWorkItem" : "UpdateWorkItem"));
      if (hasErrors)
        return element;
      element.Add((object[]) new XAttribute[3]
      {
        new XAttribute((XName) "ID", (object) result.Id),
        new XAttribute((XName) "Number", (object) "0"),
        new XAttribute((XName) "Revision", (object) result.Rev)
      });
      if (wrapper.HasTempId)
        element.Add((object) new XAttribute((XName) "TempID", (object) (result.UpdateId * -1)));
      XElement content1 = new XElement((XName) "ComputedColumns");
      content1.SetAttributeValue(XNamespace.Xml + "space", (object) "preserve");
      element.Add((object) content1);
      IEnumerable<string> strings1;
      if (!wrapper.HasTagUpdates)
        strings1 = wrapper.ComputedColumns.Where<string>((Func<string, bool>) (x => !TFStringComparer.WorkItemFieldReferenceName.Equals(x, "System.Tags")));
      else
        strings1 = wrapper.ComputedColumns.Union<string>((IEnumerable<string>) new string[1]
        {
          "System.Tags"
        });
      IEnumerable<string> strings2 = strings1;
      IEnumerable<string> updatedBoardFields = result.UpdatedBoardFields;
      if (updatedBoardFields.Any<string>())
        strings2 = strings2.Union<string>(updatedBoardFields);
      bool flag1 = false;
      foreach (string str in strings2)
      {
        FieldEntry field1;
        if (!TFStringComparer.WorkItemFieldReferenceName.Equals(str, "System.CreatedDate") && fieldTypeDictionary.TryGetField(str, out field1))
        {
          object fieldValue = (object) null;
          bool flag2 = result.Fields.TryGetValue(field1.FieldId.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo), out fieldValue);
          if (!flag2)
          {
            foreach (KeyValuePair<string, object> field2 in wrapper.Fields)
            {
              if (TFStringComparer.WorkItemFieldReferenceName.Equals(field2.Key, str))
              {
                flag2 = true;
                fieldValue = field2.Value;
                break;
              }
            }
          }
          if (flag2)
          {
            XElement content2 = new XElement((XName) "ComputedColumn", new object[3]
            {
              (object) new XAttribute((XName) "Column", (object) field1.ReferenceName),
              (object) new XAttribute((XName) "Type", (object) field1.GetWireTypeName()),
              (object) new XElement((XName) "Value", field1.ConvertFieldValueToWireFormat(fieldValue))
            });
            content1.Add((object) content2);
          }
          WorkItemUpdateWrapper.WorkItemWrapperUpdateType? updateType2 = wrapper.UpdateType;
          WorkItemUpdateWrapper.WorkItemWrapperUpdateType wrapperUpdateType4 = WorkItemUpdateWrapper.WorkItemWrapperUpdateType.Insert;
          if (updateType2.GetValueOrDefault() == wrapperUpdateType4 & updateType2.HasValue && !flag1 && (TFStringComparer.WorkItemFieldReferenceName.Equals(str, "System.AuthorizedDate") || TFStringComparer.WorkItemFieldReferenceName.Equals(str, "System.ChangedDate")))
          {
            flag1 = true;
            FieldEntry field3;
            if (fieldTypeDictionary.TryGetField("System.CreatedDate", out field3))
            {
              XElement content3 = new XElement((XName) "ComputedColumn", new object[3]
              {
                (object) new XAttribute((XName) "Column", (object) field3.ReferenceName),
                (object) new XAttribute((XName) "Type", (object) field3.GetWireTypeName()),
                (object) new XElement((XName) "Value", field1.ConvertFieldValueToWireFormat(fieldValue))
              });
              content1.Add((object) content3);
            }
          }
        }
      }
      foreach (KeyValuePair<string, object> field4 in (IEnumerable<KeyValuePair<string, object>>) result.Fields)
      {
        int result1;
        FieldEntry field5;
        if (int.TryParse(field4.Key, out result1) && fieldTypeDictionary.TryGetField(result1, out field5) && field5.IsHtml && !strings2.Contains<string>(field5.ReferenceName))
        {
          XElement content4 = new XElement((XName) "ComputedColumn", new object[3]
          {
            (object) new XAttribute((XName) "Column", (object) field5.ReferenceName),
            (object) new XAttribute((XName) "Type", (object) field5.GetWireTypeName()),
            (object) new XElement((XName) "Value", field5.ConvertFieldValueToWireFormat(field4.Value))
          });
          content1.Add((object) content4);
        }
      }
      if (result.CurrentExtensions != null && result.CurrentExtensions.Any<Guid>())
      {
        XElement content5 = new XElement((XName) "WorkItemTypeExtensions");
        element.Add((object) content5);
        XElement currentExtensionsElement1 = new XElement((XName) "Current");
        XElement currentExtensionsElement2 = new XElement((XName) "Attached");
        XElement currentExtensionsElement3 = new XElement((XName) "Detached");
        content5.Add((object) currentExtensionsElement1, (object) currentExtensionsElement2, (object) currentExtensionsElement3);
        WorkItemUpdateElementExtensions.AddExtensionElements(currentExtensionsElement1, result.CurrentExtensions);
        WorkItemUpdateElementExtensions.AddExtensionElements(currentExtensionsElement2, result.AttachedExtensions);
        WorkItemUpdateElementExtensions.AddExtensionElements(currentExtensionsElement3, result.DetachedExtensions);
      }
      return element;
    }

    public static XElement ToElement(this WorkItemLinkUpdateResult result)
    {
      ArgumentUtility.CheckForNull<WorkItemLinkUpdateResult>(result, nameof (result));
      bool flag = false;
      string name;
      switch (result.UpdateType)
      {
        case LinkUpdateType.Add:
          name = "InsertWorkItemLink";
          break;
        case LinkUpdateType.Update:
          name = "UpdateWorkItemLink";
          break;
        case LinkUpdateType.Delete:
          name = "DeleteWorkItemLink";
          flag = true;
          break;
        default:
          throw new ArgumentException("Unsupported LinkUpdateType");
      }
      return new XElement((XName) name, new object[8]
      {
        (object) new XAttribute((XName) "SourceID", (object) result.SourceWorkItemId),
        (object) new XAttribute((XName) "TargetID", (object) result.TargetWorkItemId),
        (object) new XAttribute((XName) "LinkType", (object) result.LinkType),
        (object) new XAttribute((XName) "fDeleted", flag ? (object) "True" : (object) "False"),
        (object) new XAttribute((XName) "ChangedBy", (object) result.ChangeBy),
        (object) new XAttribute((XName) "ChangedDate", (object) result.ChangedDate),
        (object) new XAttribute((XName) "ID", (object) result.SourceWorkItemId),
        (object) new XAttribute((XName) "Number", (object) 0)
      });
    }

    public static XElement ToElement(this WorkItemLinkUpdate linkUpdate)
    {
      ArgumentUtility.CheckForNull<WorkItemLinkUpdate>(linkUpdate, nameof (linkUpdate));
      string name;
      switch (linkUpdate.UpdateType)
      {
        case LinkUpdateType.Add:
          name = "InsertWorkItemLink";
          break;
        case LinkUpdateType.Update:
          name = "UpdateWorkItemLink";
          break;
        case LinkUpdateType.Delete:
          name = "DeleteWorkItemLink";
          break;
        default:
          throw new ArgumentException("Unsupported LinkUpdateType");
      }
      return new XElement((XName) name);
    }

    public static XElement ToElement(
      this WorkItemResourceLinkUpdateResult result,
      WorkItemUpdateWrapper wrapper,
      IFieldTypeDictionary fieldTypeDictionary)
    {
      ArgumentUtility.CheckForNull<WorkItemResourceLinkUpdateResult>(result, nameof (result));
      ArgumentUtility.CheckForNull<WorkItemUpdateWrapper>(wrapper, nameof (wrapper));
      ArgumentUtility.CheckForNull<IFieldTypeDictionary>(fieldTypeDictionary, nameof (fieldTypeDictionary));
      if (string.IsNullOrEmpty(result.CorrelationId))
        throw new ArgumentException("The result has a null correlationId, correlationId can not be null in a response from the server");
      WorkItemResourceLinkUpdate resourceLinkUpdate = (WorkItemResourceLinkUpdate) null;
      if (!wrapper.TryGetCorrelationObject<WorkItemResourceLinkUpdate>(result.CorrelationId, out resourceLinkUpdate))
        throw new InvalidOperationException("Can not locate the correlation object.  Correlation is required for processing responses from the server.");
      XElement element = (XElement) null;
      switch (result.UpdateType)
      {
        case LinkUpdateType.Add:
          int fieldId = TypeHelper.GetFieldFromResourceLinkType(resourceLinkUpdate.Type, fieldTypeDictionary).FieldId;
          ResourceLinkType? type = resourceLinkUpdate.Type;
          ResourceLinkType resourceLinkType = ResourceLinkType.Attachment;
          element = new XElement((XName) (type.GetValueOrDefault() == resourceLinkType & type.HasValue ? "InsertFile" : "InsertResourceLink"), new object[2]
          {
            (object) new XAttribute((XName) "FieldName", (object) fieldId),
            (object) new XAttribute((XName) "ID", (object) result.ResourceId)
          });
          if (!string.IsNullOrEmpty(resourceLinkUpdate.Location))
          {
            element.Add((object) new XAttribute((XName) "Location", (object) resourceLinkUpdate.Location));
            break;
          }
          break;
      }
      return element;
    }

    private static void AddExtensionElements(
      XElement currentExtensionsElement,
      IEnumerable<Guid> extensions)
    {
      foreach (Guid extension in extensions)
        currentExtensionsElement.Add((object) new XElement((XName) "WorkItemTypeExtension", (object) new XAttribute((XName) "id", (object) extension)));
    }
  }
}
