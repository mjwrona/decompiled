// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess.TableEntityFilter
// Assembly: Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D3998EAE-13E8-421A-93CB-363047218BB4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess.dll

namespace Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess
{
  public class TableEntityFilter
  {
    public TableEntityFilter(
      string property,
      string compareOperator,
      string value,
      object objectValue = null)
    {
      this.Property = property;
      this.CompareOperator = compareOperator;
      this.Value = value;
      this.ObjectValue = objectValue;
    }

    public string Property { get; private set; }

    public string CompareOperator { get; private set; }

    public string Value { get; private set; }

    public object ObjectValue { get; private set; }
  }
}
