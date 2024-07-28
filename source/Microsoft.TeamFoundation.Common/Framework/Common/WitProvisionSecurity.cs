// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Common.WitProvisionSecurity
// Assembly: Microsoft.TeamFoundation.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E643B42-FE11-4FC2-A9D6-79417E26CF92
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Common.dll

using System;
using System.ComponentModel;

namespace Microsoft.TeamFoundation.Framework.Common
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public static class WitProvisionSecurity
  {
    public static readonly Guid NamespaceId = new Guid("5A6CD233-6615-414D-9393-48DBB252BD23");
    public const string RootToken = "$";
    public const string Separator = "/";
    public const int Admin = 1;
    public const int ManageLinkTypes = 2;
  }
}
