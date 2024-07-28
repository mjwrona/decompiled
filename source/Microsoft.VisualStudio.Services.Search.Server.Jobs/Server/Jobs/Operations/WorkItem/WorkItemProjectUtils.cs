// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.Jobs.Operations.WorkItem.WorkItemProjectUtils
// Assembly: Microsoft.VisualStudio.Services.Search.Server.Jobs, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 23ACAECB-A4CB-4AA5-8366-092C41D8D4A8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Server.Jobs.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Entities.EntityProperties;
using System;

namespace Microsoft.VisualStudio.Services.Search.Server.Jobs.Operations.WorkItem
{
  internal class WorkItemProjectUtils
  {
    internal static bool HasWorkItemProjectChanged(
      IndexingUnit indexingUnit,
      TeamProjectReference teamProject,
      bool checkForProperties)
    {
      return !((ProjectWorkItemTFSAttributes) indexingUnit.TFSEntityAttributes).ProjectName.Equals(teamProject.Name, StringComparison.Ordinal) || (checkForProperties ? (((ProjectWorkItemTFSAttributes) indexingUnit.TFSEntityAttributes).ProjectName.Equals(indexingUnit.Properties?.Name, StringComparison.Ordinal) ? 1 : 0) : 1) == 0;
    }
  }
}
