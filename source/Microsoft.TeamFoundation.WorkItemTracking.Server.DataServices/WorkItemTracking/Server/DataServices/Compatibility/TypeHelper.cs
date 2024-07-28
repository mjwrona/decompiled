// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.DataServices.Compatibility.TypeHelper
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server.DataServices, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 929F0284-16B2-4277-9F4A-B615689A77D1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Server.DataServices.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems;
using Microsoft.VisualStudio.Services.Common;
using System;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.DataServices.Compatibility
{
  internal class TypeHelper
  {
    public static string GetWireTypeName(string dataTypeName)
    {
      ArgumentUtility.CheckForNull<string>(dataTypeName, nameof (dataTypeName));
      if (TFStringComparer.DataType.Equals(dataTypeName, "Int32"))
        return "Number";
      if (TFStringComparer.DataType.Equals(dataTypeName, "Double"))
        return "Double";
      if (TFStringComparer.DataType.Equals(dataTypeName, "DateTime"))
        return "DateTime";
      if (TFStringComparer.DataType.Equals(dataTypeName, "Boolean"))
        return "Boolean";
      return TFStringComparer.DataType.Equals(dataTypeName, "Guid") ? "UniqueIdentifier" : dataTypeName;
    }

    public static ColumnType GetColumnType(string dataTypeName)
    {
      ColumnType result = ColumnType.String;
      if (!string.IsNullOrEmpty(dataTypeName))
        Enum.TryParse<ColumnType>(TypeHelper.GetWireTypeName(dataTypeName), out result);
      return result;
    }

    public static ResourceLinkType GetResourceLinkTypeFromField(string fieldName)
    {
      ArgumentUtility.CheckForNull<string>(fieldName, nameof (fieldName));
      switch (fieldName)
      {
        case "System.AttachedFiles":
          return ResourceLinkType.Attachment;
        case "System.BISLinks":
          return ResourceLinkType.ArtifactLink;
        case "System.LinkedFiles":
          return ResourceLinkType.Hyperlink;
        default:
          throw new ArgumentException("Unsupported field type");
      }
    }

    public static FieldEntry GetFieldFromResourceLinkType(
      ResourceLinkType? resourceLinkType,
      IFieldTypeDictionary fieldTypeDictionary)
    {
      ArgumentUtility.CheckForNull<ResourceLinkType>(resourceLinkType, nameof (resourceLinkType));
      ArgumentUtility.CheckForNull<IFieldTypeDictionary>(fieldTypeDictionary, nameof (fieldTypeDictionary));
      switch (resourceLinkType.Value)
      {
        case ResourceLinkType.Attachment:
          return fieldTypeDictionary.GetField("System.AttachedFiles");
        case ResourceLinkType.Hyperlink:
          return fieldTypeDictionary.GetField("System.LinkedFiles");
        case ResourceLinkType.ArtifactLink:
          return fieldTypeDictionary.GetField("System.BISLinks");
        default:
          throw new ArgumentException("Invalid Resource Type");
      }
    }

    public static LinkUpdateType GetLinkUpdateType(string name)
    {
      if (name == null)
        throw new ArgumentNullException(nameof (name));
      if (name.Equals("UpdateWorkItemLink", StringComparison.InvariantCultureIgnoreCase))
        return LinkUpdateType.Update;
      return name.Equals("DeleteWorkItemLink", StringComparison.InvariantCultureIgnoreCase) ? LinkUpdateType.Delete : LinkUpdateType.Add;
    }
  }
}
