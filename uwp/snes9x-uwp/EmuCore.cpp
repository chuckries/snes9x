#include "pch.h"
#include "EmuCore.h"
#include "snes9x.h"
#include "memmap.h"

EmuCore::EmuCore()
{
}

void EmuCore::Init()
{
    Memory.Init();
}
