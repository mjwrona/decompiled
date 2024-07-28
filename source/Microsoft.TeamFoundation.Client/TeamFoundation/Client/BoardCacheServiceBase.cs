// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Client.BoardCacheServiceBase
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.TeamFoundation.Work.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System.ComponentModel;

namespace Microsoft.TeamFoundation.Client
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public abstract class BoardCacheServiceBase : SuggestedValueCacheService
  {
    protected override void OnInitialize() => this.m_proxy = (IVssHttpClient) this.m_projectCollection.GetClient<WorkHttpClient>();
  }
}
