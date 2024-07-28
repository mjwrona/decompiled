// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.CodeSense.Server.AggregateDescriptor
// Assembly: Microsoft.TeamFoundation.CodeSense.Server.Shared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 548902A5-AE61-4BC7-8D52-315B40AB5900
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.CodeSense.Server.Shared.dll

using System;

namespace Microsoft.TeamFoundation.CodeSense.Server
{
  public class AggregateDescriptor
  {
    public AggregateDescriptor(string path, int fileId, Guid projectGuid)
    {
      this.Path = path;
      this.FileId = fileId;
      this.ProjectGuid = projectGuid;
    }

    public string Path { get; set; }

    public Guid ProjectGuid { get; private set; }

    public int FileId { get; private set; }

    public override string ToString() => string.Format("' ProjectGuid: {0}, Path: {1}, FileId: {2} '", (object) this.ProjectGuid, (object) this.Path, (object) this.FileId);
  }
}
