// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Edm.Csdl.EdmParseException
// Assembly: Microsoft.TeamFoundation.OData.Edm, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 75C95BF5-2544-413A-959F-F9F18C11D35F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Edm.dll

using Microsoft.OData.Edm.Validation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Microsoft.OData.Edm.Csdl
{
  [SuppressMessage("Microsoft.Design", "CA1032", Justification = "We do not intend to support serialization of this exception yet, nor does it need the full suite of constructors.")]
  [SuppressMessage("Microsoft.Usage", "CA2237", Justification = "We do not intend to support serialization of this exception yet.")]
  [DebuggerDisplay("{Message}")]
  public class EdmParseException : Exception
  {
    public EdmParseException(IEnumerable<EdmError> parseErrors)
      : this(parseErrors.ToList<EdmError>())
    {
    }

    private EdmParseException(List<EdmError> parseErrors)
      : base(EdmParseException.ConstructMessage((IEnumerable<EdmError>) parseErrors))
    {
      this.Errors = new ReadOnlyCollection<EdmError>((IList<EdmError>) parseErrors);
    }

    public ReadOnlyCollection<EdmError> Errors { get; private set; }

    private static string ConstructMessage(IEnumerable<EdmError> parseErrors) => Strings.EdmParseException_ErrorsEncounteredInEdmx((object) string.Join(Environment.NewLine, parseErrors.Select<EdmError, string>((Func<EdmError, string>) (p => p.ToString())).ToArray<string>()));
  }
}
