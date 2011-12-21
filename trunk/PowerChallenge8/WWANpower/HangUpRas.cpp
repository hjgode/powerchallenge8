// HangUpRas.cpp

#include "StdAfx.h"
#include "HangUpRas.h"
#include <Ras.h>

////////////////////////////////////////////////////////////////////////////////

#if defined(USE_LOG_MSG)
	#include "Logging.h"
	#define LOGMSG LogMsg
#else
	#define LOGMSG  1 ? (void)0 : ::_tprintf
#endif

////////////////////////////////////////////////////////////////////////////////

int HangUpRas(BOOL bIncludeActiveSync, BOOL bHangUp)
{
	RASCONN rasConns[MAX_RAS_CONN];
	memset(&rasConns, 0, sizeof(rasConns));

	rasConns[0].dwSize = sizeof(RASCONN);
	DWORD nSize = sizeof(rasConns);
	DWORD nConns = 0;

	// Enumerate up to MAX_RAS_CONN connections
	DWORD dwResult = RasEnumConnections(rasConns, &nSize, &nConns);
	if (dwResult != ERROR_SUCCESS)
	{
		LogMsg(_T("HangUpRas: failed to enumerate connections (error %d)"), dwResult);
		return 0;
	}

	// Count all RAS connections, disconnecting if required.
	DWORD nConn, nCount;
	for (nConn=0, nCount=0; nConn < nConns; nConn++)
	{
		if ( bIncludeActiveSync || (rasConns[nConn].szEntryName[0] != _T('`')) )
		{
			nCount++;

			if (bHangUp)
			{
				SetCursor(LoadCursorW(NULL, IDC_WAIT));
				DWORD dwTickStart = GetTickCount();
				LogMsg(_T("HangUpRas: hanging up '%s'..."), rasConns[nConn].szEntryName);
				if (RasHangUp(rasConns[nConn].hrasconn) == ERROR_SUCCESS)
				{
					DWORD dwTickEnd = GetTickCount();
					LogMsg(_T("HangUpRas: hang up succeeded [%d msec]"), dwTickEnd-dwTickStart);
				}
				else
				{
					LogMsg(_T("HangUpRas: failed to hangup connection (%d)"), GetLastError());
				}
				SetCursor(NULL);
			}
			else
			{
				LogMsg(_T("HangUpRas: connection '%s'"), rasConns[nConn].szEntryName);
			}
		}
		else
		{
			LogMsg(_T("HangUpRas: ignoring ActiveSync connection '%s'"), rasConns[nConn].szEntryName);
		}
	}

	return nCount;
}
