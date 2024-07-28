// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Licensing.DataAccess.VisualStudioTrialUserInfo
// Assembly: Microsoft.VisualStudio.Services.Licensing, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B3CC7E6-129F-4267-B8E5-2210320176C7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Licensing.dll

using System;

namespace Microsoft.VisualStudio.Services.Licensing.DataAccess
{
  internal class VisualStudioTrialUserInfo
  {
    public Guid UserId { get; set; }

    public int MajorVersion { get; set; }

    public int ProductFamilyId { get; set; }

    public int ProductEditionId { get; set; }

    public bool IsMigrated { get; set; }

    public DateTimeOffset ExpirationDate { get; set; }

    public DateTimeOffset CreatedDate { get; set; }
  }
}
