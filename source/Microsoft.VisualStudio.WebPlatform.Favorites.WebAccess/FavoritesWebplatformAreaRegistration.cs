// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.WebPlatform.Favorites.WebAccess.FavoritesWebplatformAreaRegistration
// Assembly: Microsoft.VisualStudio.WebPlatform.Favorites.WebAccess, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B09BDB0-7575-41B5-8197-FCB4157B6C4A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.WebPlatform.Favorites.WebAccess.dll

using Microsoft.TeamFoundation.Server.WebAccess;
using System;
using System.Resources;
using System.Web.Mvc;

namespace Microsoft.VisualStudio.WebPlatform.Favorites.WebAccess
{
  public class FavoritesWebplatformAreaRegistration : AreaRegistration
  {
    public override string AreaName => "Favorites";

    public override void RegisterArea(AreaRegistrationContext context) => ScriptRegistration.RegisterBundledArea(this.AreaName, this.AreaName + "/Resources", "TFS").RegisterResource(this.AreaName, (Func<ResourceManager>) (() => FavoritesResources.Manager));
  }
}
