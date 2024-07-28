// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.XEventConstants
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;

namespace Microsoft.VisualStudio.Services.Cloud
{
  public static class XEventConstants
  {
    public static readonly Guid SessionBaseContainerId = new Guid("6969358E-6832-4859-9E55-F3310109F64D");
    public static readonly string ConfigurationRegistryRoot = "/Configuration/XEvent";
    public static readonly RegistryQuery EnableLoggingQuery = (RegistryQuery) RegistryHelpers.CombinePath(XEventConstants.ConfigurationRegistryRoot, "EnableLogging");
    public static readonly RegistryQuery StoragePathQuery = (RegistryQuery) RegistryHelpers.CombinePath(XEventConstants.ConfigurationRegistryRoot, "StoragePath");
  }
}
