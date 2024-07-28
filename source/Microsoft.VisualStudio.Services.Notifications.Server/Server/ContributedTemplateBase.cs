// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.Server.ContributedTemplateBase
// Assembly: Microsoft.VisualStudio.Services.Notifications.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8ED1F52D-E567-4EFA-AAED-F2D9262BE9C2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.Server.dll

using Microsoft.VisualStudio.Services.WebApi;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Notifications.Server
{
  internal class ContributedTemplateBase
  {
    public string Id { get; set; }

    public Dictionary<string, Microsoft.VisualStudio.Services.Notifications.Server.TemplateFields> TemplateFields { get; set; }

    public MustacheExpression Criteria { get; set; }

    public int Rank { get; set; }
  }
}
