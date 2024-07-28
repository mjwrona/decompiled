// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Client.DocumentResponse`1
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using Microsoft.CSharp.RuntimeBinder;
using Newtonsoft.Json;
using System;
using System.Runtime.CompilerServices;

namespace Microsoft.Azure.Documents.Client
{
  public sealed class DocumentResponse<TDocument> : 
    ResourceResponseBase,
    IDocumentResponse<TDocument>,
    IResourceResponseBase
  {
    private TDocument document;
    private JsonSerializerSettings settings;

    public DocumentResponse()
    {
    }

    public DocumentResponse(TDocument document)
      : this()
    {
      this.document = document;
    }

    internal DocumentResponse(DocumentServiceResponse response, JsonSerializerSettings settings = null)
      : base(response)
    {
      this.settings = settings;
    }

    public TDocument Document
    {
      get
      {
        if ((object) this.document == null)
        {
          Microsoft.Azure.Documents.Document resource = this.response.GetResource<Microsoft.Azure.Documents.Document>();
          resource.SerializerSettings = this.settings;
          if (DocumentResponse<TDocument>.\u003C\u003Eo__6.\u003C\u003Ep__0 == null)
            DocumentResponse<TDocument>.\u003C\u003Eo__6.\u003C\u003Ep__0 = CallSite<Func<CallSite, object, TDocument>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof (TDocument), typeof (DocumentResponse<TDocument>)));
          this.document = DocumentResponse<TDocument>.\u003C\u003Eo__6.\u003C\u003Ep__0.Target((CallSite) DocumentResponse<TDocument>.\u003C\u003Eo__6.\u003C\u003Ep__0, (object) resource);
        }
        return this.document;
      }
    }

    public static implicit operator TDocument(DocumentResponse<TDocument> source) => source.Document;
  }
}
