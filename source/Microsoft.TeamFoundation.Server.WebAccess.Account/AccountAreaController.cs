// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Account.AccountAreaController
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Account, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AC21A176-69BE-407E-B3DD-80612369F784
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.Account.dll

namespace Microsoft.TeamFoundation.Server.WebAccess.Account
{
  public class AccountAreaController : TfsAreaController
  {
    private static readonly string s_traceArea = "WebAccess.Account";

    public override string AreaName => "Account";

    public override string TraceArea => AccountAreaController.s_traceArea;
  }
}
