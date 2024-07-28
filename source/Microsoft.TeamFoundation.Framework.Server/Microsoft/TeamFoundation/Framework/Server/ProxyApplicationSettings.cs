// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ProxyApplicationSettings
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class ProxyApplicationSettings : IApplicationSettings
  {
    public static readonly Guid ProxyDeploymentId = new Guid("{1F1A4969-6C30-4BAE-B529-BE9242C04A24}");

    public string this[string key] => (string) null;

    public string ConfigDbConnectionString => (string) null;

    public string ConfigDbPassword => (string) null;

    public string ConfigDbUserId => (string) null;

    public Guid InstanceId => ProxyApplicationSettings.ProxyDeploymentId;
  }
}
