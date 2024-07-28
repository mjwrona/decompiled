// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Server.Common.StrongBoxItemChangeHandler
// Assembly: Microsoft.VisualStudio.Services.Content.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 203E0171-FB50-4FDE-9B1F-EFC6366423BC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Content.Server.Common
{
  public class StrongBoxItemChangeHandler
  {
    private readonly ISecretItemChangeListener itemFactory;

    public StrongBoxItemChangeHandler(ISecretItemChangeListener itemFactory)
    {
      ArgumentUtility.CheckForNull<ISecretItemChangeListener>(itemFactory, nameof (itemFactory));
      this.itemFactory = itemFactory;
    }

    public void OnStrongBoxItemChanged(
      IVssRequestContext requestContext,
      IEnumerable<StrongBoxItemName> itemNames)
    {
      ITeamFoundationStrongBoxService service = requestContext.GetService<ITeamFoundationStrongBoxService>();
      foreach (StrongBoxItemName itemName in itemNames)
      {
        string lookupKey = itemName.LookupKey;
        StrongBoxItemInfo itemInfo = service.GetItemInfo(requestContext, itemName.DrawerId, lookupKey);
        string newSecretValue = service.GetString(requestContext, itemInfo);
        this.itemFactory.OnSecretChanged(lookupKey, newSecretValue);
      }
    }
  }
}
