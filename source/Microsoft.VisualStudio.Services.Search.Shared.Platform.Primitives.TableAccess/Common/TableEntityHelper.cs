// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess.Common.TableEntityHelper
// Assembly: Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D3998EAE-13E8-421A-93CB-363047218BB4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess.dll

using System;

namespace Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess.Common
{
  public class TableEntityHelper
  {
    public static string RetrieveFilterValue(TableEntityFilterList filterList, string propertyName)
    {
      if (filterList == null)
        throw new TableAccessException(TableAcessErrorCodeEnum.INVALID_ARGUMENTS, (Exception) new ArgumentNullException(nameof (filterList)));
      return filterList.RetrieveFilter(propertyName).Value;
    }
  }
}
