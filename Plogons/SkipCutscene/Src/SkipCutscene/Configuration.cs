using Dalamud.Configuration;
using Dalamud.Plugin;
using System;

namespace SkipCutscene;

[Serializable]
public class Configuration : IPluginConfiguration
{
    public int Version { get; set; } = 1;
    public bool IsEnabled { get; private set; } = true;

    [NonSerialized]
    private DalamudPluginInterface? PluginInterface;

    internal void Initialize(DalamudPluginInterface pluginInterface)
    {
        PluginInterface = pluginInterface;
    }

    public void SetStateTo(bool isEnabled)
    {
        IsEnabled = isEnabled;

        Save();
    }

    private void Save() => PluginInterface!.SavePluginConfig(this);
}
