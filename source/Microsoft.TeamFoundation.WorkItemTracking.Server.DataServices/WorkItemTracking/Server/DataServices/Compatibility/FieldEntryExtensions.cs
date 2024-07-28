// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.DataServices.Compatibility.FieldEntryExtensions
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server.DataServices, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 929F0284-16B2-4277-9F4A-B615689A77D1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Server.DataServices.dll

using Microsoft.TeamFoundation.WorkItemTracking.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Xml;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.DataServices.Compatibility
{
  internal static class FieldEntryExtensions
  {
    public static string GetWireTypeName(this FieldEntry fieldEntry)
    {
      ArgumentUtility.CheckForNull<FieldEntry>(fieldEntry, nameof (fieldEntry));
      return TypeHelper.GetWireTypeName(fieldEntry.SystemType.Name);
    }

    public static object ConvertFieldValueToWireFormat(
      this FieldEntry fieldEntry,
      object fieldValue)
    {
      ArgumentUtility.CheckForNull<FieldEntry>(fieldEntry, nameof (fieldEntry));
      return fieldValue != null && fieldEntry.FieldType == InternalFieldType.DateTime && fieldValue is DateTime dateTime ? (object) XmlConvert.ToString(dateTime, "yyyy-MM-ddTHH:mm:ss.fff") : fieldValue;
    }
  }
}
