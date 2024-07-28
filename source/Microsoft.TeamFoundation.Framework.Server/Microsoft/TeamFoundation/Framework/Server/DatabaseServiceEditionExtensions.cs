// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.DatabaseServiceEditionExtensions
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

namespace Microsoft.TeamFoundation.Framework.Server
{
  public static class DatabaseServiceEditionExtensions
  {
    public static bool IsDtuBased(this DatabaseServiceEdition edition) => edition == DatabaseServiceEdition.Standard || edition == DatabaseServiceEdition.Premium;

    public static bool IsVCoreBased(this DatabaseServiceEdition edition) => edition == DatabaseServiceEdition.GeneralPurpose || edition == DatabaseServiceEdition.HyperScale || edition == DatabaseServiceEdition.BusinessCritical;

    public static bool CanRightSize(this DatabaseServiceEdition edition) => edition == DatabaseServiceEdition.Standard || edition == DatabaseServiceEdition.Premium || edition == DatabaseServiceEdition.GeneralPurpose || edition == DatabaseServiceEdition.BusinessCritical || edition == DatabaseServiceEdition.HyperScale;
  }
}
