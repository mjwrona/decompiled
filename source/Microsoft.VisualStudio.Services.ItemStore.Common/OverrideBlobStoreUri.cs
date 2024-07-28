// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ItemStore.Common.OverrideBlobStoreUri
// Assembly: Microsoft.VisualStudio.Services.ItemStore.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 44753C0C-D541-4975-AF3F-2B606DE6FF70
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ItemStore.Common.dll

using Microsoft.VisualStudio.Services.Content.Common;
using System;

namespace Microsoft.VisualStudio.Services.ItemStore.Common
{
  public class OverrideBlobStoreUri
  {
    private const string OverideBlobStoreUriEnvVarName = "OVERRIDE_BLOBSTOREUPLOADURI";

    public OverrideBlobStoreUri(IEnvironment environment)
    {
      Uri result;
      this.BlobStoreOverrideUri = Uri.TryCreate(environment.GetEnvironmentVariable("OVERRIDE_BLOBSTOREUPLOADURI"), UriKind.Absolute, out result) ? result : (Uri) null;
    }

    public Uri GetUri(Uri providedBlobStoreUri)
    {
      Uri storeOverrideUri = this.BlobStoreOverrideUri;
      return (object) storeOverrideUri != null ? storeOverrideUri : providedBlobStoreUri;
    }

    public Uri BlobStoreOverrideUri { get; private set; }

    public bool IsOverrideInPlace => this.BlobStoreOverrideUri != (Uri) null;
  }
}
