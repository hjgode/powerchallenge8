// TapiWrapper.cpp: implementation of the CTapiWrapper class.
//
//////////////////////////////////////////////////////////////////////

#include "stdafx.h"
#include "TapiWrapper.h"
#include "Logging.h"

#pragma comment(lib,"cellcore.lib")

// Taken from ExTAPI example in PPC2003 SDK
DWORD GetTSPLineDeviceID(const HLINEAPP hLineApp, 
                         const DWORD dwNumberDevices, 
                         const DWORD dwAPIVersionLow, 
                         const DWORD dwAPIVersionHigh, 
                         const TCHAR* const psTSPLineName);

//////////////////////////////////////////////////////////////////////
// Construction/Destruction
//////////////////////////////////////////////////////////////////////

CTapiWrapper::CTapiWrapper(LPCTSTR lpszFriendlyAppName)
{
	if (lpszFriendlyAppName && *lpszFriendlyAppName)
	{
		LogMsg(_T("CTapiWrapper::CTapiWrapper(%s)"), lpszFriendlyAppName);

		m_lpszFriendlyAppName = new TCHAR[_tcslen(lpszFriendlyAppName) + 1];
		if (m_lpszFriendlyAppName)
		{
			_tcscpy(m_lpszFriendlyAppName, lpszFriendlyAppName);
		}
	}
	else
	{
		LogMsg(_T("CTapiWrapper::CTapiWrapper(NULL)"));
		m_lpszFriendlyAppName = NULL;
	}

	m_hLineApp = NULL;
	m_hLine = NULL;
	m_dwAPIVersion = 0;
	m_dwTAPILineDeviceID = 0;
	m_hTapiEvent = NULL;
	m_hThread = NULL;

	m_nRequestId = INVALID_TAPI_REQUEST_ID;
	m_nReplyReqId = INVALID_TAPI_REQUEST_ID;
	m_dwReplyResult = 0;
	m_hReplyEvent = NULL;
}

CTapiWrapper::~CTapiWrapper()
{
	LogMsg(_T("CTapiWrapper::~CTapiWrapper"));

	Uninitialize();

	if (m_lpszFriendlyAppName)
	{
		delete[] m_lpszFriendlyAppName;
	}
}

bool CTapiWrapper::Initialize()
{
	LogMsg(_T("CTapiWrapper::Initialize: start"));

    LINEINITIALIZEEXPARAMS liep;
    DWORD dwMediaMode = LINEMEDIAMODE_DATAMODEM | LINEMEDIAMODE_INTERACTIVEVOICE;
    DWORD dwExtVersion;
	bool bResult = false;

	memset(&liep, 0, sizeof(liep));
    liep.dwTotalSize = sizeof(liep);
    liep.dwOptions = LINEINITIALIZEEXOPTION_USEEVENT;

	m_dwAPIVersion = TAPI_API_HIGH_VERSION;

	InitializeCriticalSection(&m_csReplyRequest);

	if ( (m_hReplyEvent = CreateEvent(NULL, FALSE /*auto reset*/, FALSE /*unsignalled*/, NULL)) != INVALID_HANDLE_VALUE)
	{
		if (lineInitializeEx(&m_hLineApp, 0, 0, m_lpszFriendlyAppName, &m_dwNumDevs, &m_dwAPIVersion, &liep) == ERROR_SUCCESS)
		{
			m_hTapiEvent = liep.Handles.hEvent;

			m_dwTAPILineDeviceID = GetTSPLineDeviceID(m_hLineApp, m_dwNumDevs, TAPI_API_LOW_VERSION, TAPI_API_HIGH_VERSION, CELLTSP_LINENAME_STRING);

			if (m_dwTAPILineDeviceID != 0xFFFFFFFF)
			{
				if(lineOpen(m_hLineApp, m_dwTAPILineDeviceID, &m_hLine, m_dwAPIVersion, 0, 0, LINECALLPRIVILEGE_OWNER, dwMediaMode, 0) == ERROR_SUCCESS)
				{
					if (lineNegotiateExtVersion(m_hLineApp, m_dwTAPILineDeviceID, m_dwAPIVersion, EXT_API_LOW_VERSION, EXT_API_HIGH_VERSION, &dwExtVersion) == ERROR_SUCCESS)
					{
						DWORD dwThreadId;

						m_hThread = CreateThread(NULL, 0, StaticThreadProc, this, 0, &dwThreadId);
						bResult = (m_hThread != NULL);
					}
				}
			}
		}
	}

	if (!bResult)
	{
		Uninitialize();
	}

	LogMsg(_T("CTapiWrapper::Initialize: end (%s)"), (bResult ? _T("success") : _T("failure")) );

	return bResult;
}

void CTapiWrapper::Uninitialize()
{
	LogMsg(_T("CTapiWrapper::Uninitialize: start"));

    if (m_hLine)
	{
		lineClose(m_hLine);
	}

    if (m_hLineApp)
	{
		lineShutdown(m_hLineApp);
	}

	if (m_hThread)
	{
		WaitForSingleObject(m_hThread, 10000);
	}

	if (m_hReplyEvent)
	{
		CloseHandle(m_hReplyEvent);
	}

	DeleteCriticalSection(&m_csReplyRequest);

	m_hLineApp = NULL;
	m_hLine = NULL;
	m_dwAPIVersion = 0;
	m_dwTAPILineDeviceID = 0;
	m_hTapiEvent = NULL;
	m_hThread = NULL;

	m_nRequestId = INVALID_TAPI_REQUEST_ID;
	m_nReplyReqId = INVALID_TAPI_REQUEST_ID;
	m_dwReplyResult = 0;
	m_hReplyEvent = NULL;

	LogMsg(_T("CTapiWrapper::Uninitialize: end"));
}

bool CTapiWrapper::GetRadioPower(bool* pbRadioOn)
{
	if (pbRadioOn != NULL)
	{
		DWORD dwState, dwSupport;
		if (lineGetEquipmentState(m_hLine, &dwState, &dwSupport) == ERROR_SUCCESS)
		{
			*pbRadioOn = (dwState == LINEEQUIPSTATE_FULL) ? true : false;
			return true;
		}
	}

	return false;
}

bool CTapiWrapper::SetRadioPower(bool bRadioOn)
{
	bool bSuccess = false;
	DWORD dwState = bRadioOn ? LINEEQUIPSTATE_FULL : LINEEQUIPSTATE_MINIMUM;

	LogMsg(_T("CTapiWrapper::SetRadioPower: calling lineSetEquipmentState(%s)..."), (bRadioOn ? _T("true") : _T("false")) );

	LONG lReqId = lineSetEquipmentState(m_hLine, dwState);
	
	if (lReqId > 0)
	{
		LogMsg(_T("CTapiWrapper::SetRadioPower: ...lineSetEquipmentState() done, request ID=%d"), lReqId);

		DWORD dwResult;
		if (WaitForReplyMsg( lReqId, &dwResult ))
		{
			bSuccess = (dwResult == 0);
		}
	}
	else
	{
		LogMsg(_T("CTapiWrapper::SetRadioPower: ...lineSetEquipmentState() failed, error 0x%08x"), lReqId);
	}

	return bSuccess;
}

bool CTapiWrapper::GetRegisterStatus(DWORD* pdwRegisterStatus)
{
	if (pdwRegisterStatus != NULL)
	{
		return (lineGetRegisterStatus(m_hLine, pdwRegisterStatus) == ERROR_SUCCESS);
	}
	else
	{
		return false;
	}
}

////////////////////////////////////////////////////////////////////////////////

DWORD CTapiWrapper::StaticThreadProc(LPVOID lpParameter)
{
	return ((CTapiWrapper*)lpParameter)->ThreadProc();
}

DWORD CTapiWrapper::ThreadProc()
{
	LogMsg(_T("CTapiWrapper::ThreadProc: start"));

	LINEMESSAGE lineMsg;
	DWORD dwResult;

	for (;;)
	{
		dwResult = WaitForSingleObject(m_hTapiEvent, INFINITE);

		if (dwResult == WAIT_OBJECT_0)
		{
			if (lineGetMessage(m_hLineApp, &lineMsg, 0) == ERROR_SUCCESS)
			{
				switch(lineMsg.dwMessageID)
				{
				case LINE_REPLY:
					// Got reply to asynchonous request
					HandleLineReply(lineMsg);
					break;

				default:
					// TODO: handle these other kinds of TAPI messages
					LogMsg(_T("CTapiWrapper::ThreadProc: MsgId=%d, Param1=%d, Param2=%d, Param3=%d"), lineMsg.dwMessageID, lineMsg.dwParam1, lineMsg.dwParam2, lineMsg.dwParam3);
					break;
				}
			}
			else
			{
				LogMsg(_T("CTapiWrapper::ThreadProc: ending thread now (lineGetMessage() failed)"));
				break;
			}
		}
		else if (dwResult == WAIT_TIMEOUT)
		{
			LogMsg(_T("CTapiWrapper::ThreadProc: timeout (this should never happen with infinite timeout!)"));
		}
		else
		{
			LogMsg(_T("CTapiWrapper::ThreadProc: ending thread now (WaitForSingleObject() failed)"));
			break;
		}
	}

	LogMsg(_T("CTapiWrapper::ThreadProc: end"));

	return 0;
}

void CTapiWrapper::HandleLineReply(LINEMESSAGE& lineMsg)
{
	// !!! IMPORTANT NOTE !!!
	// This assumes we will only ever get a LINE_REPLY for our current outstanding request.
	// If this is not true, it is possible we will overwrite replies to outstanding requests.

	EnterCriticalSection(&m_csReplyRequest);

	LogMsg(_T("CTapiWrapper::ThreadProc: LINE_REPLY ReqID=%d, Result=%d"), lineMsg.dwParam1, lineMsg.dwParam2);

	m_nReplyReqId = lineMsg.dwParam1;
	m_dwReplyResult = lineMsg.dwParam2;

	if (m_nReplyReqId == m_nRequestId)
	{
		// Release WaitForReplyMsg() which should be waiting for this reply
		SetEvent(m_hReplyEvent);
	}

	LeaveCriticalSection(&m_csReplyRequest);
}

bool CTapiWrapper::WaitForReplyMsg(DWORD dwRequestId, LPDWORD lpdwResult)
{
	if ((dwRequestId <= 0) || (!lpdwResult))
	{
		return false; // bad parameters
	}

	bool bResult = false;

	// We are going to access member variables also used by background thread.
	EnterCriticalSection(&m_csReplyRequest);

	// Save this request ID so thread can check it against incoming reply msgs,
	m_nRequestId = dwRequestId;

	if (m_nRequestId == m_nReplyReqId)
	{
		// Already got reply to current request
		LogMsg(_T("CTapiWrapper::WaitForReplyMsg: already got reply for Request ID %d"), m_nRequestId);
		ResetEvent(m_hReplyEvent); // this shouldn't be set, but reset anyway
	}
	else
	{
		// Wait for reply to current request
		LogMsg(_T("CTapiWrapper::WaitForReplyMsg: waiting for reply to Request ID %d..."), m_nRequestId);
		LeaveCriticalSection(&m_csReplyRequest); // leave CS to avoid deadlock
		WaitForSingleObject(m_hReplyEvent, TAPI_REPLY_TIMEOUT);
		EnterCriticalSection(&m_csReplyRequest); // go back into CS after wait is done
	}

	// These two should match by now unless we timed out or there was an error.
	if (m_nRequestId == m_nReplyReqId)
	{
		LogMsg(_T("CTapiWrapper::WaitForReplyMsg: Request ID %d result=%d"), m_nRequestId, m_dwReplyResult);
		*lpdwResult = m_dwReplyResult;
		bResult = true;
	}

	// We are done with this current request, so clear the ID.
	m_nRequestId = INVALID_TAPI_REQUEST_ID;

	LeaveCriticalSection(&m_csReplyRequest);

	return bResult;
}

////////////////////////////////////////////////////////////////////////////////
// Taken from ExTAPI example in PPC2003 SDK
// Modified to exit loop early once desired device is found

// ***************************************************************************
// Function Name: GetTSPLineDeviceID
//
// Purpose: To get a TSP Line Device ID
//
// Arguments:
//  hLineApp = the HLINEAPP returned by lineInitializeEx
//  dwNumberDevices = also returned by lineInitializeEx
//  dwAPIVersionLow/High = min version of TAPI that we need
//  psTSPLineName = "Cellular Line"
//
// Return Values: Current Device ID
//
// Description:
//  This function returns the device ID of a named TAPI TSP.  The Device ID is 
//  used in the call to lineOpen

DWORD GetTSPLineDeviceID(const HLINEAPP hLineApp, 
                         const DWORD dwNumberDevices, 
                         const DWORD dwAPIVersionLow, 
                         const DWORD dwAPIVersionHigh, 
                         const TCHAR* const psTSPLineName)
{
    DWORD dwReturn = 0xffffffff;
    for(DWORD dwCurrentDevID = 0 ; (dwCurrentDevID < dwNumberDevices) && (dwReturn == 0xffffffff) ; dwCurrentDevID++)
    {
        DWORD dwAPIVersion;
        LINEEXTENSIONID LineExtensionID;
        if(0 == lineNegotiateAPIVersion(hLineApp, dwCurrentDevID, 
                                        dwAPIVersionLow, dwAPIVersionHigh, 
                                        &dwAPIVersion, &LineExtensionID)) 
        {
            LINEDEVCAPS LineDevCaps;
            LineDevCaps.dwTotalSize = sizeof(LineDevCaps);
            if(0 == lineGetDevCaps(hLineApp, dwCurrentDevID, 
                                   dwAPIVersion, 0, &LineDevCaps)) 
            {
                BYTE* pLineDevCapsBytes = new BYTE[LineDevCaps.dwNeededSize];
                if(0 != pLineDevCapsBytes) 
                {
                    LINEDEVCAPS* pLineDevCaps = (LINEDEVCAPS*)pLineDevCapsBytes;
                    pLineDevCaps->dwTotalSize = LineDevCaps.dwNeededSize;
                    if(0 == lineGetDevCaps(hLineApp, dwCurrentDevID, 
                                           dwAPIVersion, 0, pLineDevCaps)) 
                    {
						LogMsg(_T("GetTSPLineDeviceID: %d = %s"), dwCurrentDevID, (TCHAR*)((BYTE*)pLineDevCaps+pLineDevCaps->dwLineNameOffset) );

                        if(0 == _tcscmp((TCHAR*)((BYTE*)pLineDevCaps+pLineDevCaps->dwLineNameOffset), 
                                        psTSPLineName)) 
                        {
                            dwReturn = dwCurrentDevID;
							LogMsg(_T("GetTSPLineDeviceID: selected device ID %d"), dwReturn);
                        }
                    }
                    delete[]  pLineDevCapsBytes;
                }
            }
        }
    }
    return dwReturn;
}
