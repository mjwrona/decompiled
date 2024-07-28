// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.ODataErrorException
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using System;
using System.Diagnostics;

namespace Microsoft.OData
{
  [DebuggerDisplay("{Message}")]
  public sealed class ODataErrorException : ODataException
  {
    private ODataErrorException.ODataErrorExceptionSafeSerializationState state;

    public ODataErrorException()
      : this(Strings.ODataErrorException_GeneralError)
    {
    }

    public ODataErrorException(string message)
      : this(message, (Exception) null)
    {
    }

    public ODataErrorException(string message, Exception innerException)
      : this(message, innerException, new ODataError())
    {
    }

    public ODataErrorException(ODataError error)
      : this(Strings.ODataErrorException_GeneralError, (Exception) null, error)
    {
    }

    public ODataErrorException(string message, ODataError error)
      : this(message, (Exception) null, error)
    {
    }

    public ODataErrorException(string message, Exception innerException, ODataError error)
      : base(message, innerException)
    {
      this.state.ODataError = error;
    }

    public ODataError Error => this.state.ODataError;

    private struct ODataErrorExceptionSafeSerializationState
    {
      public ODataError ODataError { get; set; }
    }
  }
}
