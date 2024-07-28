// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ExtensionManagement.Server.Components.RequestedExtensionColumns
// Assembly: Microsoft.VisualStudio.Services.ExtensionManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 57F50803-C5C4-41A9-A26F-AD293D563111
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ExtensionManagement.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.VisualStudio.Services.ExtensionManagement.Server.Components
{
  internal class RequestedExtensionColumns : ObjectBinder<RequestedExtension>
  {
    private SqlColumnBinder partitionIdColumn = new SqlColumnBinder("PartitionId");
    private SqlColumnBinder publisherNameColumn = new SqlColumnBinder("PublisherName");
    private SqlColumnBinder extensionNameColumn = new SqlColumnBinder("ExtensionName");
    private SqlColumnBinder requesterIdColumn = new SqlColumnBinder("RequesterId");
    private SqlColumnBinder resolverIdColumn = new SqlColumnBinder("ResolverId");
    private SqlColumnBinder requestMessageColumn = new SqlColumnBinder("RequestMessage");
    private SqlColumnBinder rejectMessageColumn = new SqlColumnBinder("RejectMessage");
    private SqlColumnBinder requestDateColumn = new SqlColumnBinder("RequestDate");
    private SqlColumnBinder resolveDateColumn = new SqlColumnBinder("ResolveDate");
    private SqlColumnBinder requestStateColumn = new SqlColumnBinder("RequestState");
    private IVssRequestContext m_requestContext;
    private const string s_area = "ExtensionRequestComponent";
    private const string s_layer = "ObjectBinder";

    public RequestedExtensionColumns(IVssRequestContext requestContext)
    {
      requestContext.TraceEnter(70325, "ExtensionRequestComponent", "ObjectBinder", "RequestedExtensionColumns.ctor");
      this.m_requestContext = requestContext;
      this.m_requestContext.TraceLeave(70330, "ExtensionRequestComponent", "ObjectBinder", "RequestedExtensionColumns.ctor");
    }

    protected override RequestedExtension Bind()
    {
      this.m_requestContext.TraceEnter(70335, "ExtensionRequestComponent", "ObjectBinder", nameof (Bind));
      string str = (string) null;
      Guid guid = this.resolverIdColumn.GetGuid((IDataReader) this.Reader, true);
      if (guid != Guid.Empty)
        str = guid.ToString();
      ExtensionRequest extensionRequest = new ExtensionRequest()
      {
        RequestedBy = new IdentityRef()
        {
          Id = this.requesterIdColumn.GetGuid((IDataReader) this.Reader, false).ToString()
        },
        ResolvedBy = new IdentityRef() { Id = str },
        RequestMessage = this.requestMessageColumn.GetString((IDataReader) this.Reader, false),
        RejectMessage = this.rejectMessageColumn.GetString((IDataReader) this.Reader, true),
        RequestDate = this.requestDateColumn.GetDateTime((IDataReader) this.Reader),
        ResolveDate = this.resolveDateColumn.GetDateTime((IDataReader) this.Reader),
        RequestState = (ExtensionRequestState) this.requestStateColumn.GetByte((IDataReader) this.Reader)
      };
      RequestedExtension requestedExtension = new RequestedExtension()
      {
        PublisherName = this.publisherNameColumn.GetString((IDataReader) this.Reader, false),
        ExtensionName = this.extensionNameColumn.GetString((IDataReader) this.Reader, false),
        ExtensionRequests = new List<ExtensionRequest>((IEnumerable<ExtensionRequest>) new ExtensionRequest[1]
        {
          extensionRequest
        }),
        RequestCount = 1
      };
      this.m_requestContext.TraceLeave(70340, "ExtensionRequestComponent", "ObjectBinder", nameof (Bind));
      return requestedExtension;
    }
  }
}
