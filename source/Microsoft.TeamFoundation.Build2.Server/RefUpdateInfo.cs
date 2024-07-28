// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.RefUpdateInfo
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Build2.Server
{
  public class RefUpdateInfo
  {
    public string RefName { get; set; }

    public string OldObjectId { get; set; }

    public string NewObjectId { get; set; }

    public string MergeRef { get; set; }

    public string MergeObjectId { get; set; }

    public List<string> CompleteFilesChangedList { get; set; }
  }
}
