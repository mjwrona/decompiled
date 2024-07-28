// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess.TableEntityFilterList
// Assembly: Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D3998EAE-13E8-421A-93CB-363047218BB4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess.dll

using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess.Common;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess
{
  public class TableEntityFilterList : List<TableEntityFilter>
  {
    public bool TryRetrieveFilter(string propertyName, out TableEntityFilter propertyFilter)
    {
      propertyFilter = this.Find((Predicate<TableEntityFilter>) (filter => filter.Property == propertyName));
      return propertyFilter != null;
    }

    public TableEntityFilter RetrieveFilter(string propertyName) => this.Find((Predicate<TableEntityFilter>) (filter => filter.Property == propertyName)) ?? throw new TableAccessException(TableAcessErrorCodeEnum.INVALID_ARGUMENTS, (Exception) new ArgumentException(string.Format("Property {0} not found", (object) propertyName)));
  }
}
