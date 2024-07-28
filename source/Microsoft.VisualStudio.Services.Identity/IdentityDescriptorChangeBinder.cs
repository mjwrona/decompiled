// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.IdentityDescriptorChangeBinder
// Assembly: Microsoft.VisualStudio.Services.Identity, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1372DA81-8681-4BFE-8E91-D1AB4333F834
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Identity.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.VisualStudio.Services.Identity
{
  internal class IdentityDescriptorChangeBinder : ObjectBinder<IdentityDescriptorChange>
  {
    private static SqlColumnBinder TypeIdColumn = new SqlColumnBinder("TypeId");
    private static SqlColumnBinder IdentityIdColumn = new SqlColumnBinder("IdentityId");

    protected override IdentityDescriptorChange Bind() => new IdentityDescriptorChange()
    {
      ChangeType = (DescriptorChangeType) IdentityDescriptorChangeBinder.TypeIdColumn.GetByte((IDataReader) this.Reader),
      IdentityId = IdentityDescriptorChangeBinder.IdentityIdColumn.GetGuid((IDataReader) this.Reader, true)
    };
  }
}
