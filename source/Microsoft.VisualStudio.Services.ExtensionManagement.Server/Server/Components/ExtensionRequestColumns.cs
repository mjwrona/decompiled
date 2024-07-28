// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ExtensionManagement.Server.Components.ExtensionRequestColumns
// Assembly: Microsoft.VisualStudio.Services.ExtensionManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 57F50803-C5C4-41A9-A26F-AD293D563111
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ExtensionManagement.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Data;

namespace Microsoft.VisualStudio.Services.ExtensionManagement.Server.Components
{
  internal class ExtensionRequestColumns : ObjectBinder<ExtensionRequest>
  {
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

    public ExtensionRequestColumns(IVssRequestContext requestContext) => this.m_requestContext = requestContext;

    protected override ExtensionRequest Bind()
    {
      this.m_requestContext.TraceEnter(70335, "ExtensionRequestComponent", "ObjectBinder", nameof (Bind));
      Guid guid = this.resolverIdColumn.GetGuid((IDataReader) this.Reader, true);
      ExtensionRequest extensionRequest = new ExtensionRequest();
      extensionRequest.RequestedBy = new IdentityRef()
      {
        Id = this.requesterIdColumn.GetGuid((IDataReader) this.Reader, false).ToString()
      };
      extensionRequest.ResolvedBy = new IdentityRef()
      {
        Id = guid != Guid.Empty ? guid.ToString() : (string) null
      };
      extensionRequest.RequestMessage = this.requestMessageColumn.GetString((IDataReader) this.Reader, false);
      extensionRequest.RejectMessage = this.rejectMessageColumn.GetString((IDataReader) this.Reader, true);
      extensionRequest.RequestDate = this.requestDateColumn.GetDateTime((IDataReader) this.Reader);
      extensionRequest.ResolveDate = this.resolveDateColumn.GetDateTime((IDataReader) this.Reader);
      extensionRequest.RequestState = (ExtensionRequestState) this.requestStateColumn.GetByte((IDataReader) this.Reader);
      this.m_requestContext.TraceLeave(70340, "ExtensionRequestComponent", "ObjectBinder", nameof (Bind));
      return extensionRequest;
    }
  }
}
