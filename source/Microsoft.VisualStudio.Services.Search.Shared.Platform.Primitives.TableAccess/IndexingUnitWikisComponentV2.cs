// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess.IndexingUnitWikisComponentV2
// Assembly: Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D3998EAE-13E8-421A-93CB-363047218BB4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess.dll

using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess.Common;
using System;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess
{
  internal class IndexingUnitWikisComponentV2 : IndexingUnitWikisComponentV1
  {
    public virtual void DeleteIndexingUnitWikisEntry(int indexingUnitId)
    {
      this.ValidateIndexingUnitId(indexingUnitId);
      try
      {
        this.PrepareStoredProcedure("Search.prc_DeleteIndexingUnitWikisEntry");
        this.BindInt("@indexingUnitId", indexingUnitId);
        this.ExecuteNonQuery(false);
      }
      catch (Exception ex)
      {
        throw new TableAccessException(TableAcessErrorCodeEnum.UNEXPECTED_ERROR, ex, FormattableString.Invariant(FormattableStringFactory.Create("Failed to delete IndexingUnitWikis entry with IndexingUnitId {0}", (object) indexingUnitId)));
      }
    }
  }
}
