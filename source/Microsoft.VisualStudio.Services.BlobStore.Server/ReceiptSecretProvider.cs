// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.ReceiptSecretProvider
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D3AB5C9B-EB54-4477-A304-63BB297414A3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.BlobStore.Server.Common;

namespace Microsoft.VisualStudio.Services.BlobStore.Server
{
  public class ReceiptSecretProvider : ReceiptSecretProviderBase
  {
    public ReceiptSecretProvider(IVssRequestContext context) => this.ReceiptSecrets = this.GetReceiptSecretsFromStrongBox(context);

    private ReceiptSecretTandem GetReceiptSecretsFromStrongBox(IVssRequestContext context)
    {
      IVssRequestContext vssRequestContext = context.Elevate().To(TeamFoundationHostType.Deployment);
      ITeamFoundationStrongBoxService service = vssRequestContext.GetService<ITeamFoundationStrongBoxService>();
      StrongBoxItemInfo itemInfo1 = service.GetItemInfo(vssRequestContext, "ConfigurationSecrets", "PrimaryChunkDedupReceiptKey", true);
      string primarySecret = service.GetString(vssRequestContext, itemInfo1);
      StrongBoxItemInfo itemInfo2 = service.GetItemInfo(vssRequestContext, "ConfigurationSecrets", "SecondaryChunkDedupReceiptKey", true);
      string secondarySecret = service.GetString(vssRequestContext, itemInfo2);
      return new ReceiptSecretTandem(primarySecret, secondarySecret);
    }
  }
}
