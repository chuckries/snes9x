#pragma once

#define __WIN32__
#include "snes9x.h"
#include "memmap.h"
#include "controls.h"
#include "apu\apu.h"
#include "gfx.h"
#include "snapshot.h"

static bool HasSram()
{
    if (Settings.SuperFX && Memory.ROMType < 0x15) // doesn't have SRAM
        return false;

    if (Settings.SA1 && Memory.ROMType == 0x34)    // doesn't have SRAM
        return false;

    return true;
}

static int GetSramByteCount()
{
    int size = Memory.SRAMSize ? (1 << (Memory.SRAMSize + 3)) * 128 : 0;
    if (size > 0x20000)
    {
        size = 0x20000;
    }
    return size;
}