// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.CspConstants
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public static class CspConstants
  {
    public const string CspPartnerClaim = "CspPartner";

    public static class WellKnownRoleTemplateIds
    {
      public const string CompanyAdministrator = "62e90394-69f5-4237-9190-012177145e10";
      public const string HelpdeskAdministrator = "729827e3-9c14-49f7-bb1b-9608f156bbb8";
      public const string CloudServiceProvider = "08372b87-7d02-482a-9e02-fb03ea5fe193";
    }

    public static class CspPartnerSubjectStoreIds
    {
      public static readonly Guid CompanyAdministrator = new Guid("56EDF84D-76F3-4CB9-AF19-6F79A9E918CD");
      public static readonly Guid HelpdeskAdministrator = new Guid("FB28751D-A7D3-42EF-81C1-82510A9202D1");
    }
  }
}
