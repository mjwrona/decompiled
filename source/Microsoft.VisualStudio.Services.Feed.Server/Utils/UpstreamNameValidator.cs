// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Feed.Server.Utils.UpstreamNameValidator
// Assembly: Microsoft.VisualStudio.Services.Feed.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 55555083-3B79-4F4F-AA85-92D66019974E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Feed.Server.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Feed.Server.Utils
{
  public class UpstreamNameValidator : NameValidator
  {
    public const int UpstreamSourceMaxNameLength = 30;

    public UpstreamNameValidator() => this.IllegalChars = ((IEnumerable<char>) base.IllegalChars).Where<char>((Func<char, bool>) (x => x != '@')).ToArray<char>();

    protected override char[] IllegalChars { get; }

    public override bool IsValidName(string input, out string errorMessage)
    {
      if (string.IsNullOrWhiteSpace(input) || input.Length > 30)
      {
        errorMessage = Resources.Error_UpstreamSourceNameWrongLength((object) input, (object) 30);
        return false;
      }
      if (input != input.Trim())
      {
        errorMessage = Resources.Error_UpstreamSourceNameSurroundingWhitespace((object) input);
        return false;
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      if (input.Replace(" ", (string) null).Any<char>(UpstreamNameValidator.\u003C\u003EO.\u003C0\u003E__IsWhiteSpace ?? (UpstreamNameValidator.\u003C\u003EO.\u003C0\u003E__IsWhiteSpace = new Func<char, bool>(char.IsWhiteSpace))))
      {
        errorMessage = Resources.Error_UpstreamSourceNameContainsInvalidWhitespace((object) input);
        return false;
      }
      if (base.IsValidName(input, out errorMessage))
        return true;
      errorMessage = Resources.Error_UpstreamSourceNameIsInvalid((object) input, (object) errorMessage);
      return false;
    }
  }
}
