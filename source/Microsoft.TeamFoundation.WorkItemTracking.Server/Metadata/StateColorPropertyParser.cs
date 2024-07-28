// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.StateColorPropertyParser
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Core.WebApi;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata
{
  internal class StateColorPropertyParser : PropertyKeyValueParser<string>
  {
    protected override IEqualityComparer<string> KeyComparer => (IEqualityComparer<string>) TFStringComparer.WorkItemStateName;

    protected override string ParsePropertyKey(
      string key,
      string currentPair,
      Func<string, bool> isKeyDuplicated)
    {
      if (isKeyDuplicated(key))
        throw new ArgumentException(ServerResources.Validation_StateColors_State_Duplicate((object) key));
      return !string.IsNullOrEmpty(key) ? key : throw new ArgumentException(ServerResources.Validation_StateColors_StateOrColor_Invalid((object) currentPair));
    }

    protected override string ParsePropertyValue(string value, string currentPair) => !string.IsNullOrEmpty(value) && ProcessConfigurationConstants.ColorValidCheck.IsMatch(value) ? value : throw new ArgumentException(ServerResources.Validation_StateColors_StateOrColor_Invalid((object) currentPair));
  }
}
