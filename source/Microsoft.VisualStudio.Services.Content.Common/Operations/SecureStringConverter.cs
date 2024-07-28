// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Common.Operations.SecureStringConverter
// Assembly: Microsoft.VisualStudio.Services.Content.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DC45E7D4-4445-41B3-8FA2-C13CD848D0F1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Common.dll

using Microsoft.VisualStudio.Services.Common.CommandLine;
using System;
using System.Security;

namespace Microsoft.VisualStudio.Services.Content.Common.Operations
{
  public sealed class SecureStringConverter : ValueConverter
  {
    protected override Type ResultType => typeof (SecureString);

    protected override object ConvertValue(string value) => (object) SecureStringFactory.Create(value);
  }
}
