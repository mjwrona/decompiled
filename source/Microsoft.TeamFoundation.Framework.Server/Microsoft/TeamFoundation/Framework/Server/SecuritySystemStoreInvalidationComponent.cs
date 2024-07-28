// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.SecuritySystemStoreInvalidationComponent
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class SecuritySystemStoreInvalidationComponent : TeamFoundationSqlResourceComponent
  {
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[2]
    {
      (IComponentCreator) new ComponentCreator<SecuritySystemStoreInvalidationComponent>(1, true),
      (IComponentCreator) new ComponentCreator<SecuritySystemStoreInvalidationComponent2>(2)
    }, "SystemStoreInvalidation");

    public virtual long InvalidateSystemStore(IVssRequestContext requestContext, string reason) => 1;
  }
}
