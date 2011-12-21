// Logging.cpp
//
// Logging functions
//

#include "StdAfx.h"
#include "Logging.h"

#define DEBUG_TIMESTAMP_LEN	9

static TCHAR m_szLogPrefix[MAX_LOG_PREFIX + 1] = {0};
static TCHAR m_szLogFile[MAX_LOG_FILE + 1] = {0};
static HWND  m_hwndLogWnd = NULL;
static UINT  m_uiLogMsg = 0;

static CRITICAL_SECTION csFileAccess = {0};

void InitLogging(LPCTSTR lpszLogPrefix, LPCTSTR lpszLogFile, HWND hwndLogWnd, UINT uiLogMsg)
{
	if (lpszLogPrefix && *lpszLogPrefix)
	{
		_tcsncpy(m_szLogPrefix, lpszLogPrefix, MAX_LOG_PREFIX);
	}

	if (lpszLogFile && *lpszLogFile)
	{
		_tcsncpy(m_szLogFile, lpszLogFile, MAX_LOG_FILE);
		InitializeCriticalSection(&csFileAccess);
	}

	if (hwndLogWnd)
	{
		m_hwndLogWnd = hwndLogWnd;
		m_uiLogMsg = uiLogMsg;
	}
}

void UninitLogging()
{
	if (m_szLogFile[0])
	{
		// We had a logging filename, so assume we created a critical section.
		DeleteCriticalSection(&csFileAccess);
	}

	memset(m_szLogPrefix, 0, (MAX_LOG_PREFIX + 1) * sizeof(TCHAR));
	memset(m_szLogFile, 0, (MAX_LOG_FILE + 1) * sizeof(TCHAR));
	m_hwndLogWnd = NULL;
	m_uiLogMsg = 0;
}

void LogMsg(LPCTSTR lpszFmt, ...)
{
	// Format of full message sent to debugger is:
	// [<log prefix>] HH:mm:ss <log msg>\r\n\0
	TCHAR szBuffer[1 + MAX_LOG_PREFIX + 2 + DEBUG_TIMESTAMP_LEN + MAX_LOG_MSG + 3];

	// Add prefix
	if (m_szLogPrefix[0])
	{
		_sntprintf(szBuffer, MAX_LOG_PREFIX + 4, _T("[%s] "), m_szLogPrefix);
	}
	else
	{
		_stprintf(szBuffer, _T("[-]"));
	}

	// Determine where other parts of log msg will start within szBuffer
	TCHAR* lpszTimeAndMsg = &szBuffer[_tcslen(szBuffer)];
	TCHAR* lpszMsgOnly = lpszTimeAndMsg + DEBUG_TIMESTAMP_LEN;

	// Add timestamp of form "HH:mm:ss " (9 chars)
	GetTimeFormat(LOCALE_SYSTEM_DEFAULT, 0, NULL, _T("HH':'mm':'ss' '"), lpszTimeAndMsg, DEBUG_TIMESTAMP_LEN+1);

	// Add message body
	va_list args;
	va_start(args, lpszFmt);
	_vsntprintf(lpszMsgOnly, MAX_LOG_MSG, lpszFmt, args);
	va_end(args);

	// Post message to window - it is up to window how it handles the message,
	// though it must release memory pointed to by lParam using LocalFree().
	if (m_hwndLogWnd && IsWindow(m_hwndLogWnd))
	{
		size_t nMsgLen = _tcslen(lpszTimeAndMsg);
		LPTSTR lpszBuf = (LPTSTR)LocalAlloc(LPTR, (nMsgLen + 1) * sizeof(TCHAR));
		if (lpszBuf)
		{
			_tcsncpy(lpszBuf, lpszTimeAndMsg, nMsgLen);
			PostMessage(m_hwndLogWnd, m_uiLogMsg, 0, (LPARAM)lpszBuf);
		}
	}

	// Add new line for debugger and file
	_tcscat(lpszMsgOnly, _T("\n"));

	// Log to debugger in all builds
	// This can also be captured via serial port when "retail debugging" is enabled on device
	OutputDebugString(szBuffer);

	// Log to file if required i.e. if first char in log file pathname is not NULL
	if (m_szLogFile[0])
	{
		EnterCriticalSection(&csFileAccess);

		FILE *fp = _tfopen(m_szLogFile, _T("a"));
		if (fp)
		{
			_fputts(lpszTimeAndMsg, fp);
			fclose(fp);
		}

		LeaveCriticalSection(&csFileAccess);
	}
}
