#include "pch.h"
#include "snes9x.h"
#include "memmap.h"
#include "apu\apu.h"
#include "display.h"
#include "controls.h"
#include "movie.h"

// Implementations of snes9x's required functions

// snes9x.h
void S9xExit(void) { }
void S9xMessage(int, int, const char *) { }

// memmap.h
uint32  MemLoader(uint8 *, const char*, uint32) { return 0; }
void S9xAutoSaveSRAM(void) { }

// apu.h
bool8 S9xOpenSoundDevice() { return false; }

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
bool8 S9xOpenSnapshotFile(const char *, bool8, STREAM *) { return 0; }
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
bool8 S9xInitUpdate(void) { return 0; }
bool8 S9xDeinitUpdate(int, int) { return 0; }
bool8 S9xContinueUpdate(int, int) { return 0; }
void S9xSyncSpeed(void) { }