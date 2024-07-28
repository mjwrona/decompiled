// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.DefaultCultureInfoModel
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Platform, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A6A2C403-5081-466C-A570-9B50BFA8E213
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Platform.dll

using System;
using System.Globalization;

namespace Microsoft.TeamFoundation.Server.WebAccess
{
  public class DefaultCultureInfoModel : CultureInfoModel
  {
    public DefaultCultureInfoModel(CultureInfo cultureInfo, DateTime now)
      : base(cultureInfo, now)
    {
    }

    public override string DisplayName => string.Format("{0}: {1}", (object) WACommonResources.AutomaticCulture, (object) base.DisplayName);
  }
}
