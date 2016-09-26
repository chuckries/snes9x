#include "pch.h"
#include "Emulator.h"
#include "LibSnes9x.h"
#include "CXAudio2.h"

// Implementations of snes9x's required functions

// snes9x.h
void S9xExit(void) { }
void S9xMessage(int, int, const char *) { }

// memmap.h
void S9xAutoSaveSRAM(void) { }

// apu.h
bool8 S9xOpenSoundDevice()
{
    if (!CXAudio2::s_Audio.InitSoundOutput())
    {
        return false;
    }

    if (!CXAudio2::s_Audio.SetupSound())
    {
        return false;
    }

    S9xSetSamplesAvailableCallback([](void*)
    {
        CXAudio2::s_Audio.ProcessSound();
    }, nullptr);

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

bool8 S9xOpenSnapshotFile(const char * filePath, bool8 readOnly, STREAM * pStream)
{
    char openMode[] = "wb";
    if (readOnly)
    {
        openMode[0] = 'r';
    }

    *pStream = OPEN_STREAM(filePath, openMode);
    if (*pStream != nullptr)
    {
        return true;
    }

    return false;
}

void S9xCloseSnapshotFile(STREAM stream)
{
    CLOSE_STREAM(stream);
}

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
bool8 S9xInitUpdate(void) { return true; }

bool8 S9xDeinitUpdate(int width, int height)
{
    Snes9xCore::CoreEmulator::Instance->SetResolution(width, height);
    return true;
}

bool8 S9xContinueUpdate(int, int) { return true; }
void S9xSyncSpeed(void) { }