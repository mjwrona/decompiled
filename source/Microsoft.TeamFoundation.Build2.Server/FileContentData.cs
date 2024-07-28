// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.FileContentData
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

namespace Microsoft.TeamFoundation.Build2.Server
{
  public class FileContentData
  {
    public string Path { get; set; }

    public string ObjectId { get; set; }

    public string Url { get; set; }

    public string Content { get; set; }

    public Change LastChange { get; set; }
  }
}
