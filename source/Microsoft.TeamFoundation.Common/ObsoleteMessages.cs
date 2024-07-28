// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.ObsoleteMessages
// Assembly: Microsoft.TeamFoundation.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E643B42-FE11-4FC2-A9D6-79417E26CF92
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Common.dll

using System.ComponentModel;

namespace Microsoft.TeamFoundation
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public static class ObsoleteMessages
  {
    public const string UseTfsConnectionProperty = "The TeamFoundationServer class is obsolete and so is this property. Use the TfsConnection property instead.";
    public const string UseTfsTeamProjectCollectionOverload = "The TeamFoundationServer class is obsolete and so is this method. Use the overload of this method that takes a TfsTeamProjectCollection object instead.";
    public const string UseTfsTeamProjectCollectionProperty = "The TeamFoundationServer class is obsolete and so is this property. Use the corresponding property of type TfsTeamProjectCollection instead.";
  }
}
