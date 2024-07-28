// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.SqlDatabaseRole
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Diagnostics;
using System.Security.Principal;

namespace Microsoft.TeamFoundation.Framework.Server
{
  [DebuggerDisplay("Name: {Name}, Id: {Id}, Owner: {Owner}")]
  public sealed class SqlDatabaseRole
  {
    public string Name { get; set; }

    public DateTime CreateDate { get; set; }

    public DateTime ModifyDate { get; set; }

    public int Id { get; set; }

    public bool IsFixedRole { get; set; }

    public string Owner { get; set; }

    public SecurityIdentifier Sid { get; set; }
  }
}
