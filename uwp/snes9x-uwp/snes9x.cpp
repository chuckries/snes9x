#include "pch.h"

#include "App.xaml.h"
#include "EmuCore.h"
#include <fstream>

// Implementations of snes9x's required functions

// snes9x.h
void S9xExit(void) { }
void S9xMessage(int type, int number, const char * message) { S9xSetInfoString(message); }

// memmap.h
void S9xAutoSaveSRAM(void) { }

void S9xSoundCallback(void *data)
{
    ((snes9x_uwp::App^)Application::Current)->Core->S9XAudio2.ProcessSound();
}

// apu.h
bool8 S9xOpenSoundDevice()
{
    auto app = Application::Current;
    auto emu = ((snes9x_uwp::App^)Application::Current)->Core;
    emu->S9XAudio2.InitSoundOutput();
    emu->S9XAudio2.SetupSound();
    S9xSetSamplesAvailableCallback(S9xSoundCallback, nullptr);
    return true;
}

// port.h
void SetInfoDlgColor(unsigned char, unsigned char, unsigned char) { }

// display.h
void S9xPutImage(int, int) { }
void S9xInitDisplay(int, char **) { }
void S9xDeinitDisplay(void) { }
void S9xTextMode(void) { }
void S9xGraphicsMode(void) { }
void S9xSetPalette(void) { }
void S9xToggleSoundChannel(int) { }

bool8 S9xOpenSnapshotFile(const char* name, bool8 readonly, STREAM* stream)
{
    return create_task(ApplicationData::Current->LocalFolder->CreateFileAsync("test.smc", Windows::Storage::CreationCollisionOption::ReplaceExisting)).then([=](task<StorageFile^> t)
    {
        try
        {
            auto storageFile = t.get();

            FILE* file;
            errno_t err = _wfopen_s(&file, storageFile->Path->Data(), L"w+b");

            *stream = new fStream(file);

            return true;
        }
        catch (COMException^ e)
        {
            return false;
        }
    }).wait();
}

void S9xCloseSnapshotFile(STREAM) { }
const char * S9xStringInput(const char *) { return ""; }
const char * S9xGetDirectory(enum s9x_getdirtype) { return ""; }
const char * S9xGetFilename(const char *, enum s9x_getdirtype) { return ""; }
const char * S9xGetFilenameInc(const char *, enum s9x_getdirtype) { return ""; }
const char * S9xChooseFilename(bool8) { return ""; }
const char * S9xBasename(const char *) { return ""; }

// controls.h
bool S9xPollButton(uint32 id, bool *pressed) { return false; }
bool S9xPollPointer(uint32 id, int16 *x, int16 *y) { return false; }
bool S9xPollAxis(uint32 id, int16 *value) { return false; }
void S9xHandlePortCommand(s9xcommand_t cmd, int16 data1, int16 data2) { }
void S9xOnSNESPadRead(void) { }
s9xcommand_t S9xGetPortCommandT(const char *name) { return{ 0 }; }
char * S9xGetPortCommandName(s9xcommand_t command) { return ""; }
void S9xSetupDefaultKeymap(void) { }
bool8 S9xMapInput(const char *name, s9xcommand_t *cmd) { return false; }

// movie.h
const char * S9xChooseMovieFilename(bool8) { return ""; }

// gfx.h
bool8 S9xInitUpdate(void) { return TRUE; }
bool8 S9xDeinitUpdate(int width, int height) {
    auto core = ((snes9x_uwp::App^)Application::Current)->Core;
    core->Size = Size(width, height);
    core->Width = width;
    core->Height = height;
    return TRUE; }
bool8 S9xContinueUpdate(int, int) { return TRUE; }
void S9xSyncSpeed(void) { }