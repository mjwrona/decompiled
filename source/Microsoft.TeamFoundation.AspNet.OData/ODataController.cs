// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.ODataController
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.AspNet.OData.Extensions;
using Microsoft.AspNet.OData.Results;
using System.Web.Http;
using System.Web.Http.Description;

namespace Microsoft.AspNet.OData
{
  [ODataFormatting]
  [ODataRouting]
  [ApiExplorerSettings(IgnoreApi = true)]
  public abstract class ODataController : ApiController
  {
    protected override void Dispose(bool disposing)
    {
      if (disposing && this.Request != null)
        this.Request.DeleteRequestContainer(true);
      base.Dispose(disposing);
    }

    protected virtual CreatedODataResult<TEntity> Created<TEntity>(TEntity entity) => (object) entity != null ? new CreatedODataResult<TEntity>(entity, (ApiController) this) : throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (entity));

    protected virtual UpdatedODataResult<TEntity> Updated<TEntity>(TEntity entity) => (object) entity != null ? new UpdatedODataResult<TEntity>(entity, (ApiController) this) : throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (entity));
  }
}
