// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Feed.Server.FeedInternalStateBinder
// Assembly: Microsoft.VisualStudio.Services.Feed.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 55555083-3B79-4F4F-AA85-92D66019974E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Feed.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Feed.Server.Types;
using System;
using System.Data;

namespace Microsoft.VisualStudio.Services.Feed.Server
{
  internal class FeedInternalStateBinder : ObjectBinder<FeedInternalState>
  {
    private SqlColumnBinder feedId = new SqlColumnBinder("FeedId");
    private SqlColumnBinder internalState = new SqlColumnBinder("InternalState");
    private SqlColumnBinder internalStateChangedDate = new SqlColumnBinder("InternalStateChangedDate");

    protected override FeedInternalState Bind() => new FeedInternalState(this.feedId.GetGuid((IDataReader) this.Reader), this.internalState.GetInt32((IDataReader) this.Reader, 0, 0), this.internalStateChangedDate.GetNullableDateTime((IDataReader) this.Reader, new DateTime?()));
  }
}
