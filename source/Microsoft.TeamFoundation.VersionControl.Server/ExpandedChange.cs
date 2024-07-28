// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.ExpandedChange
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using System;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  public class ExpandedChange
  {
    internal int requestIndex;
    internal ItemPathPair itemPathPair;
    internal int itemId;
    internal Guid dataspaceId;
    internal string localItem;
    internal ItemPathPair targetItemPathPair;
    internal LockLevel requiredLockLevel;
    internal int propertyId;

    internal string serverItem => this.itemPathPair.ProjectNamePath;

    internal string targetServerItem => this.targetItemPathPair.ProjectNamePath;
  }
}
