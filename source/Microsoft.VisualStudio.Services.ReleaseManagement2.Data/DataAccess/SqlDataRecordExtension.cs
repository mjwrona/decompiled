// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataAccess.SqlDataRecordExtension
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.SqlServer.Server;
using System;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataAccess
{
  public static class SqlDataRecordExtension
  {
    public static void SetNullableDateTime(this SqlDataRecord rec, int index, DateTime? value)
    {
      if (rec == null)
        throw new ArgumentNullException(nameof (rec));
      if (value.HasValue)
        rec.SetDateTime(index, value.GetValueOrDefault());
      else
        rec.SetDBNull(index);
    }
  }
}
