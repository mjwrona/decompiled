// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Common.VersionControlPropertyKinds
// Assembly: Microsoft.TeamFoundation.VersionControl.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 156CCB01-0A1F-468C-A332-06DB9F9B179E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Common.dll

using System;

namespace Microsoft.TeamFoundation.VersionControl.Common
{
  public static class VersionControlPropertyKinds
  {
    public static readonly Guid Changeset = new Guid("{E68BE4B5-E432-4b89-9F19-32E8442BA8B2}");
    public static readonly Guid VersionedItem = new Guid("{980F2101-6236-48cb-924E-8DA58146585C}");
    public static readonly Guid Annotation = new Guid("{B3C2D349-12F8-4923-8BF4-BD11D7405A75}");
    public static readonly Guid Shelveset = new Guid("{83155693-55B8-4256-994D-E883753870C4}");
    public static readonly Guid ImmutableVersionedItem = new Guid("{13B7A2F5-4B3A-4725-9CF8-11A3A3D3AD66}");
    [Obsolete("This no longer works on TFS 2015 and up.", false)]
    public static readonly Guid PendingChange = new Guid("{0E833E7E-08D5-46A5-814A-AF16E69F3A1D}");
  }
}
