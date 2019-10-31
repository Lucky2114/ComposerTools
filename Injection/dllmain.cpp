// dllmain.cpp : Definiert den Einstiegspunkt für die DLL-Anwendung.
#include "pch.h"
#include <tlhelp32.h>
#include <psapi.h>
#include <string>
#include <tchar.h>
#include <iostream>

using namespace std;

HMODULE GetRemoteModuleHandle(DWORD lpProcessId, LPCSTR lpModule);


template <typename I> std::string n2hexstr(I w, size_t hex_len = sizeof(I) << 1) {
	static const char* digits = "0123456789ABCDEF";
	std::string rc(hex_len, '0');
	for (size_t i = 0, j = (hex_len - 1) * 4; i < hex_len; ++i, j -= 4)
		rc[i] = digits[(w >> j) & 0x0f];
	return rc;
}

DWORD WINAPI ComposerThread(HMODULE hModule) 
{
	
	MessageBox(NULL, "Thread Start!", "Success", MB_OK);


	//HWND hWnd = FindWindow(0, "FL Studio 20");
	HWND hWnd = FindWindow("TFruityLoopsMainForm", 0);
	if (hWnd == 0) {
		MessageBox(0, "Error cannot find window.", "Error", MB_OK | MB_ICONERROR);
	}
	else {
		DWORD proccess_ID;
		GetWindowThreadProcessId(hWnd, &proccess_ID);

		HANDLE hProcess = OpenProcess(PROCESS_ALL_ACCESS, FALSE, proccess_ID);
		HMODULE hModule = GetRemoteModuleHandle(proccess_ID, "flengine_x64.dll");

		MODULEINFO mi;
		GetModuleInformation(hProcess, hModule, &mi, sizeof(mi));

		//Offset from flengine_x64.dll base addresss = 0x2498C0
		//alt 0x2498DC
		//new 0x1D9F90
		//1DA007
		int i = intptr_t(mi.lpBaseOfDll);

		MessageBox(0, n2hexstr(i).c_str(), "Base Address", MB_OK);

		int fnl = intptr_t(mi.lpBaseOfDll) + 0x1D9F90;

		MessageBox(0, n2hexstr(fnl).c_str(), "Final Address To Call", MB_OK);

		/*typedef int func(void);
		func* f = (func*)(fnl);
		int j = f();*/

		((void(*)(void))fnl)();
	}
	return 0;
}

BOOL APIENTRY DllMain(HMODULE hModule, DWORD  ul_reason_for_call, LPVOID lpReserved)
{
	switch (ul_reason_for_call)
	{
	case DLL_PROCESS_ATTACH:
	{
		//DisableThreadLibraryCalls(hModule);
		//// Get a handle to our file map
		//hMapFile = CreateFileMappingA(INVALID_HANDLE_VALUE, nullptr, PAGE_READWRITE, 0, SHMEMSIZE, SHMEMNAME);
		//if (hMapFile == nullptr) {

		//	DWORD tmp = GetLastError();
		//	TCHAR s[100];
		//	_stprintf_s(s, _T("%X"), tmp);
		//	MessageBoxA(nullptr, s, "DLL_PROCESS_ATTACH", MB_OK | MB_ICONERROR);
		//	return FALSE;
		//}

		//// Get our shared memory pointer
		//lpMemFile = MapViewOfFile(hMapFile, FILE_MAP_ALL_ACCESS, 0, 0, 0);
		//if (lpMemFile == nullptr) {
		//	MessageBoxA(nullptr, "Failed to map shared memory!", "DLL_PROCESS_ATTACH", MB_OK | MB_ICONERROR);
		//	return FALSE;
		//}

		//// Set shared memory to hold what our remote process needs
		//memset(lpMemFile, 0, SHMEMSIZE);
		//data.hModule = hModule;
		//data.lpInit = LPDWORD(GetProcAddress(hModule, DLL_REMOTEINIT_FUNCNAME));
		//data.dwOffset = DWORD(data.lpInit) - DWORD(data.hModule);
		//memcpy(lpMemFile, &data, sizeof(TSharedData));
		MessageBox(NULL, "Injection Successful!", "Success", MB_OK);


		CloseHandle(CreateThread(nullptr, 0, (LPTHREAD_START_ROUTINE)ComposerThread, hModule, 0, nullptr));
		break;
	}
	case DLL_THREAD_ATTACH:
	case DLL_THREAD_DETACH:
	case DLL_PROCESS_DETACH:
		break;
	}
	return TRUE;
}

HMODULE GetRemoteModuleHandle(DWORD lpProcessId, LPCSTR lpModule)
{
	HMODULE hResult = NULL;
	HANDLE hSnapshot;
	MODULEENTRY32 me32;

	hSnapshot = CreateToolhelp32Snapshot(TH32CS_SNAPMODULE, lpProcessId);
	if (hSnapshot != INVALID_HANDLE_VALUE)
	{
		me32.dwSize = sizeof(MODULEENTRY32);
		if (Module32First(hSnapshot, &me32))
		{
			do
			{
				if (!_stricmp((me32.szModule), lpModule))
				{
					hResult = me32.hModule;
					break;
				}
			} while (Module32Next(hSnapshot, &me32));
		}
		CloseHandle(hSnapshot);
	}
	return hResult;
}