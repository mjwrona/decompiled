// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.IBuildCheckInTicketValidator
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.ComponentModel.Composition;

namespace Microsoft.TeamFoundation.Build2.Server
{
  [InheritedExport]
  public interface IBuildCheckInTicketValidator
  {
    void ValidateCheckInTicket(
      IVssRequestContext requestContext,
      string checkInTicket,
      IReadOnlyBuildDefinition definition,
      IReadOnlyBuildData build);
  }
}
