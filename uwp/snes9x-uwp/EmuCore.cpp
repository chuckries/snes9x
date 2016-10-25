#include "pch.h"
#include "EmuCore.h"
#include "snes9x.h"
#include "memmap.h"
#include "apu\apu.h"
#include "snapshot.h"

EmuCore::EmuCore()
{
    Screen = ref new Array<byte>(2 * MAX_SNES_WIDTH * MAX_SNES_HEIGHT);
}

IAsyncOperation<bool>^ EmuCore::Init()
{
    InitSettings();
    Memory.Init();
    S9xInitAPU();

    GFX.Screen = (uint16*)Screen->begin();
    GFX.Pitch = 2 * MAX_SNES_WIDTH;
    S9xGraphicsInit();

    //S9xSetRenderPixelFormat(RGB565);

    S9xUnmapAllControls();
    S9xSetController(1, CTL_JOYPAD, 1, 0, 0, 0);

    S9xMapButton(0, S9xGetCommandT("Joypad1 X"), false);
    S9xMapButton(1, S9xGetCommandT("Joypad1 A"), false);
    S9xMapButton(2, S9xGetCommandT("Joypad1 B"), false);
    S9xMapButton(3, S9xGetCommandT("Joypad1 Y"), false);
    S9xMapButton(4, S9xGetCommandT("Joypad1 L"), false);
    S9xMapButton(5, S9xGetCommandT("Joypad1 R"), false);
    S9xMapButton(6, S9xGetCommandT("Joypad1 Select"), false);
    S9xMapButton(7, S9xGetCommandT("Joypad1 Start"), false);
    S9xMapButton(8, S9xGetCommandT("Joypad1 Up"), false);
    S9xMapButton(9, S9xGetCommandT("Joypad1 Down"), false);
    S9xMapButton(10, S9xGetCommandT("Joypad1 Left"), false);
    S9xMapButton(11, S9xGetCommandT("Joypad1 Right"), false);

    S9xMapButton(12, S9xGetCommandT("Joypad2 X"), false);
    S9xMapButton(13, S9xGetCommandT("Joypad2 A"), false);
    S9xMapButton(14, S9xGetCommandT("Joypad2 B"), false);
    S9xMapButton(15, S9xGetCommandT("Joypad2 Y"), false);
    S9xMapButton(16, S9xGetCommandT("Joypad2 L"), false);
    S9xMapButton(17, S9xGetCommandT("Joypad2 R"), false);
    S9xMapButton(18, S9xGetCommandT("Joypad2 Select"), false);
    S9xMapButton(19, S9xGetCommandT("Joypad2 Start"), false);
    S9xMapButton(20, S9xGetCommandT("Joypad2 Up"), false);
    S9xMapButton(21, S9xGetCommandT("Joypad2 Down"), false);
    S9xMapButton(22, S9xGetCommandT("Joypad2 Left"), false);
    S9xMapButton(23, S9xGetCommandT("Joypad2 Right"), false);

    S9xInitSound(100, 0);
    S9xSetSoundMute(FALSE);

    return LoadRom();
}

void EmuCore::InitSettings()
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
    Settings.UpAndDown = false;
}

IAsyncOperation<bool>^ EmuCore::LoadRom()
{
    FileOpenPicker^ picker = ref new FileOpenPicker();
    picker->ViewMode = PickerViewMode::List;
    picker->SuggestedStartLocation = PickerLocationId::Downloads;
    picker->FileTypeFilter->Append(".sfc");

    return create_async([this, picker]()
    {
        //auto t = create_task(picker->PickSingleFileAsync());

        auto t = create_task(StorageFile::GetFileFromApplicationUriAsync(ref new Uri("ms-appx:///smw.sfc")));

        return t.then([this](task<StorageFile^> t)
        {
            try
            {
                StorageFile^ file = t.get();
                if (file == nullptr)
                {
                    return task_from_result(false);
                }

                RomName = file->DisplayName;

                return create_task(FileIO::ReadBufferAsync(file))
                    .then([this](task<IBuffer^> t)
                {
                    try
                    {
                        IBuffer^ buffer = t.get();
                        DataReader^ dataReader = DataReader::FromBuffer(buffer);
                        Array<byte>^ romBytes = ref new Array<byte>(buffer->Length);
                        dataReader->ReadBytes(romBytes);
                        return (bool)Memory.LoadROMMem(romBytes->begin(), romBytes->Length);
                    }
                    catch (COMException^ e)
                    {
                        return false;
                    }
                });
            }
            catch (COMException^ e)
            {
                return task_from_result(false);
            }
        });
    });

    //return create_async([=]()
    //{
    //    auto t = create_task(picker->PickSingleFileAsync()).then([=](StorageFile^ file)
    //    {
    //        return file->OpenReadAsync();
    //    }).then([](IRandomAccessStream^ stream)
    //    {
    //        DataReader^ reader = ref new DataReader(stream);
    //        return create_task(reader->LoadAsync(stream->Size)).then([reader](unsigned int bytesLoaded) {
    //            Array<byte>^ fileBytes = ref new Array<byte>(bytesLoaded);
    //            reader->ReadBytes(fileBytes);

    //            Memory.LoadROMMem(fileBytes->begin(), fileBytes->Length);
    //        });
    //    });

    //    return t;
    //});

}

IAsyncOperation<bool>^ EmuCore::SaveState()
{
    return create_async([=]()
    {
        //ApplicationData::Current->LocalFolder;

        return (bool)S9xFreezeGame("test.smc");
    });
}

IAsyncOperation<bool>^ EmuCore::LoadState()
{
    return create_async([=]()
    {
        //ApplicationData::Current->LocalFolder;

        return (bool)S9xUnfreezeGame("test.smc");
    });
}