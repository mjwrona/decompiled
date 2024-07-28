// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Boards.CssNodes.ChangedNodeInfo
// Assembly: Microsoft.Azure.Boards.CssNodes, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D887A041-2C68-42E5-BA83-E261159AB40A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.Boards.CssNodes.dll

using System;

namespace Microsoft.Azure.Boards.CssNodes
{
  public class ChangedNodeInfo
  {
    public string Uri { get; set; }

    public bool Deleted { get; set; }

    public string ProjectUri { get; set; }

    public string Name { get; set; }

    public string Type { get; set; }

    public string ParentUri { get; set; }

    public string ReclassifyUri { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? FinishDate { get; set; }
  }
}
