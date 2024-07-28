// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.ODataMessageInfo
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Edm;
using System;
using System.IO;
using System.Text;

namespace Microsoft.OData
{
  public sealed class ODataMessageInfo
  {
    public ODataMediaType MediaType { get; set; }

    public Encoding Encoding { get; set; }

    public IEdmModel Model { get; set; }

    public bool IsResponse { get; set; }

    public IODataPayloadUriConverter PayloadUriConverter { get; set; }

    public IServiceProvider Container { get; set; }

    public bool IsAsync { get; set; }

    public Stream MessageStream { get; set; }

    internal ODataPayloadKind PayloadKind { get; set; }
  }
}
