#include "pch.h"
#include "Joypad.h"
#include "Snes9xWrapper.h"
#include "JoypadIds.h"

namespace Snes9x { namespace Core
{
    CoreJoypad::CoreJoypad(unsigned int joypadNumber)
    {
        JoypadNumber = joypadNumber;
    }

#define IMPL_REPORT(s, n) void CoreJoypad::Report##s(bool pressed) { S9xWrapper::ReportButton((k_C1 << (JoypadNumber - 1)) + n, pressed); }
    IMPL_REPORT(A, 0)
    IMPL_REPORT(B, 1)
    IMPL_REPORT(X, 2)
    IMPL_REPORT(Y, 3)
    IMPL_REPORT(L, 4)
    IMPL_REPORT(R, 5)
    IMPL_REPORT(Start, 6)
    IMPL_REPORT(Select, 7)
    IMPL_REPORT(Up, 8)
    IMPL_REPORT(Down, 9)
    IMPL_REPORT(Left, 10)
    IMPL_REPORT(Right, 11)
} }
