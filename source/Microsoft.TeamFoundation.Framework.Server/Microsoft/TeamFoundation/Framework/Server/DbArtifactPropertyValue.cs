// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.DbArtifactPropertyValue
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal sealed class DbArtifactPropertyValue
  {
    public int SequenceId { get; set; }

    public byte[] ArtifactId { get; set; }

    public string Moniker { get; set; }

    public int InternalKindId { get; set; }

    public int Version { get; set; }

    public int RequestedVersion { get; set; }

    public string PropertyName { get; set; }

    public object Value { get; set; }

    public PropertyTypeMatch TypeMatch { get; set; }

    public DateTime? ChangedDate { get; set; }

    public Guid? ChangedBy { get; set; }

    public Guid DataspaceIdentifier { get; set; }
  }
}
