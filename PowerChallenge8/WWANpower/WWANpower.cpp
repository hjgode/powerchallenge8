// WWANpower.cpp : Defines the entry point for the DLL application.
// based on code by Darren Beckley

#include "stdafx.h"

#define DELAY_AFTER_RAS_HANGUP	3000

////////////////////////////////////////////////////////////////////////////////
// Global variables

////////////////////////////////////////////////////////////////////////////////
// Forward declarations

void LogError(LPCTSTR lpszMsg);
bool ProcessCmdLine(int argc, TCHAR *argv[]);

static bool		g_bSwitchOn = false;
static bool		g_bQuiet = true;
static TCHAR	g_szLogFile[MAX_PATH+1] = L"\\WWANPower.log.txt";


extern "C" __declspec (dllexport) int __cdecl WWANpower(bool bPowerOn){
	int nExitCode = EXIT_FAILURE;

	g_bSwitchOn=bPowerOn;

	// Initalize logging with no log file to begin with
	InitLogging(_T("WWANRadioPower"), NULL, NULL, 0);
	LogMsg(_T("***** WWANRadioPower started *****"));

	// Re-initialize logging if using log file
	if (g_szLogFile[0])
	{
		UninitLogging();
		InitLogging(_T("WWANRadioPower"), g_szLogFile, NULL, 0);
		LogMsg(_T("***** WWANRadioPower started with file logging *****"));
	}
	LPCTSTR lpszNewState = (g_bSwitchOn ? _T("on") : _T("off"));
	LogMsg(_T("Requested WWAN radio state: %s"), lpszNewState);

	CTapiWrapper tapi(_T("WWANRadioPower"));
	if (tapi.Initialize())
	{
		bool bIsRadioOn;
		if (tapi.GetRadioPower(&bIsRadioOn))
		{
			if (bIsRadioOn != g_bSwitchOn)
			{
				if (!g_bSwitchOn)
				{
					// Drop any RAS connections before switching radio off
					if (HangUpRas(false, true) > 0)
					{
						// Wait a few seconds before continuing
						Sleep(DELAY_AFTER_RAS_HANGUP);
					}
				}
					
				LogMsg(_T("Switching WWAN radio %s"), lpszNewState);

				if (tapi.SetRadioPower(g_bSwitchOn))
				{
					LogMsg(_T("WWAN radio power switched %s"), lpszNewState);
					nExitCode = EXIT_SUCCESS;
				}
				else
				{
					TCHAR szErrMsg[MAX_LOG_MSG+1] = {0};
					_sntprintf(szErrMsg, MAX_LOG_MSG, _T("Failed to switch WWAN radio power %s"), lpszNewState);
					LogError(szErrMsg);
				}
			}
			else
			{
				LogMsg(_T("WWAN radio power is already %s"), lpszNewState);
				nExitCode = EXIT_SUCCESS;
			}
		}
		else
		{
			LogError(_T("Failed to get current WWAN radio power state"));
		}

		tapi.Uninitialize();
	}
	else
	{
		LogError(_T("Failed to initialize TAPI"));
	}

	LogMsg(_T("WWANRadioPower ended, exit code=%d"), nExitCode);
	UninitLogging();

	return nExitCode;

}

void LogError(LPCTSTR lpszMsg)
{
	LogMsg(_T("ERROR: %s"), lpszMsg);
}

// ############## MAIN #################
BOOL APIENTRY DllMain( HANDLE hModule, 
                       DWORD  ul_reason_for_call, 
                       LPVOID lpReserved
					 )
{
    return TRUE;
}

