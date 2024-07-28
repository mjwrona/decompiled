// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceHooks.Server.SubscriptionBinder
// Assembly: Microsoft.VisualStudio.Services.ServiceHooks.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 25B6D63E-3809-4A04-9AE1-1F77D8FFEE67
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ServiceHooks.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ServiceHooks.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.VisualStudio.Services.ServiceHooks.Server
{
  internal class SubscriptionBinder : ObjectBinder<Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription>
  {
    private SqlColumnBinder Id = new SqlColumnBinder("SubscriptionId");
    private SqlColumnBinder Status = new SqlColumnBinder(nameof (Status));
    private SqlColumnBinder PublisherId = new SqlColumnBinder(nameof (PublisherId));
    private SqlColumnBinder EventType = new SqlColumnBinder(nameof (EventType));
    private SqlColumnBinder ConsumerId = new SqlColumnBinder(nameof (ConsumerId));
    private SqlColumnBinder ConsumerActionId = new SqlColumnBinder(nameof (ConsumerActionId));
    private SqlColumnBinder ProbationRetries = new SqlColumnBinder(nameof (ProbationRetries));
    private SqlColumnBinder CreatedBy = new SqlColumnBinder(nameof (CreatedBy));
    private SqlColumnBinder CreatedDate = new SqlColumnBinder(nameof (CreatedDate));
    private SqlColumnBinder ModifiedBy = new SqlColumnBinder(nameof (ModifiedBy));
    private SqlColumnBinder ModifiedDate = new SqlColumnBinder(nameof (ModifiedDate));

    protected override Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription Bind()
    {
      Guid guid1 = this.Id.GetGuid((IDataReader) this.Reader);
      SubscriptionStatus int16 = (SubscriptionStatus) this.Status.GetInt16((IDataReader) this.Reader);
      string str1 = this.PublisherId.GetString((IDataReader) this.Reader, false);
      string str2 = this.EventType.GetString((IDataReader) this.Reader, false);
      string str3 = this.ConsumerId.GetString((IDataReader) this.Reader, false);
      string str4 = this.ConsumerActionId.GetString((IDataReader) this.Reader, false);
      byte num = this.ProbationRetries.ColumnExists((IDataReader) this.Reader) ? this.ProbationRetries.GetByte((IDataReader) this.Reader) : (byte) 0;
      Guid guid2 = this.CreatedBy.GetGuid((IDataReader) this.Reader, false);
      DateTime dateTime1 = this.CreatedDate.GetDateTime((IDataReader) this.Reader);
      Guid guid3 = this.ModifiedBy.GetGuid((IDataReader) this.Reader, false);
      DateTime dateTime2 = this.ModifiedDate.GetDateTime((IDataReader) this.Reader);
      return new Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription()
      {
        Id = guid1,
        Status = int16,
        PublisherId = str1,
        EventType = str2,
        ConsumerId = str3,
        ConsumerActionId = str4,
        ProbationRetries = num,
        CreatedBy = new IdentityRef()
        {
          Id = guid2.ToString()
        },
        CreatedDate = dateTime1,
        ModifiedBy = new IdentityRef()
        {
          Id = guid3.ToString()
        },
        ModifiedDate = dateTime2,
        PublisherInputs = (IDictionary<string, string>) new Dictionary<string, string>(),
        ConsumerInputs = (IDictionary<string, string>) new Dictionary<string, string>()
      };
    }
  }
}
