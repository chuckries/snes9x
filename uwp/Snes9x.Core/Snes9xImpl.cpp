#include "pch.h"
#include "Engine.h"
#include "LibSnes9x.h"
#include "CXAudio2.h"
#include "JoypadIds.h"

// Implementations of snes9x's required functions

// snes9x.h
void S9xExit(void) { }
void S9xMessage(int, int, const char *) { }

// memmap.h
void S9xAutoSaveSRAM(void)
{
    Memory.SaveSRAM(CW2A(Snes9x::Core::Engine::Instance->GetSavePath()->Data()));
}

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

#define MAPBUTTONf(n, s) S9xMapButton(n, S9xGetCommandT(s), false)
void S9xSetupDefaultKeymap(void)
{
    MAPBUTTONf(kPad1A,      "Joypad1 A");
    MAPBUTTONf(kPad1B,      "Joypad1 B");
    MAPBUTTONf(kPad1X,      "Joypad1 X");
    MAPBUTTONf(kPad1Y,      "Joypad1 Y");
    MAPBUTTONf(kPad1L,      "Joypad1 L");
    MAPBUTTONf(kPad1R,      "Joypad1 R");
    MAPBUTTONf(kPad1Start,  "Joypad1 Start");
    MAPBUTTONf(kPad1Select, "Joypad1 Select");
    MAPBUTTONf(kPad1Up,     "Joypad1 Up");
    MAPBUTTONf(kPad1Down,   "Joypad1 Down");
    MAPBUTTONf(kPad1Left,   "Joypad1 Left");
    MAPBUTTONf(kPad1Right,  "Joypad1 Right");

    MAPBUTTONf(kPad2A,      "Joypad2 A");
    MAPBUTTONf(kPad2B,      "Joypad2 B");
    MAPBUTTONf(kPad2X,      "Joypad2 X");
    MAPBUTTONf(kPad2Y,      "Joypad2 Y");
    MAPBUTTONf(kPad2L,      "Joypad2 L");
    MAPBUTTONf(kPad2R,      "Joypad2 R");
    MAPBUTTONf(kPad2Start,  "Joypad2 Start");
    MAPBUTTONf(kPad2Select, "Joypad2 Select");
    MAPBUTTONf(kPad2Up,     "Joypad2 Up");
    MAPBUTTONf(kPad2Down,   "Joypad2 Down");
    MAPBUTTONf(kPad2Left,   "Joypad2 Left");
    MAPBUTTONf(kPad2Right,  "Joypad2 Right");

    MAPBUTTONf(kPad3A,      "Joypad3 A");
    MAPBUTTONf(kPad3B,      "Joypad3 B");
    MAPBUTTONf(kPad3X,      "Joypad3 X");
    MAPBUTTONf(kPad3Y,      "Joypad3 Y");
    MAPBUTTONf(kPad3L,      "Joypad3 L");
    MAPBUTTONf(kPad3R,      "Joypad3 R");
    MAPBUTTONf(kPad3Start,  "Joypad3 Start");
    MAPBUTTONf(kPad3Select, "Joypad3 Select");
    MAPBUTTONf(kPad3Up,     "Joypad3 Up");
    MAPBUTTONf(kPad3Down,   "Joypad3 Down");
    MAPBUTTONf(kPad3Left,   "Joypad3 Left");
    MAPBUTTONf(kPad3Right,  "Joypad3 Right");

    MAPBUTTONf(kPad4A,      "Joypad4 A");
    MAPBUTTONf(kPad4B,      "Joypad4 B");
    MAPBUTTONf(kPad4X,      "Joypad4 X");
    MAPBUTTONf(kPad4Y,      "Joypad4 Y");
    MAPBUTTONf(kPad4L,      "Joypad4 L");
    MAPBUTTONf(kPad4R,      "Joypad4 R");
    MAPBUTTONf(kPad4Start,  "Joypad4 Start");
    MAPBUTTONf(kPad4Select, "Joypad4 Select");
    MAPBUTTONf(kPad4Up,     "Joypad4 Up");
    MAPBUTTONf(kPad4Down,   "Joypad4 Down");
    MAPBUTTONf(kPad4Left,   "Joypad4 Left");
    MAPBUTTONf(kPad4Right,  "Joypad4 Right");

    MAPBUTTONf(kPad5A,      "Joypad5 A");
    MAPBUTTONf(kPad5B,      "Joypad5 B");
    MAPBUTTONf(kPad5X,      "Joypad5 X");
    MAPBUTTONf(kPad5Y,      "Joypad5 Y");
    MAPBUTTONf(kPad5L,      "Joypad5 L");
    MAPBUTTONf(kPad5R,      "Joypad5 R");
    MAPBUTTONf(kPad5Start,  "Joypad5 Start");
    MAPBUTTONf(kPad5Select, "Joypad5 Select");
    MAPBUTTONf(kPad5Up,     "Joypad5 Up");
    MAPBUTTONf(kPad5Down,   "Joypad5 Down");
    MAPBUTTONf(kPad5Left,   "Joypad5 Left");
    MAPBUTTONf(kPad5Right,  "Joypad5 Right");

    MAPBUTTONf(kPad6A,      "Joypad6 A");
    MAPBUTTONf(kPad6B,      "Joypad6 B");
    MAPBUTTONf(kPad6X,      "Joypad6 X");
    MAPBUTTONf(kPad6Y,      "Joypad6 Y");
    MAPBUTTONf(kPad6L,      "Joypad6 L");
    MAPBUTTONf(kPad6R,      "Joypad6 R");
    MAPBUTTONf(kPad6Start,  "Joypad6 Start");
    MAPBUTTONf(kPad6Select, "Joypad6 Select");
    MAPBUTTONf(kPad6Up,     "Joypad6 Up");
    MAPBUTTONf(kPad6Down,   "Joypad6 Down");
    MAPBUTTONf(kPad6Left,   "Joypad6 Left");
    MAPBUTTONf(kPad6Right,  "Joypad6 Right");

    MAPBUTTONf(kPad7A,      "Joypad7 A");
    MAPBUTTONf(kPad7B,      "Joypad7 B");
    MAPBUTTONf(kPad7X,      "Joypad7 X");
    MAPBUTTONf(kPad7Y,      "Joypad7 Y");
    MAPBUTTONf(kPad7L,      "Joypad7 L");
    MAPBUTTONf(kPad7R,      "Joypad7 R");
    MAPBUTTONf(kPad7Start,  "Joypad7 Start");
    MAPBUTTONf(kPad7Select, "Joypad7 Select");
    MAPBUTTONf(kPad7Up,     "Joypad7 Up");
    MAPBUTTONf(kPad7Down,   "Joypad7 Down");
    MAPBUTTONf(kPad7Left,   "Joypad7 Left");
    MAPBUTTONf(kPad7Right,  "Joypad7 Right");

    MAPBUTTONf(kPad8A,      "Joypad8 A");
    MAPBUTTONf(kPad8B,      "Joypad8 B");
    MAPBUTTONf(kPad8X,      "Joypad8 X");
    MAPBUTTONf(kPad8Y,      "Joypad8 Y");
    MAPBUTTONf(kPad8L,      "Joypad8 L");
    MAPBUTTONf(kPad8R,      "Joypad8 R");
    MAPBUTTONf(kPad8Start,  "Joypad8 Start");
    MAPBUTTONf(kPad8Select, "Joypad8 Select");
    MAPBUTTONf(kPad8Up,     "Joypad8 Up");
    MAPBUTTONf(kPad8Down,   "Joypad8 Down");
    MAPBUTTONf(kPad8Left,   "Joypad8 Left");
    MAPBUTTONf(kPad8Right,  "Joypad8 Right");
}

bool8 S9xMapInput(const char *name, s9xcommand_t *cmd) { return false; }

// movie.h
const char * S9xChooseMovieFilename(bool8) { return ""; }

// gfx.h
bool8 S9xInitUpdate(void) { return true; }

bool8 S9xDeinitUpdate(int width, int height)
{
    Snes9x::Core::Engine::Instance->SetResolution(width, height);
    return true;
}

bool8 S9xContinueUpdate(int, int) { return true; }
void S9xSyncSpeed(void) { }