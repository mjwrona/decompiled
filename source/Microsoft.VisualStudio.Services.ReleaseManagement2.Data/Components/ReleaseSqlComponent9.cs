// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components.ReleaseSqlComponent9
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using System;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components
{
  [SuppressMessage("Microsoft.Maintainability", "CA1501:AvoidExcessiveInheritance", Justification = "This is our versioning mechanism")]
  public class ReleaseSqlComponent9 : ReleaseSqlComponent8
  {
    public override void DeleteRelease(
      Guid projectId,
      int releaseId,
      Guid modifiedBy,
      string comment)
    {
      this.PrepareStoredProcedure("Release.prc_Release_Delete", projectId);
      this.BindInt(nameof (releaseId), releaseId);
      this.BindGuid(nameof (modifiedBy), modifiedBy);
      this.ExecuteNonQuery();
    }
  }
}
