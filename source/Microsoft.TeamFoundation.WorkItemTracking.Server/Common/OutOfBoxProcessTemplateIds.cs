// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Common.OutOfBoxProcessTemplateIds
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Common
{
  public static class OutOfBoxProcessTemplateIds
  {
    public static readonly Guid Agile = new Guid("ADCC42AB-9882-485E-A3ED-7678F01F66BC");
    public static readonly Guid Scrum = new Guid("6B724908-EF14-45CF-84F8-768B5384DA45");
    public static readonly Guid Cmmi = new Guid("27450541-8E31-4150-9947-DC59F998FC01");
    public static readonly Guid Basic = new Guid("B8A3A935-7E91-48B8-A94C-606D37C3E9F2");
    private static HashSet<Guid> oobTypes = new HashSet<Guid>()
    {
      OutOfBoxProcessTemplateIds.Scrum,
      OutOfBoxProcessTemplateIds.Agile,
      OutOfBoxProcessTemplateIds.Cmmi,
      OutOfBoxProcessTemplateIds.Basic
    };

    public static bool IsOOBTypeProcess(Guid templateTypeId) => OutOfBoxProcessTemplateIds.oobTypes.Contains(templateTypeId);
  }
}
