#pragma once

namespace Snes9xWRC { namespace Core {

    void InitMemory();
    bool InitGraphics(uint16_t* screen, uint32_t pitch);
    bool InitApu();
    bool InitSound(int buffer_ms, int lag_ms);

    bool LoadRomMem(const byte* source, unsigned int sourceSize);

    void MainLoop();
} }