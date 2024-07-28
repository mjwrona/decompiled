// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Common.Internal.HResult
// Assembly: Microsoft.TeamFoundation.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E643B42-FE11-4FC2-A9D6-79417E26CF92
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Common.dll

using System.ComponentModel;

namespace Microsoft.TeamFoundation.Common.Internal
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public static class HResult
  {
    public const int S_OK = 0;
    public const int S_FALSE = 1;
    public const int E_FAIL = -2147467259;
    public const int E_ABORT = -2147467260;
    public const int E_UNEXPECTED = -2147418113;
    public const int E_NOTIMPL = -2147467263;
    public const int E_OUTOFMEMORY = -2147024882;
    public const int E_INVALIDARG = -2147024809;
    public const int E_NOINTERFACE = -2147467262;
    public const int E_POINTER = -2147467261;
    public const int E_HANDLE = -2147024890;
    public const int E_ACCESSDENIED = -2147024891;
    public const int E_PENDING = -2147483638;
    public const int RPC_E_CALL_REJECTED = -2147418111;
    public const int E_WIN32_ERROR_SHARING_VIOLATION = -2147024864;
    public const int E_WIN32_ERROR_LOCK_VIOLATION = -2147024863;
    public const int E_DS_NO_SUCH_OBJECT = -2147016656;
    public const int E_DS_UNWILLING_TO_PERFORM = -2147016651;
    public const int E_DS_SERVER_NOT_OPERATIONAL = -2147016646;
    public const int E_DS_LOGIN_FAILURE = -2147023570;
    public const int E_DISK_FULL = -2147024784;

    public static bool Succeeded(int hr) => hr >= 0;

    public static bool Failed(int hr) => hr < 0;

    public static bool IsFileInUse(int hr) => hr == -2147024864 || hr == -2147024863;
  }
}
