// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.PendingDelta
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  internal class PendingDelta
  {
    public string ServerItem;
    public int ItemId;
    public int VersionOld;
    public int VersionNew;
    public long FileLengthOld;
    public long FileLengthNew;
    internal int oldFileId;
    internal int newFileId;
    internal int retries;
  }
}
