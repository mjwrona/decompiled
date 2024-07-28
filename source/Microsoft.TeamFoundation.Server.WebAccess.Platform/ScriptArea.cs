// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.ScriptArea
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Platform, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A6A2C403-5081-466C-A570-9B50BFA8E213
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Platform.dll

using System;
using System.Collections.Generic;
using System.Resources;

namespace Microsoft.TeamFoundation.Server.WebAccess
{
  public class ScriptArea
  {
    public string ResourcesModulePath { get; set; }

    public string BaseModulePath { get; set; }

    public string Prefix { get; set; }

    public Dictionary<string, Func<ResourceManager>> RegisteredResources { get; set; }

    public ScriptArea(string baseModulePath, string resourcesModulePath)
    {
      this.BaseModulePath = baseModulePath;
      this.ResourcesModulePath = resourcesModulePath;
      this.RegisteredResources = new Dictionary<string, Func<ResourceManager>>((IEqualityComparer<string>) StringComparer.Ordinal);
    }

    public ScriptArea RegisterResource(string name, Func<ResourceManager> resourceGenerator)
    {
      this.RegisteredResources[name] = resourceGenerator;
      return this;
    }
  }
}
