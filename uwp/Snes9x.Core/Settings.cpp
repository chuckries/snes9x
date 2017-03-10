#include "pch.h"
#include "Settings.h"

#include "LibSnes9x.h"

namespace Snes9x { namespace Core
{
    CoreSettings::CoreSettings()
    {
        memset(&Settings, 0, sizeof(Settings));
        Settings.MouseMaster = false;
        Settings.SuperScopeMaster = false;
        Settings.JustifierMaster = false;
        Settings.MultiPlayer5Master = false;
        Settings.FrameTimePAL = 20000;
        Settings.FrameTimeNTSC = 16667;
        Settings.SixteenBitSound = true;
        Settings.Stereo = true;
        Settings.SoundPlaybackRate = 32000;
        Settings.SoundInputRate = 32000;
        Settings.SupportHiRes = true;
        Settings.Transparency = true;
        Settings.AutoDisplayMessages = true;
        Settings.InitialInfoStringTimeout = 120;
        Settings.HDMATimingHack = 100;
        Settings.BlockInvalidVRAMAccessMaster = true;
        Settings.DisplayFrameRate = false;
    }

    bool CoreSettings::DisplayFrameRate::get()
    {
        return Settings.DisplayFrameRate;
    }

    void CoreSettings::DisplayFrameRate::set(bool value)
    {
        Settings.DisplayFrameRate = value;
    }
} }