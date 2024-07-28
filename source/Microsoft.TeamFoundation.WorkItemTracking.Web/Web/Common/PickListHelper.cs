// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Web.Common.PickListHelper
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CCDEBCB9-DB6B-41A2-8797-78C2455AE321
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Web.dll

using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Microsoft.VisualStudio.Services.Common;
using System;

namespace Microsoft.TeamFoundation.WorkItemTracking.Web.Common
{
  public class PickListHelper
  {
    internal static WorkItemPickListType GetPickListType(string type)
    {
      if (type.Equals("String", StringComparison.OrdinalIgnoreCase))
        return WorkItemPickListType.String;
      if (type.Equals("Integer", StringComparison.OrdinalIgnoreCase))
        return WorkItemPickListType.Integer;
      if (type.Equals("Double", StringComparison.OrdinalIgnoreCase))
        return WorkItemPickListType.Double;
      throw new VssPropertyValidationException(nameof (type), ResourceStrings.InvalidPickListType());
    }
  }
}
