// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Boards.Settings.IAccountUserSettingsHive
// Assembly: Microsoft.Azure.Boards.Settings, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2AC3574E-9414-4605-BAB7-1F6B28A75804
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.Boards.Settings.dll

using System;
using System.Collections.Generic;

namespace Microsoft.Azure.Boards.Settings
{
  public interface IAccountUserSettingsHive : IDisposable
  {
    T ReadSetting<T>(string path, T defaultValue);

    void WriteSetting<T>(string path, T value);

    IEnumerable<T> ReadEnumerableSetting<T>(string path, IEnumerable<T> defaultValue);

    void WriteEnumerableSetting<T>(string path, IEnumerable<T> value);
  }
}
