// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.OnPremLicenseName
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public static class OnPremLicenseName
  {
    public static readonly Guid Standard = new Guid("8B71784C-27AB-4490-BB97-E699ED4123E1");
    public static readonly Guid Limited = new Guid("242A857E-50CE-43D9-BA9F-3AA82457D726");
    public static readonly Guid Full = new Guid("F29E17F1-60BD-44F0-AB2F-D67207EE9484");
    public static readonly Guid Enterprise = new Guid("519A4528-2BD6-4EA4-B3CB-5440C1AAEBC3");
  }
}
