// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Models.ResourceUsageModel
// Assembly: Microsoft.TeamFoundation.Server.WebAccess, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A2CCA8C5-6910-48A5-82D9-BDC1350B5B4D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.dll

using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Server.WebAccess.Models
{
  public class ResourceUsageModel
  {
    public string Title { get; set; }

    public string StartDate { get; set; }

    public string EndDate { get; set; }

    public IList<ResourceModel> Resources { get; set; }
  }
}
