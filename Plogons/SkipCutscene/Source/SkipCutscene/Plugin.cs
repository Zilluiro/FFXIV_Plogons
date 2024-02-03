using Dalamud;
using Dalamud.Game;
using Dalamud.Game.Command;
using Dalamud.IoC;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using SkipCutscene.Utilities;
using System;

namespace SkipCutscene;

public sealed class Plugin : IDalamudPlugin
{
    public Configuration Configuration { get; init; }

    public Plugin(
        [RequiredVersion("1.0")] DalamudPluginInterface pluginInterface,
        [RequiredVersion("1.0")] ICommandManager commandManager,
        [RequiredVersion("1.0")] IChatGui chatGui,
        [RequiredVersion("1.0")] ISigScanner sigScanner)
    {
        _pluginInterface = ThrowIfArgument.IsNull(pluginInterface);
        _commandManager = ThrowIfArgument.IsNull(commandManager);
        _chatGui = ThrowIfArgument.IsNull(chatGui);
        sigScanner = ThrowIfArgument.IsNull(sigScanner);

        Configuration = _pluginInterface.GetPluginConfig() as Configuration ?? new Configuration();
        Configuration.Initialize(_pluginInterface);

        _address.Offset1 = sigScanner.ScanText("75 33 48 8B 0D ?? ?? ?? ?? BA ?? 00 00 00 48 83 C1 10 E8 ?? ?? ?? ?? 83 78");
        _address.Offset2 = sigScanner.ScanText("74 18 8B D7 48 8D 0D");

        if (_address.Offset1 != IntPtr.Zero && _address.Offset2 != IntPtr.Zero)
        {
            if (Configuration.IsEnabled)
            {
                ConfigurePlugin();
            }

            _commandManager.AddHandler(CommandName, new CommandInfo(OnCommand));
        }
        else
        {
            Dispose();
        }
    }

    public void Dispose()
    {
        _commandManager.RemoveHandler(CommandName);
    }

    private void OnCommand(string command, string args)
    {
        _chatGui.Print(Configuration.IsEnabled ? "Skip Cutscene: Disabled." : "Skip Cutscene: Enabled.");

        Configuration.SetStateTo(!Configuration.IsEnabled);

        ConfigurePlugin();
    }

    private void ConfigurePlugin()
    {
        if (_address.Offset1 == IntPtr.Zero || _address.Offset2 == IntPtr.Zero)
            return;

        if (Configuration.IsEnabled)
        {
            EnablePlugin();
        }
        else
        {
            DisablePlugin();
        }
    }

    private void EnablePlugin()
    {
        SafeMemory.Write<short>(_address.Offset1, -28528);
        SafeMemory.Write<short>(_address.Offset2, -28528);
    }

    private void DisablePlugin()
    {
        SafeMemory.Write<short>(_address.Offset1, 13173);
        SafeMemory.Write<short>(_address.Offset2, 6260);
    }

    public const string Name = "SkipCutscene.Updated_RV";
    private const string CommandName = "/scrv";

    private readonly (IntPtr Offset1, IntPtr Offset2) _address = (IntPtr.Zero, IntPtr.Zero);
    private readonly ICommandManager _commandManager;
    private readonly DalamudPluginInterface _pluginInterface;
    private readonly IChatGui _chatGui;
}
