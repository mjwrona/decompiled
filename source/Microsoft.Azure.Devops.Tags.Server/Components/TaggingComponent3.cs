// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Devops.Tags.Server.Components.TaggingComponent3
// Assembly: Microsoft.Azure.Devops.Tags.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 10FDC8E3-D1DB-4668-B2F2-04DAA10A7143
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Devops.Tags.Server.dll

using System;

namespace Microsoft.Azure.Devops.Tags.Server.Components
{
  internal class TaggingComponent3 : TaggingComponent2
  {
    internal override void CleanUnusedTagDefinitions(DateTime cutoffTime)
    {
      this.PrepareStoredProcedure("prc_CleanUnusedTagDefinitions", 3600);
      this.BindDateTime("@cutoffTime", cutoffTime);
      this.ExecuteNonQuery();
    }
  }
}
