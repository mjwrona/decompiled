// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.Transform.TransformComponent28
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.VisualStudio.Services.Analytics.Transform
{
  internal class TransformComponent28 : TransformComponent21
  {
    public override void GenerateCalendar(string cultureName, int lcid)
    {
      this.PrepareStoredProcedure("[AnalyticsModel].[prc_GenerateCalendar]", true);
      if (this.Version >= 28)
      {
        this.BindString("@cultureName", cultureName, (int) byte.MaxValue, BindStringBehavior.Unchanged, SqlDbType.VarChar);
        this.BindInt("@LCID", lcid);
      }
      this.ExecuteNonQuery();
    }
  }
}
