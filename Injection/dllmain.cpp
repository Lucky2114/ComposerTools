// dllmain.cpp : Definiert den Einstiegspunkt für die DLL-Anwendung.
#include "pch.h"
#include <tlhelp32.h>
#include <psapi.h>
#include <string>

using namespace std;

HMODULE GetRemoteModuleHandle(DWORD lpProcessId, LPCSTR lpModule);

template <typename I> std::string n2hexstr(I w, size_t hex_len = sizeof(I) << 1) {
	static const char* digits = "0123456789ABCDEF";
	std::string rc(hex_len, '0');
	for (size_t i = 0, j = (hex_len - 1) * 4; i < hex_len; ++i, j -= 4)
		rc[i] = digits[(w >> j) & 0x0f];
	return rc;
}

BOOL APIENTRY DllMain(HMODULE hModule,
	DWORD  ul_reason_for_call,
	LPVOID lpReserved
)
{
	switch (ul_reason_for_call)
	{
	case DLL_PROCESS_ATTACH:
	{
		MessageBox(NULL, "Hello World!", "Dll says:", MB_OK);



		HWND hWnd = FindWindow(0, "FL Studio 20");
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
			int i = intptr_t(mi.lpBaseOfDll);

			MessageBox(0, n2hexstr(i).c_str(), "Base Address", MB_OK);


			int fnl = intptr_t(mi.lpBaseOfDll) + 0x2498C0;

			MessageBox(0, n2hexstr(fnl).c_str(), "Final Address To Call", MB_OK);

			/*typedef int func(void);
			func* f = (func*)(fnl);
			int j = f();*/

			((void(*)(void))fnl)();

			break;
		}
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





