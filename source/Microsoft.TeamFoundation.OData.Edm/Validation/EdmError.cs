// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Edm.Validation.EdmError
// Assembly: Microsoft.TeamFoundation.OData.Edm, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 75C95BF5-2544-413A-959F-F9F18C11D35F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Edm.dll

using System;
using System.Globalization;

namespace Microsoft.OData.Edm.Validation
{
  public class EdmError
  {
    public EdmError(EdmLocation errorLocation, EdmErrorCode errorCode, string errorMessage)
    {
      this.ErrorLocation = errorLocation;
      this.ErrorCode = errorCode;
      this.ErrorMessage = errorMessage;
    }

    public EdmLocation ErrorLocation { get; private set; }

    public EdmErrorCode ErrorCode { get; private set; }

    public string ErrorMessage { get; private set; }

    public override string ToString()
    {
      if (this.ErrorLocation == null || this.ErrorLocation is ObjectLocation)
        return Convert.ToString((object) this.ErrorCode, (IFormatProvider) CultureInfo.InvariantCulture) + " : " + this.ErrorMessage;
      return Convert.ToString((object) this.ErrorCode, (IFormatProvider) CultureInfo.InvariantCulture) + " : " + this.ErrorMessage + " : " + this.ErrorLocation.ToString();
    }
  }
}
