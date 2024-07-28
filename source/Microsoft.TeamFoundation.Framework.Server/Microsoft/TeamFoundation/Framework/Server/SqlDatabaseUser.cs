// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.SqlDatabaseUser
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Diagnostics;

namespace Microsoft.TeamFoundation.Framework.Server
{
  [DebuggerDisplay("Name: {Name}; Id: {UserId}")]
  public sealed class SqlDatabaseUser
  {
    public string Name { get; set; }

    public int UserId { get; set; }

    public byte[] Sid { get; set; }

    public DateTime CreateDate { get; set; }

    public DateTime UpdateDate { get; set; }

    public bool HasDbAccess { get; set; }

    public bool IsNTGroup { get; set; }

    public bool IsNTUser { get; set; }

    public bool IsSqlUser { get; set; }

    public bool IsNTName => this.IsNTUser || this.IsNTGroup;
  }
}
