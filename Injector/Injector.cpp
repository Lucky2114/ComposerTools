/*
Written by: SaEeD
Description: Injecting DLL to Target process using Process Id or Process name
*/
#include <iostream>
#include <string>
#include <ctype.h>
#include <Windows.h>
#include <tlhelp32.h>
#include <Shlwapi.h>
#include <tchar.h>
using namespace std;
//Library needed by Linker to check file existance
#pragma comment(lib, "Shlwapi.lib")

int getProcID(const string& p_name);
bool InjectDLL(const int& pid, const string& DLL_Path);
void usage();
void callRemoteFunction();

int main(int argc, char** argv)
{
	char* instr = argv[1];

	if (strcmp(instr, "remote") == 0)
	{
		callRemoteFunction();
	}

	if (argc != 3)
	{
		usage();
		return EXIT_FAILURE;
	}
	if (PathFileExists(argv[2]) == FALSE)
	{
		cerr << "[!]DLL file does NOT exist!" << endl;
		return EXIT_FAILURE;
	}

	if (isdigit(argv[1][0]))
	{
		cout << "[+]Input Process ID: " << atoi(argv[1]) << endl;
		InjectDLL(atoi(argv[1]), argv[2]);
	}
	else {
		InjectDLL(getProcID(argv[1]), argv[2]);
	}

	return EXIT_SUCCESS;
}

//-----------------------------------------------------------
// Get Process ID by its name
//-----------------------------------------------------------
int getProcID(const string& p_name)
{
	HANDLE snapshot = CreateToolhelp32Snapshot(TH32CS_SNAPPROCESS, 0);
	PROCESSENTRY32 structprocsnapshot = { 0 };

	structprocsnapshot.dwSize = sizeof(PROCESSENTRY32);

	if (snapshot == INVALID_HANDLE_VALUE)return 0;
	if (Process32First(snapshot, &structprocsnapshot) == FALSE)return 0;

	while (Process32Next(snapshot, &structprocsnapshot))
	{
		if (!strcmp(structprocsnapshot.szExeFile, p_name.c_str()))
		{
			CloseHandle(snapshot);
			cout << "[+]Process name is: " << p_name << "\n[+]Process ID: " << structprocsnapshot.th32ProcessID << endl;
			return structprocsnapshot.th32ProcessID;
		}
	}
	CloseHandle(snapshot);
	cerr << "[!]Unable to find Process ID" << endl;
	return 0;
}
//-----------------------------------------------------------
// Inject DLL to target process
//-----------------------------------------------------------
bool InjectDLL(const int& pid, const string& DLL_Path)
{
	long dll_size = DLL_Path.length() + 1;
	HANDLE hProc = OpenProcess(PROCESS_ALL_ACCESS, FALSE, pid);

	if (hProc == NULL)
	{
		cerr << "[!]Fail to open target process!" << endl;
		return false;
	}
	cout << "[+]Opening Target Process..." << endl;

	LPVOID MyAlloc = VirtualAllocEx(hProc, NULL, dll_size, MEM_COMMIT, PAGE_EXECUTE_READWRITE);
	if (MyAlloc == NULL)
	{
		cerr << "[!]Fail to allocate memory in Target Process." << endl;
		return false;
	}

	cout << "[+]Allocating memory in Targer Process." << endl;
	int IsWriteOK = WriteProcessMemory(hProc, MyAlloc, DLL_Path.c_str(), dll_size, 0);
	if (IsWriteOK == 0)
	{
		cerr << "[!]Fail to write in Target Process memory." << endl;
		return false;
	}
	cout << "[+]Creating Remote Thread in Target Process" << endl;

	DWORD dWord;
	LPTHREAD_START_ROUTINE addrLoadLibrary = (LPTHREAD_START_ROUTINE)GetProcAddress(LoadLibrary("kernel32"), "LoadLibraryA");
	HANDLE ThreadReturn = CreateRemoteThread(hProc, NULL, 0, addrLoadLibrary, MyAlloc, 0, &dWord);
	if (ThreadReturn == NULL)
	{
		cerr << "[!]Fail to create Remote Thread" << endl;
		return false;
	}

	if ((hProc != NULL) && (MyAlloc != NULL) && (IsWriteOK != ERROR_INVALID_HANDLE) && (ThreadReturn != NULL))
	{
		cout << "[+]DLL Successfully Injected :)" << endl;
		return true;
	}

	return false;
}
//-----------------------------------------------------------
// Usage help
//-----------------------------------------------------------
void usage()
{
	cout << "Usage: DLL_Injector.exe <Process name | Process ID> <DLL Path to Inject>" << endl;
}

// Data struct to be shared between processes
struct TSharedData
{
	DWORD dwOffset = 0;
	HMODULE hModule = nullptr;
	LPDWORD lpInit = nullptr;
};
// Name of the exported function you wish to call from the Launcher process
#define DLL_REMOTEINIT_FUNCNAME "RemoteInit"
// Size (in bytes) of data to be shared
#define SHMEMSIZE sizeof(TSharedData)
// Name of the shared file map (NOTE: Global namespaces must have the SeCreateGlobalPrivilege privilege)
#define SHMEMNAME "Global\\Injection.dll_SHMEM"
static HANDLE hMapFile;
static LPVOID lpMemFile;

void callRemoteFunction()
{
	cerr << "[+]Trying to call remote function in injected dll" << endl;
	HWND hWnd = FindWindow("TFruityLoopsMainForm", 0);
	if (hWnd == 0)
	{
		cerr << "[-]Error cannot find window." << endl;
	}
	else
	{
		// Get a handle to our file map
		hMapFile = CreateFileMappingA(INVALID_HANDLE_VALUE, nullptr, PAGE_READWRITE, 0, SHMEMSIZE, SHMEMNAME);
		if (hMapFile == nullptr) {

			DWORD tmp = GetLastError();
			cerr << "Failed to create file map" << endl;
			cerr << tmp << endl;
			//MessageBoxA(nullptr, s, "DLL_PROCESS_ATTACH", MB_OK | MB_ICONERROR);
			return;
		}

		// Get our shared memory pointer
		lpMemFile = MapViewOfFile(hMapFile, FILE_MAP_ALL_ACCESS, 0, 0, 0);
		if (lpMemFile == nullptr)
		{
			DWORD tmp = GetLastError();
			cerr << "Failed to map shared memory!" << endl;
			cerr << tmp << endl;
			return;
		}

		DWORD proccess_ID;
		GetWindowThreadProcessId(hWnd, &proccess_ID);
		HANDLE hProcess = OpenProcess(PROCESS_ALL_ACCESS, FALSE, proccess_ID);

		// Copy from shared memory
		TSharedData data;
		memcpy(&data, lpMemFile, SHMEMSIZE);
		// Clean up
		UnmapViewOfFile(lpMemFile);
		CloseHandle(hMapFile);
		// Call the remote function
		DWORD dwThreadId = 0;
		auto hThread = CreateRemoteThread(hProcess, nullptr, 0, LPTHREAD_START_ROUTINE(data.lpInit), nullptr, 0, &dwThreadId);
		cerr << "Thread Id" << endl;
		cerr << hThread << endl;
	}
}