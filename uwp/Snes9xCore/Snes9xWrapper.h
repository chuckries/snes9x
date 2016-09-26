#pragma once

namespace Snes9xCore
{
    static class S9xWrapper
    {
    public:
        static bool InitMemory();
        static bool InitGraphics(uint16_t* screen, uint32_t pitch);
        static bool InitApu();
        static bool InitSound(int buffer_ms, int lag_ms);

        static bool LoadRomMem(const byte* source, unsigned int sourceSize);

        static void MainLoop();

        static bool SaveState(const char* path);
        static bool LoadState(const char* path);
    };
}