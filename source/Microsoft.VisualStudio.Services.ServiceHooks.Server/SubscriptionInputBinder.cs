// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceHooks.Server.SubscriptionInputBinder
// Assembly: Microsoft.VisualStudio.Services.ServiceHooks.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 25B6D63E-3809-4A04-9AE1-1F77D8FFEE67
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ServiceHooks.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ServiceHooks.Common;
using Microsoft.VisualStudio.Services.ServiceHooks.WebApi;
using System.Data;

namespace Microsoft.VisualStudio.Services.ServiceHooks.Server
{
  internal class SubscriptionInputBinder : ObjectBinder<SubscriptionInputValue>
  {
    private SqlColumnBinder Id = new SqlColumnBinder("InputId");
    private SqlColumnBinder Value = new SqlColumnBinder("InputValue");
    private SqlColumnBinder SubscriptionId = new SqlColumnBinder(nameof (SubscriptionId));
    private SqlColumnBinder Scope = new SqlColumnBinder(nameof (Scope));

    protected override SubscriptionInputValue Bind() => new SubscriptionInputValue()
    {
      InputId = this.Id.GetString((IDataReader) this.Reader, false),
      InputValue = this.Value.GetString((IDataReader) this.Reader, false),
      SubscriptionId = this.SubscriptionId.GetGuid((IDataReader) this.Reader),
      Scope = (SubscriptionInputScope) this.Scope.GetInt16((IDataReader) this.Reader)
    };
  }
}
