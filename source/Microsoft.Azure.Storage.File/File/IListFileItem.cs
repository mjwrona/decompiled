// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Storage.File.IListFileItem
// Assembly: Microsoft.Azure.Storage.File, Version=11.2.3.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: C68E95B0-8DFB-410C-8E70-706406D1A279
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Storage.File.dll

using System;

namespace Microsoft.Azure.Storage.File
{
  public interface IListFileItem
  {
    Uri Uri { get; }

    StorageUri StorageUri { get; }

    CloudFileDirectory Parent { get; }

    CloudFileShare Share { get; }
  }
}
