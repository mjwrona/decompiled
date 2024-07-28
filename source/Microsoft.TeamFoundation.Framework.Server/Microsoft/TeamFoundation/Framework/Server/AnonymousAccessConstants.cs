// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.AnonymousAccessConstants
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public static class AnonymousAccessConstants
  {
    public const string AnonymousAccessFeatureName = "VisualStudio.Services.Identity.AnonymousAccess";
    public const string AnonymousAccessKalypsoAlert = "AnonymousAccessKalypsoAlert";
    internal const string DelayedIdentityValidation = "VisualStudio.Services.DelayedIdentityValidation";
    public static readonly Guid AnonymousSubjectId = new Guid("AAAAAAAA-AAAA-AAAA-AAAA-AAAAAAAAAAAA");
    internal const string AnonymousAccountName = "Anonymous";
    internal static readonly SecuritySubjectEntry AnonymousSubject = new SecuritySubjectEntry(AnonymousAccessConstants.AnonymousSubjectId, SecuritySubjectType.PublicAccess, AnonymousAccessConstants.AnonymousSubjectId.ToString(), "Anonymous Subject");
    internal static readonly Guid PublicUserSubjectId = new Guid("EEEEEEEE-EEEE-EEEE-EEEE-EEEEEEEEEEEE");
    internal static readonly SecuritySubjectEntry PublicUserSubject = new SecuritySubjectEntry(AnonymousAccessConstants.PublicUserSubjectId, SecuritySubjectType.PublicAccess, AnonymousAccessConstants.PublicUserSubjectId.ToString(), "Public User Subject");
  }
}
