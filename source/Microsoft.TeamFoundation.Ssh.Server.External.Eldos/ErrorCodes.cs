// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Ssh.Server.External.Eldos.ErrorCodes
// Assembly: Microsoft.TeamFoundation.Ssh.Server.External.Eldos, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 76A7154E-5D66-408C-AA1C-E130B17CCD4D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Ssh.Server.External.Eldos.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Ssh.Server.External.Eldos
{
  internal static class ErrorCodes
  {
    private static readonly Dictionary<int, string> s_mappings = new Dictionary<int, string>()
    {
      {
        1,
        "ERROR_SSH_INVALID_IDENTIFICATION_STRING"
      },
      {
        2,
        "ERROR_SSH_INVALID_VERSION"
      },
      {
        3,
        "ERROR_SSH_INVALID_MESSAGE_CODE"
      },
      {
        4,
        "ERROR_SSH_INVALID_CRC"
      },
      {
        5,
        "ERROR_SSH_INVALID_PACKET_TYPE"
      },
      {
        6,
        "ERROR_SSH_INVALID_PACKET"
      },
      {
        7,
        "ERROR_SSH_UNSUPPORTED_CIPHER"
      },
      {
        8,
        "ERROR_SSH_UNSUPPORTED_AUTH_TYPE"
      },
      {
        9,
        "ERROR_SSH_INVALID_RSA_CHALLENGE"
      },
      {
        10,
        "ERROR_SSH_AUTHENTICATION_FAILED"
      },
      {
        11,
        "ERROR_SSH_INVALID_PACKET_SIZE"
      },
      {
        101,
        "ERROR_SSH_HOST_NOT_ALLOWED_TO_CONNECT"
      },
      {
        102,
        "ERROR_SSH_PROTOCOL_ERROR"
      },
      {
        103,
        "ERROR_SSH_KEY_EXCHANGE_FAILED"
      },
      {
        105,
        "ERROR_SSH_INVALID_MAC"
      },
      {
        106,
        "ERROR_SSH_COMPRESSION_ERROR"
      },
      {
        107,
        "ERROR_SSH_SERVICE_NOT_AVAILABLE"
      },
      {
        108,
        "ERROR_SSH_PROTOCOL_VERSION_NOT_SUPPORTED"
      },
      {
        109,
        "ERROR_SSH_HOST_KEY_NOT_VERIFIABLE"
      },
      {
        110,
        "ERROR_SSH_CONNECTION_LOST"
      },
      {
        111,
        "ERROR_SSH_APPLICATION_CLOSED"
      },
      {
        112,
        "ERROR_SSH_TOO_MANY_CONNECTIONS"
      },
      {
        113,
        "ERROR_SSH_AUTH_CANCELLED_BY_USER"
      },
      {
        114,
        "ERROR_SSH_NO_MORE_AUTH_METHODS_AVAILABLE"
      },
      {
        115,
        "ERROR_SSH_ILLEGAL_USERNAME"
      },
      {
        200,
        "ERROR_SSH_INTERNAL_ERROR"
      },
      {
        222,
        "ERROR_SSH_NOT_CONNECTED"
      },
      {
        501,
        "ERROR_SSH_CONNECTION_CANCELLED_BY_USER"
      },
      {
        502,
        "ERROR_SSH_FORWARD_DISALLOWED"
      },
      {
        503,
        "ERROR_SSH_ONKEYVALIDATE_NOT_ASSIGNED"
      },
      {
        601,
        "ERROR_SSH_GSSKEX_SERVER_ERROR_MESSAGE"
      },
      {
        603,
        "ERROR_SSH_GSSAPI_SERVER_ERROR_MESSAGE"
      },
      {
        3329,
        "SB_ERROR_SSH_KEYS_INVALID_PUBLIC_KEY"
      },
      {
        3330,
        "SB_ERROR_SSH_KEYS_INVALID_PRIVATE_KEY"
      },
      {
        3331,
        "SB_ERROR_SSH_KEYS_FILE_READ_ERROR"
      },
      {
        3332,
        "SB_ERROR_SSH_KEYS_FILE_WRITE_ERROR"
      },
      {
        3333,
        "SB_ERROR_SSH_KEYS_UNSUPPORTED_ALGORITHM"
      },
      {
        3334,
        "SB_ERROR_SSH_KEYS_INTERNAL_ERROR"
      },
      {
        3335,
        "SB_ERROR_SSH_KEYS_BUFFER_TOO_SMALL"
      },
      {
        3336,
        "SB_ERROR_SSH_KEYS_NO_PRIVATE_KEY"
      },
      {
        3337,
        "SB_ERROR_SSH_KEYS_INVALID_PASSPHRASE"
      },
      {
        3338,
        "SB_ERROR_SSH_KEYS_UNSUPPORTED_PEM_ALGORITHM"
      },
      {
        3339,
        "SB_ERROR_SSH_KEYS_UNSUPPORTED_CURVE"
      }
    };
    private static readonly Dictionary<string, int> s_errorStatusesToCodeMapping = ErrorCodes.s_mappings.ToDictionary<KeyValuePair<int, string>, string, int>((Func<KeyValuePair<int, string>, string>) (x => x.Value), (Func<KeyValuePair<int, string>, int>) (x => x.Key));

    public static string GetMessage(int errorCode)
    {
      string str;
      return !ErrorCodes.s_mappings.TryGetValue(errorCode, out str) ? string.Format("Unknown SSH error {0}", (object) errorCode) : string.Format("{0} ({1})", (object) str, (object) errorCode);
    }

    public static Dictionary<string, int> ErrorStatusesToCodeMapping => ErrorCodes.s_errorStatusesToCodeMapping;

    internal static IReadOnlyDictionary<int, string> TEST_Mappings => (IReadOnlyDictionary<int, string>) ErrorCodes.s_mappings;
  }
}
