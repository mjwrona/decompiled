// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Client.IUICredentialsProvider
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using System;
using System.ComponentModel;
using System.Net;

namespace Microsoft.TeamFoundation.Client
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Obsolete("This interface has been deprecated and will be removed in a future release. Use VssClientCredentials with CredentialPromptType.PromptIfNeeded instead.", false)]
  public interface IUICredentialsProvider : ICredentialsProvider
  {
    ICredentials GetCredentials(
      Uri uri,
      ICredentials failedCredentials,
      string caption,
      string messageText,
      IntPtr parentWindowHandle);
  }
}
