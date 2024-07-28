// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Mention.Server.Data.MentionSourceContext
// Assembly: Microsoft.TeamFoundation.Mention.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C680EDB7-9FDC-4722-A198-4B5BA1B43B52
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Mention.Server.dll

using System;

namespace Microsoft.TeamFoundation.Mention.Server.Data
{
  internal class MentionSourceContext : IMentionSourceContext
  {
    public string Type { get; set; }

    public string Identifier { get; set; }

    public Guid ProjectGuid { get; set; }

    public string DisplayText { get; set; }

    public MentionActionType SupportedActionTypes => MentionActionType.WorkItem | MentionActionType.Save;

    public Guid Mentioner { get; set; }

    public string NormalizedId { get; set; }
  }
}
