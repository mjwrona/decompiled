// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.VersionedApiControllerCustomNameAttribute
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;

namespace Microsoft.TeamFoundation.Framework.Server
{
  [AttributeUsage(AttributeTargets.Class)]
  public class VersionedApiControllerCustomNameAttribute : Attribute
  {
    public VersionedApiControllerCustomNameAttribute(
      string area,
      string resourceName,
      int resourceVersion)
    {
      this.Area = area;
      this.ResourceName = resourceName;
      this.ResourceVersion = resourceVersion;
    }

    public VersionedApiControllerCustomNameAttribute() => this.ResourceVersion = 1;

    public string Area { get; set; }

    public string ResourceName { get; set; }

    public int ResourceVersion { get; set; }
  }
}
