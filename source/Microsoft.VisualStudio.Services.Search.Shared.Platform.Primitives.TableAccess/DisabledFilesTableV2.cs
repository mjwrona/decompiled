// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess.DisabledFilesTableV2
// Assembly: Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D3998EAE-13E8-421A-93CB-363047218BB4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess.dll

using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess.Common;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess
{
  internal class DisabledFilesTableV2 : DisabledFilesTable
  {
    public DisabledFilesTableV2()
    {
    }

    internal DisabledFilesTableV2(string connectionString, int partitionId)
      : base(connectionString, partitionId)
    {
    }

    public override DisabledFile Update(DisabledFile disabledFile)
    {
      this.ValidateNotNull<DisabledFile>(nameof (disabledFile), disabledFile);
      try
      {
        this.PrepareStoredProcedure("Search.prc_UpdateEntryForDisabledFilesTable");
        IList<DisabledFile> rows = (IList<DisabledFile>) new List<DisabledFile>();
        rows.Add(disabledFile);
        this.BindDisabledFileEntityLookupTable("@itemList", (IEnumerable<DisabledFile>) rows);
        this.ExecuteNonQuery(false);
      }
      catch (Exception ex)
      {
        throw new TableAccessException(TableAcessErrorCodeEnum.UNEXPECTED_ERROR, ex, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Failed to update entity with File Path {0}  with SQL Azure platform", (object) disabledFile.FilePath));
      }
      return (DisabledFile) null;
    }
  }
}
